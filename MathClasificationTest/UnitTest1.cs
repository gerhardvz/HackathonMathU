using System;
using System.IO;
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
    public void Test1()
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
                Console.WriteLine(xmldoc.InnerXml);
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
    private static void ValidationCallBack(object sender, ValidationEventArgs e) {
        Console.WriteLine("Validation Error: {0}", e.Message);
    }
}