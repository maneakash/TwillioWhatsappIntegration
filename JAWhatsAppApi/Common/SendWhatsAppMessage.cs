using JAWhatsAppApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace JAWhatsAppApi.Common
{
    public class SendWhatsAppMessage
    {
        public MessageResource SendMessage(TwilloConfig twilloConfig, SendSmsInput sendSmsInput)
        {
            var accountSid = twilloConfig.AccountSid;
            var authToken = twilloConfig.AuthToken;
            var fromNumber = twilloConfig.FromNumber;
            var toNumber = twilloConfig.ToNumber == null ? "919036478920" : twilloConfig.ToNumber;

            TwilioClient.Init(accountSid, authToken);

            var message = MessageResource.Create(
                body: sendSmsInput.MessageBody,
                from: new PhoneNumber(WhatsAppConstants.WHATSAPPPREFIX + fromNumber),
                // statusCallback: new Uri("https://localhost:44308/api/Sms/ReceiveSendSmsResponse"),
                to: new PhoneNumber(WhatsAppConstants.WHATSAPPPREFIX + toNumber)
            );

            return message;
        }
    }
}
