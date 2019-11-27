using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JAWhatsAppApi.Common;
using JAWhatsAppApi.Models;
using JAWhatsAppApi.RabbitMq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Twilio.AspNet.Common;
using Twilio.AspNet.Core;
using Twilio.TwiML;

namespace JAWhatsAppApi.Controllers
{
    [Route("api/[controller]")]
    public class AutoResponseController : TwilioController
    {
        private IOptions<RMQConfig> _configurationRMQ;
        private Sender _sender;

        public AutoResponseController(IOptions<RMQConfig> configurationRMQ)
        {
            _sender = new Sender();
            _configurationRMQ = configurationRMQ;
        }

        [HttpPost]
        [Route("ReceiveSms")]
        public TwiMLResult ReceiveSms(SmsRequest incomingMessage)
        {
            RMQMessage message = new RMQMessage();
            message.Message = incomingMessage.Body;
            message.isSendToExpert = !incomingMessage.From.Equals(WhatsAppConstants.WHATSAPPPREFIX + "919561172408");
            _sender.sendSms(_configurationRMQ.Value, message);

            /*
            messagingResponse.Message("Response: Hi how may I help you? : " + incomingMessage.Body);*/

            //return TwiML(messagingResponse);
            return null;
        }
    }
}