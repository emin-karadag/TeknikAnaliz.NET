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
    }
}
