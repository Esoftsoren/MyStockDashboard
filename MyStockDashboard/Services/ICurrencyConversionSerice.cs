namespace MyStockDashboard.Services;

public interface ICurrencyConversionService
{
    /// <summary>
    /// Converts an amount from the specified currency to the base currency.
    /// </summary>
    Task<decimal> ConvertAsync(decimal amount, string fromCurrency);
}