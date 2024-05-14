using Microsoft.AspNetCore.Mvc;
using WebApi.Dto;
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
    
    [HttpPost("event")]
    public IActionResult Event([FromBody] EventDto transaction)
    {
        switch (transaction.Type)
        {
            case "deposit":
                if(transaction.Destination == null)
                    return BadRequest();
                _accountService.CreateOrUpdateAccount(transaction.Destination, transaction.Amount);
                var newBalance = _accountService.GetBalance(transaction.Destination);
                return Created("", new { destination = new { id = transaction.Destination, balance = newBalance } });
            case "withdraw":
                if(transaction.Origin == null)
                    return BadRequest();
                if (_accountService.Withdraw(transaction.Origin, transaction.Amount))
                {
                    newBalance = _accountService.GetBalance(transaction.Origin);
                    return Created("", new { origin = new { id = transaction.Origin, balance = newBalance } });
                }
                return NotFound(0);
            case "transfer":
                if(transaction.Origin == null || transaction.Destination == null)
                    return BadRequest();
                if (_accountService.Transfer(transaction.Origin, transaction.Destination, transaction.Amount))
                {
                    var originBalance = _accountService.GetBalance(transaction.Origin);
                    var destinationBalance = _accountService.GetBalance(transaction.Destination);
                    return Created("", new { origin = new { id = transaction.Origin, balance = originBalance }, destination = new { id = transaction.Destination, balance = destinationBalance } });
                }
                return NotFound(0);
            default:
                return BadRequest();
        }
    }
    
    [HttpPost("reset")]
    public IActionResult Reset()
    {
        _accountService.ResetAccounts();
        return Ok();
    }
    
}