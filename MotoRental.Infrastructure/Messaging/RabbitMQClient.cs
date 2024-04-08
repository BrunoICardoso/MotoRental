using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MotoRental.Core.Enum;
using RabbitMQ.Client;


namespace MotoRental.Infrastructure.Messaging
{
    public class RabbitMQClient
    {
        private readonly ConnectionFactory _connectionFactory;

        public RabbitMQClient(string hostName, string userName, string password)
        {
            _connectionFactory = new ConnectionFactory() { HostName = hostName, UserName = userName, Password = password };
        }

        public void PublishMessage(QueueNamesEnum queueNameEnum, string message)
        {
            string queueName = queueNameEnum.GetDescription();

            using (var connection = _connectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);
            }
        }
    }
}
