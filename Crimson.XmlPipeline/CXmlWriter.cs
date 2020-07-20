using System.Xml;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Crimson.Pipeline;

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
            return typeof(string).AssemblyQualifiedName;
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(StringReader).AssemblyQualifiedName;
        }
    }
}