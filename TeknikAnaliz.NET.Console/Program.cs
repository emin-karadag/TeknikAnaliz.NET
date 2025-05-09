﻿using Binance.Net.Clients;
using Binance.Net.Enums;
using TeknikAnaliz.NET;

try
{
    var client = new BinanceRestClient();
    var klinesResult = await client.SpotApi.ExchangeData.GetKlinesAsync(
        symbol: "BTCUSDT",
        interval: KlineInterval.OneDay,
        limit: 100);

    if (klinesResult.Success && (klinesResult.Data?.Any() ?? false))
    {
        var closePrices = klinesResult.Data.Select(p => (double)p.ClosePrice).ToArray();

        var emaResult = TAHelper.EMA(closePrices, 9);
        var smaResult = TAHelper.SMA(closePrices, 9);
        var rmaResult = TAHelper.RMA(closePrices, 15);
        var stDevResult = TAHelper.STDEV(closePrices, 5);
        var rsiResult = TAHelper.RSI(closePrices, 14);
        var bbResult = TAHelper.BB(closePrices, 5, 4);

        Console.WriteLine($"EMA => {emaResult.Last():F3} USDT");
        Console.WriteLine($"SMA => {smaResult.Last():F3} USDT");
        Console.WriteLine($"RMA => {rmaResult.Last():F3} USDT");
        Console.WriteLine($"STDEV => {stDevResult.Last():F3} USDT");
        Console.WriteLine($"RSI => {rsiResult.Last():F3} USDT");
        Console.WriteLine($"BB => {bbResult.middle.Last():F3} USDT");
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

Console.ReadLine();
