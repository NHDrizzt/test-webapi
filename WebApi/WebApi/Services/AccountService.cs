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
    
    public void CreateOrUpdateAccount(string accountId, decimal amount)
    {
        if(accounts.ContainsKey(accountId))
        {
            accounts[accountId] += amount;
        }
        else
        {
            accounts.Add(accountId, amount);
        }
    }
    
    public bool Withdraw(string accountId, decimal amount)
    {
        if(accounts.ContainsKey(accountId) && accounts[accountId] >= amount)
        {
            accounts[accountId] -= amount;
            return true;
        }
        return false;
    }
    
    public bool Transfer(string origin, string destination, decimal amount)
    {
        if(Withdraw(origin, amount))
        {
            CreateOrUpdateAccount(destination, amount);
            return true;
        }
        return false;
    }
    
    public void ResetAccounts()
    {
        accounts.Clear();
    }
}