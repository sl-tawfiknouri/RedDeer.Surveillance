namespace RedDeer.Surveillance.Api.Client.Nodes
{
    using RedDeer.Surveillance.Api.Client.Infrastructure;

    public class FinancialInstrumentNode : Node<FinancialInstrumentNode>
    {
        public FinancialInstrumentNode(Parent parent)
            : base(parent)
        {
        }

        public FinancialInstrumentNode FieldBloombergTicker()
        {
            return this.AddField("bloombergTicker");
        }

        public FinancialInstrumentNode FieldCfi()
        {
            return this.AddField("cfi");
        }

        public FinancialInstrumentNode FieldClientIdentifier()
        {
            return this.AddField("clientIdentifier");
        }

        public FinancialInstrumentNode FieldCusip()
        {
            return this.AddField("cusip");
        }

        public FinancialInstrumentNode FieldExchangeSymbol()
        {
            return this.AddField("exchangeSymbol");
        }

        public FinancialInstrumentNode FieldFigi()
        {
            return this.AddField("figi");
        }

        public FinancialInstrumentNode FieldId()
        {
            return this.AddField("id");
        }

        public FinancialInstrumentNode FieldIsin()
        {
            return this.AddField("isin");
        }

        public FinancialInstrumentNode FieldIssuerIdentifier()
        {
            return this.AddField("issuerIdentifier");
        }

        public FinancialInstrumentNode FieldLei()
        {
            return this.AddField("lei");
        }

        public FinancialInstrumentNode FieldReddeerId()
        {
            return this.AddField("reddeerId");
        }

        public FinancialInstrumentNode FieldSecurityName()
        {
            return this.AddField("securityName");
        }

        public FinancialInstrumentNode FieldSedol()
        {
            return this.AddField("sedol");
        }
    }
}