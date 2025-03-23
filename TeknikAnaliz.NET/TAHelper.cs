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
    }
}
