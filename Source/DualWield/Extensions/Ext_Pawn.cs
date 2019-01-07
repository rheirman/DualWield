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
            if (equipment != null && equipment.TryGetOffHandEquipment(out ThingWithComps result) && result != equipment.Primary)
            {
                offHandEquip = result;//TODO: replace this temp code.
                compEquippable = offHandEquip.TryGetComp<CompEquippable>();
            }
            if (compEquippable != null && compEquippable.PrimaryVerb.Available() && (!compEquippable.PrimaryVerb.verbProps.onlyManualCast || (instance.CurJob != null && instance.CurJob.def != JobDefOf.Wait_Combat) || allowManualCastWeapons))
            {
                return compEquippable.PrimaryVerb;
            }
            else
            {
                return instance.TryGetMeleeVerbOffHand(target);
            }
        }
        public static bool HasMissingArmOrHand(this Pawn instance)
        {
            bool hasMissingHand = false;
            foreach (Hediff_MissingPart missingPart in instance.health.hediffSet.GetMissingPartsCommonAncestors())
            {
                if (missingPart.Part.def == BodyPartDefOf.Hand || missingPart.Part.def == BodyPartDefOf.Arm)
                {
                    hasMissingHand = true;
                }
            }
            return hasMissingHand;
        }
        public static Verb TryGetMeleeVerbOffHand(this Pawn instance, Thing target)
        {

            List<VerbEntry> usableVerbs = new List<VerbEntry>();
            if (instance.equipment != null && instance.equipment.TryGetOffHandEquipment(out ThingWithComps offHandEquip))
            {
                
                CompEquippable comp = offHandEquip.GetComp<CompEquippable>();
                //if(comp.AllVerbs.First((Verb verb) => verb.bu
                if (comp != null)
                {
                    List<Verb> allVerbs = comp.AllVerbs;
                    if (allVerbs != null)
                    {
                        for (int k = 0; k < allVerbs.Count; k++)
                        {
                            if (allVerbs[k].IsStillUsableBy(instance))
                            {
                                usableVerbs.Add(new VerbEntry(allVerbs[k], instance));
                            }
                        }
                    }
                }           
            }
            if(usableVerbs.TryRandomElementByWeight((VerbEntry ve) => ve.GetSelectionWeight(target), out VerbEntry result))
            {
                return result.verb;
            }
            return null;       
        }

    }
}
