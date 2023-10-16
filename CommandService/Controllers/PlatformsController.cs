using AutoMapper;
using CommandService.Data;
using CommandService.DTOs;
using CommandService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers;
[ApiController]
[Route("api/c/[controller]")]
public class PlatformsController : ControllerBase
{
    private readonly ICommandRepo commandRepo;
    private readonly IMapper mapper;

    public PlatformsController(ICommandRepo commandRepo, IMapper mapper)
    {
        this.commandRepo = commandRepo;
        this.mapper = mapper;
    }

    [HttpGet]
    public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms(){
        Console.WriteLine("--> Getting platforms from CommandService");
        var platformItems = commandRepo.GetAllPlatforms();
        return Ok(mapper.Map<IEnumerable<PlatformReadDto>>(platformItems));
    }

    [HttpPost]
    public ActionResult TestInboundConnection()
    {
        Console.WriteLine("--> Inbound POST # Command Service");

        return Ok("Inbound test from Platforms Controller");
    }
}