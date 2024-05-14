using System.Net;

namespace TestApi.Test;

using Microsoft.AspNetCore.Hosting;

using Microsoft.AspNetCore.Mvc.Testing;

using WebApi.Controllers;

public class IntegrationTest: IClassFixture<WebApplicationFactory<Program>>
{
    public HttpClient _clientTest;
    
    public IntegrationTest(WebApplicationFactory<Program> factory)
    {
        _clientTest = factory.CreateClient();
    }
    
    [Theory(DisplayName = "Testing if route /balance returns 404 when account does not exist")]
    [InlineData("/balance?accountId=123")]
    public async Task TestBalance(string url)
    {
        var response = await _clientTest.GetAsync(url);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
}