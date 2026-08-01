using Microsoft.AspNetCore.Mvc;

namespace BiometricClockingAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AttendanceController : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(new { message = "Attendance endpoint ready" });
    }
}
