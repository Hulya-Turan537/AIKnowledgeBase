using AIKnowledgeBase.Core.Entities;
using AIKnowledgeBase.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AIKnowledgeBase.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class  TagsController : ControllerBase
    {

        private readonly ITagService _tagService;

        public TagsController(ITagService tagService)
        {
            _tagService = tagService;
        }



        //dökümana ait tüm etiketler, getir
        [HttpGet("document/{documentId}")]
        public async Task<IActionResult> GetTagsByDocument(int documentId)
        {
            var tags = await _tagService.GetTagsByDocumentIdAsync(documentId);
            return Ok(tags);
        }

        //MANUEL ETİKET EKLE
        [HttpPost("{documentId}/add-manual")]
        public async Task<IActionResult> AddManualTag(int documentId, [FromBody] string tagName)
        {
            var result = await _tagService.AddManualTagToDocumentAsync(documentId, tagName);
            if (result)
                return Ok(new {Message = $"'{tagName}' etiketi başarıyla eklendi."});

            return BadRequest("Etiket eklenirken bir hata oluştu.");
        }

        //etiketi dökümandan kaldır
        [HttpDelete("{documentId}/remove/{tagId}")]
        public async Task<IActionResult> RemoveTag(int documentId, int tagId)
        {
            var result = await _tagService.RemoveTagFromDocumentAsync(documentId, tagId);
            if (result)
                return Ok(new {Message = "Etiket dökümandan başarıyla kaldırıldı."});

            return NotFound("Belirtilen etiket veya döküman bulunamadı.");

        }
    }
}
