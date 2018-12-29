using DualWield.Storage;
using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace DualWield.Harmony
{

    [HarmonyPatch(typeof(Projectile), "Launch")]
    [HarmonyPatch(new Type[] { typeof(Thing), typeof(Vector3), typeof(LocalTargetInfo), typeof(LocalTargetInfo), typeof(ProjectileHitFlags), typeof(Thing), typeof(ThingDef) })]
    static class Projectile_Launch
    {
        static void Prefix(ref Thing launcher, ref Vector3 origin, Thing equipment)
        {
            if (!(launcher is Pawn pawn) || !(equipment is ThingWithComps twc))
            {
                return;
            }
            ExtendedThingWithCompsData twcData = Base.Instance.GetExtendedDataStorage().GetExtendedDataFor(twc);
            if (twcData.isOffHand)
            {
                origin += new Vector3(0, 0, 0.25f);
            }
            
        }
    }
}
