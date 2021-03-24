using System;
using System.Text;
using System.Xml;

namespace Triggerless.XAFLib
{
    [Serializable]
    public class Definition : IndexXmlGenerator
    {
        public ActionDefinition ActionDefinition { get; }
        public ActionType? ActionType { get; set; }
        public Definition() {
            ActionDefinition = new ActionDefinition();
        }

        public override void AddXml(XmlElement parent, int? index = null) {
            if (parent?.OwnerDocument != null) {
                XmlElement result = parent.OwnerDocument.CreateElement("Definition");
                ActionDefinition.AddXml(result);
                if (ActionType.HasValue) {

                    XmlElement at = parent.OwnerDocument.CreateElement("AssetType");
                    at.InnerText = ActionType.Value.ToString();
                    result.AppendChild(at);
                }
                parent.AppendChild(result);
            }
        }

        public override void AppendText(StringBuilder sb, int? index = null) {
            sb.AppendUnixLine(Open("Definition"));
            ActionDefinition.AppendText(sb);
            if (ActionType.HasValue) sb.AppendUnixLine(Open("ActionType") + ActionType + Close("ActionType"));
            sb.AppendUnixLine(Close("Definition"));
        }

        public override void LoadXml(XmlNode node) {
            XmlNode at = node.SelectSingleNode("ActionType");
            if (!string.IsNullOrWhiteSpace(at?.InnerText)) {
                //Console.WriteLine("*" + at.InnerText + "*");
                ActionType = (ActionType)Enum.Parse(typeof(ActionType), at.InnerText);
            }

            ActionDefinition.LoadXml(node.ChildNodes[0]);
        }
    }

}
