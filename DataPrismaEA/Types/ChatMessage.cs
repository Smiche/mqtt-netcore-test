using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataPrismaEA.Types
{
    public class ChatMessage
    {
        public ChatMessage() { }
        public string eventId { get; set; }

        public string chatRoomId { get; set; }

        public string content { get; set; }

        public string timestamp { get; set; }
        public User user { get; set; }

    }
}
