namespace TeknikAnaliz.NET
{
    public static class TAHelper
    {
        #region EMA (Exponential Moving Average)

        /// <summary>
        /// Exponential Moving Average (EMA) hesaplar.
        /// TradingView'in ta.ema() fonksiyonu ile aynı sonuçları verir.
        /// </summary>
        /// <param name="source">Kaynak fiyat dizisi</param>
        /// <param name="length">Periyot uzunluğu</param>
        /// <returns>EMA değerlerini içeren dizi</returns>
        public static double[] EMA(double[] source, int length)
        {
            if (source == null || source.Length == 0)
                return [];

            if (length <= 0)
                throw new ArgumentException("Periyot uzunluğu pozitif bir sayı olmalıdır.", nameof(length));

            double[] result = new double[source.Length];
            double alpha = 2.0 / (length + 1);

            // İlk geçerli değeri buluyoruz (na olmayan)
            int firstValidIndex = -1;
            for (int i = 0; i < source.Length; i++)
            {
                if (!double.IsNaN(source[i]))
                {
                    firstValidIndex = i;
                    break;
                }
            }

            if (firstValidIndex == -1)
                return result; // Tüm değerler NaN ise boş dizi döndür

            // İlk geçerli indeksten önceki indeksler için NaN değerlerini doldur
            for (int i = 0; i < firstValidIndex; i++)
            {
                result[i] = double.NaN;
            }

            // İlk geçerli değeri başlangıç değeri olarak kullanıyoruz
            double sum = source[firstValidIndex];
            result[firstValidIndex] = sum;

            // Diğer değerleri hesaplıyoruz
            for (int i = firstValidIndex + 1; i < source.Length; i++)
            {
                if (double.IsNaN(source[i]))
                {
                    result[i] = double.NaN; // NaN değerleri korunuyor
                    continue;
                }

                sum = double.IsNaN(sum) ? source[i] : alpha * source[i] + (1 - alpha) * sum;
                result[i] = sum;
            }

            return result;
        }

        #endregion

        #region SMA (Simple Moving Average)

        /// <summary>
        /// Simple Moving Average (SMA) hesaplar.
        /// TradingView'in ta.sma() fonksiyonu ile aynı sonuçları verir.
        /// </summary>
        /// <param name="source">Kaynak fiyat dizisi</param>
        /// <param name="length">Periyot uzunluğu</param>
        /// <returns>SMA değerlerini içeren dizi</returns>
        public static double[] SMA(double[] source, int length)
        {
            if (source == null || source.Length == 0)
                return [];

            if (length <= 0)
                throw new ArgumentException("Periyot uzunluğu pozitif bir sayı olmalıdır.", nameof(length));

            double[] result = new double[source.Length];

            // Kayan pencere yaklaşımı ile SMA hesapla
            double sum = 0.0;
            int count = 0;

            for (int i = 0; i < source.Length; i++)
            {
                // Yeni değeri ekle
                if (!double.IsNaN(source[i]))
                {
                    sum += source[i];
                    count++;
                }

                // Eğer length değerinden fazla değerimiz varsa eski değeri çıkar
                if (i >= length && !double.IsNaN(source[i - length]))
                {
                    sum -= source[i - length];
                    count--;
                }

                // Yeterli veri varsa SMA hesapla
                if (i >= length - 1)
                    result[i] = count > 0 ? sum / count : double.NaN;
                else
                    result[i] = double.NaN;
            }

            return result;
        }

        #endregion

        #region RMA (Relative Moving Average)

        /// <summary>
        /// Relative Moving Average (RMA) hesaplar.
        /// TradingView'in ta.rma() fonksiyonu ile aynı sonuçları verir.
        /// RSI'da kullanılan hareketli ortalama türüdür.
        /// </summary>
        /// <param name="source">Kaynak fiyat dizisi</param>
        /// <param name="length">Periyot uzunluğu</param>
        /// <returns>RMA değerlerini içeren dizi</returns>
        public static double[] RMA(double[] source, int length)
        {
            if (source == null || source.Length == 0)
                return [];

            if (length <= 0)
                throw new ArgumentException("Periyot uzunluğu pozitif bir sayı olmalıdır.", nameof(length));

            double[] result = new double[source.Length];
            double alpha = 1.0 / length;

            // İlk değer için SMA hesapla
            double[] sma = SMA(source, length);

            // İlk geçerli SMA değerini bul
            int firstValidIndex = -1;
            for (int i = 0; i < sma.Length; i++)
            {
                if (!double.IsNaN(sma[i]))
                {
                    firstValidIndex = i;
                    break;
                }
            }

            if (firstValidIndex == -1)
                return result; // Tüm değerler NaN ise boş dizi döndür

            // İlk geçerli indeksten önceki indeksler için NaN değerlerini doldur
            for (int i = 0; i < firstValidIndex; i++)
            {
                result[i] = double.NaN;
            }

            // İlk geçerli SMA değerini başlangıç değeri olarak kullanıyoruz
            double sum = sma[firstValidIndex];
            result[firstValidIndex] = sum;

            // Diğer değerleri hesaplıyoruz
            for (int i = firstValidIndex + 1; i < source.Length; i++)
            {
                if (double.IsNaN(source[i]))
                {
                    result[i] = double.IsNaN(sum) ? double.NaN : sum; // NaN değer gelirse önceki değeri koru
                    continue;
                }

                sum = double.IsNaN(sum) ? sma[i] : alpha * source[i] + (1 - alpha) * sum;
                result[i] = sum;
            }

            return result;
        }

