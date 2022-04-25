using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Xsl;
using NUnit.Framework;
using MathClasification;


namespace MathClasificationTest;

public class Tests
{
    [SetUp]
    public void Setup()
    {
        Element startElement;
    }

    [Test]
    public void ParseFromXMLDocument()
    {
        XmlUrlResolver resolver = new XmlUrlResolver();
        resolver.Credentials = CredentialCache.DefaultCredentials;
        
        XmlDocument doc = new XmlDocument();
        XmlReaderSettings readerSettings = new XmlReaderSettings();
        readerSettings.XmlResolver = resolver;
        readerSettings.DtdProcessing = DtdProcessing.Parse;
        readerSettings.ValidationType = ValidationType.DTD;
        readerSettings.ValidationEventHandler += new ValidationEventHandler (ValidationCallBack);
        
        using (XmlReader reader = XmlReader.Create("MathTest1.ml", readerSettings)) 
        {
            while (reader.Read())
            {
                reader.MoveToContent();
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.Load(reader);
                // XmlNode? node = xmldoc.ReadNode(reader);
                var parent = xmldoc.ChildNodes;
                Console.WriteLine(parent.Count);

                var parent2 = parent.Item(0).Cast<XmlNode>();
                Console.WriteLine(parent2);
                foreach (var ty in parent2)
                {
                    var children = ty.ChildNodes;
                    Console.WriteLine(children.Count);
                    foreach (XmlNode child in children)
                    {
                        
                        Console.WriteLine(child.Name);
                    }
                }
            }
           

        }

        
        // XslCompiledTransform xslt = new XslCompiledTransform();
        // xslt.Load( reader );
        // reader.ResolveEntity();
        // Console.WriteLine(reader.SchemaInfo);
        // 
        // doc.Load("MathTest1.ml");
        // SimilarityIndex si = new SimilarityIndex(doc);
        Assert.IsTrue(true);
        Assert.Pass();
    }
    
    [Test]
    public void ParseFromXMLFile()
    {
        try
        {
            var elements = MathClasification.Element.parseMathMLFile("MathTest1.ml");
            foreach (var element in elements)
            {
                Console.WriteLine(element.ToString());
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            Assert.Fail();
        }
        Assert.Pass();
    }
    private static void ValidationCallBack(object sender, ValidationEventArgs e) {
        Console.WriteLine("Validation Error: {0}", e.Message);
    }
}