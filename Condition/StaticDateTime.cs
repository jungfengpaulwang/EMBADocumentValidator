using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Globalization;

namespace EMBA.DocumentValidator
{
    internal class StaticDateTime : IConditionExpression
    {
        private string mValue;
        private string mOperator;
        private string mField;

        public StaticDateTime(XmlElement xml)
        {
            mField = xml.SelectSingleNode("@Field").InnerText;
            mValue = xml.SelectSingleNode("@Value").InnerText;
            mOperator = xml.SelectSingleNode("@Operator").InnerText;
        }

        #region IConditionExpression Members

        public bool Evaluate(IRowStream rowSource)
        {
            #region DATE
            string FieldValue = rowSource.GetValue(mField);

            Nullable<DateTime> t1 = default(Nullable<DateTime>);
            Nullable<DateTime> t2 = default(Nullable<DateTime>);

            bool Result = false;

            switch (mOperator.Trim())
            {
                case "=": //Equal
                    t1 = ParseDate(FieldValue);
                    t2 = ParseDate(mValue);

                    if ((!t1.HasValue) & (!t2.HasValue))
                    {
                        Result = true;
                    }
                    else if (t1.HasValue & t2.HasValue)
                    {
                        Result = (t1.Value == t2.Value);
                    }
                    else if (t1.HasValue | t2.HasValue)
                    {
                        Result = false;
                    }

                    break;
                case "!=": //NotEqual
                case "<>":
                    t1 = ParseDate(FieldValue);
                    t2 = ParseDate(mValue);

                    if ((!t1.HasValue) & (!t2.HasValue))
                    {
                        Result = false;
                    }
                    else if (t1.HasValue & t2.HasValue)
                    {
                        Result = (t1.Value != t2.Value);
                    }
                    else if (t1.HasValue | t2.HasValue)
                    {
                        Result = true;
                    }
                    break;

                case ">": //MoreThen
                    throw new NotImplementedException();

                case "<": //LessThen
                    throw new NotImplementedException();

                case ">=": //MoreThenEqual
                    throw new NotImplementedException();

                case "<=": //LessThenEqual
                    throw new NotImplementedException();

                default:
                    Result = false;
                    break;
            }

            return Result;
            #endregion
        }

        private Nullable<DateTime> ParseDate(string Value)
        {
            try
            {
                DateTime result = default(DateTime);

                if (DateTime.TryParse(Value, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out result))
                    return result;
                else
                    return null;
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
