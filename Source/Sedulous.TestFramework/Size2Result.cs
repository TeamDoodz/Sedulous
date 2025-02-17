﻿using System;
using System.Drawing;
using NUnit.Framework;

namespace Sedulous.TestFramework
{
    /// <summary>
    /// Represents a unit test result containing a two-dimensional size value.
    /// </summary>
    public sealed class Size2Result
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Size2Result"/> class.
        /// </summary>
        /// <param name="value">The value being examined.</param>
        internal Size2Result(Size value)
        {
            this.value = value;
        }

        /// <summary>
        /// Asserts that this size should have the specified values.
        /// </summary>
        /// <param name="width">The expected width.</param>
        /// <param name="height">The expected height.</param>
        /// <returns>The result object.</returns>
        public Size2Result ShouldBe(Int32 width, Int32 height)
        {
            Assert.AreEqual(width, value.Width);
            Assert.AreEqual(height, value.Height);
            return this;
        }
        
        /// <summary>
        /// Gets the underlying value.
        /// </summary>
        public Size Value
        {
            get { return value; }
        }

        // State values.
        private readonly Size value;
    }
}
