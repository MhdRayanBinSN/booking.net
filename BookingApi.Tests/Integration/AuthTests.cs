using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BookingApi.Tests.Integration;

public class AuthTests :
    IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public AuthTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Register_ReturnsSuccess()
    {
        // Arrange
        var client = _factory.CreateClient();

        var request = new
        {
            username = $"test_{Guid.NewGuid()}",
            email = $"{Guid.NewGuid()}@example.com",
            password = "Password@123"
        };

        // Act
        var response = await client.PostAsJsonAsync(
            "/api/Auth/register",
            request);

        // Assert
        Assert.True(
            response.StatusCode == HttpStatusCode.OK ||
            response.StatusCode == HttpStatusCode.Conflict);
    }
}