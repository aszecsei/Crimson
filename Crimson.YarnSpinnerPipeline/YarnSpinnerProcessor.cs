﻿using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Crimson.YarnSpinner;
using Microsoft.Xna.Framework.Content.Pipeline;
using Yarn.Compiler;

namespace Crimson.YarnSpinnerPipeline
{
    [ContentProcessor(DisplayName = "YarnSpinner Processor - Crimson")]
    public class YarnSpinnerProcessor : ContentProcessor<YarnSpinnerFile, YarnProgram>
    {
        public override YarnProgram Process(YarnSpinnerFile input, ContentProcessorContext context)
        {
            string baseLanguageId = CultureInfo.CurrentCulture.Name;

            try
            {
                context.Logger.LogMessage("Compiling Yarn program {0}", input.FileName);

                var compilationStatus =
                    Compiler.CompileString(input.Text, input.FileName, out var compiledProgram, out var stringTable);
                if (compilationStatus == Status.SucceededUntaggedStrings)
                {
                    context.Logger.LogMessage("Yarn compilation includes untagged strings.");
                }
                
                var programContainer = new YarnProgram();

                using (var memoryStream = new MemoryStream())
                using (var outputStream = new Google.Protobuf.CodedOutputStream(memoryStream))
                {
                    // Serialize the compiled program to memory
                    compiledProgram.WriteTo(outputStream);
                    outputStream.Flush();

                    byte[] compiledBytes = memoryStream.ToArray();
                    programContainer.CompiledProgram = compiledBytes;
                }

                using (var memoryStream = new MemoryStream())
                {
                    using (var textWriter = new StreamWriter(memoryStream))
                    {
                        // Generate the localized .csv file

                        // Use the invariant culture when writing the CSV
                        var configuration = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture);

                        var csv = new CsvHelper.CsvWriter(textWriter, configuration);
                        var lines = stringTable.Select(x => new
                        {
                            id = x.Key,
                            text = x.Value.text,
                            file = x.Value.fileName,
                            node = x.Value.nodeName,
                            lineNumber = x.Value.lineNumber
                        });

                        csv.WriteRecords(lines);

                        textWriter.Flush();

                        memoryStream.Position = 0;

                        using (var reader = new StreamReader(memoryStream))
                        {
                            string text = reader.ReadToEnd();
                            string textFileName = $"{input.FileName} ({baseLanguageId})";

                            programContainer.BaseLocalizationId = baseLanguageId;
                            programContainer.BaseLocalisationStringTable = text;
                            programContainer.Localizations = new YarnTranslation[0];
                        }
                    }
                }

                return programContainer;
            }
            catch (Exception ex)
            {
                context.Logger.LogMessage("Error {0}", ex);
                throw;
            }
        }
    }
}