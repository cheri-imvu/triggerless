using System;
using System.Text;
using System.Xml;

namespace Triggerless.XAFLib
{
    [Serializable]

    public class ActionAttributes : IndexXmlGenerator {
        public ActionEnsemblePicking ActionEnsemblePicking { get; set; }
        public int? ActionTerminationIterations { get; set; }

        public override void AddXml(XmlElement parent, int? index = null) {
            XmlElement el = parent.OwnerDocument.CreateElement(nameof(ActionEnsemblePicking));
            el.InnerText = ActionEnsemblePicking.ToString();
            parent.AppendChild(el);

            if (ActionTerminationIterations != null )
            {
                el = parent.OwnerDocument.CreateElement(nameof(ActionTerminationIterations));
                el.InnerText = ActionTerminationIterations.Value.ToString();
                parent.AppendChild(el);
            }
        }

        public override void AppendText(StringBuilder sb, int? index = null) {
            sb.AppendUnixLine($"<{nameof(ActionAttributes)}>");
            sb.Append($"<{nameof(ActionEnsemblePicking)}>");
            sb.Append(ActionEnsemblePicking);
            sb.AppendUnixLine($"</{nameof(ActionEnsemblePicking)}>");
            if (ActionTerminationIterations != null )
            {
                sb.Append($"<{nameof(ActionTerminationIterations)}>");
                sb.Append(ActionTerminationIterations.Value.ToString());
                sb.AppendLine($"</{nameof(ActionTerminationIterations)}>");
            }
            sb.AppendUnixLine($"</{nameof(ActionAttributes)}>");
        }

        public override void LoadXml(XmlNode node) {
            XmlNode aep = node.SelectSingleNode(nameof(ActionEnsemblePicking));
            if (!string.IsNullOrWhiteSpace(aep?.InnerText)) {
                ActionEnsemblePicking = (ActionEnsemblePicking) Enum.Parse(typeof(ActionEnsemblePicking), aep.InnerText);
            }

            XmlNode ati = node.SelectSingleNode(nameof(ActionTerminationIterations));
            if (!string.IsNullOrWhiteSpace(ati?.InnerText))
            {
                ActionTerminationIterations = Convert.ToInt32(ati.InnerText);
            }
        }
    }

}
