using DualWield.Stances;
using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Verse;

namespace DualWield.Harmony
{
    /**
     * Since a Run and Gun transpiler targets exactly the same line of code we want to target, we add this patch for compatbility. We basically postfix the method used by RunAndGun on the same line. We use "TargetMethod()" to target the to be patched method in RunAndGun without introducing a dependency. 
     **/
    [HarmonyPatch]
    public class RunAndGun
    {
        static MethodBase TargetMethod()
        {
            Assembly ass = GetAssemblyByName("RunAndGun");
            if(ass != null)
            {
                Type predicateClass = GetAssemblyByName("RunAndGun").GetTypes().FirstOrDefault((Type type) => type.Name == "Verb_TryCastNextBurstShot");
                if(predicateClass != null)
                {
                    MethodInfo minfo = predicateClass.GetMethods(AccessTools.all).FirstOrDefault(m => m.Name.Contains("SetStanceRunAndGun"));
                    if (minfo != null)
                    {
                        return minfo;
                    }
                }
            }
            return typeof(RunAndGun).GetMethod("Stub");
        }
        static void Postfix(Pawn_StanceTracker stanceTracker, Stance_Cooldown stance)
        {
            Log.Message("postfixed RunAndGun Verb_TryCastNextBurstShot");
            //Make sure this is called when run and gun patches the same line of code as we do in the harmony Patch Verb_TryCastNextBurstShot. 
            //SetStanceOffHand(stanceTracker, stance);
            Verb_TryCastNextBurstShot.SetStanceOffHand(stanceTracker, stance);
        }
        /*
        static void SetStanceOffHand(Pawn_StanceTracker stanceTracker, Stance_Cooldown stance)
        {
            ThingWithComps offHandEquip = null;
            CompEquippable compEquippable = null;
            Log.Message("calling RunAndGun.SetStanceOffHand");
            if (stanceTracker.pawn.equipment != null && stanceTracker.pawn.equipment.TryGetOffHandEquipment(out ThingWithComps result) && result != stanceTracker.pawn.equipment.Primary)
            {
                offHandEquip = result;
                compEquippable = offHandEquip.TryGetComp<CompEquippable>();
            }
            //Check if verb is one from a offhand weapon. 
            if (compEquippable != null && stance.verb == compEquippable.PrimaryVerb && stanceTracker.pawn.GetStancesOffHand().curStance is Stance_Warmup_DW)
            {
                Log.Message("found offhand verb!, setting offhand stance cooldown");
                Log.Message("Stance.type: " + stance.GetType().Name);
                Log.Message("Stance.ticks: " + stance.ticksLeft);
                stanceTracker.pawn.GetStancesOffHand().SetStance(stance);
            }
        }
        */


        //When Run and Gun or the to be patched method isn't found, patch this stub method so no error is thrown. 
        public static void Stub(Pawn_StanceTracker stanceTracker, Stance_Cooldown stance)
        {
            //Do nothing
        }
       
        static Assembly GetAssemblyByName(string name)
        {
            return AppDomain.CurrentDomain.GetAssemblies().
                   SingleOrDefault(assembly => assembly.GetName().Name == name);
        }


    }
}
