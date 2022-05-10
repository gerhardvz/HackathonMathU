using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Xsl;
using NUnit.Framework;
using MathClasification;


namespace MathClasificationTest;
///<summary>
/// This class is used for testing the MathClasification Library
/// </summary>
public class Tests
{
    [SetUp]
    public void Setup()
    {
        Element startElement;
    }

    /// <summary>
    /// ParseFromXMLDocument is a Test Method to read from a predefined file
    /// named "MathTest1.ml" in the running directory
    /// </summary>
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
    
    /// <summary>
    /// ParseFromXMLDocument is a Test Method to read from a predefined list of files
    /// named "MathTestX.ml" where X is the number from 1 to 5 in the running directory
    /// </summary>
    [Test]
    public void ParseFromXMLFile()
    {

        List<Element> elements1 = new List<Element>();
        try
        {
            for (int i = 1; i <= 5; i++)
            {
                System.Console.WriteLine("Testing file: "+i);
                var elm = Element.parseMathMLFile("MathTest" + i + ".ml");
                if (elm!=null)
                {
                    elements1.Add(elm);
                }
                
            }
            
            
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            Assert.Fail();
        }

        foreach (var element in elements1)
        {
           Console.WriteLine(@"Files are {0}% Similar", elements1[0].Similarity(element)*100);
           var list = new List<List<Int64>>();
           list.Add(element.Hash_M3());
           saveHashArray(list,"3");
        }
        
        Assert.Pass();
    }
    private static void ValidationCallBack(object sender, ValidationEventArgs e) {
        Console.WriteLine("Validation Error: {0}", e.Message);
    }
    

    private float calcSame(List<Int64> a, List<Int64> b)
    {
        var count = 0;
        foreach (var hashb in b)
        {

            var pos = a.IndexOf(hashb);
                if (pos>=0)
                {
                    count++;
                }
            
        }
        Console.WriteLine(@"Count {0}, amount {1}",count,a.Count);

        return (float)count / (float)a.Count;
    }

    
    /// <summary>
    /// Saves the list of HashArray's to a defined file name
    /// </summary>
    /// <param name="lists"> List of HashArray's</param>
    /// <param name="name"> File Name</param>
    private void saveHashArray(List<List<Int64>> lists,string name)
    {
        // var hashlist = File.Create("HashList.csv");
        List<string> lines = new List<string>();
        foreach (var list in lists)
        {
            string sHashList = "";
            foreach (var hash in list)
            {
                
            sHashList+=hash+",";
            }

            lines.Add(sHashList);
        }
        
        File.AppendAllLines("HashList"+name+".csv", lines.ToArray());
    }
}