using System.Xml;

namespace EMBA.DocumentValidator
{
    public class DecimalValidator : IFieldValidator
    {
        private decimal mMinValue;
        private decimal mMaxValue;
        private bool mMinInclusive;
        private bool mMaxInclusive;

        public DecimalValidator(XmlElement XmlNode)
        {
            XmlHelper xml = new XmlHelper(XmlNode);

            mMinValue = xml.GetInteger("MinValue", 0);
            mMaxValue = xml.GetInteger("MaxValue", int.MaxValue);

            mMinInclusive = xml.GetBoolean("MinValue/@Inclusive", true);
            mMaxInclusive = xml.GetBoolean("MaxValue/@Inclusive", true);
        }

        public bool Validate(string Value)
        {
            bool result = false;

            decimal val;

            if (!decimal.TryParse(Value, out val)) return result;

            if (mMinInclusive)
            {
                if (val < mMinValue) return result;
            }
            else
            {
                if (val <= mMinValue) return result;
            }

            if (mMaxInclusive)
            {
                if (val > mMaxValue) return result;
            }
            else
            {
                if (val >= mMaxValue) return result;
            }

            return true;
        }

        public string Correct(string Value)
        {
            return "";
        }

        public string ToString(string Description)
        {
            Description = Description.Replace("%MaxValue", mMaxValue.ToString());
            Description = Description.Replace("%MinValue", mMinValue.ToString());

            return Description;
        }
    }
}