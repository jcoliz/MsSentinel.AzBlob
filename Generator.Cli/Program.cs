using System.IO.Compression;
using Azure.Storage.Blobs;
using Generator.Cli;
using Microsoft.Extensions.Configuration;

//
// Load configuration
//

var configuration = new ConfigurationBuilder()
    .AddTomlFile("config.toml", optional: false, reloadOnChange: true)
    .Build();

BlobStorageOptions options = new();
configuration.Bind(BlobStorageOptions.Section, options);

var client = new BlobServiceClient(configuration.GetConnectionString("StorageAccount"));

var data = DataFactory.CreateWebTransactions(10,TimeSpan.FromMinutes(2));

Directory.CreateDirectory("out");

var session = Guid.NewGuid();
var now = DateTimeOffset.UtcNow;
var dataname = $"txn-{now.ToUnixTimeMilliseconds()}-{session.ToString().Replace("-","")}";
var filename = $"out/{dataname}.csv";

{
    using var stream = File.OpenWrite(filename);
    var writer = new StreamWriter(stream);

    await writer.WriteLineAsync("x-cs-timestamp,date,time,cs-username,time-taken,cs-bytes,sc-bytes,bytes");

    foreach(var item in data)
    {
        await writer.WriteLineAsync($"{item.XCsTimestamp},{item.Date:d},{item.Time},{item.CsUsername},{item.TimeTaken},{item.CsBytes},{item.ScBytes},{item.Bytes}");
    }

    await writer.FlushAsync();
}

{
    using FileStream originalFileStream = File.Open(filename, FileMode.Open);
    using FileStream compressedFileStream = File.Create(filename + ".gz");
    using var compressor = new GZipStream(compressedFileStream, CompressionMode.Compress);
    await originalFileStream.CopyToAsync(compressor);
    await compressedFileStream.FlushAsync();
}

var containerClient = client.GetBlobContainerClient(options.Container);
await containerClient.CreateIfNotExistsAsync();

var blobClient = containerClient.GetBlobClient($"{options.Folder}/{dataname}.csv.gz");
await blobClient.UploadAsync(filename + ".gz");
