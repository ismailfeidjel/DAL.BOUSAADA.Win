using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DevExpress.ProductsDemo.Win.Core.Helpers
{
    public static class ValidationHelper
    {
        public static bool Required(string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        public static bool Required(object value)
        {
            return value != null && !string.IsNullOrWhiteSpace(value.ToString());
        }

        public static bool MaxLength(string value, int maxLength)
        {
            if (value == null)
                return true;

            return value.Length <= maxLength;
        }

        public static bool MinLength(string value, int minLength)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            return value.Length >= minLength;
        }

        public static bool BetweenLength(string value, int minLength, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            return value.Length >= minLength &&
                   value.Length <= maxLength;
        }

        public static bool Positive(int value)
        {
            return value > 0;
        }

        public static bool Positive(decimal value)
        {
            return value > 0;
        }

        public static bool Positive(double value)
        {
            return value > 0;
        }

        public static bool NonNegative(decimal value)
        {
            return value >= 0;
        }

        public static bool NonNegative(int value)
        {
            return value >= 0;
        }

        public static bool Between(int value, int min, int max)
        {
            return value >= min && value <= max;
        }

        public static bool Between(decimal value, decimal min, decimal max)
        {
            return value >= min && value <= max;
        }

        public static bool ValidYear(int year)
        {
            return year >= 2000 &&
                   year <= DateTime.Now.Year + 10;
        }

        public static bool Email(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return Regex.IsMatch(
                email,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        public static bool Phone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            return Regex.IsMatch(
                phone,
                @"^[0-9+\-\s()]{6,20}$");
        }

        public static bool Url(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return false;

            return Uri.TryCreate(
                url,
                UriKind.Absolute,
                out _);
        }

        public static bool EqualsIgnoreCase(string a, string b)
        {
            return string.Equals(
                a?.Trim(),
                b?.Trim(),
                StringComparison.OrdinalIgnoreCase);
        }

        public static bool Unique<T>(
            IEnumerable<T> source,
            Func<T, string> selector,
            string value)
        {
            if (source == null)
                return true;

            return !source.Any(x =>
                string.Equals(
                    selector(x)?.Trim(),
                    value?.Trim(),
                    StringComparison.OrdinalIgnoreCase));
        }

        public static bool Unique<T, TKey>(
            IEnumerable<T> source,
            Func<T, TKey> selector,
            TKey value)
        {
            if (source == null)
                return true;

            return !source.Any(x =>
                EqualityComparer<TKey>.Default.Equals(
                    selector(x),
                    value));
        }

        public static bool UniqueProgram(
            IEnumerable<Domain.ProgramLookupItem> source,
            Domain.ProgramLookupItem current)
        {
            return !source.Any(x =>
                x.Id != current.Id &&
                x.Type == current.Type &&
                x.Year == current.Year);
        }

        public static bool GreaterThan(decimal value, decimal minimum)
        {
            return value > minimum;
        }

        public static bool LessThan(decimal value, decimal maximum)
        {
            return value < maximum;
        }

        public static bool InList<T>(
            T value,
            IEnumerable<T> list)
        {
            return list.Contains(value);
        }

        public static bool Custom(bool condition)
        {
            return condition;
        }

        public static string FirstError(params (bool IsValid, string Error)[] rules)
        {
            foreach (var rule in rules)
            {
                if (!rule.IsValid)
                    return rule.Error;
            }

            return string.Empty;
        }
    }
}