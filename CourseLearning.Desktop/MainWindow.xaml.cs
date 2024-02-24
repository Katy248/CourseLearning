using System.Windows;
using CourseLearning.Desktop.Pages;

namespace CourseLearning.Desktop;

/// <summary>
/// Логика взаимодействия для MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void sideBarCreatingCourses_Click(object sender, RoutedEventArgs e)
    {
        // Navigate to the creating courses page
        contentFrame.Navigate(new CreatingCoursesPage());
    }

    private void sideBarReadingCourses_Click(object sender, RoutedEventArgs e)
    {
        // Navigate to the read courses page
        contentFrame.Navigate(new ReadingCoursesPage());
    }

    private void sideBarHelp_Click(object sender, RoutedEventArgs e)
    {
        contentFrame.Navigate(new HelpPage());
    }
}