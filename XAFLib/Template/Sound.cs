using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Triggerless.XAFLib
{
    public class Sound : IndexXmlGenerator
    {
        public string Name { get; set; }
        public override void AddXml(XmlElement parent, int? index = null)
        {
            if (string.IsNullOrWhiteSpace(Name)) return; // don't add the sound XML
            var sound = parent.OwnerDocument.CreateElement("Sound");
            var name = parent.OwnerDocument.CreateElement("Name");
            name.InnerText = Name;
            sound.AppendChild(name);
            parent.AppendChild(sound);
        }

        public override void AppendText(StringBuilder sb, int? index = null)
        {
            if (string.IsNullOrWhiteSpace(Name)) return; // don't add the sound XML
            string tagName = "Sound" + index;
            sb.AppendUnixLine(Open(tagName));
            sb.AppendUnixLine($"<Name>{Name}</Name>");
            sb.AppendUnixLine(Close(tagName));
        }

        public override void LoadXml(XmlNode node)
        {
            var name = node.SelectSingleNode("Name");
            if (name != null) Name = name.InnerText;
        }
    }
}
