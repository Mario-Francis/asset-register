using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.DTOs;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlatformsController:ControllerBase{
    private readonly IPlatformRepo platformRepo;
    private readonly IMapper mapper;
    private readonly ICommandDataClient commandDataClient;
    private readonly IMessageBusClient messageBusClient;

    public PlatformsController(
        IPlatformRepo platformRepo, 
        IMapper mapper,
        ICommandDataClient commandDataClient,
        IMessageBusClient messageBusClient)
    {
        this.platformRepo = platformRepo;
        this.mapper = mapper;
        this.commandDataClient = commandDataClient;
        this.messageBusClient = messageBusClient;
    }

    [HttpGet]
    public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms(){
        Console.WriteLine("--> Getting platforms...");
        var platforms = platformRepo.GetAllPlatforms();
        return Ok(mapper.Map<IEnumerable<PlatformReadDto>>(platforms));
    }

    [HttpGet("{id:int}", Name ="GetPlatformById")]
    public ActionResult<PlatformReadDto> GetPlatformById(int id){
        var platformItem = platformRepo.GetPlatformById(id);
        if(platformItem!=null){
            return Ok(mapper.Map<PlatformReadDto>(platformItem));
        }
        return NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto platformDto){
        var platform = mapper.Map<Platform>(platformDto);
        platformRepo.CreatePlatform(platform);
        platformRepo.SaveChanges();

        var platformReadDto = mapper.Map<PlatformReadDto>(platform);
        // send sync message
        try{
            await commandDataClient.SendPlatformToCommand(platformReadDto);
        }catch(Exception ex){
            Console.WriteLine($"--> Could not send synchronously: {ex.Message}");
        }
        // send async message
         try{
            var publishedDto = mapper.Map<PlatformPublishedDto>(platformReadDto);
            publishedDto.Event = "Platform_Published";
            messageBusClient.PublishNewPlatform(publishedDto);
        }catch(Exception ex){
            Console.WriteLine($"--> Could not send asynchronously: {ex.Message}");
        }
        return CreatedAtRoute(nameof(GetPlatformById), new {id=platform.Id}, platformReadDto);
    }

}