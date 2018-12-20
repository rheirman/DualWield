using DualWield.Stances;
using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DualWield.Harmony
{
    [HarmonyPatch(typeof(Pawn), "TryStartAttack")]
    class Pawn_TryStartAttack
    {
        static void Postfix(Pawn __instance, LocalTargetInfo targ, ref bool __result)
        {
            if (__result)
            {
                Log.Message("normal TryStartAttack successful");
            }
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
            bool success = verb.OffhandTryStartCastOn(targ);
            if (success)
            {
                Log.Message("offhand TryStartAttack successful");
            }
            __result = __result || (verb != null && success);
        }
    }
}
