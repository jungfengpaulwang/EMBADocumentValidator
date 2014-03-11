using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace EMBA.DocumentValidator
{
    public static class FactoryProvider
    {
        public static List<IRowValidatorFactory> RowFactory { get; private set; }

        public static List<IFieldValidatorFactory> FieldFactory { get; private set; }

        public static List<IConditionFactory> ConditionFactory { get; private set; }

        static FactoryProvider()
        {
            RowFactory = new List<IRowValidatorFactory>();
            FieldFactory = new List<IFieldValidatorFactory>();
            ConditionFactory = new List<IConditionFactory>();
        }

        internal static IRowVaildator CreateRowValidator(string name, XmlElement validatorDescription)
        {
            IRowVaildator result = null;
            foreach (var each in RowFactory)
            {
                result = each.CreateRowValidator(name, validatorDescription);
                if ((result != null)) break;
            }

            return result;
        }

        internal static IFieldValidator CreateFieldValidator(string name, XmlElement validatorDescription)
        {
            IFieldValidator result = null;
            foreach (var each in FieldFactory)
            {
                result = each.CreateFieldValidator(name, validatorDescription);
                if ((result != null)) break;
            }

            return result;
        }

        internal static IConditionExpression CreateConditionExpression(string name, XmlElement condDescription)
        {
            IConditionExpression result = null;
            foreach (var each in ConditionFactory)
            {
                result = each.CreateConditionExpression(name, condDescription);
                if ((result != null)) break;
            }

            return result;
        }
    }
}
