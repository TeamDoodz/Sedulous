﻿using System;

namespace Sedulous.Graphics.Graphics2D.Text
{
    partial class ShapedString
    {
        /// <inheritdoc/>
        public void GetChar(Int32 index, out ShapedChar ch) => ch = buffer[index];

        /// <inheritdoc/>
        public Boolean IsNull => false;

        /// <inheritdoc/>
        public Boolean IsEmpty => Length == 0;

        /// <inheritdoc/>
        public ShapedStringSegment CreateShapedStringSegment() =>
            new ShapedStringSegment(this);

        /// <inheritdoc/>
        public ShapedStringSegment CreateShapedStringSegmentFromSubstring(Int32 start, Int32 length) =>
            new ShapedStringSegment(this, start, length);

        /// <inheritdoc/>
        public ShapedStringSegment CreateShapedStringSegmentFromSameOrigin(Int32 start, Int32 length) =>
            new ShapedStringSegment(this, start, length);
    }
}
