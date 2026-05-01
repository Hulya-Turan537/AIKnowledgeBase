using AIKnowledgeBase.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIKnowledgeBase.Core.Interfaces
{
    public interface ITagService
    {
        //geminiye dökümanı gönderip etiket önerisi alır
        Task<List<string>> GetSuggestTagsForDocumentAsync(int documentId);

        //alınan etiketleri dökümana bağlayacak metod
        Task AssignTagsToDocumentAsync(int documentId, List<string> tagNames);
    }
}
