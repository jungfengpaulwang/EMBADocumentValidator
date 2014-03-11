using System.Xml;
using System.Text.RegularExpressions;

namespace EMBA.DocumentValidator
{
    public class RegexValidator : IFieldValidator
    {
        private Regex _regex;

        public RegexValidator(XmlElement XmlNode)
        {
            string pattern = string.Empty;

            foreach (XmlNode node in XmlNode.SelectSingleNode("ValidPattern").ChildNodes)
            {
                if (node is XmlCDataSection)
                {
                    pattern = (node as XmlCDataSection).Value;
                    break;
                }
            }

            _regex = new Regex(pattern, RegexOptions.Singleline);
        }

        public bool Validate(string Value)
        {
            return _regex.Match(Value).Success;
        }

        public string Correct(string Value)
        {
            return string.Empty;
        }

        public string ToString(string template)
        {
            return template;
        }
    }
}
