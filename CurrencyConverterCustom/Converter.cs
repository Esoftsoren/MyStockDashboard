using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CurrencyConverterCustom
{
    public class Converter
    {
        protected readonly HttpClient httpClient;
        protected CurrencyConverterUrlOptions urlOptions;

        public Converter(HttpClient httpClient, CurrencyConverterUrlOptions options)
        {
            this.httpClient = httpClient;
            this.urlOptions = options;
        }

        // We'll expose the rates dictionary with a protected setter.
        // We'll store the quotes from the API in our Rates property.
        public Dictionary<string, double> Rates { get; protected set; } = new Dictionary<string, double>();

        public DateTime Date { get; protected set; }
        public string Base => this.urlOptions.BaseSymbol;  // For exchangerate.host, this may come from the "source" property

        protected virtual async Task SetRatesAsync()
        {
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            string requestUrl = urlOptions.ToString();
            // Optionally log the URL:
            // Console.WriteLine("Requesting rates from: " + requestUrl);

            string rawJson = await httpClient.GetStringAsync(requestUrl);
            CurrencyModel model = JsonSerializer.Deserialize<CurrencyModel>(rawJson, jsonOptions);

            if (model == null)
            {
                throw new Exception("Failed to deserialize the API response. Check the URL and API response.");
            }

            if (!model.Success)
            {
                string errorInfo = model.Error != null
                    ? $"{model.Error.Type} (Code: {model.Error.Code}): {model.Error.Info}"
                    : "Unknown error.";
                throw new Exception($"API request unsuccessful: {errorInfo}");
            }

            if (model.Quotes == null)
            {
                throw new Exception("The API response did not contain a 'quotes' object.");
            }

            // For consistency, we can assign the quotes to our Rates property.
            this.Rates = model.Quotes;
            // Convert the UNIX timestamp to DateTime:
            this.Date = DateTimeOffset.FromUnixTimeSeconds(model.Timestamp).UtcDateTime;
        }

        public async Task<Dictionary<string, double>> GetRatesAsync(bool retrieveNewRates = false, CurrencyConverterUrlOptions options = null)
        {
            if (this.Rates.Count == 0 || retrieveNewRates || options != null)
            {
                if (options != null)
                    this.urlOptions = options;
                await this.SetRatesAsync();
            }
            return this.Rates;
        }

        public Dictionary<string, double> GetRates(bool retrieveNewRates = false, CurrencyConverterUrlOptions options = null)
        {
            if (this.Rates.Count == 0 || retrieveNewRates || options != null)
            {
                if (options != null)
                    this.urlOptions = options;
                Task.Run(() => this.SetRatesAsync()).GetAwaiter().GetResult();
            }
            return this.Rates;
        }

        public double Convert(double conversionValue, double conversionRate)
        {
            return conversionValue * conversionRate;
        }

        public double Convert(string conversionRateCurrency, double conversionValue)
        {
            double conversionRate = this.Rates.Single(rate => rate.Key == conversionRateCurrency).Value;
            return this.Convert(conversionValue, conversionRate);
        }

        public async Task<double> ConvertAsync(string conversionRateCurrency, double conversionValue)
        {
            double conversionRate = (await this.GetRatesAsync()).Single(rate => rate.Key == conversionRateCurrency).Value;
            return this.Convert(conversionValue, conversionRate);
        }

        public List<string> Currencies => this.Rates.Select(rate => rate.Key).ToList();
    }
}
