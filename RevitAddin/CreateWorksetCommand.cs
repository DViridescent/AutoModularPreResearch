using ArchiAlpha.AutoModular.PreResearch.Revit.Helpers;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchiAlpha.AutoModular.PreResearch.Revit
{
    [Transaction(TransactionMode.Manual)]
    internal class CreateWorksetCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // 尝试开启文档的工作共享（添加"建筑“，”共享标高和轴网“）
            // 创建新工作集“结构”和“机电”
            // 在最高的Level中创建三面墙，分别放置在不同的工作集中
            Document doc = commandData.Application.ActiveUIDocument.Document;

            if (!doc.IsWorkshared)
            {
                if (doc.CanEnableWorksharing())
                {
                    // 开启工作共享
                    doc.EnableWorksharing("共享标高和轴网", "建筑");
                    RevitDialog.Show($"开启工作共享", "文档已开启工作共享，创建了新工作集[共享标高和轴网]和[建筑]");
                }
                else
                {
                    RevitDialog.Error("文档无法开启工作共享");
                    return Result.Failed;
                }
            }

            using Transaction tran = new Transaction(doc);
            tran.Start("创建工作集");

            try
            {
                var worksets = GetAllWorksets(doc);
                ShowWorksets(worksets);

                var newWorkset1 = CreateNewWorkset(doc, "结构");
                RevitDialog.Show("创建新工作集成功", $"新建工作集：{newWorkset1.Name}");
                var newWorkset2 = CreateNewWorkset(doc, "机电");
                RevitDialog.Show("创建新工作集成功", $"新建工作集：{newWorkset2.Name}");

                tran.Commit();
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                tran.RollBack();
                message = ex.Message;
                return Result.Failed;
            }
        }

        private IList<Workset> GetAllWorksets(Document doc)
        {
            return new FilteredWorksetCollector(doc).OfKind(WorksetKind.UserWorkset).ToWorksets().ToList();
        }

        private void ShowWorksets(IList<Workset> worksets)
        {
            var sb = new StringBuilder();
            sb.AppendLine("所有工作集：");
            foreach (var workset in worksets)
            {
                sb.AppendLine($"工作集：{workset.Name}");
            }
            RevitDialog.Show("当前工作集信息", sb.ToString());
        }

        private Workset CreateNewWorkset(Document doc, string worksetName)
        {
            var newWorkset = Workset.Create(doc, worksetName);
            return newWorkset;
        }
    }
}
