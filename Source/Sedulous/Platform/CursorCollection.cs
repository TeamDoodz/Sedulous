﻿using System;
using System.Collections.Generic;
using Sedulous.Core;

namespace Sedulous
{
    /// <summary>
    /// Represents a collection of named <see cref="Cursor"/> objects.
    /// </summary>
    public sealed class CursorCollection : FrameworkResource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CursorCollection"/> class.
        /// </summary>
        /// <param name="context">The Framework context.</param>
        internal CursorCollection(FrameworkContext context)
            : base(context)
        {

        }

        /// <summary>
        /// Gets the <see cref="Cursor"/> with the specified name.
        /// </summary>
        /// <param name="name">The name of the <see cref="Cursor"/> to retrieve.</param>
        /// <returns>The <see cref="Cursor"/> with the specified name, or <see langword="null"/> if no such <see cref="Cursor"/> exists.</returns>
        public Cursor this[String name]
        {
            get
            {
                Cursor cursor;
                cursors.TryGetValue(name, out cursor);
                return cursor;
            }
            internal set
            {
                Contract.RequireNotEmpty(name, nameof(name));
                Contract.Require(value, nameof(value));

                cursors[name] = value;
            }
        }

        /// <summary>
        /// Releases resources associated with the object.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> if the object is being disposed; <see langword="false"/> if the object is being finalized.</param>
        protected override void Dispose(Boolean disposing)
        {
            if (Disposed)
                return;

            foreach (var cursor in cursors)
                cursor.Value.Dispose();

            cursors.Clear();

            base.Dispose(disposing);
        }

        // The cursor registry.
        private readonly Dictionary<String, Cursor> cursors = 
            new Dictionary<String, Cursor>();
    }
}
