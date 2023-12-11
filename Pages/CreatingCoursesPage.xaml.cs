using CourseLearning_Lite.Classes;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
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

namespace CourseLearning_Lite.Pages
{
    /// <summary>
    /// Логика взаимодействия для CreatingCoursesPage.xaml
    /// </summary>
    public partial class CreatingCoursesPage : Page
    {
        //Лист страниц, необходимый для дальнейшего сохранения в JSON файле
        List<PageObject> pageObjects = new List<PageObject>();

        //Итератор для страниц
        int actualIterator = 1;

        public CreatingCoursesPage()
        {
            InitializeComponent();
        }

        private PageObject ExtractPageObjectFromMarkup()
        {
            PageObject pageObject = new PageObject();

            // Extract page number
            string pageNumberText = PageNumber.Text;
            string[] pageNumberParts = pageNumberText.Split(':');
            int pageNumber = int.Parse(pageNumberParts[1].Trim());
            pageObject.page_number = pageNumber;

            // Extract header
            pageObject.header = PageHeader.Text;

            // Extract text
            pageObject.text = PageText.Text;

            // Extract standardized test
            TestObject testObject = new TestObject();
            testObject.question = TestQuestion.Text;
            testObject.answer_options = new string[]
            {
                AnswerOption1.Text,
                AnswerOption2.Text,
                AnswerOption3.Text,
                AnswerOption4.Text
            };
            testObject.correct_answer = CorrectAnswer.SelectedIndex + 1;
            pageObject.standardized_test = testObject;

            // Extract regular question with written answer
            pageObject.question = RegularQuestion.Text;
            pageObject.correct_answer = CorrectAnswerText.Text;

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
            PageObject existingPage = pageObjects.FirstOrDefault(p => p.page_number == actualIterator);

            if (existingPage != null)
            {
                // Если страница существует, обновляем её данные
                existingPage = ExtractPageObjectFromMarkup();
            }
            else
            {
                // Если страницы нет, добавляем новую
                PageObject result = ExtractPageObjectFromMarkup();
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
                PageObject result = ExtractPageObjectFromMarkup();
                pageObjects.Add(result);
            }
            else
            {
                PageObject result = ExtractPageObjectFromMarkup();
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
        public void FillPageObjectsCourseCreating(List<PageObject> pObjects, int iterator)
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
                PageHeader.Text = pObjects[iterator].header;
                PageText.Text = pObjects[iterator].text;
                TestQuestion.Text = pObjects[iterator].standardized_test.question;
                AnswerOption1.Text = pObjects[iterator].standardized_test.answer_options[0];
                AnswerOption2.Text = pObjects[iterator].standardized_test.answer_options[1];
                AnswerOption3.Text = pObjects[iterator].standardized_test.answer_options[2];
                AnswerOption4.Text = pObjects[iterator].standardized_test.answer_options[3];
                CorrectAnswer.SelectedIndex = pObjects[iterator].standardized_test.correct_answer - 1;

                RegularQuestion.Text = pObjects[iterator].question;
                CorrectAnswerText.Text = pObjects[iterator].correct_answer;
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
    }
}
