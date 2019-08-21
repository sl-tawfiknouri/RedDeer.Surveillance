namespace Surveillance.DataLayer.Aurora.Market.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Firefly.Service.Data.BMLL.Shared.Dtos;

    public interface IReddeerMarketTimeBarRepository
    {
        Task Save(List<MinuteBarDto> barDtos);
    }
}