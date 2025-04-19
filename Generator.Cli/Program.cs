using Generator.Cli;

var data = DataFactory.CreateWebTransactions(1,TimeSpan.FromMinutes(5));

Directory.CreateDirectory("out");

var session = Guid.NewGuid();
var now = DateTimeOffset.UtcNow;
var filename = $"out/txn-{now.ToUnixTimeMilliseconds()}-{session}.csv";

using var stream = File.OpenWrite(filename);
var writer = new StreamWriter(stream);

await writer.WriteLineAsync("x-cs-timestamp,date,time,cs-username,time-taken,cs-bytes,sc-bytes,bytes");

foreach(var item in data)
{
    await writer.WriteLineAsync($"{item.XCsTimestamp},{item.Date:d},{item.Time},{item.CsUsername},{item.TimeTaken},{item.CsBytes},{item.ScBytes},{item.Bytes}");
}

await writer.FlushAsync();