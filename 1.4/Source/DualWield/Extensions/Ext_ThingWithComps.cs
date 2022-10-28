using DualWield.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DualWield
{
    public static class Ext_ThingWithComps
    {
        public static bool IsOffHand(this ThingWithComps instance)
        {
            ExtendedDataStorage store = Base.Instance.GetExtendedDataStorage();
            if(store != null)
            {
                if(store.TryGetExtendedDataFor(instance, out ExtendedThingWithCompsData twcData))
                {
                    return twcData.isOffHand;
                }
            }
            return false;
        }
    }
}
