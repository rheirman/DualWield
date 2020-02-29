using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;
using Verse;

namespace DualWield.Harmony
{

    [HarmonyPatch(typeof(VerbTracker), "CreateVerbTargetCommand")]
    public class VerbTracker_CreateVerbTargetCommand
    {
        static void Postfix(VerbTracker __instance, Thing ownerThing, ref Verb verb, ref Command_VerbTarget __result) {
            if (ownerThing is ThingWithComps twc && twc.ParentHolder is Pawn_EquipmentTracker peqt)
            {
                CompEquippable ce = __instance.directOwner as CompEquippable;

                if (peqt.pawn.equipment.TryGetOffHandEquipment(out ThingWithComps offHandEquip))
                {
                    if (offHandEquip != twc)
                    {
                        __result = CreateDualWieldCommand(ownerThing, offHandEquip, verb);
                    }
                }
            }
        }
        private static Command_VerbTarget CreateVerbTargetCommand(VerbTracker __instance, Thing ownerThing, Verb verb)
        {

            Command_VerbTarget command_VerbTarget = new Command_VerbTarget();
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
                if (verb.CasterPawn.WorkTagIsDisabled(WorkTags.Violent))
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
                if (verb.CasterPawn.story.DisabledWorkTagsBackstoryAndTraits.HasFlag(WorkTags.Violent))
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
                        bool offhandIsPrimary = false;
                        //Remove offhand gizmo when dual wielding
                        //Don't remove offhand gizmo when offhand weapon is the only weapon being carried by the pawn
                        if (peqt.pawn.equipment.TryGetOffHandEquipment(out ThingWithComps offHandEquip) && offHandEquip == twc && offHandEquip != peqt.Primary)
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
