using System;
using System.Xml;

namespace EMBA.DocumentValidator
{
    public class Condition
    {
        private IConditionExpression Expression { get; set; }

        public Condition(XmlElement xmlNode)
        {
            if (CountSubElement(xmlNode) != 1)
                throw new ArgumentException(string.Format("初始化 Condition 錯誤，必須只有一個 Sub Element。\n\n", xmlNode.OuterXml));

            XmlElement FirstChild = GetFirstChildElement(xmlNode);

            Expression = BuildExpression(FirstChild);
        }

        internal static IConditionExpression BuildExpression(XmlElement evaluateDescription)
        {
            switch (evaluateDescription.LocalName)
            {
                case "And":
                    return new AndCondition(evaluateDescription);

                case "Or":
                    return new OrCondition(evaluateDescription);

                case "Not":
                    return new NotCondition(evaluateDescription);

                case "Xor":
                    return new XorCondition(evaluateDescription);

                case "Evaluate":
                    string evaluator = evaluateDescription.GetAttribute("Evaluator");

                    IConditionExpression expression = null;

                    switch (evaluator.ToUpper())
                    {
                        case "STATICSTRING": //StaticString
                            expression = new StaticString(evaluateDescription);
                            break;

                        case "STATICINTEGER": //StaticInteger
                            expression = new StaticInteger(evaluateDescription);
                            break;

                        case "STATICDATETIME": //StaticDateTime
                            expression = new StaticInteger(evaluateDescription);
                            break;

                        default:
                            expression = FactoryProvider.CreateConditionExpression(evaluator, evaluateDescription);
                            break;
                    }


                    if (expression == null)
                        throw new ArgumentException("指定的 Evaluator 找不到：" + evaluator);

                    return expression;

                default:
                    throw new ArgumentException(string.Format("不支援此種運算類型({0})。", evaluateDescription.LocalName));
            }
        }

        public bool Evaluate(IRowStream RowSource)
        {
            return Expression.Evaluate(RowSource);
        }

        private int CountSubElement(XmlElement em)
        {
            int count = 0;

            foreach (XmlNode eachem in em.ChildNodes)
            {
                if (eachem.NodeType == XmlNodeType.Element)
                {
                    count += 1;
                }
            }
            return count;
        }

        public XmlElement GetFirstChildElement(XmlElement em)
        {
            foreach (XmlNode eachem in em.ChildNodes)
            {
                if (eachem.NodeType == XmlNodeType.Element)
                    return eachem as XmlElement;
            }
            return null;
        }
    }
}