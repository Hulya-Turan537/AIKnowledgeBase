using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIKnowledgeBase.Core.Dtos
{
    public class ChatMessageDto
    {
        public string Role { get; set; } = string.Empty; //bu mesajı kim attı user/ai
        public string Content { get; set; } = string.Empty; //mesajın içeriği
        public DateTime CreatedDate { get; set; } //mesajın oluşturulma tarihi
    }
}
