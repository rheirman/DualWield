using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DualWield.Harmony
{
    [HarmonyPatch(typeof(Pawn_RotationTracker), "UpdateRotation")]
    class Pawn_RotationTracker_UpdateRotation
    {
        static void Postfix(Pawn_RotationTracker __instance, ref Pawn ___pawn)
        {
            Stance_Busy stance_Busy = ___pawn.GetStancesOffHand()?.curStance as Stance_Busy;
            if (stance_Busy != null && stance_Busy.focusTarg.IsValid && !___pawn.pather.Moving)
            {
                if (stance_Busy.focusTarg.HasThing)
                {
                    __instance.Face(stance_Busy.focusTarg.Thing.DrawPos);
                }
                else
                {
                    __instance.FaceCell(stance_Busy.focusTarg.Cell);
                }
            }
        }
    }
}
