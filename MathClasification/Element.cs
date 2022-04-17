using System.Xml;
namespace MathClasification;

public class Element
{
   protected Element[] _elements;
   protected Boolean _canSwap;
   protected object _value;
   
   
   protected Element(object value, Boolean swappable,Element[] elements)
   {
      this._value = value;
      this._elements = elements;
      this._canSwap = swappable;
   }

    static Element parseMathML(IEnumerable<XmlNode> mathML)
    {
       var OrderofCalculation = new Dictionary<String,Boolean>(){{"=",true}, {"+",true}, {"-",false}, {"/",false}, {"*",true}};
       var MathFunctions = new List<Func<IEnumerable<XmlNode> , Element>>() { 
          checkAddition,
          checkSubtraction,
          checkDivision,
          checkMultiplication
          
       };
       //Search for the position of the '=' in the calculation - should be on top level
       //Only happens once except if there is an error
       int nodePos;
       foreach (var order in MathFunctions)
       {
          var elm = order(mathML);
          if ( elm is not null)
          {
             return elm;
          }

          
       }
      //
      // if code reaches this stage there was invalid mathml
      throw new Exception("MathML doc was read and format was incorrect.");
       // look at <mo> and split into left and right
       //  nodePos=0;
       // foreach (XmlNode node in mathML)
       // {
       //    if (node.Name == "mo")
       //    {
       //       string value = node.Value;
       //       //ToDo: Check if value is '-' and if there is a value left and right
       //       
       //       
       //       //Split equation into two parts,
       //       //Left hand side
       //       IEnumerable<XmlNode> left = mathML.Take(nodePos);
       //       //Right hand side
       //       IEnumerable<XmlNode> right = mathML.Skip(nodePos+1);
       //
       //       var leftElement = parseMathML(left);
       //       var rightElement = parseMathML(right);
       //       return new Element(value, new[] {leftElement, rightElement});
       //    }
       //
       //    nodePos++;
       // }
       //
       // //Test for fractions
       // //Must contain two node trees
       // nodePos=0;
       // foreach (XmlNode node in mathML)
       // {
       //    if (node.Name == "mfrac" )
       //    {
       //       //First Child (top)
       //       IEnumerable<XmlNode> left = mathML.Take(nodePos);
       //       //Second Child (Bottom)
       //       IEnumerable<XmlNode> right = mathML.Skip(nodePos+1);
       //
       //       var leftElement = parseMathML(left);
       //       var rightElement = parseMathML(right);
       //       return new Element(value, new[] {leftElement, rightElement});
       //    }
       //
       //    nodePos++;
       // }
       //
       //
       //
       //
       //
       //
       // //Test if fragment has Child node, if not it is not the following functions (mrow,mfrac.ec)
       // if (!mathML.HasChildNodes)
       // {
       //    
       // }
       //
       // XmlNode childNode = mathML.FirstChild;
       // while (childNode !=mathML.LastChild)
       // {
       //    if (childNode.HasChildNodes)
       //    {
       //       Element treeElements = Element.parseMathML(childNode);
       //    }
       // }
       // //
       //
       // return new Element();
    }

    // static private Element parseFraction(XmlNode fracNode)
    // {
    //    if (fracNode.ChildNodes.Count != 2)
    //    {
    //       throw new Exception("Invalid Fraction node given");
    //    }
    //
    //    int count = 0;
    //    Element[] elements = new Element[2];
    //    foreach (XmlNode nodeChild in fracNode.ChildNodes)
    //    {
    //       Element element;
    //       if (isContained(nodeChild))
    //       {
    //          element = parseMathML(nodeChild);
    //       }
    //       else
    //       {
    //          if (nodeChild.Name=="mo")
    //          {
    //             throw new Exception("Invalid Expression, cant have an operator at this position");
    //          }
    //          if (nodeChild.Name=="mtext")
    //          {
    //             throw new Exception("Invalid Expression, cant have an text at this position");
    //          }
    //          if (nodeChild.Name=="mn")
    //          {
    //             Number number = new Number(nodeChild.Value);
    //          }
    //          
    //       }
    //
    //
    //       elements[count++] = element;
    //    }
    // }

