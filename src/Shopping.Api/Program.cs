using Shopping.Api.Services;
using Shopping.Api.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Bind configuration
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDB"));

builder.Services.AddHttpClient("notificationClient", (sp, client) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var baseUrl = config["NotificationService:BaseUrl"] ?? throw new InvalidOperationException("Missing NotificationService:BaseUrl");

    client.BaseAddress = new Uri(baseUrl);
    client.DefaultRequestHeaders.Accept.Add(
        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
});

// Register service
builder.Services.AddSingleton<MongoDbService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var mongoService = scope.ServiceProvider.GetRequiredService<MongoDbService>();
    await mongoService.SeedAsync();
}

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Shopping API v1");
    options.RoutePrefix = "swagger"; // garante acesso em /swagger/index.html
});

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
