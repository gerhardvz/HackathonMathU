using System.Xml.Serialization;

namespace MathClasification;

public class Operator : Element
{
    Operator(string value,Element[] elements):base(value.Clone(), (Element[])elements.Clone())
    {
        
    }
    
    
}