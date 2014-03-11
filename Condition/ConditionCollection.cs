using System.Xml;
using System.Collections.Generic;
using System;
using System.Collections;

namespace EMBA.DocumentValidator
{
    public class ConditionCollection : IEnumerable<Condition>
    {
        //存放集合物件的區域變數
        private IDictionary<string, Condition> Conditions { get; set; }

        public ConditionCollection(XmlElement xmlNode)
        {
            Conditions = new Dictionary<string, Condition>();

            if (xmlNode == null) return;

            foreach (XmlElement n in xmlNode.SelectNodes("Condition"))
            {
                try
                {
                    XmlHelper xmln = new XmlHelper(n);
                    Condition Cond = new Condition(n);
                    string name = xmln.GetString("@Name");

                    if (Conditions.ContainsKey(name))
                        throw new ArgumentException(string.Format("Condition 名稱重覆：{0}。", name));

                    Conditions.Add(name, Cond);
                }
                catch (Exception ex)
                {
                    throw new Exception("初始化 Condition 錯誤！", ex);
                }
            }
        }

        public Condition this[string key]
        {
            get { return Conditions[key]; }
        }

        public int Count
        {
            get { return Conditions.Count; }
        }

        public bool Contains(string condName)
        {
            return Conditions.ContainsKey(condName);
        }

        /// <summary>
        /// 列舉 Condition 物件。
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public IEnumerator GetEnumerator()
        {
            return Conditions.Values.GetEnumerator();
        }

        #region IEnumerable<Condition> Members

        IEnumerator<Condition> IEnumerable<Condition>.GetEnumerator()
        {
            return Conditions.Values.GetEnumerator();
        }

        #endregion
    }
}