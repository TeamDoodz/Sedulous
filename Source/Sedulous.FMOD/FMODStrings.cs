using System.Reflection;
using Sedulous.Core.Text;

namespace Sedulous.Fmod
{
    /// <summary>
    /// Contains the implementation's string resources.
    /// </summary>
    public static class FmodStrings
    {
        /// <summary>
        /// Initializes the <see cref="FmodStrings"/> type.
        /// </summary>
        static FmodStrings()
        {
            var asm = Assembly.GetExecutingAssembly();
            using (var stream = asm.GetManifestResourceStream("Sedulous.FMOD.Resources.Strings.xml"))
            {
                StringDatabase.LoadFromStream(stream);
            }
        }

        private static readonly LocalizationDatabase StringDatabase = new LocalizationDatabase();

#pragma warning disable 1591
        public static readonly StringResource FMODVersionMismatch         = new StringResource(StringDatabase, "FMOD_VERSION_MISMATCH");
        public static readonly StringResource NotCurrentlyValid           = new StringResource(StringDatabase, "NOT_CURRENTLY_VALID");
        public static readonly StringResource CannotFindPlatformShimClass = new StringResource(StringDatabase, "CANNOT_FIND_PLATFORM_SHIM_CLASS");
#pragma warning restore 1591
    }
}
