using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DualWield.Stances
{
    class Stance_Cooldown_DW : Stance_Cooldown
    {
        private const float MaxRadius = 0.5f;
        public override bool StanceBusy
        {
            get
            {
                return Pawn.stances.curStance.StanceBusy;
            }
        }
        public Stance_Cooldown_DW()
        {
        }
        public Stance_Cooldown_DW(int ticks, LocalTargetInfo focusTarg, Verb verb) : base(ticks, focusTarg, verb)
        {
        }

    }
}
