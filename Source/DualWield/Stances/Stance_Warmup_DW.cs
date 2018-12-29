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
                return Pawn.stances.curStance.StanceBusy;
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
                //GenDraw.DrawAimPie(this.stanceTracker.pawn, this.focusTarg, (int)((float)this.ticksLeft * this.pieSizeFactor), 0.2f);
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
                GenDraw.DrawAimPieRaw(shooter.DrawPos + new Vector3(0, 0.2f, 0.25f), facing, (int)((float)this.ticksLeft * this.pieSizeFactor));
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
