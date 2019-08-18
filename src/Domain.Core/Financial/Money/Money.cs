namespace Domain.Core.Financial.Money
{
    using System;

    public struct Money
    {
        public Money(decimal? value, string currency)
        {
            Value = value.GetValueOrDefault(0);
            Currency = new Currency(currency ?? string.Empty);
        }

        public Money(decimal? value, Currency currency)
        {
            Value = value.GetValueOrDefault(0);
            Currency = currency;
        }

        public decimal Value { get; set; }
        public Currency Currency { get; set; }

        public bool DenominatedInCommonCurrency(Money amount)
        {
            return Equals(Currency, amount.Currency);
        }

        public static Money operator +(Money a, Money b)
        {
            if (!a.DenominatedInCommonCurrency(b))
            {
                throw new ArgumentException($"Currency of A did not match currency of B ({a.Currency.Code}, {b.Currency.Code})");
            }

            return new Money(a.Value + b.Value, a.Currency);
        }

        public static Money operator -(Money a, Money b)
        {
            if (!a.DenominatedInCommonCurrency(b))
            {
                throw new ArgumentException($"Currency of A did not match currency of B ({a.Currency.Code}, {b.Currency.Code})");
            }

            return new Money(a.Value - b.Value, a.Currency);
        }

        public override string ToString()
        {
            return $"({Currency.Code}) {Value}";
        }
    }
}
