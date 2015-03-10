using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace DefinedContent.UI.Helpers
{
    public class Serialiser
    {
        public static XmlDocument Serialize<T>(T toSerialize)
        {
            return Serialize<T>(toSerialize, null, null);
        } // Serialize 

        public static XmlDocument Serialize<T>(T toSerialize, XmlRootAttribute rootAttribute, string defaultNamespace)
        {
            XmlDocument xmlDoc = new XmlDocument();

            XmlAttributes atts = new XmlAttributes();
            // Set to true to preserve namespaces, 
            // or false to ignore them.
            atts.Xmlns = true;

            XmlAttributeOverrides xover = new XmlAttributeOverrides();
            // Add the XmlAttributes and specify the name of the element 
            // containing namespaces.
            // Create the XmlSerializer using the 

            XmlSerializer xs = (rootAttribute == null) ? new XmlSerializer(typeof(T)) : new XmlSerializer(typeof(T), xover, null, rootAttribute, defaultNamespace);

            MemoryStream serStream = new MemoryStream();

            xs.Serialize(serStream, toSerialize);

            serStream.Seek(0, SeekOrigin.Begin);

            xmlDoc.Load(serStream);

            return xmlDoc;
        } // Serialize  

        public static T Deserialize<T>(XmlDocument toDeserialize)
        {
            return Deserialize<T>(toDeserialize, null, null);
        } // Serialize 

        public static T Deserialize<T>(XmlDocument toDeserialize, XmlRootAttribute rootAttribute, string defaultNamespace)
        {
            XmlSerializer xs = (defaultNamespace == null) ? new XmlSerializer(typeof(T)) : new XmlSerializer(typeof(T), new XmlAttributeOverrides(), null, rootAttribute, defaultNamespace);

            MemoryStream desStream = new MemoryStream();

            toDeserialize.Save(desStream);

            desStream.Seek(0, SeekOrigin.Begin);

            return (T)xs.Deserialize(desStream);
        } // Deserialize   
    }
}
