using CommandService.AsyncDataServices;
using CommandService.Data;
using CommandService.EventProcessing;
using CommandService.SyncDataServices.Grpc;
using Microsoft.EntityFrameworkCore;

namespace CommandService;
public class ServiceRegistry
{
    public static void RegisterService(WebApplicationBuilder builder){
        builder.Services.AddDbContext<AppDbContext>(options=> options.UseInMemoryDatabase("in-mem"));
        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        builder.Services.AddScoped<ICommandRepo, CommandRepo>();
        builder.Services.AddSingleton<IEventProcessor, EventProcessor>();
        builder.Services.AddScoped<IPlatformDataClient, PlatformDataClient>();

        builder.Services.AddHostedService<MessageBusSubscriber>();
    }
}