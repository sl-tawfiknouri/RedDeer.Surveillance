using RedDeer.Surveillance.Api.Client.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace RedDeer.Surveillance.Api.Client.Nodes
{
    public class FinancialInstrumentNode : Node<FinancialInstrumentNode>
    {
        public FinancialInstrumentNode(Parent parent) : base(parent) { }

        public FinancialInstrumentNode FieldId() => AddField("id");
        public FinancialInstrumentNode FieldClientIdentifier() => AddField("clientIdentifier");
        public FinancialInstrumentNode FieldSedol() => AddField("sedol");
        public FinancialInstrumentNode FieldIsin() => AddField("isin");
        public FinancialInstrumentNode FieldFigi() => AddField("figi");
        public FinancialInstrumentNode FieldCusip() => AddField("cusip");
        public FinancialInstrumentNode FieldLei() => AddField("lei");
        public FinancialInstrumentNode FieldExchangeSymbol() => AddField("exchangeSymbol");
        public FinancialInstrumentNode FieldBloombergTicker() => AddField("bloombergTicker");
        public FinancialInstrumentNode FieldSecurityName() => AddField("securityName");
        public FinancialInstrumentNode FieldCfi() => AddField("cfi");
        public FinancialInstrumentNode FieldIssuerIdentifier() => AddField("issuerIdentifier");
        public FinancialInstrumentNode FieldReddeerId() => AddField("reddeerId");
    }
}
