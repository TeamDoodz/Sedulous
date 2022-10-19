﻿using System;
using Sedulous.Content;
using Sedulous.UI;

namespace Sedulous.Presentation.Tests.Screens
{
    public abstract class TestScreenBase<TViewModel> : UIScreen
        where TViewModel : class, new()
    {
        public TestScreenBase(String rootDirectory, String definitionAsset, ContentManager globalContent)
            : base(rootDirectory, definitionAsset, globalContent)
        {

        }

        protected override Object CreateViewModel(UIView view)
        {
            return new TViewModel();
        }
    }
}
