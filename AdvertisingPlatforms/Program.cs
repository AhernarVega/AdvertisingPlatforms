using AdvertisingPlatforms.Core.Domain.PersistenceContracts;
using AdvertisingPlatforms.Core.Services;
using AdvertisingPlatforms.Core.Services.Abstractions;
using AdvertisingPlatforms.Infrastructure.Persistence;
using AdvertisingPlatforms.Infrastructure.Storage;
using AdvertisingPlatforms.Presentation.Middlewares;
using AdvertisingPlatforms.Presentation.Validators;
using FluentValidation;

namespace AdvertisingPlatforms;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddSingleton<IDataStorage, DataStorage>();
        builder.Services.AddScoped<IAdvertisingPlatformsService, AdvertisingPlatformsService>();
        builder.Services.AddScoped<IAdPlatformsReader, AdPlatformsReader>();
        builder.Services.AddLogging(options =>
        {
            options.AddConsole();
            options.AddDebug();
        });

        builder.Services.AddControllers();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddScoped<IValidator<string>, LocationRequestValidator>();
        
        var app = builder.Build();
        
        app.UseHttpsRedirection();
        
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseMiddleware<CustomExceptionMiddleware>();

        app.MapControllers();

        app.Run();
    }
}