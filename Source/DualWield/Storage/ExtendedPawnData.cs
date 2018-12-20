using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DualWield.Storage
{
    public class ExtendedPawnData : IExposable
    {
        public Pawn_StanceTracker stancesOffhand = null;
        public void ExposeData()
        {
            Scribe_Deep.Look(ref stancesOffhand, "stancesOffhand", false);
        }
    }
}
