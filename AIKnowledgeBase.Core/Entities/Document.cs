using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIKnowledgeBase.Core.Entities;

public class Document : BaseEntity
{
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string ContentSummary { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty; // Dökümanın tam metni 

    public int UserId { get; set; } // Belgeyi yükleyen kullanıcının ID'si
    public User User { get; set; } // Belgeyi yükleyen kullanıcıyla ilişki

    public ICollection<Tag> Tags { get; set; } = new List<Tag>();
}

