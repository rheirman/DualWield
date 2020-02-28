using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DualWield.Alerts
{
    /**
     * Disabled this: too annoying for players who know what they are doing. I might enable it again if too many non-readers get confused. 
     **/

    /*
    class Altert_SkillLow : Alert
    {
        public Altert_SkillLow()
        {
            this.defaultLabel = "DW_Alert_SkillTooLow_Label".Translate();
        }
        private IEnumerable<Pawn> SkillTooLowPawns
        {
            get
            {
                return from p in PawnsFinder.AllMaps_FreeColonists
                       where SkillTooLow(p)
                       select p;
            }
        }
        public override AlertReport GetReport()
        {
            return AlertReport.CulpritIs(this.SkillTooLowPawns.FirstOrDefault<Pawn>());
        }
        public override string GetExplanation()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();
            stringBuilder.AppendLine();
            foreach (Pawn current in this.SkillTooLowPawns)
            {
                stringBuilder.AppendLine("    " + current.Name);
            }
            stringBuilder.AppendLine();
            return string.Format("WTH_Alert_Maintenance_Low_Description".Translate(), stringBuilder.ToString());
        }

        private bool SkillTooLow(Pawn pawn)
        {
            if (pawn.equipment != null && pawn.equipment.TryGetOffHandEquipment(out ThingWithComps offHand) && pawn.skills != null)
            {
                if (pawn.equipment.Primary is ThingWithComps primary)
                {
                    if (SkillToLowForWeapon(pawn, primary))
                    {
                        return true;
                    }
                }
                if (SkillToLowForWeapon(pawn, offHand))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool SkillToLowForWeapon(Pawn pawn, ThingWithComps primary)
        {

            int level = 0;
            if (primary.def.IsMeleeWeapon)
            {
                level = pawn.skills.GetSkill(SkillDefOf.Melee).Level;
            }
            else
            {
                level = pawn.skills.GetSkill(SkillDefOf.Shooting).Level;
            }
            int levelsShort = 20 - level;
            if (levelsShort * Base.dynamicCooldownP > 75f)
            {
                return true;
            }
            return false;
        }
    }
    */
}
