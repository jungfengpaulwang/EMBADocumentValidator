using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace EMBA.DocumentValidator
{
    internal class StaticInteger : IConditionExpression
    {
        private string mValue;
        private string mOperator;
        private string mField;

        public StaticInteger(XmlElement xml)
        {
            mField = xml.SelectSingleNode("@Field").InnerText;
            mValue = xml.SelectSingleNode("@Value").InnerText;
            mOperator = xml.SelectSingleNode("@Operator").InnerText;
        }

        #region IConditionExpression Members

        public bool Evaluate(IRowStream rowSource)
        {
            #region INTEGER
            string FieldValue = rowSource.GetValue(mField);

            int src = 0;
            int dest = 0;

            if (mValue == string.Empty)
            {
                src = int.MinValue;
            }
            else
            {
                src = int.Parse(mValue);
            }

            if (FieldValue == string.Empty)
            {
                dest = int.MinValue;
            }
            else
            {
                if (!int.TryParse(FieldValue, out dest))
                {
                    dest = int.MaxValue;
                }
            }

            switch (mOperator.Trim())
            {
                case "=":
                    return (dest == src);
                case ">":
                    return (dest > src);
                case ">=":
                    return (dest >= src);
                case "<":
                    return (dest < src);
                case "<=":
                    return (dest <= src);
                case "!=":
                case "<>":
                    return (dest != src);
                default:
                    return false;
            }
            #endregion
        }

        #endregion
    }
}
