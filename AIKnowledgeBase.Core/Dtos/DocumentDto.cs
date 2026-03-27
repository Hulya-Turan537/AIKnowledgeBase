using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIKnowledgeBase.Core.Dtos;

public class DocumentDto
{
    public string Filename { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string ContentSummary { get; set; } = string.Empty;

    public int UserId { get; set; } // Bu dökümanı hangi kullanıcıya kaydedeceğimizi bilmemiz lazım
}
