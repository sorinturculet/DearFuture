﻿using DearFuture.Repositories;
using DearFuture.Services;
using DearFuture.Views;
using DearFuture.ViewModels;
using Microsoft.Extensions.Logging;

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
        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "dearfuture.db");
        builder.Services.AddSingleton<ICapsuleRepository>(sp => new CapsuleRepository(dbPath));

        // Register Service Layer
        builder.Services.AddSingleton<CapsuleService>();

        // Register ViewModels
        builder.Services.AddSingleton<MainViewModel>();
        builder.Services.AddTransient<CreateCapsuleViewModel>(); // ✅ Register ViewModel

        // Register Views

        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddTransient<CreateCapsulePage>();
        builder.Services.AddTransient<ArchivedCapsulesPage>();

        return builder.Build();
    }
}

