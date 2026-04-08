using System;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;

namespace IATry
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("--- Gemini API Doğrudan Bağlantı Testi ---");

            // 1. ADIM: Yeni aldığın API Key'i buraya yapıştır
            string apiKey = "AIzaSyDZtK4v2tflYzubxNOSGQXI15N9_kIjHng";

            // 2. ADIM: URL (Endpoint) ayarı - v1beta ve gemini-1.5-flash kullanımı en garanti yoldur
            string url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apiKey}";

            using var client = new HttpClient();

            // 3. ADIM: Google'ın beklediği JSON gövdesi (Payload)
            // Bu format Google dökümantasyonundaki tam formattır.
            var jsonPayload = "{\"contents\": [{\"parts\":[{\"text\": \"Merhaba Gemini! Ben Hülya. Bağlantımız başarılı mı?\"}]}]}";
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            Console.WriteLine("İstek gönderiliyor, lütfen bekle...");

            try
            {
                // İsteği gönderiyoruz
                var response = await client.PostAsync(url, content);
                var result = await response.Content.ReadAsStringAsync();

                Console.WriteLine("\n--- SUNUCU YANITI ---");
                Console.WriteLine("Durum Kodu: " + response.StatusCode);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("\nBAŞARILI! AI CEVABI:");
                    Console.WriteLine(result);
                }
                else
                {
                    Console.WriteLine("\nBir hata oluştu:");
                    Console.WriteLine(result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nAğ Hatası: " + ex.Message);
            }

            Console.WriteLine("\nTest bitti. Kapatmak için bir tuşa bas.");
            Console.ReadKey();
        }
    }
}