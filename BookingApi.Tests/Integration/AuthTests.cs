using System.Net;
using System.Net.Http.Json;
using BookingApi.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BookingApi.Tests.Integration;

public class AuthTests :
    IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public AuthTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<IDbContextOptionsConfiguration<BookingDbContext>>();
                services.RemoveAll<DbContextOptions<BookingDbContext>>();
                services.AddDbContext<BookingDbContext>(options =>
                    options.UseInMemoryDatabase("BookingApiTests"));
            });
        });
    }

    [Fact]
    public async Task Register_ReturnsSuccess()
    {
        // Arrange
        var client = _factory.CreateClient();

        var request = new
        {
            username = $"test_{Guid.NewGuid().ToString("N")[..20]}",
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