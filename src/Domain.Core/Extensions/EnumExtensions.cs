using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace Domain.Core.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDescription<T>(this T enumerationValue)
          where T : struct
        {
            var type = enumerationValue.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException($"{nameof(enumerationValue)} must be of Enum type", nameof(enumerationValue));
            }
            var memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo.Length > 0)
            {
                var attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs.Length > 0)
                {
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }
            return enumerationValue.ToString();
        }

        public static IList<T> GetEnumPermutations<T>(this T enumerationValue) where T : struct
        {
            var enumTypes = new List<T>();
            var enumValues = System.Enum.GetValues(typeof(T));
            foreach (var val in enumValues)
            {
                enumTypes.Add((T)val);
            }

            return enumTypes;
        }

        public static bool TryParsePermutations<T>(string value, out T result) where T : struct
        {
            var propertyValue = value?.ToUpper() ?? string.Empty;

            var success = Enum.TryParse(propertyValue, out T result1);

            if (success)
            {
                result = result1;
                return true;
            }

            success = Enum.TryParse(propertyValue.ToLower(), out T result2);

            if (success)
            {
                result = result2;
                return true;
            }

            success = Enum.TryParse(propertyValue.ToUpper(), out T result3);

            if (success)
            {
                result = result3;
                return true;
            }

            var textInfo = new CultureInfo(CultureInfoConstants.DefaultCultureInfo, false).TextInfo;
            success = Enum.TryParse(textInfo.ToTitleCase(propertyValue.ToLower()), out T result4);
            result = result4;

            return success;
        }
    }
}
