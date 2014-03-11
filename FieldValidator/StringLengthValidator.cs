using System.Xml;

namespace EMBA.DocumentValidator
{
    public class StringLengthValidator : IFieldValidator
    {
        private short mMinLength;
        private short mMaxLength;
        private bool mMinInclusive;
        private bool mMaxInclusive;

        public StringLengthValidator(XmlElement XmlNode)
        {
            XmlHelper xml = new XmlHelper(XmlNode);

            mMinLength = xml.GetShort("MinLength", 0);
            mMaxLength = xml.GetShort("MaxLength", short.MaxValue);
            mMinInclusive = xml.GetBoolean("MinLength/@Inclusive", true);
            mMaxInclusive = xml.GetBoolean("MaxLength/@Inclusive", true);
        }

        public bool Validate(string Value)
        {
            int v = 0;

            v = Value.Length;

            if (mMinInclusive)
            {
                if (v < mMinLength)
                    return false;
            }
            else
            {
                if (v <= mMinLength)
                    return false;
            }

            if (mMaxInclusive)
            {
                if (v > mMaxLength)
                    return false;
            }
            else
            {
                if (v >= mMaxLength)
                    return false;
            }

            return true;
        }

        public string Correct(string Value)
        {
            return "";
        }

        public string ToString(string Description)
        {
            Description = Description.Replace("%MaxLength", mMaxLength.ToString());
            Description = Description.Replace("%MinLength", mMaxLength.ToString());

            return Description;
        }
    }
}