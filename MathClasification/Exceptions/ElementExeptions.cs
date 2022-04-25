namespace MathClasification;

public class ElementExceptions : Exception
{
    public ElementExceptions(String ex) : base(ex)
    {
        
    }
    public ElementExceptions() : base()
    {
        
    }
}

public class ElementParseExceptions : ElementExceptions
{
    public ElementParseExceptions(String ex) : base(ex)
    {
        
    }
    public ElementParseExceptions() : base()
    {
        
    }
}

public class ElementCreationExceptions : Exception
{
    public ElementCreationExceptions(String ex) : base(ex)
    {
        
    }
    public ElementCreationExceptions() : base()
    {
        
    }
}
public class InvalidValue : ElementCreationExceptions
{
    public InvalidValue() : base("Invalid Value Specified.")
    {
        
    }
    
}
public class InvalidElements : ElementCreationExceptions
{
    public InvalidElements() : base("Invalid Elements Specified.")
    {
        
    }
    
}