using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DualWield.Harmony
{
    [HarmonyPatch(typeof(VerbProperties), "AdjustedCooldown")]
    [HarmonyPatch(new Type[]{typeof(Tool), typeof(Pawn), typeof(Thing)})]
    class VerbProperties_AdjustedCooldown
    {
        static void Postfix(VerbProperties __instance, Thing equipment, Pawn attacker, ref float __result)
        {
            if (attacker != null && attacker.skills != null)
            {
                SkillRecord skillRecord = __instance.IsMeleeAttack ? attacker.skills.GetSkill(SkillDefOf.Melee) : attacker.skills.GetSkill(SkillDefOf.Shooting);
                if(skillRecord == null)
                {
                    return;
                }
                if (equipment != null && equipment is ThingWithComps twc && twc.IsOffHand())
                {
                    Log.Message("2");
                    __result = CalcCooldownPenalty(__result, skillRecord, Base.staticCooldownPOffHand/100f);
                }
                else if (attacker.equipment != null && attacker.equipment.TryGetOffHandEquipment(out ThingWithComps offHandEq))
                {
                    Log.Message("3");
                    __result = CalcCooldownPenalty(__result, skillRecord, Base.staticCooldownPMainHand/100f);
                }
                Log.Message("4");
            }


        }

        private static float CalcCooldownPenalty(float __result, SkillRecord skillRecord, float staticPenalty)
        {
            //TODO: make mod settings
            float perLevelPenalty = Base.dynamicCooldownP/100f;
            int levelsShort = 20 - skillRecord.levelInt;
            float dynamicPenalty = perLevelPenalty * levelsShort;
            __result *= 1.0f + staticPenalty + dynamicPenalty;
            return __result;
        }
    }
    [HarmonyPatch(typeof(VerbProperties), "AdjustedAccuracy")]
    class VerbProperties_AdjustedAccuracy
    {
        static void Postfix(VerbProperties __instance, Thing equipment, ref float __result)
        {
            if (equipment.ParentHolder is Pawn_EquipmentTracker peqt && equipment != null)
            {
                Pawn pawn = peqt.pawn;
                if(pawn.skills == null)
                {
                    return;
                }
                SkillRecord skillRecord = __instance.IsMeleeAttack ? pawn.skills.GetSkill(SkillDefOf.Melee) : pawn.skills.GetSkill(SkillDefOf.Shooting);
                if (equipment is ThingWithComps twc && twc.IsOffHand())
                {
                    __result = CalcAccuracyPenalty(__result, skillRecord, Base.staticAccPOffHand/100f);
                }
                else if (pawn.equipment.TryGetOffHandEquipment(out ThingWithComps offHandEq))
                {
                    __result = CalcAccuracyPenalty(__result, skillRecord, Base.staticAccPMainHand/100f);
                }
            }
        }

        private static float CalcAccuracyPenalty(float __result, SkillRecord skillRecord, float staticPenalty)
        {
            //TODO: make mod settings
            float perLevelPenalty = Base.dynamicAccP/100f;
            int levelsShort = 20 - skillRecord.levelInt;
            float dynamicPenalty = perLevelPenalty * levelsShort;
            __result *= 1.0f - staticPenalty - dynamicPenalty;
            return __result;
        }
    }
}
