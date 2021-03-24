using System;
using System.Text;
using System.Xml;

namespace Triggerless.XAFLib
{
    [Serializable]

    public class MorphEffectControls : IndexXmlGenerator
    {
        public int? LoopIterations { get; set; }
        public int? LoopSectionEndFrame { get; set; }
        public int? LoopSectionStartFrame { get; set; }
        public int? RampDownFrames { get; set; }
        public int? RampUpFrames { get; set; }

        public override void AddXml(XmlElement parent, int? index = null)
        {
            if (parent?.OwnerDocument != null)
            {
                XmlElement result = parent.OwnerDocument.CreateElement("EffectControls");

                if (LoopIterations.HasValue)
                {
                    XmlElement li = parent.OwnerDocument.CreateElement("LoopIterations");
                    li.InnerText = LoopIterations.Value.ToString();
                    result.AppendChild(li);
                }

                if (LoopSectionEndFrame.HasValue)
                {
                    XmlElement li = parent.OwnerDocument.CreateElement("LoopSectionEndFrame");
                    li.InnerText = LoopSectionEndFrame.Value.ToString();
                    result.AppendChild(li);
                }

                if (LoopSectionStartFrame.HasValue)
                {
                    XmlElement li = parent.OwnerDocument.CreateElement("LoopSectionStartFrame");
                    li.InnerText = LoopSectionStartFrame.Value.ToString();
                    result.AppendChild(li);
                }

                if (RampDownFrames.HasValue)
                {
                    XmlElement li = parent.OwnerDocument.CreateElement("RampDownFrames");
                    li.InnerText = RampDownFrames.Value.ToString();
                    result.AppendChild(li);
                }

                if (RampUpFrames.HasValue)
                {
                    XmlElement ruf = parent.OwnerDocument.CreateElement("RampUpFrames");
                    ruf.InnerText = RampUpFrames.Value.ToString();
                    result.AppendChild(ruf);
                }

                parent.AppendChild(result);
            }
        }

        public override void AppendText(StringBuilder sb, int? index = null)
        {
            string tagName;
            sb.AppendUnixLine(Open("EffectControls"));

            if (LoopIterations.HasValue)
            {
                tagName = "LoopIterations";
                sb.AppendUnixLine(Open(tagName) + LoopIterations + Close(tagName));
            }

            if (LoopSectionEndFrame.HasValue)
            {
                tagName = "LoopSectionEndFrame";
                sb.AppendUnixLine(Open(tagName) + LoopSectionEndFrame + Close(tagName));
            }

            if (LoopSectionStartFrame.HasValue)
            {
                tagName = "LoopSectionStartFrame";
                sb.AppendUnixLine(Open(tagName) + LoopSectionStartFrame + Close(tagName));
            }

            if (RampDownFrames.HasValue)
            {
                tagName = "RampDownFrames";
                sb.AppendUnixLine(Open(tagName) + RampDownFrames + Close(tagName));
            }
            if (RampUpFrames.HasValue)
            {
                tagName = "RampUpFrames";
                sb.AppendUnixLine(Open(tagName) + RampUpFrames + Close(tagName));
            }
            sb.AppendUnixLine(Close("EffectControls"));
        }

        public override void LoadXml(XmlNode node)
        {
            XmlElement li = node.SelectSingleNode("LoopIterations") as XmlElement;
            if (li != null)
            {
                LoopIterations = int.Parse(li.InnerText);
            }
            XmlElement lsef = node.SelectSingleNode("LoopSectionEndFrame") as XmlElement;
            if (lsef != null)
            {
                LoopSectionEndFrame = int.Parse(lsef.InnerText);
            }
            XmlElement lssf = node.SelectSingleNode("LoopSectionStartFrame") as XmlElement;
            if (lssf != null)
            {
                LoopSectionStartFrame = int.Parse(lssf.InnerText);
            }
            XmlElement rdf = node.SelectSingleNode("RampDownFrames") as XmlElement;
            if (rdf != null)
            {
                RampDownFrames = int.Parse(rdf.InnerText);
            }
            XmlElement ruf = node.SelectSingleNode("RampUpFrames") as XmlElement;
            if (ruf != null)
            {
                RampUpFrames = int.Parse(ruf.InnerText);
            }
        }
    }

}
