using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace AggieGlobal.WebApi.Common
{
    public static class DictionaryExtension
    {
        public static string XMLSerialize(this IDictionary<string, string> ItemCollection)
        {
            string strXML = null;
            if (ItemCollection == null || ItemCollection.Count == 0) return strXML;

            try
            {
                StringBuilder SB = new StringBuilder();
                XmlWriterSettings settings = new XmlWriterSettings();
                //settings.Encoding = UTF8Encoding.UTF8;
                settings.OmitXmlDeclaration = true;
                using (XmlWriter writer = XmlTextWriter.Create(SB, settings))
                {
                    // Write the root element 
                    writer.WriteStartElement("dictionary");
                    // Foreach object in this (as i am a Hashtable)
                    foreach (string key in ItemCollection.Keys)
                    {
                        string value = ItemCollection[key];
                        // Write item, key and value
                        writer.WriteStartElement("item");
                        writer.WriteElementString("key", key.ToString());
                        writer.WriteElementString("value", !string.IsNullOrEmpty(value) ? value.ToString() : string.Empty);
                        // write </item>
                        writer.WriteEndElement();
                    }
                    // write </dictionary>
                    writer.WriteEndElement();
                    writer.Close();

                    strXML = SB.ToString();
                }
            }
            catch (Exception)
            {
                //throw;
            }
            return strXML;
        }
        public static IDictionary<string, string> XMLDeSerialize(this IDictionary<string, string> ItemCollection, string XmlSource)
        {
            IDictionary<string, string> objRet = null;
            if (!string.IsNullOrEmpty(XmlSource))
            {
                try
                {
                    using (StringReader reader = new StringReader(XmlSource))
                    {
                        using (XmlReader xmlReader = new XmlTextReader(reader))
                        {
                            objRet = new Dictionary<string, string>();
                            // Start to use the reader.
                            xmlReader.Read();
                            // Read the first element i.e. root of this object
                            xmlReader.ReadStartElement("dictionary");

                            string key = default(string);
                            string value = default(string);
                            // Read all elements
                            while (xmlReader.NodeType != XmlNodeType.EndElement)
                            {

                                // parsing the item
                                xmlReader.ReadStartElement("item");
                                // Parsing the key and value 
                                key = xmlReader.ReadElementString("key");
                                value = xmlReader.ReadElementString("value");
                                // end reading the item.
                                xmlReader.ReadEndElement();
                                xmlReader.MoveToContent();
                                // add the item
                                objRet[key] = value;
                            }
                            // Extremely important to read the node to its end.
                            // next call of the reader methods will crash if not called.
                            xmlReader.ReadEndElement();
                            xmlReader.Close();
                        }
                        reader.Close();
                    }
                }
                catch (Exception)
                {
                    //throw;
                }
            }
            return objRet;
        }
        public static IDictionary<string, string> XMLDeSerialize(this string XmlSource)
        {
            IDictionary<string, string> objRet = new Dictionary<string, string>();
            return objRet.XMLDeSerialize(XmlSource);
        }
    }
}
