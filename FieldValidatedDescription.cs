
namespace EMBA.DocumentValidator
{
    /// <summary>
    /// 驗證過後的欄位定義描述
    /// </summary>
    public class FieldValidatedDescription : FieldDescription
    {
        /// <summary>
        /// 欄位是否在來源中
        /// </summary>
        public bool InSource { get; set; }

        /// <summary>
        /// 欄位是否在資料驗證描述中
        /// </summary>
        public bool InDefinition { get; set; }

        /// <summary>
        /// 建構式，傳入資料驗證欄位定義，若傳入null則會將相關屬設為空白或是false。
        /// </summary>
        /// <param name="Field"></param>
        public FieldValidatedDescription(FieldDescription Field)
        {
            if (Field != null)
            {
                this.EmptyAlsoValidate = Field.EmptyAlsoValidate;
                this.Name = Field.Name;
                this.Required = Field.Required;
            }
            else
            {
                this.EmptyAlsoValidate = false;
                this.Name = string.Empty;
                this.Required = false;
                this.InSource = false;
                this.InDefinition = false; 
            }            
        }
    }
}