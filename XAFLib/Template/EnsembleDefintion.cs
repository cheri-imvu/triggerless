using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Triggerless.XAFLib
{
    [Serializable]

    public class EnsembleDefintion : IndexXmlGenerator
    {
        public List<SkeletalAnimationEffect> SkeletalAnimationEffects { get; }
        public List<MorphAnimationEffect> MorphAnimationEffects { get; }
        public EnsembleAttributes EnsembleAttributes { get; set; }

        public EnsembleDefintion() {
            SkeletalAnimationEffects = new List<SkeletalAnimationEffect>();
            MorphAnimationEffects = new List<MorphAnimationEffect>();
            EnsembleAttributes = new EnsembleAttributes();
        }

        public override void AddXml(XmlElement parent, int? index = null) {
            if (parent == null || !index.HasValue) return;
            if (parent.OwnerDocument != null) {
                XmlElement result = parent.OwnerDocument.CreateElement("EnsembleDefinition" + index);

                if (EnsembleAttributes != null)
                {
                    EnsembleAttributes.AddXml(result);
                }

                for (int i = 0; i < SkeletalAnimationEffects.Count; i++)
                {
                    SkeletalAnimationEffect effect = SkeletalAnimationEffects[i];
                    effect.AddXml(result, i);
                }

                for (int i = 0; i < MorphAnimationEffects.Count; i++)
                {
                    MorphAnimationEffect effect = MorphAnimationEffects[i];
                    effect.AddXml(result, i);
                }
                parent.AppendChild(result);
            }
        }

        public override void AppendText(StringBuilder sb, int? index = null) {
            string tagName = "EnsembleDefinition" + index;
            sb.AppendUnixLine(Open(tagName));

            if (EnsembleAttributes != null)
            {
                EnsembleAttributes.AppendText(sb);
            }

            for (int i = 0; i < SkeletalAnimationEffects.Count; i++)
            {
                SkeletalAnimationEffects[i].AppendText(sb, i);
            }
            for (int i = 0; i < MorphAnimationEffects.Count; i++)
            {
                MorphAnimationEffects[i].AppendText(sb, i);
            }
            sb.AppendUnixLine(Close(tagName));
        }

        public override void LoadXml(XmlNode node) {

            foreach (XmlElement el in node.ChildNodes) {
                if (el.Name == nameof(EnsembleAttributes))
                {
                    EnsembleAttributes = new EnsembleAttributes();
                    try
                    {
                        EnsembleAttributes.LoadXml(el);
                    } catch (Exception)
                    {
                        // skip it
                    }
                }

                if (el.Name.StartsWith("SkeletalAnimationEffect"))
                {
                    SkeletalAnimationEffect sae = new SkeletalAnimationEffect();
                    sae.LoadXml(el);
                    SkeletalAnimationEffects.Add(sae);
                }

                if (el.Name.StartsWith("MorphAnimationEffect"))
                {
                    MorphAnimationEffect mae = new MorphAnimationEffect();
                    mae.LoadXml(el);
                    MorphAnimationEffects.Add(mae);
                }
            }
        }
    }

}
