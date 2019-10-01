namespace Surveillance.Reddeer.ApiClient.BmllMarketData.Interfaces
{
    using System.Threading;
    using System.Threading.Tasks;

    using Firefly.Service.Data.BMLL.Shared.Commands;
    using Firefly.Service.Data.BMLL.Shared.Requests;

    /// <summary>
    /// The Time Bar interface.
    /// </summary>
    public interface IBmllTimeBarApi
    {
        /// <summary>
        /// The get minute bars.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<GetMinuteBarsResponse> GetMinuteBarsAsync(GetMinuteBarsRequest request);

        /// <summary>
        /// The heart beating.
        /// </summary>
        /// <param name="token">
        /// The token.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<bool> HeartBeatingAsync(CancellationToken token);

        /// <summary>
        /// The request minute bars.
        /// </summary>
        /// <param name="createCommand">
        /// The create command.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task RequestMinuteBarsAsync(CreateMinuteBarRequestCommand createCommand);

        /// <summary>
        /// The status minute bars.
        /// </summary>
        /// <param name="statusCommand">
        /// The status command.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<BmllStatusMinuteBarResult> StatusMinuteBarsAsync(GetMinuteBarRequestStatusesRequest statusCommand);
    }
}