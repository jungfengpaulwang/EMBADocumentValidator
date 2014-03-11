using System.Xml;

namespace EMBA.DocumentValidator
{
    public class DefaultRowValidator : IRowVaildator
    {
        #region IRowVaildator 成員

        public bool Validate(IRowStream Value)
        {
            return false;
        }

        public string Correct(IRowStream Value)
        {
            return string.Empty;
        }

        public string ToString(string template)
        {
            return template;
        }

        #endregion
    }

    public class DefaultRowValidatorFactory : IRowValidatorFactory
    {
        #region IRowValidatorFactory 成員

        public IRowVaildator CreateRowValidator(string typeName, XmlElement validatorDescription)
        {
            switch (typeName.ToUpper())
            {
                case "":
                    return new DefaultRowValidator();
                default:
                    return null;
            }
        }

        #endregion
    }

}
