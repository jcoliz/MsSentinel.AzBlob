using Generator.Cli;

var data = DataFactory.CreateWebTransactions(1,TimeSpan.FromMinutes(5));

Console.WriteLine("x-cs-timestamp,date,time,cs-username,time-taken,cs-bytes,sc-bytes,bytes");

foreach(var item in data)
{
    Console.WriteLine($"{item.XCsTimestamp},{item.Date:d},{item.Time},{item.CsUsername},{item.TimeTaken},{item.CsBytes},{item.ScBytes},{item.Bytes}");
}
