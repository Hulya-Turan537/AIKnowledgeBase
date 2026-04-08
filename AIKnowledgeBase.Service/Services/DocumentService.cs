using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AIKnowledgeBase.Core.Interfaces;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using System.Text;
using System.Security.Cryptography.X509Certificates;

namespace AIKnowledgeBase.Service.Services
{
    public class DocumentService : IDocumentService
    {
        public async Task<string> GetTextFromFileAsync(string filePath)
        {
            var text = new StringBuilder();

            //itext7 kütüphanesini kullanarak PDF i açıyoruz
            using (PdfReader reader = new PdfReader(filePath))
            using (PdfDocument pdfDoc = new PdfDocument(reader))
            {
                await Task.CompletedTask; //asenkron işlemi simüle ediyoruz, çünkü iText7'nin PDF okuma işlemi asenkron değil
                for (int i =1; i <= pdfDoc.GetNumberOfPages(); i++)
                {
                    //her sayfanın içeriğini alıyoruz
                    var strategy = new LocationTextExtractionStrategy();
                    string pageText = PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(i), strategy);
                    text.Append(pageText); //metni birleştiriyoruz
                }
            }

            return text.ToString(); //metni string olarak döndürüyoruz
        }
    }
}
