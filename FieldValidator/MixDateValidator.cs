using System.Xml;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System;

namespace EMBA.DocumentValidator
{
    public class MixDateValidator : IFieldValidator
    {
        private string _DateFormat;
        private List<DateMatcher> _Matchs;
        private DateMatcher _ValidPattern;

        public MixDateValidator(XmlElement xmlNode)
        {
            XmlHelper xml = new XmlHelper(xmlNode);

            _DateFormat = xml.GetString("Matchs/@CorrectTo");
            _ValidPattern = new DateMatcher(xmlNode.SelectSingleNode("ValidPattern") as XmlElement);

            _Matchs = new List<DateMatcher>();
            foreach (XmlElement e in xmlNode.SelectNodes("Matchs/Match"))
                _Matchs.Add(new DateMatcher(e));
        }

        public bool Validate(string value)
        {
            if (_ValidPattern.IsMatch(value))
            {
                Nullable<DateTime> result = _ValidPattern.Parse(value);

                //有值代表 Parse 過，就是驗證成功。
                return result.HasValue;
            }
            else
            {
                return false;
            }
        }

        public string Correct(string value)
        {
            DateMatcher parser = null;

            foreach (DateMatcher e in _Matchs)
            {
                if (e.IsMatch(value))
                {
                    parser = e;
                    break; // TODO: might not be correct. Was : Exit For
                }
            }

            if ((parser != null))
            {
                Nullable<DateTime> result = parser.Parse(value);

                if (!result.HasValue)
                {
                    return string.Empty;
                }
                else
                {
                    return "<Correct>" + result.Value.ToString(_DateFormat, DateTimeFormatInfo.InvariantInfo) + "</Correct>";
                }
            }
            else
            {
                return string.Empty;
            }
        }

        public string ToString(string description)
        {
            return description;
        }

        private class DateMatcher
        {

            private string _DateType;
            private System.Text.RegularExpressions.Regex _regex_match;

            public DateMatcher(XmlElement matchInfo)
            {
                _DateType = matchInfo.GetAttribute("DateType");
                _regex_match = new System.Text.RegularExpressions.Regex(matchInfo.InnerText, RegexOptions.Singleline);

                Console.WriteLine(_regex_match);
            }

            private string DateType
            {
                get { return _DateType; }
            }

            private Regex CurrentRegex
            {
                get { return _regex_match; }
            }

            public bool IsMatch(string value)
            {
                return CurrentRegex.Match(value).Success;
            }

            public Nullable<DateTime> Parse(string value)
            {

                Match m = CurrentRegex.Match(value);

                if (!m.Success) return null;

                Group year = m.Groups["Year"];
                Group month = m.Groups["Month"];
                Group day = m.Groups["Day"];

                if (DateType.ToUpper() == "GREGORIAN")
                {

                    return ParseGEDate(year.Value + "/" + month.Value + "/" + day.Value);
                }
                else  //(DateType.ToUpper() == "TAIWAN")
                {

                    int pYear = 0;
                    if (int.TryParse(year.Value, out pYear))
                    {
                        pYear = pYear + 1911;
                    }
                    else
                    {
                        return null;
                    }

                    return ParseGEDate(pYear.ToString() + "/" + month.Value + "/" + day.Value);
                }
            }

            private Nullable<DateTime> ParseGEDate(string value)
            {
                DateTime result = default(DateTime);

                if (DateTime.TryParse(value, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out result))
                    return result;
                else
                    return null;
            }

        }
    }
}