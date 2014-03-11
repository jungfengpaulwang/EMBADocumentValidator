using System.Xml;
using System.Collections.Generic;

namespace EMBA.DocumentValidator
{
    public class UniqueKeyValidator : IFieldValidator
    {
        const string ERROR_Source = "UniqueKeyValidator";

        private Dictionary<string, string> mKey;

        public UniqueKeyValidator()
            : base()
        {
            mKey = new Dictionary<string, string>();
        }

        public string Correct(string Value)
        {
            return "";
        }

        public bool Validate(string Value)
        {
            bool functionReturnValue = false;
            functionReturnValue = true;

            if (mKey.ContainsKey(Value))
            {
                functionReturnValue = false;
            }
            else
            {
                mKey.Add(Value, string.Empty);
            }

            return functionReturnValue;
        }

        public string ToString(string Description)
        {
            return Description;
        }
    }
}