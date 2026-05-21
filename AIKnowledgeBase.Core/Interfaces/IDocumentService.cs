using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace AIKnowledgeBase.Core.Interfaces
{
    public interface IDocumentService
    {
        //dosya yolunu vereceğiz o bize içindeki metni string olarak dönecek
        Task<string> GetTextFromFileAsync(string filePath);

        Task<string> AskQuestionAsync(int documentId, string question);

        //bu metot dökümanı işleyecek, özetini çıkaracak ve UnitOfWork ile kaydedecek, böylece döküman veritabanında saklanacak ve daha sonra sorulara cevap verirken kullanılabilecek
        Task<AIKnowledgeBase.Core.Entities.Document> SaveAndProcessDocumentAsync(string fileName, string filePath, int userId);
    }
}
