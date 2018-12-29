using DualWield.Storage;
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
        //Tag offhand equipment so it can be recognised as offhand equipment during later evaluations. 
        public static void AddOffHandEquipment(this Pawn_EquipmentTracker instance, ThingWithComps newEq)
        {
            ThingOwner<ThingWithComps> equipment = Traverse.Create(instance).Field("equipment").GetValue<ThingOwner<ThingWithComps>>();
            Base.Instance.GetExtendedDataStorage().GetExtendedDataFor(newEq).isOffHand = true;
            equipment.TryAdd(newEq, true);
        }
        //Only returns true when offhand weapon is used alongside a mainhand weapon. 
        public static bool TryGetOffHandEquipment(this Pawn_EquipmentTracker instance, out ThingWithComps result)
        {
            result = null;
            ExtendedDataStorage store = Base.Instance.GetExtendedDataStorage();
            foreach (ThingWithComps twc in instance.AllEquipmentListForReading)
            {
                if(store.TryGetExtendedDataFor(twc, out ExtendedThingWithCompsData ext) && ext.isOffHand){
                    result = twc;
                    if (twc != instance.Primary)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return false;
        }
    }
}
