using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataPrismaEA.Types
{
    public class ChatMessage
    {
        public ChatMessage() { }
        public int eventId { get; set; }

        public int chatRoomId { get; set; }

        public string content { get; set; }

        public string timestamp { get; set; }
        public User user { get; set; }

    }
}
