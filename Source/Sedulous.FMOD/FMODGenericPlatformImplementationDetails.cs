namespace Sedulous.Fmod
{
    /// <summary>
    /// Represents an implementation of the <see cref="FmodPlatformSpecificImplementationDetails"/> class which does nothing.
    /// </summary>
    public class FmodGenericPlatformImplementationDetails : FmodPlatformSpecificImplementationDetails
    {
        /// <inheritdoc/>
        public override void OnInitialized() { }

        /// <inheritdoc/>
        public override void OnApplicationCreated() { }

        /// <inheritdoc/>
        public override void OnApplicationTerminating() { }
    }
}
