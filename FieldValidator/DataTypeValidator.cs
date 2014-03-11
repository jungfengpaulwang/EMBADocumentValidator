using System.Xml;
using System.Globalization;
using System;

namespace EMBA.DocumentValidator
{
    public class DataTypeValidator : IFieldValidator
    {
        private string mTypeName;

        public DataTypeValidator(XmlElement XmlNode)
        {
            mTypeName = XmlNode.SelectSingleNode("Type").InnerText;
        }

        public bool Validate(string Value)
        {
            switch (mTypeName.ToUpper())
            {
                case "INTEGER":
                    int numValue;
                    return int.TryParse(Value, out numValue);

                case "DECIMAL":
                    decimal val;
                    return decimal.TryParse(Value, out val);

                case "DATE":
                    return (ParseGEDate(Value).HasValue);

                case "CDATE":
                    return (ParseTWDate(Value).HasValue);
            }

            return false;
        }

        public string Correct(string Value)
        {
            return "";
        }

        public string ToString(string Description)
        {
            return Description;
        }

        private Nullable<DateTime> ParseTWDate(string Value)
        {
            string[] v = null;

            try
            {
                v = Value.Split('/');
                return ParseGEDate((int.Parse(v[0]) + 1911).ToString() + "/" + v[1] + "/" + v[2]);
            }
            catch
            {
                return null;
            }
        }

        private Nullable<DateTime> ParseGEDate(string value)
        {
            DateTime result = default(DateTime);

            try
            {
                if (DateTime.TryParse(value, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out result))
                    return result;
                else
                    return null;
            }
            catch
            {
                return null;
            }
        }
    }
}