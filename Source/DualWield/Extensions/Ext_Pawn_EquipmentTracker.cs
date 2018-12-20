using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DualWield
{
    static class Ext_Pawn_EquipmentTracker
    {
        public static void AddEquipmentForOffhand(this Pawn_EquipmentTracker instance, ThingWithComps newEq)
        {
            ThingOwner<ThingWithComps> equipment = Traverse.Create(instance).Field("equipment").GetValue<ThingOwner<ThingWithComps>>();
            equipment.TryAdd(newEq, true);
        }
    }
}
