using System.Xml;
using System;
using System.Collections.Generic;
using System.Data;

namespace EMBA.DocumentValidator
{
    /// <summary>
    /// 資料驗證主要類別
    /// </summary>
    public class DocumentValidate
    {
        private FieldValidatorCollection FieldValidators { get; set; }

        private RowValidatorCollection RowValidators { get; set; }

        private ConditionCollection Conditions { get; set; }

        private FieldValidate FieldValidate { get; set; }

        private RowValidate RowValidate { get; set; }

        private DuplicateDetection Duplication { get; set; }

        //  儲存資料庫中的資料於 DataTable，用於驗證資料是否存在於資料庫
        //private DataSet DataSources { get; set; }

        //  資料轉換：Key 轉 Value
        //private List<object> MappingTables { get; set; }

        /// <summary>
        /// 當一個驗證出錯誤資料時。
        /// </summary>
        public event EventHandler<ErrorCapturedEventArgs> ErrorCaptured;

        /// <summary>
        /// 當需要執行修正時。
        /// </summary>
        public event EventHandler<AutoCorrectEventArgs> AutoCorrect;

        /// <summary>
        /// 空白建構式
        /// </summary>
        public DocumentValidate()
        {
        }

        /// <summary>
        /// 驗證欄位
        /// </summary>
        /// <param name="RowSource"></param>
        /// <returns></returns>
        public List<FieldValidatedDescription> ValidateField(IRowStream RowSource)
        {
            return FieldValidate.ValidateHeader(RowSource);
        }

        /// <summary>
        /// 驗證資料
        /// </summary>
        /// <param name="RowSource"></param>
        /// <returns></returns>
        public bool Validate(IRowStream RowSource)
        {
            bool fieldResult = FieldValidate.Validate(RowSource);
            bool rowResult = RowValidate.Validate(RowSource);

            Duplication.Detecte(RowSource);

            return fieldResult & rowResult;
        }

        /// <summary>
        /// 開始主鍵驗證
        /// </summary>
        public void BeginDetecteDuplicate()
        {
            Duplication.BeginDetecte();
        }

        /// <summary>
        /// 結束主鍵驗證
        /// </summary>
        /// <returns></returns>
        public IList<DuplicateData> EndDetecteDuplicate()
        {
            return Duplication.EndDetecte();
        }

        /// <summary>
        /// 載入驗證規則
        /// </summary>
        /// <param name="XmlNode"></param>
        public void LoadRule(XmlElement XmlNode)
        {
            FieldValidators = new FieldValidatorCollection(XmlNode.SelectSingleNode("ValidatorList") as XmlElement);
            RowValidators = new RowValidatorCollection(XmlNode.SelectSingleNode("ValidatorList") as XmlElement);
            Conditions = new ConditionCollection(XmlNode.SelectSingleNode("ConditionList") as XmlElement);
            FieldValidate = new FieldValidate(XmlNode.SelectSingleNode("FieldList") as XmlElement, FieldValidators, Conditions);
            RowValidate = new RowValidate(XmlNode.SelectSingleNode("RowValidate") as XmlElement, RowValidators, Conditions);
            Duplication = new DuplicateDetection(XmlNode.SelectSingleNode("DuplicateDetection") as XmlElement);
            //MappingTables = LoadMappingTable(XmlNode.SelectSingleNode("MappingTable") as XmlElement);
            //DataSources = LoadDataSource(XmlNode.SelectSingleNode("DataSource") as XmlElement);

            FieldValidate.AutoCorrect += new EventHandler<AutoCorrectEventArgs>(mFieldValidate_AutoCorrect);
            FieldValidate.ErrorCaptured += new EventHandler<ErrorCapturedEventArgs>(mFieldValidate_ErrorCaptured);

            RowValidate.AutoCorrect += new EventHandler<AutoCorrectEventArgs>(mRowValidate_AutoCorrect);
            RowValidate.ErrorCaptured += new EventHandler<ErrorCapturedEventArgs>(mRowValidate_ErrorCaptured);
        }

        private List<object> LoadMappingTable(XmlElement xElement)
        {
            return new List<object>();
        }

        private DataSet LoadDataSource(XmlElement xElement)
        {
            return new DataSet("DataSources");
        }

        private void mRowValidate_ErrorCaptured(object sender, ErrorCapturedEventArgs e)
        {
            if (ErrorCaptured != null) ErrorCaptured(this, e);
        }

        private void mRowValidate_AutoCorrect(object sender, AutoCorrectEventArgs e)
        {
            if (AutoCorrect != null) AutoCorrect(this, e);
        }

        private void mFieldValidate_ErrorCaptured(object sender, ErrorCapturedEventArgs e)
        {
            if (ErrorCaptured != null) ErrorCaptured(this, e);
        }

        private void mFieldValidate_AutoCorrect(object sender, AutoCorrectEventArgs e)
        {
            if (AutoCorrect != null) AutoCorrect(this, e);
        }
    }
}