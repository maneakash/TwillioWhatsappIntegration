using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JAWhatsAppApi.Models
{
    public class TwilloConfig
    {
        public string AccountSid { get; set; }
        public string AuthToken { get; set; }
        public string FromNumber { get; set; }
    }
}
