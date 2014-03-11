using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace EMBA.DocumentValidator
{
    public class DuplicationRowValidator : IRowVaildator
    {
        public DuplicationRowValidator(XmlElement XmlNode)
        {
        }

        #region IRowVaildator Members

        public bool Validate(IRowStream Value)
        {
            throw new NotImplementedException();
        }

        public string Correct(IRowStream Value)
        {
            throw new NotImplementedException();
        }

        public string ToString(string template)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
