using PlatformService.Models;

namespace PlatformService.Data;
public class PlatformRepo : IPlatformRepo
{
    private readonly AppDbContext db;

    public PlatformRepo(AppDbContext db)
    {
        this.db = db;
    }

    public void CreatePlatform(Platform platform)
    {
        if(platform is null){
            throw new ArgumentNullException(nameof(platform));
        }

        db.Platforms.Add(platform);
    }

    public IEnumerable<Platform> GetAllPlatforms()
    {
        return db.Platforms.AsEnumerable();
    }

    public Platform? GetPlatformById(int id)
    {
        return db.Platforms.FirstOrDefault(p=>p.Id == id);
    }

    public bool SaveChanges()
    {
        return db.SaveChanges() > 0;
    }
}
