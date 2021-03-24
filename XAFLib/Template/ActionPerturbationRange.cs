using System;
using System.Text;
using System.Xml;

namespace Triggerless.XAFLib
{
    [Serializable]

    public class ActionPerturbationRange: IndexXmlGenerator
    {
        public float? LoopIterationsOffsetRange { get; set; }
        public float? PlaybackSpeedScaleScaleOffsetRange { get; set; }

        public override void AddXml(XmlElement parent, int? index = null)
        {
            XmlElement result = parent.OwnerDocument.CreateElement(nameof(ActionPerturbationRange));
            if (LoopIterationsOffsetRange.HasValue)
            {
                XmlElement elL = parent.OwnerDocument.CreateElement(nameof(LoopIterationsOffsetRange));
                elL.InnerText = LoopIterationsOffsetRange.Value.ToString();
                result.AppendChild(elL);
            }

            if (PlaybackSpeedScaleScaleOffsetRange.HasValue)
            {
                XmlElement elP = parent.OwnerDocument.CreateElement(nameof(PlaybackSpeedScaleScaleOffsetRange));
                elP.InnerText = PlaybackSpeedScaleScaleOffsetRange.Value.ToString();
                result.AppendChild(elP);
            }
            parent.AppendChild(result);
        }

        public override void AppendText(StringBuilder sb, int? index = null)
        {
            string tagName = nameof(ActionPerturbationRange);
            sb.AppendUnixLine(Open(tagName));
            if (LoopIterationsOffsetRange.HasValue)
            {
                string nameL = nameof(LoopIterationsOffsetRange);
                sb.AppendUnixLine($"<{nameL}>{LoopIterationsOffsetRange.Value}</{nameL}>");

            }
            if (PlaybackSpeedScaleScaleOffsetRange.HasValue)
            {
                string nameL = nameof(PlaybackSpeedScaleScaleOffsetRange);
                sb.AppendUnixLine($"<{nameL}>{PlaybackSpeedScaleScaleOffsetRange.Value}</{nameL}>");

            }
            sb.AppendUnixLine(Close(tagName));
        }

        public override void LoadXml(XmlNode node)
        {
            XmlElement elL = node.SelectSingleNode(nameof(LoopIterationsOffsetRange)) as XmlElement;
            if (elL != null & float.TryParse(elL.InnerText, out float val)) LoopIterationsOffsetRange = val;

            elL = node.SelectSingleNode(nameof(PlaybackSpeedScaleScaleOffsetRange)) as XmlElement;
            if (elL != null & float.TryParse(elL.InnerText, out float val2)) PlaybackSpeedScaleScaleOffsetRange = val2;
        }
    }

}
