using DualWield.Settings;
using DualWield.Storage;
using HugsLib;
using HugsLib.Settings;
using HugsLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DualWield
{
    public class Base : ModBase
    {
        public static Base Instance { get; private set; }
        ExtendedDataStorage _extendedDataStorage;
        public override string ModIdentifier => "DualWield";

        internal static SettingHandle<DictRecordHandler> dualWieldSelection;
        internal static SettingHandle<DictRecordHandler> twoHandSelection;


        public Base()
        {
            Instance = this;
        }
        public override void DefsLoaded()
        {
            base.DefsLoaded();
            dualWieldSelection = Settings.GetHandle<DictRecordHandler>("factionRestrictions", "dualwield TODO", "dualwield TODO", null);
            List<ThingDef> allWeapons = GetAllWeapons();
            dualWieldSelection.CustomDrawer = rect => { return GUIDrawUtility.CustomDrawer_MatchingThingDefs_active(rect, dualWieldSelection, GetDualWieldDefaults(allWeapons), allWeapons, "dualwield TODO".Translate(), "dualwield TODO".Translate()); };
            twoHandSelection = Settings.GetHandle<DictRecordHandler>("twoHandSelection", "twoHandSelection TODO", "twoHandSelection TODO", null);
            twoHandSelection.CustomDrawer = rect => { return GUIDrawUtility.CustomDrawer_MatchingThingDefs_active(rect, twoHandSelection, GetTwoHandDefaults(allWeapons), allWeapons, "twoHandSelection TODO".Translate(), "twoHandSelection TODO".Translate()); };

        }
        private static Dictionary<string, Record> GetDualWieldDefaults(List<ThingDef> allWeapons)
        {
            Dictionary<string, Record> dict = new Dictionary<string, Record>();
            foreach(ThingDef td in allWeapons)
            {
                dict.Add(td.defName, new Record(true, td.label));
            }
            return dict;
        }
        private static Dictionary<string, Record> GetTwoHandDefaults(List<ThingDef> allWeapons)
        {
            Dictionary<string, Record> dict = new Dictionary<string, Record>();
            foreach (ThingDef td in allWeapons)
            {
                dict.Add(td.defName, new Record(false, td.label));
            }
            return dict;
        }
        private static List<ThingDef> GetAllWeapons()
        {
            List<ThingDef> allWeapons = new List<ThingDef>();

            Predicate<ThingDef> isWeapon = (ThingDef td) => td.equipmentType == EquipmentType.Primary && !td.weaponTags.NullOrEmpty<string>();
            foreach (ThingDef thingDef in from td in DefDatabase<ThingDef>.AllDefs
                                          where isWeapon(td)
                                          select td)
            {
                allWeapons.Add(thingDef);
            }
            return allWeapons;
        }

        public override void MapComponentsInitializing(Map map)
        {
            _extendedDataStorage = UtilityWorldObjectManager.GetUtilityWorldObject<ExtendedDataStorage>();
            base.MapComponentsInitializing(map);
        }
        public ExtendedDataStorage GetExtendedDataStorage()
        {
            return _extendedDataStorage;
        }
        
    }
}
