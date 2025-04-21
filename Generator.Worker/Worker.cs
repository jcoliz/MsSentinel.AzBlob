using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Text;
using Azure.Storage.Blobs;
using Generator.Worker.Options;
using Microsoft.Extensions.Options;

namespace Generator.Worker;

public partial class Worker(BlobServiceClient blobServiceClient, IOptions<BlobStorageOptions> options, ILogger<Worker> logger) : BackgroundService
{
    private readonly TimeSpan _period = TimeSpan.FromMinutes(1);
    private readonly int _numLines = 10;
    private readonly Guid _session = Guid.NewGuid();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logSessionOk(_session);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var now = DateTimeOffset.UtcNow;
                var data = DataFactory.CreateWebTransactions(_numLines,_period);

                using var stream = await DataToStream(data);
                var bytes = await CompressStream(stream);
                using var gzipStream = new MemoryStream(bytes);

                var containerClient = blobServiceClient.GetBlobContainerClient(options.Value.Container);
                await containerClient.CreateIfNotExistsAsync();

                var dataname = $"txn-{now.ToUnixTimeMilliseconds()}-{_session.ToString().Replace("-","")}";
                var blobClient = containerClient.GetBlobClient($"{options.Value.Folder}/{dataname}.csv.gz");
                var result = await blobClient.UploadAsync(gzipStream);

                logOkResult(System.Text.Json.JsonSerializer.Serialize(result));
            }
            catch(Exception ex)
            {
                logFail(ex);
            }
            await Task.Delay(_period, stoppingToken);
        }
    }

    private static async Task<Stream> DataToStream(IEnumerable<WebTransaction> data)
    {
        var rawstream = new MemoryStream();
        var writer = new StreamWriter(rawstream);

        await writer.WriteLineAsync("x-cs-timestamp,date,time,cs-username,time-taken,cs-bytes,sc-bytes,bytes");

        foreach(var item in data)
        {
            await writer.WriteLineAsync($"{item.XCsTimestamp},{item.Date:d},{item.Time},{item.CsUsername},{item.TimeTaken},{item.CsBytes},{item.ScBytes},{item.Bytes}");
        }

        await writer.FlushAsync();
        rawstream.Seek(0,SeekOrigin.Begin);

        return rawstream;
    }

    private static async Task<byte[]> CompressStream(Stream inputStream)
    {
        // https://stackoverflow.com/questions/3722192/how-do-i-use-gzipstream-with-system-io-memorystream

        byte[] compressed;

        using (var outStream = new MemoryStream())
        {
            using (var tinyStream = new GZipStream(outStream, CompressionMode.Compress))
                await inputStream.CopyToAsync (tinyStream);

            compressed = outStream.ToArray();
        }

        return compressed;
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "{Location}: OK", EventId = 1000)]
    public partial void logOk([CallerMemberName] string? location = null);

    [LoggerMessage(Level = LogLevel.Information, Message = "{Location}: OK {result}", EventId = 1010)]
    public partial void logOkResult(string result,[CallerMemberName] string? location = null);

    [LoggerMessage(Level = LogLevel.Information, Message = "{Location}: Session Started OK {session}", EventId = 1020)]
    public partial void logSessionOk(Guid session, [CallerMemberName] string? location = null);

    [LoggerMessage(Level = LogLevel.Error, Message = "{Location}: Failed", EventId = 1008)]
    public partial void logFail(Exception ex,[CallerMemberName] string? location = null);
}
