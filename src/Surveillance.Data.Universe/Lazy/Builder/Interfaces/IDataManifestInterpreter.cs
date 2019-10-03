namespace Surveillance.Data.Universe.Lazy.Builder.Interfaces
{
    using System;
    using System.Threading.Tasks;

    using Surveillance.Data.Universe.Interfaces;

    /// <summary>
    /// The ManifestInterpreter interface.
    /// </summary>
    public interface IDataManifestInterpreter
    {
        /// <summary>
        /// Gets the data manifest.
        /// </summary>
        IDataManifest DataManifest { get; }

        /// <summary>
        /// The play forward.
        /// </summary>
        /// <param name="span">
        /// The span.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<IUniverse> PlayForward(TimeSpan span);
    }
}
