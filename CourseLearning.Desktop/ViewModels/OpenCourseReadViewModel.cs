using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CourseLearning.Desktop.Services;
using Microsoft.Win32;

namespace CourseLearning.Desktop.ViewModels;
public partial class OpenCourseReadViewModel : ObservableObject
{
    private readonly CourseService _courseService;
    private readonly NavigationService _navigationService;

    public OpenCourseReadViewModel(CourseService courseService, NavigationService navigationService)
    {
        _courseService = courseService;
        _navigationService = navigationService;
    }
    [RelayCommand]
    public async Task OpenFile()
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter = "Json files (*.json)|*.json|All files (*.*)|*.*",
            Multiselect = false,
        };

        if (openFileDialog.ShowDialog() is true)
        {
            //Получение пути файла
            var file = new FileInfo(openFileDialog.FileName);

            var pages = _courseService.OpenFile(file);

            if (pages.Any())
            {
                //TODO:
            }
        }
    }
}
