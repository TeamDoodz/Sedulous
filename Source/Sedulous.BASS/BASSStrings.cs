using System.Reflection;
using Sedulous.Core.Text;

namespace Sedulous.Bass
{
    /// <summary>
    /// Contains the implementation's string resources.
    /// </summary>
    public static class BassStrings
    {
        /// <summary>
        /// Initializes the BASSStrings type.
        /// </summary>
        static BassStrings()
        {
            var asm = Assembly.GetExecutingAssembly();
            using (var stream = asm.GetManifestResourceStream("Sedulous.BASS.Resources.Strings.xml"))
            {
                StringDatabase.LoadFromStream(stream);
            }
        }

        private static readonly LocalizationDatabase StringDatabase = new LocalizationDatabase();

#pragma warning disable 1591
        public static readonly StringResource StreamsNotSupported = new StringResource(StringDatabase, "STREAMS_NOT_SUPPORTED");
        public static readonly StringResource NotCurrentlyValid   = new StringResource(StringDatabase, "NOT_CURRENTLY_VALID");
#pragma warning restore 1591
    }
}
