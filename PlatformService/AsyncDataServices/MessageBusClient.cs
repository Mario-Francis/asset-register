using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using PlatformService.DTOs;
using RabbitMQ.Client;

namespace PlatformService.AsyncDataServices;
public class MessageBusClient : IMessageBusClient
{
    private readonly IConfiguration config;
    private readonly IConnection? rmqConnection;
    private readonly IModel? rmqChannel;

    public MessageBusClient(IConfiguration config)
    {
        this.config = config;
        var factory = new ConnectionFactory { HostName = config["RabbitMQHost"], Port = Convert.ToInt32(config["RabbitMQPort"]) };

        try
        {
            rmqConnection = factory.CreateConnection();
            rmqChannel = rmqConnection.CreateModel();
            rmqChannel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
            rmqConnection.ConnectionShutdown += OnRabbitMQConnectionShutdown;
            Console.WriteLine("--> Connected to Message Bus!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Could not connect to the Message Bus: {ex.Message}");
        }
    }
    public void PublishNewPlatform(PlatformPublishedDto platformPublishedDto)
    {
        var message = JsonSerializer.Serialize(platformPublishedDto);
        if(rmqConnection!.IsOpen){
            Console.WriteLine("--> RabbitMQ connection open, sending message...");
            SendMessaage(message);
        }else{
            Console.WriteLine("--> RabbitMQ connection closed, not sending");
        }
    }

    private void SendMessaage(string message){
        var body = Encoding.UTF8.GetBytes(message);
        rmqChannel.BasicPublish(exchange:"trigger", routingKey:"", basicProperties:null, body: body);
        Console.WriteLine($"--> We have sent message: {message}");
    }

    public void Dispose(){
        Console.WriteLine("MessageBus disposed");
        if(rmqChannel!.IsOpen){
            rmqChannel.Close();
            rmqConnection!.Close();
        }
    }

    private void OnRabbitMQConnectionShutdown(object? sender, ShutdownEventArgs e)
    {
        Console.WriteLine("--> RabbitMQ connection shutdown!");
    }
}