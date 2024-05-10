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
    internal class CreateObjectsCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // 生成三面墙，高度为1000，厚度为200，
            // 放置在elevation最大的Level中
            // 分别放设置在“建筑”、“结构”和“机电”工作集，
            Document doc = commandData.Application.ActiveUIDocument.Document;

            // 创建墙的位置
            Line line1 = Line.CreateBound(new XYZ(0, 0, 0), new XYZ(2000d.MillimetersToFeet(), 0, 0));
            Line line2 = Line.CreateBound(new XYZ(2000d.MillimetersToFeet(), 0, 0), new XYZ(4000d.MillimetersToFeet(), 0, 0));
            Line line3 = Line.CreateBound(new XYZ(4000d.MillimetersToFeet(), 0, 0), new XYZ(6000d.MillimetersToFeet(), 0, 0));

            using Transaction trans = new Transaction(doc, "Create Walls");

            try
            {
                // 获取工作集
                WorksetId worksetId1 = GetWorkset(doc, "建筑").Id;
                WorksetId worksetId2 = GetWorkset(doc, "结构").Id;
                WorksetId worksetId3 = GetWorkset(doc, "机电").Id;

                // 获取最高的Level
                Level highestLevel = LevelHelper.GetAllLevels(doc).OrderByDescending(l => l.Elevation).First();

                // 创建墙
                trans.Start();

                Wall wall1 = Wall.Create(doc, line1, highestLevel.Id, false);
                wall1.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM).Set(1000d.MillimetersToFeet());
                wall1.get_Parameter(BuiltInParameter.ELEM_PARTITION_PARAM).Set(worksetId1.IntegerValue);

                Wall wall2 = Wall.Create(doc, line2, highestLevel.Id, false);
                wall2.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM).Set(1000d.MillimetersToFeet());
                wall2.get_Parameter(BuiltInParameter.ELEM_PARTITION_PARAM).Set(worksetId2.IntegerValue);

                Wall wall3 = Wall.Create(doc, line3, highestLevel.Id, false);
                wall3.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM).Set(1000d.MillimetersToFeet());
                wall3.get_Parameter(BuiltInParameter.ELEM_PARTITION_PARAM).Set(worksetId3.IntegerValue);

                trans.Commit();
            }
            catch
            {
                trans.RollBack();
                return Result.Failed;
            }

            return Result.Succeeded;
        }

        private Workset GetWorkset(Document doc, string name)
        {
            return new FilteredWorksetCollector(doc)
                .OfKind(WorksetKind.UserWorkset)
                .ToWorksets()
                .Where(workset => workset.Name == name)
                .First();
        }
    }
}
