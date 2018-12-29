using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;
using Verse;

namespace DualWield.Harmony
{
    [HarmonyPatch(typeof(PawnRenderer), "RenderPawnAt")]
    [HarmonyPatch(new Type[]{typeof(Vector3), typeof(RotDrawMode), typeof(bool)})]
    class PawnRenderer_RenderPawnAt
    {
        static void Postfix(PawnRenderer __instance)
        {
            Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
            if (pawn.Spawned && !pawn.Dead)
            {
                pawn.GetStancesOffHand().StanceTrackerDraw();
            }
        }

    }
    [HarmonyPatch(typeof(PawnRenderer), "DrawEquipment")]
    public class PawnRenderer_DrawEquipment
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var instructionsList = new List<CodeInstruction>(instructions);
            foreach (CodeInstruction instruction in instructionsList)
            {
                if(instruction.operand == typeof(PawnRenderer).GetMethod("DrawEquipmentAiming"))
                {
                    yield return new CodeInstruction(OpCodes.Call, typeof(PawnRenderer_DrawEquipment).GetMethod("DrawEquipmentAimingModified"));
                }
                else
                {
                    yield return instruction;
                }
            }
        }
        public static void DrawEquipmentAimingModified(PawnRenderer instance, Thing eq, Vector3 drawLoc, float aimAngle)
        {
            ThingWithComps offHandEquip = null;
            Pawn pawn = Traverse.Create(instance).Field("pawn").GetValue<Pawn>();
            float xOffsetMain = 0;
            float yOffsetMain = 0;
            float zOffsetMain = 0;

            float xOffsetOffHand = 0;
            float yOffsetOffHand = 0;
            float zOffsetOffHand = 0;

            float mainHandAngle = aimAngle;
            float offHandAngle = aimAngle;
            Stance_Busy stance_Busy = pawn.stances.curStance as Stance_Busy;
            if(pawn.equipment == null)
            {
                return;
            }
            if (pawn.equipment.TryGetOffHandEquipment(out ThingWithComps result))
            {
                offHandEquip = result;
            }

            //When wielding offhand weapon, facing south, and not aiming, draw differently 
            if ((stance_Busy != null && !stance_Busy.neverAimWeapon && stance_Busy.focusTarg.IsValid) || (pawn.Rotation == Rot4.East) || offHandEquip == null)
            {
                yOffsetOffHand = -1f;
                zOffsetOffHand = 0.25f;
            }
            else if (pawn.Rotation == Rot4.West)
            {
                yOffsetMain = -1f;
                zOffsetMain = 0.25f;
            }
            else if (pawn.Rotation == Rot4.North)
            {
                xOffsetMain = 0.5f;
                offHandAngle = 360 - aimAngle;
                xOffsetOffHand = -0.5f;
            }
            else
            {
                xOffsetMain = -0.5f;
                mainHandAngle = 360 - aimAngle;
                xOffsetOffHand = 0.5f;
            }
            if(offHandEquip != pawn.equipment.Primary)
            {
                instance.DrawEquipmentAiming(eq, drawLoc + new Vector3(xOffsetMain, yOffsetMain, zOffsetMain) , mainHandAngle);
            }

            if (offHandEquip != null)
            {
                instance.DrawEquipmentAiming(offHandEquip, drawLoc + new Vector3(xOffsetOffHand, yOffsetOffHand, zOffsetOffHand), offHandAngle);
            }

        }
    }

}
