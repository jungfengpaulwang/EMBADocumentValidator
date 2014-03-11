using System.Xml;
using System.Collections.Generic;
using System;

namespace EMBA.DocumentValidator
{
    public class AndCondition : IConditionExpression
    {
        /// <summary>
        /// 儲存所有 Sub Condition 的集合
        /// </summary>
        /// <remarks></remarks>
        private List<IConditionExpression> Expressions { get; set; }

        public AndCondition(XmlElement ConditionSource)
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
            foreach (IConditionExpression Exp in Expressions)
                if (!Exp.Evaluate(RowSource)) return false;

            return true;
        }
    }
}