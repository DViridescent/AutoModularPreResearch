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
    internal class CreateViewCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // 创建新标高
            // 然后创建新楼层平面视图
            // 然后为视图设置视图样板
            Document doc = commandData.Application.ActiveUIDocument.Document;

            using Transaction tran = new Transaction(doc);
            tran.Start("创建标高");

            try
            {
                var levels = LevelHelper.GetAllLevels(doc);
                ShowLevels(levels);

                // 获取leves最大elevation
                double maxElevation = levels.Max(l => l.Elevation.FeetToMillimeters());
                var newLevel = CreateNewLevel(doc, maxElevation + 1000);
                RevitDialog.Show("创建新标高成功", $"新建标高：{newLevel.Name}，高程：{newLevel.Elevation}");

                var viewPlans = GetAllViewPlans(doc);
                ShowViewPlans(viewPlans);

                var newViewPlane = CreateViewPlan(doc, newLevel);
                newViewPlane.Name = $"新建楼层平面({DateTime.Now:mm分ss秒})";
                RevitDialog.Show("创建新楼层平面成功", $"新建楼层平面：{newViewPlane.Name}，标高：{newViewPlane.GenLevel?.Name}");

                var viewTemplates = GetAllViewTemplates(doc);
                ShowViewTemplates(viewTemplates);
                var selectedTemplate = viewTemplates.FirstOrDefault(template => template.Title == "平面_详图 1/5");
                selectedTemplate ??= viewTemplates.FirstOrDefault();

                newViewPlane.ViewTemplateId = selectedTemplate.Id;
                RevitDialog.Show("设置视图样板成功", $"新建楼层平面：{newViewPlane.Name}，视图样板：{selectedTemplate.Title}");

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


        private IList<ViewPlan> GetAllViewPlans(Document doc)
        {
            return new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Views)
                .OfClass(typeof(ViewPlan))
                .WhereElementIsNotElementType().OfType<ViewPlan>()
                .ToList();
        }
        private IList<View> GetAllViewTemplates(Document doc)
        {
            return new FilteredElementCollector(doc)
                .OfClass(typeof(View))
                .ToElements()
                .OfType<View>()
                .Where(v => v.IsTemplate)
                .ToList();
        }

        private void ShowLevels(IList<Level> levels)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"共有{levels.Count}个标高");
            foreach (var level in levels)
            {
                sb.AppendLine($"标高名称：{level.Name}，标高高程：{level.Elevation.FeetToMillimeters()}");
            }
            RevitDialog.Show("当前标高信息", sb.ToString());
        }
        private void ShowViewPlans(IList<ViewPlan> allFloorPlans)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"共有{allFloorPlans.Count}个楼层平面视图");
            foreach (var view in allFloorPlans)
            {
                sb.AppendLine($"视图标题：{view.Title}，对应标高：{view.GenLevel?.Name}");
            }
            RevitDialog.Show("当前楼层平面视图信息", sb.ToString());
        }
        private void ShowViewTemplates(IList<View> viewTemplates)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"共有{viewTemplates.Count}个视图样板");
            foreach (var template in viewTemplates)
            {
                if (!template.IsTemplate) continue;

                sb.AppendLine($"视图样板标题：{template.Title}");
            }
            RevitDialog.Show("当前视图样板信息", sb.ToString());
        }

        private Level CreateNewLevel(Document doc, double elevation = 1000)
        {
            return Level.Create(doc, elevation.MillimetersToFeet());
        }
        private ViewPlan CreateViewPlan(Document doc, Level level)
        {
            return ViewPlan.Create(doc, doc.GetDefaultElementTypeId(ElementTypeGroup.ViewTypeFloorPlan), level.Id);
        }
    }
}
