﻿using System;
using System.Reflection;
using Sedulous.Core;

namespace Sedulous.Presentation.Uvml
{
    /// <summary>
    /// Represents a UVML mutator which sets a standard event handler.
    /// </summary>
    internal sealed class UvmlStandardEventHandlerMutator : UvmlEventMutator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UvmlStandardEventHandlerMutator"/> class.
        /// </summary>
        /// <param name="eventInfo">The event which is being mutated.</param>
        /// <param name="eventHandler">The event handler to add to the event.</param>
        public UvmlStandardEventHandlerMutator(EventInfo eventInfo, UvmlNode eventHandler)
        {
            Contract.Require(eventInfo, nameof(eventInfo));
            Contract.Require(eventHandler, nameof(eventHandler));

            this.eventInfo = eventInfo;
            this.eventHandler = eventHandler;
        }

        /// <inheritdoc/>
        public override Object InstantiateValue(FrameworkContext frameworkContext, Object instance, UvmlInstantiationContext context)
        {
            return eventHandler.Instantiate(frameworkContext, context);
        }

        /// <inheritdoc/>
        public override void Mutate(FrameworkContext frameworkContext, Object instance, UvmlInstantiationContext context)
        {
            var value = InstantiateValue(frameworkContext, instance, context);
            Mutate(frameworkContext, instance, value, context);
        }

        /// <inheritdoc/>
        public override void Mutate(FrameworkContext frameworkContext, Object instance, Object value, UvmlInstantiationContext context)
        {
            var eventHandlerName = ProcessPrecomputedValue<String>(value, context);
            if (eventHandlerName == null)
                throw new UvmlException(PresentationStrings.InvalidEventHandler.Format(eventInfo.Name, "(null)"));

            var eventHandlerDelegate = CreateEventHandlerDelegate(eventHandlerName, eventInfo.EventHandlerType, context);
            eventInfo.AddEventHandler(instance, eventHandlerDelegate);
        }

        // State values.
        private readonly EventInfo eventInfo;
        private readonly UvmlNode eventHandler;
    }
}
