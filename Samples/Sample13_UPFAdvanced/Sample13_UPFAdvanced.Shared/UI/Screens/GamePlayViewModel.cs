﻿using System;
using System.Threading.Tasks;
using Sample13_UPFAdvanced.UI.Dialogs;
using Sedulous;
using Sedulous.Core;
using Sedulous.Input;
using Sedulous.Presentation;
using Sedulous.Presentation.Controls;
using Sedulous.Presentation.Input;

namespace Sample13_UPFAdvanced.UI.Screens
{
    /// <summary>
    /// Represents the view model for <see cref="GamePlayScreen"/>.
    /// </summary>
    public sealed class GamePlayViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GamePlayViewModel"/> class.
        /// </summary>
        /// <param name="owner">The <see cref="GameMenuScreen"/> that owns this view model.</param>
        /// <param name="escMenuDialog">The escape menu dialog box used by this screen.</param>
        public GamePlayViewModel(GamePlayScreen owner, EscMenuDialog escMenuDialog)
        {
            Contract.Require(owner, "owner");

            this.owner = owner;
            this.escMenuDialog = escMenuDialog;

            this.V1Color = Color.Red;
            this.V2Color = Color.Lime;
            this.V3Color = Color.Blue;
        }

        /// <summary>
        /// Handles the <see cref="View.OpenedEvent"/> attached event.
        /// </summary>
        /// <param name="dobj">The object that raised the event.</param>
        /// <param name="data">The routed event metadata for this event invocation.</param>
        public void HandleViewOpened(DependencyObject dobj, RoutedEventData data)
        {
            if (container != null)
                container.Focus();
        }

        /// <summary>
        /// Handles the <see cref="Keyboard.KeyDownEvent"/> attached event for the view's topmost <see cref="Grid"/> instance.
        /// </summary>
        /// <param name="dobj">The object that raised the event.</param>
        /// <param name="device">The <see cref="KeyboardDevice"/> that raised the event.</param>
        /// <param name="key">The <see cref="Key"/> value that represents the key that was pressed.</param>
        /// <param name="modifiers">A <see cref="ModifierKeys"/> value indicating which of the key modifiers are currently active.</param>
        /// <param name="data">The routed event metadata for this event invocation.</param>
        public void HandleKeyDown(DependencyObject dobj, KeyboardDevice device, Key key, ModifierKeys modifiers, RoutedEventData data)
        {
            switch (key)
            {
                case Key.Escape:
                    {
                        Modal.ShowDialogAsync(escMenuDialog).ContinueWith(HandleEscMenuDialogResult);
                        data.Handled = true;
                    }
                    break;

                case Key.AppControlBack:
                    {
                        ReturnToMainMenu();
                        data.Handled = true;
                    }
                    break;
            }
        }
        
        /// <summary>
        /// Gets the <see cref="GameMenuScreen"/> that owns the view model.
        /// </summary>
        public GamePlayScreen Owner
        {
            get { return owner; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the displayed triangle is spinning.
        /// </summary>
        public Boolean IsTriangleSpinning
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the rotation of the displayed triangle.
        /// </summary>
        public Single TriangleRotation
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the zoom of the displayed triangle.
        /// </summary>
        public Single TriangleZoom
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the color of the first triangle vertex.
        /// </summary>
        public Color V1Color
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the color of the second triangle vertex.
        /// </summary>
        public Color V2Color
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the color of the third triangle vertex.
        /// </summary>
        public Color V3Color
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the help message which is displayed in the lower-left corner.
        /// </summary>
        public String HelpMessage
        {
            get
            {
                switch (owner.FrameworkContext.Platform)
                {
                    case FrameworkPlatform.Android:
                        return "Press |c:ffffff00|BACK|c| to exit.";

                    case FrameworkPlatform.iOS:
                        return "Press |c:ffffff00|HOME|c| to exit.";

                    default:
                        return "Press |c:ffffff00|ESC|c| to exit.";
                }
            }
        }

        /// <summary>
        /// Handles the result of the escape menu.
        /// </summary>
        private void HandleEscMenuDialogResult(Task<Boolean?> task)
        {
            if (task.Result ?? false)
            {
                ReturnToMainMenu();
            }
        }

        /// <summary>
        /// Moves the game back to the main menu screen.
        /// </summary>
        private void ReturnToMainMenu()
        {
            var screenClosing = owner;
            var screenOpening = owner.UIScreenService.Get<GameMenuScreen>();
            owner.FrameworkContext.GetUI().GetScreens().CloseThenOpen(screenClosing, screenOpening);
        }
        
        // Property values.
        private readonly GamePlayScreen owner;
        
        // Component references.
        private readonly Grid container = null;
        private EscMenuDialog escMenuDialog;
    }
}
