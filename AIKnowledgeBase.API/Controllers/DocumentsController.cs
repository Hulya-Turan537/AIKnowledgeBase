using AIKnowledgeBase.Core.Dtos;
using AIKnowledgeBase.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AIKnowledgeBase.Core.Entities;
using AutoMapper;


namespace AIKnowledgeBase.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper; 


        public DocumentsController(IDocumentRepository documentRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _documentRepository = documentRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> CreateDocument(IFormFile file, int userId)
        {
            //dosya geldi mi
            if (file == null || file.Length == 0)
                return BadRequest("Dosya seçilmedi.");

            //kaydedilecek yolu belirliyoruz
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", file.FileName);

            //dosyayı fiziksel olarak klasöre kopyalıyoruz
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            //veritanına kaydetmek için Document entitysi oluşturuyoruz
            var document = new Document
            {
                FileName = file.FileName,
                FilePath = "/uploads/" + file.FileName, //dosyanın erişim yolu, bu yolu frontendde kullanacağız
                UserId = userId,
                CreatedDate = DateTime.Now
            };

            await _documentRepository.AddAsync(document); //entityi ekliyoruz
            await _unitOfWork.CommitAsync(); //değişiklikleri kaydediyoruz
            var documentDto = _mapper.Map<DocumentDto>(document); //entityi DTOya çeviriyoruz
            return Ok(CustomResponseDto<DocumentDto>.Success(201, documentDto)); //standart yanıt paketimizle dönüyoruz
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            // 1. Özel repository'mizi kullanarak verileri çekiyoruz
            var documents = await _documentRepository.GetDocumentsByUserIdAsync(userId);

            // 2. Entity listesini DTO listesine çeviriyoruz
            var documentsDto = _mapper.Map<List<DocumentDto>>(documents);

            // 3. Standart yanıt paketimizle (Success 200) dönüyoruz
            return Ok(CustomResponseDto<List<DocumentDto>>.Success(200, documentsDto));
        }
    }
}
