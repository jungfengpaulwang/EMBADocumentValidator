// ERROR: Not supported in C#: OptionDeclaration using System.Xml;
using System.Collections.Generic;
using System.Xml;
using System;

namespace EMBA.DocumentValidator
{
    public class EnumerationValidator : IFieldValidator
    {
        //記錄「值」，用代碼當作Key。
        private Dictionary<string, string> mValues;
        //記錄項目內容
        //記錄「代碼」，用值當作key。
        private Dictionary<string, string> mCodes;
        //代碼？
        //用「Variation」當作key尋找「值」。(Variation是不會重覆的。)
        private Dictionary<string, string> mCorrects;
        //Correct的內容

        public EnumerationValidator(XmlElement XmlNode)
        {
            mValues = new Dictionary<string, string>();
            mCodes = new Dictionary<string, string>();
            mCorrects = new Dictionary<string, string>();

            LoadEnum(XmlNode.ChildNodes);
        }

        public int Count
        {
            get { return mValues.Count; }
        }

        //用名字找 Code，因為此類別有其他用途，所以有此方法。
        public string GetCode(string Name)
        {
            return GetItem(mCodes, Name);
        }

        //用 Code 找名字，因為此類別有其他用途，所以有此方法。
        public string GetName(string Code)
        {
            return GetItem(mValues, Code);
        }

        public bool Validate(string Value)
        {
            //檢查「值」是否在清單中。
            return mValues.ContainsValue(Value.Trim());
        }

        public string Correct(string Value)
        {
            string result = string.Empty;

            if (mCorrects.TryGetValue(Value, out result))
                return string.Format("<Correct>{0}</Correct>", result);

            return result;
        }

        private string GetItem(Dictionary<string, string> Coll, string Key)
        {
            if (Coll.ContainsKey(Key))
                return Coll[Key];
            else
                return string.Empty;
        }

        public string ToString(string Description)
        {
            return Description;
        }

        private EnumerationValidator LoadEnum(XmlNodeList NodeList)
        {
            string Value = null;
            string Code = null;

            try
            {

                foreach (XmlNode each in NodeList)
                {
                    if (!(each is XmlElement)) continue;

                    XmlElement n = each as XmlElement;

                    if (n.NodeType == XmlNodeType.Element)
                    {

                        Value = n.SelectSingleNode("@Value").InnerText.Trim();
                        if (n.SelectSingleNode("@Code") == null)
                        {
                            Code = "";
                        }
                        else
                        {
                            Code = n.SelectSingleNode("@Code").InnerText.Trim();
                        }

                        //如果沒有 Code ，就把 Value 當作 Code，以防加入集合時發生錯誤。

                        if (string.IsNullOrEmpty(Code)) Code = Value;

                        //因為此類別有其他用途，所以有此集合
                        //以 Code 當key尋找 Value
                        mValues.Add(Code, Value);

                        //因為此類別有其他用途，所以有此集合
                        //以 Value 當key尋找 Code
                        mCodes.Add(Value, Code);

                        //For Each Variation Element
                        foreach (XmlElement n1 in SubElements(n))
                        {
                            //記錄Correct的內容，以value當Key
                            mCorrects.Add(n1.InnerText, Value);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return this;
        }

        private IEnumerable<XmlElement> SubElements(XmlElement parent)
        {
            foreach (XmlNode one in parent.ChildNodes)
            {
                if (one.NodeType == XmlNodeType.Element)
                    yield return one as XmlElement;
            }
        }
    }
}