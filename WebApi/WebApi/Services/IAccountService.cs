namespace WebApi.Services;

public interface IAccountService
{
    decimal? GetBalance(string accountId);
    void CreateOrUpdateAccount(string accountId, decimal amount);
    void ResetAccounts();
    bool Withdraw(string accountId, decimal amount);
    bool Transfer(string origin, string destination, decimal amount);
}