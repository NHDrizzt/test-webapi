using Microsoft.AspNetCore.Mvc;
using WebApi.Services;

namespace WebApi.Controllers;

[ApiController]
[Route("")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;
    
    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpGet("balance")]
    public IActionResult Get([FromQuery] string accountId)
    {
        var balance = _accountService.GetBalance(accountId);
        if (balance.HasValue)
        {
            return Ok(balance.Value);
        }
        return NotFound(0);
    }
    
}