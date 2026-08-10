using Microsoft.AspNetCore.Mvc;

namespace GestaodeReembolsos.Controllers;

[ApiController]
[Route("api/v1/status")]
public class StatusController : ControllerBase
{
    [HttpGet("")]
    public IActionResult Get()
    {
        return Ok();
    }
}