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
        public string sendSms(RMQConfig configuration)
        {
            var factory = new ConnectionFactory() { HostName = configuration.HostName, Password = configuration.Password, UserName = configuration.UserName };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "hello",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);



                string message = "Testing RMQ ";
                var body = Encoding.UTF8.GetBytes(message);



                channel.BasicPublish(exchange: "",
                                     routingKey: "hello",
                                     basicProperties: null,
                                     body: body);

                return message;
            }
        }
    }
}
