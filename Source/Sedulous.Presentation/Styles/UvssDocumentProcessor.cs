﻿using System;
using System.IO;
using Sedulous.Content;
using Sedulous.Presentation.Uvss;
using Sedulous.Presentation.Uvss.Syntax;

namespace Sedulous.Presentation.Styles
{
    /// <summary>
    /// Represents a content processor for the *.uvss file type.
    /// </summary>
    //[ContentProcessor]
    public class UvssDocumentProcessor : ContentProcessor<String, UvssDocument>
    {
        /// <inheritdoc/>
        public override UvssDocument Process(ContentManager manager, IContentProcessorMetadata metadata, String input)
        {
            return UvssDocument.Compile(manager.FrameworkContext, input);
        }

        /// <inheritdoc/>
        public override void ExportPreprocessed(ContentManager manager, 
            IContentProcessorMetadata metadata, BinaryWriter writer, String input, Boolean delete)
        {            
            const Int32 FileVersion = 1;
            writer.Write(FileVersion);

            var ast = UvssParser.Parse(input);
            SyntaxSerializer.ToStream(writer, ast, FileVersion);
        }

        /// <inheritdoc/>
        public override UvssDocument ImportPreprocessed(ContentManager manager,
            IContentProcessorMetadata metadata, BinaryReader reader)
        {
            var version = reader.ReadInt32();
            if (version != 1)
                throw new InvalidDataException();
            
            var ast = (UvssDocumentSyntax)SyntaxSerializer.FromStream(reader, version);           
            return UvssCompiler.Compile(manager.FrameworkContext, ast);
        }

        /// <inheritdoc/>
        public override Boolean SupportsPreprocessing => true;
    }
}
