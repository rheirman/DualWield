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
        static void Postfix(Thing equipment, Pawn attacker, ref float __result)
        {
            
            SkillRecord skillRecord = attacker.skills.GetSkill(SkillDefOf.Shooting);

            if (equipment != null && equipment is ThingWithComps twc && twc.IsOffHand())
            {
                float staticPenalty = 0.1f;
                __result = CalcCooldownPenalty(__result, skillRecord, staticPenalty);
            }
            else if (attacker.equipment != null && attacker.equipment.TryGetOffHandEquipment(out ThingWithComps offHandEq))
            {
                float staticPenalty = 0.2f;
                __result = CalcCooldownPenalty(__result, skillRecord, staticPenalty);
            }
        }

        private static float CalcCooldownPenalty(float __result, SkillRecord skillRecord, float staticPenalty)
        {
            //TODO: make mod settings
            float perLevelPenalty = 0.05f;
            int levelsShort = 20 - skillRecord.levelInt;
            float dynamicPenalty = perLevelPenalty * levelsShort;
            __result *= 1.0f + staticPenalty + dynamicPenalty;
            return __result;
        }
    }
    [HarmonyPatch(typeof(VerbProperties), "AdjustedAccuracy")]
    class VerbProperties_AdjustedAccuracy
    {
        static void Postfix(Thing equipment, ref float __result)
        {
            if(equipment.ParentHolder is Pawn_EquipmentTracker pet)
            {
                Pawn pawn = pet.pawn;
                SkillRecord skillRecord = pawn.skills.GetSkill(SkillDefOf.Shooting);

                if (equipment != null && equipment is ThingWithComps twc && twc.IsOffHand())
                {
                    __result = CalcAccuracyPenalty(__result, skillRecord);
                    //Todo: replace magic numbers. 
                }
                else if (pawn.equipment != null && pawn.equipment.TryGetOffHandEquipment(out ThingWithComps offHandEq))
                {
                    __result = CalcAccuracyPenalty(__result, skillRecord);
                    //Todo: replace magic numbers. 
                }
            }
        }

        private static float CalcAccuracyPenalty(float __result, SkillRecord skillRecord)
        {
            //TODO: make mod settings
            float staticPenalty = 0.1f;
            float perLevelPenalty = 0.005f;
            int levelsShort = 20 - skillRecord.levelInt;
            float dynamicPenalty = perLevelPenalty * levelsShort;
            __result *= 1.0f - staticPenalty - dynamicPenalty;
            return __result;
        }
    }
}
