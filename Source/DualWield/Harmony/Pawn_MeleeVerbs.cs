using DualWield.Stances;
using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DualWield.Harmony
{
    [HarmonyPatch(typeof(Pawn_MeleeVerbs),"TryMeleeAttack")]
    class Pawn_MeleeVerbs_TryMeleeAttack
    {
        static void Postfix(Pawn_MeleeVerbs __instance, Thing target, Verb verbToUse, bool surpriseAttack, ref bool __result)
        {
            if (__result)
            {
                Log.Message("normal TryMeleeAttack successful");
            }
            Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
            if (pawn.GetStancesOffHand() == null || pawn.GetStancesOffHand().curStance is Stance_Warmup_DW || pawn.GetStancesOffHand().curStance is Stance_Cooldown)
            {
                return;
            }

            Log.Message("pawn.GetStancesOffHand().curStance: " + pawn.GetStancesOffHand().curStance);
            Verb verb = __instance.Pawn.TryGetOffhandAttackVerb(target);
            if(verb != null)
            {
                Log.Message("trystartcaston called!");
                bool success = verb.OffhandTryStartCastOn(target);
                if (success)
                {
                    Log.Message("offhand TryMeleeAttack successful");
                }
                __result = __result || (verb != null && success);
            }
        }
    }
}
