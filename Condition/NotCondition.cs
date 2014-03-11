using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace EMBA.DocumentValidator
{
    public class NotCondition : IConditionExpression
    {
        /// <summary>
        /// 儲存所有 Sub Condition 的集合
        /// </summary>
        /// <remarks></remarks>
        private IConditionExpression Expression { get; set; }

        public NotCondition(XmlElement ConditionSource)
        {
            int count = 0;

            foreach (XmlNode each in ConditionSource.ChildNodes)
            {
                XmlElement elm = each as XmlElement;
                if (elm == null) continue;

                if (count > 0) throw new ArgumentException("NotCondition 只允許一個運算元。");

                Expression = Condition.BuildExpression(elm);
                count++;
            }
        }

        public bool Evaluate(IRowStream RowSource)
        {
            return (!Expression.Evaluate(RowSource));
        }
    }
}
