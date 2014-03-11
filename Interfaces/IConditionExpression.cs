namespace EMBA.DocumentValidator
{
    public interface IConditionExpression
    {
        bool Evaluate(IRowStream RowSource);
    }
}