using System.Xml;
namespace EMBA.DocumentValidator
{
    public interface IFieldValidatorFactory
    {
        IFieldValidator CreateFieldValidator(string typeName,XmlElement validatorDescription);
    }
}