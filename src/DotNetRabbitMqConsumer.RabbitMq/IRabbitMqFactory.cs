namespace insert_google_search_products.Base;

public interface IRabbitMqFactory
{
    Task PublishMessage(string message);
    Task ConsumeMessage();
}