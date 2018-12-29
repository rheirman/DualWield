using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DualWield.Storage
{
    public class ExtendedThingWithCompsData : IExposable
    {
        public bool isOffHand = false;
        public void ExposeData()
        {
            Scribe_Values.Look(ref isOffHand, "isOffHand", false);
        }
    }
}
