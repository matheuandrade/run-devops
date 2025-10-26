using Shopping.Api.Models;
using System.Text;
using System.Text.Json;

namespace Shopping.Api.Services;

public interface INotificationClient
{
    Task<HttpResponseMessage> SendNotificationAsync(Notification notification);
}

public class NotificationClient : INotificationClient
{
    private readonly HttpClient _httpClient;

    public NotificationClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<HttpResponseMessage> SendNotificationAsync(Notification notification)
    {
        var json = JsonSerializer.Serialize(notification);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        return await _httpClient.PostAsync("notifications", content);
    }
}