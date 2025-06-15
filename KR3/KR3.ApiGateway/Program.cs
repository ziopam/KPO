using KR3.Shared.Models;
using Polly;
using Polly.Extensions.Http;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "API Gateway",
        Version = "v1",
        Description = "API Gateway for the e-commerce microservices system",
    });

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

builder.Services.AddHttpClient("PaymentService", client =>
{
    var paymentServiceUrl = builder.Configuration["ServiceUrls:PaymentService"];
    client.BaseAddress = new Uri(paymentServiceUrl ?? "http://paymentservice:8080");
    client.Timeout = TimeSpan.FromSeconds(30);
})
.AddPolicyHandler(HttpPolicyExtensions
    .HandleTransientHttpError()
    .WaitAndRetryAsync(
        3,
        retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
        onRetry: (outcome, timespan, retryAttempt, context) =>
        {
            var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();
            logger.LogWarning("Retrying connection to PaymentService: Attempt {RetryAttempt} after {TimeBetweenRetries}s",
                retryAttempt, timespan.TotalSeconds);
        }))
.AddPolicyHandler(HttpPolicyExtensions
    .HandleTransientHttpError()
    .CircuitBreakerAsync(
        handledEventsAllowedBeforeBreaking: 5,
        durationOfBreak: TimeSpan.FromSeconds(30),
        onBreak: (outcome, timespan) =>
        {
            var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();
            logger.LogWarning("Circuit breaker opened for PaymentService. Service will not be called for {BreakDuration}s", timespan.TotalSeconds);
        },
        onReset: () =>
        {
            var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Circuit breaker reset for PaymentService. Service calls will be allowed.");
        }));

builder.Services.AddHealthChecks()
    .AddUrlGroup(new Uri(builder.Configuration["ServiceUrls:PaymentService"] + "/health"),
        name: "payment-service",
        tags: new[] { "microservice" });

var messageBusSettings = builder.Configuration.GetSection("MessageBus").Get<MessageBusSettings>()
    ?? new MessageBusSettings();
builder.Services.AddSingleton(messageBusSettings);

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName.Contains("Docker"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsProduction() && !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"))
    && !Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT").Contains("Docker"))
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
