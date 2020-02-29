using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DualWield.Harmony
{
    [HarmonyPatch(typeof(PawnComponentsUtility), "AddComponentsForSpawn")]
    class PawnComponentsUtility_AddComponentsForSpawn
    {
        static void Postfix(Pawn pawn)
        {
            if (!(pawn.GetStancesOffHand() is Pawn_StanceTracker stancesOffHand))
            {
                pawn.SetStancesOffHand(new Pawn_StanceTracker(pawn));
            }
        }
    }
}
