﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml.Linq;
using Sedulous.Content;
using Sedulous.Core.Xml;

namespace Sedulous
{
    /// <summary>
    /// Represents a content processor that loads cursor collections.
    /// </summary>
    //[ContentProcessor]
    internal sealed class CursorCollectionProcessorFromXDocument : ContentProcessor<XDocument, CursorCollection>
    {
        /// <inheritdoc/>
        public override CursorCollection Process(ContentManager manager, IContentProcessorMetadata metadata, XDocument input)
        {
            var collectionDesc = new CursorCollectionDescription();
            var collectionCursors = new List<CursorDescription>();

            var image = input.Root.AttributeValueString("Image");
            if (String.IsNullOrEmpty(image))
                throw new InvalidOperationException(FrameworkStrings.InvalidCursorImage);
            
            collectionDesc.Texture = image;
            collectionDesc.Cursors = collectionCursors;

            foreach (var cursorElement in input.Root.Elements("Cursor"))
            {
                var name = cursorElement.AttributeValueString("Name");
                if (String.IsNullOrEmpty(name))
                    throw new InvalidOperationException(FrameworkStrings.InvalidCursorName);

                var position = cursorElement.AttributeValue<Point>("Position");
                var size = cursorElement.AttributeValue<Size>("Size");
                var hotspot = cursorElement.AttributeValue<Point>("Hotspot");

                var cursorDesc = new CursorDescription();
                cursorDesc.Name = name;
                cursorDesc.Area = new Rectangle(position.X, position.Y, size.Width, size.Height);
                cursorDesc.Hotspot = hotspot;

                collectionCursors.Add(cursorDesc);
            }

            return innerProcessor.Process(manager, metadata, collectionDesc);
        }

        private static readonly CursorCollectionProcessor innerProcessor =
            new CursorCollectionProcessor();
    }
}
