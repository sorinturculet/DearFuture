using DearFuture.Repositories;
using CommunityToolkit.Mvvm.Input;
using DearFuture.Services;
using DearFuture.Views;
using DearFuture.ViewModels;
using Microsoft.Extensions.Logging;
using ViewModels;

namespace DearFuture;
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });
        
        // Register SQLite Repository
        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "capsules.db");
        builder.Services.AddSingleton<ICapsuleRepository>(s => 
            new CapsuleRepository(dbPath));

        // Register Geolocation and Map Services.
        builder.Services.AddSingleton<IGeolocation>(Geolocation.Default);
        builder.Services.AddSingleton<IMap>(Map.Default);
        // Register Service Layer
        builder.Services.AddSingleton<CapsuleService>();

        // Register ViewModels
        builder.Services.AddSingleton<MainViewModel>();
        builder.Services.AddTransient<CreateCapsuleViewModel>();
        builder.Services.AddTransient<ArchivedCapsulesViewModel>();
        builder.Services.AddTransient<TrashCapsulesViewModel>();

        // Register Views

        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddTransient<CreateCapsulePage>();
        builder.Services.AddTransient<ArchivedCapsulesPage>();
        builder.Services.AddTransient<TrashCapsulesPage>();

        return builder.Build();
    }
}

