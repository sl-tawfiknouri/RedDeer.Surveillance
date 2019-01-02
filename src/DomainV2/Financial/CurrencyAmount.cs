namespace DomainV2.Financial
{
    public struct CurrencyAmount
    {
        public CurrencyAmount(decimal value, string currency)
        {
            Value = value;
            Currency = new Currency(currency ?? string.Empty);
        }

        public CurrencyAmount(decimal value, Currency currency)
        {
            Value = value;
            Currency = currency;
        }

        public CurrencyAmount(decimal? value, string currency)
        {
            Value = value.GetValueOrDefault(0);
            Currency = new Currency(currency ?? string.Empty);
        }

        public decimal Value { get; set; }
        public Currency Currency { get; set; }

        public bool DenominatedInCommonCurrency(CurrencyAmount amount)
        {
            return Equals(Currency, amount.Currency);
        }

        public static CurrencyAmount operator +(CurrencyAmount a, CurrencyAmount b)
        {
            return new CurrencyAmount(a.Value + b.Value, a.Currency);
        }

        public static CurrencyAmount operator -(CurrencyAmount a, CurrencyAmount b)
        {
            return new CurrencyAmount(a.Value - b.Value, a.Currency);
        }

        public override string ToString()
        {
            return $"({Currency.Value}) {Value}";
        }
    }
}
