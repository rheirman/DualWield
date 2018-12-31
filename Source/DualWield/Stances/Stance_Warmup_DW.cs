using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace DualWield.Stances
{
    class Stance_Warmup_DW : Stance_Warmup
    {
        public override bool StanceBusy
        {
            get
            {
                return false;
            }
        }
        public Stance_Warmup_DW()
        {
        }
        public Stance_Warmup_DW(int ticks, LocalTargetInfo focusTarg, Verb verb) : base(ticks, focusTarg, verb)
        {
        }
        public override void StanceDraw()
        {
            if (Find.Selector.IsSelected(this.stanceTracker.pawn))
            {
                Pawn shooter = this.stanceTracker.pawn;
                LocalTargetInfo target = this.focusTarg;
                float facing = 0f;
                if (target.Cell != shooter.Position)
                {
                    if (target.Thing != null)
                    {
                        facing = (target.Thing.DrawPos - shooter.Position.ToVector3Shifted()).AngleFlat();
                    }
                    else
                    {
                        facing = (target.Cell - shooter.Position).AngleFlat;
                    }
                }
                float zOffSet = 0f;
                if(shooter.Rotation == Rot4.East)
                {
                    zOffSet = 0.1f;
                }
                else if(shooter.Rotation == Rot4.West)
                {
                    zOffSet = -0.1f;
                }
                GenDraw.DrawAimPieRaw(shooter.DrawPos + new Vector3(0, 0.2f, zOffSet), facing, (int)((float)this.ticksLeft * this.pieSizeFactor));
            }
        }
        public override void StanceTick()
        {
            base.StanceTick();
            //if (Pawn.pather.MovingNow)
            //Using reflection here for Run and Gun Compatibility. 
            bool runAndGunEnabled = false;
            if(Pawn.AllComps.First((ThingComp tc) => tc.GetType().Name == "CompRunAndGun") is ThingComp comp)
            {
                runAndGunEnabled = Traverse.Create(comp).Field("isEnabled").GetValue<bool>();
            }
            if(!runAndGunEnabled && Pawn.pather.MovingNow)
            {
                    this.stanceTracker.pawn.GetStancesOffHand().SetStance(new Stance_Mobile());
            }
        }
        protected override void Expire()
        {
            this.verb.WarmupComplete();
            if (this.stanceTracker.curStance == this)
            {
                this.stanceTracker.pawn.GetStancesOffHand().SetStance(new Stance_Mobile());
            }
        }
    }
}
