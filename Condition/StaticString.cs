using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace EMBA.DocumentValidator
{
    internal class StaticString : IConditionExpression
    {
        private string mValue;
        private string mOperator;
        private string mField;

        public StaticString(XmlElement xml)
        {
            mField = xml.SelectSingleNode("@Field").InnerText;
            mValue = xml.SelectSingleNode("@Value").InnerText;
            mOperator = xml.SelectSingleNode("@Operator").InnerText;
        }

        #region IConditionExpression Members

        public bool Evaluate(IRowStream rowSource)
        {
            string FieldValue = rowSource.GetValue(mField);

            switch (mOperator.ToUpper())
            {
                case "=":
                    return FieldValue == mValue;
                case "!=":
                case "<>":
                    return FieldValue != mValue;
                default:
                    return false;
            }
        }

        #endregion
    }
}
