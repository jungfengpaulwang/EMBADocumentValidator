using System.Xml;
namespace EMBA.DocumentValidator
{
    public interface IRowValidatorFactory
    {
        IRowVaildator CreateRowValidator(string typeName, XmlElement validatorDescription);
    }
}