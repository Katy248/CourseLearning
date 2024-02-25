using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CourseLearning.Desktop.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CourseLearning.Desktop.ViewModels;
public partial class SideBarViewModel : ObservableObject
{
    private readonly NavigationService _navigation;
    private readonly IServiceProvider _serviceProvider;

    public SideBarViewModel(NavigationService navigation, IServiceProvider serviceProvider)
    {
        _navigation = navigation;
        _serviceProvider = serviceProvider;
    }

    [RelayCommand]
    public void ToCreatePage()
    {
        // Navigate to the creating courses page
        //contentFrame.Navigate(new CreatingCoursesPage());
    }

    [RelayCommand]
    public void ToReadPage()
    {
        _navigation.NavigateTo(_serviceProvider.GetRequiredService<OpenCourseReadViewModel>());
    }
    [RelayCommand]
    public void ToHelpPage()
    {
        _navigation.NavigateTo(_serviceProvider.GetRequiredService<HelpViewModel>());
    }
}
