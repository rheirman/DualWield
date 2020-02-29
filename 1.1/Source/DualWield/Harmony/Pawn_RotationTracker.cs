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
        static void Postfix(Pawn_RotationTracker __instance)
        {
            Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
            Stance_Busy stance_Busy = pawn.GetStancesOffHand().curStance as Stance_Busy;
            if (stance_Busy != null && stance_Busy.focusTarg.IsValid && !pawn.pather.Moving)
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
