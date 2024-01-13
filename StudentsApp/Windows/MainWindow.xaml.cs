using Microsoft.EntityFrameworkCore;

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using StudentsApp.Models;
using StudentsApp.Windows;

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
    }
}