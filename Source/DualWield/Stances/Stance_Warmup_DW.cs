using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        protected override void Expire()
        {
            this.verb.WarmupComplete();
            this.stanceTracker.pawn.GetStancesOffHand().SetStance(new Stance_Mobile());
            Log.Message("Warmup complete for Stance_Warmup_DW");
        }
    }
}
