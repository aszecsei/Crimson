using Yarn;

namespace Crimson.YarnSpinner
{
    /// <summary>
    /// Stores compiled Yarn programs in a form that Crimson can use
    /// </summary>
    public class YarnProgram
    {
        public byte[] CompiledProgram;

        public string BaseLocalisationStringTable;

        public string BaseLocalizationId;
        
        public YarnTranslation[] Localizations = new YarnTranslation[0];

        /// <summary>
        /// Deserializes a compiled Yarn program from the stored bytes in this object
        /// </summary>
        public Program GetProgram()
        {
            return Program.Parser.ParseFrom(CompiledProgram);
        }
    }
}