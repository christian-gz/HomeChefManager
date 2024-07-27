using System;
using System.IO;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using HomeChefManager.Configuration;
using HomeChefManager.DataAccess;
using HomeChefManager.Services;
using HomeChefManager.ViewModels;
using HomeChefManager.ViewModels.Ingredients;
using HomeChefManager.ViewModels.Recipes;
using HomeChefManager.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HomeChefManager;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            IServiceProvider serviceProvider = BuildServiceProvider();

            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(
                    serviceProvider.GetRequiredService<RecipeListViewModel>(),
                    serviceProvider.GetRequiredService<IngredientListViewModel>()
                ),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private IServiceProvider BuildServiceProvider()
    {
        IServiceCollection collection = new ServiceCollection();

        IConfig config;
        try
        {
            config = LoadConfig();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            config = new Config();
        }

        collection.AddSingleton<IConfig>(config);
        collection.AddSingleton<IRecipeRepository, RecipeRepository>();
        collection.AddSingleton<IIngredientRepository, IngredientRepository>();
        collection.AddSingleton<IRecipeService, RecipeService>();
        collection.AddSingleton<IIngredientService, IngredientService>();
        collection.AddSingleton<ICookRecipeViewModel, CookRecipeViewModel>();
        collection.AddSingleton<RecipeListViewModel>();
        collection.AddSingleton<IngredientListViewModel>();

        return collection.BuildServiceProvider();
    }

    private Config LoadConfig()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        Config config = builder.Get<Config>() ?? throw new InvalidOperationException("Configuration cannot be loaded.");

        if (config.ConnectionStrings == null || string.IsNullOrEmpty(config.ConnectionStrings.Default))
        {
            throw new InvalidOperationException("The required 'Default' connection string is missing or empty.");
        }

        return config;
    }
}