using System.Net;
using System.Xml;
using System.Xml.Schema;

namespace MathClasification;

public class Element
{
    protected Element[] _elements;
    protected Boolean _canSwap;
    protected String _value;
    protected const bool debug = true;

    protected Element(String value, Boolean swappable, Element[] elements)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new InvalidValue();
        }
        this._value = value;
        
        if (elements!=null && (elements.Count()==0 || elements.Count()>2))
        {
            throw new InvalidElements();
        }
        this._elements = elements;
        this._canSwap = swappable;
        
        debugPrint("Creating "+ value  + ". Elements count = "+(elements!=null?elements.Count():"0"));
    }

    protected Element(String value, Element[] elements)
    {
        this._value = value;
        if (!(elements.Length > 0 && elements.Length <= 2))
        {
            throw new Exception("Incorrect amount of elements");
        }

        this._elements = elements;
        //if element lenght is 0 then the value must be a number
        if (this._elements.Length == 0)
        {
            Console.WriteLine("Created number/identifier");
        }
        //else it is an operator
        else if (this._value.GetType() == new int().GetType())
        {
            Console.WriteLine("Created Operator");
            //certain operators can swap their elements without changing the output of the equation
            //Identify them here
        }
        debugPrint("Creating"+ this.ToString()   );
    }

    public static Element[] parseMathMLFile(String File)
    {
        XmlUrlResolver resolver = new XmlUrlResolver();
        resolver.Credentials = CredentialCache.DefaultCredentials;

        XmlDocument doc = new XmlDocument();
        XmlReaderSettings readerSettings = new XmlReaderSettings();
        readerSettings.XmlResolver = resolver;
        readerSettings.DtdProcessing = DtdProcessing.Parse;
        readerSettings.ValidationType = ValidationType.DTD;
        readerSettings.ValidationEventHandler += new ValidationEventHandler(ValidationCallBack);

        List<Element> elements = new List<Element>();
        using (XmlReader reader = XmlReader.Create("MathTest1.ml", readerSettings))
        {
            while (reader.Read())
            {
                reader.MoveToContent();
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.Load(reader);
                // XmlNode? node = xmldoc.ReadNode(reader);
                var nodeList = xmldoc.ChildNodes;


                //Itterate through child object and read all that are <math>
                foreach (XmlNode childNode in nodeList)
                {
                    if (childNode.Name == "math")
                    {
                        //parse MathML
                        //TODO Dont use casting
                        elements.Add(parseMathML(childNode.ChildNodes.Cast<XmlNode>()));
                    }
                }
                //No more child nodes found with math name
            }
        }

        return elements.ToArray();
    }

    private static void ValidationCallBack(object sender, ValidationEventArgs e)
    {
        Console.WriteLine("Validation Error: {0}", e.Message);
    }

    public static Element parseMathML(IEnumerable<XmlNode> mathML)
    {
        var mathFunctions = new List<Func<IEnumerable<XmlNode>, Element?>>()
        {
            walkItem,
            checkBrackets,
            checkEquals,
            checkAddition,
            checkSubtraction,
            checkSymbol,
            checkDivision,
            checkMultiplication,
            checkSquarroot,
            checkPower,
            checkNumber
        };
        //Search for the position of the '=' in the calculation - should be on top level
        //Only happens once except if there is an error
        int nodePos;
        foreach (var order in mathFunctions)
        {
            var elm = order(mathML);
            
            if (elm is not null)
            {
                debugPrint(elm.ToString());
                return elm;
            }
        }

        // if code reaches this stage there was invalid mathml
        throw new ElementParseExceptions("MathML doc was read and format was incorrect.");
    }


    
    //checkNumber Working
    static private Element checkNumber(IEnumerable<XmlNode> mathML)
    {
        debugPrint("Number" );
        if (mathML.Count() == 1)
        {
            debugPrint("Count 1" );
            var node = mathML.First();
            if (node.Name == "mn" | node.Name == "mi")
            {
                debugPrint("Number" + node.InnerText);
                return new Number(node.InnerText);
            }
            debugPrint(node.Name + " " +node.InnerText);
            throw new ElementExceptions("Not a valid number identifier.");
        }
        if (mathML.Count() == 2)
        {
            debugPrint("Count 2" );
            debugPrint("Signed Number");
            var first = mathML.First();
            var last = mathML.Last();
            if (first.Name == "mo")
            {
                debugPrint("Signed Number:" + String.Concat(first.InnerText,last.InnerText));
                return new Number(String.Concat(first.InnerText,last.InnerText));
            }
        }
        debugPrint("Oops" + mathML.Count() );
        
        debugPrint(mathML.First().Name + " :: " + mathML.First().InnerText);
        foreach (var node in mathML)
        {
            debugPrint(node.Name + " :: " + node.InnerText);
        }
        throw new ElementExceptions("More that two elements remaining. Something went wrong E101");
    }

    //isContained returns false if the element type is one of the folowing {mi,mo,mn,mtext}
    static private bool isContained(XmlNode element)
    {
        string name = element.Name;
        switch (name)
        {
            case "mi":
                return false;
                break;
            case "mn":
                return false;
                break;
            case "mo":
                return false;
                break;
            case "mtext":
                return false;
                break;
        }

        return true;
    }

    //checkEquals Working
    static private Element? checkEquals(IEnumerable<XmlNode> mathML)
    {
        var elementList = SplitOnElementType(mathML, "mo", "=");
        if (elementList != null)
        {
            
            return new Element("=", true, elementList);
        }
        
        return null;
        ;
    }

    static private Element? checkSquarroot(IEnumerable<XmlNode> mathML)
    {
        
        var elementList = SplitOnElementType(mathML, "msqrt");
        if (elementList != null)
        {
            debugPrint("SquaredRoot");
            //TODO Dont use casting
            return new Element("root",false,new [] {parseMathML(elementList.Cast<XmlNode>())});
        }

        return null;
        ;
    }

    static private Element? checkPower(IEnumerable<XmlNode> mathML)
    {
        
        var nodes = SplitOnElementType(mathML, "msup");
       
        if (nodes != null )
            debugPrint("Here: " + nodes.Count);
        
        
        if (nodes != null &&  nodes.Count == 2)
        {
            debugPrint("Here");
            var bas = nodes.Cast<XmlNode>();
            var left = bas.Take(1);
            var right = bas.Skip(1);
            debugPrint("Read Nodes");
            debugPrint("Base:" + (left.First()).Name);
            debugPrint("Exponent:" + (right.First()).Name);
            if (left != null || right != null)
            {
                //Add nodes to Enumerables - otherwise if element is casted and was an number operator or identifier it will be thrown as #text
                
                //Todo Fix Suppressed cast "!"
                return new Element("^", true, new[] {parseMathML(left), parseMathML(right)});
            }

            throw new ElementExceptions("Empty Fraction Element.");
        }
        return null;
    }

    static private Element? checkMultiplication(IEnumerable<XmlNode> mathML)
    {
        string[] Symbols = new[] {"x", "*","⁢",""};
        Element[] elementList;
        foreach (var symbol in Symbols)
        {
            if ((elementList = SplitOnElementType(mathML, "mo", symbol)) != null)
            {
                return new Element("x", true, elementList);
            }  
        }
       
        //gives problem when using msup or mfrac
        //Todo Check that two censecutive items are of mi or mn
        if (mathML.Count()>1)
       foreach (var node in mathML)
        {
            if (node.Name=="mi" || node.Name=="mn")
            {
                var nextSibling = node.NextSibling;
                if (nextSibling != null && (nextSibling.Name=="mi" || nextSibling.Name=="mn"))
                {
                    
                    var left = mathML.Take(1);
                    var right = mathML.Skip(1);  
                    //Test if right first item == sibling
                    if (right.First()==nextSibling)
                    {
                        debugPrint("They are sibling");
                        return new Element("x", true, new []{parseMathML(left),parseMathML(right)});
                    }
                    //Consecutive Elements
                    //TODO Dont use casting
                    return new Element("x", true, new []{parseMathML(node.Cast<XmlNode>()),parseMathML(nextSibling.Cast<XmlNode>())});
                }
            }
        }
       
        //test for consecutive mn and mi entity after each other
        // 2xyz will be ((2) * ((x) * ((y) * (z) ) )


        return null;
    }

    static private Element? checkDivision(IEnumerable<XmlNode> mathML)
    {
        Element[] elementList;

        if ((elementList = SplitOnElementType(mathML, "mo", "/")) != null)
        {
            return new Element("/", true, elementList);
        }

        XmlNodeList nodes = SplitOnElementType(mathML, "mfrac");
        //Element can only have 2 child elements in it
        if (nodes != null &&  nodes.Count == 2)
        {
            debugPrint("Test mfrac");
            var left = nodes[0];
            var right = nodes[1];
            debugPrint("Meth");
            debugPrint(left.Name);
            debugPrint(right.Name);
            if (left != null || right != null)
            {
                //Todo Fix Suppressed cast "!"
                //TODO Dont use casting
                return new Element("/", true,
                    new[] {parseMathML(left!.Cast<XmlNode>()), parseMathML(right!.Cast<XmlNode>())});
            }

            throw new ElementExceptions("Empty Fraction Element.");
        }

        if (elementList != null)
        {
            return new Element("/", false, elementList);
        }

        //TODO:Split on frac

        return null;
    }

    static private Element? checkAddition(IEnumerable<XmlNode> mathML)
    {
        var elementList = SplitOnElementType(mathML, "mo", "+");
        if (elementList != null)
        {
            return new Element("+", true, elementList);
        }

        return null;
    }
    
    //checkSymbol Working
    static private Element? checkSymbol(IEnumerable<XmlNode> mathML)
    {
        String[] symbols = new string[] {"±"};
        foreach (var symbol in symbols)
        {
            var elementList = SplitOnElementType(mathML, "mo", symbol);
            if (elementList != null)
            {
                return new Element(symbol, false, elementList);
            } 
        }
        return null;
    }

    //checkBrackets Working
    static private Element? checkBrackets(IEnumerable<XmlNode> mathML)
    {
        foreach (XmlNode node in mathML)
        {
            if (node.Name == "mrow")
            {
                debugPrint("Creating Brackets under "+ node.ParentNode.Name );
                //TODO Dont use casting
                return parseMathML(node.ChildNodes.Cast<XmlNode>());
            }
            // if ((elementList = SplitOnElementType(mathML, "mo", "("))!= null)
            // {
            //    return  new Element("/", true,elementList); 
            // }
        }

        return null;
    }

    //checkSubtraction Working
    static private Element? checkSubtraction(IEnumerable<XmlNode> mathML)
    {
        //Different possible symbols for minus
        String[] subtractionSymbols = new string[] {"-","−"};
        foreach (var symbol in subtractionSymbols)
        {
            var elementList = SplitOnElementType(mathML, "mo", symbol);
            if (elementList != null)
            {
                
                if (elementList.Count() == 1)
                {
                    return null;
                }
                debugPrint("Subtracted");
                return new Element("−", false, elementList);
                
            } 
        }

        return null;
    }

    //walkItem Working
    static private Element? walkItem(IEnumerable<XmlNode> mathM)
    {
        foreach (var node in mathM)
        {
            if (node.Name == "semantics")
            {
                //TODO Dont use casting
                return parseMathML(node.ChildNodes.Cast<XmlNode>());
            }
        }

        return null;
    }

    //SplitOnElementType Working
    static private Element[]? SplitOnElementType(IEnumerable<XmlNode> mathML, string elementName, string elementValue)
    {
        
        int nodePos = 0;
        foreach (XmlNode node in mathML)
        {
            
            
            if (node.Name == elementName && node.InnerText == elementValue)
            {
                
                //Split equation into two parts,
                //Left hand side
                IEnumerable<XmlNode> left = mathML.Take(nodePos);
                
                //Right hand side
                IEnumerable<XmlNode> right = mathML.Skip(nodePos + 1);
                
               
               
                if (left.Count() == 0)
                {
                    return new[] {parseMathML(right)};
                }

                return new[] {parseMathML(left), parseMathML(right)};
            }
            
            nodePos++;
        }

        //if the Element with name and value not found return null
        return null; 
    }

    static private Element SplitOnMathOperator(IEnumerable<XmlNode> mathML, string elementName)
    {
        int nodePos = 0;
        foreach (XmlNode node in mathML)
        {
            if (node.Name == elementName)
            {
                //Split equation into two parts,
                //Left hand side
                IEnumerable<XmlNode> left = mathML.Take(nodePos);

                //Right hand side
                IEnumerable<XmlNode> right = mathML.Skip(nodePos + 1);
                Element[] elem;
                if (left.Count() == 0)
                {
                    elem = new[] {parseMathML(right)};
                }

                elem = new[] {parseMathML(left), parseMathML(right)};
                return new Element(node.Value, false, elem);
            }

            nodePos++;
        }

        //if the Element with name and value not found return null
        return null;
    }

    static private XmlNodeList? SplitOnElementType(IEnumerable<XmlNode> mathML, string elementName)
    {
        foreach (XmlNode node in mathML)
        {
            if (node.Name == elementName)
            {
                return node.ChildNodes;
            }
        }

        //if the Element with name and value not found return null
        return null;
    }

    public static void debugPrint(String str)
    {
        if (debug)
        {
            Console.WriteLine(str);
        }
    }

    public String ToString()
    {
        string statement = "Node value = " + _value + ": {";
        if (_elements!=null)
        {
            foreach (var elem in _elements)
            {
                if (elem==null)
                {
                    debugPrint("Error item is null");
                }
                else
                {
                    statement += " " + elem.ToString();
                }
                
            }  
        }
        
        statement += " }";
        return statement;
    }
}