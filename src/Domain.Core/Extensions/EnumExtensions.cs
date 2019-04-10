﻿using System;
using System.Collections.Generic;
using System.ComponentModel;

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
            var enumValues = Enum.GetValues(typeof(T));
            foreach (var val in enumValues)
            {
                enumTypes.Add((T)val);
            }

            return enumTypes;
        }
    }
}
