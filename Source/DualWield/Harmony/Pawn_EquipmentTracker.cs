using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Verse;

namespace DualWield.Harmony
{
    //This patch prevent an error thrown when a offhand weapon is equipped and the primary weapon is switched. 
    [HarmonyPatch(typeof(Pawn_EquipmentTracker), "AddEquipment")]
    class Pawn_EquipmentTracker_AddEquipment
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var instructionsList = new List<CodeInstruction>(instructions);
            foreach (CodeInstruction instruction in instructionsList)
            {
                if(instruction.operand == typeof(Pawn_EquipmentTracker).GetMethod("get_Primary"))
                {
                    yield return new CodeInstruction(OpCodes.Call, typeof(Pawn_EquipmentTracker_AddEquipment).GetMethod("PrimaryNoOffHand"));
                }
                else
                {
                    yield return instruction;
                }
            }
        }
        public static ThingWithComps PrimaryNoOffHand(Pawn_EquipmentTracker instance)
        {
            ThingWithComps result = null;
            //When there's no offhand weapon equipped, use vanilla behaviour and throw the error when needed. Otherwise, make sure the error is never thrown. 
            if(instance.TryGetOffHandEquipment(out ThingWithComps r) && r == null)
            {
                return instance.Primary;
            }
            return result;
        }
    }
}
