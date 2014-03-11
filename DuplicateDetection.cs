using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Linq;

namespace EMBA.DocumentValidator
{
    /// <summary>
    /// 欄位值重覆組合偵測
    /// </summary>
    internal class DuplicateDetection
    {
        private List<Detector> Detectors { get; set; }

        private XmlElement OriginDescription { get; set; }

        public DuplicateDetection(XmlElement description)
        {
            Detectors = new List<Detector>();
            OriginDescription = description;
        }

        private void InitDetectors()
        {
            Detectors = new List<Detector>();

            if (OriginDescription == null) return;

            foreach (XmlElement each in OriginDescription.SelectNodes("Detector"))
            {
                if (each.HasAttribute("IsImportKey") && each.Attributes["IsImportKey"].Value.ToUpper() == "FALSE")
                    continue;

                Detectors.Add(new Detector(each));
            }
        }

        public void Detecte(IRowStream rowStream)
        {
            foreach (Detector each in Detectors)
                each.Detecte(rowStream);
        }

        /// <summary>
        /// 開始偵測重覆
        /// </summary>
        public void BeginDetecte()
        {
            InitDetectors();
        }

        /// <summary>
        /// 結束偵測重覆
        /// </summary>
        /// <returns></returns>
        public IList<DuplicateData> EndDetecte()
        {
            List<DuplicateData> datas = new List<DuplicateData>();
            foreach (Detector each in Detectors)
                datas.Add(each.GetResult());

            return datas;
        }
    }

    internal class Detector
    {
        private List<string> Fields = new JoinList<string>();

        private Dictionary<string, DataKey> Records { get; set; }

        public Detector(XmlElement description)
        {
            Name = description.GetAttribute("Name");
            ErrorType = ParseErrorType(description.GetAttribute("ErrorType"));
            Records = new Dictionary<string, DataKey>();

            foreach (XmlElement each in description.SelectNodes("Field"))
                Fields.Add(each.GetAttribute("Name"));
        }

        public string Name { get; set; }

        public ErrorType ErrorType { get; set; }

        private ErrorType ParseErrorType(string errorType)
        {
            if (errorType.ToUpper() == "ERROR" || string.IsNullOrEmpty(errorType))
                return ErrorType.Error;
            else if (errorType.ToUpper() == "WARNING")
                return ErrorType.Warning;
            else
                throw new ArgumentException("不支援此種 ErrorType：" + errorType);
        }

        public void Detecte(IRowStream rowStream)
        {
            StringBuilder uniqueBuilder = new StringBuilder();

            foreach (string field in Fields)
            {
                //如果來源資料沒有包含全部所需欄位，Detector 就不運作
                if (!rowStream.Contains(field)) break;

                uniqueBuilder.AppendFormat("{0}:", rowStream.GetValue(field).Trim());
            }

            string unique = uniqueBuilder.ToString();

            if (!Records.ContainsKey(unique))
            {
                Records.Add(unique, new DataKey());

                DataKey dr = Records[unique];

                foreach (string field in Fields)
                    dr.KeyData.Add(field, rowStream.GetValue(field).Trim());
            }

            Records[unique].Positions.Add(rowStream.Position);
        }

        public DuplicateData GetResult()
        {
            List<DuplicateRecord> records = new List<DuplicateRecord>();            

            foreach (string key in Records.Keys)
            {
                bool IsDuplicated = false;

                DataKey dataKey = Records[key];
                //  鍵值的每一欄位之串接不得為空
                string keySeries = string.Empty;
                dataKey.KeyData.Values.ToList().ForEach(x=>keySeries+=x.Trim());

                if (string.IsNullOrWhiteSpace(keySeries))
                    IsDuplicated = true;
                //  鍵值的每一欄位，其值的組合不得重覆
                if (Records[key].IsDuplicated) 
                    IsDuplicated = true;

                if (!IsDuplicated) continue;

                List<string> values = new JoinList<string>();
                List<int> positions = new JoinList<int>();

                foreach (KeyValuePair<string, string> eachKey in dataKey.KeyData)
                    values.Add(eachKey.Value);

                foreach (int eachRow in dataKey.Positions)
                    positions.Add(eachRow);

                records.Add(new DuplicateRecord(values, positions));
            }

            return new DuplicateData(Name, ErrorType, Fields, records);
        }
    }

    /// <summary>
    /// 代表某一筆資料的識別。
    /// </summary>
    internal class DataKey
    {
        public DataKey()
        {
            KeyData = new Dictionary<string, string>();
            Positions = new List<int>();
        }

        public Dictionary<string, string> KeyData { get; private set; }

        public List<int> Positions { get; private set; }

        public bool IsDuplicated 
        { 
            get 
            { 
                return Positions.Count > 1 ; 
            } 
        }
    }

    /// <summary>
    /// 重覆資料組合
    /// </summary>
    public class DuplicateData : IEnumerable<DuplicateRecord>
    {
        /// <summary>
        /// 建構式，傳入鍵值名稱、錯誤型態、欄位組合及重覆資料記錄
        /// </summary>
        /// <param name="name"></param>
        /// <param name="errorType"></param>
        /// <param name="fields"></param>
        /// <param name="records"></param>
        public DuplicateData(string name, ErrorType errorType, List<string> fields, IList<DuplicateRecord> records)
        {
            Name = name;
            ErrorType = errorType;
            Fields = fields;
            Records = records;
            Count = records.Count;
        }

        /// <summary>
        /// 在驗證規則中的欄位組合描述名稱
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 錯誤型態
        /// </summary>
        public ErrorType ErrorType { get; private set; }

        /// <summary>
        /// 欄位組合
        /// </summary>
        public IList<string> Fields { get; private set; }

        /// <summary>
        /// 欄位組合顯示字串
        /// </summary>
        public string FieldsDispaly
        {
            get
            {
                List<string> DisplayFields = new List<string>(Fields);

                return string.Join(",", DisplayFields.ToArray());
            }
        }

        /// <summary>
        /// 實際重覆的資料
        /// </summary>
        private IList<DuplicateRecord> Records { get; set; }

        /// <summary>
        /// 重覆的鍵值組合數目
        /// </summary>
        public int Count { get; private set; }

        public DuplicateRecord this[int index] { get { return Records[index]; } }

        #region IEnumerable<DuplicateRecord> 成員

        public IEnumerator<DuplicateRecord> GetEnumerator()
        {
            return Records.GetEnumerator();
        }

        #endregion

        #region IEnumerable 成員

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Records.GetEnumerator();
        }

        #endregion
    }

    /// <summary>
    /// 實際資料組合重覆的資料列
    /// </summary>
    public class DuplicateRecord
    {
        /// <summary>
        /// 建構式，傳入欄位值組合，以及這些值重覆的位置
        /// </summary>
        /// <param name="values"></param>
        /// <param name="positions"></param>
        public DuplicateRecord(List<string> values, List<int> positions)
        {
            Values = values;
            Positions = positions;
        }

        /// <summary>
        /// 識別 Row 的相關資料。
        /// </summary>
        public IList<string> Values { get; private set; }

        /// <summary>
        /// 重覆的 Row 資訊。
        /// </summary>
        public IList<int> Positions { get; private set; }
    }

    internal class JoinList<T> : List<T>
    {
        public override string ToString()
        {
            List<string> join = new List<string>();

            foreach (T each in this)
                join.Add(each.ToString());

            return string.Join(",", join.ToArray());
        }
    }
}
