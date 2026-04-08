using AIKnowledgeBase.Core.Dtos;
using AIKnowledgeBase.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AIKnowledgeBase.Core.Entities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;




namespace AIKnowledgeBase.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper; 
        private readonly IDocumentService _documentService;
        private readonly IAIService _aiService;


        public DocumentsController(
            IDocumentRepository documentRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper, 
            IDocumentService documentService,
            IAIService aiService)
        {
            _documentRepository = documentRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _documentService = documentService;
            _aiService = aiService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> CreateDocument(IFormFile file, int userId)
        {
            try
            {


                //dosya geldi mi
                if (file == null || file.Length == 0)
                    return BadRequest(CustomResponseDto<NoContentDto>.Fail(400, new List<string> { "Dosya seçilmedi." }));

                //dosya boyutu
                if (file.Length > 5 *1024 *1024)
                    return BadRequest(CustomResponseDto<NoContentDto>.Fail(400, new List<string> { "Dosya boyutu 5MB'dan büyük olamaz." }));

                //dosyanın uzantısını alıyoruz(.pdf , .jpg, vb,)
                var allowedExtensions = new[] {".pdf", ".doc", ".docx", ".jpg", ".jpeg", ".png" };
                var extension = Path.GetExtension(file.FileName).ToLower();
                if(!allowedExtensions.Contains(extension))
                    return BadRequest(CustomResponseDto<NoContentDto>.Fail(400, new List<string> { "Sadece PDF, Word veya Görsel dosyaları yüklenebilir." }));

                //Rastgele ve benzersiz bir isim oluşturuyoruz
                var newFileName = Guid.NewGuid().ToString() + extension;

                //kaydedilecek yolu belirliyoruz
                var folderpath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

                //klasör yoksa oluşturuyoruz
                if (!Directory.Exists(folderpath))
                {
                    Directory.CreateDirectory(folderpath);
                }

                var path = Path.Combine(folderpath, newFileName);

                //dosyayı fiziksel olarak klasöre kopyalıyoruz
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                //veritanına kaydetmek için Document entitysi oluşturuyoruz
                var document = new Document
                {
                    FileName = file.FileName,
                    FilePath = "/uploads/" + newFileName, //dosyanın erişim yolu, bu yolu frontendde kullanacağız
                    UserId = userId,
                    CreatedDate = DateTime.Now
                };

                await _documentRepository.AddAsync(document); //entityi ekliyoruz
                await _unitOfWork.CommitAsync(); //değişiklikleri kaydediyoruz

                var documentDto = _mapper.Map<DocumentDto>(document); //entityi DTOya çeviriyoruz
                return Ok(CustomResponseDto<DocumentDto>.Success(201, documentDto)); //standart yanıt paketimizle dönüyoruz
            }
            catch (Exception ex)
            {
                //beklenmedik bir hata olursa sistemi çokertme , logla ve bilgi ver
                return StatusCode(500, CustomResponseDto<NoContentDto>.Fail(500, new List<string> { "Sunucu taraflı bir hata oluştu: " + ex.Message }));
            }
        }


        [HttpPost("{id}/ask")]
        public async Task<IActionResult> AskQuestionToDocument(int id, [FromBody] string question)
        {
            var document = await _documentRepository.GetByIdAsync(id);
            if (document == null) return NotFound("Döküman bulunamadı.");

            //fiziksel yolu oluştur ve dökümanı metne çevir
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", document.FilePath.TrimStart('/'));
            var documentText = await _documentService.GetTextFromFileAsync(fullPath);

            //okunan metni ve kuulanıcı sorusunu AI servisine gönder ve cevabı al
            try
            {
                var aiResponse = await _aiService.AnalyzeTextAsync(documentText, question);

                return Ok(new { Answer = aiResponse });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"AI analizi sırasında bir hata oluştu: {ex.Message}");
            }
        }











        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetDocumentsByUserId(int userId)
        {
            // repository üzerinden bu userId ye ait dökümanları filtrele
            var documents = await _documentRepository.Where(x => x.UserId == userId).ToListAsync();

            if (documents == null || !documents.Any())
                return Ok(CustomResponseDto<List<DocumentDto>>.Success(200, new List<DocumentDto>()));

            //gelen documents listesi boş değilse DTOya çevir ve dön
            var documentsDto = _mapper.Map<List<DocumentDto>>(documents);

            //customresponse ile dönüyoruz
            return Ok(CustomResponseDto<List<DocumentDto>>.Success(200, documentsDto));

           
        }

        [HttpGet("{id}/content")]
        public async Task<IActionResult> GetDocumentContent(int id)
        {
            //  Veritabanından dökümanı bul
            var document = await _documentRepository.GetByIdAsync(id);
            if (document == null) return NotFound("Döküman bulunamadı.");

            //  Dosyanın fiziksel tam yolunu oluştur
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", document.FilePath.TrimStart('/'));

            //  Servisimizi kullanarak metni oku
            try
            {
                var text = await _documentService.GetTextFromFileAsync(fullPath);

                // Okunan metni geri dön
                return Ok(new { Content = text });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Dosya okunurken hata oluştu: {ex.Message}");
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateDocument(DocumentUpdateDto documentUpdateDto)
        {
            //güncellenecek belgeyi bu
            var document = await _documentRepository.GetByIdAsync(documentUpdateDto.Id);

            if (document == null)
            {
                return NotFound(CustomResponseDto<NoContentDto>.Fail(404, new List<string> { " Güncellenecek belge bulunamadı." }));
            }

            //sadece belge adını güncelliyoruz, dosya değişmiyor, bu yüzden filePath ve diğer özellikler aynı kalacak
            document.FileName = documentUpdateDto.FileName;

            //repository update ve commit
            _documentRepository.Update(document);
            await _unitOfWork.CommitAsync();

            return Ok(CustomResponseDto<NoContentDto>.Success(204));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDocument(int id)
        {
            // Belgeyi veritabanından buluyoruz
            var document = await _documentRepository.GetByIdAsync(id);

            if (document == null)
                return NotFound(CustomResponseDto<NoContentDto>.Fail(404, new List<string> { "Belge bulunamadı." }));

            //dosya varsa fiziksel yolunu belirle
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", document.FilePath.TrimStart('/'));

            // Fiziksel dosyayı diskten siliyoruz
            try
            {
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }
            catch (IOException ex)
            {
                // Dosya o an başka bir işlem tarafından kullanılıyor olabilir (Lock)
                return StatusCode(500, CustomResponseDto<NoContentDto>.Fail(500, new List<string> { "Dosya fiziksel olarak silinirken bir hata oluştu: " + ex.Message }));
            }

            // Veritabanından belge kaydını siliyoruz
            _documentRepository.Remove(document);
           await _unitOfWork.CommitAsync(); //değişiklikleri kaydediyoruz

            return Ok(CustomResponseDto<NoContentDto>.Success(204));
        }


    }
}
