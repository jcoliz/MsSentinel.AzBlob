namespace Generator.Cli;

public static class DataFactory
{
    public static IEnumerable<WebTransaction> CreateWebTransactions(int count, TimeSpan duration)
    {
        var now = DateTimeOffset.UtcNow;

        var result = new WebTransaction()
        {
            XCsTimestamp = now.ToUnixTimeSeconds(),
            Date = now.Date,
            Time = now.TimeOfDay,
            CsUsername = "Generated",
            TimeTaken = "100",
            CsBytes = "200",
            ScBytes = "400",
            Bytes = "600"
        };

        return [ result ];
    }
}