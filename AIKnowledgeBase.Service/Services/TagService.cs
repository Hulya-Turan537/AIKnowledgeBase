using AIKnowledgeBase.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AIKnowledgeBase.Core.Entities;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace AIKnowledgeBase.Service.Services
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _tagRepository;
        private readonly IDocumentRepository _documentRepository;
        private readonly IAIService _geminiService;

        //constructor service oluşturulurken ihtiyaç duyulan bağımlılıkları alır ve sınıf içinde kullanılmak üzere saklar
        public TagService(ITagRepository tagRepository, IDocumentRepository documentRepository, IAIService geminiService)
        {
            _tagRepository = tagRepository;
            _documentRepository = documentRepository;
            _geminiService = geminiService;
        }

        public async Task<List<string>> GetSuggestTagsForDocumentAsync(int documentId)
        {
            //dökümanı bul. veritabanından dökümanı alıyoruz
            var document = await _documentRepository.GetByIdAsync(documentId);
            if (document == null) return new List<string>();

            string? extractedPath = null;

            if(document.Content != null && document.Content.Contains("[IMAGE_FILE]"))
            {
                extractedPath = document.Content
                    .Replace("[IMAGE_FILE]:", "")
                    .Replace("[IMAGE_FILE]", "")
                    .Trim()
                    .Replace("/", "\\"); // Windows slaşlarını düzeltelim
            }

            //aı ya dökümanının içeriğini gönderip bana 3 etiket öner diyelim
            string prompt = $"Bu Döküman içeriğine (görseline) dayanarak en uygun 3 anahtar kelimeyi (etiket) sadece aralarına virgül koyarak yaz: {document.Content}";

            var aiResponse = await _geminiService.AnalyzeTextAsync(
                document.Content, //documentText
                prompt, //userQuestion
                new List<ChatMessage>(), // history, etiket önerisi için geçmişe ihtiyacımız yok, boş liste gönderiyoruz
                extractedPath
                );

            if (string.IsNullOrEmpty(aiResponse)) return new List<string>();

            //gelen cevabı listeye çevirelim
            return aiResponse.Split(',').Select(t => t.Trim()).ToList();
        }

        public async Task AssignTagsToDocumentAsync(int documentId, List<string> tagNames)
        {

            var document = await _documentRepository.GetByIdAsync(documentId);
            if (document == null) return;


            foreach (var name in tagNames)
            {
                //önce veritabanında bu isimde bir etiket var mı kontrol edelim. yoksa yeni oluşturacağız
                var existingTag = await _tagRepository.GetByNameAsync(name);

                if (existingTag == null)
                {
                    existingTag = new Tag { Name = name };
                    await _tagRepository.AddAsync(existingTag);

                    await _tagRepository.SaveAsync(); //yeni etiketi kaydedelim ki id'si oluşsun ve ilişki kurabilelim
                }

                

                //eğer bu dökümanda bu etiket zaten yoksa ekleyelim. böylece aynı etiketten birden fazla olmaz
                if (!document.DocumentTags.Any(dt => dt.TagId == existingTag.Id))
                {
                    document.DocumentTags.Add(new DocumentTag
                    {
                        DocumentId = documentId,
                        TagId = existingTag.Id
                    });

                    await _tagRepository.SaveAsync();
                }
            }


        }

        public async Task<bool> RemoveTagFromDocumentAsync(int documentId, int tagId)
        {
            var document = await _documentRepository.GetByIdAsync(documentId);
            if (document == null) return false;

            //dökümanın etiketleri arasında bu tagId ye sahip olanı bul
            var tagToRemove = document.DocumentTags.FirstOrDefault(dt => dt.TagId == tagId);

            if (tagToRemove != null)
            {
                document.DocumentTags.Remove(tagToRemove);
                await _tagRepository.SaveAsync();
                return true;
            }
            return false;
        }


        public async Task<bool> AddManualTagToDocumentAsync(int documentId, string tagName)
        {
            await AssignTagsToDocumentAsync(documentId, new List<string> { tagName });
            return true;
        }

        public async Task<List<Object>> GetTagsByDocumentIdAsync(int documentId)
        {
            var document = await _documentRepository.GetByIdAsync(documentId);
            if (document == null) return new List<object>();

            return document.DocumentTags
        .Select(dt => new
        {
            Id = dt.TagId,
            Name = dt.Tag != null ? dt.Tag.Name : "İsimsiz Etiket"
        })
        .ToList<object>();
        }
    }
}
