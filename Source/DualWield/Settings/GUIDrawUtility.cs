using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;
using HugsLib.Settings;

namespace DualWield.Settings
{
    public class GUIDrawUtility
    {

        private const float ContentPadding = 5f;

        private const float TextMargin = 20f;
        private const float BottomMargin = 2f;
        private const float buttonHeight = 28f;
        private static readonly Color iconBaseColor = new Color(0.5f, 0.5f, 0.5f, 1f);
        private static readonly Color iconMouseOverColor = new Color(0.6f, 0.6f, 0.4f, 1f);

        private static readonly Color SelectedOptionColor = new Color(0.5f, 1f, 0.5f, 1f);
        private static readonly Color constGrey = new Color(0.8f, 0.8f, 0.8f, 1f);

        private static Color background = new Color(0.5f, 0, 0, 0.1f);
        private static Color selectedBackground = new Color(0f, 0.5f, 0, 0.1f);
        private static Color notSelectedBackground = new Color(0.5f, 0, 0, 0.1f);
        private const float IconSize = 32f;
        private const float IconGap = 1f;


        private static void DrawBackground(Rect rect, Color background)
        {
            Color save = GUI.color;
            GUI.color = background;
            GUI.DrawTexture(rect, TexUI.FastFillTex);
            GUI.color = save;
        }
        private static void DrawLabel(string labelText, Rect textRect, float offset)
        {
            var labelHeight = Text.CalcHeight(labelText, textRect.width);
            labelHeight -= 2f;
            var labelRect = new Rect(textRect.x, textRect.yMin - labelHeight + offset, textRect.width, labelHeight);
            GUI.DrawTexture(labelRect, TexUI.GrayTextBG);
            GUI.color = Color.white;
            Text.Anchor = TextAnchor.UpperCenter;
            Widgets.Label(labelRect, labelText);
            Text.Anchor = TextAnchor.UpperLeft;
            GUI.color = Color.white;
        }
        private static Color GetColor(ThingDef thingDef)
        {
            if (thingDef.graphicData != null)
            {
                return thingDef.graphicData.color;
            }
            return Color.white;
        }

        private static bool DrawTileForThingDef(ThingDef thingDef, KeyValuePair<String, Record> kv, Rect contentRect, Vector2 iconOffset, int buttonID)
        {
            var iconRect = new Rect(contentRect.x + iconOffset.x, contentRect.y + iconOffset.y, IconSize, IconSize);
            MouseoverSounds.DoRegion(iconRect, SoundDefOf.Mouseover_Command);
            Color save = GUI.color;

            if (Mouse.IsOver(iconRect))
            {
                GUI.color = iconMouseOverColor;
            }
            else if (kv.Value.isSelected == true)
            {
                GUI.color = selectedBackground;
            }
            else
            {
                GUI.color = notSelectedBackground;
            }
            GUI.DrawTexture(iconRect, TexUI.FastFillTex);
            GUI.color = save;

            TooltipHandler.TipRegion(iconRect, thingDef.label);

            Graphic g2 = null;
            Color color = GetColor(thingDef);
            if (thingDef.graphicData != null && thingDef.graphicData.Graphic != null)
            {
                Graphic g = thingDef.graphicData.Graphic;
                g2 = thingDef.graphicData.Graphic.GetColoredVersion(g.Shader, color, color);
            }
            Texture resolvedIcon;
            if (!thingDef.uiIconPath.NullOrEmpty())
            {
                resolvedIcon = thingDef.uiIcon;
            }
            else if (g2 != null)
            {
                resolvedIcon = g2.MatSingle.mainTexture;
            }
            else
            {
                resolvedIcon = new Texture();
            }

            GUI.color = color;
            GUI.DrawTexture(iconRect, resolvedIcon);
            GUI.color = Color.white;

            if (Widgets.ButtonInvisible(iconRect, true))
            {
                Event.current.button = buttonID;
                return true;
            }
            else
                return false;

        }


        public static bool CustomDrawer_Button(Rect rect, SettingHandle<bool> setting, String activateText, String deactivateText, int xOffset = 0, int yOffset = 0)
        {
            int labelWidth = (int)rect.width - 20;
            int horizontalOffset = 0;
            int verticalOffset = 0;
            bool change = false;
            Rect buttonRect = new Rect(rect);
            buttonRect.width = labelWidth;
            buttonRect.position = new Vector2(buttonRect.position.x + horizontalOffset + xOffset, buttonRect.position.y + verticalOffset + yOffset);
            Color activeColor = GUI.color;
            bool isSelected = setting.Value;
            String text = setting ? deactivateText : activateText;

            if (isSelected)
                GUI.color = SelectedOptionColor;
            bool clicked = Widgets.ButtonText(buttonRect, text);
            if (isSelected)
                GUI.color = activeColor;

            if (clicked)
            {
                setting.Value = !setting.Value;
                change = true;
            }
            return change;
        }

