using System;
using System.Text;
using System.Xml;

namespace XAFLib
{
    [Serializable]

    public class SkeletalAnimationEffect : IndexXmlGenerator
    {
        public string AssetName { get; set; }
        public SkeletalEffectControls EffectControls { get; set; }
        public SkeletalAnimationEffect() {
            EffectControls = new SkeletalEffectControls();
        }

        public override void AddXml(XmlElement parent, int? index = null) {
            if (parent == null || !index.HasValue) return;

            if (parent.OwnerDocument != null) {
                XmlElement result = parent.OwnerDocument.CreateElement("SkeletalAnimationEffect" + index);
                XmlElement an = parent.OwnerDocument.CreateElement("AssetName");
                an.InnerText = AssetName;
                result.AppendChild(an);
                EffectControls.AddXml(result);
                parent.AppendChild(result);
            }
        }

        public override void AppendText(StringBuilder sb, int? index = null) {
            if (index == null) return;
            string tagName = "SkeletalAnimationEffect" + index;
            sb.AppendUnixLine(Open(tagName));
            sb.AppendUnixLine(Open("AssetName") + AssetName + Close("AssetName"));
            EffectControls.AppendText(sb);
            sb.AppendUnixLine(Close(tagName));
        }

        public override void LoadXml(XmlNode node) {
            XmlElement an = node.SelectSingleNode("AssetName") as XmlElement;
            AssetName = an?.InnerText;
            var effcEl = node.SelectSingleNode("EffectControls") as XmlElement;
            if (effcEl != null) EffectControls.LoadXml(effcEl);
        }
    }

}
