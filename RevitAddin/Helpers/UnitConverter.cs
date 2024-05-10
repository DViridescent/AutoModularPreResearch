using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchiAlpha.AutoModular.PreResearch.Revit.Helpers
{
    internal static class UnitConverter
    {
        public static double MillimetersToFeet(this double millimetersValue) => UnitUtils.Convert(millimetersValue, DisplayUnitType.DUT_MILLIMETERS, DisplayUnitType.DUT_DECIMAL_FEET);

        public static double FeetToMillimeters(this double feetValue) => UnitUtils.Convert(feetValue, DisplayUnitType.DUT_DECIMAL_FEET, DisplayUnitType.DUT_MILLIMETERS);
    }
}
