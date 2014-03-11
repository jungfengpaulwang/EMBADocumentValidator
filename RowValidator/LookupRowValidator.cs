using System.Xml;

namespace EMBA.DocumentValidator
{
    public class LookupRowValidator : IRowVaildator
    {
        private EnumerationValidator mevKeyValue;
        private string mKeyFieldName;
        private string mValueFieldName;

        public LookupRowValidator(XmlElement XmlNode)
        {
            mevKeyValue = new EnumerationValidator(XmlNode.SelectSingleNode("Enumeration") as XmlElement);
            mKeyFieldName = XmlNode.SelectSingleNode("KeyField").InnerText;
            mValueFieldName = XmlNode.SelectSingleNode("ValueField").InnerText;
        }

        //public string KeyField
        //{
        //    get { return mKeyFieldName; }
        //}

        public bool Validate(IRowStream Value)
        {
            string KeyData = null;
            string ValueData = null;

            KeyData = Value.GetValue(mKeyFieldName);
            ValueData = Value.GetValue(mValueFieldName);

            if (string.IsNullOrEmpty(KeyData) | string.IsNullOrEmpty(ValueData)) return false;

            return (mevKeyValue.GetCode(ValueData) == KeyData);
        }

        public string Correct(IRowStream Value)
        {
            string KeyData = null;
            string ValueData = null;
            string KeyCorrect = null;
            string ValueCorrect = null;

            KeyData = Value.GetValue(mKeyFieldName);
            ValueData = Value.GetValue(mValueFieldName);

            KeyCorrect = "";
            ValueCorrect = "";

            if (string.IsNullOrEmpty(KeyData))
            {
                if (!string.IsNullOrEmpty(ValueData))
                {
                    KeyCorrect = mevKeyValue.GetCode(ValueData);
                    if (string.IsNullOrEmpty(KeyCorrect))
                    {
                        ValueCorrect = mevKeyValue.Correct(ValueData);
                        if (!string.IsNullOrEmpty(ValueCorrect))
                        {
                            KeyCorrect = mevKeyValue.GetCode(ValueCorrect);
                        }
                    }
                }
            }
            else
                ValueCorrect = mevKeyValue.GetName(KeyData);

            return "<Result>" + (string.IsNullOrEmpty(KeyCorrect) ? "" : "<" + mKeyFieldName + ">" + KeyCorrect + "</" + mKeyFieldName + ">") + (string.IsNullOrEmpty(ValueCorrect) ? "" : "<" + mValueFieldName + ">" + ValueCorrect + "</" + mValueFieldName + ">") + "</Result>";
        }

        public string ToString(string Description)
        {
            return Description;
        }
    }
}