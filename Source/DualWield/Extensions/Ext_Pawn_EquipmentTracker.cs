using DualWield.Storage;
using Harmony;
using RimWorld;
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
                    return true;
                }
            }
            return false;
        }
        public static void MakeRoomForOffHand(this Pawn_EquipmentTracker instance, ThingWithComps eq)
        {
            instance.TryGetOffHandEquipment(out ThingWithComps currentOffHand);
            if (currentOffHand != null)
            {
                ThingWithComps thingWithComps;
                if (instance.TryDropEquipment(currentOffHand, out thingWithComps, instance.pawn.Position, true))
                {
                    if (thingWithComps != null)
                    {
                        thingWithComps.SetForbidden(false, true);
                    }
                }
                else
                {
                    Log.Error(instance.pawn + " couldn't make room for equipment " + eq, false);
                }
            }
        }
    }
}
