using System.Xml.Serialization;

namespace MathClasification;

public class Number : Element
{
    Number(string value):base(value.Clone(), null)
    {
        
    }
    
    
}