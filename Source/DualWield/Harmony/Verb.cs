using DualWield.Stances;
using DualWield.Storage;
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
            if(stance.verb.EquipmentSource == null)
            {
                Log.Message("EquipmentSource is null");
            }
            if (stance.verb.EquipmentSource != null && Base.Instance.GetExtendedDataStorage().TryGetExtendedDataFor(stance.verb.EquipmentSource, out ExtendedThingWithCompsData twcdata) && twcdata.isOffHand)
            {
                offHandEquip = stance.verb.EquipmentSource;
                compEquippable = offHandEquip.TryGetComp<CompEquippable>();
            }
            //Check if verb is one from a offhand weapon. 
            if(compEquippable != null && stance.verb == compEquippable.PrimaryVerb && offHandEquip != stanceTracker.pawn.equipment.Primary) //TODO: check this code 
            {
                stanceTracker.pawn.GetStancesOffHand().SetStance(stance);
            }
            else if (!(stanceTracker.curStance is Stance_Cooldown))
            {
                stanceTracker.SetStance(stance);
            }
        }
    }
}
