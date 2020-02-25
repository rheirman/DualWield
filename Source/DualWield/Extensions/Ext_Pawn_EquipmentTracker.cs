using DualWield.Storage;
using HarmonyLib;
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
            ExtendedDataStorage store = Base.Instance.GetExtendedDataStorage();
            if(store != null)
            {
                store.GetExtendedDataFor(newEq).isOffHand = true;
                LessonAutoActivator.TeachOpportunity(DW_DefOff.DW_Penalties, OpportunityType.GoodToKnow);
                LessonAutoActivator.TeachOpportunity(DW_DefOff.DW_Settings, OpportunityType.GoodToKnow);
                equipment.TryAdd(newEq, true);
            }

        }
        //Only returns true when offhand weapon is used alongside a mainhand weapon. 
        public static bool TryGetOffHandEquipment(this Pawn_EquipmentTracker instance, out ThingWithComps result)
        {
            Log.Message("TryGetOffHandEquipment ");
            result = null;
            Log.Message("TryGetOffHandEquipment 1");
            if (instance.pawn.HasMissingArmOrHand())
            {
                Log.Message("TryGetOffHandEquipment 1 1");
                return false;
            }
            Log.Message("TryGetOffHandEquipment 2");
            ExtendedDataStorage store = Base.Instance.GetExtendedDataStorage();
            Log.Message("TryGetOffHandEquipment 3");
            foreach (ThingWithComps twc in instance.AllEquipmentListForReading)
            {
                Log.Message("TryGetOffHandEquipment 3 1");
                if (store.TryGetExtendedDataFor(twc, out ExtendedThingWithCompsData ext) && ext.isOffHand)
                {
                    Log.Message("TryGetOffHandEquipment 3 1 1");
                    result = twc;
                    return true;
                }
                Log.Message("TryGetOffHandEquipment 3 2");
            }
            Log.Message("TryGetOffHandEquipment 4");
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
