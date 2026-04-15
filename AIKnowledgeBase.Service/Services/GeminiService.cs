using AIKnowledgeBase.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AIKnowledgeBase.Core.Entities;
using Microsoft.VisualBasic;


namespace AIKnowledgeBase.Service.Services
{
    public class GeminiService : IAIService
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;

        public GeminiService(IConfiguration configuration)
        {
            // 1. Anahtarı appsettings.json'dan alıyoruz
            _apiKey = configuration["Gemini:ApiKey"];

            Console.WriteLine($"DEBUG: API Key okundu mu?: {!string.IsNullOrEmpty(_apiKey)}");

            // 2. HttpClient'ı başlatıyoruz (En sağlam ağ aracıdır)
            _httpClient = new HttpClient();
        }

        public async Task<string> AnalyzeTextAsync(string documentText, string userQuestion, List<ChatMessage> history, string? filePath = null)
        {

            // LOG: Dışarıdan gerçekten ne geliyor görelim
            Console.WriteLine($"--- AI ANALİZ BAŞLADI ---");
            Console.WriteLine($"Gelen filePath: '{(filePath ?? "NULL")}'");
            Console.WriteLine($"Gelen documentText ilk 20 karakter: '{documentText.Substring(0, Math.Min(20, documentText.Length))}'");


            try
            {
               

                //  URL ve Model ismini kullanıyoruz
                 string url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={_apiKey}";


                //Gemini'nin anlayacağı formatta geçmişi ve yeni soruyu hazırlıyoruz
                var chatParts = new List<object>();

                //önce dökümanı ve geçmişini yüklüyoruz
                foreach (var msg in history)
                {
                    chatParts.Add(new
                    {
                        role = msg.Role == "user" ? "user" : "model",
                        parts = new[] { new { text = msg.Content } }
                    });
                }

                var currentParts = new List<object>();

                //eğer bir resim dosyasıysa, resmi Base64 formatına çevirip özel bir token ile gönderiyoruz, böylece Gemini resmi de anlayabilir
                if (!string.IsNullOrEmpty(filePath))
                {
                    Console.WriteLine($"SİSTEM: Resim moduna girildi. Yol: {filePath}");


                    byte[] imageBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                    string base64Image = Convert.ToBase64String(imageBytes);

                    Console.WriteLine("-----------------------------------------");
                    Console.WriteLine($"DOSYA YOLU: {filePath}");
                    Console.WriteLine($"RESİM BOYUTU (Byte): {imageBytes.Length}");
                    Console.WriteLine("-----------------------------------------");



                    // Dosya uzantısına göre mime_type'ı dinamik belirleyelim (Profesyonel yaklaşım)
                    string extension = Path.GetExtension(filePath).ToLower();
                    string mimeType = (extension == ".png") ? "image/png" : "image/jpeg";

                    currentParts.Add(new { inline_data = new {mime_type = mimeType, data = base64Image} });
                    //currentParts.Add(new { text = $"Bu görseli analiz et ve şu soruya cevap ver: {userQuestion}" });
                    currentParts.Add(new
                    {
                        text = "Sen profesyonel bir döküman analiz asistanısın. Ekteki görsel teknik bir doküman veya ders notudur. " +
               "Lütfen bu görseldeki metinleri, diyagramları ve başlıkları dikkatlice analiz ederek şu soruyu cevapla: " + userQuestion
                    });
                }
                else
                {
                    //normal metin mesajıysa direkt ekliyoruz
                    currentParts.Add(new { text = $"Döküman içeriğine göre cevapla: {documentText}\n\nSoru: {userQuestion}" });
                }

                //En sona da güncel dökümanı ve soruyu ekliyoruz (Bu Gemini'nin son odaklanacağı yer olur)
                chatParts.Add(new
                {
                    role = "user",
                    parts = currentParts 
                });

                // 5. Google'ın beklediği JSON formatını oluşturuyoruz
                var requestBody = new
                {
                    contents = chatParts //tek bir parça değil tüm liste gidiyor
                    
                };

                var jsonPayload = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                // 6. İstek gönderiliyor
                var response = await _httpClient.PostAsync(url, content);
                var resultJson = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return $"API Hatası ({response.StatusCode}): {resultJson}";
                }

                // 7. Gelen karmaşık JSON'u temizleyip sadece AI'nın cevabını alıyoruz
                using var doc = JsonDocument.Parse(resultJson);
                var aiResponseText = doc.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString();

                return aiResponseText ?? "Cevap alınamadı";
            }
            catch (Exception ex)
            {
                return $"Servis Hatası: {ex.Message}";
            }
        }
    }
}