using System.Xml;
using System.Collections.Generic;
using System.IO;
using System;

namespace EMBA.DocumentValidator
{
    public class FieldValidatorCollection : IEnumerable<IFieldValidator>
    {
        //存放集合物件的區域變數
        private Dictionary<string, IFieldValidator> Validators { get; set; }

        public FieldValidatorCollection(XmlElement xmlNode)
        {
            Validators = new Dictionary<string, IFieldValidator>();

            if (xmlNode == null) throw new ArgumentException("FieldValidator 資料不允許 Null。");

            foreach (XmlElement each in xmlNode.SelectNodes("FieldValidator"))
            {
                IFieldValidator fieldValidator = null;
                string validatorType = each.SelectSingleNode("@Type").InnerText.ToUpper();

                switch (validatorType)
                {
                    case "ENUMERATION":
                        fieldValidator = new EnumerationValidator(each);
                        break;
                    case "STRINGLENGTH":
                        fieldValidator = new StringLengthValidator(each);
                        break;
                    //case "CDATE": //核心暫時不支援。
                    //    valid = new CDateValidator(n);
                    //    break;
                    case "DATE":
                        fieldValidator = new DateValidator(each);
                        break;
                    case "MIXDATE":
                        fieldValidator = new MixDateValidator(each);
                        break;
                    case "DATATYPE":
                        fieldValidator = new DataTypeValidator(each);
                        break;
                    case "INTEGER":
                        fieldValidator = new IntegerValidator(each);
                        break;
                    case "DECIMAL":
                        fieldValidator = new DecimalValidator(each);
                        break;
                    case "UNIQUEKEY":
                        fieldValidator = new UniqueKeyValidator();
                        break;
                    case "REGEX":
                        fieldValidator = new RegexValidator(each);
                        break;
                    default:

                        fieldValidator = FactoryProvider.CreateFieldValidator(validatorType, each);

                        if (fieldValidator == null)
                            throw new ArgumentException("指定的 Validator 找不到：" + validatorType);

                        break;
                }

                Add(each.SelectSingleNode("@Name").InnerText, fieldValidator);
            }
        }

        private void Add(string validatorName, IFieldValidator FieldValidator)
        {
            Validators.Add(validatorName, FieldValidator);
        }

        public IFieldValidator this[string key]
        {
            get { return Validators[key]; }
        }

        public int Count
        {
            get { return Validators.Count; }
        }

        public bool Contains(string validatorName)
        {
            return Validators.ContainsKey(validatorName);
        }

        public System.Collections.IEnumerator GetEnumerator()
        {
            return Validators.Values.GetEnumerator();
        }

        IEnumerator<IFieldValidator> IEnumerable<IFieldValidator>.GetEnumerator()
        {
            return Validators.Values.GetEnumerator();
        }
    }
}