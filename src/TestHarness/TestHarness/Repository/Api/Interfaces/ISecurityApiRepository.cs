namespace TestHarness.Repository.Api.Interfaces
{
    using System;
    using System.Threading.Tasks;

    using RedDeer.Contracts.SurveillanceService.Api.SecurityPrices;

    public interface ISecurityApiRepository
    {
        Task<SecurityPriceResponseDto> Get(DateTime from, DateTime to, string market);

        Task<bool> Heartbeating();
    }
}