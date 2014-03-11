using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Collections;

namespace EMBA.DocumentValidator
{
    public class XorCondition : IConditionExpression
    {
        /// <summary>
        /// 儲存所有 Sub Condition 的集合
        /// </summary>
        /// <remarks></remarks>
        private List<IConditionExpression> Expressions { get; set; }

        public XorCondition(XmlElement ConditionSource)
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
            List<bool> evaluates = new List<bool>();

            foreach (IConditionExpression Exp in Expressions)
                evaluates.Add(Exp.Evaluate(RowSource));

            bool evaluateResult = evaluates[0];
            for (int i = 1; i < evaluates.Count; i++)
                evaluateResult = evaluateResult ^ evaluates[i];

            return evaluateResult;
        }
    }
}