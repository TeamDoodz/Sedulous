﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Sedulous.Presentation.Controls
{
    partial class ItemCollection : IEnumerable<Object>
    {
        /// <inheritdoc/>
        public IEnumerator<Object> GetEnumerator()
        {
            return IsBoundToItemsSource ? itemsSource.Cast<Object>().GetEnumerator() : itemsStorage.GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
