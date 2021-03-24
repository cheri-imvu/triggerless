using System;
using System.Text;
using System.Xml;

namespace Triggerless.XAFLib
{
    [Serializable]

    public class ActionAttributes : IndexXmlGenerator {
        public ActionEnsemblePicking ActionEnsemblePicking { get; set; }
        public override void AddXml(XmlElement parent, int? index = null) {
            XmlElement el = parent.OwnerDocument.CreateElement("ActionEnsemblePicking");
            el.InnerText = ActionEnsemblePicking.ToString();
            parent.AppendChild(el);
        }

        public override void AppendText(StringBuilder sb, int? index = null) {
            sb.AppendUnixLine("<ActionAttributes>");
            sb.Append("<ActionEnsemblePicking>");
            sb.Append(ActionEnsemblePicking);
            sb.AppendUnixLine("</ActionEnsemblePicking>");
            sb.AppendUnixLine("</ActionAttributes>");
        }

        public override void LoadXml(XmlNode node) {
            XmlNode aep = node.SelectSingleNode("ActionEnsemblePicking");
            if (!string.IsNullOrWhiteSpace(aep?.InnerText)) {
                ActionEnsemblePicking = (ActionEnsemblePicking) Enum.Parse(typeof(ActionEnsemblePicking), aep.InnerText);
            }
        }
    }

}
