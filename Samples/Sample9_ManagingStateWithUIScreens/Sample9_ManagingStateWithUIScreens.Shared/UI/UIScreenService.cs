﻿using System;
using System.Collections.Generic;
using Sample9_ManagingStateWithUIScreens.UI.Screens;
using Sedulous.Content;
using Sedulous.UI;

namespace Sample9_ManagingStateWithUIScreens.UI
{
    /// <summary>
    /// Represents a service which provides instances of UI screen types upon request.
    /// </summary>
    public sealed class UIScreenService
    {
        /// <summary>
        /// Initializes a new instance of the UIScreenService type.
        /// </summary>
        public UIScreenService(ContentManager globalContent)
        {
            Register(new SampleScreen1(globalContent, this));
            Register(new SampleScreen2(globalContent, this));
        }

        /// <summary>
        /// Gets an instance of the specified screen type.
        /// </summary>
        /// <returns>An instance of the specified screen type.</returns>
        public T Get<T>() where T : UIScreen
        {
            UIScreen screen;
            screens.TryGetValue(typeof(T), out screen);
            return (T)screen;
        }

        /// <summary>
        /// Registers the screen instance that is returned for the specified type.
        /// </summary>
        /// <param name="instance">The instance to return for the specified type.</param>
        private void Register<T>(T instance) where T : UIScreen
        {
            screens[typeof(T)] = instance;
        }

        // State values.
        private readonly Dictionary<Type, UIScreen> screens = 
            new Dictionary<Type, UIScreen>();
    }
}
