![TeknikAnaliz.Net](https://github.com/emin-karadag/TeknikAnaliz.NET/blob/main/TeknikAnaliz.NET/Images/banner-min.png)

# TeknikAnaliz.NET

**Finansal dünyayı C# ile daha da yakından analiz etmek için TeknikAnaliz.NET'i keşfedin!** 🚀

TeknikAnaliz.NET, finansal piyasalarda teknik analiz yapmak için tasarlanmış güçlü ve kullanımı kolay bir C# kütüphanesidir. Bu kütüphane, yaygın olarak kullanılan teknik indikatörlerin (RSI, EMA, Bollinger Bantları vb.) TradingView'in Pine Script programlama dilindeki hesaplamalarına uygun şekilde C#'a uyarlanmış halini sunar.

## Kurulum

NuGet Package Manager üzerinden paketi yükleyebilirsiniz: https://www.nuget.org/packages/TeknikAnaliz.NET/1.0.4

**NuGet PM**
```
Install-Package TeknikAnaliz.NET -Version 1.0.4
```

**dotnet cli**
```
dotnet add package TeknikAnaliz.NET --version 1.0.4
```

### Neden TeknikAnaliz.NET?

- **Güvenilir Hesaplamalar:** TradingView'in doğruluğuyla bilinen formülleri temel alır.
- **Modüler ve Genişletilebilir:** Yeni indikatörler eklemek için esnek bir yapıya sahiptir.
- **Kullanıcı Dostu:** Basit ve anlaşılır API tasarımı sayesinde hızlıca entegre edilebilir.

## Yol Haritası
Önümüzdeki süreçte `TeknikAnaliz.NET` kütüphanesine yeni özelliklerin eklenmesi ve genişletilmesi için çalışmalar yapılacaktır. Aşağıdaki tabloda üzerinde çalıştığımız yeni özellikleri görebilirsiniz.

| Özellik                 |    Durum     |  
|------------------------|:--------------:|
| EMA (Exponential Moving Average)              |   ✔  |
| SMA (Simple Moving Average)                   |   ✔  |  
| RMA (Relative Moving Average)                 |   ✔  |
| STDEV (Standard Deviation)                    |   ✔  |
| RSI (Relative Strength Index)                 |   ✔  |
| BB (Bollinger Bands)                          |       | |

## Kullanım

**Binance üzerinden fiyat verisi alma:**
```csharp
using Binance.Net.Clients;
using Binance.Net.Enums;

var client = new BinanceRestClient();
var klinesResult = await client.SpotApi.ExchangeData.GetKlinesAsync(
    symbol: "BTCUSDT",
    interval: KlineInterval.FifteenMinutes,
    limit: 100);
```

------------

**1. EMA (Exponential Moving Average):**
EMA metodu, TradingView'in `ta.ema()` fonksiyonu ile aynı sonuçları verir.

```csharp
using TeknikAnaliz.NET;

var closePrices = klinesResult.Data.Select(p => (double)p.ClosePrice).ToArray();
var result = TAHelper.EMA(closePrices, 9);
```

**2. SMA (Simple Moving Average):**
SMA metodu, TradingView'in `ta.sma()` fonksiyonu ile aynı sonuçları verir.

```csharp
using TeknikAnaliz.NET;

var closePrices = klinesResult.Data.Select(p => (double)p.ClosePrice).ToArray();
var result = TAHelper.SMA(closePrices, 9);
```

**3. RMA (Relative Moving Average):**
RMA metodu, TradingView'in `ta.rma()` fonksiyonu ile aynı sonuçları verir.

```csharp
using TeknikAnaliz.NET;

var closePrices = klinesResult.Data.Select(p => (double)p.ClosePrice).ToArray();
var result = TAHelper.RMA(closePrices, 15);
```

**4. STDEV (Standard Deviation):**
STDEV metodu, TradingView'in `ta.stdev()` fonksiyonu ile aynı sonuçları verir.

```csharp
using TeknikAnaliz.NET;

var closePrices = klinesResult.Data.Select(p => (double)p.ClosePrice).ToArray();
var result = TAHelper.STDEV(closePrices, 5);
```

**5. RSI (Relative Strength Index):**
RSI metodu, TradingView'in `ta.rsi()` fonksiyonu ile aynı sonuçları verir.

```csharp
using TeknikAnaliz.NET;

var closePrices = klinesResult.Data.Select(p => (double)p.ClosePrice).ToArray();
var result = TAHelper.RSI(closePrices, 14);
```

### Lisans 
    MIT License
