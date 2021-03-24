using System;
using System.Text;
using System.Xml;

namespace Triggerless.XAFLib
{
    [Serializable]

    public abstract class IndexXmlGenerator {
        public abstract void AddXml(XmlElement parent, int? index = null);
        public abstract void AppendText(StringBuilder sb, int? index = null);
        public abstract void LoadXml(XmlNode node);
        protected string Open(string thing)
        {
            return "<" + thing + ">";
        }
        protected string Close(string thing)
        {
            return "</" + thing + ">";
        }
    }

}
