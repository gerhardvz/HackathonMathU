using System.Xml;
namespace MathClasification;

public class Element
{
   protected Element[] _elements;
   protected Boolean _canSwap;
   protected object _value;
   
   protected Element(object value,Element[] elements)
   {
      this._value = value;
      this._elements = elements;
   }

    static Element parseMathML(IEnumerable<XmlNode> mathML)
    {
   
       //Search for the position of the '=' in the calculation - should be on top level
       //Only happens once except if there is an error
       int nodePos=0;
       foreach (XmlNode node in mathML)
       {
          if (node.Name == "mo" && node.Value=="=")
          {
             //ToDo: Callback Function
             
             
             //Split equation into two parts,
             //Left hand side
             IEnumerable<XmlNode> left = mathML.Take(nodePos);
             //Right hand side
             IEnumerable<XmlNode> right = mathML.Skip(nodePos+1);

             var leftElement = parseMathML(left);
             var rightElement = parseMathML(right);
             return new Element("=", new[] {leftElement, rightElement});
          }
   
          nodePos++;
       }
       // look at <mo> and split into left and right
        nodePos=0;
       foreach (XmlNode node in mathML)
       {
          if (node.Name == "mo")
          {
             string value = node.Value;
             //ToDo: Check if value is '-' and if there is a value left and right
             
             
             //Split equation into two parts,
             //Left hand side
             IEnumerable<XmlNode> left = mathML.Take(nodePos);
             //Right hand side
             IEnumerable<XmlNode> right = mathML.Skip(nodePos+1);

             var leftElement = parseMathML(left);
             var rightElement = parseMathML(right);
             return new Element(value, new[] {leftElement, rightElement});
          }
   
          nodePos++;
       }
       
       //Test for fractions
       //Must contain two node trees
       nodePos=0;
       foreach (XmlNode node in mathML)
       {
          if (node.Name == "mfrac" )
          {
             //First Child (top)
             IEnumerable<XmlNode> left = mathML.Take(nodePos);
             //Second Child (Bottom)
             IEnumerable<XmlNode> right = mathML.Skip(nodePos+1);

             var leftElement = parseMathML(left);
             var rightElement = parseMathML(right);
             return new Element(value, new[] {leftElement, rightElement});
          }
   
          nodePos++;
       }
       
       
       

       
       
       //Test if fragment has Child node, if not it is not the following functions (mrow,mfrac.ec)
       if (!mathML.HasChildNodes)
       {
          
       }
   
       XmlNode childNode = mathML.FirstChild;
       while (childNode !=mathML.LastChild)
       {
          if (childNode.HasChildNodes)
          {
             Element treeElements = Element.parseMathML(childNode);
          }
       }
       //
       
       return new Element();
    }

    static private Element parseFraction(XmlNode fracNode)
    {
       if (fracNode.ChildNodes.Count != 2)
       {
          throw new Exception("Invalid Fraction node given");
       }
   
       int count = 0;
       Element[] elements = new Element[2];
       foreach (XmlNode nodeChild in fracNode.ChildNodes)
       {
          Element element;
          if (isContained(nodeChild))
          {
             element = parseMathML(nodeChild);
          }
          else
          {
             if (nodeChild.Name=="mo")
             {
                throw new Exception("Invalid Expression, cant have an operator at this position");
             }
             if (nodeChild.Name=="mtext")
             {
                throw new Exception("Invalid Expression, cant have an text at this position");
             }
             if (nodeChild.Name=="mn")
             {
                Number number = new Number(nodeChild.Value);
             }
             
          }
   
   
          elements[count++] = element;
       }
    }

   // Todo: Method that returns an number or identifier


    static private Element parseOperator(XmlDocument node)
    {
       var nodeList = node.GetElementsByTagName("mo");
       
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
   
   
}