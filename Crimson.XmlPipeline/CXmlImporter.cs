using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using System.Xml;
using System.Xml.Serialization;

namespace Crimson.XmlPipeline
{
    [ContentImporter(".xml", DefaultProcessor = "CXmlProcessor", DisplayName = "XML Importer - Crimson")]
    public class CXmlImporter : ContentImporter<XmlImporterResult>
    {
        public override XmlImporterResult Import(string filename, ContentImporterContext context)
        {
            context.Logger.LogMessage("Importing Xml file: {0}", filename);

            return new XmlImporterResult()
            {
                Text = File.ReadAllText(filename),
                FileName = filename
            };
        }
    }
}