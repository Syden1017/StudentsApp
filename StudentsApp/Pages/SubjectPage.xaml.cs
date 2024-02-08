using Microsoft.EntityFrameworkCore;

using System;
using System.Windows;
using System.Windows.Controls;
using System.Linq;

using StudentsApp.Models;
using StudentsApp.Windows;

namespace StudentsApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для SubjectPage.xaml
    /// </summary>
    public partial class SubjectPage : Page
    {
        StudentsContext _db = new StudentsContext();

        public SubjectPage()
        {
            InitializeComponent();

            UpdateSubjectList();
        }

        private void UpdateSubjectList()
        {
            _db.Subjects.Load();
            lViewSubjectList.ItemsSource = _db.Subjects.ToList();
        }

        private void ShowAddOrEditSubject(Subject subject)
        {
            AddEditSubjectWindow addEditSubjectWindow = new AddEditSubjectWindow(subject);
            addEditSubjectWindow.Show();

            UpdateSubjectList();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            ShowAddOrEditSubject(null);
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            Subject editedSubject = (sender as Button).DataContext as Subject;

            ShowAddOrEditSubject(editedSubject);
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
                    Subject? deletedSubject = (sender as Button).DataContext as Subject;

                    _db.Subjects.Remove(deletedSubject);

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

            UpdateSubjectList();
        }
    }
}