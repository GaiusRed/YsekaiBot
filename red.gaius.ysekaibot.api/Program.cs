using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using Microsoft.Extensions.Logging.ApplicationInsights;
using Red.Gaius.YsekaiBot.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1",  new Microsoft.OpenApi.Models.OpenApiInfo()
    {
        Title = "Ysekai Bot API",
        Version = "v1",
        Description = "Isekai Discord Bot. " +
            "Click <a href=\"auth?state=/api\">here</a> for Authentication.",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact()
        {
            Email = "erik@gaius.red",
            Name = "Erik Gaius Capistrano",
        },
    });
    var filePath = Path.Combine(System.AppContext.BaseDirectory, "red.gaius.ysekaibot.api.xml");
    c.IncludeXmlComments(filePath);
});

var options = new ApplicationInsightsServiceOptions
{
    ConnectionString = builder.Configuration.GetConnectionString("ApplicationInsights"),
};

builder.Services.AddApplicationInsightsTelemetry(options: options);

builder.Host.ConfigureLogging((context, builder) =>
 {
     builder.AddApplicationInsights();

     // Capture all log-level entries from Program
     builder.AddFilter<ApplicationInsightsLoggerProvider>(
        typeof(Program).FullName, LogLevel.Trace);
});

builder.Services.AddSingleton<Database>();

builder.Services.AddHttpClient("DiscordApi", (client) =>
{
    client.BaseAddress = new Uri("https://discordapp.com/api/");
});

builder.Services.AddSingleton<DiscordAuth>();

builder.Services.AddMemoryCache();

// Build and run application
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    TelemetryDebugWriter.IsTracingDisabled = true;
}

app.UseSwagger(c =>
{
    c.RouteTemplate = "api/{documentName}/swagger.json";
});

app.UseSwaggerUI(c =>
{
    c.RoutePrefix = "api";
    c.SwaggerEndpoint("v1/swagger.json", "Ysekai Bot API v1");
});

app.UseHttpsRedirection();

app.UseDefaultFiles();

app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();
