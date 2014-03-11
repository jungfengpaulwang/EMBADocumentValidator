using System.Xml;

namespace EMBA.DocumentValidator
{
    /// <summary>
    /// 欄位描述
    /// </summary>
    public class FieldDescription
    {
        /// <summary>
        /// 欄位名稱
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// 取得當欄位沒有資料時，是否還需要執行資料驗證。
        /// </summary>
        public bool EmptyAlsoValidate { get; protected set; }

        /// <summary>
        /// 欄位是否為必填
        /// </summary>
        public bool Required { get; protected set; }

        /// <summary>
        /// 欄位驗證規則
        /// </summary>
        private ValidateStatements mValidateStatements;

        public FieldDescription()
        {
 
        }

        public FieldDescription(XmlElement XmlNode)
        {
            XmlHelper xml = new XmlHelper(XmlNode);

            Name = xml.GetString("@Name");

            EmptyAlsoValidate = xml.GetBoolean("@EmptyAlsoValidate", true);
            Required = xml.GetBoolean("@Required", false);

            mValidateStatements = new ValidateStatements(XmlNode);
        }

        public ValidateStatements ValidateStatements
        {
            get { return mValidateStatements; }
        }
    }
}