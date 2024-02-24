using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Windows;
using CourseLearning.Core.Models;
using Microsoft.Win32;

namespace CourseLearning.Desktop.Pages;

public partial class ReadingCoursesPage : System.Windows.Controls.Page
{
    private string jsonPath = "";
    private List<Page> _pages = new();
    private int _currentPageIndex = 0;
    private Page CurrentPage => _pages[_currentPageIndex];

    public ReadingCoursesPage()
    {
        InitializeComponent();

        //Скрытые элементов отображения страницы
        PageReadingLayout.Visibility = Visibility.Collapsed;
    }

    private void NextPageReadingCoursesButton_Click(object sender, RoutedEventArgs e)
    {

        if (!ResultTestReading() && CurrentPage.StandartTest.Question != "") //Проверка результата теста
        {
            MessageBox.Show($"Дан неверный вариант ответа на тест. Попробуй ещё раз");
            return;
        }
        if (!ResultAnswerReading(CurrentPage) && CurrentPage.Question != "") //Проверка ответа на вопрос
        {
            MessageBox.Show($"Дан неверный ответ на вопрос. Попробуй ещё раз");
            return;
        }
        //Переход по итератору на новую страницу
        if (_currentPageIndex < _pages.Count)
        {
            _currentPageIndex++;
        }
        UpdatePageData();

        //Проверка на заполненность значений в тесте и наличие вопроса
        CheckTestPageReading(CurrentPage);
        CheckStandardQuestionReading(CurrentPage);

        //Снятие галочки с выбранных вариантов ответа в тесте
        
    }

