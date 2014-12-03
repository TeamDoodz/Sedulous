﻿using System;

namespace TwistedLogik.Ultraviolet.Layout
{
    /// <summary>
    /// Represents a dependency property's value.
    /// </summary>
    internal interface IDependencyPropertyValue
    {
        /// <summary>
        /// Evaluates whether the dependency property's value has changed and, if so, invokes the appropriate callbacks.
        /// </summary>
        void Digest();

        /// <summary>
        /// Clears the dependency property's local value, if it has one.
        /// </summary>
        void ClearLocalValue();

        /// <summary>
        /// Clears the dependency property's styled value, if it has one.
        /// </summary>
        void ClearStyledValue();

        /// <summary>
        /// Gets or sets the dependency object which owns this property value.
        /// </summary>
        DependencyObject Owner
        {
            get;
        }

        /// <summary>
        /// Gets the dependency property which has its value represented by this object.
        /// </summary>
        DependencyProperty Property
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether the dependency property is bound to a property on a model object.
        /// </summary>
        Boolean IsDataBound
        {
            get;
        }
    }
}
