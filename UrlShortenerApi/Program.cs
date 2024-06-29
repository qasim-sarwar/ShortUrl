using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Amazon.Lambda.AspNetCoreServer;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapPost("/shorten", (ShortenUrlRequest request) =>
{
    var shortenedUrl = $"https://short.url/{Guid.NewGuid().ToString().Substring(0, 6)}";

    if (!Uri.TryCreate(request.Url, UriKind.Absolute, out _))
    {
        return Results.BadRequest("The specified URL is invalid.");
    }

    return Results.Ok(new ShortenUrlResponse { ShortenedUrl = shortenedUrl });
});

app.Run();

public class ShortenUrlRequest
{
    [JsonPropertyName("url")]
    public string Url { get; set; }
}

public class ShortenUrlResponse
{
    [JsonPropertyName("shortenedUrl")]
    public string ShortenedUrl { get; set; }
}

public class LambdaEntryPoint : APIGatewayHttpApiV2ProxyFunction
{
    protected override void Init(IWebHostBuilder builder)
    {
        builder.UseStartup<Startup>();
    }
}

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRouting();
        app.UseEndpoints(endpoints => endpoints.MapControllers());
    }
}
