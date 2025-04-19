namespace Generator.Cli;

public static class DataFactory
{
    public static IEnumerable<WebTransaction> CreateWebTransactions(int count, TimeSpan duration)
    {
        var now = DateTimeOffset.UtcNow;
        var slice = duration / count;

        var result = Enumerable.Range(0,count).Select(x=> new WebTransaction()
        {
            XCsTimestamp = (now-slice*x).ToUnixTimeSeconds(),
            Date = (now-slice*x).Date,
            Time = (now-slice*x).TimeOfDay,
            CsUsername = "Generated",
            TimeTaken = $"{100+x}",
            CsBytes = $"{200+x}",
            ScBytes = $"{400+x}",
            Bytes = $"{600+x}"
        });

        return result;
    }
}