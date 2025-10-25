using Microsoft.AspNetCore.Mvc;
using Shopping.Api.Model;
using Shopping.Api.Services;

namespace Shopping.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly MongoDbService _mongoService;

    public UsersController(MongoDbService mongoService)
    {
        _mongoService = mongoService;
    }

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
    public async Task<IActionResult> Post(User user)
    {
        await _mongoService.CreateAsync(user);
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
