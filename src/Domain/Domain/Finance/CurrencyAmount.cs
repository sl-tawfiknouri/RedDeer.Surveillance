﻿namespace Domain.Finance
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

        public decimal Value { get; }
        public Currency Currency { get; }

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
    }
}
