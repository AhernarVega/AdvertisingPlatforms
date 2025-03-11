using AdvertisingPlatforms.Core.Domain.PersistenceContracts;
using AdvertisingPlatforms.Core.Services;
using AdvertisingPlatforms.Infrastructure.Persistence;
using AdvertisingPlatforms.Infrastructure.Storage;

namespace AdvertisingPlatforms;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddSingleton<IDataStorage, DataStorage>();
        builder.Services.AddScoped<IAdvertisingPlatformsService, AdvertisingPlatformsService>();
        builder.Services.AddScoped<IAdPlatformsReader, AdPlatformsReader>();
        builder.Services.AddLogging();
        
        builder.Services.AddControllers();
        
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();
        
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.MapControllers();

        app.Run();
    }
}