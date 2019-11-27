using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JAWhatsAppApi.Common;
using JAWhatsAppApi.Models;
using JAWhatsAppApi.RabbitMq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace JAWhatsAppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RabbitMqController : ControllerBase
    {

        private IOptions<RMQConfig> _configuration;
        private Sender _sender;

        public RabbitMqController(IOptions<RMQConfig> configuration)
        {
            _configuration = configuration;
            _sender = new Sender();
        }

        [HttpGet]
        public string Index()
        {
            return "RabitMQ is  running";
        }

        [HttpPost]
        [Route("SendMessageToRabbitQueue")]
        public ActionResult SendMessageToRabbitQueue(RMQMessage messageBody)
        {
            var response = _sender.sendSms(_configuration.Value, messageBody);
            return Content(response);
        }


    }
}