using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;

namespace DualWield.Harmony
{
    [HarmonyPatch(typeof(FloatMenuMakerMap), "AddHumanlikeOrders")]
    class FloatMenuMakerMap_AddHumanlikeOrders
    {
        static void Postfix(Vector3 clickPos, Pawn pawn, ref List<FloatMenuOption> opts)
        {
            IntVec3 c = IntVec3.FromVector3(clickPos);

            foreach (LocalTargetInfo current in GenUI.TargetsAt(clickPos, TargetingParameters.ForSelf(pawn), true))
            {
                if (pawn.equipment.TryGetOffHandEquipment(out ThingWithComps eq))
                {
                    FloatMenuOption unequipOffHandOption = new FloatMenuOption("Unequip offhand weapon", new Action(delegate {
                        pawn.jobs.TryTakeOrderedJob(new Job(JobDefOf.DropEquipment, eq));
                    })); //TODO translation
                    opts.Add(unequipOffHandOption);
                }
            }

            if (pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
            {
                if (pawn.equipment != null)
                {
                    ThingWithComps equipment = null;
                    List<Thing> thingList = c.GetThingList(pawn.Map);
                    for (int i = 0; i < thingList.Count; i++)
                    {
                        if (thingList[i].TryGetComp<CompEquippable>() != null)
                        {
                            equipment = (ThingWithComps)thingList[i];
                            break;
                        }
                    }
                    if (equipment != null)
                    {
                        FloatMenuOption equipOffHandOption = GetEquipOffHandOption(pawn, equipment);
                        opts.Add(equipOffHandOption);
                    }
                }
            }
        }

        private static FloatMenuOption GetEquipOffHandOption(Pawn pawn, ThingWithComps equipment)
        {
            string labelShort = equipment.LabelShort;
            FloatMenuOption menuItem;

            if (equipment.def.IsWeapon && pawn.story.WorkTagIsDisabled(WorkTags.Violent))
            {
                menuItem = new FloatMenuOption("CannotEquip".Translate(labelShort) + " (" + "IsIncapableOfViolenceLower".Translate(pawn.LabelShort, pawn) + ")", null, MenuOptionPriority.Default, null, null, 0f, null, null);
            }
            else if (!pawn.CanReach(equipment, PathEndMode.ClosestTouch, Danger.Deadly, false, TraverseMode.ByPawn))
            {
                menuItem = new FloatMenuOption("CannotEquip".Translate(labelShort) + " (" + "NoPath".Translate() + ")", null, MenuOptionPriority.Default, null, null, 0f, null, null);
            }
            else if (!pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
            {
                menuItem = new FloatMenuOption("CannotEquip".Translate(labelShort) + " (" + "Incapable".Translate() + ")", null, MenuOptionPriority.Default, null, null, 0f, null, null);
            }
            else if (equipment.IsBurning())
            {
                menuItem = new FloatMenuOption("CannotEquip".Translate(labelShort) + " (" + "BurningLower".Translate() + ")", null, MenuOptionPriority.Default, null, null, 0f, null, null);
            }
            else if (pawn.HasMissingArmOrHand())
            {
                menuItem = new FloatMenuOption("CannotEquip".Translate(labelShort) + " (pawn needs two hands)", null, MenuOptionPriority.Default, null, null, 0f, null, null);//TODO: translation
            }

            //Add check for only 1 arm.
            else
            {
                string text5 = "Equip in off hand"; //TODO: translation
                if (equipment.def.IsRangedWeapon && pawn.story != null && pawn.story.traits.HasTrait(TraitDefOf.Brawler))
                {
                    text5 = text5 + " " + "EquipWarningBrawler".Translate();
                }
                menuItem = FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(text5, delegate
                {
                    equipment.SetForbidden(false, true);
                    pawn.jobs.TryTakeOrderedJob(new Job(DW_DefOff.DW_EquipOffhand, equipment), JobTag.Misc);
                    MoteMaker.MakeStaticMote(equipment.DrawPos, equipment.Map, ThingDefOf.Mote_FeedbackEquip, 1f);
                    PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.EquippingWeapons, KnowledgeAmount.Total);
                }, MenuOptionPriority.High, null, null, 0f, null, null), pawn, equipment, "ReservedBy");
            }

            return menuItem;
        }


    }
}
