using Microsoft.AspNetCore.Mvc;
using Shopping.Api.Models;
using Shopping.Api.Services;
using System.Text;
using System.Text.Json;

namespace Shopping.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(MongoDbService mongoService, IHttpClientFactory httpClient, ILogger<UsersController> logger) : ControllerBase
{
    private readonly MongoDbService _mongoService = mongoService;
    private readonly HttpClient _notificationClient = httpClient.CreateClient("notificationClient");

    [HttpGet]
    public async Task<IActionResult> Get() =>
        Ok(await _mongoService.GetAsync());

    [HttpGet("{id:length(24)}")]
    public async Task<IActionResult> Get(string id)
    {
        var user = await _mongoService.GetAsync(id);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] User user)
    {
        if (user == null)
            return BadRequest("User data is required.");

        // Save user in MongoDB
        await _mongoService.CreateAsync(user);

        // Prepare notification payload
        var notification = new Notification
        {
            Title = "New User Created",
            Message = $"User {user.Name} was successfully created.",
            Email = user.Email,
            Id = user.Id
        };

        try
        {
            // Convert notification to JSON
            var json = JsonSerializer.Serialize(notification);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Send notification (adjust URL as needed)
            var response = await _notificationClient.PostAsync("/api/notification", content);

            if (!response.IsSuccessStatusCode)
            {
                // Optional: log or handle notification failure
                return StatusCode((int)response.StatusCode, "User created, but notification failed.");
            }
        }
        catch (Exception ex) 
        {
            logger.LogError(ex, "Internal server error.");
            return StatusCode(500);
        }       

        // Return CreatedAtAction response
        return CreatedAtAction(nameof(Get), new { id = user.Id }, user);
    }

    [HttpPut("{id:length(24)}")]
    public async Task<IActionResult> Put(string id, User updatedUser)
    {
        var user = await _mongoService.GetAsync(id);
        if (user is null) return NotFound();

        updatedUser.Id = user.Id;
        await _mongoService.UpdateAsync(id, updatedUser);
        return NoContent();
    }

    [HttpDelete("{id:length(24)}")]
    public async Task<IActionResult> Delete(string id)
    {
        var user = await _mongoService.GetAsync(id);
        if (user is null) return NotFound();

        await _mongoService.RemoveAsync(id);
        return NoContent();
    }
}
