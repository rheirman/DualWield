using DualWield.Stances;
using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Verse;

namespace DualWield.Harmony
{
    [HarmonyPatch(typeof(Verb), "TryCastNextBurstShot")]
    public class Verb_TryCastNextBurstShot
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var instructionsList = new List<CodeInstruction>(instructions);
            foreach (CodeInstruction instruction in instructionsList)
            {
                if(instruction.operand == typeof(Pawn_StanceTracker).GetMethod("SetStance"))
                {
                    yield return new CodeInstruction(OpCodes.Call, typeof(Verb_TryCastNextBurstShot).GetMethod("SetStanceOffHand"));
                }
                else
                {
                    yield return instruction;
                }
            }
        }
        public static void SetStanceOffHand(Pawn_StanceTracker stanceTracker,  Stance_Cooldown stance)
        {
            ThingWithComps offHandEquip = null;
            CompEquippable compEquippable = null;

            if (stanceTracker.pawn.equipment != null && stanceTracker.pawn.equipment.TryGetOffHandEquipment(out ThingWithComps result))
            {
                offHandEquip = result;
                compEquippable = offHandEquip.TryGetComp<CompEquippable>();
            }
            //Check if verb is one from a offhand weapon. 
            if(compEquippable != null && stance.verb == compEquippable.PrimaryVerb)
            {
                Log.Message("found offhand verb!, setting offhand stance"); 
                stanceTracker.pawn.GetStancesOffHand().SetStance(stance);
            }
            else
            {
                stanceTracker.SetStance(stance);
            }
        }
    }
}
