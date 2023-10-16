using System.Text.Json;
using AutoMapper;
using CommandService.Data;
using CommandService.DTOs;
using CommandService.Models;

namespace CommandService.EventProcessing;
public class EventProcessor : IEventProcessor
{
    private readonly IServiceScopeFactory scopeFactory;
    private readonly IMapper mapper;

    public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
    {
        this.scopeFactory = scopeFactory;
        this.mapper = mapper;
    }

    public void ProcessEvent(string message)
    {
        var eventType = DetermineEvent(message);
        switch (eventType)
        {
            case EventType.PlatformPublished:
                // todo
                AddPlatform(message);
                break;
            default:
                break;
        }
    }

    private EventType DetermineEvent(string notificationMessage)
    {
        Console.WriteLine("--> Determining event...");
        var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);
        return eventType!.Event switch
        {
            "Platform_Published" => EventType.PlatformPublished,
            _ => EventType.Undetermined
        };
    }

    private void AddPlatform(string platformPublishedMessage)
    {
        using (var scope = scopeFactory.CreateScope())
        {
            var commandRepo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();
            var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMessage);
            try
            {
                var platform = mapper.Map<Platform>(platformPublishedDto);
                if (!commandRepo.ExternalPlatformExists(platform.ExternalId))
                {
                    commandRepo.CreatePlatform(platform);
                    commandRepo.SaveChanges();
                }
                else
                {
                    Console.WriteLine("--> Platform already exist!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not add platform to db: {ex.Message}");
            }
        }
    }
}
enum EventType
{
    PlatformPublished,
    Undetermined
}