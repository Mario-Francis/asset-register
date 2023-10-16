using System.Net;
using AutoMapper;
using CommandService.Models;
using Grpc.Core;
using Grpc.Net.Client;
using PlatformService;

namespace CommandService.SyncDataServices.Grpc;

public class PlatformDataClient : IPlatformDataClient
{
    private readonly IConfiguration config;
    private readonly IMapper mapper;

    public PlatformDataClient(IConfiguration config, IMapper mapper)
    {
        this.config = config;
        this.mapper = mapper;
    }

    public IEnumerable<Platform> ReturnAllPlatforms()
    {
        Console.WriteLine($"--> Calling Grpc service {config["GrpcPlatform"]}");


        var channel = GrpcChannel.ForAddress(config["GrpcPlatform"]!);
        var client = new GrpcPlatform.GrpcPlatformClient(channel);
        var request = new GetAllRequest();

        try
        {
            var reply = client.GetAllPlatforms(request);
            return mapper.Map<IEnumerable<Platform>>(reply.Platform);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Could not call Grpc server: {ex.Message}");
            return null!;
        }
    }
}