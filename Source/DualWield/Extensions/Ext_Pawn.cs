using DualWield.Storage;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DualWield
{
    public static class Ext_Pawn
    {
        public static Pawn_StanceTracker GetStancesOffHand(this Pawn instance)
        {
            if(Base.Instance.GetExtendedDataStorage() is ExtendedDataStorage store)
            {
                return store.GetExtendedDataFor(instance).stancesOffhand;
            }
            return null;
        }
        public static void SetStancesOffHand(this Pawn instance, Pawn_StanceTracker stancesOffHand)
        {
            if (Base.Instance.GetExtendedDataStorage() is ExtendedDataStorage store)
            {
                store.GetExtendedDataFor(instance).stancesOffhand = stancesOffHand;
            }
        }
        public static Verb TryGetOffhandAttackVerb(this Pawn instance, Thing target, bool allowManualCastWeapons = false)
        {
            Pawn_EquipmentTracker equipment = instance.equipment;
            ThingWithComps offHandEquip = null;
            CompEquippable compEquippable = null;
            if (equipment != null && equipment.AllEquipmentListForReading.Count > 1)
            {
                offHandEquip = equipment.AllEquipmentListForReading[1];//TODO: replace this temp code.
                compEquippable = offHandEquip.TryGetComp<CompEquippable>();
            }

            if (compEquippable != null && compEquippable.PrimaryVerb.Available() && (!compEquippable.PrimaryVerb.verbProps.onlyManualCast || (instance.CurJob != null && instance.CurJob.def != JobDefOf.Wait_Combat) || allowManualCastWeapons))
            {
                return compEquippable.PrimaryVerb;
            }
            return instance.meleeVerbs.TryGetMeleeVerb(target);
        }

    }
}
