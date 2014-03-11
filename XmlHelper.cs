using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace EMBA.DocumentValidator
{
    class XmlHelper
    {
        public XmlHelper(XmlElement elm)
        {
            if (elm == null) throw new ArgumentException("基礎 XmlElement 物件不可為 Null。", "elm");

            InnerElement = elm;
        }

        public static XmlHelper Parse(string xmldata)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmldata);

            return new XmlHelper(doc.DocumentElement);
        }

        public XmlElement InnerElement { get; set; }

        public string GetString(string xpath)
        {
            XmlNode elm = InnerElement.SelectSingleNode(xpath);

            if (elm == null)
                return string.Empty;
            else
                return elm.InnerText;
        }

        public bool GetBoolean(string xpath, bool defaultValue)
        {
            string value = GetString(xpath);
            bool result;

            if (bool.TryParse(value, out result))
                return result;
            else
                return defaultValue;
        }

        public int GetInteger(string xpath, int defaultValue)
        {
            string value = GetString(xpath);
            int result;

            if (int.TryParse(value, out result))
                return result;
            else
                return defaultValue;
        }

        public short GetShort(string xpath, short defaultValue)
        {
            string value = GetString(xpath);
            short result;

            if (short.TryParse(value, out result))
                return result;
            else
                return defaultValue;
        }

        public DateTime GetDateTime(string xpath, DateTime defaultValue)
        {
            string value = GetString(xpath);
            DateTime result;

            if (DateTime.TryParse(value, out result))
                return result;
            else
                return defaultValue;
        }

        public List<XmlElement> GetElements(string xpath)
        {
            List<XmlElement> elements = new List<XmlElement>();

            foreach (XmlNode each in InnerElement.SelectNodes(xpath))
            {
                if (each is XmlElement)
                    elements.Add(each as XmlElement);
            }

            return elements;
        }
    }
}
