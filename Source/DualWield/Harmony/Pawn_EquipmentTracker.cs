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
    //This patch prevent an error thrown when a offhand weapon is equipped and the primary weapon is switched. 
    [HarmonyPatch(typeof(Pawn_EquipmentTracker), "AddEquipment")]
    class Pawn_EquipmentTracker_AddEquipment
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var instructionsList = new List<CodeInstruction>(instructions);
            foreach (CodeInstruction instruction in instructionsList)
            {
                if (instruction.operand == typeof(Pawn_EquipmentTracker).GetMethod("get_Primary"))
                {
                    yield return new CodeInstruction(OpCodes.Call, typeof(Pawn_EquipmentTracker_AddEquipment).GetMethod("PrimaryNoOffHand"));
                }
                else
                {
                    yield return instruction;
                }
            }
        }
        //Make sure offhand weapons are never stored first in the list. 
        static void Postfix(Pawn_EquipmentTracker __instance, ThingWithComps newEq)
        {
            ExtendedDataStorage store = Base.Instance.GetExtendedDataStorage();
            ThingWithComps primary = __instance.Primary;
            if (store.TryGetExtendedDataFor(primary, out ExtendedThingWithCompsData twcData) && twcData.isOffHand)
            {
                ThingOwner<ThingWithComps> equipment = Traverse.Create(__instance).Field("equipment").GetValue<ThingOwner<ThingWithComps>>();
                equipment.Remove(primary);
                __instance.AddOffHandEquipment(primary);
            }
        }
        public static ThingWithComps PrimaryNoOffHand(Pawn_EquipmentTracker instance)
        {
            ThingWithComps result = null;
            //When there's no offhand weapon equipped, use vanilla behaviour and throw the error when needed. Otherwise, make sure the error is never thrown. 
            if (!instance.TryGetOffHandEquipment(out ThingWithComps r))
            {
                return instance.Primary;
            }
            return result;
        }
    }
    [HarmonyPatch(typeof(Pawn_EquipmentTracker) ,"MakeRoomFor")]
    class Pawn_EquipmentTracker_MakeRoomFor
    {
        static bool Prefix(Pawn_EquipmentTracker __instance)
        {
            if(__instance.TryGetOffHandEquipment(out ThingWithComps offHand) && offHand == __instance.Primary)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

}
