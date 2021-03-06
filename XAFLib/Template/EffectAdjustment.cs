using System;
using System.Text;
using System.Xml;

namespace XAFLib
{
    [Serializable]

    public class EffectAdjustment: IndexXmlGenerator
    {
        public float? PlaybackSpeedScaleScale { get; set; }

        public override void AddXml(XmlElement parent, int? index = null)
        {
            var result = parent.OwnerDocument.CreateElement(nameof(EffectAdjustment));
            if (PlaybackSpeedScaleScale.HasValue)
            {
                var elG = parent.OwnerDocument.CreateElement(nameof(PlaybackSpeedScaleScale));
                elG.InnerText = PlaybackSpeedScaleScale.Value.ToString();
                result.AppendChild(elG);
            }
            parent.AppendChild(result);
        }

        public override void AppendText(StringBuilder sb, int? index = null)
        {
            var tagName = nameof(EffectAdjustment);
            sb.AppendUnixLine(Open(tagName));
            if (PlaybackSpeedScaleScale.HasValue)
            {
                var nameL = nameof(PlaybackSpeedScaleScale);
                sb.AppendUnixLine($"<{nameL}>{PlaybackSpeedScaleScale.Value}</{nameL}>");
            }
            sb.AppendUnixLine(Close(tagName));
        }

        public override void LoadXml(XmlNode node)
        {
            XmlElement elL = node.SelectSingleNode(nameof(PlaybackSpeedScaleScale)) as XmlElement;
            if (elL != null & float.TryParse(elL.InnerText, out float val)) PlaybackSpeedScaleScale = val;
        }
    }

}
