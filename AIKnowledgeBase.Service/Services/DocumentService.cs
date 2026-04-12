using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AIKnowledgeBase.Core.Interfaces;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using AIKnowledgeBase.Core.Entities;


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
                
            //geçmişi getir
            var history = await _unitOfWork.GetRepository<ChatMessage>()
                .GetAllAsync(x => x.DocumentId == documentId); //bu dökümanla ilgili geçmiş mesajları alıyoruz

            //ai ya sor: döküman metni, yeni soru ve hafızayı paketleyip gonder
            var aiResponse = await _aiService.AnalyzeTextAsync(document.Content,question, history.ToList());

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
            var text = new StringBuilder();

            //itext7 kütüphanesini kullanarak PDF i açıyoruz
            using (PdfReader reader = new PdfReader(filePath))
            using (PdfDocument pdfDoc = new PdfDocument(reader))
            {
                await Task.CompletedTask; //asenkron işlemi simüle ediyoruz, çünkü iText7'nin PDF okuma işlemi asenkron değil
                for (int i =1; i <= pdfDoc.GetNumberOfPages(); i++)
                {
                    //her sayfanın içeriğini alıyoruz
                    var strategy = new LocationTextExtractionStrategy();
                    string pageText = PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(i), strategy);
                    text.Append(pageText); //metni birleştiriyoruz
                }
            }

            return text.ToString(); //metni string olarak döndürüyoruz
        }
    }
}
