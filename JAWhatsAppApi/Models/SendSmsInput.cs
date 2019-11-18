using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JAWhatsAppApi.Models
{
    public class SendSmsInput
    {
        public string ToNumber { get; set; }
        public string MessageBody { get; set; }
    }
}