   // Todo: Method that returns an number or identifier
   static private Element checkNumber(XmlNode node)
   {
      if (node.Name == "mn")
      {
         return new Number(node.Value);
      }

      throw new Exception("Not a valid number.");
   }
   
   //isContained returns false if the element type is one of the folowing {mi,mo,mn,mtext}
   static private bool isContained(XmlNode element)
   {
      string name = element.Name;
      switch (name)
      {
         case "mi": return false; break;
         case "mn": return false; break;
         case "mo": return false; break;
         case "mtext": return false; break;
      }

      return true;
   }
   
   static private Element? checkEquals(IEnumerable<XmlNode> mathML)
   {
      var elementList = SplitOnElementType(mathML, "mo", "=");
      if (elementList == null)
      {
         return  null;
      }
      
      return  new Element("=", true,elementList);;
   }
   
   static private Element checkSquarroot(IEnumerable<XmlNode> mathML)
   {
      var elementList = SplitOnElementType(mathML, "msqrt");
      if (elementList==null)
      {
         return  null;
      }
      
      return  new Element("sqrt", true,elementList);;
   }
   static private Element checkPower(IEnumerable<XmlNode> mathML)
   {
      var elementList = SplitOnElementType(mathML, "msup");
      if (elementList==null)
      {
         return  null;
      }
      
      return  new Element("power", true,elementList);;
   }
   
   static private Element checkMultiplication(IEnumerable<XmlNode> mathML)
   {
      var elementList = SplitOnElementType(mathML, "mo", "x");
      if (elementList==null)
      {
         return  null;
      }
      
      return  new Element("x", true,elementList);;
   }
   static private Element checkDivision(IEnumerable<XmlNode> mathML)
   {
      var elementList = SplitOnElementType(mathML, "mo", "/");
      if (elementList!=null)
      {
         return  new Element("/", false,elementList);
      }
   
      //TODO:Split on frac
      
      return  new Element("/", false,elementList);;
   }
   
   static private Element checkAddition(IEnumerable<XmlNode> mathML)
   {
      var elementList = SplitOnElementType(mathML, "mo", "+");
      if (elementList != null)
      {
         return new Element("+", true, elementList);
      }

      return  null;
   }
   
   static private Element checkSubtraction(IEnumerable<XmlNode> mathML)
   {
      var elementList = SplitOnElementType(mathML, "mo", "-");
      if (elementList == null)
      {
         return null;
      }

      if (elementList.Count() == 1)
      {
         //Then this mo element acts as a negative indicator
         //TODO: Implement negative indicator
         // return element as a negative value
         return new Element("-", false, elementList);
      }

      return new Element("-", false, elementList);
   }
   
   static private Element[]? SplitOnElementType(IEnumerable<XmlNode> mathML,string elementName, string elementValue)
   {
      int nodePos=0;
      foreach (XmlNode node in mathML)
      {
         if (node.Name == elementName && node.Value==elementValue)
         {
            //Split equation into two parts,
            //Left hand side
            IEnumerable<XmlNode> left = mathML.Take(nodePos);
            //Right hand side
            IEnumerable<XmlNode> right = mathML.Skip(nodePos+1);
            if (left.Count()==0)
            {
               return new [] { parseMathML(right)};
            }
            return new [] {parseMathML(left), parseMathML(right)};
         }
   
         nodePos++;
      }
      //if the Element with name and value not found return null
      return null
      ;
   }
   
   static private Element[]? SplitOnElementType(IEnumerable<XmlNode> mathML,string elementName)
   {
      int nodePos=0;
      foreach (XmlNode node in mathML)
      {
         if (node.Name == elementName)
         {
            //Split equation into two parts,
            //Left hand side
            IEnumerable<XmlNode> left = mathML.Take(nodePos);
            
            //Right hand side
            IEnumerable<XmlNode> right = mathML.Skip(nodePos+1);
            if (left.Count()==0)
            {
               return new [] { parseMathML(right)};
            }
            return new [] {parseMathML(left), parseMathML(right)};
         }
   
         nodePos++;
      }
      //if the Element with name and value not found return null
      return null;
   }
}