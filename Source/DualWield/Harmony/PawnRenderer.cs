using DualWield.Settings;
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
                if (instruction.operand == typeof(PawnRenderer).GetMethod("DrawEquipmentAiming"))
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

            float mainHandAngle = aimAngle;
            float offHandAngle = aimAngle;
            Stance_Busy mainStance = pawn.stances.curStance as Stance_Busy;
            Stance_Busy offHandStance = null;
            if (pawn.GetStancesOffHand() != null)
            {
                offHandStance = pawn.GetStancesOffHand().curStance as Stance_Busy;
            }
            LocalTargetInfo focusTarg = null;
            if (mainStance != null && !mainStance.neverAimWeapon)
            {
                focusTarg = mainStance.focusTarg;
            }
            else if (offHandStance != null && !offHandStance.neverAimWeapon)
            {
                focusTarg = offHandStance.focusTarg;
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

            Vector3 offsetMainHand = new Vector3();
            Vector3 offsetOffHand = new Vector3();
            //bool currentlyAiming = (mainStance != null && !mainStance.neverAimWeapon && mainStance.focusTarg.IsValid) || stancesOffHand.curStance is Stance_Busy ohs && !ohs.neverAimWeapon && ohs.focusTarg.IsValid;
            //When wielding offhand weapon, facing south, and not aiming, draw differently 

            if (offHandEquip != null)
            {
                SetAnglesAndOffsets(eq, offHandEquip, aimAngle, pawn, ref offsetMainHand, ref offsetOffHand, ref mainHandAngle, ref offHandAngle, mainHandAiming, offHandAiming);
            }

            if (offHandEquip != pawn.equipment.Primary)
            {
                instance.DrawEquipmentAiming(eq, drawLoc + offsetMainHand, mainHandAngle);
            }

            if (offHandEquip != null)
            {
                if ((offHandAiming || mainHandAiming) && focusTarg != null)
                {
                    offHandAngle = GetAimingRotation(pawn, focusTarg);
                    offsetOffHand.y += 0.1f;
                    Vector3 adjustedDrawPos = pawn.DrawPos + new Vector3(0f, 0f, 0.4f).RotatedBy(offHandAngle) + offsetOffHand;
                    instance.DrawEquipmentAiming(offHandEquip, adjustedDrawPos, offHandAngle);
                }
                else
                {
                    instance.DrawEquipmentAiming(offHandEquip, drawLoc + offsetOffHand, offHandAngle);
                }
            }

        }
        //Bloated spaghetti function :(. Maybe refactor this at some point. 
        private static void SetAnglesAndOffsets(Thing eq, ThingWithComps offHandEquip, float aimAngle, Pawn pawn, ref Vector3 offsetMainHand, ref Vector3 offsetOffHand, ref float mainHandAngle, ref float offHandAngle, bool mainHandAiming, bool offHandAiming)
        {
            bool offHandIsMelee = IsMeleeWeapon(offHandEquip);
            bool mainHandIsMelee = IsMeleeWeapon(pawn.equipment.Primary);
            if (pawn.Rotation == Rot4.East)
            {
                offsetOffHand.y = -1f;
                offsetOffHand.z = 0.1f;
            }
            else if (pawn.Rotation == Rot4.West)
            {
                offsetMainHand.y = -1f;
                //zOffsetMain = 0.25f;
                offsetOffHand.z = -0.1f;
            }
            else if (pawn.Rotation == Rot4.North)
            {
                if (!mainHandAiming && !offHandAiming)
                {
                    offsetMainHand.x = mainHandIsMelee ? Base.meleeXOffset : Base.rangedXOffset;
                    offsetOffHand.x = offHandIsMelee ? -Base.meleeXOffset : -Base.rangedXOffset;
                    offHandAngle = offHandIsMelee ? Base.meleeAngle : Base.rangedAngle;
                    mainHandAngle = mainHandIsMelee ? 360 - Base.meleeAngle : 360 - Base.rangedAngle;

                }
                else
                {
                    offsetOffHand.x = -0.1f;
                }
            }
            else
            {
                if (!mainHandAiming && !offHandAiming)
                {
                    offsetMainHand.x = mainHandIsMelee ? -Base.meleeXOffset : -Base.rangedXOffset;
                    offsetOffHand.x = offHandIsMelee ? Base.meleeXOffset : Base.rangedXOffset;
                    offHandAngle = offHandIsMelee ? 360 - Base.meleeAngle : 360 - Base.rangedAngle;
                    mainHandAngle = mainHandIsMelee ? Base.meleeAngle : Base.rangedAngle;
                }
                else
                {
                    offsetOffHand.x = 0.1f;
                }
            }
            if (!pawn.Rotation.IsHorizontal)
            {
                if (Base.customRotations.Value.inner.TryGetValue((offHandEquip.def.defName), out Record offHandValue))
                {
                    offHandAngle += pawn.Rotation == Rot4.North ? offHandValue.extraRotation : -offHandValue.extraRotation;
                    //offHandAngle %= 360;
                }                
                if (Base.customRotations.Value.inner.TryGetValue((eq.def.defName), out Record mainHandValue))
                {
                    mainHandAngle += pawn.Rotation == Rot4.North ? -mainHandValue.extraRotation : mainHandValue.extraRotation;
                    //mainHandAngle %= 360;
                }
            }
        }

        private static float GetAimingRotation(Pawn pawn, LocalTargetInfo focusTarg)
        {
            Vector3 a;
            if (focusTarg.HasThing)
            {
                a = focusTarg.Thing.DrawPos;
            }
            else
            {
                a =focusTarg.Cell.ToVector3Shifted();
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
