using System;
using System.Collections.Generic;
using System.Text;

namespace EMBA.DocumentValidator
{
    public class ErrorCapturedEventArgs : EventArgs
    {
        public ErrorCapturedEventArgs(IRowStream source, ValidatorType validType, ErrorType errorType, string fieldName, string description)
        {
            FieldName = fieldName;
            ErrorType = errorType;
            Description = description;
            Row = source;
            ValidatorType = validType;
        }

        public string FieldName { get; private set; }

        public ErrorType ErrorType { get; private set; }

        public string Description { get; private set; }

        public IRowStream Row { get; private set; }

        public ValidatorType ValidatorType { get; private set; }
    }

    public class AutoCorrectEventArgs : EventArgs
    {
        public AutoCorrectEventArgs(IRowStream source, ValidatorType validType, string fieldName, string oldValue, string newValue)
        {
            FieldName = fieldName;
            OldValue = oldValue;
            NewValue = newValue;
            Row = source;
            ValidatorType = validType;
        }

        public string FieldName { get; private set; }

        public string OldValue { get; private set; }

        public string NewValue { get; private set; }

        public IRowStream Row { get; private set; }

        public ValidatorType ValidatorType { get; private set; }
    }
}
