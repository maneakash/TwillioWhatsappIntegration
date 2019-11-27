using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JAWhatsAppApi.RabbitMq
{
    public class RMQMessage
    {
        public string Message { get; set; }
        public bool isSendToExpert { get; set; }
    }
}
