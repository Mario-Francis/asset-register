using CommandService.Models;
using CommandService.SyncDataServices.Grpc;

namespace CommandService.Data;

public static class PrepDb
{
    public static void PrepPopulation(IApplicationBuilder applicationBuilder){
        using(var serviceScope = applicationBuilder.ApplicationServices.CreateScope()){
            var grpcClient = serviceScope.ServiceProvider.GetRequiredService<IPlatformDataClient>();

            var platforms = grpcClient.ReturnAllPlatforms();
            SeedData(serviceScope.ServiceProvider.GetRequiredService<ICommandRepo>(), platforms);
        }
    }

    private static void SeedData(ICommandRepo repo, IEnumerable<Platform> platforms){
        Console.WriteLine("--> Seeding new platforms...");

        foreach(var p in platforms){
            if(!repo.ExternalPlatformExists(p.ExternalId)){
                repo.CreatePlatform(p);
            }
        }
        repo.SaveChanges();
    }
}