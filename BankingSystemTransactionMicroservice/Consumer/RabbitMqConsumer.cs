using System.Text;
using BankingSystemTransactionMicroservice.Models;
using BankingSystemTransactionMicroservice.Services;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace BankingSystemTransactionMicroservice.Consumer;


public class RabbitMqConsumerService : IHostedService
{
    private readonly ILogger<RabbitMqConsumerService> _logger;
    private readonly RabbitMqService _rabbitMqService;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private IModel _channel;
    private IConnection _connection;

    public RabbitMqConsumerService(ILogger<RabbitMqConsumerService> logger, RabbitMqService rabbitMqService, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _rabbitMqService = rabbitMqService;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("RabbitMqConsumerService is starting.");

        try
        {
            _connection = _rabbitMqService.GetConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: "recurrent_transactions",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                _logger.LogInformation("Received message: {Message}", message);

                try
                {
                    var data = JsonConvert.DeserializeObject<Recurrenttransaction>(message);
                    await HandleMessageAsync(data);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error handling message");
                }
            };

            _channel.BasicConsume(queue: "recurrent_transactions",
                autoAck: true,
                consumer: consumer);

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while starting RabbitMqConsumerService.");
            throw;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _channel?.Close();
        _connection?.Close();
        return Task.CompletedTask;
    }

    private async Task HandleMessageAsync(Recurrenttransaction data)
    {
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<BankingSystemMicroContext>();
            var recurrentTransaction = new Recurrenttransaction
            {
                Accountid = data.Accountid,
                Amount = data.Amount,
                Transactiontype = data.Transactiontype,
                Createdat = data.Createdat,
                Frequency = data.Frequency,
                Nexttransactiondate = data.Nexttransactiondate,
                Recurrenttransactionid = data.Recurrenttransactionid,
                Branchid = data.Branchid
            };
            dbContext.Recurrenttransactions.Add(recurrentTransaction);
            await dbContext.SaveChangesAsync();
        }
    }
}