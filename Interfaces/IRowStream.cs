using System.Collections.Generic;

namespace EMBA.DocumentValidator
{
    public interface IRowStream : IEnumerable<string>
    {
        /// <summary>
        /// 取得欄位資料。
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        string GetValue(string fieldName);

        /// <summary>
        /// 判斷是否包含某個欄位。
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        bool Contains(string fieldName);

        /// <summary>
        /// 目前 Row 的位置。
        /// </summary>
        int Position { get; }
    }
}