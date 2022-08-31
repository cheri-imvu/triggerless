using System;
using System.Text;
using System.Xml;

namespace Triggerless.XAFLib
{
    [Serializable]
    public class EnsembleAttributes : IndexXmlGenerator
    {

        public ActionEnsembleTermination ActionEnsembleTermination { get; set; }
        public ActionPerturbationRange ActionPerturbationRange { get; set; }
        public EffectAdjustment EffectAdjustment { get; set; }
        public float? EnsembleRelativeLikelihood { get; set; }
        public bool? disablesGaze { get; set; }

        public override void AddXml(XmlElement parent, int? index = null)
        {
            var result = parent.OwnerDocument.CreateElement(nameof(EnsembleAttributes));

            if (ActionEnsembleTermination != ActionEnsembleTermination.NotUsed)
            {
                var elAET = parent.OwnerDocument.CreateElement(nameof(ActionEnsembleTermination));
                elAET.InnerText = ActionEnsembleTermination.ToString();
                result.AppendChild(elAET);
            }

            if (ActionPerturbationRange != null)
            {
                ActionPerturbationRange.AddXml(result);
            }

            if (EffectAdjustment != null)
            {
                EffectAdjustment.AddXml(result);
            }

            if (EnsembleRelativeLikelihood.HasValue)
            {
                var elERL = parent.OwnerDocument.CreateElement(nameof(EnsembleRelativeLikelihood));
                elERL.InnerText = EnsembleRelativeLikelihood.Value.ToString();
                result.AppendChild(elERL);
            }

            if (disablesGaze.HasValue)
            {
                var elDG = parent.OwnerDocument.CreateElement(nameof(disablesGaze));
                elDG.InnerText = disablesGaze.Value ? "1" : "0";
                result.AppendChild(elDG);
            }


            parent.AppendChild(result);
        }

        public override void AppendText(StringBuilder sb, int? index = null)
        {
            var tagname = nameof(EnsembleAttributes);
            sb.AppendUnixLine(Open(tagname));
            if (ActionEnsembleTermination != ActionEnsembleTermination.NotUsed)
            {
                var name = nameof(ActionEnsembleTermination);
                sb.AppendUnixLine($"<{name}>{ActionEnsembleTermination}</{name}>");
            }
            if (ActionPerturbationRange != null) ActionPerturbationRange.AppendText(sb);
            if (EffectAdjustment != null) EffectAdjustment.AppendText(sb);

            if (EnsembleRelativeLikelihood.HasValue)
            {
                var name = nameof(EnsembleRelativeLikelihood);
                sb.AppendUnixLine($"<{name}>{EnsembleRelativeLikelihood}</{name}>");

            }

            if (disablesGaze.HasValue)
            {
                var name = nameof(disablesGaze);
                var gaze = disablesGaze.Value ? "1" : "0";
                sb.AppendUnixLine($"<{name}>{gaze}</{name}>");
            }
            sb.AppendUnixLine(Close(tagname));
        }

        public override void LoadXml(XmlNode node)
        {
            XmlElement el = node.SelectSingleNode(nameof(ActionEnsembleTermination)) as XmlElement;
            if (el != null && !string.IsNullOrWhiteSpace(el.InnerText))
            {
                ActionEnsembleTermination = (ActionEnsembleTermination)Enum.Parse(typeof(ActionEnsembleTermination), el.InnerText);
            }

            el = node.SelectSingleNode(nameof(ActionPerturbationRange)) as XmlElement;
            if (el != null)
            {
                ActionPerturbationRange = new ActionPerturbationRange();
                ActionPerturbationRange.LoadXml(el);
            }

            el = node.SelectSingleNode(nameof(EffectAdjustment)) as XmlElement;
            if (el != null)
            {
                EffectAdjustment = new EffectAdjustment();
                EffectAdjustment.LoadXml(el);
            }

            el = node.SelectSingleNode(nameof(EnsembleRelativeLikelihood)) as XmlElement;
            if (el != null)
            {
                if (float.TryParse(el.InnerText, out float val)) EnsembleRelativeLikelihood = val;
            }

            el = node.SelectSingleNode(nameof(disablesGaze)) as XmlElement;
            if (el != null)
            {
                if (el.InnerText == "1") disablesGaze = true;
                if (el.InnerText == "0") disablesGaze = false;
            } else
            {
                disablesGaze = null;
            }
        }
    }

}
