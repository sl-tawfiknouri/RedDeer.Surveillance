namespace Surveillance.Api.App.Types.Trading
{
    using System.Linq;

    using GraphQL.Authorization;
    using GraphQL.DataLoader;
    using GraphQL.Types;

    using Surveillance.Api.App.Authorization;
    using Surveillance.Api.DataAccess.Abstractions.Entities;
    using Surveillance.Api.DataAccess.Abstractions.Repositories;

    public class FinancialInstrumentGraphType : ObjectGraphType<IFinancialInstrument>
    {
        public FinancialInstrumentGraphType(IMarketRepository marketRepository, IDataLoaderContextAccessor dataLoader)
        {
            this.AuthorizeWith(PolicyManifest.UserPolicy);

            this.Field(i => i.Id).Description("Financial Instrument Identifier");

            this.Field<ListGraphType<MarketGraphType>>(
                "market",
                "Market associated with the instrument",
                resolve: context =>
                    {
                        IQueryable<IMarket> IdQuery(IQueryable<IMarket> i)
                        {
                            return i.Where(x => x.Id == context.Source.MarketId);
                        }

                        var loader = dataLoader.Context.GetOrAddLoader(
                            $"GetMarketById-{context.Source.MarketId}",
                            () => marketRepository.Query(IdQuery));

                        return loader.LoadAsync();
                    });

            this.Field(i => i.ClientIdentifier, true)
                .Description("The client identifier for the instrument in question");
            this.Field(i => i.Sedol, true).Description("Sedol for the instrument");
            this.Field(i => i.Isin, true).Description("Isin for the instrument");
            this.Field(i => i.Figi, true).Description("Figi for the instrument");
            this.Field(i => i.Cusip, true).Description("Cusip for the instrument");
            this.Field(i => i.Lei, true).Description("Lei for the instrument");
            this.Field(i => i.ExchangeSymbol, true).Description("Exchange symbol for the instrument");
            this.Field(i => i.BloombergTicker, true).Description("Bloomberg Ticker for the instrument");
            this.Field(i => i.SecurityName).Description("Security name for the instrument");
            this.Field(i => i.Cfi, true).Description("Classification for Financial Instruments");
            this.Field(i => i.IssuerIdentifier, true).Description("Issuer identifier");
            this.Field(i => i.SecurityCurrency, true).Description("The currency the security is traded in");
            this.Field(i => i.ReddeerId, true).Description("The reddeer id (security master list) for the instrument");
            this.Field(i => i.Enrichment, true).Type(new DateTimeGraphType()).Description(
                "The date the security was enriched on in UTC and in UK time format");

            this.Field<InstrumentTypeGraphType>("instrumentType", "The type of the instrument");

            this.Field(i => i.UnderlyingCfi, true).Description("CFI code for the underlying");
            this.Field(i => i.UnderlyingName, true).Description("Name of the underlying instrument");
            this.Field(i => i.UnderlyingSedol, true).Description("Sedol of the underlying");
            this.Field(i => i.UnderlyingIsin, true).Description("Isin of the underlying");
            this.Field(i => i.UnderlyingFigi, true).Description("Figi of the underlying");
            this.Field(i => i.UnderlyingCusip, true).Description("Cusip of the underlying");
            this.Field(i => i.UnderlyingLei, true).Description("Lei of the underlying");
            this.Field(i => i.UnderlyingExchangeSymbol, true).Description("Exchange symbol of the underlying");
            this.Field(i => i.UnderlyingBloombergTicker, true).Description("Bloomberg ticker of the underlying");
            this.Field(i => i.UnderlyingClientIdentifier, true).Description("Client Identifier of the underlying");
        }
    }
}