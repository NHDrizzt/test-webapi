using System.Net;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using WebApi.Services;

namespace TestApi.Test;

using Microsoft.AspNetCore.Mvc.Testing;

public class AccountIntegrationTest: IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _clientTest;
    
    public AccountIntegrationTest(WebApplicationFactory<Program> factory)
    {
        _clientTest = factory.CreateClient();
        var scopeFactory = factory.Services.GetService<IServiceScopeFactory>(); // Allows to create a new scope to resolve services of the app
        using var scope = scopeFactory.CreateScope(); // Allows to instantiate  any services that were registered with a scoped lifetime
        var service = scope.ServiceProvider.GetService<IAccountService>(); // Resolves the service
        service.ResetAccounts();
    }
    
    [Theory(DisplayName = "Testing if route /balance returns 404 when account does not exist")]
    [InlineData("/balance?account_Id=123")]
    public async Task TestBalance(string url)
    {
        var response = await _clientTest.GetAsync(url);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    
    [Theory(DisplayName = "Testing if route /event returns 201 with the correct body when deposit is successful")]
    [InlineData("/event", "{\"type\":\"deposit\",\"destination\":\"100\",\"amount\":10}")]
    public async Task Test2(string url, string body)
    {
        var content = new StringContent(body, Encoding.UTF8, "application/json");
        var response = await _clientTest.PostAsync(url, content);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("{\"destination\":{\"id\":\"100\",\"balance\":10}}", await response.Content.ReadAsStringAsync());
    }
    
    [Theory(DisplayName = "Testing if route /event returns 400 when deposit is missing destination")]
    [InlineData("/event", "{\"type\":\"deposit\",\"amount\":10}")]
    public async Task Test3(string url, string body)
    {
        var content = new StringContent(body, Encoding.UTF8, "application/json");
        var response = await _clientTest.PostAsync(url, content);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    
    [Theory(DisplayName = "Testing if route /event returns 201 with the amount value updated correctly after two deposits")]
    [InlineData("/event", "{\"type\":\"deposit\",\"destination\":\"100\",\"amount\":10}")]
    public async Task Test4(string url, string body)
    {
        var content = new StringContent(body, Encoding.UTF8, "application/json");

        // First deposit
        var response = await _clientTest.PostAsync(url, content);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("{\"destination\":{\"id\":\"100\",\"balance\":10}}", await response.Content.ReadAsStringAsync());

        // Second deposit
        response = await _clientTest.PostAsync(url, content);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("{\"destination\":{\"id\":\"100\",\"balance\":20}}", await response.Content.ReadAsStringAsync());
    }
    
    [Theory(DisplayName = "Testing if route /event returns 201 with the correct body when withdraw is successful")]
    [InlineData("/event", "{\"type\":\"deposit\",\"destination\":\"100\",\"amount\":10}", 
        "{\"type\":\"withdraw\",\"origin\":\"100\",\"amount\":5}")]
    public async Task Test5(string url, string depositBody, string withdrawBody)
    {
        // Deposit
        var depositContent = new StringContent(depositBody, Encoding.UTF8, "application/json");
        var response = await _clientTest.PostAsync(url, depositContent);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("{\"destination\":{\"id\":\"100\",\"balance\":10}}", await response.Content.ReadAsStringAsync());

        // Withdraw
        var withdrawContent = new StringContent(withdrawBody, Encoding.UTF8, "application/json");
        response = await _clientTest.PostAsync(url, withdrawContent);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("{\"origin\":{\"id\":\"100\",\"balance\":5}}", await response.Content.ReadAsStringAsync());
    }
    
    [Theory(DisplayName = "Testing if route /event returns 404 when withdraw is unsuccessful")]
    [InlineData("/event", "{\"type\":\"withdraw\",\"origin\":\"100\",\"amount\":5}")]
    public async Task Test6(string url, string body)
    {
        var content = new StringContent(body, Encoding.UTF8, "application/json");
        var response = await _clientTest.PostAsync(url, content);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Theory(DisplayName = "Testing if route /event returns 400 when withdraw is missing origin")]
    [InlineData("/event", "{\"type\":\"withdraw\",\"amount\":5}")]
    public async Task Test7(string url, string body)
    {
        var content = new StringContent(body, Encoding.UTF8, "application/json");
        var response = await _clientTest.PostAsync(url, content);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory(DisplayName = "Testing if route /event returns 201 with the correct body when transfer is successful")]
    [InlineData("/event", "{\"type\":\"deposit\",\"destination\":\"100\",\"amount\":10}",
        "{\"type\":\"transfer\",\"origin\":\"100\",\"destination\":\"200\",\"amount\":5}")]
    public async Task Test8(string url, string depositBody, string transferBody)
    {
        // Deposit
        var depositContent = new StringContent(depositBody, Encoding.UTF8, "application/json");
        var response = await _clientTest.PostAsync(url, depositContent);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("{\"destination\":{\"id\":\"100\",\"balance\":10}}", await response.Content.ReadAsStringAsync());

        // Transfer
        var transferContent = new StringContent(transferBody, Encoding.UTF8, "application/json");
        response = await _clientTest.PostAsync(url, transferContent);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("{\"origin\":{\"id\":\"100\",\"balance\":5},\"destination\":{\"id\":\"200\",\"balance\":5}}", await response.Content.ReadAsStringAsync());
    }
    
    [Theory(DisplayName = "Testing if route /event returns 404 when transfer is unsuccessful")]
    [InlineData("/event", "{\"type\":\"transfer\",\"origin\":\"100\",\"destination\":\"200\",\"amount\":5}")]
    public async Task Test9(string url, string body)
    {
        var content = new StringContent(body, Encoding.UTF8, "application/json");
        var response = await _clientTest.PostAsync(url, content);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Theory(DisplayName = "Testing if route /event returns 400 when transfer is missing origin")]
    [InlineData("/event", "{\"type\":\"transfer\",\"destination\":\"200\",\"amount\":5}")]
    public async Task Test10(string url, string body)
    {
        var content = new StringContent(body, Encoding.UTF8, "application/json");
        var response = await _clientTest.PostAsync(url, content);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Theory(DisplayName = "Testing if route /event returns 400 when transfer is missing destination")]
    [InlineData("/event", "{\"type\":\"transfer\",\"origin\":\"100\",\"amount\":5}")]
    public async Task Test11(string url, string body)
    {
        var content = new StringContent(body, Encoding.UTF8, "application/json");
        var response = await _clientTest.PostAsync(url, content);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Theory(DisplayName = "Testing if route /reset returns 200")]
    [InlineData("/reset")]
    public async Task Test12(string url)
    {
        var response = await _clientTest.PostAsync(url, null);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}