using MediatR;
using MiniDZ2.Application.EventHandler;
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
            builder.Services.AddSwaggerGen();

            builder.Services.AddTransient<INotificationHandler<AnimalMovedEvent>, AnimalMovedEventHandler>();
            builder.Services.AddTransient<INotificationHandler<FeedingTimeEvent>, FeedingTimeEventHandler>();

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            builder.Services.AddScoped<IAnimalRepository, AnimalRepository>();
            builder.Services.AddScoped<IEnclosureRepository, EnclosureRepository>();
            builder.Services.AddScoped<IFeedingScheduleRepository, FeedingScheduleRepository>();

            builder.Services.AddScoped<AnimalTransferService>();
            builder.Services.AddScoped<FeedingOrganizationService>();
            builder.Services.AddScoped<ZooStatisticsService>();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.MapControllers();
            app.Run();
        }
    }
}
