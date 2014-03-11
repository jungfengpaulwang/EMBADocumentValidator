using System.Xml;
using System.Globalization;
using System;

namespace EMBA.DocumentValidator
{
    public class DateValidator : IFieldValidator
    {
        private const string DateFormat = "yyyy/MM/dd";

        private DateTime mMinValue;
        private DateTime mMaxValue;
        private bool mMinInclusive;
        private bool mMaxInclusive;

        public DateValidator(XmlElement XmlNode)
        {
            XmlHelper xml = new XmlHelper(XmlNode);

            mMinValue = xml.GetDateTime("MinDate", DateTime.Now.AddYears(-100));
            mMaxValue = xml.GetDateTime("MaxDate", DateTime.Now.AddYears(100));

            mMinInclusive = xml.GetBoolean("MinDate/@Inclusive", true);
            mMaxInclusive = xml.GetBoolean("MaxDate/@Inclusive", true);
        }

        public bool Validate(string Value)
        {
            DateTime v = default(DateTime);

            bool result = false;

            if (!DateTime.TryParse(Value, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out v))
                return false;

            if (mMinInclusive)
            {
                if (v < mMinValue) return result;
            }
            else
            {
                if (v <= mMinValue) return result;
            }

            if (mMaxInclusive)
            {
                if (v > mMaxValue) return result;
            }
            else
            {
                if (v >= mMaxValue) return result;
            }

            return true;
        }

        public string Correct(string Value)
        {
            return "";
        }

        public string ToString(string Description)
        {
            Description = Description.Replace("%MaxDate", mMaxValue.ToString(DateFormat, DateTimeFormatInfo.InvariantInfo));
            Description = Description.Replace("%MinDate", mMaxValue.ToString(DateFormat, DateTimeFormatInfo.InvariantInfo));

            return Description;
        }
    }
}