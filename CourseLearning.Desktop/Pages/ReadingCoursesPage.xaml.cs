using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Windows;
using Microsoft.Win32;

namespace CourseLearning.Desktop.Pages;

/// <summary>
/// Логика взаимодействия для ReadingCoursesPage.xaml
/// </summary>
public partial class ReadingCoursesPage : System.Windows.Controls.Page
{
    private string jsonPath = "";
    private List<Models.Page> pageObjects = new List<Models.Page>();
    private int iterator = 0;

    public ReadingCoursesPage()
    {
        InitializeComponent();

        //Скрытые элементов отображения страницы
        PageReadingLayout.Visibility = Visibility.Collapsed;
    }

    private void NextPageReadingCoursesButton_Click(object sender, RoutedEventArgs e)
    {
        if (!ResultTestReading(pageObjects[iterator]) && pageObjects[iterator].StandartTest.Question != "") //Проверка результата теста
        {
            MessageBox.Show($"Дан неверный вариант ответа на тест. Попробуй ещё раз");
        }
        else if (!ResultAnswerReading(pageObjects[iterator]) && pageObjects[iterator].Question != "") //Проверка ответа на вопрос
        {
            MessageBox.Show($"Дан неверный ответ на вопрос. Попробуй ещё раз");
        }
        else
        {
            //Переход по итератору на новую страницу
            iterator++;
            FillPageObjectsReading(pageObjects, iterator);

            //Проверка на заполненность значений в тесте и наличие вопроса
            CheckTestPageReading(pageObjects[iterator]);
            CheckStandartQuestionReading(pageObjects[iterator]);

            //Снятие галочки с выбранных вариантов ответа в тесте
            FirstAnswerTestReading.IsChecked = false;
            SecondAnswerTestReading.IsChecked = false;
            ThirdAnswerTestReading.IsChecked = false;
            FourAnswerTestReading.IsChecked = false;
        }
    }

    private void FindFileButton_Click(object sender, RoutedEventArgs e)
    {
        //Открытие файла
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "Json files (*.json)|*.json|All files (*.*)|*.*";

        if (openFileDialog.ShowDialog() == true)
        {
            //Получение пути файла
            jsonPath = openFileDialog.FileName;

            //Получение контекста
            string jsonContents = File.ReadAllText(jsonPath);

            JsonSerializerOptions jso = new JsonSerializerOptions();
            jso.Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic);

            //Считывания с файла в список объектов класса
            pageObjects = JsonSerializer.Deserialize<List<Models.Page>>(jsonContents, jso);

            //Получение значения итератора
            //iterator = 0;
            //iterator = GetCourseProgress(CurentUser.Id, pageObjects[0].header, pageObjects.Count);

            if (iterator < pageObjects.Count)
            {
                //Скрытие кнопки открытия файла
                PageReadingLayout.Visibility = Visibility.Visible;
                PageLoadingLayout.Visibility = Visibility.Collapsed;

                //Заполнение значений
                FillPageObjectsReading(pageObjects, iterator);

                CheckStandartQuestionReading(pageObjects[iterator]);
                CheckTestPageReading(pageObjects[iterator]);
            }
            else
            {
                MessageBox.Show("Курс пройден! Поздравляю!");
            }
        }
    }

    //Функция, заполняющая элементы разметки список объекта по итератору
    public void FillPageObjectsReading(List<Models.Page> pObjects, int iterator)
    {
        PageCountReading.Text = $"Страница {pObjects[iterator].PageNumber}/{pObjects.Count}";
        HeaderPageReading.Text = pObjects[iterator].Header;
        TextPageReading.Text = pObjects[iterator].Text;
        TestQuestionReading.Text = pObjects[iterator].StandartTest.Question;

        FirstAnswerTestReading.Content = pObjects[iterator].StandartTest.AnswerOptions[0];
        SecondAnswerTestReading.Content = pObjects[iterator].StandartTest.AnswerOptions[1];
        ThirdAnswerTestReading.Content = pObjects[iterator].StandartTest.AnswerOptions[2];
        FourAnswerTestReading.Content = pObjects[iterator].StandartTest.AnswerOptions[3];

        StandartQuestionReading.Text = pObjects[iterator].Question;
        StandartAnswerReading.Text = "";

        //Проверка на последнюю страницу
        if (CheckLastPage(pObjects, iterator))
        {
            NextPageReadingCoursesButton.Visibility = Visibility.Collapsed;
            if (!CheckTestAndStandartQuestionReading(pObjects[iterator]))
            {
                LastPageReadingCoursesButton.Visibility = Visibility.Visible;
            }
        }
        ScrollToTop();
    }

    //Функция, проверяющая последняя ли это страница в списке
    public bool CheckLastPage(List<Models.Page> pObjects, int iterator)
    {
        return pObjects[iterator].PageNumber == pObjects.Count;
    }

    //Функция, которая скрывает тест, если он не заполнен
    public void CheckTestPageReading(Models.Page pObject)
    {
        if (pObject.StandartTest.Question == "")
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

    //Функция, которая скрывает стандартный вопрос, если он не заполнен
    public void CheckStandartQuestionReading(Models.Page pObject)
    {
        if (pObject.Question == "")
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

    //Функция, проверяющая на наличие теста или вопроса
    public bool CheckTestAndStandartQuestionReading(Models.Page pObject)
    {
        return pObject.Question == "" || pObject.StandartTest.Question == "";
    }

    //Кнопка для последней страницы
    private void LastPageReadingCoursesButton_Click(object sender, RoutedEventArgs e)
    {
        if (!ResultTestReading(pageObjects[iterator])) //Проверка результата теста
        {
            MessageBox.Show($"Дан неверный вариант ответа на тест. Попробуй ещё раз");
        }
        else if (!ResultAnswerReading(pageObjects[iterator])) //Проверка ответа на вопрос
        {
            MessageBox.Show($"Дан неверный ответ на вопрос. Попробуй ещё раз");
        }
        else
        {
            MessageBox.Show($"Тест пройден!");
        }
    }

    //Функция, проверяющая ответ теста на правильность
    public bool ResultTestReading(Models.Page pObject)
    {
        switch (pObject.StandartTest.CorrectAnswer)
        {
            case 1:
                return FirstAnswerTestReading.IsChecked == true;

            case 2:
                return SecondAnswerTestReading.IsChecked == true;

            case 3:
                return ThirdAnswerTestReading.IsChecked == true;

            case 4:
                return FourAnswerTestReading.IsChecked == true;
        }
        return false;
    }

    //Функция, проверяющая ответ на вопрос
    public bool ResultAnswerReading(Models.Page pObject)
    {
        return StandartAnswerReading.Text == pObject.CorrectAnswer.ToString();
    }

    private void ScrollToTop()
    {
        // Установка вертикального смещения прокрутки в 0 для прокрутки вверх
        ReadingScroll.ScrollToVerticalOffset(0);
    }
}