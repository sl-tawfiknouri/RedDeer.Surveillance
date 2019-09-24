namespace Surveillance.Api.DataAccess.Abstractions.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Surveillance.Api.DataAccess.Abstractions.Entities;

    /// <summary>
    /// The BrokerRepository interface.
    /// </summary>
    public interface IBrokerRepository
    {
        /// <summary>
        /// The get by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<IBroker> GetById(int? id);

        /// <summary>
        /// The query for broker entities.
        /// </summary>
        /// <param name="query">
        /// The query to filter by.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<IEnumerable<IBroker>> Query(Func<IQueryable<IBroker>, IQueryable<IBroker>> query);
    }
}