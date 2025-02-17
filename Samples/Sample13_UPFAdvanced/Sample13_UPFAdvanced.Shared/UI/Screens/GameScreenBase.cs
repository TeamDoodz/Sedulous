﻿using System;
using Sedulous.Content;
using Sedulous.Core;
using Sedulous.UI;

namespace Sample13_UPFAdvanced.UI.Screens
{
    /// <summary>
    /// Represents the base class for screens in this application.
    /// </summary>
    public abstract class GameScreenBase : UIScreen
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GameScreenBase"/> class.
        /// </summary>
        /// <param name="rootDirectory">The root directory of the panel's local content manager.</param>
        /// <param name="definitionAsset">The asset path of the screen's definition file.</param>
        /// <param name="globalContent">The content manager with which to load globally-available assets.</param>
        /// <param name="uiScreenService">The screen service which created this screen.</param>
        public GameScreenBase(String rootDirectory, String definitionAsset, ContentManager globalContent, UIScreenService uiScreenService)
            : base(rootDirectory, definitionAsset, globalContent)
        {
            Contract.Require(uiScreenService, "uiScreenService");

            this.UIScreenService = uiScreenService;
        }

        /// <summary>
        /// Gets the screen service which created this screen.
        /// </summary>
        public UIScreenService UIScreenService { get; private set; }
    }
}
