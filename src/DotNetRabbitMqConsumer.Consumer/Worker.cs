using insert_google_search_products.Base;

namespace insert_google_search_products.Consumer;

public class Worker(
    ILogger<Worker> logger,
    IRabbitMqFactory rabbitMqFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (logger.IsEnabled(LogLevel.Information))
                logger.LogInformation("Google-search-products-consumer running at: {time}", DateTimeOffset.Now);

            await rabbitMqFactory.ConsumeMessage();

            logger.LogInformation("Google-search-products-consumer stopping at: {time}", DateTimeOffset.Now);
            await Task.Delay(1000, stoppingToken);
        }
    }
}