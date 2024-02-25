using System.Windows;
using CourseLearning.Desktop.Services;
using CourseLearning.Desktop.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace CourseLearning.Desktop;

public partial class App : Application
{
    private readonly IServiceProvider _serviceProvider;
    public App()
    {
        _serviceProvider = new ServiceCollection()
            .AddTransient<HelpViewModel>()
            .AddSingleton<SideBarViewModel>()
            .AddSingleton<MainWindow>()
            .AddSingleton<MainViewModel>()
            .AddSingleton<NavigationService>()
            .BuildServiceProvider();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        MainWindow = _serviceProvider.GetService<MainWindow>();
        MainWindow.DataContext = _serviceProvider.GetService<MainViewModel>();
        MainWindow.Show();
        base.OnStartup(e);
    }
}