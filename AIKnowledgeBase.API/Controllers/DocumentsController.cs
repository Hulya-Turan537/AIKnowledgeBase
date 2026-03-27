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

        [HttpPost]
        public async Task<IActionResult> CreateDocument(DocumentDto documentDto)
        {
            // DTO yu gerçek entitye çeviriyoruz
            var document = _mapper.Map<Document>(documentDto);
            document.CreatedDate = DateTime.Now;

            await _documentRepository.AddAsync(document);
            await _unitOfWork.CommitAsync();

            return Ok(CustomResponseDto<DocumentDto>.Success(201, documentDto));
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
