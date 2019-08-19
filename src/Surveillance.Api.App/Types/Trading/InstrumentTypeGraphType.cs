namespace Surveillance.Api.App.Types.Trading
{
    using Domain.Core.Financial.Assets;

    using GraphQL.Authorization;
    using GraphQL.Types;

    using Surveillance.Api.App.Authorization;

    public class InstrumentTypeGraphType : EnumerationGraphType<InstrumentTypes>
    {
        public InstrumentTypeGraphType()
        {
            this.AuthorizeWith(PolicyManifest.UserPolicy);
            this.Name = "InstrumentType";
        }
    }
}