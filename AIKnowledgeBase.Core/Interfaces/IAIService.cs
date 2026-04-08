using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIKnowledgeBase.Core.Interfaces
{
    public interface IAIService
    {
        //kullanıcıdan bir prompt ve döküman metni alıp cevap dönecek
        Task<string> AnalyzeTextAsync(string documentText, string userQuestion);
    }
}
