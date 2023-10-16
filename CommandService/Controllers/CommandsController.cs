using AutoMapper;
using CommandService.Data;
using CommandService.DTOs;
using CommandService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers;
[Route("api/c/platforms/{platformId}/[controller]")]
[ApiController]
public class CommandsController : ControllerBase
{
    private readonly ICommandRepo commandRepo;
    private readonly IMapper mapper;

    public CommandsController(ICommandRepo commandRepo, IMapper mapper)
    {
        this.commandRepo = commandRepo;
        this.mapper = mapper;
    }

    [HttpGet]
    public ActionResult<IEnumerable<CommandReadDto>> GetCommandsForPlatform(int platformId)
    {
        Console.WriteLine($"--> Hit GetCommandsForPlatforms: {platformId}");
        if (!commandRepo.PlatformExists(platformId))
        {
            return NotFound();
        }
        var commandItems = commandRepo.GetCommandsForPlatform(platformId);
        return Ok(mapper.Map<IEnumerable<CommandReadDto>>(commandItems));
    }

    [HttpGet("{commandId}", Name = "GetCommand")]
    public ActionResult<CommandReadDto> GetCommand(int platformId, int commandId)
    {
        Console.WriteLine($"--> Hit GetCommand: platformId={platformId}, commandId={commandId}");
        if (!commandRepo.PlatformExists(platformId))
        {
            return NotFound();
        }
        var command = commandRepo.GetCommand(platformId, commandId);
        if (command == null)
        {
            return NotFound();
        }
        return Ok(mapper.Map<CommandReadDto>(command));
    }

    [HttpPost]
    public ActionResult<CommandReadDto> CreateCommand(int platformId, CommandCreateDto model){
        Console.WriteLine($"--> Hit CreateCommand: platformId={platformId}");
        if (!commandRepo.PlatformExists(platformId))
        {
            return NotFound();
        }
        var command = mapper.Map<Command>(model);
        commandRepo.CreateCommand(platformId, command);
        commandRepo.SaveChanges();

        var commandReadDto = mapper.Map<CommandReadDto>(command);
        return CreatedAtRoute(nameof(GetCommand), new {platformId=command.PlatformId, commandId=command.Id}, commandReadDto);
    }
}