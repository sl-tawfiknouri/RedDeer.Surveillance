using System.Linq;
using GraphQL.Authorization;
using GraphQL.DataLoader;
using GraphQL.Types;
using Surveillance.Api.App.Authorization;
using Surveillance.Api.DataAccess.Abstractions.Entities;
using Surveillance.Api.DataAccess.Abstractions.Repositories;

namespace Surveillance.Api.App.Types.Trading
{
    public class FinancialInstrumentGraphType : ObjectGraphType<IFinancialInstrument>
    {
        public FinancialInstrumentGraphType(
            IMarketRepository marketRepository,
            IDataLoaderContextAccessor dataLoader)
        {
            this.AuthorizeWith(PolicyManifest.UserPolicy);

            Field(i => i.Id).Description("Financial Instrument Identifier");

            Field<ListGraphType<MarketGraphType>>(
                "Market",
                description: "Market associated with the instrument",
                resolve: context =>
                {
                    IQueryable<IMarket> IdQuery(IQueryable<IMarket> i) => i.Where(x => x.Id == context.Source.MarketId);

                    var loader = dataLoader.Context.GetOrAddLoader(
                        $"GetMarketById-{context.Source.MarketId}",
                        () => marketRepository.Query(IdQuery));

                    return loader.LoadAsync();
                });

            Field(i => i.ClientIdentifier).Description("The client identifier for the instrument in question");
            Field(i => i.Sedol).Description("Sedol for the instrument");
            Field(i => i.Isin).Description("Isin for the instrument");
            Field(i => i.Figi).Description("Figi for the instrument");
            Field(i => i.Cusip).Description("Cusip for the instrument");
            Field(i => i.Lei).Description("Lei for the instrument");
            Field(i => i.ExchangeSymbol).Description("Exchange symbol for the instrument");
            Field(i => i.BloombergTicker).Description("Bloomberg Ticker for the instrument");
            Field(i => i.SecurityName).Description("Security name for the instrument");
            Field(i => i.Cfi).Description("Classification for Financial Instruments");
            Field(i => i.IssuerIdentifier).Description("Issuer identifier");
            Field(i => i.SecurityCurrency).Description("The currency the security is traded in");
            Field(i => i.ReddeerId).Description("The reddeer id (security master list) for the instrument");
            Field(i => i.EnrichmentDate).Description("The date the security was enriched on in UTC and in UK time format");

            Field<InstrumentTypeGraphType>("InstrumentType", description: "The type of the instrument");

            Field(i => i.UnderlyingCfi).Description("CFI code for the underlying");
            Field(i => i.UnderlyingName).Description("Name of the underlying instrument");
            Field(i => i.UnderlyingSedol).Description("Sedol of the underlying");
            Field(i => i.UnderlyingIsin).Description("Isin of the underlying");
            Field(i => i.UnderlyingFigi).Description("Figi of the underlying");
            Field(i => i.UnderlyingCusip).Description("Cusip of the underlying");
            Field(i => i.UnderlyingLei).Description("Lei of the underlying");
            Field(i => i.UnderlyingExchangeSymbol).Description("Exchange symbol of the underlying");
            Field(i => i.UnderlyingBloombergTicker).Description("Bloomberg ticker of the underlying");
            Field(i => i.UnderlyingClientIdentifier).Description("Client Identifier of the underlying");
        }
    }
}
