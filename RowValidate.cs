using System.Xml;
using System;
using System.Collections.Generic;

namespace EMBA.DocumentValidator
{
    public class RowValidate
    {
        //存放集合物件的區域變數
        private ValidateStatements Statements { get; set; }

        private RowValidatorCollection Validators { get; set; }

        private ConditionCollection Conditions { get; set; }

        /// <summary>
        /// 當一個驗證出錯誤資料時。
        /// </summary>
        public event EventHandler<ErrorCapturedEventArgs> ErrorCaptured;

        /// <summary>
        /// 當需要執行修正時。
        /// </summary>
        public event EventHandler<AutoCorrectEventArgs> AutoCorrect;

        public RowValidate(XmlElement rowDescription, RowValidatorCollection RVList, ConditionCollection CondList)
        {
            Validators = RVList;
            Conditions = CondList;
            Statements = new ValidateStatements(rowDescription);
        }

        public bool Validate(IRowStream RowSource)
        {
            bool isValidated = true;

            //檢查該列驗證規則
            foreach (ValidateStatement vs in Statements)
            {
                if (IsValidatorEnabled(RowSource, vs))
                {
                    IRowVaildator rv = GetValidatorInstance(vs);

                    if (!rv.Validate(RowSource))
                        isValidated &= IsCorrectable(RowSource, vs, rv);
                }
            }

            return isValidated;
        }

        private bool IsCorrectable(IRowStream RowSource, ValidateStatement vs, IRowVaildator rv)
        {
            bool result = false;

            if (vs.AutoCorrect)
            {
                string newValue = rv.Correct(RowSource);

                XmlDocument xmldoc = new XmlDocument();

                try
                {
                    xmldoc.LoadXml(newValue);
                }
                catch
                {
                    throw new ArgumentException(string.Format("Correct Specification Invalid ({0}).", rv.ToString()));
                }

                if (xmldoc.DocumentElement.ChildNodes.Count > 0)
                {
                    foreach (XmlNode each in xmldoc.DocumentElement.ChildNodes)
                    {
                        XmlElement n = each as XmlElement;
                        if (n == null) continue;

                        string OldValue = RowSource.GetValue(n.LocalName);
                        if (AutoCorrect != null)
                        {
                            AutoCorrectEventArgs args = new AutoCorrectEventArgs(RowSource, ValidatorType.Row, n.LocalName, OldValue, n.InnerText);
                            if (AutoCorrect != null) AutoCorrect(this, args);
                            result = true; //修正成功。
                        }
                    }
                }
                else
                {
                    ErrorCapturedEventArgs args = new ErrorCapturedEventArgs(RowSource, ValidatorType.Row, vs.ErrorType, string.Empty, rv.ToString(vs.Description));
                    if (ErrorCaptured != null) ErrorCaptured(this, args);
                }
            }
            else
            {
                ErrorCapturedEventArgs args = new ErrorCapturedEventArgs(RowSource, ValidatorType.Row, vs.ErrorType, string.Empty, rv.ToString(vs.Description));
                if (ErrorCaptured != null) ErrorCaptured(this, args);
            }

            return result;
        }

        private bool IsValidatorEnabled(IRowStream RowSource, ValidateStatement vs)
        {
            //取出 When 值, 判斷是否要檢查
            bool doValidate = true;
            if (!string.IsNullOrEmpty(vs.When))
            {
                if (!Conditions.Contains(vs.When))
                    throw new ArgumentException(string.Format("找不到指定的 Condition：{0}", vs.When));

                doValidate = Conditions[vs.When].Evaluate(RowSource);
            }
            return doValidate;
        }

        private IRowVaildator GetValidatorInstance(ValidateStatement vs)
        {
            if (!Validators.Contains(vs.Validator))
                throw new ArgumentException(string.Format("找不到指定的 Row Validator：{0}", vs.Validator));

            return Validators[vs.Validator];
        }
    }
}