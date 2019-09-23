namespace Domain.Core.Financial.Money
{
    using System;

    public struct Money
    {
        public Money(decimal? value, string currency)
        {
            this.Value = value.GetValueOrDefault(0);
            this.Currency = new Currency(currency ?? string.Empty);
        }

        public Money(decimal? value, Currency currency)
        {
            this.Value = value.GetValueOrDefault(0);
            this.Currency = currency;
        }

        public decimal Value { get; set; }

        public Currency Currency { get; set; }

        public bool DenominatedInCommonCurrency(Money amount)
        {
            return Equals(this.Currency, amount.Currency);
        }

        public static Money operator +(Money a, Money b)
        {
            if (!a.DenominatedInCommonCurrency(b))
                throw new ArgumentException(
                    $"Currency of A did not match currency of B ({a.Currency.Code}, {b.Currency.Code})");

            return new Money(a.Value + b.Value, a.Currency);
        }

        public static Money operator -(Money a, Money b)
        {
            if (!a.DenominatedInCommonCurrency(b))
                throw new ArgumentException(
                    $"Currency of A did not match currency of B ({a.Currency.Code}, {b.Currency.Code})");

            return new Money(a.Value - b.Value, a.Currency);
        }

        public override string ToString()
        {
            return $"({this.Currency.Code}) {this.Value}";
        }
    }
}