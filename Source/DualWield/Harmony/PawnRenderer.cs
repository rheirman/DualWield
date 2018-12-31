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
            Stance_Busy mainStance = pawn.stances.curStance as Stance_Busy;
            Stance_Busy offHandStance = null;
            if (pawn.GetStancesOffHand() != null)
            {
                offHandStance = pawn.GetStancesOffHand().curStance as Stance_Busy;
            }

            if (pawn.equipment == null)
            {
                return;
            }
            if (pawn.equipment.TryGetOffHandEquipment(out ThingWithComps result))
            {
                offHandEquip = result;
            }
            bool mainHandAiming = CurrentlyAiming(mainStance);
            bool offHandAiming = CurrentlyAiming(offHandStance);
            bool offHandIsMelee = IsMeleeWeapon(offHandEquip);
            bool mainHandIsMelee = IsMeleeWeapon(pawn.equipment.Primary);
            //bool currentlyAiming = (mainStance != null && !mainStance.neverAimWeapon && mainStance.focusTarg.IsValid) || stancesOffHand.curStance is Stance_Busy ohs && !ohs.neverAimWeapon && ohs.focusTarg.IsValid;
            //When wielding offhand weapon, facing south, and not aiming, draw differently 

            if(offHandEquip != null)
            {
                SetAnglesAndOffsets(aimAngle, pawn, ref xOffsetMain, ref yOffsetMain, ref xOffsetOffHand, ref yOffsetOffHand, ref zOffsetOffHand, ref mainHandAngle, ref offHandAngle, mainHandAiming, offHandAiming);
                if (!offHandAiming)
                {
                    if (pawn.Rotation == Rot4.South)
                    {
                        offHandAngle = offHandIsMelee ? 110 : 150;
                    }
                    if (pawn.Rotation == Rot4.North)
                    {
                        offHandAngle = offHandIsMelee ? 250 : 210;
                    }
                }
                if (!mainHandAiming)
                {
                    if (pawn.Rotation == Rot4.South)
                    {
                        mainHandAngle = mainHandIsMelee ? 250 : 210;
                    }
                    if (pawn.Rotation == Rot4.North)
                    {
                        mainHandAngle = mainHandIsMelee ? 110 : 150;
                    }
                }
            }
           

            if (offHandEquip != pawn.equipment.Primary)
            {
                instance.DrawEquipmentAiming(eq, drawLoc + new Vector3(xOffsetMain, yOffsetMain, zOffsetMain), mainHandAngle);
            }


            if (offHandEquip != null)
            {
                if (offHandAiming)
                {
                    offHandAngle = GetAimingRotation(pawn, offHandStance);
                    Vector3 adjustedDrawPos = pawn.DrawPos + new Vector3(0f, 0f, 0.4f).RotatedBy(offHandAngle) + new Vector3(0f, yOffsetOffHand + 0.1f, zOffsetOffHand);
                    instance.DrawEquipmentAiming(offHandEquip, adjustedDrawPos, offHandAngle);
                }
                else
                {
                    instance.DrawEquipmentAiming(offHandEquip, drawLoc + new Vector3(xOffsetOffHand, yOffsetOffHand, zOffsetOffHand), offHandAngle);
                }
            }
        }

        private static void SetAnglesAndOffsets(float aimAngle, Pawn pawn, ref float xOffsetMain, ref float yOffsetMain, ref float xOffsetOffHand, ref float yOffsetOffHand, ref float zOffsetOffHand, ref float mainHandAngle, ref float offHandAngle, bool mainHandAiming, bool offHandAiming)
        {
            if (pawn.Rotation == Rot4.East)
            {
                yOffsetOffHand = -1f;
                zOffsetOffHand = 0.1f;
            }
            else if (pawn.Rotation == Rot4.West)
            {
                yOffsetMain = -1f;
                //zOffsetMain = 0.25f;
                zOffsetOffHand = -0.1f;
            }
            else if (pawn.Rotation == Rot4.North)
            {
                if (!mainHandAiming)
                {
                    xOffsetMain = 0.5f;
                }
                if (!offHandAiming)
                {
                    offHandAngle = 360 - aimAngle;
                    xOffsetOffHand = -0.5f;
                }
            }
            else
            {
                if (!mainHandAiming)
                {
                    xOffsetMain = -0.5f;
                    mainHandAngle = 360 - aimAngle;
                }
                if (!offHandAiming)
                {
                    xOffsetOffHand = 0.5f;
                }
            }
        }

        private static float GetAimingRotation(Pawn pawn, Stance_Busy stance_Busy)
        {
            Vector3 a;
            if (stance_Busy.focusTarg.HasThing)
            {
                a = stance_Busy.focusTarg.Thing.DrawPos;
            }
            else
            {
                a = stance_Busy.focusTarg.Cell.ToVector3Shifted();
            }
            float num = 0f;
            if ((a - pawn.DrawPos).MagnitudeHorizontalSquared() > 0.001f)
            {
                num = (a - pawn.DrawPos).AngleFlat();
            }

            return num;
        }
        private static bool CurrentlyAiming(Stance_Busy stance)
        {
            return (stance != null && !stance.neverAimWeapon && stance.focusTarg.IsValid);
        }
        private static bool IsMeleeWeapon(ThingWithComps eq)
        {
            if(eq == null)
            {
                return false;
            }
            if(eq.TryGetComp<CompEquippable>() is CompEquippable ceq)
            {
                if (ceq.PrimaryVerb.IsMeleeAttack)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
