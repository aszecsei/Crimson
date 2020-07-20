using System;
using System.ComponentModel;
using Microsoft.Xna.Framework.Content.Pipeline;
using System.Xml;

namespace Crimson.XmlPipeline
{
    [ContentProcessor(DisplayName = "XML Processor - Crimson")]

    public class CXmlProcessor : ContentProcessor<XmlImporterResult, XmlDocument> {

        public override XmlDocument Process(XmlImporterResult input, ContentProcessorContext context) {
            XmlDocument xmlDoc = new XmlDocument();

            try {
                xmlDoc.LoadXml(input.Text);
                return xmlDoc;
            }
            catch (Exception ex) {
                context.Logger.LogMessage("Error {0}", ex);
                throw;
            }
        }
    }
}
