using System.Xml;
using System.Collections.Generic;

namespace EMBA.DocumentValidator
{
    public class UniqueKeyRowValidator : IRowVaildator
    {
        private IList<string> mKeyFieldCol;
        private Dictionary<string, string> mKey;
        private const string Sep = "※＆＃";

        public UniqueKeyRowValidator(XmlElement XmlNode)
        {
            mKey = new Dictionary<string, string>();
            mKeyFieldCol = new List<string>();

            foreach (XmlElement n in XmlNode.SelectNodes("KeyField"))
                mKeyFieldCol.Add(n.InnerText);
        }

        public string KeyField
        {
            get { return mKeyFieldCol[0]; }
        }

        public bool Validate(IRowStream Value)
        {
            bool functionReturnValue = false;

            functionReturnValue = false;

            // ERROR: Not supported in C#: OnErrorStatement

            string ComplexKey = string.Empty;
            foreach (var kv in mKeyFieldCol)
            {
                ComplexKey = ComplexKey + Value.GetValue(kv) + Sep;
            }

            mKey.Add(ComplexKey, "");

            functionReturnValue = true;

            return functionReturnValue;
        }

        public string Correct(IRowStream Value)
        {
            return "<Result/>";
        }

        public string ToString(string Description)
        {
            return Description;
        }
    }
}