using System.Xml;
using System;

namespace EMBA.DocumentValidator
{
    public class ValidateStatement
    {
        public string Validator { get; private set; }
        public ErrorType ErrorType { get; private set; }
        public string Description { get; private set; }
        public bool AutoCorrect { get; private set; }
        public string When { get; private set; }

        public ValidateStatement(XmlElement statementDesc)
        {
            this.Validator = statementDesc.SelectSingleNode("@Validator").InnerText;
            ErrorType = ParseErrorType(statementDesc);
            this.Description = statementDesc.SelectSingleNode("@Description").InnerText;
            this.AutoCorrect = bool.Parse(statementDesc.SelectSingleNode("@AutoCorrect").InnerText);

            if (!string.IsNullOrEmpty(statementDesc.GetAttribute("When")))
            {
                this.When = statementDesc.GetAttribute("When");
            }
        }

        private ErrorType ParseErrorType(XmlElement XmlNode)
        {
            string strType = XmlNode.SelectSingleNode("@ErrorType").InnerText;

            if (strType.ToUpper() == "ERROR")
                return ErrorType.Error;
            else if (strType.ToUpper() == "WARNING")
                return ErrorType.Warning;
            else
                throw new ArgumentException("不支援此種 ErrorType：" + strType);
        }
    }
}