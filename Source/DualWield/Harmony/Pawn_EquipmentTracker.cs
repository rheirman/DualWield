using DualWield.Storage;
using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DualWield.Harmony
{
    //Make sure "offhand" tag is removed when equipment is removed. 
    [HarmonyPatch(typeof(Pawn_EquipmentTracker), "Remove")]
    class Pawn_EquipmentTracker_Remove
    {
        static void Postfix(ThingWithComps eq)
        {
            ExtendedDataStorage store = Base.Instance.GetExtendedDataStorage();
            if(store.TryGetExtendedDataFor(eq,out ExtendedThingWithCompsData result)){
                store.DeleteExtendedDataFor(eq);
            }
        }
    }
}
