namespace MyStockDashboard.Services;

public static class RequestThrottler
{
    // Allow one request at a time.
    private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    // The minimum delay between requests, e.g., 1 second.
    private static readonly TimeSpan _delay = TimeSpan.FromMilliseconds(50);

    public static async Task<T> ExecuteWithThrottleAsync<T>(Func<Task<T>> action)
    {
        await _semaphore.WaitAsync();
        try
        {
            // Execute the request.
            T result = await action();

            // Wait the delay before releasing.
            await Task.Delay(_delay);
            return result;
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
