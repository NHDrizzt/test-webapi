namespace WebApi.Services;

public class AccountService : IAccountService
{
    private static Dictionary<string, decimal> accounts = new Dictionary<string, decimal>();
    
    public decimal? GetBalance(string accountId)
    {
        if(accounts.TryGetValue(accountId, out var balance))
        {
            return balance;
        }
        return null;
    }
    
}