using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIKnowledgeBase.Core.Entities
{
    public class ChatMessage : BaseEntity
    {
        public int DocumentId { get; set; }
        public string Role { get; set; } = string.Empty; //bu mesajı kim attı user/ai
        public string Content { get; set; } = string.Empty; //mesajın içeriği
        public virtual Document Document { get; set; } = null!; //mesajın ait olduğu dökümanla ilişki
    }
}
