using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace EMBA.DocumentValidator
{
    public interface IConditionFactory
    {
        IConditionExpression CreateConditionExpression(string TypeName, XmlElement condDescription);
    }
}
