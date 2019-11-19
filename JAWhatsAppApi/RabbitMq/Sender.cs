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
        public string sendSms(RMQConfig configuration, RMQMessageBody messageBody)
        {
            //var factory = new ConnectionFactory() { HostName = configuration.HostName, Password = configuration.Password, UserName = configuration.UserName };

            var factory = new ConnectionFactory();
            factory.Uri = new Uri(configuration.HostName.Replace("amqp://", "amqps://"));
            using (var connection = factory.CreateConnection())
            using (var model = connection.CreateModel())
            {
                string message = messageBody.MessageBody;
                var properties = model.CreateBasicProperties();

                properties.Persistent = false;

                // Create Exchange
                model.ExchangeDeclare("demoExchange", ExchangeType.Direct);

                // Create Queue
                model.QueueDeclare("demoqueue", true, false, false, null);

                // Bind Queue to Exchange
                model.QueueBind("demoqueue", "demoExchange", "directexchange_key");

                byte[] messagebuffer = Encoding.Default.GetBytes(message);
                // Publish message to Queue
                model.BasicPublish("demoExchange", "directexchange_key", properties, messagebuffer);

                return message;
            }
        }
    }
}
