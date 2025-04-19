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
    }
}