        #endregion

        #region STDEV (Standard Deviation)

        /// <summary>
        /// Standart Sapma (Standard Deviation) hesaplar.
        /// TradingView'in ta.stdev() fonksiyonu ile aynı sonuçları verir.
        /// </summary>
        /// <param name="source">Kaynak fiyat dizisi</param>
        /// <param name="length">Periyot uzunluğu</param>
        /// <param name="biased">Taraflı tahmin kullanılıp kullanılmayacağı. True: tüm popülasyonun taraflı tahmini, False: bir örneğin tarafsız tahmini</param>
        /// <returns>Standart sapma değerlerini içeren dizi</returns>
        public static double[] STDEV(double[] source, int length, bool biased = true)
        {
            if (source == null || source.Length == 0)
                return [];

            if (length <= 0)
                throw new ArgumentException("Periyot uzunluğu pozitif bir sayı olmalıdır.", nameof(length));

            // Önce SMA hesapla
            double[] sma = SMA(source, length);
            double[] result = new double[source.Length];

            // Her bir indeks için standart sapma hesapla
            for (int i = 0; i < source.Length; i++)
            {
                // Eğer yeterli veri yoksa veya SMA değeri NaN ise NaN döndür
                if (i < length - 1 || double.IsNaN(sma[i]))
                {
                    result[i] = double.NaN;
                    continue;
                }

                double sumOfSquareDeviations = 0.0;
                int validCount = 0;

                for (int j = 0; j < length; j++)
                {
                    int sourceIndex = i - j;
                    if (sourceIndex >= 0 && !double.IsNaN(source[sourceIndex]))
                    {
                        double deviation = source[sourceIndex] - sma[i];
                        // Çok küçük değerler için sıfır kabul et
                        if (Math.Abs(deviation) <= 1e-10)
                            deviation = 0;

                        sumOfSquareDeviations += deviation * deviation;
                        validCount++;
                    }
                }

                // Eğer hiç geçerli değer yoksa NaN döndür
                if (validCount == 0)
                {
                    result[i] = double.NaN;
                }
                else
                {
                    // biased parametresine göre hesapla
                    // true: tüm popülasyonun taraflı tahmini (n)
                    // false: bir örneğin tarafsız tahmini (n-1)
                    double divisor = biased ? validCount : (validCount - 1);

                    // n-1 = 0 olabilir, bu durumda NaN döndür
                    if (divisor <= 0)
                    {
                        result[i] = double.NaN;
                    }
                    else
                    {
                        result[i] = Math.Sqrt(sumOfSquareDeviations / divisor);
                    }
                }
            }

            return result;
        }

        #endregion

        #region RSI (Relative Strength Index)

        /// <summary>
        /// Göreceli Güç Endeksi (Relative Strength Index - RSI) hesaplar.
        /// TradingView'in ta.rsi() fonksiyonu ile aynı sonuçları verir.
        /// </summary>
        /// <param name="source">Kaynak fiyat dizisi</param>
        /// <param name="length">Periyot uzunluğu</param>
        /// <returns>RSI değerlerini içeren dizi</returns>
        public static double[] RSI(double[] source, int length)
        {
            if (source == null || source.Length == 0)
                return [];

            if (length <= 0)
                throw new ArgumentException("Periyot uzunluğu pozitif bir sayı olmalıdır.", nameof(length));

            double[] result = new double[source.Length];

            double[] upChanges = new double[source.Length];
            double[] downChanges = new double[source.Length];

            upChanges[0] = double.NaN;
            downChanges[0] = double.NaN;
            result[0] = double.NaN;

            for (int i = 1; i < source.Length; i++)
            {
                if (double.IsNaN(source[i]) || double.IsNaN(source[i - 1]))
                {
                    upChanges[i] = double.NaN;
                    downChanges[i] = double.NaN;
                }
                else
                {
                    upChanges[i] = Math.Max(source[i] - source[i - 1], 0);
                    downChanges[i] = Math.Max(source[i - 1] - source[i], 0);
                }
            }

            // RMA hesaplama
            double[] upRMA = RMA(upChanges, length);
            double[] downRMA = RMA(downChanges, length);

            // RSI hesaplama
            for (int i = 0; i < source.Length; i++)
            {
                if (double.IsNaN(upRMA[i]) || double.IsNaN(downRMA[i]))
                {
                    result[i] = double.NaN;
                }
                else if (Math.Abs(downRMA[i]) < 1e-10)
                {
                    // Sıfıra bölme durumunda RSI = 100
                    result[i] = 100.0;
                }
                else
                {
                    double rs = upRMA[i] / downRMA[i];
                    result[i] = 100.0 - (100.0 / (1.0 + rs));
                }
            }

            return result;
        }

