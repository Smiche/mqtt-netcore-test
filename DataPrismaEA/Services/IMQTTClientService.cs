using DataPrismaEA.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataPrismaEA.Services
{
    public interface IMQTTClientService
    {
        Boolean SendMQTTMessage(ChatMessage msg);

    }
}
