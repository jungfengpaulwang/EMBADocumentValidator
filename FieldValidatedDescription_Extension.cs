using System;
using System.Collections.Generic;
using System.Text;

namespace EMBA.DocumentValidator
{
    public static class FieldValidatedDescription_Extension
    {
        public static string ToDisplay(this IList<FieldValidatedDescription> FieldDescriptions)
        {
            //缺少的必填欄位
            List<string> errorFields = new List<string>();
            //缺少的選填欄位
            List<string> warningFields = new List<string>();
            //不在證驗規則中的欄位
            List<string> reminderFields = new List<string>();

            foreach (FieldValidatedDescription Field in FieldDescriptions)
            {
                if (!Field.InSource && Field.InDefinition)
                    if (Field.Required)
                        errorFields.Add(string.Format("「{0}」", Field.Name));
                    else
                        warningFields.Add(string.Format("「{0}」", Field.Name));
                else if (Field.InSource && !Field.InDefinition)
                    reminderFields.Add(string.Format("「{0}」", Field.Name));
            }

            List<string> result = new List<string>();
            if (errorFields.Count > 0)
                result.Add(string.Format("錯誤：缺少必填欄位{0}", string.Join("、", errorFields.ToArray())));
            //if (warningFields.Count > 0)
            //    result.Add(string.Format("警告：缺少選填欄位{0}", string.Join("、", warningFields.ToArray())));
            //if (reminderFields.Count >0)
            //    result.Add(string.Format("提示：欄位不在定義中, 不匯入系統{0}",string.Join("、",reminderFields.ToArray())));

            return string.Join("\r\n", result.ToArray());
        }
    }
}