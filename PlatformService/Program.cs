using PlatformService;
using PlatformService.Data;
using PlatformService.SyncDataServices.Grpc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
ServiceRegistry.RegisterServices(builder);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
Console.WriteLine($"--> Command Service endpoint {builder.Configuration["CommandServiceBaseUrl"]}");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapGrpcService<GrpcPlatformService>();

app.MapGet("/protos/platforms.proto", async context=>{
    await context.Response.WriteAsync(File.ReadAllText("Protos/platforms.proto"));
});
// prep database
PrepDb.PrepPopulation(app, app.Environment.IsProduction());

app.Run();
