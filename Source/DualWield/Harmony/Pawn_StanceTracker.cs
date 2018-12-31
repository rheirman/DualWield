using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DualWield.Harmony
{
    /*
    [HarmonyPatch(typeof(Pawn_StanceTracker), "get_FullBodyBusy")]
    class Pawn_StanceTracker_get_FullBodyBusy
    {
        static void Postfix(Pawn_StanceTracker __instance, ref bool __result)
        {
            if(__instance.pawn.GetStancesOffHand() is Pawn_StanceTracker stancesOffHand)
            {
                if (stancesOffHand != __instance && stancesOffHand.curStance.StanceBusy)
                {
                    __result = true;
                }
            }
        }
    }
    */
}
