using System.Xml.Serialization;

namespace MathClasification;

public class Number : Element
{
    public Number(string value):base(value, false,null)
    {
        Console.WriteLine("Created number:"+_value);
    }
    
    
}