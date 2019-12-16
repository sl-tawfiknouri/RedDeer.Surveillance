namespace Domain.Core.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;

    public static class EnumExtensions
    {
        /// <summary>
        ///     Enumerations only
        ///     Requires c# 7.3+ to resolve fully with enum as a generic constraint
        /// </summary>
        public static string GetDescription<T>(this T enumerationValue)
            where T : struct
        {
            var type = enumerationValue.GetType();

            if (!type.IsEnum)
                throw new ArgumentException(
                    $"{nameof(enumerationValue)} must be of Enum type",
                    nameof(enumerationValue));

            var memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo.Length > 0)
            {
                var attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs.Length > 0) return ((DescriptionAttribute)attrs[0]).Description;
            }

            return enumerationValue.ToString();
        }

        public static IList<T> GetEnumPermutations<T>(this T enumerationValue)
            where T : struct
        {
            var enumTypes = new List<T>();
            var enumValues = Enum.GetValues(typeof(T));
            foreach (var val in enumValues) enumTypes.Add((T)val);

            return enumTypes;
        }

        public static bool TryParsePermutations<T>(string value, out T result)
            where T : struct
        {
            var propertyValue = value ?? string.Empty;

            if (TryParseEnum<T>(propertyValue, out var enumValue))
            {
                result = enumValue;
                return true;
            }

            var textInfo = new CultureInfo(CultureInfoConstants.DefaultCultureInfo, false).TextInfo;

            if (TryParseEnum<T>(textInfo.ToTitleCase(propertyValue.ToLower()), out var enumValue2))
            {
                result = enumValue2;
                return true;
            }

            result = enumValue2;

            return false;
        }

        public static bool TryParseEnum<T>(string value, out T result) where T : struct
        {
            if (!Enum.TryParse<T>(value, true, out T enumVal))
            {
                result = enumVal;
                return false;
            }

            if (!Enum.IsDefined(typeof(T), enumVal))
            {
                result = enumVal;
                return false;
            }

            result = enumVal;
            return true;
        }
    }
}