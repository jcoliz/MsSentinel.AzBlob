using Azure.Identity;
using Azure.Storage.Blobs;
using Generator.Worker;
using Generator.Worker.Options;
using Microsoft.Extensions.Options;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddTomlFile("config.toml", optional: true, reloadOnChange: true);

builder.Services.AddSingleton(sp => 
{
    var connectionString = builder.Configuration.GetConnectionString("StorageAccount");
    if (connectionString is not null)
        return new BlobServiceClient(connectionString);
    else
    {
        var options = sp.GetRequiredService<IOptions<BlobStorageOptions>>();
        var credential = new DefaultAzureCredential();
        return new BlobServiceClient(options.Value.Endpoint,credential);
    }
});

builder.Services.Configure<BlobStorageOptions>(builder.Configuration.GetSection(BlobStorageOptions.Section));
builder.Services.AddHostedService<Worker>();
var host = builder.Build();
host.Run();
