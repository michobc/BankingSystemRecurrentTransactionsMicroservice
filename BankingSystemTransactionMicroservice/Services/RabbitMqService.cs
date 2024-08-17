using RabbitMQ.Client;

namespace BankingSystemTransactionMicroservice.Services;

public class RabbitMqService
{
    private readonly IConfiguration _configuration;

    public RabbitMqService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IConnection GetConnection()
    {
        var factory = new ConnectionFactory()
        {
            HostName = _configuration["RabbitMQ:HostName"],
            UserName = _configuration["RabbitMQ:UserName"],
            Password = _configuration["RabbitMQ:Password"]
        };
        return factory.CreateConnection();
    }
}