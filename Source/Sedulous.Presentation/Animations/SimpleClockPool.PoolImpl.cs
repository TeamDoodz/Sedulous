﻿using System;

namespace Sedulous.Presentation.Animations
{
    partial class SimpleClockPool
    {
        /// <summary>
        /// Represents a pool of <see cref="SimpleClock"/> objects.
        /// </summary>
        private sealed class PoolImpl : UpfPool<SimpleClock>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="PoolImpl"/> class.
            /// </summary>
            /// <param name="context">The Sedulous context.</param>
            /// <param name="capacity">The pool's initial capacity.</param>
            /// <param name="watermark">The pool's high watermark value.</param>
            /// <param name="allocator">The pool's allocator function.</param>
            public PoolImpl(FrameworkContext context, Int32 capacity, Int32 watermark, Func<SimpleClock> allocator)
                : base(context, capacity, watermark, allocator) { }

            /// <inheritdoc/>
            protected override void OnCleaningUpObject(SimpleClock @object)
            {
                @object.Stop();
                base.OnCleaningUpObject(@object);
            }
        }
    }
}
