namespace Domain.Surveillance.Categories
{
    /// <summary>
    /// The Monoid interface.
    /// </summary>
    public interface IMonoid<T>
    {
        /// <summary>
        /// Gets the case.
        /// </summary>
        T Case { get; }

        /// <summary>
        /// The m empty.
        /// </summary>
        /// <returns>
        /// The <see cref="IMonoid"/>.
        /// </returns>
        IMonoid<T> MEmpty();

        /// <summary>
        /// The m append.
        /// </summary>
        /// <param name="_">
        /// The _.
        /// </param>
        /// <returns>
        /// The <see cref="IMonoid"/>.
        /// </returns>
        IMonoid<T> MAppend(IMonoid<T> _);

        /// <summary>
        /// The m concatenate.
        /// </summary>
        /// <param name="_">
        /// The _.
        /// </param>
        /// <returns>
        /// The <see cref="IMonoid"/>.
        /// </returns>
        IMonoid<T> MConcat(params IMonoid<T>[] _);
    }
}
