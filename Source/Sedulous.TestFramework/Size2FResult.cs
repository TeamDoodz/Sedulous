﻿using System;
using System.Drawing;
using NUnit.Framework;

namespace Sedulous.TestFramework
{
    /// <summary>
    /// Represents a unit test result containing a two-dimensional floating point size value.
    /// </summary>
    public sealed class Size2FResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Size2FResult"/> class.
        /// </summary>
        /// <param name="value">The value being examined.</param>
        internal Size2FResult(SizeF value)
        {
            this.value = value;
        }

        /// <summary>
        /// Asserts that this size should have the specified values.
        /// </summary>
        /// <param name="width">The expected width.</param>
        /// <param name="height">The expected height.</param>
        /// <returns>The result object.</returns>
        public Size2FResult ShouldBe(Single width, Single height)
        {
            Assert.AreEqual(width, value.Width, delta);
            Assert.AreEqual(height, value.Height, delta);
            return this;
        }

        /// <summary>
        /// Specifies that subsequent comparisons should be within the specified delta.
        /// </summary>
        /// <param name="delta">The delta value to set.</param>
        /// <returns>The result object.</returns>
        public Size2FResult WithinDelta(Single delta)
        {
            this.delta = delta;
            return this;
        }

        /// <summary>
        /// Gets the underlying value.
        /// </summary>
        public SizeF Value
        {
            get { return value; }
        }

        // State values.
        private readonly SizeF value;
        private Single delta = 0.001f;
    }
}
