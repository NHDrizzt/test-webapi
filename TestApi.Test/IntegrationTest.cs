using System.Net;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using WebApi.Services;

namespace TestApi.Test;

using Microsoft.AspNetCore.Mvc.Testing;

using WebApi.Controllers;

public class IntegrationTest: IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _clientTest;
    
    public IntegrationTest(WebApplicationFactory<Program> factory)
    {
        _clientTest = factory.CreateClient();
        var scopeFactory = factory.Services.GetService<IServiceScopeFactory>(); // Allows to create a new scope to resolve services of the app
        using var scope = scopeFactory.CreateScope(); // Allows to instantiate  any services that were registered with a scoped lifetime
        var service = scope.ServiceProvider.GetService<IAccountService>(); // Resolves the service
        service.ResetAccounts();
    }
    
    [Theory(DisplayName = "Testing if route /balance returns 404 when account does not exist")]
    [InlineData("/balance?accountId=123")]
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

    
}