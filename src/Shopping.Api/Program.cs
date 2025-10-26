using Shopping.Api.Services;
using Shopping.Api.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Bind configuration
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDB"));

builder.Services.AddHttpClient<INotificationClient, NotificationClient>(client =>
{
    var baseUrl = builder.Configuration["NotificationService:BaseUrl"];
    client.BaseAddress = new Uri(baseUrl!);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
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
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
