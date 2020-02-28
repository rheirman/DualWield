using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace DualWield.Settings
{
    class Dialog_Weapon_Rotation : Dialog_NodeTree
    {
        public float sliderValue = 0;
        public Dialog_Weapon_Rotation(float sliderValue, DiaNode nodeRoot, bool delayInteractivity = false, bool radioMode = false, string title = null) : base(nodeRoot, delayInteractivity, radioMode, title)
        {
            this.sliderValue = sliderValue;
        }
        public override void DoWindowContents(Rect inRect)
        {
            
            Rect rect = windowRect.AtZero();
            Rect sliderPortion = new Rect(rect);
            int labelWidth = 50;

            sliderPortion.width = sliderPortion.width - labelWidth;

            Rect labelPortion = new Rect(rect);
            labelPortion.width = labelWidth;
            labelPortion.position = new Vector2(sliderPortion.position.x + sliderPortion.width + 5f, sliderPortion.position.y + 4f);

            sliderPortion = sliderPortion.ContractedBy(2f);
            sliderValue = Widgets.HorizontalSlider(sliderPortion, sliderValue, 0, 360, true);
            Widgets.Label(labelPortion, sliderValue.ToString("F2"));

            base.DoWindowContents(inRect);
        }
    }
}
