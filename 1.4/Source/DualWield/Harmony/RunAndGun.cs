using DualWield.Stances;
using HarmonyLib;
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
        static IEnumerable<MethodBase> TargetMethods()
        {
            Assembly ass = GetAssemblyByName("RunAndGun");
            if(ass != null)
            {
                Type predicateClass = ass.GetTypes().FirstOrDefault((Type type) => type.Name == "Verb_TryCastNextBurstShot");
                if(predicateClass != null)
                {
                    MethodInfo minfo = predicateClass.GetMethods(AccessTools.all).FirstOrDefault(m => m.Name.Contains("SetStanceRunAndGun"));
                    if (minfo != null)
                    {
                        yield return minfo;
                    }
                }
            }
        }
        static void Prepare(MethodBase original)
        {
            if (original != null)
            {
                Log.Message("patching run and gun");
            }
        }
        static void Postfix(Pawn_StanceTracker stanceTracker, Stance_Cooldown stance)
        {
            //Make sure this is called when run and gun patches the same line of code as we do in the harmony Patch Verb_TryCastNextBurstShot. 
            //SetStanceOffHand(stanceTracker, stance);
            Verb_TryCastNextBurstShot.SetStanceOffHand(stanceTracker, stance);
        }

        static Assembly GetAssemblyByName(string name)
        {
            return AppDomain.CurrentDomain.GetAssemblies().
                   SingleOrDefault(assembly => assembly.GetName().Name == name);
        }


    }
}
