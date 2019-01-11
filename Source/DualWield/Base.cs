using DualWield.Settings;
using DualWield.Storage;
using HugsLib;
using HugsLib.Settings;
using HugsLib.Utils;
using RimWorld;
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

        internal static SettingHandle<bool> settingsGroup_DualWield;
        internal static SettingHandle<bool> settingsGroup_TwoHand;
        internal static SettingHandle<bool> settingsGroup_Balancing;


        internal static SettingHandle<DictRecordHandler> dualWieldSelection;
        internal static SettingHandle<DictRecordHandler> twoHandSelection;
        internal static SettingHandle<float> staticCooldownPOffHand;
        internal static SettingHandle<float> staticCooldownPMainHand;
        internal static SettingHandle<float> staticAccPOffHand;
        internal static SettingHandle<float> staticAccPMainHand;
        internal static SettingHandle<float> dynamicCooldownP;
        internal static SettingHandle<float> dynamicAccP;


        public Base()
        {
            Instance = this;
        }
        public override void DefsLoaded()
        {
            base.DefsLoaded();
            List<ThingDef> allWeapons = GetAllWeapons();

            settingsGroup_DualWield = Settings.GetHandle<bool>("settingsGroup_DualWieldSelection", "DW_SettingsGroup_DualWieldSelection_Title".Translate(), "DW_SettingsGroup_DualWieldSelection_Description".Translate(), false);
            settingsGroup_DualWield.CustomDrawer = rect => { return GUIDrawUtility.CustomDrawer_Button(rect, settingsGroup_DualWield, "DW_Expand".Translate() + "..", "DW_Collapse".Translate()); };
            dualWieldSelection = Settings.GetHandle<DictRecordHandler>("dualWieldSelection", "", "", null);
            if (dualWieldSelection.Value != null)
            {
                if(dualWieldSelection.Value.inner.Count < allWeapons.Count)
                    AddMissingWeaponsForDualWieldSelection(allWeapons);
                if(dualWieldSelection.Value.inner.Count > allWeapons.Count)
                {
                    RemoveDepricatedRecords(allWeapons, dualWieldSelection.Value.inner);
                }
            }
            dualWieldSelection.CustomDrawer = rect => { return GUIDrawUtility.CustomDrawer_MatchingThingDefs_active(rect, dualWieldSelection, GetDualWieldDefaults(allWeapons), allWeapons, "DW_Setting_DualWield_OK".Translate(), "DW_Setting_DualWield_NOK".Translate()); };
            dualWieldSelection.VisibilityPredicate = delegate { return settingsGroup_DualWield; };

            settingsGroup_TwoHand = Settings.GetHandle<bool>("settingsGroup_TwoHandSelection", "DW_SettingsGroup_TwoHandSelection_Title".Translate(), "DW_SettingsGroup_TwoHandSelection_Description".Translate(), false);
            settingsGroup_TwoHand.CustomDrawer = rect => { return GUIDrawUtility.CustomDrawer_Button(rect, settingsGroup_TwoHand, "DW_Expand".Translate() + "..", "DW_Collapse".Translate()); };
            twoHandSelection = Settings.GetHandle<DictRecordHandler>("twoHandSelection", "", "", null);
            if (twoHandSelection.Value != null)
            {
                if(twoHandSelection.Value.inner.Count < allWeapons.Count)
                {
                    AddMissingWeaponsForTwoHandSelection(allWeapons);
                }
                if (twoHandSelection.Value.inner.Count > allWeapons.Count)
                {
                    RemoveDepricatedRecords(allWeapons, twoHandSelection.Value.inner);
                }
            }
            twoHandSelection.CustomDrawer = rect => { return GUIDrawUtility.CustomDrawer_MatchingThingDefs_active(rect, twoHandSelection, GetTwoHandDefaults(allWeapons), allWeapons, "DW_Setting_TwoHanded_OK".Translate(), "DW_Setting_TwoHanded_NOK".Translate()); };
            twoHandSelection.VisibilityPredicate = delegate { return settingsGroup_TwoHand; };

            //TODO TRANSLATIONS!!!
            settingsGroup_Balancing = Settings.GetHandle<bool>("settingsGroup_Balancing", "DW_SettingsGroup_Balancing_Title".Translate(), "DW_SettingsGroup_Balancing_Description".Translate(), false);
            settingsGroup_Balancing.CustomDrawer = rect => { return GUIDrawUtility.CustomDrawer_Button(rect, settingsGroup_Balancing, "DW_Expand".Translate() + "..", "DW_Collapse".Translate()); };
            staticCooldownPOffHand = Settings.GetHandle<float>("staticCooldownPenOffHand", "DW_Setting_StaticCooldownPenOffHand_Title".Translate(), "DW_Setting_StaticCooldownPenOffHand_Description".Translate(), 0.2f, Validators.IntRangeValidator(0, 5));
            staticCooldownPMainHand = Settings.GetHandle<float>("staticCooldownPMainHand", "DW_Setting_StaticCooldownPMainHand_Title".Translate(), "DW_Setting_StaticCooldownPMainHand_Description".Translate(), 0.1f, Validators.IntRangeValidator(0, 5));
            staticAccPOffHand = Settings.GetHandle<float>("staticAccPOffHand", "DW_Setting_StaticAccPOffHand_Title".Translate(), "DW_Setting_StaticAccPOffHand_Description".Translate(), 0.1f, Validators.IntRangeValidator(0, 5));
            staticAccPMainHand = Settings.GetHandle<float>("staticAccPMainHand", "DW_Setting_StaticAccPMainHand_Title".Translate(), "DW_Setting_StaticAccPMainHand_Description".Translate(), 0.1f, Validators.IntRangeValidator(0, 5));
            dynamicCooldownP = Settings.GetHandle<float>("dynamicCooldownP", "DW_Setting_DynamicCooldownP_Title".Translate(), "DW_Setting_DynamicCooldownP_Description".Translate(), 0.05f, Validators.IntRangeValidator(0, 1));
            dynamicAccP = Settings.GetHandle<float>("dynamicAccP", "DW_Setting_DynamicAccP_Title".Translate(), "DW_Setting_DynamicAccP_Description".Translate(), 0.005f, Validators.IntRangeValidator(0, 1));
        }

        private static void RemoveDepricatedRecords(List<ThingDef> allWeapons, Dictionary<string, Record> dict)
        {
            List<string> shouldRemove = new List<string>();
            foreach (string key in from string defName in dict.Keys where !allWeapons.Exists((ThingDef td) => td.defName == defName) select defName)
            {
                shouldRemove.Add(key);
            }
            foreach(string key in shouldRemove)
            {
                dict.Remove(key);
            }
        }

        private static void AddMissingWeaponsForDualWieldSelection(List<ThingDef> allWeapons)
        {
            foreach (ThingDef weapon in from td in allWeapons where !dualWieldSelection.Value.inner.ContainsKey(td.defName) select td)
            {
                SetDualWieldDefault(dualWieldSelection.Value.inner, weapon);
            }
        }
        private static void AddMissingWeaponsForTwoHandSelection(List<ThingDef> allWeapons)
        {
            foreach (ThingDef weapon in from td in allWeapons where !twoHandSelection.Value.inner.ContainsKey(td.defName) select td)
            {
                SetTwoHandDefault(twoHandSelection.Value.inner, weapon);
            }
        }
        private static Dictionary<string, Record> GetDualWieldDefaults(List<ThingDef> allWeapons)
        {
            Dictionary<string, Record> dict = new Dictionary<string, Record>();
            foreach(ThingDef td in allWeapons)
            {
                SetDualWieldDefault(dict, td);
            }
            return dict;
        }

        private static void SetDualWieldDefault(Dictionary<string, Record> dict, ThingDef td)
        {
            if (td.defName.Contains("Bow_") || td.GetStatValueAbstract(StatDefOf.Mass) > 3f || (td.IsMeleeWeapon && td.GetStatValueAbstract(StatDefOf.Mass) > 1.5f))
            {
                dict.Add(td.defName, new Record(false, td.label));
            }
            else
            {
                dict.Add(td.defName, new Record(true, td.label));
            }
        }

        private static Dictionary<string, Record> GetTwoHandDefaults(List<ThingDef> allWeapons)
        {
            Dictionary<string, Record> dict = new Dictionary<string, Record>();
            foreach (ThingDef td in allWeapons)
            {
                SetTwoHandDefault(dict, td);
            }
            return dict;
        }

        private static void SetTwoHandDefault(Dictionary<string, Record> dict, ThingDef td)
        {
            if (td.defName.Contains("Bow") || td.defName.Contains("Shotgun") || td.GetStatValueAbstract(StatDefOf.Mass) > 3f)
            {
                dict.Add(td.defName, new Record(true, td.label));
            }
            else
            {
                dict.Add(td.defName, new Record(false, td.label));
            }
        }

        private static List<ThingDef> GetAllWeapons()
        {
            List<ThingDef> allWeapons = new List<ThingDef>();

            Predicate<ThingDef> isWeapon = (ThingDef td) => td.equipmentType == EquipmentType.Primary && !td.weaponTags.NullOrEmpty<string>() && !td.destroyOnDrop;
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
