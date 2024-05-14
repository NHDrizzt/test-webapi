
using WebApi.Services;

namespace TestApi.Test;

public class AccountServiceTests
{
    private readonly IAccountService _accountService;

    public AccountServiceTests()
    {
        _accountService = new AccountService();
        _accountService.ResetAccounts();
    }

    [Fact]
    public void GetBalance_ExistingAccount_ReturnsCorrectBalance()
    {
        _accountService.CreateOrUpdateAccount("123", 100);
        var balance = _accountService.GetBalance("123");
        Assert.Equal(100, balance);
    }

    [Fact]
    public void GetBalance_NonExistingAccount_ReturnsNull()
    {
        var balance = _accountService.GetBalance("999");
        Assert.Null(balance);
    }

    [Fact]
    public void CreateOrUpdateAccount_NewAccount_CreatesSuccessfully()
    {
        _accountService.CreateOrUpdateAccount("124", 50);
        Assert.Equal(50, _accountService.GetBalance("124"));
    }

    [Fact]
    public void CreateOrUpdateAccount_ExistingAccount_UpdatesSuccessfully()
    {
        _accountService.CreateOrUpdateAccount("123", 50);
        _accountService.CreateOrUpdateAccount("123", 50);
        Assert.Equal(100, _accountService.GetBalance("123"));
    }

    [Fact]
    public void Withdraw_EnoughBalance_WithdrawsSuccessfully()
    {
        _accountService.CreateOrUpdateAccount("123", 100);
        var result = _accountService.Withdraw("123", 50);
        Assert.True(result);
        Assert.Equal(50, _accountService.GetBalance("123"));
    }

    [Fact]
    public void Withdraw_NotEnoughBalance_FailsToWithdraw()
    {
        _accountService.CreateOrUpdateAccount("123", 100);
        var result = _accountService.Withdraw("123", 150);
        Assert.False(result);
        Assert.Equal(100, _accountService.GetBalance("123"));
    }

    [Fact]
    public void Transfer_ValidTransaction_TransfersSuccessfully()
    {
        _accountService.CreateOrUpdateAccount("123", 100); 
        _accountService.CreateOrUpdateAccount("124", 50);
        var result = _accountService.Transfer("123", "124", 50);
        Assert.True(result);
        Assert.Equal(50, _accountService.GetBalance("123"));
        Assert.Equal(100, _accountService.GetBalance("124"));
    }

    [Fact]
    public void Transfer_InsufficientFunds_FailsToTransfer()
    {
        _accountService.CreateOrUpdateAccount("123", 100);
        var result = _accountService.Transfer("123", "999", 150); 
        Assert.False(result);
        Assert.Equal(100, _accountService.GetBalance("123"));
        Assert.Null(_accountService.GetBalance("999")); 
    }
    
    [Fact]
    public void ResetAccounts_ClearsAllAccounts()
    {
        _accountService.ResetAccounts();
        Assert.Null(_accountService.GetBalance("123")); 
    }
}