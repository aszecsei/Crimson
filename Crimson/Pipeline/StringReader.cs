using Microsoft.Xna.Framework.Content;

namespace Crimson.Pipeline
{
    public class StringReader : ContentTypeReader<string>
    {
        protected override string Read(ContentReader input, string existingInstance)
        {
            return input.ReadString();
        }
    }
}
