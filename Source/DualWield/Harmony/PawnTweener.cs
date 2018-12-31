using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DualWield.Harmony
{
    /*
    [HarmonyPatch(typeof(PawnTweener), "MovedPercent")]
    class PawnTweener_MovedPercent
    {
        static void Postfix(PawnTweener __instance, ref float __result)
        {
            Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
            if (pawn.GetStancesOffHand() is Pawn_StanceTracker stancesOffHand)
            {
                if (stancesOffHand.curStance.StanceBusy)
                {
                    __result = 0f;
                }
            }
        }
    }
    */
}
