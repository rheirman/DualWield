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

        internal static SettingHandle<bool> settingsGroup_Drawing;
        internal static SettingHandle<bool> settingsGroup_DualWield;
        internal static SettingHandle<bool> settingsGroup_TwoHand;
        internal static SettingHandle<bool> settingsGroup_Penalties;


        internal static SettingHandle<DictRecordHandler> dualWieldSelection;
        internal static SettingHandle<DictRecordHandler> twoHandSelection;
        internal static SettingHandle<DictRecordHandler> customRotations;

        internal static SettingHandle<float> staticCooldownPOffHand;
        internal static SettingHandle<float> staticCooldownPMainHand;
        internal static SettingHandle<float> staticAccPOffHand;
        internal static SettingHandle<float> staticAccPMainHand;
        internal static SettingHandle<float> dynamicCooldownP;
        internal static SettingHandle<float> dynamicAccP;

        internal static SettingHandle<float> meleeAngle;
        internal static SettingHandle<float> rangedAngle;
        internal static SettingHandle<float> meleeXOffset;
        internal static SettingHandle<float> rangedXOffset;

        internal static SettingHandle<string> note;

        internal static SettingHandle<int> NPCDualWieldChance;


        public Base()
        {
            Instance = this;
        }
        public override void DefsLoaded()
        {
            base.DefsLoaded();
            List<ThingDef> allWeapons = GetAllWeapons();

            settingsGroup_Drawing = Settings.GetHandle<bool>("settingsGroup_Drawing", "DW_SettingsGroup_Drawing_Title".Translate(), "DW_SettingsGroup_Drawing_Description".Translate(), false);
            settingsGroup_Drawing.CustomDrawer = rect => { return GUIDrawUtility.CustomDrawer_Button(rect, settingsGroup_Drawing, "DW_Expand".Translate() + "..", "DW_Collapse".Translate()); };

            note = Settings.GetHandle<string>("note", null, null, "DW_Setting_Note_Drawing".Translate());
            note.CustomDrawer = rect => { return GUIDrawUtility.CustomDrawer_Note(rect, note); };
            note.VisibilityPredicate = delegate { return settingsGroup_Drawing; };

            meleeAngle = Settings.GetHandle<float>("meleeAngle", "DW_Setting_MeleeAngle_Title".Translate(), "DW_Setting_MeleeAngle_Description".Translate(), 270f, Validators.FloatRangeValidator(0, 360f));
            meleeAngle.VisibilityPredicate = delegate { return settingsGroup_Drawing; };

            rangedAngle = Settings.GetHandle<float>("rangedAngle", "DW_Setting_RangedAngle_Title".Translate(), "DW_Setting_RangedAngle_Description".Translate(), 135f, Validators.FloatRangeValidator(0, 360f));
            rangedAngle.VisibilityPredicate = delegate { return settingsGroup_Drawing; };

            meleeXOffset = Settings.GetHandle<float>("meleeXOffset", "DW_Setting_MeleeXOffset_Title".Translate(), "DW_Setting_MeleeXOffset_Description".Translate(), 0.4f, Validators.FloatRangeValidator(0, 2f));
            meleeXOffset.VisibilityPredicate = delegate { return settingsGroup_Drawing; };

            rangedXOffset = Settings.GetHandle<float>("rangedXOffset", "DW_Setting_RangedXOffset_Title".Translate(), "DW_Setting_RangedXOffset_Description".Translate(), 0.1f, Validators.FloatRangeValidator(0, 2f));
            rangedXOffset.VisibilityPredicate = delegate { return settingsGroup_Drawing; };

            customRotations = Settings.GetHandle<DictRecordHandler>("customRotations", "DW_Setting_CustomRotations_Title".Translate(), "", null);
            customRotations.CustomDrawer = rect => { return GUIDrawUtility.CustomDrawer_MatchingThingDefs_dialog(rect, customRotations, GetRotationDefaults(allWeapons), allWeapons, "DW_Setting_CustomRotations_Header".Translate()); };
            customRotations.VisibilityPredicate = delegate { return settingsGroup_Drawing; };

            if (customRotations.Value != null)
            {
                if (customRotations.Value.inner.Count < allWeapons.Count)
                    AddMissingWeaponsForRotationSelection(allWeapons);
                if (customRotations.Value.inner.Count > allWeapons.Count)
                {
                    RemoveDepricatedRecords(allWeapons, customRotations.Value.inner);
                }
            }

            //settingsGroup_DualWield
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
            dualWieldSelection.CustomDrawer = rect => { return GUIDrawUtility.CustomDrawer_MatchingThingDefs_active(rect, dualWieldSelection, GetDualWieldDefaults(allWeapons), allWeapons, "DW_Setting_DualWield_OK".Translate(), "DW_Setting_DualWield_NOK".Translate(), twoHandSelection.Value != null ? twoHandSelection.Value.inner : null, "DW_Setting_DualWield_DisabledReason".Translate()); };
            dualWieldSelection.VisibilityPredicate = delegate { return settingsGroup_DualWield; };

            //settingsGroup_TwoHand
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
            twoHandSelection.CustomDrawer = rect => { return GUIDrawUtility.CustomDrawer_MatchingThingDefs_active(rect, twoHandSelection, GetTwoHandDefaults(allWeapons), allWeapons, "DW_Setting_TwoHanded_OK".Translate(), "DW_Setting_TwoHanded_NOK".Translate(), dualWieldSelection.Value != null ? dualWieldSelection.Value.inner : null, "DW_Setting_TwoHand_DisabledReason".Translate()); };
            twoHandSelection.VisibilityPredicate = delegate { return settingsGroup_TwoHand; };

            //settingsGroup_Penalties
            settingsGroup_Penalties = Settings.GetHandle<bool>("settingsGroup_Penalties", "DW_SettingsGroup_Penalties_Title".Translate(), "DW_SettingsGroup_Penalties_Description".Translate(), false);
            settingsGroup_Penalties.CustomDrawer = rect => { return GUIDrawUtility.CustomDrawer_Button(rect, settingsGroup_Penalties, "DW_Expand".Translate() + "..", "DW_Collapse".Translate()); };
            staticCooldownPOffHand = Settings.GetHandle<float>("staticCooldownPenOffHand", "DW_Setting_StaticCooldownPenOffHand_Title".Translate(), "DW_Setting_StaticCooldownPenOffHand_Description".Translate(), 20f, Validators.FloatRangeValidator(0, 500f));
            staticCooldownPOffHand.VisibilityPredicate = delegate { return settingsGroup_Penalties; };
            staticCooldownPMainHand = Settings.GetHandle<float>("staticCooldownPMainHand", "DW_Setting_StaticCooldownPMainHand_Title".Translate(), "DW_Setting_StaticCooldownPMainHand_Description".Translate(), 10f, Validators.FloatRangeValidator(0, 500f));
            staticCooldownPMainHand.VisibilityPredicate = delegate { return settingsGroup_Penalties; };
            staticAccPOffHand = Settings.GetHandle<float>("staticAccPOffHand", "DW_Setting_StaticAccPOffHand_Title".Translate(), "DW_Setting_StaticAccPOffHand_Description".Translate(), 10f, Validators.FloatRangeValidator(0, 500f));
            staticAccPOffHand.VisibilityPredicate = delegate { return settingsGroup_Penalties; };
            staticAccPMainHand = Settings.GetHandle<float>("staticAccPMainHand", "DW_Setting_StaticAccPMainHand_Title".Translate(), "DW_Setting_StaticAccPMainHand_Description".Translate(), 10f, Validators.FloatRangeValidator(0, 500f));
            staticAccPMainHand.VisibilityPredicate = delegate { return settingsGroup_Penalties; };
            dynamicCooldownP = Settings.GetHandle<float>("dynamicCooldownP", "DW_Setting_DynamicCooldownP_Title".Translate(), "DW_Setting_DynamicCooldownP_Description".Translate(), 5f, Validators.FloatRangeValidator(0, 100f));
            dynamicCooldownP.VisibilityPredicate = delegate { return settingsGroup_Penalties; };
            dynamicAccP = Settings.GetHandle<float>("dynamicAccP", "DW_Setting_DynamicAccP_Title".Translate(), "DW_Setting_DynamicAccP_Description".Translate(), 0.5f, Validators.FloatRangeValidator(0, 10f));
            dynamicAccP.VisibilityPredicate = delegate { return settingsGroup_Penalties; };

            NPCDualWieldChance = Settings.GetHandle<int>("NPCDualWieldChance", "DW_Setting_NPCDualWieldChance_Title".Translate(), "DW_Setting_NPCDualWieldChance_Description".Translate(), 40, Validators.IntRangeValidator(0, 100));


            if (customRotations.Value == null)
            {
                customRotations.Value = new DictRecordHandler();
                customRotations.Value.inner = GetRotationDefaults(allWeapons);
            }
            if(twoHandSelection.Value == null)
            {
                twoHandSelection.Value = new DictRecordHandler();
                twoHandSelection.Value.inner = GetTwoHandDefaults(allWeapons);
            }
            if(dualWieldSelection.Value == null)
            {
                dualWieldSelection.Value = new DictRecordHandler();
                dualWieldSelection.Value.inner = GetDualWieldDefaults(allWeapons);
            }
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

        private static void AddMissingWeaponsForRotationSelection(List<ThingDef> allWeapons)
        {
            foreach (ThingDef weapon in from td in allWeapons where !customRotations.Value.inner.ContainsKey(td.defName) select td)
            {
                SetRotationDefault(customRotations.Value.inner, weapon);
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
        
        private static Dictionary<string, Record> GetRotationDefaults(List<ThingDef> allWeapons)
        {
            Dictionary<string, Record> dict = new Dictionary<string, Record>();
            foreach (ThingDef td in allWeapons)
            {
                SetRotationDefault(dict, td);
            }
            return dict;
        }

        private static void SetRotationDefault(Dictionary<string, Record> dict, ThingDef td)
        {
            Record record = new Record(false, td.label);
            if (td.GetModExtension<DefModextension_CustomRotation>() is DefModextension_CustomRotation modExt)
            {
                record.extraRotation = modExt.extraRotation;
                record.isSelected = true;
            }
            dict.Add(td.defName, record);
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
