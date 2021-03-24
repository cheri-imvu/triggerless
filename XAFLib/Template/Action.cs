using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Triggerless.XAFLib
{
    [Serializable]
    public class Action: IndexXmlGenerator {
        public Definition Definition { get; }
        public string Name { get; set; }
        public Action() {
            Definition = new Definition();
        }

        public Action Clone()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<Container />");
            AddXml(doc.DocumentElement, 0);

            var clone = new Action();
            clone.LoadXml(doc.DocumentElement.FirstChild);
            return clone;
        }

        public override void AddXml(XmlElement parent, int? index = null) {
            if (parent == null || !index.HasValue) return;
            if (parent.OwnerDocument != null) {
                XmlElement result = parent.OwnerDocument.CreateElement("Action" + index);
                Definition.AddXml(result);
                XmlElement name = parent.OwnerDocument.CreateElement("Name");
                name.InnerText = Name;
                result.AppendChild(name);
                parent.AppendChild(result);
            }
        }

        public override void AppendText(StringBuilder sb, int? index = null) {
            sb.AppendUnixLine(Open("Action" + index));
            Definition.AppendText(sb);
            sb.AppendUnixLine(Open("Name") + Name + Close("Name"));
            sb.AppendUnixLine(Close("Action" + index));
        }

        public override void LoadXml(XmlNode node) {
            XmlElement nameEl = node.SelectSingleNode("Name") as XmlElement;
            if (nameEl != null) Name = nameEl.InnerText;
            XmlNode d = node.SelectSingleNode("Definition");
            if (d != null) Definition.LoadXml(d);
        }
    }

}
