using JAWhatsAppApi.Common;
using JAWhatsAppApi.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace JAWhatsAppApi.RabbitMq
{
    public class ConsumeRabbitMQHostedService : BackgroundService
    {
        private readonly ILogger _logger;
        private IConnection _connection;
        private IModel _channel;
        private IOptions<RMQConfig> _configuration;
        private IOptions<TwilloConfig> _twilloConfig;

        public ConsumeRabbitMQHostedService(ILoggerFactory loggerFactory, IOptions<RMQConfig> _configuration, IOptions<TwilloConfig> _twilloConfig)
        {
            this._logger = loggerFactory.CreateLogger<ConsumeRabbitMQHostedService>();
            this._configuration = _configuration;
            this._twilloConfig = _twilloConfig;
            InitRabbitMQ();
        }

        private void InitRabbitMQ()
        {
            //var factory = new ConnectionFactory() { HostName = _configuration.Value.HostName, Password = _configuration.Value.Password, UserName = _configuration.Value.UserName };
            var factory = new ConnectionFactory();
            factory.Uri = new Uri(_configuration.Value.HostName.Replace("amqp://", "amqps://"));

            // create connection  
            _connection = factory.CreateConnection();

            // create channel  
            _channel = _connection.CreateModel();

            _channel.QueueDeclare("demoqueue", true, false, false, null);
            _channel.BasicQos(0, 1, false);

            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ch, ea) =>
            {
                // received message  
                var content = System.Text.Encoding.UTF8.GetString(ea.Body);

                // handle the received message  
                HandleMessage(content);
                _channel.BasicAck(ea.DeliveryTag, false);
            };

            consumer.Shutdown += OnConsumerShutdown;
            consumer.Registered += OnConsumerRegistered;
            consumer.Unregistered += OnConsumerUnregistered;
            consumer.ConsumerCancelled += OnConsumerConsumerCancelled;

            _channel.BasicConsume("demoqueue", false, consumer);
            return Task.CompletedTask;
        }

        private void HandleMessage(string content)
        {
            RMQMessage msgContent = JsonConvert.DeserializeObject<RMQMessage>(content);
            string toNumber;
            if (msgContent.isSendToExpert)
            {
                content = "A customer has asked you a question with following details : " + msgContent.Message +
                          " Please click here https://bit.ly/2CWbJta to answer the question!";
                toNumber = _twilloConfig.Value.ExpertNumber;

            }
            else
            {
                content = "An expert on JustAnswer responded to your question with following details : " + msgContent.Message +
                          " Please click here https://bit.ly/2CWbJta to continue chat!";
                toNumber = _twilloConfig.Value.CustomerNumber;
            }

            SendWhatsAppMessage sendWhatsApp = new SendWhatsAppMessage();
            sendWhatsApp.SendMessage(_twilloConfig.Value, new SendSmsInput() { MessageBody = content, ToNumber = toNumber });
            _logger.LogInformation($"consumer received {content}");
        }

        private void OnConsumerConsumerCancelled(object sender, ConsumerEventArgs e) { }
        private void OnConsumerUnregistered(object sender, ConsumerEventArgs e) { }
        private void OnConsumerRegistered(object sender, ConsumerEventArgs e) { }
        private void OnConsumerShutdown(object sender, ShutdownEventArgs e) { }
        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e) { }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}
