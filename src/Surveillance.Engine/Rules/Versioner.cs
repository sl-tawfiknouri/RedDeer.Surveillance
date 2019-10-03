namespace Surveillance.Engine.Rules.Rules
{
    /// <summary>
    /// The version.
    /// </summary>
    public static class Versioner
    {
        /// <summary>
        /// The version.
        /// </summary>
        /// <param name="major">
        /// The major.
        /// </param>
        /// <param name="minor">
        /// The minor.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string Version(int major, int minor)
        {
            return $"V{major}.{minor}";
        }
    }
}