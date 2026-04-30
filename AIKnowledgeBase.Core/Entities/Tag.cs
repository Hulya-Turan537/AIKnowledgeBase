using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIKnowledgeBase.Core.Entities;

public class  Tag : BaseEntity
{
    public string Name { get; set; } = string.Empty;

       public ICollection<DocumentTag> DocumentTags { get; set; }
}