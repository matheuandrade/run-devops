using Microsoft.AspNetCore.Mvc;

namespace Notification.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Post(Models.Notification notification)
    {
        return Ok(notification);
    }
}
