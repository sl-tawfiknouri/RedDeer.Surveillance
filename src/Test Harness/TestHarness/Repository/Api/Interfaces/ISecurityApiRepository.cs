using System;
using System.Threading.Tasks;
using RedDeer.Contracts.SurveillanceService.Api.SecurityPrices;

namespace TestHarness.Repository.Api.Interfaces
{
    public interface ISecurityApiRepository
    {
        Task<bool> Heartbeating();
        Task<SecurityPriceResponseDto> Get(DateTime from, DateTime to, string market);
    }
}