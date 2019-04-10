using Domain.Core.Financial.Assets;
using GraphQL.Authorization;
using GraphQL.Types;
using Surveillance.Api.App.Authorization;

namespace Surveillance.Api.App.Types.Trading
{
    public class InstrumentTypeGraphType : EnumerationGraphType<InstrumentTypes>
    {
        public InstrumentTypeGraphType()
        {
            this.AuthorizeWith(PolicyManifest.UserPolicy);
            Name = "InstrumentType";
        }    
    }
}
