using Microsoft.EntityFrameworkCore;

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using StudentsApp.Models;
using StudentsApp.Windows;

namespace StudentsApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для MarksPage.xaml
    /// </summary>
    public partial class MarksPage : Page
    {
        StudentsContext _db = new StudentsContext();

        public MarksPage()
        {
            InitializeComponent();

            UpdateMarks();
        }

        #region CRUD operations
        private void UpdateMarks()
        {
            _db.StudentsSuccesses.Load();

            dGridMarks.ItemsSource = _db.StudentsSuccesses.ToList();
        }

        private void ShowAddOrEditWindow(StudentsSuccess mark)
        {
            AddEditMarksWindow addEditMarksWindow = new AddEditMarksWindow(mark);
            addEditMarksWindow.ShowDialog();

            UpdateMarks();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            ShowAddOrEditWindow(null);
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            StudentsSuccess? editedMark = (sender as Button).DataContext as StudentsSuccess;

            ShowAddOrEditWindow(editedMark);
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result =
                MessageBox.Show(
                    "Удалить запись?",
                    "Удаление записи",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning
                    );

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    StudentsSuccess? deletedMark = (sender as Button).DataContext as StudentsSuccess;

                    _db.StudentsSuccesses.Remove(deletedMark);

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

            UpdateMarks();
        }
        #endregion
    }
}