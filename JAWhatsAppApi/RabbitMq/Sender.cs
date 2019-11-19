using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using JAWhatsAppApi.Models;
using JAWhatsAppApi.RabbitMq;

namespace JAWhatsAppApi.RabbitMq
{
    public class Sender
    {
        public string sendSms(RMQConfig configuration,RMQMessageBody messageBody)
        {
            var factory = new ConnectionFactory() { HostName = configuration.HostName, Password = configuration.Password, UserName = configuration.UserName };
            using (var connection = factory.CreateConnection())
            using (var model = connection.CreateModel())
            {
                string message = messageBody.MessageBody;
                var properties = model.CreateBasicProperties();

                properties.Persistent = false;

                byte[] messagebuffer = Encoding.Default.GetBytes(message);
                model.BasicPublish("demoExchange", "directexchange_key", properties, messagebuffer);

                return message;
            }
        }
    }
}
