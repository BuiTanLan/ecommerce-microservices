using System.Text.RegularExpressions;
using Ardalis.GuardClauses;
using ECommerce.Services.Customers.Customers.Exceptions;

namespace ECommerce.Services.Customers.Customers;

public static class GuardExtensions
{
    private static readonly Regex Regex = new(
        @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
        @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
        RegexOptions.Compiled);

    private static readonly HashSet<string> AllowedNationality = new()
    {
        "IR",
        "DE",
        "FR",
        "ES",
        "GB",
        "US"
    };

    private static readonly HashSet<string> AllowedCurrency = new() { "USD", "EUR", };

    public static string InvalidEmail(this IGuardClause guardClause, string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new InvalidEmailException(email ?? "null");
        }

        if (email.Length > 100)
        {
            throw new InvalidEmailException(email);
        }

        email = email.ToLowerInvariant();
        if (!Regex.IsMatch(email))
        {
            throw new InvalidEmailException(email);
        }

        return email;
    }

    public static string InvalidNationality(this IGuardClause guardClause, string? nationality)
    {
        if (string.IsNullOrWhiteSpace(nationality) || nationality.Length != 2)
        {
            throw new InvalidNationalityException(nationality ?? "null");
        }

        nationality = nationality.ToUpperInvariant();
        if (!AllowedNationality.Contains(nationality))
        {
            throw new UnsupportedNationalityException(nationality);
        }

        return nationality;
    }

    public static string InvalidCurrency(this IGuardClause guardClause, string? currency)
    {
        if (string.IsNullOrWhiteSpace(currency) || currency.Length != 3)
        {
            throw new InvalidCurrencyException(currency);
        }

        currency = currency.ToUpperInvariant();
        if (!AllowedCurrency.Contains(currency))
        {
            throw new UnsupportedCurrencyException(currency);
        }

        return currency;
    }

    public static string InvalidPhoneNumber(this IGuardClause guardClause, string? phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            throw new InvalidPhoneNumberException(phoneNumber ?? "null");
        }

        if (phoneNumber.Length < 7)
        {
            throw new InvalidPhoneNumberException(phoneNumber);
        }

        if (phoneNumber.Length > 15)
        {
            throw new InvalidPhoneNumberException(phoneNumber);
        }

        return phoneNumber;
    }
}
