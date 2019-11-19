using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JAWhatsAppApi.Common;
using JAWhatsAppApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Twilio;
using Twilio.AspNet.Common;
using Twilio.AspNet.Core;
using Twilio.Rest.Api.V2010.Account;
using Twilio.TwiML;
using Twilio.Types;


namespace CoreWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SmsController : TwilioController
    {
        private IOptions<TwilloConfig> _configuration;

        public SmsController(IOptions<TwilloConfig> configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public string Index()
        {
            return "Whatsapp api running";
        }



        [HttpPost]
        [Route("SendSms")]
        public ActionResult SendSms(SendSmsInput sendSmsInput)
        {
            SendWhatsAppMessage sendWhatsApp = new SendWhatsAppMessage();
            var message = sendWhatsApp.SendMessage(_configuration.Value, sendSmsInput);
            return Content("Sid : " + message.AccountSid + " Body : " + message.Body);

        }

      /*  [HttpPost]
        [Route("ReceiveSendSmsResponse")]
        public ActionResult ReceiveSendSmsResponse(SmsStatusCallbackRequest incomingMessage)
        {
            SendSmsInput sendSmsInput = new SendSmsInput();

            sendSmsInput.ToNumber = "919561172408";
            sendSmsInput.MessageBody = "Your reg code is 123";

            return SendSms(sendSmsInput);

        }*/

        [HttpPost]
        [Route("ReceiveSms")]
        public TwiMLResult ReceiveSms(SmsRequest incomingMessage)
        {
            var messagingResponse = new MessagingResponse();
            messagingResponse.Message("Response: Hi how may I help you? : " + incomingMessage.Body);

            return TwiML(messagingResponse);
        }
    }

}