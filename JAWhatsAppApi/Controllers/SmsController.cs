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

        public string Index()
        {
            return "Whatsapp api running";
        }

        [HttpGet]
        [Route("ReadMessageFromRMQ")]
        public ActionResult ReadMessageFromRMQ()
        {
            /*  var factory = new ConnectionFactory() { HostName = "4dc06ccf.ngrok.io" , UserName = "guest", Password = "guest"};
              var message = "";
              using (var connection = factory.CreateConnection())
              using (var channel = connection.CreateModel())
              {
                  channel.ExchangeDeclare(exchange: "testExchange", type: ExchangeType.Direct);

                  var queueName = channel.QueueDeclare().QueueName;
                  channel.QueueBind(queue: queueName,
                      exchange: "testExchange",
                      routingKey: "");

                  Console.WriteLine(" [*] Waiting for logs.");

                  var consumer = new EventingBasicConsumer(channel);
                  consumer.Received += (model, ea) =>
                  {
                      var body = ea.Body;
                       message = Encoding.UTF8.GetString(body);

                      Console.WriteLine(" [x] {0}", message);
                  };
                  channel.BasicConsume(queue: queueName,
                      autoAck: true,
                      consumer: consumer);

                  Console.WriteLine(" Press [enter] to exit.");
                  Console.ReadLine();
              }*/

            var factory = new ConnectionFactory()
            { HostName = "4dc06ccf.ngrok.io" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "testFromCode",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                string message = "Hello World!";
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                    routingKey: "hello",
                    basicProperties: null,
                    body: body);
                Console.WriteLine(" [x] Sent {0}", message);

                return Content(message);
            }
        }

        [HttpPost]
        [Route("SendSms")]
        public ActionResult SendSms(SendSmsInput sendSmsInput)
        {
            var accountSid = _configuration.Value.AccountSid;
            var authToken = _configuration.Value.AuthToken;
            var fromNumber = _configuration.Value.FromNumber;

            TwilioClient.Init(accountSid, authToken);

            /*var messageOptions = new CreateMessageOptions(
                new PhoneNumber(WhatsAppConstants.WHATSAPPPREFIX + sendSmsInput.ToNumber));
            messageOptions.From = new PhoneNumber(WhatsAppConstants.WHATSAPPPREFIX + fromNumber);
            messageOptions.Body = sendSmsInput.MessageBody;

            var message = MessageResource.Create(messageOptions);*/

            var message = MessageResource.Create(
                body: sendSmsInput.MessageBody,
                from: new PhoneNumber(WhatsAppConstants.WHATSAPPPREFIX + fromNumber),
                // statusCallback: new Uri("https://localhost:44308/api/Sms/ReceiveSendSmsResponse"),
                to: new PhoneNumber(WhatsAppConstants.WHATSAPPPREFIX + sendSmsInput.ToNumber)
            );
            return Content("Sid : " + message.AccountSid + " Body : " + message.Body);

        }

        [HttpPost]
        [Route("ReceiveSendSmsResponse")]
        public ActionResult ReceiveSendSmsResponse(SmsStatusCallbackRequest incomingMessage)
        {
            SendSmsInput sendSmsInput = new SendSmsInput();

            sendSmsInput.ToNumber = "919561172408";
            sendSmsInput.MessageBody = "Your reg code is 123";

            return SendSms(sendSmsInput);

        }

        [HttpPost]
        [Route("ReceiveSms")]
        public TwiMLResult ReceiveSms(SmsRequest incomingMessage)
        {
            var messagingResponse = new MessagingResponse();
            messagingResponse.Message("Response: Go to hell! : " + incomingMessage.Body);

            return TwiML(messagingResponse);
        }
    }

}