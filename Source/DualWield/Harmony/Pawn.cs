using DualWield.Stances;
using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DualWield.Harmony
{
    [HarmonyPatch(typeof(Pawn), "Tick")]
    class Pawn_Tick
    {
        static void Postfix(Pawn __instance)
        {
            if (!__instance.Suspended)
            {
                if (__instance.Spawned && __instance.GetStancesOffHand() is Pawn_StanceTracker stancesOffHand)
                {
                    stancesOffHand.StanceTrackerTick();
                }
            }
        }
    }
    [HarmonyPatch(typeof(Pawn), "TryStartAttack")]
    class Pawn_TryStartAttack
    {
        static void Postfix(Pawn __instance, LocalTargetInfo targ, ref bool __result)
        {
            if(__instance.GetStancesOffHand().curStance is Stance_Warmup_DW || __instance.GetStancesOffHand().curStance is Stance_Cooldown)
            {
                return; 
            }
            if (__instance.story != null && __instance.story.WorkTagIsDisabled(WorkTags.Violent))
            {
                return;
            }
            bool allowManualCastWeapons = !__instance.IsColonist;
            Verb verb = __instance.TryGetOffhandAttackVerb(targ.Thing, true);
            if(verb != null)
            {
                bool success = verb.OffhandTryStartCastOn(targ);
                __result = __result || (verb != null && success);
            }

        }
    }
}
