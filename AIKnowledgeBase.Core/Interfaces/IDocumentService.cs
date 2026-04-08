using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIKnowledgeBase.Core.Interfaces
{
    public interface IDocumentService
    {
        //dosya yolunu vereceğiz o bize içindeki metni string olarak dönecek
        Task<string> GetTextFromFileAsync(string filePath);
    }
}
