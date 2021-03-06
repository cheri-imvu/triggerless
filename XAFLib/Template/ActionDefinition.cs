using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace XAFLib
{
    [Serializable]

    public class ActionDefinition : IndexXmlGenerator
    {
        public List<EnsembleDefintion> Ensembles { get; }
        public ActionAttributes ActionAttributes { get; }

        public ActionDefinition() {
            Ensembles = new List<EnsembleDefintion>();
            ActionAttributes = new ActionAttributes();
        }

        public override void AddXml(XmlElement parent, int? index = null) {
            if (parent?.OwnerDocument != null) {
                XmlElement result = parent.OwnerDocument.CreateElement("ActionDefinition");

                ActionAttributes.AddXml(result);
                for (int i = 0; i < Ensembles.Count; i++) {
                    EnsembleDefintion ed = Ensembles[i];
                    ed.AddXml(result, i);
                }
                parent.AppendChild(result);
            }
        }

        public override void AppendText(StringBuilder sb, int? index = null) {
            sb.AppendUnixLine(Open("ActionDefinition"));
            ActionAttributes.AppendText(sb);
            for (int i = 0; i < Ensembles.Count; i++ ) {
                Ensembles[i].AppendText(sb, i);
            }
            sb.AppendUnixLine(Close("ActionDefinition"));
        }

        public override void LoadXml(XmlNode node) {
            XmlNode aa = node.SelectSingleNode("ActionAttributes");
            if (aa != null) ActionAttributes.LoadXml(aa);

            foreach (XmlNode childNode in node.ChildNodes) {
                if (childNode.Name.StartsWith("EnsembleDefinition")) {
                    EnsembleDefintion ed = new EnsembleDefintion();
                    ed.LoadXml(childNode);
                    Ensembles.Add(ed);
                }
            }
        }
    }

}
