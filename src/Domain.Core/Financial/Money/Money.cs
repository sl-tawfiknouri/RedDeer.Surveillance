namespace Domain.Core.Financial
{
    public struct Money
    {
        public Money(decimal value, string currency)
        {
            Value = value;
            Currency = new Currency(currency ?? string.Empty);
        }

        public Money(decimal value, Currency currency)
        {
            Value = value;
            Currency = currency;
        }

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
            return new Money(a.Value + b.Value, a.Currency);
        }

        public static Money operator -(Money a, Money b)
        {
            return new Money(a.Value - b.Value, a.Currency);
        }

        public override string ToString()
        {
            return $"({Currency.Code}) {Value}";
        }
    }
}
