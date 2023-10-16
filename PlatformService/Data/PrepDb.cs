using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data;
public static class PrepDb
{
    public static void PrepPopulation(IApplicationBuilder app, bool isProd)
    {
        using (var scope = app.ApplicationServices.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            SeedData(db, isProd);
        }
    }

    private static void SeedData(AppDbContext db, bool isProd)
    {
        if (isProd)
        {
            Console.WriteLine("--> Attemting to apply migrations");
            try
            {
                db.Database.Migrate();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not run migrations: {ex.Message}");
            }

        }
        if (!db.Platforms.Any())
        {
            Console.WriteLine("--> Seeding data!");
            db.Platforms.AddRange(
                new Platform { Name = "Dot Net", Publisher = "Microsoft", Cost = "Free" },
                new Platform { Name = "Sql Server Express", Publisher = "Microsoft", Cost = "Free" },
                new Platform { Name = "Kubernetes", Publisher = "Cloud Native Computing Foundation", Cost = "Free" }
            );
            db.SaveChanges();
        }
        else
        {
            Console.WriteLine("--> We already have data!");
        }
    }
}