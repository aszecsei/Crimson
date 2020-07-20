using System.Xml;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace Crimson.XmlPipeline
{
    [ContentTypeWriter]
    public class CXmlWriter : ContentTypeWriter<XmlDocument>
    {
        protected override void Write(ContentWriter output, XmlDocument value)
        {
            output.Write(value.OuterXml);
        }

        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return typeof(XmlDocument).AssemblyQualifiedName;
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(XmlDocument).AssemblyQualifiedName;
        }
    }
}