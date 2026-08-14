using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaodeReembolsos.Controllers;

[ApiController]
[Route("api/v1/status")]
public class StatusController : ControllerBase
{
    [Authorize]
    [HttpGet("")]
    public IActionResult Get()
    {
        return Ok();
    }
}