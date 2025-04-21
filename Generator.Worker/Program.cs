using Azure.Storage.Blobs;
using Generator.Worker;
using Generator.Worker.Options;

var builder = Host.CreateApplicationBuilder(args);

// Add optional TOML config source
builder.Configuration.AddTomlFile("config.toml", optional: true, reloadOnChange: true);

builder.Services.AddSingleton(x => new BlobServiceClient(builder.Configuration.GetConnectionString("StorageAccount")));
builder.Services.Configure<BlobStorageOptions>(builder.Configuration.GetSection(BlobStorageOptions.Section));
builder.Services.AddHostedService<Worker>();
var host = builder.Build();
host.Run();
