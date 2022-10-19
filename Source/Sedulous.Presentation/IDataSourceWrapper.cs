﻿using System;

namespace Sedulous.Presentation
{
    /// <summary>
    /// Represents an object which wraps some underlying data source.
    /// </summary>
    public interface IDataSourceWrapper
    {
        /// <summary>
        /// Gets the data source which is being wrapped by this object.
        /// </summary>
        Object WrappedDataSource
        {
            get;
        }
    }
}
