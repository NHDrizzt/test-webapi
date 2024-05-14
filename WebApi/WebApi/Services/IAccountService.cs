namespace WebApi.Services;

public interface IAccountService
{
    decimal? GetBalance(string accountId);
}