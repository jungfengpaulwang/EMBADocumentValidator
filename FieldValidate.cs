using System.Xml;
using System.Collections.Generic;
using System;
using System.Collections;

namespace EMBA.DocumentValidator
{
    public class FieldValidate
    {
        private Dictionary<string, FieldDescription> Fields { get; set; }

        private FieldValidatorCollection Validators { get; set; }

        private ConditionCollection Conditions { get; set; }

        /// <summary>
        /// 當一個驗證出錯誤資料時。
        /// </summary>
        public event EventHandler<ErrorCapturedEventArgs> ErrorCaptured;

        /// <summary>
        /// 當需要執行修正時。
        /// </summary>
        public event EventHandler<AutoCorrectEventArgs> AutoCorrect;

        public FieldValidate(XmlElement fieldDescription, FieldValidatorCollection validators, ConditionCollection conditions)
        {
            Validators = validators;
            Conditions = conditions;
            Fields = new Dictionary<string, FieldDescription>();

            foreach (XmlElement n in fieldDescription.SelectNodes("Field"))
            {
                FieldDescription fd = new FieldDescription(n);
                Fields.Add(fd.Name, fd);
            }
        }

        public List<FieldValidatedDescription> ValidateHeader(IRowStream row)
        {
            if (row == null) throw new ArgumentException("參數 row 不允許 Null。", "row");


            List<FieldValidatedDescription> FieldDescriptions = new List<FieldValidatedDescription>();

            foreach (FieldDescription DefinitionField in Fields.Values)
            {
                FieldValidatedDescription FieldDescription = new FieldValidatedDescription(DefinitionField);

                FieldDescription.InSource = row.Contains(DefinitionField.Name) ? true : false;
                FieldDescription.InDefinition = true;
                FieldDescriptions.Add(FieldDescription);
            }

            foreach (string SourceField in row)
            {
                if (!Fields.ContainsKey(SourceField))
                {
                    FieldValidatedDescription FieldDescription = new FieldValidatedDescription(null);

                    FieldDescription.Name = SourceField;
                    FieldDescription.InSource = true;
                    FieldDescription.InDefinition = false;
                    FieldDescriptions.Add(FieldDescription);
                }
            }

            return FieldDescriptions;
        }

        public bool Validate(IRowStream row)
        {
            if (row == null) throw new ArgumentException("參數 row 不允許 Null。", "row");

            bool allValidated = true;

            foreach (FieldDescription field in Fields.Values)
            {
                //if (field.Required && !row.Contains(field.Name))
                //{
                //    ErrorCapturedEventArgs args = new ErrorCapturedEventArgs(row, ValidatorType.Field, ErrorType.Error, field.Name, "缺少指定欄位「" + field.Name + "」。");
                //    if (ErrorCaptured != null) ErrorCaptured(this, args);

                //    throw new ArgumentException(string.Format("RowSource 缺少指定欄位「{0}」。", field.Name));
                //}

                //如果 RowSource 有此欄位才驗證
                if (!row.Contains(field.Name))
                    continue;

                //取得欄位值。
                string fieldValue = row.GetValue(field.Name);

                //確認空值是否要驗證資料。
                if (!field.EmptyAlsoValidate)
                    if (string.IsNullOrEmpty(fieldValue)) continue;

                bool isValidated = true;

                //驗證欄位各項規則
                foreach (ValidateStatement vstatement in field.ValidateStatements)
                {
                    if (IsValidatorEnabled(row, vstatement))
                    {
                        IFieldValidator validator = GetValidatorInstance(vstatement);

                        if (!validator.Validate(fieldValue))
                        {
                            if (!IsCorrectable(row, field, fieldValue, isValidated, vstatement, validator))
                                isValidated = false;

                            //如果是 Error 就不繼續驗證了， Warning 就需要再驗證。
                            if (vstatement.ErrorType == ErrorType.Error)
                                break;
                        }
                    }
                }

                if (!isValidated) allValidated = false;
            }

            return allValidated;
        }

        private IFieldValidator GetValidatorInstance(ValidateStatement vs)
        {
            if (!Validators.Contains(vs.Validator))
                throw new ArgumentException(string.Format("找不到指定的 Field Validator：{0}", vs.Validator));

            return Validators[vs.Validator];
        }

        private bool IsValidatorEnabled(IRowStream row, ValidateStatement vs)
        {
            bool doValidate = true;
            if (!string.IsNullOrEmpty(vs.When))
            {
                if (!Conditions.Contains(vs.When))
                    throw new ArgumentException(string.Format("找不到指定的 Condition：{0}", vs.When));

                doValidate = Conditions[vs.When].Evaluate(row);
            }
            return doValidate;
        }

        private bool IsCorrectable(IRowStream row, FieldDescription field,
            string fieldValue, bool isValidated,
            ValidateStatement vs, IFieldValidator validator)
        {
            if (vs.AutoCorrect)
            {
                string newValue = validator.Correct(fieldValue);
                if (!string.IsNullOrEmpty(newValue))
                {
                    XmlDocument xmldoc = new XmlDocument();
                    xmldoc.LoadXml(newValue);
                    newValue = xmldoc.DocumentElement.InnerText;
                    if (AutoCorrect != null)
                    {
                        AutoCorrectEventArgs args = new AutoCorrectEventArgs(row, ValidatorType.Field, field.Name, fieldValue, newValue);
                        if (AutoCorrect != null) AutoCorrect(this, args);
                    }
                    isValidated = false;
                }
                else
                {
                    ErrorCapturedEventArgs args = new ErrorCapturedEventArgs(row, ValidatorType.Field, vs.ErrorType, field.Name, validator.ToString(vs.Description));
                    if (ErrorCaptured != null) ErrorCaptured(this, args);
                }
            }
            else
            {
                ErrorCapturedEventArgs args = new ErrorCapturedEventArgs(row, ValidatorType.Field, vs.ErrorType, field.Name, validator.ToString(vs.Description));
                if (ErrorCaptured != null) ErrorCaptured(this, args);
            }
            return isValidated;
        }
    }
}