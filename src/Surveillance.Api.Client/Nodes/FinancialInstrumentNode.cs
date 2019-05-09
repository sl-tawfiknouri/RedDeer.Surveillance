using Surveillance.Api.Client.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Surveillance.Api.Client.Nodes
{
    public class FinancialInstrumentNode : Node
    {
        public FinancialInstrumentNode(NodeParent parent) : base(parent) { }

        public FinancialInstrumentNode FieldId() => AddField("id", this);
        public FinancialInstrumentNode FieldClientIdentifier() => AddField("clientIdentifier", this);
        public FinancialInstrumentNode FieldSedol() => AddField("sedol", this);
        public FinancialInstrumentNode FieldIsin() => AddField("isin", this);
        public FinancialInstrumentNode FieldFigi() => AddField("figi", this);
        public FinancialInstrumentNode FieldCusip() => AddField("cusip", this);
        public FinancialInstrumentNode FieldLei() => AddField("lei", this);
        public FinancialInstrumentNode FieldExchangeSymbol() => AddField("exchangeSymbol", this);
        public FinancialInstrumentNode FieldBloombergTicker() => AddField("bloombergTicker", this);
        public FinancialInstrumentNode FieldSecurityName() => AddField("securityName", this);
        public FinancialInstrumentNode FieldCfi() => AddField("cfi", this);
        public FinancialInstrumentNode FieldIssuerIdentifier() => AddField("issuerIdentifier", this);
        public FinancialInstrumentNode FieldReddeerId() => AddField("reddeerId", this);
    }
}
