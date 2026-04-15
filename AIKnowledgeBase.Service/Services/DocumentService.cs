using AIKnowledgeBase.Core.Entities;
using AIKnowledgeBase.Core.Interfaces;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text;
using System.Threading.Tasks;


namespace AIKnowledgeBase.Service.Services
{
    public class DocumentService : IDocumentService
    {

        private readonly IAIService _aiService;
        private readonly IUnitOfWork _unitOfWork;

        public DocumentService(IAIService aiService, IUnitOfWork unitOfWork)
        {
            _aiService = aiService;
            _unitOfWork = unitOfWork;
        }


        public async Task<string> AskQuestionAsync(int documentId, string question)
        {
            //dökümanı bul. veritabanından dökümanı alıyoruz
            var document = await _unitOfWork.GetRepository<Document>().GetByIdAsync(documentId);
            if (document == null) return "Döküman bulunamadı.";

            if (string.IsNullOrEmpty(document.Content))
            {
                return "Hata: Dökümanın içeriği boş. Lütfen önce dökümanı yükleyin ve içeriğini özetleyin.";
            }


            string? filePath = null;

            // İster iki nokta olsun ister olmasın, yakala!
            if (document.Content != null && document.Content.Contains("[IMAGE_FILE]"))
            {
                // Önce etiketi (varsa iki noktasıyla beraber) temizle
                filePath = document.Content
                    .Replace("[IMAGE_FILE]:", "")
                    .Replace("[IMAGE_FILE]", "")
                    .Trim();

                
                // Bazen yol "C:\..." yerine "C:/..." gelebilir. Windows slaşlarını düzeltelim.
                filePath = filePath.Replace("/", "\\");
            }

            // KONSOLDA BUNU GÖRMELİYİZ
            Console.WriteLine($"--- KRİTİK AYIKLAMA KONTROLÜ ---");
            Console.WriteLine($"Ayıklanan Yol: {filePath ?? "BULUNAMADI"}");


            //geçmişi getir
            var history = await _unitOfWork.GetRepository<ChatMessage>()
                .GetAllAsync(x => x.DocumentId == documentId); //bu dökümanla ilgili geçmiş mesajları alıyoruz

            //ai ya sor: döküman metni, yeni soru ve hafızayı paketleyip gonder
            var aiResponse = await _aiService.AnalyzeTextAsync(document.Content,question, history.ToList(), filePath);

            //hafızayı kaydet: hem soruyu hem cevabı veritabanına ekliyoruz
            var userMsg = new ChatMessage { DocumentId = documentId, Role = "user", Content = question};
            var aiMsg = new ChatMessage { DocumentId = documentId, Role = "model", Content = aiResponse};

            await _unitOfWork.GetRepository<ChatMessage>().AddAsync(userMsg);
            await _unitOfWork.GetRepository<ChatMessage>().AddAsync(aiMsg);

            await _unitOfWork.CommitAsync();
            return aiResponse; //ai cevabını döndürüyoruz

        }





        public async Task<string> GetTextFromFileAsync(string filePath)
        {
            //Dosya uzantısını alıyoruz(.pdf, .jpg, .png gibi)
            string extension = Path.GetExtension(filePath).ToLower();

            if (extension == ".pdf")
            {
                return await ExtractTextFromPdf(filePath);
            }
            else if (extension == ".jpg" || extension == ".jpeg" || extension == ".png")
            {
                return "[IMAGE_FILE]" + filePath;
            }
            else
            {
                throw new NotSupportedException("Desteklenmeyen dosya formatı.");
            }
        }

        //pdf okuma mantığını ayrı bir metotta yapıyoruz, böylece kod daha temiz ve okunabilir olur
        private async Task<string> ExtractTextFromPdf(string filePath)
        {
            var text = new StringBuilder();
            using (PdfReader reader = new PdfReader(filePath))
            using (PdfDocument pdfDoc = new PdfDocument(reader))
            {
                await Task.CompletedTask; //asenkron işlemi simüle ediyoruz, çünkü iText7'nin PDF okuma işlemi asenkron değil
                for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
                {
                    var strategy = new LocationTextExtractionStrategy();
                    string pageText = PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(i), strategy);
                    text.Append(pageText);
                }
            }
            return text.ToString();

        }



           
        }
}

