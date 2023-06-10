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
        public Sound Sound { get; set; }
        public Action() {
            Definition = new Definition();
            Sound = new Sound();
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
                XmlElement result = parent.OwnerDocument.CreateElement(nameof(Action) + index);

                if (Definition.ActionDefinition.Ensembles.Count > 0)
                {
                    Definition.AddXml(result);
                }

                XmlElement name = parent.OwnerDocument.CreateElement(nameof(Name));
                name.InnerText = Name;
                result.AppendChild(name);

                if (!string.IsNullOrWhiteSpace(Sound.Name))
                {
                    Sound.AddXml(result);
                }
                parent.AppendChild(result);
            }
        }

        public override void AppendText(StringBuilder sb, int? index = null) {
            sb.AppendUnixLine(Open(nameof(Action) + index));
            Definition.AppendText(sb);
            Sound.AppendText(sb);
            sb.AppendUnixLine(
                Open(nameof(Name)) + Name + Close(nameof(Name))
            );
            sb.AppendUnixLine(Close(nameof(Action) + index));
        }

        public override void LoadXml(XmlNode node) {
            XmlElement nameEl = node.SelectSingleNode("Name") as XmlElement;
            if (nameEl != null) Name = nameEl.InnerText;
            XmlNode d = node.SelectSingleNode("Definition");
            if (d != null) Definition.LoadXml(d);
            XmlElement sound = node.SelectSingleNode("Sound") as XmlElement;
            if (sound != null) Sound.LoadXml(sound);
        }
    }

}
