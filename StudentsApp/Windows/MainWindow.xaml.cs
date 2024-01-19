using Microsoft.EntityFrameworkCore;

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using StudentsApp.Models;
using StudentsApp.Windows;
using System.Collections.Generic;

namespace StudentsApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        StudentsContext _db = new StudentsContext();

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Загрузка данных в DataGrid
        /// </summary>
        private void LoadDbInDataGrid()
        {
            _db.Students.Load();
            dgStudents.ItemsSource = _db.Students.ToList();
        }

        /// <summary>
        /// Обновление данных в DataGrid
        /// </summary>
        /// <param name="grid">DataGrid для обновления</param>
        private void UpdateDataGrid(DataGrid grid)
        {
            grid.ItemsSource = null;
            grid.ItemsSource = _db.Students.
                               OrderBy(student => student.StudentId).
                               ToList();
        }

        /// <summary>
        /// Вывод окна добавления / редактирования студентов
        /// </summary>
        /// <param name="student">Информация о студенте</param>
        private void ShowAddOrEditWindows(Student student)
        {
            AddEditStudentWindow addEditStudentWindow = new AddEditStudentWindow(student);
            addEditStudentWindow.ShowDialog();

            UpdateDataGrid(dgStudents);
        }

        /// <summary>
        /// Загрузка окна
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDbInDataGrid();
            UpdateDataGrid(dgStudents);
        }

        /// <summary>
        /// Поиск студента по ФИО
        /// </summary>
        private void txtBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            List<Student> currentStudent = new List<Student>();

            string request = txtBoxSearch.Text.
                                          Replace(" ", "").
                                          ToLower();

            currentStudent = _db.Students.Where(s => (s.LastName + 
                                                      s.FirstName +
                                                      s.MiddleName).
                                                           ToLower().
                                                           Contains(request)).
                                                           ToList();

            dgStudents.ItemsSource = currentStudent;
        }

        /// <summary>
        /// <para>Получение списка годов поступления студентов</para>
        /// <para>Загрузка списка в ComboBox</para>
        /// </summary>
        /// <param name="filterType">ComboBox для загрузки</param>
        private void LoadAdmissionYearsInComboBox(ComboBox filterType)
        {
            List<string> admissionYears = _db.Students.Select(s => s.StudentId).ToList();

            for (int i = 0; i < admissionYears.Count; i++)
            {
                admissionYears[i] = "20" + admissionYears[i].Split('-')[1];
            }

            admissionYears = admissionYears.Distinct().OrderBy(y => y).ToList();
            admissionYears.Insert(0, "Все года");

            try
            {
                filterType.Items.Clear();
            }
            catch (Exception)
            {
                filterType.ItemsSource = null;
            }

            filterType.ItemsSource = admissionYears;
            filterType.SelectedIndex = 0;
        }

        /// <summary>
        /// <para>Получение списка годов рождения студентов</para>
        /// <para>Загрузка списка в ComboBox</para>
        /// </summary>
        /// <param name="filterType">ComboBox для загрузки</param>
        private void LoadBirthYearsInComboBox(ComboBox filterType)
        {
            List<string> birthYears = _db.Students.Select(s => s.BirthDate.Year.ToString()).
                                                   Distinct().
                                                   OrderBy(y => y).
                                                   ToList();

            birthYears.Insert(0, "Все года");

            try
            {
                filterType.Items.Clear();
            }
            catch (Exception)
            {
                filterType.ItemsSource = null;
            }

            filterType.ItemsSource = birthYears;
            filterType.SelectedIndex = 0;
        }

        // Фильтр по году рождения и по году поступления
        const int FILTER_BY_ADMISSION_YEAR = 1;
        const int FILTER_BY_BIRTH_YEAR = 2;

        private void cmbBoxFilterField_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (cmbBoxFilterField.SelectedIndex)
            {
                // Фильтр по году поступления
                case FILTER_BY_ADMISSION_YEAR:
                    LoadAdmissionYearsInComboBox(cmbBoxFilterType);
                    break;

                // Фильтр по году рождения
                case FILTER_BY_BIRTH_YEAR:
                    LoadBirthYearsInComboBox(cmbBoxFilterType);
                    break;

                // Поле фильтра не выбрано
                default:
                    break;
            }
        }

        private void cmbBoxFilterType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cmbBoxSortField_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cmbBoxSortType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        /// <summary>
        /// Кнопка "Добавить"
        /// </summary>
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            ShowAddOrEditWindows(null);
        }

        /// <summary>
        /// Кнопка "Редактировать"
        /// </summary>
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            Student? editedStudent = (sender as Button).DataContext as Student;

            ShowAddOrEditWindows(editedStudent);
        }

        /// <summary>
        /// Кнопка "Удалить"
        /// </summary>
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result =
                MessageBox.Show(
                    "Удалить запись?",
                    "Удаление записи",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning
                    );

            if ( result == MessageBoxResult.Yes )
            {
                try
                {
                    Student? deletedStudent = (sender as Button).DataContext as Student;

                    _db.Students.Remove(deletedStudent);

                    MessageBox.Show(
                        "Запись удалена",
                        "Информация",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information
                        );

                    _db.SaveChanges();
                }
                catch (ArgumentOutOfRangeException)
                {

                    MessageBox.Show(
                        "Выберите запись для удаления",
                        "Предупреждение",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                        );
                }
            }

            UpdateDataGrid(dgStudents);
        }

        private void cBoxPageCount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int currentPage = 1;

            int pageSize = (cBoxPageCount.SelectedIndex + 1) * 5;

            int maxPage = (int)Math.Ceiling((double)_db.Students.Count()  / pageSize);

            var studentsList = _db.Students.ToList().Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();

            txtBoxCurrentPage.Text = currentPage.ToString();
            txtBoxTotalPage.Text = maxPage.ToString();

            dgStudents.ItemsSource = studentsList;
        }

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {

        }

        private void txtBoxCurrentPage_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnEnd_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}