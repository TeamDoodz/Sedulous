﻿using System;

namespace Sedulous.Core.Data
{
    /// <summary>
    /// Represents an object that has been assigned a globally-unique identification value.
    /// </summary>
    public interface IGloballyIdentified
    {
        /// <summary>
        /// Gets the object's globally-unique identifier.
        /// </summary>
        Guid GlobalId
        {
            get;
        }
    }
}
