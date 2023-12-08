namespace Sedulous.Fmod
{
    /// <summary>
    /// Represents a factory method which constructs instances of the <see cref="FmodPlatformSpecificImplementationDetails"/> class.
    /// </summary>
    /// <param name="context">The Sedulous context.</param>
    /// <returns>The instance of <see cref="FmodPlatformSpecificImplementationDetails"/> that was created.</returns>
    public delegate FmodPlatformSpecificImplementationDetails FMODPlatformSpecificImplementationDetailsFactory(FrameworkContext context);

    /// <summary>
    /// Represents platform-specific implementation details for FMOD which can be provided by an additional assembly.
    /// </summary>
    public abstract class FmodPlatformSpecificImplementationDetails
    {
        /// <summary>
        /// Creates a new instance of the <see cref="FmodPlatformSpecificImplementationDetails"/> class.
        /// </summary>
        /// <returns>The instance of <see cref="FmodPlatformSpecificImplementationDetails"/> that was created.</returns>
        public static FmodPlatformSpecificImplementationDetails Create()
        {
            var uv = FrameworkContext.DemandCurrent();
            var factory = uv.TryGetFactoryMethod<FMODPlatformSpecificImplementationDetailsFactory>();
            if (factory == null)
                return new FmodGenericPlatformImplementationDetails();

            return factory(uv);
        }

        /// <summary>
        /// Called when FMOD is initialized.
        /// </summary>
        public abstract void OnInitialized();

        /// <summary>
        /// Called when the operating system creates the application.
        /// </summary>
        public abstract void OnApplicationCreated();

        /// <summary>
        /// Called when the operating system destroys the application.
        /// </summary>
        public abstract void OnApplicationTerminating();
    }
}
