using insert_google_search_products.Base;
using insert_google_search_products.Consumer;

var builder = Host.CreateApplicationBuilder(args);
var configurationBuilder = new ConfigurationBuilder().AddJsonFile("appsettings.json", false);
var configuration = configurationBuilder.Build();

builder.Services.AddSingleton<IConfiguration>(configuration);
builder.Services.AddSingleton<IRabbitMqFactory, RabbitMqFactory>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();