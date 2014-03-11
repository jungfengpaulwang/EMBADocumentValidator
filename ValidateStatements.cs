using System.Xml;
using System.Collections.Generic;

namespace EMBA.DocumentValidator
{
    public class ValidateStatements : IEnumerable<ValidateStatement>
    {
        //存放集合物件的區域變數
        private IList<ValidateStatement> mCol;

        public ValidateStatements(XmlElement XmlNode)
        {
            mCol = new List<ValidateStatement>();

            if (XmlNode == null) return;

            ValidateStatement vs = default(ValidateStatement);
            foreach (XmlElement n in XmlNode.SelectNodes("Validate"))
            {
                vs = new ValidateStatement(n);
                mCol.Add(vs);
            }
        }

        public ValidateStatement Add(ValidateStatement ValidState)
        {
            mCol.Add(ValidState);
            return ValidState;
        }

        public ValidateStatement this[int index]
        {
            get { return mCol[index]; }
        }

        public int Count
        {
            get { return mCol.Count; }
        }

        public System.Collections.IEnumerator GetEnumerator()
        {
            return mCol.GetEnumerator();
        }

        public IEnumerator<ValidateStatement> GetEnumeratorGeneric()
        {
            return mCol.GetEnumerator();
        }

        IEnumerator<ValidateStatement> IEnumerable<ValidateStatement>.GetEnumerator()
        {
            return GetEnumeratorGeneric();
        }
    }
}