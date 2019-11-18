using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JAWhatsAppApi.Common
{
    public class WhatsAppConstants
    {
        public static string WHATSAPP { get; set; } = "whatsapp";
        public static string WHATSAPPPREFIX { get; set; } = WHATSAPP + SymbolUtil.COLON + SymbolUtil.PLUS;
    }
}
