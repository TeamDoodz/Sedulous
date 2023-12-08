using Android.App;
using System;

namespace Sedulous.Fmod
{
    /// <summary>
    /// Represents Android-specific implementation details for FMOD.
    /// </summary>
    public sealed class FMODAndroidSpecificImplementationDetails : FmodPlatformSpecificImplementationDetails
    {
        private Boolean initialized = false;

        /// <inheritdoc/>
        public override void OnInitialized()
        {
            Java.Lang.JavaSystem.LoadLibrary("fmod");

            var context = Application.Context;
            if (context != null)
            {
                Org.Fmod.FMOD.Init(context);
                initialized = true;
            }
        }

        /// <inheritdoc/>
        public override void OnApplicationCreated()
        {
            var context = Application.Context;
            if (context != null && !initialized)
            {
                Org.Fmod.FMOD.Init(context);
                initialized = true;
            }
        }

        /// <inheritdoc/>
        public override void OnApplicationTerminating()
        {
            Org.Fmod.FMOD.Close();
        }
    }
}