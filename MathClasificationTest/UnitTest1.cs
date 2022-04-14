using System;
using System.IO;
using System.Xml;
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
        XmlDocument doc = new XmlDocument();
        XmlReaderSettings readerSettings = new XmlReaderSettings();
        readerSettings.DtdProcessing = DtdProcessing.Parse;
        readerSettings.ValidationType = ValidationType.DTD;
        readerSettings.ValidationEventHandler += new ValidationEventHandler (ValidationCallBack);
        XmlReader reader = XmlReader.Create("MathTest1.ml",readerSettings);
        XslCompiledTransform xslt = new XslCompiledTransform();
        xslt.Load( reader );
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