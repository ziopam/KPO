using MediatR;
using MiniDZ2.Application.EventHandler;
using MiniDZ2.Application.Interfaces;
using MiniDZ2.Application.Services;
using MiniDZ2.Domain.Events;
using MiniDZ2.Infrastructure.Interfaces;
using MiniDZ2.Infrastructure.Repositories;
using System.Reflection;

namespace MiniDZ2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddMediatR(a => a.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                var baseDirectory = AppContext.BaseDirectory;

                var domainXml = Path.Combine(baseDirectory, "MiniDZ2.Domain.xml");
                var appXml = Path.Combine(baseDirectory, "MiniDZ2.Presentation.xml");

                options.IncludeXmlComments(domainXml);
                options.IncludeXmlComments(appXml);
            });

            builder.Services.AddTransient<INotificationHandler<AnimalMovedEvent>, AnimalMovedEventHandler>();
            builder.Services.AddTransient<INotificationHandler<FeedingTimeEvent>, FeedingTimeEventHandler>();

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            builder.Services.AddSingleton<IAnimalRepository, AnimalRepository>();
            builder.Services.AddSingleton<IEnclosureRepository, EnclosureRepository>();
            builder.Services.AddSingleton<IFeedingScheduleRepository, FeedingScheduleRepository>();

            builder.Services.AddScoped<IAnimalTransferService, AnimalTransferService>();
            builder.Services.AddScoped<FeedingOrganizationService>();
            builder.Services.AddScoped<ZooStatisticsService>();

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();
            app.MapControllers();
            app.Run();
        }
    }
}
