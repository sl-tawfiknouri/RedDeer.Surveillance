using Domain.Core.Financial.Assets;
using GraphQL.Types;

namespace Surveillance.Api.App.Types.Trading
{
    public class InstrumentTypeGraphType : EnumerationGraphType<InstrumentTypes>
    {
        public InstrumentTypeGraphType()
        {
            Name = "InstrumentType";
        }    
    }
}
