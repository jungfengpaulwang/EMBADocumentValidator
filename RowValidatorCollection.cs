using System.Xml;
using System.Collections.Generic;
using System.Collections;
using System;

namespace EMBA.DocumentValidator
{
    public class RowValidatorCollection : IEnumerable<IRowVaildator>
    {
        //存放集合物件的區域變數
        private Dictionary<string, IRowVaildator> Validators { get; set; }

        public RowValidatorCollection(XmlElement xmlNode)
        {
            Validators = new Dictionary<string, IRowVaildator>();

            if (xmlNode == null) throw new ArgumentException("RowValidator 資料不允許 Null。");

            foreach (XmlElement each in xmlNode.SelectNodes("RowValidator"))
            {
                IRowVaildator rowValidator = null;
                string validatorType = each.SelectSingleNode("@Type").InnerText.ToUpper();

                switch (validatorType)
                {
                    case "LOOKUP":
                        rowValidator = new LookupRowValidator(each);
                        break;
                    case "UNIQUEKEY":
                        rowValidator = new UniqueKeyRowValidator(each);
                        break;
                    default:

                        rowValidator = FactoryProvider.CreateRowValidator(validatorType, each);

                        if (rowValidator == null)
                            throw new ArgumentException(string.Format("找不到指定的 RowValidator 類型({0})。", validatorType));

                        break;
                }

                Add(each.SelectSingleNode("@Name").InnerText, rowValidator);
            }
        }

        private void Add(string validatorName, IRowVaildator RowValidator)
        {
            Validators.Add(validatorName, RowValidator);
        }

        public IRowVaildator this[string key]
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

        public IEnumerator GetEnumerator()
        {
            return Validators.Values.GetEnumerator();
        }

        #region IEnumerable<IRowVaildator> Members

        IEnumerator<IRowVaildator> IEnumerable<IRowVaildator>.GetEnumerator()
        {
            return Validators.Values.GetEnumerator();
        }

        #endregion
    }
}