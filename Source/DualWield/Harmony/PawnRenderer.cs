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
            float xOffset = 0;
            float yOffset = 0;
            float zOffset = 0;
            float offHandAngle = aimAngle;
            Stance_Busy stance_Busy = pawn.stances.curStance as Stance_Busy;
            if (pawn.equipment != null && pawn.equipment.AllEquipmentListForReading.Count > 1)
            {
                offHandEquip = pawn.equipment.AllEquipmentListForReading[1];//TODO: replace this temp code.
            }
            //When wielding offhand weapon, facing south, and not aiming, draw differently 
            if ((stance_Busy != null && !stance_Busy.neverAimWeapon && stance_Busy.focusTarg.IsValid) || (pawn.Rotation == Rot4.East) || offHandEquip == null)
            {
                instance.DrawEquipmentAiming(eq, drawLoc, aimAngle);
                yOffset = -1f;
                zOffset = 0.25f;
            }
            else if(pawn.Rotation == Rot4.West){
                instance.DrawEquipmentAiming(eq, drawLoc + new Vector3(0, -1f, 0.25f), aimAngle);
            }
            else if(pawn.Rotation == Rot4.North)
            {
                instance.DrawEquipmentAiming(eq, drawLoc + new Vector3(0.5f, 0, 0), aimAngle);
                offHandAngle = 360 - aimAngle;
                xOffset = -0.5f;
            }
            else
            {
                instance.DrawEquipmentAiming(eq, drawLoc + new Vector3(-0.5f, 0,0), 360 - aimAngle);
                xOffset = 0.5f;
            }


            if (offHandEquip != null)
            {
                instance.DrawEquipmentAiming(offHandEquip, drawLoc + new Vector3(xOffset, yOffset, zOffset), offHandAngle);
            }

        }
    }

}
