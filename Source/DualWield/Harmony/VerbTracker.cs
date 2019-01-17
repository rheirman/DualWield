using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Verse;

namespace DualWield.Harmony
{
    [HarmonyPatch]
    public class VerbTracker_GetVerbsCommands
    {
        static MethodBase TargetMethod()
        {
            var predicateClass = typeof(VerbTracker).GetNestedTypes(AccessTools.all)
                .FirstOrDefault(t => t.FullName.Contains("c__Iterator0"));
            return predicateClass.GetMethods(AccessTools.all).FirstOrDefault(m => m.Name.Contains("MoveNext"));
        }
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var instructionsList = new List<CodeInstruction>(instructions);
            foreach (CodeInstruction instruction in instructionsList)
            {
                if (instruction.operand == AccessTools.Method(typeof(VerbTracker), "CreateVerbTargetCommand")){
                    yield return new CodeInstruction(OpCodes.Call, typeof(VerbTracker_GetVerbsCommands).GetMethod("CreateVerbTargetCommandDW"));
                }
                else
                {
                    yield return instruction;
                }
            }
        }
        public static Command CreateVerbTargetCommandDW(VerbTracker instance, Thing ownerThing, Verb verb)
        {
            if(ownerThing is ThingWithComps twc && twc.ParentHolder is Pawn_EquipmentTracker peqt){
                CompEquippable ce = instance.directOwner as CompEquippable;
                if(peqt.pawn.equipment.TryGetOffHandEquipment(out ThingWithComps offHandEquip)){
                    if(offHandEquip != twc)
                    {
                        return CreateDualWieldCommand(ownerThing, offHandEquip, verb);
                    }
                }
            }
            return Traverse.Create(instance).Method("CreateVerbTargetCommand", new Object[]{ ownerThing, verb}).GetValue<Command_VerbTarget>();
        }
        private static Command_VerbTarget CreateDualWieldCommand(Thing ownerThing, Thing offHandThing, Verb verb)
        {
            Command_DualWield command_VerbTarget = new Command_DualWield(offHandThing);
            command_VerbTarget.defaultDesc = ownerThing.LabelCap + ": " + ownerThing.def.description.CapitalizeFirst();
            command_VerbTarget.icon = ownerThing.def.uiIcon;
            command_VerbTarget.iconAngle = ownerThing.def.uiIconAngle;
            command_VerbTarget.iconOffset = ownerThing.def.uiIconOffset;
            command_VerbTarget.tutorTag = "VerbTarget";
            command_VerbTarget.verb = verb;
            if (verb.caster.Faction != Faction.OfPlayer)
            {
                command_VerbTarget.Disable("CannotOrderNonControlled".Translate());
            }
            else if (verb.CasterIsPawn)
            {
                if (verb.CasterPawn.story.WorkTagIsDisabled(WorkTags.Violent))
                {
                    command_VerbTarget.Disable("IsIncapableOfViolence".Translate(verb.CasterPawn.LabelShort, verb.CasterPawn));
                }
                else if (!verb.CasterPawn.drafter.Drafted)
                {
                    command_VerbTarget.Disable("IsNotDrafted".Translate(verb.CasterPawn.LabelShort, verb.CasterPawn));
                }
            }
            return command_VerbTarget;
        }


    }
    [HarmonyPatch(typeof(VerbTracker), "GetVerbsCommands")]
    class VerbTracker_GetVerbsCommands_Postfix
    {
        static void Postfix(VerbTracker __instance, ref IEnumerable<Command> __result)
        {
            __result = RemoveCommandForOffHand(__result);
        }

        private static IEnumerable<Command> RemoveCommandForOffHand(IEnumerable<Command> __result)
        {
            foreach (Command command in __result)
            {
                if (command is Command_VerbTarget cVerbTarget)
                {
                    Verb verb = cVerbTarget.verb;

                    if (verb.EquipmentSource is ThingWithComps twc && twc.ParentHolder is Pawn_EquipmentTracker peqt)
                    {
                        if (peqt.pawn.equipment.TryGetOffHandEquipment(out ThingWithComps offHandEquip) && offHandEquip == twc)
                        {
                            continue;
                        }
                    }
                }
                yield return command;
            }
        }
    }
    
}
