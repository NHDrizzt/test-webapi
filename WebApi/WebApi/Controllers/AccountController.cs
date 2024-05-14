using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("")]
public class AccountController : ControllerBase
{

    [HttpGet("/balance")]
    public IActionResult Get()
    {
        return Ok("Hello World!");
    }
    
}