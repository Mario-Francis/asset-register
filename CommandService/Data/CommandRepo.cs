using CommandService.Models;

namespace CommandService.Data;
public class CommandRepo : ICommandRepo
{
    private readonly AppDbContext db;

    public CommandRepo(AppDbContext appDbContext)
    {
        this.db = appDbContext;
    }

    public void CreateCommand(int platformId, Command command)
    {
        if(command==null){
            throw new ArgumentNullException(nameof(command));
        }
        command.PlatformId = platformId;
        db.Commands.Add(command);
    }

    public void CreatePlatform(Platform platform)
    {
        if(platform==null){
            throw new ArgumentNullException(nameof(platform));
        }
        db.Platforms.Add(platform);
    }

    public bool ExternalPlatformExists(int externalPlatformId)
    {
        return db.Platforms.Any(p=>p.ExternalId == externalPlatformId);
    }

    public IEnumerable<Platform> GetAllPlatforms()
    {
        return db.Platforms.ToList();
    }

    public Command? GetCommand(int platformId, int commandId)
    {
        return db.Commands.Where(c=>c.PlatformId == platformId && c.Id == commandId).FirstOrDefault();
    }

    public IEnumerable<Command> GetCommandsForPlatform(int platformId)
    {
        return db.Commands.Where(c=>c.PlatformId==platformId).OrderBy(c=>c.Platform!.Name);
    }

    public bool PlatformExists(int platformId)
    {
        return db.Platforms.Any(p=>p.Id == platformId);
    }

    public bool SaveChanges()
    {
        return db.SaveChanges() > 0;
    }
}