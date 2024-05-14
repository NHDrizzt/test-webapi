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
    
    [Theory(DisplayName = "Testing route /balance")]
    [InlineData("/balance")]
    public async Task TestBalance(string url)
    {
        var response = await _clientTest.GetAsync(url);
        response.EnsureSuccessStatusCode();
    }
}