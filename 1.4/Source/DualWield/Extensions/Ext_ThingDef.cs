using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DualWield
{
    public static class Ext_ThingDef
    {
        public static bool CanBeOffHand(this ThingDef td)
        {
            return td.IsWeapon && Base.dualWieldSelection.Value != null && Base.dualWieldSelection.Value.inner.TryGetValue(td.defName, out Settings.Record value) && value.isSelected; 
        }
        public static bool IsTwoHand(this ThingDef td)
        {
            return td.IsWeapon && Base.twoHandSelection.Value != null && Base.twoHandSelection.Value.inner.TryGetValue(td.defName, out Settings.Record value) && value.isSelected;
        }
    }
}
