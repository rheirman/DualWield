using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Verse;
using Verse.AI;

namespace DualWield.Harmony
{
    [HarmonyPatch(typeof(Pawn_PathFollower), "PatherTick")]
    public class Pawn_PathFollower_PatherTick
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var instructionsList = new List<CodeInstruction>(instructions);
            foreach (CodeInstruction instruction in instructionsList)
            {
                if(instruction.operand == typeof(Pawn_StanceTracker).GetMethod("get_FullBodyBusy"))
                {
                    yield return new CodeInstruction(OpCodes.Call, typeof(Pawn_PathFollower_PatherTick).GetMethod("FullBodyBusyOrOffHandCooldown"));
                }
                else
                {
                    yield return instruction;
                }
            }
        }
        public static bool FullBodyBusyOrOffHandCooldown(Pawn_StanceTracker instance)
        {
            bool result = false;
            Pawn pawn = instance.pawn;
            if (pawn.stances.FullBodyBusy)
            {
                return true;
            }
            if (pawn.GetStancesOffHand() is Pawn_StanceTracker stancesOffHand)
            {
                bool runAndGunEnabled = false;
                if (pawn.AllComps.FirstOrDefault((ThingComp tc) => tc.GetType().Name == "CompRunAndGun") is ThingComp comp)
                {
                    runAndGunEnabled = Traverse.Create(comp).Field("isEnabled").GetValue<bool>();
                }
                if(pawn.GetStancesOffHand() is Pawn_StanceTracker offHandStance && offHandStance.curStance is Stance_Cooldown && !runAndGunEnabled)
                {
                    result = stancesOffHand.curStance.StanceBusy;
                }
            }
            return result;
        }
    }


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
                    bool runAndGunEnabled = false;
                    if (pawn.AllComps.FirstOrDefault((ThingComp tc) => tc.GetType().Name == "CompRunAndGun") is ThingComp comp)
                    {
                        runAndGunEnabled = Traverse.Create(comp).Field("isEnabled").GetValue<bool>();
                    }
                    if (!runAndGunEnabled)
                    {
                        __result = 0f;
                    }
                }
            }
        }
    }
    */

}
