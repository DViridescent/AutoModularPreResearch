using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchiAlpha.AutoModular.PreResearch.Revit.Helpers
{
    internal static class LevelHelper
    {
        public static IList<Level> GetAllLevels(Document doc)
        {
            return new FilteredElementCollector(doc)
                 .OfCategory(BuiltInCategory.OST_Levels)
                 .OfClass(typeof(Level))
                 .ToElements()
                 .OfType<Level>()
                 .ToList();
        }
    }
}
