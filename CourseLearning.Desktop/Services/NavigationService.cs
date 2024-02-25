using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CourseLearning.Desktop.Services;
public partial class NavigationService : ObservableObject
{
    [ObservableProperty]
    private ObservableObject _currentView;

    public void NavigateTo(ObservableObject newView)
    {
        CurrentView = newView;
    }

}
