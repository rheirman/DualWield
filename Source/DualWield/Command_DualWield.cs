using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace DualWield
{
    public class Command_DualWield : Command_VerbTarget
    {
        private Thing offHandThing;
        private Verb offHandVerb;
        public Command_DualWield(Thing offHandThing)
        {
            this.offHandThing = offHandThing;
            if (this.offHandThing.TryGetComp<CompEquippable>() is CompEquippable ce)
            {
                offHandVerb = ce.PrimaryVerb;
            }
        }

        public override float GetWidth(float maxWidth)
        {
            return base.GetWidth(maxWidth);
        }

        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth)
        {
            GizmoResult res = base.GizmoOnGUI(topLeft, maxWidth);
            GUI.color = offHandThing.DrawColor;
            Material material = (!this.disabled) ? null : TexUI.GrayscaleGUI;
            Texture2D tex = offHandThing.def.uiIcon;
            if (tex == null)
            {
                tex = BaseContent.BadTex;
            }
            Rect rect = new Rect(topLeft.x, topLeft.y + 5, this.GetWidth(maxWidth), 75f);
            Widgets.DrawTextureFitted(rect, tex, this.iconDrawScale * 0.85f, this.iconProportions, this.iconTexCoords, this.iconAngle, material);
            GUI.color = Color.white;
            return res;
        }

        public override void GizmoUpdateOnMouseover()
        {
            base.GizmoUpdateOnMouseover();
            this.offHandVerb.verbProps.DrawRadiusRing(this.offHandVerb.caster.Position);       
        }
        public override void ProcessInput(Event ev)
        {
            base.ProcessInput(ev);
            if (offHandVerb.IsMeleeAttack)
            {
                return;
            }
            Targeter targeter = Find.Targeter;
            if (this.offHandVerb.CasterIsPawn && targeter.targetingVerb != null && targeter.targetingVerb.verbProps == this.offHandVerb.verbProps)
            {
                Pawn casterPawn = this.offHandVerb.CasterPawn;
                if (!targeter.IsPawnTargeting(casterPawn))
                {
                    targeter.targetingVerbAdditionalPawns.Add(casterPawn);
                }
            }
            else
            {
                Find.Targeter.BeginTargeting(this.offHandVerb);
            }
        }
    }
}
