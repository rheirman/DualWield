using DualWield.Stances;
using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DualWield.Harmony
{
    //Tick the stance tracker of the offfhand weapon
    [HarmonyPatch(typeof(Pawn), "Tick")]
    class Pawn_Tick
    {
        static void Postfix(Pawn __instance)
        {
            if (!__instance.Suspended)
            {
                if (__instance.Spawned && __instance.GetStancesOffHand() is Pawn_StanceTracker stancesOffHand)
                {
                    stancesOffHand.StanceTrackerTick();
                }
            }
        }
    }
    //Also try start off hand weapons attack when trystartattack is called
    [HarmonyPatch(typeof(Pawn), "TryStartAttack")]
    class Pawn_TryStartAttack
    {
        static void Postfix(Pawn __instance, LocalTargetInfo targ, ref bool __result)
        {
            __instance.TryStartOffHandAttack(targ, ref __result);
        }
    }

    //If main weapon has shorter range than off hand weapon, use offhand weapon instead. 
    [HarmonyPatch(typeof(Pawn), "get_CurrentEffectiveVerb")]
    class Pawn_get_CurrentEffectiveVerb
    {
        static void Postfix(Pawn __instance, ref Verb __result)
        {
            if (__instance.MannedThing() == null &&
                __instance.equipment != null &&
                __instance.equipment.TryGetOffHandEquipment(out ThingWithComps offHandEquip) &&
                !offHandEquip.def.IsMeleeWeapon &&
                offHandEquip.TryGetComp<CompEquippable>() is CompEquippable compEquip)
            {
                Verb verb = compEquip.PrimaryVerb;
                if (__result.IsMeleeAttack || __result.verbProps.range < verb.verbProps.range)
                {
                    __result = verb;
                }
            }
        }
    }
}
