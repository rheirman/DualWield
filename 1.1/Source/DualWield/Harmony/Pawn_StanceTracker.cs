using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DualWield.Harmony
{
    [HarmonyPatch(typeof(Pawn_StanceTracker), "get_FullBodyBusy")]
    class Pawn_StanceTracker_FullBodyBusy
    {
        static void Postfix(Pawn_StanceTracker __instance, ref bool __result)
        {
            if (__result)
            {
                return;
            }
            else
            {
                __result = FullBodyBusyOrOffHandCooldown(__instance);
            }
        }
        public static bool FullBodyBusyOrOffHandCooldown(Pawn_StanceTracker instance)
        {
            bool result = false;
            Pawn pawn = instance.pawn;
            if (pawn.GetStancesOffHand() is Pawn_StanceTracker stancesOffHand)
            {
                if (pawn.GetStancesOffHand() is Pawn_StanceTracker offHandStance && offHandStance.curStance is Stance_Cooldown && !RunAndGunEnabled(pawn))
                {
                    result = stancesOffHand.curStance.StanceBusy;
                }
            }
            return result;
        }
        private static bool RunAndGunEnabled(Pawn pawn)
        {
            bool runAndGunEnabled = false;
            if (pawn.AllComps.FirstOrDefault((ThingComp tc) => tc.GetType().Name == "CompRunAndGun") is ThingComp comp)
            {
                if (comp.GetDescriptionPart() != "")//Try to get value of isEnabled by abusing compInspectString. Only works with newer versions of RunAndGun. 
                {
                    runAndGunEnabled = Convert.ToBoolean(comp.GetDescriptionPart());
                }
                else //Otherwise use reflections, which is much more expensive in terms of execution time. 
                {
                    runAndGunEnabled = Traverse.Create(comp).Field("isEnabled").GetValue<bool>();
                }
            }
            return runAndGunEnabled;
        }
    }
}
