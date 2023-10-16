using System.Text;
using System.Text.Json;
using PlatformService.DTOs;

namespace PlatformService.SyncDataServices.Http;
public class HttpCommandDataClient : ICommandDataClient
{
    private readonly HttpClient httpClient;
    private readonly IConfiguration configuration;

    public HttpCommandDataClient(HttpClient httpClient, IConfiguration configuration)
    {
        this.httpClient = httpClient;
        this.configuration = configuration;
    }

    public async Task SendPlatformToCommand(PlatformReadDto platform)
    {
        var httpContent = new StringContent(JsonSerializer.Serialize(platform), Encoding.UTF8, "application/json");

        var url = configuration["CommandServiceBaseUrl"];
        var response = await httpClient.PostAsync(url, httpContent);
        if(response.IsSuccessStatusCode){
            Console.WriteLine("--> Sync POST to Command Service was ok!");
        }else{
            Console.WriteLine("--> Sync POST to Command Service was NOT ok!");
        }

    }
}