using DualWield.Stances;
using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DualWield.Harmony
{
    [HarmonyPatch(typeof(Pawn_MeleeVerbs), "GetUpdatedAvailableVerbsList")]
    class Pawn_MeleeVerbs_GetUpdatedAvailableVerbsList
    {
        static void Postfix(ref List<VerbEntry> __result)
        {
            //remove all offhand verbs so they're not used by for mainhand melee attacks.
            List<VerbEntry> shouldRemove = new List<VerbEntry>();
            foreach (VerbEntry ve in __result)
            {
                if (ve.verb.EquipmentSource != null && ve.verb.EquipmentSource.IsOffHand())
                {
                    shouldRemove.Add(ve);
                }
            }
            foreach (VerbEntry ve in shouldRemove)
            {
                __result.Remove(ve);
            }
        }
    }

    [HarmonyPatch(typeof(Pawn_MeleeVerbs),"TryMeleeAttack")]
    class Pawn_MeleeVerbs_TryMeleeAttack
    {
        static void Postfix(Pawn_MeleeVerbs __instance, Thing target, Verb verbToUse, bool surpriseAttack, ref bool __result, ref Pawn ___pawn)
        {
            if (___pawn.GetStancesOffHand() == null || ___pawn.GetStancesOffHand().curStance is Stance_Warmup_DW || ___pawn.GetStancesOffHand().curStance is Stance_Cooldown)
            {
                return;
            }
            if (___pawn.equipment == null || !___pawn.equipment.TryGetOffHandEquipment(out ThingWithComps offHandEquip))
            {
                return;
            }
            if(offHandEquip == ___pawn.equipment.Primary)
            {
                return;
            }
            if (___pawn.InMentalState)
            {
                return;
            }

            Verb verb = __instance.Pawn.TryGetMeleeVerbOffHand(target);
            if(verb != null)
            {
                bool success = verb.OffhandTryStartCastOn(target);
                __result = __result || (verb != null && success);
            }
        }
    }
}
