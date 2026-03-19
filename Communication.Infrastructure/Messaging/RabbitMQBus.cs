using Communication.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Communication.Infrastructure.Messaging
{
    public class RabbitMQBus : IMessageBus
    {
        private readonly string _hostName;

        public RabbitMQBus(IConfiguration configuration)
        {
            _hostName = configuration["RabbitMQ:Host"] ?? "localhost";
        }

        public async Task PublishAsync<T>(T message, string queueName)
        {
            // 1. Criamos a fábrica de conexões
            var factory = new ConnectionFactory { HostName = _hostName };

            // 2. Abrimos a conexão e o canal (Channel)
            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            // 3. Declaramos a fila (garante que ela exista)
            await channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            // 4. Serializamos o objeto para JSON e depois para bytes
            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            // 5. Publicamos a mensagem na fila
            await channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: queueName,
                body: body);
        }
    }
}
