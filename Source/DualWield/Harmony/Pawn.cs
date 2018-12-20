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
}
