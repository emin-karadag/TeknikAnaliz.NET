using Binance.Net.Clients;
using Binance.Net.Enums;
using TeknikAnaliz.NET;

try
{
    var client = new BinanceRestClient();
    var klinesResult = await client.SpotApi.ExchangeData.GetKlinesAsync(
        symbol: "BTCUSDT",
        interval: KlineInterval.FifteenMinutes,
        limit: 100);

    if (klinesResult.Success && (klinesResult.Data?.Any() ?? false))
    {
        var closePrices = klinesResult.Data.Select(p => (double)p.ClosePrice).ToArray();
        var result = TAHelper.EMA(closePrices, 9);
        Console.WriteLine($"EMA => {result.Last():F3} USDT");
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

Console.ReadLine();
