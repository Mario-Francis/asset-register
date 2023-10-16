using Microsoft.EntityFrameworkCore;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.SyncDataServices.Http;

namespace PlatformService;
public class ServiceRegistry{
    public static void RegisterServices(WebApplicationBuilder builder){
        if(builder.Environment.IsProduction()){
            Console.WriteLine("--> Using sql server db");
            var connStr = builder.Configuration.GetConnectionString("PlatformsConnection");
            builder.Services.AddDbContext<AppDbContext>(options=> options.UseSqlServer(connStr));
        }else{
            Console.WriteLine("--> Using in-mem db");
            builder.Services.AddDbContext<AppDbContext>(options=> options.UseInMemoryDatabase("in-memory"));
        }

        builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();
        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
        builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();
        builder.Services.AddGrpc();
    }
}