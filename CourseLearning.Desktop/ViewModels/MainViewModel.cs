using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CourseLearning.Desktop.Services;

namespace CourseLearning.Desktop.ViewModels;
public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private SideBarViewModel _sideBar;

    public MainViewModel(SideBarViewModel sideBar, NavigationService navigation)
    {
        _sideBar = sideBar;
        Navigation = navigation;
    }

    public NavigationService Navigation { get; }
}
