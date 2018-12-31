using DualWield.Stances;
using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DualWield
{
    public static class Ext_Verb
    {
        public static bool OffhandTryStartCastOn(this Verb instance, LocalTargetInfo castTarg)
        {
            if (instance.caster == null)
            {
                Log.Error("Verb " + instance.GetUniqueLoadID() + " needs caster to work (possibly lost during saving/loading).", false);
                return false;
            }
            if (!instance.caster.Spawned)
            {
                return false;
            }
            if (instance.state == VerbState.Bursting || !instance.CanHitTarget(castTarg))
            {
                return false;
            }
            Traverse.Create(instance).Field("currentTarget").SetValue(castTarg);
            Log.Message("initial checks ok");
            if (instance.CasterIsPawn && instance.verbProps.warmupTime > 0f)
            {
                ShootLine newShootLine;
                if (!instance.TryFindShootLineFromTo(instance.caster.Position, castTarg, out newShootLine))
                {
                    Log.Message("couldn't find shooting line");
                    return false;
                }
                instance.CasterPawn.Drawer.Notify_WarmingCastAlongLine(newShootLine, instance.caster.Position);
                float statValue = instance.CasterPawn.GetStatValue(StatDefOf.AimingDelayFactor, true);
                int ticks = (instance.verbProps.warmupTime * statValue).SecondsToTicks();
                Log.Message("setting stance Stance_Warmup_DW");
                instance.CasterPawn.GetStancesOffHand().SetStance(new Stance_Warmup_DW(ticks, castTarg, instance));
            }
            else
            {
                instance.WarmupComplete();
            }
            return true;
        }
    }
}