        #endregion

        #region BB (Bollinger Bands)

        /// <summary>
        /// Bollinger Bantları (Bollinger Bands) hesaplar.
        /// TradingView'in ta.bb() fonksiyonu ile aynı sonuçları verir.
        /// </summary>
        /// <param name="source">Kaynak fiyat dizisi</param>
        /// <param name="length">Periyot uzunluğu</param>
        /// <param name="mult">Standart sapma çarpanı</param>
        /// <returns>Bollinger Bantları değerlerini içeren tuple (middle, upper, lower)</returns>
        public static (double[] middle, double[] upper, double[] lower) BB(double[] source, int length, double mult)
        {
            if (source == null || source.Length == 0)
                return (Array.Empty<double>(), Array.Empty<double>(), Array.Empty<double>());

            if (length <= 0)
                throw new ArgumentException("Periyot uzunluğu pozitif bir sayı olmalıdır.", nameof(length));

            if (mult <= 0)
                throw new ArgumentException("Standart sapma çarpanı pozitif bir sayı olmalıdır.", nameof(mult));

            // Orta bant (SMA)
            double[] middle = SMA(source, length);

            // Standart sapma
            double[] stdev = STDEV(source, length);

            // Üst ve alt bantlar
            double[] upper = new double[source.Length];
            double[] lower = new double[source.Length];

            for (int i = 0; i < source.Length; i++)
            {
                if (double.IsNaN(middle[i]) || double.IsNaN(stdev[i]))
                {
                    upper[i] = double.NaN;
                    lower[i] = double.NaN;
                }
                else
                {
                    double dev = mult * stdev[i];
                    upper[i] = middle[i] + dev;
                    lower[i] = middle[i] - dev;
                }
            }

            return (middle, upper, lower);
        }

        #endregion

        #region ATR (Average True Range)

        /// <summary>
        /// Average True Range (ATR) hesaplar.
        /// TradingView'in ta.atr() fonksiyonu ile aynı sonuçları verir.
        /// </summary>
        /// <param name="high">Yüksek fiyat dizisi</param>
        /// <param name="low">Düşük fiyat dizisi</param>
        /// <param name="close">Kapanış fiyat dizisi</param>
        /// <param name="length">Periyot uzunluğu</param>
        /// <returns>ATR değerlerini içeren dizi</returns>
        public static double[] ATR(double[] high, double[] low, double[] close, int length)
        {
            if (high == null || low == null || close == null)
                throw new ArgumentNullException("High, low ve close dizileri null olamaz.");

            if (high.Length != low.Length || low.Length != close.Length)
                throw new ArgumentException("High, low ve close dizileri aynı uzunlukta olmalıdır.");

            if (high.Length == 0)
                return [];

            if (length <= 0)
                throw new ArgumentException("Periyot uzunluğu pozitif bir sayı olmalıdır.", nameof(length));

            // True Range hesaplama
            double[] trueRange = TrueRange(high, low, close);

            // True Range'in RMA'sını hesapla (ATR)
            return RMA(trueRange, length);
        }

        /// <summary>
        /// True Range hesaplar.
        /// TradingView'in ta.tr() fonksiyonu ile aynı sonuçları verir.
        /// </summary>
        /// <param name="high">Yüksek fiyat dizisi</param>
        /// <param name="low">Düşük fiyat dizisi</param>
        /// <param name="close">Kapanış fiyat dizisi</param>
        /// <returns>True Range değerlerini içeren dizi</returns>
        public static double[] TrueRange(double[] high, double[] low, double[] close)
        {
            if (high == null || low == null || close == null)
                throw new ArgumentNullException("High, low ve close dizileri null olamaz.");

            if (high.Length != low.Length || low.Length != close.Length)
                throw new ArgumentException("High, low ve close dizileri aynı uzunlukta olmalıdır.");

            if (high.Length == 0)
                return [];

            double[] result = new double[high.Length];

            for (int i = 0; i < high.Length; i++)
            {
                // NaN kontrolü
                if (double.IsNaN(high[i]) || double.IsNaN(low[i]) || double.IsNaN(close[i]))
                {
                    result[i] = double.NaN;
                    continue;
                }

                // İlk bar için veya önceki kapanış NaN ise: true range = high - low
                if (i == 0 || double.IsNaN(close[i - 1]))
                {
                    result[i] = high[i] - low[i];
                }
                else
                {
                    // True Range = max(high - low, abs(high - close[1]), abs(low - close[1]))
                    double range1 = high[i] - low[i];
                    double range2 = Math.Abs(high[i] - close[i - 1]);
                    double range3 = Math.Abs(low[i] - close[i - 1]);

                    result[i] = Math.Max(range1, Math.Max(range2, range3));
                }
            }

            return result;
        }

        #endregion
    }
}
