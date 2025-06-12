using KR3.PaymentService.Models;
using KR3.PaymentService.Services;
using KR3.Shared.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Payment Service API",
        Version = "v1",
        Description = "Service that manages user accounts, balances, and payment processing",
    });

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

builder.Services.AddDbContext<PaymentDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PaymentsDb")));

builder.Services.AddScoped<KR3.PaymentService.Services.IPaymentService, KR3.PaymentService.Services.PaymentService>();

var messageBusSettings = builder.Configuration.GetSection("MessageBus").Get<MessageBusSettings>()
    ?? new MessageBusSettings();

messageBusSettings.RequestedConnectionTimeout = TimeSpan.FromSeconds(5);
messageBusSettings.SocketReadTimeout = TimeSpan.FromSeconds(15);
messageBusSettings.SocketWriteTimeout = TimeSpan.FromSeconds(15);
builder.Services.AddSingleton(messageBusSettings);

builder.Services.AddHostedService<PaymentRequestConsumerService>();
builder.Services.AddHostedService<OutboxProcessorService>();

builder.Services.AddExceptionHandler(options =>
{
    options.ExceptionHandler = (httpContext) =>
    {
        var exceptionHandlerFeature = httpContext.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
        var exception = exceptionHandlerFeature?.Error;

        var logger = httpContext.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(exception, "Unhandled exception");
        return Task.CompletedTask;
    };
});

builder.Services.AddHealthChecks()
    .AddDbContextCheck<PaymentDbContext>();

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8080);
});

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Application built successfully");

if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "DockerDevelopment")
{
    logger.LogInformation("Initializing database...");
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<PaymentDbContext>();
        dbContext.Database.EnsureCreated();
    }
    logger.LogInformation("Database initialized successfully");
}

logger.LogInformation("Configuring HTTP request pipeline...");

app.UseExceptionHandler();

if (!app.Environment.IsProduction() && !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"))
    && !Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT").Contains("Docker"))
{
    app.UseHttpsRedirection();
}

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Payment Service API V1");
    options.RoutePrefix = "swagger";
});

app.UseRouting();
app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");

logger.LogInformation("HTTP request pipeline configured successfully");
logger.LogInformation("Starting application...");

try
{
    app.Run();
}
catch (Exception ex)
{
    logger.LogCritical(ex, "Application failed to start");
    throw;
}
