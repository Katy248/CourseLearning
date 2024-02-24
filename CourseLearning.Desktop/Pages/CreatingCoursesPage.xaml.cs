using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Windows;
using CourseLearning.Desktop.Models;
using Microsoft.Win32;

namespace CourseLearning.Desktop.Pages
{
    /// <summary>
    /// Логика взаимодействия для CreatingCoursesPage.xaml
    /// </summary>
    public partial class CreatingCoursesPage : System.Windows.Controls.Page
    {
        //Лист страниц, необходимый для дальнейшего сохранения в JSON файле
        private List<Models.Page> pageObjects = new List<Models.Page>();

        //Итератор для страниц
        private int actualIterator = 1;

        public CreatingCoursesPage()
        {
            InitializeComponent();
        }

        private Models.Page ExtractPageObjectFromMarkup()
        {
            var pageObject = new Models.Page();

            // Extract page number
            string pageNumberText = PageNumber.Text;
            string[] pageNumberParts = pageNumberText.Split(':');
            int pageNumber = int.Parse(pageNumberParts[1].Trim());
            pageObject.PageNumber = pageNumber;

            // Extract header
            pageObject.Header = PageHeader.Text;

            // Extract text
            pageObject.Text = PageText.Text;

            // Extract standardized test
            Test testObject = new Test();
            testObject.Question = TestQuestion.Text;
            testObject.AnswerOptions = new string[]
            {
                AnswerOption1.Text,
                AnswerOption2.Text,
                AnswerOption3.Text,
                AnswerOption4.Text
            };
            testObject.CorrectAnswer = CorrectAnswer.SelectedIndex + 1;
            pageObject.StandartTest = testObject;

            // Extract regular question with written answer
            pageObject.Question = RegularQuestion.Text;
            pageObject.CorrectAnswer = CorrectAnswerText.Text;

            return pageObject;
        }

        private void SaveButtonCreating_Click(object sender, RoutedEventArgs e)
        {
            //Добавление объекта в список объектов
            SaveCurrentPage();

            // Serialize the modified list of PageObject objects back into a JSON string
            string newJsonString = JsonSerializer.Serialize(pageObjects, new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic) });

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
            Models.Page existingPage = pageObjects.FirstOrDefault(p => p.PageNumber == actualIterator);

            if (existingPage != null)
            {
                // Если страница существует, обновляем её данные
                existingPage = ExtractPageObjectFromMarkup();
            }
            else
            {
                // Если страницы нет, добавляем новую
                Models.Page result = ExtractPageObjectFromMarkup();
                pageObjects.Add(result);
            }

            // Увеличиваем actualIterator на 1
            actualIterator += 1;

            // Вывод кнопки перехода на прошлую страницу
            PreviousPageCreating.Visibility = Visibility.Visible;

            // Заполняем значения полей разметки для новой страницы
            FillPageObjectsCourseCreating(pageObjects, actualIterator);
        }

        private void PreviousPageCreating_Click(object sender, RoutedEventArgs e)
        {
            if (CheckHeaderAndText())
            {
                return;
            }

            // Проверяем, не выходит ли итератор за границы списка
            if (actualIterator > 1)
            {
                // Сохраняем данные текущей страницы
                SaveCurrentPage();

                // Уменьшаем итератор
                actualIterator -= 1;

                // Загружаем данные предыдущей страницы
                LoadPageData();
            }
        }

        // Метод для сохранения данных текущей страницы
        private void SaveCurrentPage()
        {
            if (actualIterator > pageObjects.Count)
            {
                Models.Page result = ExtractPageObjectFromMarkup();
                pageObjects.Add(result);
            }
            else
            {
                Models.Page result = ExtractPageObjectFromMarkup();
                pageObjects[actualIterator - 1] = result;
            }
        }

        // Метод для загрузки данных предыдущей страницы
        private void LoadPageData()
        {
            // Заполняем значения полей разметки
            FillPageObjectsCourseCreating(pageObjects, actualIterator);

            // Проверяем, нужно ли скрыть кнопку предыдущей страницы
            PreviousPageCreating.Visibility = (actualIterator == 1) ? Visibility.Collapsed : Visibility.Visible;
        }

        //Функция, которая заполняет значения полей разметки
        public void FillPageObjectsCourseCreating(List<Models.Page> pObjects, int iterator)
        {
            if (iterator > pageObjects.Count)
            {
                //Если страница новая - выводим пустую разметку
                ClearTextBoxes();
                //Отнимаем еденицу от итератора, поскольку он не совпадает с индексом
                iterator -= 1;
                PageNumber.Text = $"Страница: {actualIterator}";
            }
            else
            {
                //Отнимаем еденицу от итератора, поскольку он не совпадает с индексом
                iterator -= 1;
                PageNumber.Text = $"Страница: {actualIterator}";
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
}