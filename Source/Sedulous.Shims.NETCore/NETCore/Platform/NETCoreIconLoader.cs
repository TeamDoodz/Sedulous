﻿using System;
using System.Linq;
using System.Reflection;
using Sedulous.Graphics;
using Sedulous.Platform;

namespace Sedulous.Shims.NETCore.Platform
{
    /// <summary>
    /// Represents an implementation of the <see cref="IconLoader"/> class for the .NET Core 3.0 platform.
    /// </summary>
    public sealed class NETCoreIconLoader : IconLoader
    {
        /// <inheritdoc/>
        public override Surface2D LoadIcon()
        {
            var asmEntry = Assembly.GetEntryAssembly();
            var asmLoader = typeof(NETCoreIconLoader).Assembly;

            var asmResourceNames = asmEntry.GetManifestResourceNames();
            var asmResourcePrefix = GetLongestCommonResourcePrefix(asmResourceNames);
            var asmResourceIcon = String.IsNullOrEmpty(asmResourcePrefix) && asmResourceNames.Length == 1 && asmResourceNames[0].EndsWith(".icon.ico") ?
                asmResourceNames[0] : $"{asmResourcePrefix}.icon.ico";

            var iconStream = 
                asmEntry.GetManifestResourceStream(asmResourceIcon) ??
                asmLoader.GetManifestResourceStream($"Sedulous.Shims.NETCore.icon.ico");

            // todo sed: load image from ico and pass image data to SurfaceSource.Create
            //if (iconStream != null)
            //{
            //    using (var source = SurfaceSource.Create(iconStream))
            //    {
            //        return Surface2D.Create(source, SurfaceOptions.SrgbColor);
            //    }
            //}

            return null;
        }

        /// <summary>
        /// Determines the common prefix which is shared by all of the specified manifest resource names.
        /// </summary>
        private static String GetLongestCommonResourcePrefix(String[] resourceNames)
        {
            if (resourceNames == null || resourceNames.Length <= 1)
                return String.Empty;

            var resourceNamesSplit = resourceNames.Select(x => x.Split('.')).ToArray();
            var resourceNamesMinLength = resourceNamesSplit.Min(x => x.Length);

            var commonComponents = 0;

            for (int i = 0; i < resourceNamesMinLength; i++)
            {
                if (resourceNamesSplit.All(x => x[i] == resourceNamesSplit[0][i]))
                    commonComponents++;
            }

            if (commonComponents == 0)
                return String.Empty;

            return String.Join(".", resourceNamesSplit[0].Take(commonComponents));
        }
    }
}
