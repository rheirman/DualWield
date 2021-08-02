using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Verse;
using Verse.AI;

namespace DualWield.Harmony
{


    /*
    [HarmonyPatch(typeof(Pawn_PathFollower), "PatherTick")]
    public class Pawn_PathFollower_PatherTick
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var instructionsList = new List<CodeInstruction>(instructions);
            foreach (CodeInstruction instruction in instructionsList)
            {
                if(instruction.operand as MethodInfo == typeof(Pawn_StanceTracker).GetMethod("get_FullBodyBusy"))
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
                if (pawn.GetStancesOffHand() is Pawn_StanceTracker offHandStance && offHandStance.curStance is Stance_Cooldown && !RunAndGunEnabled(pawn))
                {
                    result = stancesOffHand.curStance.StanceBusy;
                }
            }
            return result;
        }

        private static bool RunAndGunEnabled(Pawn pawn)
        {
            bool runAndGunEnabled = false;
            if (pawn.AllComps.FirstOrDefault((ThingComp tc) => tc.GetType().Name == "CompRunAndGun") is ThingComp comp)
            {
                if(comp.CompInspectStringExtra() != "")//Try to get value of isEnabled by abusing compInspectString. Only works with newer versions of RunAndGun. 
                {
                    runAndGunEnabled = Convert.ToBoolean(comp.CompInspectStringExtra());
                }
                else //Otherwise use reflections, which is much more expensive in terms of execution time. 
                {
                    Log.Message("calling traverse :(");
                    //runAndGunEnabled = Traverse.Create(comp).Field("isEnabled").GetValue<bool>();
                }
            }
            return runAndGunEnabled;
        }
    }
    */

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
