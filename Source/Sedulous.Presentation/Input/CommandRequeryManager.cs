﻿using System;
using System.Collections.Generic;
using System.Threading;
using Sedulous.Core.Collections;

namespace Sedulous.Presentation.Input
{
    /// <summary>
    /// Maintains a list of weak references to objects which are listening to the <see cref="CommandManager.RequerySuggested"/> event.
    /// </summary>
    internal sealed class CommandRequeryManager : FrameworkResource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandRequeryManager"/> class.
        /// </summary>
        /// <param name="context">The Sedulous context.</param>
        /// <param name="owner">The command manager that owns this instance.</param>
        public CommandRequeryManager(FrameworkContext context, CommandManager owner)
            : base(context)
        {
            this.owner = owner;
            context.Updating += Updating;
        }
        
        /// <summary>
        /// Adds an event handler to the listener list.
        /// </summary>
        /// <param name="handler">The event handler to add to the list.</param>
        public void Add(EventHandler handler)
        {
            listenersLock.EnterWriteLock();
            try
            {
                var weakref = refpool.Retrieve();
                weakref.Target = handler;
                listeners.Add(weakref);
                needsCleanup = true;
            }
            finally
            {
                listenersLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Removes an event handler from the listener list.
        /// </summary>
        /// <param name="handler">The event handler to remove from the list.</param>
        public void Remove(EventHandler handler)
        {
            listenersLock.EnterWriteLock();
            try
            {
                for (int i = 0; i < listeners.Count; i++)
                {
                    var reference = listeners[i];
                    if (ReferenceEquals(reference.Target, handler))
                    {
                        var weakref = listeners[i];
                        listeners.RemoveAt(i);
                        refpool.Release(weakref);
                        needsCleanup = true;
                        break;
                    }
                }
            }
            finally
            {
                listenersLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Removes dead references from the list of listeners.
        /// </summary>
        public void Cleanup()
        {
            if (!needsCleanup)
                return;

            listenersLock.EnterWriteLock();
            try
            {
                var index = 0;
                while (index < listeners.Count)
                {
                    if (!listeners[index].IsAlive)
                    {
                        listeners.RemoveAt(index);
                    }
                    else
                    {
                        index++;
                    }
                }
            }
            finally
            {
                listenersLock.ExitWriteLock();
                needsCleanup = false;
            }
        }

        /// <summary>
        /// Raises a requery event.
        /// </summary>
        public void Raise()
        {
            if (!needsRequery)
                return;

            listenersLock.EnterReadLock();
            try
            {
                for (int i = 0; i < listeners.Count; i++)
                {
                    (listeners[i].Target as EventHandler)?.Invoke(owner, EventArgs.Empty);
                }
            }
            finally
            {
                listenersLock.ExitReadLock();
                needsRequery = false;
            }
        }

        /// <summary>
        /// Indicates that command statuses should be requeried at the end of the next frame.
        /// </summary>
        public void InvalidateRequerySuggested()
        {
            needsRequery = true;
        }

        /// <summary>
        /// Cleans up the listener list when the Sedulous context is updated.
        /// </summary>
        private void Updating(FrameworkContext context, FrameworkTime time)
        {
            Cleanup();
            Raise();
        }

        // State values.
        private readonly CommandManager owner;
        private readonly IPool<WeakReference> refpool = 
            new ExpandingPool<WeakReference>(8, 32, () => new WeakReference(null), item => item.Target = null);
        private readonly List<WeakReference> listeners = new List<WeakReference>();
        private readonly ReaderWriterLockSlim listenersLock = new ReaderWriterLockSlim();
        private Boolean needsCleanup;
        private Boolean needsRequery;
    }
}
