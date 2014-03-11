using System.Xml;

namespace EMBA.DocumentValidator
{
    public interface IFieldValidator
    {
        bool Validate(string Value);
        
        string Correct(string Value);
        
        string ToString(string template);
    }
}