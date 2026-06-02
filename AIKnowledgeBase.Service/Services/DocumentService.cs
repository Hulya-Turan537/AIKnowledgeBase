using AIKnowledgeBase.Core.Dtos;
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
using AutoMapper;
using Microsoft.EntityFrameworkCore;


namespace AIKnowledgeBase.Service.Services
{
    public class DocumentService : IDocumentService
    {

        private readonly IAIService _aiService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<ChatMessage> _chatMessageRepository;
        private readonly IMapper _mapper;

        public DocumentService(IAIService aiService, 
                               IUnitOfWork unitOfWork,
                               IGenericRepository<ChatMessage> chatMessageRepository,
                               IMapper mapper)
        {
            _aiService = aiService;
            _unitOfWork = unitOfWork;
            _chatMessageRepository = chatMessageRepository;
            _mapper = mapper;
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


        public async Task<AIKnowledgeBase.Core.Entities.Document> SaveAndProcessDocumentAsync(string fileName, string filePath, int userId)
        {
            // dosyadan metni oku(pdf ise metin, görsel ise yol döner(
            string extractedContent = await GetTextFromFileAsync(filePath);

            // yapay zekaya özel özetleme promptu hazırla
            string summaryPrompt = "Sen bir döküman özetleme asistanısın. Sana verilen döküman içeriğini analiz et ve en fazla 2 cümleden oluşan, dökümanın ne hakkında olduğunu belirten profosyonel bir özet çıkar. Asla 2 cümleyi geçme.";

            //dosya uzantısını kontrol ediyoruz
            string ext = Path.GetExtension(filePath).ToLower();
            bool isImage = (ext == ".jpg" || ext == ".jpeg" || ext == ".png");

            //eğer dosya görselse yolunu gönder, PDF ise null ki yukarıdaki if e girmesin
            string aiSummary = await _aiService.AnalyzeTextAsync(
                extractedContent,
                summaryPrompt,
                new List<ChatMessage>(), //özetleme için geçmişe gerek yok, boş liste gönderiyoruz
                isImage ? filePath : null); //eğer görselse dosya yolunu gönder, değilse null gönder)

            // veritabanına kaydedilecek nesneyi oluştur
            var newDocument = new Document
            {
                FileName = fileName,
                FilePath = filePath,
                Content = extractedContent, //dökümanın tam metni
                ContentSummary = aiSummary.Trim(), //yapay zekanın oluşturduğu özet
                UserId = userId

            };

            // repository üzerinden veritabanına ekle ve UnitOfWork ile commit et
            await _unitOfWork.GetRepository<Document>().AddAsync(newDocument);
            await _unitOfWork.CommitAsync();

            // oluşan dökümanı (ID'si de dahil) geri döndür
            return newDocument;
        }


        public async Task<List<ChatMessageDto>> GetChatHistoryByDocumentIdAsync(int documentId)
        {
            //repository üzerinden bu dökümana ait mesajları veritabaınında sorguluyoruz 
            //CreatedDate parametresine göre eskiden yeniye(orderby) sıralıyoruz
            var messages = await _chatMessageRepository
                .Where(x => x.DocumentId == documentId)
                .OrderBy(x => x.CreatedDate)
                .ToListAsync();

            //veritabanından gelen ham nesneleri (Entity) , AutoMapper ile DTO listesine dönüştürüyoruz
            var messagesDto = _mapper.Map<List<ChatMessageDto>>(messages);

            //tertemiz veriyi dış dünyaya dönüyoruz
            return messagesDto;
        }

        public async Task<List<DocumentDto>> SearchDocumentsAsync(string searchTerm, int userId)
        {
            // arama teriminin başındaki ve sonundaki boşlukları temizleyelim ve küçük harfe duyarlı hale getirmek için hazırlayalım
            var term = searchTerm?.ToLower().Trim() ?? string.Empty;

            //UnitOfWork üzerinden Document repositorysine ulaşaıyoruz
            //veritanabında LINQ ile ToLower() ve Contains() metotları SQL tarafında otomatik olarak "LIKE" sorgusuna çevrilecektir
            var documents = await _unitOfWork.GetRepository<Document>()
                .GetAllAsync(x => x.UserId == userId &&
                               ((x.FileName != null && x.FileName.ToLower().Contains(term)) ||
                               (x.ContentSummary != null && x.ContentSummary.ToLower().Contains(term))));

            //veritabanından gelen ham Document entity listesini AutoMapper ile DocumentDto listesine dönüştürüyoruz
            var documentDto = _mapper.Map<List<DocumentDto>>(documents);
            return documentDto;
        }










    }
}

