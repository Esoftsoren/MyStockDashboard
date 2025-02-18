using System;
using System.Threading.Tasks;
using CurrencyConverterCustom;

namespace MyStockDashboard.Services
{
    public class RealCurrencyConversionService : ICurrencyConversionService
    {
        private readonly Converter _converter;
        
        public RealCurrencyConversionService(Converter converter)
        {
            _converter = converter;
        }
        
        public async Task<decimal> ConvertAsync(decimal amount, string fromCurrency)
        {
            // If the currency is already the base, return it.
            if (string.Equals(fromCurrency, _converter.Base, StringComparison.OrdinalIgnoreCase))
                return amount;

            // Override for DKK and SEK with manual fixed rates:
            // These fixed rates are defined relative to 1 USD.
            if (string.Equals(fromCurrency, "DKK", StringComparison.OrdinalIgnoreCase))
            {
                // 1 USD = 7.12 DKK (fixed rate)
                return amount / 7.12m;
            }
            if (string.Equals(fromCurrency, "SEK", StringComparison.OrdinalIgnoreCase))
            {
                // 1 USD = 10.71 SEK (fixed rate)
                return amount / 10.71m;
            }

            // Otherwise, use the API-provided rates.
            var rates = await _converter.GetRatesAsync();
            if (rates == null)
            {
                throw new Exception("The rates dictionary is null. Please verify that the API is returning a valid response and that your CurrencyConverterUrlOptions are properly configured.");
            }

            // Because your API response keys are prefixed with the base currency,
            // construct the lookup key. For example, if the base is "USD" and you want to convert GBP,
            // the key should be "USDGBP".
            string lookupKey = _converter.Base + fromCurrency.ToUpperInvariant();

            if (!rates.TryGetValue(lookupKey, out double rate))
            {
                var availableCurrencies = string.Join(", ", rates.Keys);
                throw new Exception($"Currency conversion rate for '{fromCurrency}' was not found. Available currencies: {availableCurrencies}");
            }

            // Convert by dividing the amount by the conversion rate.
            decimal converted = amount / (decimal)rate;
            return converted;
        }


    }
}