using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIKnowledgeBase.Core.Entities;

public class Document : BaseEntity
{
    public string FilName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string ContentSummary { get; set; } = string.Empty;

    public ICollection<Tag> Tags { get; set; } = new List<Tag>();
}