        public static bool CustomDrawer_Tabs(Rect rect, SettingHandle<String> setting, String[] defaultValues, bool vertical = false, int xOffset = 0, int yOffset = 0)
        {
            int labelWidth = (int)rect.width - 20;

            int horizontalOffset = 0;
            int verticalOffset = 0;

            bool change = false;

            foreach (String tab in defaultValues)
            {

                Rect buttonRect = new Rect(rect);
                buttonRect.width = labelWidth;
                buttonRect.position = new Vector2(buttonRect.position.x + horizontalOffset + xOffset, buttonRect.position.y + verticalOffset + yOffset);
                Color activeColor = GUI.color;
                bool isSelected = tab == setting.Value;
                if (isSelected)
                    GUI.color = SelectedOptionColor;
                bool clicked = Widgets.ButtonText(buttonRect, tab);
                if (isSelected)
                    GUI.color = activeColor;


                if (clicked)
                {
                    if (setting.Value != tab)
                    {
                        setting.Value = tab;
                    }
                    //else
                    //{
                    //    setting.Value = "none";
                    //}
                    change = true;
                }

                if (vertical)
                {
                    verticalOffset += (int)buttonRect.height;
                }
                else
                {
                    horizontalOffset += labelWidth;
                }

            }
            if (vertical)
            {
                //setting.CustomDrawerHeight = verticalOffset;
            }
            return change;
        }


        public static bool CustomDrawer_Filter(Rect rect, SettingHandle<float> slider, bool def_isPercentage, float def_min, float def_max, Color background)
        {
            DrawBackground(rect, background);
            int labelWidth = 50;

            Rect sliderPortion = new Rect(rect);
            sliderPortion.width = sliderPortion.width - labelWidth;

            Rect labelPortion = new Rect(rect);
            labelPortion.width = labelWidth;
            labelPortion.position = new Vector2(sliderPortion.position.x + sliderPortion.width + 5f, sliderPortion.position.y + 4f);

            sliderPortion = sliderPortion.ContractedBy(2f);

            if (def_isPercentage)
                Widgets.Label(labelPortion, (Mathf.Round(slider.Value * 100f)).ToString("F0") + "%");
            else
                Widgets.Label(labelPortion, slider.Value.ToString("F2"));

            float val = Widgets.HorizontalSlider(sliderPortion, slider.Value, def_min, def_max, true);
            bool change = false;

            if (slider.Value != val)
                change = true;

            slider.Value = val;
            return change;
        }


        public static bool CustomDrawer_MatchingThingDefs_active(Rect wholeRect, SettingHandle<DictRecordHandler> setting, Dictionary<string, Record> defaults, List<ThingDef> allThingDefs, string yesText = "", string noText = "")
        {
            //TODO: refactor this mess, remove redundant and quircky things.

            float rowHeight = 20f;
            if (setting.Value == null)
            {
                setting.Value = new DictRecordHandler();
                foreach (KeyValuePair<string, Record> kv in defaults)
                {
                    setting.Value.InnerList.Add(kv.Key, kv.Value);
                }
                //setting.Value = Base.GetDefaultForFactionRestrictions(new Dict2DRecordHandler(), allPawns, allFactionNames);
            }
            //CustomDrawer_Tabs(new Rect(wholeRect.x, wholeRect.y, (float)wholeRect.width, buttonHeight), filter, allFactionNames.ToArray(), true, (int)-wholeRect.width, 0);
            DrawBackground(wholeRect, background);


            GUI.color = Color.white;

            Rect leftRect = new Rect(wholeRect);
            leftRect.width = leftRect.width / 2;
            leftRect.height = wholeRect.height - TextMargin + BottomMargin;
            leftRect.position = new Vector2(leftRect.position.x, leftRect.position.y);
            Rect rightRect = new Rect(wholeRect);
            rightRect.width = rightRect.width / 2;
            leftRect.height = wholeRect.height - TextMargin + BottomMargin;
            rightRect.position = new Vector2(rightRect.position.x + leftRect.width, rightRect.position.y);

            DrawLabel(yesText, leftRect, TextMargin);
            DrawLabel(noText, rightRect, TextMargin);

            leftRect.position = new Vector2(leftRect.position.x, leftRect.position.y + TextMargin);
            rightRect.position = new Vector2(rightRect.position.x, rightRect.position.y + TextMargin);
            int iconsPerRow = (int)(leftRect.width / (IconGap + IconSize));

            bool change = false;
            //bool factionFound = setting.Value.InnerList.TryGetValue(filter.Value, out Dictionary<string, Record> selection);
            //FilterSelection(ref selection, allPawns, filter.Value);
            int indexLeft = 0;
            int indexRight = 0;
            foreach (KeyValuePair<String, Record> item in setting.Value.InnerList)
            {
                Rect rect = item.Value.isSelected ? leftRect : rightRect;
                int index = item.Value.isSelected ? indexLeft : indexRight;
                //float tileHeight = item.Value.label.Count() > 16 ? 2 * rowHeight : rowHeight;
                leftRect.height = IconSize;
                rightRect.height = IconSize;

                if (item.Value.isSelected)
                {
                    indexLeft++;
                }
                else
                {
                    indexRight++;
                }
                int column = index % iconsPerRow;
                int row = index / iconsPerRow;
                ThingDef thingDef = allThingDefs.FirstOrDefault((ThingDef td) => td.defName == item.Key);
                bool interacted = DrawTileForThingDef(thingDef, item, rect, new Vector2(IconSize * column + column * IconGap, IconSize * row + row * IconGap), index);
                if (interacted)
                {
                    change = true;
                    item.Value.isSelected = !item.Value.isSelected;
                }
            }
            int biggerRows = Math.Max(indexLeft / iconsPerRow, (setting.Value.InnerList.Count - indexLeft) / iconsPerRow) + 1;
            setting.CustomDrawerHeight = (biggerRows * IconSize) + (biggerRows * IconGap) + TextMargin;
            //setting.CustomDrawerHeight = Math.Max(leftHeight, rightHeight) + TextMargin;

            if (change)
            {
                //setting.Value.InnerList[filter.Value] = selection;
                //setting.Value.InnerList = selection;
            }
            return change;
        }


    }
}



