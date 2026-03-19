using Communication.Application.Events;
using Communication.Application.Interfaces;
using Communication.Domain.Enums;
using Communication.Domain.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Communication.Worker;

public class Worker : BackgroundService
{
    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly ILogger<Worker> logger;
    private readonly IConfiguration configuration;
    private readonly string _hostName;

    public Worker(IServiceScopeFactory serviceScopeFactory, ILogger<Worker> logger, IConfiguration configuration)
    {
        this.serviceScopeFactory = serviceScopeFactory;
        this.logger = logger;
        this.configuration = configuration;
        _hostName = configuration["RabbitMQ:Host"] ?? "localhost";
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory { HostName = _hostName };
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync("communication-requests", durable: true,
            exclusive: false, autoDelete: false, arguments: null);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);
                var evt = JsonSerializer.Deserialize<CommunicationScheduledEvent>(json);

                using var scope = serviceScopeFactory.CreateScope();
                var repository = scope.ServiceProvider.GetRequiredService<ISchedulingRepository>();

                var scheduling = await repository.GetByIdAsync(evt!.Id);
                if (scheduling is not null)
                {
                    if (evt.Type == CommunicationType.Email.ToString())
                    {
                        var message = new MimeMessage();
                        message.From.Add(new MailboxAddress(
                            configuration["Email:FromName"],
                            configuration["Email:Username"]));
                        message.To.Add(MailboxAddress.Parse(evt.Recipient));
                        message.Subject = "Mensagem automática(Não responda)";
                        message.Body = new TextPart("html") { 
                            Text = $"""
                                        <div style="font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;">
                                            <h2 style="color: #4A90D9;">📬 Communication</h2>
                                            <hr style="border: 1px solid #eee;" />
                                            <p>Olá! Você recebeu uma nova comunicação automática:</p>
                                            <div style="background: #f9f9f9; padding: 16px; border-radius: 8px;">
                                                <p><strong>Mensagem:</strong></p>
                                                <p>{evt.Message}</p>
                                            </div>
                                            <br/>
                                            <p style="color: #888; font-size: 12px;">
                                                Enviado por {configuration["Email:FromName"]}
                                            </p>
                                        </div>
                                    """
                        };
                        using var client = new SmtpClient();
                        await client.ConnectAsync(
                            configuration["Email:Host"],
                            int.Parse(configuration["Email:Port"]!),
                            SecureSocketOptions.StartTls,
                            stoppingToken);
                        await client.AuthenticateAsync(
                            configuration["Email:Username"],
                            configuration["Email:Password"],
                            stoppingToken);
                        await client.SendAsync(message, stoppingToken);
                        await client.DisconnectAsync(true, stoppingToken);
                        logger.LogInformation("Email enviado para {Recipient}", evt.Recipient);
                    }

                    scheduling.MarkAsSent();
                    await repository.UpdateAsync(scheduling);
                    logger.LogInformation("Agendamento {Id} marcado como enviado.", evt.Id);
                }

                await channel.BasicAckAsync(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao processar mensagem da fila.");
                await channel.BasicNackAsync(ea.DeliveryTag, false, requeue: false);
            }
        };

        await channel.BasicConsumeAsync("communication-requests", autoAck: false, consumer: consumer);

        // Mantém o Worker vivo até ser cancelado
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}