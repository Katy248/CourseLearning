using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CourseLearning.Desktop.Pages;

namespace CourseLearning.Desktop.Views.Components;
/// <summary>
/// Логика взаимодействия для SideBar.xaml
/// </summary>
public partial class SideBar : UserControl
{
    public SideBar()
    {
        InitializeComponent();
    }
    private void sideBarCreatingCourses_Click(object sender, RoutedEventArgs e)
    {
        // Navigate to the creating courses page
        //contentFrame.Navigate(new CreatingCoursesPage());
    }

    private void sideBarReadingCourses_Click(object sender, RoutedEventArgs e)
    {
        // Navigate to the read courses page
        //contentFrame.Navigate(new ReadingCoursesPage());
    }

    private void sideBarHelp_Click(object sender, RoutedEventArgs e)
    {
        //contentFrame.Navigate(new HelpPage());
    }
}
