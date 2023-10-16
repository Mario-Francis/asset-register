
using System.Reflection;
using System.Text;
using CommandService.EventProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CommandService.AsyncDataServices;
public class MessageBusSubscriber : BackgroundService
{
    private readonly IConfiguration config;
    private readonly IEventProcessor eventProcessor;
    private IConnection rmqConnection = null!;
    private IModel rmqChannel = null!;
    private string queueName = null!;

    public MessageBusSubscriber(IConfiguration config, IEventProcessor eventProcessor)
    {
        this.config = config;
        this.eventProcessor = eventProcessor;
        InitializeRabbitMQ();
    }

    private void InitializeRabbitMQ()
    {
        var factory = new ConnectionFactory { HostName = config["RabbitMQHost"], Port = int.Parse(config["RabbitMQPort"]!) };

        rmqConnection = factory.CreateConnection();
        rmqChannel = rmqConnection.CreateModel();
        rmqChannel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
        queueName = rmqChannel.QueueDeclare().QueueName;
        rmqChannel.QueueBind(queue: queueName, exchange: "trigger", routingKey: "");

        Console.WriteLine("--> Listening on the Message Bus");

        rmqConnection.ConnectionShutdown += RabbitMQConnectionShutdown;
    }
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new EventingBasicConsumer(rmqChannel);
        consumer.Received += (ModuleHandle, ea) =>
        {
            Console.WriteLine("--> Event received!");

            var body = ea.Body;
            var notificationMessage = Encoding.UTF8.GetString(body.ToArray());

            eventProcessor.ProcessEvent(notificationMessage);
        };
        rmqChannel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
        return Task.CompletedTask;
    }

    private void RabbitMQConnectionShutdown(object? sender, ShutdownEventArgs e)
    {
        Console.WriteLine("--> Rabbit MQ connection shutdown");
    }

    public override void Dispose()
    {
        if (rmqChannel.IsOpen)
        {
            rmqChannel.Close();
            rmqConnection.Close();
        }
        base.Dispose();
    }
}