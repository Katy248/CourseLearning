using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Windows;
using CourseLearning.Core.Models;
using Microsoft.Win32;

namespace CourseLearning.Desktop.Pages;


public partial class CreatingCoursesPage : System.Windows.Controls.Page
{
    /// <summary>
    /// Коллекция страниц.
    /// </summary>
    private readonly List<Page> _pages = new();

    /// <summary>
    /// Индекс редактируемой страницы.
    /// </summary>
    private int _currentPageIndex = 1;

    public CreatingCoursesPage()
    {
        InitializeComponent();
    }

    private Page ExtractPageObjectFromMarkup()
    {
        var page = new Page
        {
            Header = PageHeader.Text, // Extract header
            Text = PageText.Text, // Extract text
            PageNumber = int.Parse(PageNumber.Text.Split(':').FirstOrDefault()?.Trim() ?? "0"),
            StandartTest = new Test
            {
                Question = TestQuestion.Text,
                AnswerOptions = new string[]
                {
                    AnswerOption1.Text,
                    AnswerOption2.Text,
                    AnswerOption3.Text,
                    AnswerOption4.Text
                },
                CorrectAnswer = CorrectAnswer.SelectedIndex + 1
            },
            // Extract regular question with written answer
            Question = RegularQuestion.Text,
            CorrectAnswer = CorrectAnswerText.Text,
        };
        return page;
    }

    private void SaveButtonCreating_Click(object sender, RoutedEventArgs e)
    {
        //Добавление объекта в список объектов
        SaveCurrentPage();

        // Serialize the modified list of PageObject objects back into a JSON string
        string newJsonString = JsonSerializer.Serialize(_pages, new JsonSerializerOptions { WriteIndented = true });

        //Путь к файлу
        var filePath = string.Empty;

        //Фильтр для сохранения
        var saveFileDialog = new SaveFileDialog
        {
            Filter = "Json files (*.json)|*.json|All files (*.*)|*.*"
        };

        //Открытие диалогового окна для выбора пути и названия файла
        saveFileDialog.ShowDialog();
        if (saveFileDialog.FileName != string.Empty)
        {
            filePath = saveFileDialog.FileName;
        }

        // Write the new JSON string back to the file, overwriting the existing data
        File.WriteAllText(filePath, newJsonString);

        MessageBox.Show("Курс сохранен!");
    }

    private void NextPageCreating_Click(object sender, RoutedEventArgs e)
    {
        // Проверка наличия текущей страницы в списке
        Page existingPage = _pages.FirstOrDefault(p => p.PageNumber == _currentPageIndex);

        if (existingPage != null)
        {
            // Если страница существует, обновляем её данные
            existingPage = ExtractPageObjectFromMarkup();
        }
        else
        {
            // Если страницы нет, добавляем новую
            Page result = ExtractPageObjectFromMarkup();
            _pages.Add(result);
        }

        // Увеличиваем actualIterator на 1
        _currentPageIndex += 1;

        // Вывод кнопки перехода на прошлую страницу
        PreviousPageCreating.Visibility = Visibility.Visible;

        // Заполняем значения полей разметки для новой страницы
        FillPageObjectsCourseCreating(_pages, _currentPageIndex);
    }

    private void PreviousPageCreating_Click(object sender, RoutedEventArgs e)
    {
        if (CheckHeaderAndText())
        {
            return;
        }

        // Проверяем, не выходит ли итератор за границы списка
        if (_currentPageIndex > 1)
        {
            // Сохраняем данные текущей страницы
            SaveCurrentPage();

            // Уменьшаем итератор
            _currentPageIndex -= 1;

            // Загружаем данные предыдущей страницы
            LoadPageData();
        }
    }

    // Метод для сохранения данных текущей страницы
    private void SaveCurrentPage()
    {
        if (_currentPageIndex > _pages.Count)
        {
            Page result = ExtractPageObjectFromMarkup();
            _pages.Add(result);
        }
        else
        {
            Page result = ExtractPageObjectFromMarkup();
            _pages[_currentPageIndex - 1] = result;
        }
    }

    // Метод для загрузки данных предыдущей страницы
    private void LoadPageData()
    {
        // Заполняем значения полей разметки
        FillPageObjectsCourseCreating(_pages, _currentPageIndex);

        // Проверяем, нужно ли скрыть кнопку предыдущей страницы
        PreviousPageCreating.Visibility = (_currentPageIndex == 1) ? Visibility.Collapsed : Visibility.Visible;
    }

    //Функция, которая заполняет значения полей разметки
    public void FillPageObjectsCourseCreating(List<Page> pObjects, int iterator)
    {
        if (iterator > _pages.Count)
        {
            //Если страница новая - выводим пустую разметку
            ClearTextBoxes();
            //Отнимаем еденицу от итератора, поскольку он не совпадает с индексом
            iterator -= 1;
            PageNumber.Text = $"Страница: {_currentPageIndex}";
        }
        else
        {
            //Отнимаем еденицу от итератора, поскольку он не совпадает с индексом
            iterator -= 1;
            PageNumber.Text = $"Страница: {_currentPageIndex}";
            PageHeader.Text = pObjects[iterator].Header;
            PageText.Text = pObjects[iterator].Text;
            TestQuestion.Text = pObjects[iterator].StandartTest.Question;
            AnswerOption1.Text = pObjects[iterator].StandartTest.AnswerOptions[0];
            AnswerOption2.Text = pObjects[iterator].StandartTest.AnswerOptions[1];
            AnswerOption3.Text = pObjects[iterator].StandartTest.AnswerOptions[2];
            AnswerOption4.Text = pObjects[iterator].StandartTest.AnswerOptions[3];
            CorrectAnswer.SelectedIndex = pObjects[iterator].StandartTest.CorrectAnswer - 1;

            RegularQuestion.Text = pObjects[iterator].Question;
            CorrectAnswerText.Text = pObjects[iterator].CorrectAnswer;
        }
        ScrollToTop();
    }

    //Функция, которая очищает значения полей разметки
    private void ClearTextBoxes()
    {
        PageHeader.Text = "";
        PageText.Text = "";
        TestQuestion.Text = "";
        AnswerOption1.Text = "";
        AnswerOption2.Text = "";
        AnswerOption3.Text = "";
        AnswerOption4.Text = "";
        CorrectAnswer.SelectedIndex = 0;
        RegularQuestion.Text = "";
        CorrectAnswerText.Text = "";
    }

    //Проверка, пустой ли заголовок и текст страницы
    private bool CheckHeaderAndText()
    {
        return PageHeader.Text == "" || PageText.Text == "";
    }

    private void ScrollToTop()
    {
        // Установка вертикального смещения прокрутки в 0 для прокрутки вверх
        CreatingScroll.ScrollToVerticalOffset(0);
    }

    //Перенос многострочной информации в одну строку
    private string FixTextForJSON(string str)
    {
        str.Replace("\r\n\r\n", "\n");
        str.Replace("\r\n", " ");
        return str;
    }
}