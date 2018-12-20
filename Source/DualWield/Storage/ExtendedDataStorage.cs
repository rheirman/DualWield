using HugsLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DualWield.Storage
{
    /**
     * Storage used to extend existing Rimworld objects with additional data. Caution should be taken when assigning the IDs. 
     **/
    public class ExtendedDataStorage : UtilityWorldObject, IExposable
    {
        private Dictionary<int, IExposable> _store =
            new Dictionary<int, IExposable>();

        private List<int> _idWorkingList;
        private List<IExposable> _extendedPawnDataWorkingList;
        internal int lastEmergencySignalTick = 0;
        internal int lastEmergencySignalDelay = 0;
        internal int lastEmergencySignalCooldown = 0;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(
                ref _store, "store",
                LookMode.Value, LookMode.Deep,
                ref _idWorkingList, ref _extendedPawnDataWorkingList);
            Scribe_Values.Look(ref lastEmergencySignalTick, "lastEmergencySignalTick");
            Scribe_Values.Look(ref lastEmergencySignalDelay, "lastEmergencySignalDelay");
            Scribe_Values.Look(ref lastEmergencySignalDelay, "lastEmergencySignalCooldown");
        }

        // Return the associate extended data for a given Pawn, creating a new association
        // if required.
        public ExtendedPawnData GetExtendedDataFor(Pawn pawn)
        {

            var id = pawn.thingIDNumber;
            if (_store.TryGetValue(id, out IExposable data))
            {
                return (ExtendedPawnData) data;
            }

            var newExtendedData = new ExtendedPawnData();

            _store[id] = newExtendedData;
            return newExtendedData;
        }

        public void DeleteExtendedDataFor(Pawn pawn)
        {
            _store.Remove(pawn.thingIDNumber);
        }
    }
}
