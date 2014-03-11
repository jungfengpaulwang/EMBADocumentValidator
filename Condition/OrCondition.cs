using System.Xml;
using System;
using System.Collections.Generic;

namespace EMBA.DocumentValidator
{
    public class OrCondition : IConditionExpression
    {
        private List<IConditionExpression> Expressions { get; set; }

        public OrCondition(XmlElement ConditionSource)
        {
            Expressions = new List<IConditionExpression>();

            foreach (XmlNode each in ConditionSource.ChildNodes)
            {
                XmlElement elm = each as XmlElement;
                if (elm == null) continue;

                Expressions.Add(Condition.BuildExpression(elm));
            }
        }

        public bool Evaluate(IRowStream RowSource)
        {
            foreach (IConditionExpression each in Expressions)
                if (each.Evaluate(RowSource)) return true;

            return false;
        }
    }
}