    private void FindFileButton_Click(object sender, RoutedEventArgs e)
    {
        //Открытие файла
        var openFileDialog = new OpenFileDialog
        {
            Filter = "Json files (*.json)|*.json|All files (*.*)|*.*",
        };

        if (openFileDialog.ShowDialog() == true)
        {
            //Получение пути файла
            jsonPath = openFileDialog.FileName;

            //Получение контекста
            string jsonContents = File.ReadAllText(jsonPath);

            JsonSerializerOptions jso = new JsonSerializerOptions();
            jso.Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic);

            //Считывания с файла в список объектов класса
            _pages = JsonSerializer.Deserialize<List<Page>>(jsonContents, jso); //TODO: Необходима проверка правильности десериализации

            //Получение значения итератора
            //iterator = 0;
            //iterator = GetCourseProgress(CurentUser.Id, pageObjects[0].header, pageObjects.Count);

            if (_currentPageIndex < _pages.Count)
            {
                //Скрытие кнопки открытия файла
                PageReadingLayout.Visibility = Visibility.Visible;
                PageLoadingLayout.Visibility = Visibility.Collapsed;

                //Заполнение значений
                UpdatePageData();

                CheckStandardQuestionReading(_pages[_currentPageIndex]);
                CheckTestPageReading(_pages[_currentPageIndex]);
            }
            else
            {
                MessageBox.Show("Курс пройден! Поздравляю!");
            }
        }
    }

    /// <summary>
    /// Обновляет данные представления на основе текущей страницы <see cref="CurrentPage"/>.
    /// </summary>
    public void UpdatePageData()
    {
        // Снятие галочек
        FirstAnswerTestReading.IsChecked = false;
        SecondAnswerTestReading.IsChecked = false;
        ThirdAnswerTestReading.IsChecked = false;
        FourAnswerTestReading.IsChecked = false;

        PageCountReading.Text = $"Страница {CurrentPage.PageNumber}/{_pages.Count}";
        HeaderPageReading.Text = CurrentPage.Header;
        TextPageReading.Text = CurrentPage.Text;
        TestQuestionReading.Text = CurrentPage.StandartTest.Question;

        FirstAnswerTestReading.Content = CurrentPage.StandartTest.AnswerOptions[0];
        SecondAnswerTestReading.Content = CurrentPage.StandartTest.AnswerOptions[1];
        ThirdAnswerTestReading.Content = CurrentPage.StandartTest.AnswerOptions[2];
        FourAnswerTestReading.Content = CurrentPage.StandartTest.AnswerOptions[3];

        StandartQuestionReading.Text = CurrentPage.Question;
        StandartAnswerReading.Text = "";

        //Проверка на последнюю страницу
        if (IsLastPage())
        {
            NextPageReadingCoursesButton.Visibility = Visibility.Collapsed;
            if (!CheckTestAndStandardQuestionReading(CurrentPage))
            {
                LastPageReadingCoursesButton.Visibility = Visibility.Visible;
            }
        }
        ScrollToTop();
    }

    /// <summary>
    /// Функция, проверяющая последняя ли это страница в списке
    /// </summary>
    /// <returns></returns>
    public bool IsLastPage()
    {
        return CurrentPage.PageNumber == _pages.Count;
    }

    /// <summary>
    /// Функция, которая скрывает тест, если он не заполнен
    /// </summary>
    /// <param name="page"></param>
    public void CheckTestPageReading(Page page)
    {
        if (page.StandartTest.Question == "")
        {
            TestQuestionReading.Visibility = Visibility.Collapsed;
            AnswersOnTestQuestionReading.Visibility = Visibility.Collapsed;
        }
        else
        {
            TestQuestionReading.Visibility = Visibility.Visible;
            AnswersOnTestQuestionReading.Visibility = Visibility.Visible;
        }
    }


    /// <summary>
    /// Функция, которая скрывает стандартный вопрос, если он не заполнен
    /// </summary>
    /// <param name="page"></param>
    public void CheckStandardQuestionReading(Page page)
    {
        if (page.Question == "")
        {
            StandartQuestionReading.Visibility = Visibility.Collapsed;
            StandartAnswerReading.Visibility = Visibility.Collapsed;
        }
        else
        {
            StandartQuestionReading.Visibility = Visibility.Visible;
            StandartAnswerReading.Visibility = Visibility.Visible;
        }
    }

    /// <summary>
    /// Функция, проверяющая на наличие теста или вопроса
    /// </summary>
    /// <param name="page"></param>
    /// <returns></returns>
    public static bool CheckTestAndStandardQuestionReading(Page page)
    {
        return page.Question == "" || page.StandartTest.Question == "";
    }

    //Кнопка для последней странице
    private void LastPageReadingCoursesButton_Click(object sender, RoutedEventArgs e)
    {
        if (!ResultTestReading()) //Проверка результата теста
        {
            MessageBox.Show($"Дан неверный вариант ответа на тест. Попробуй ещё раз");
        }
        else if (!ResultAnswerReading(CurrentPage)) //Проверка ответа на вопрос
        {
            MessageBox.Show($"Дан неверный ответ на вопрос. Попробуй ещё раз");
        }
        else
        {
            MessageBox.Show($"Тест пройден!");
        }
    }

    /// <summary>
    /// Функция, проверяющая ответ теста текущей страницы (<see cref="CurrentPage"/>) на правильность.
    /// </summary>
    /// <param name="page"></param>
    /// <returns></returns>
    public bool ResultTestReading() =>
        CurrentPage.StandartTest.CorrectAnswer switch
        {
            1 => FirstAnswerTestReading.IsChecked == true,
            2 => SecondAnswerTestReading.IsChecked == true,
            3 => ThirdAnswerTestReading.IsChecked == true,
            4 => FourAnswerTestReading.IsChecked == true,
            _ => false,
        };
    

    /// <summary>
    /// Функция, проверяющая ответ на вопрос
    /// </summary>
    /// <param name="page"></param>
    /// <returns></returns>
    public bool ResultAnswerReading(Page page)
    {
        return StandartAnswerReading.Text == page.CorrectAnswer.ToString();
    }

    private void ScrollToTop()
    {
        // Установка вертикального смещения прокрутки в 0 для прокрутки вверх
        ReadingScroll.ScrollToVerticalOffset(0);
    }
}