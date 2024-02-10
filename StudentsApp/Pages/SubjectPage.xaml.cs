using Microsoft.EntityFrameworkCore;

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;

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
        List<Subject> _subjects = new List<Subject>();

        public SubjectPage()
        {
            InitializeComponent();

            UpdateSubjectList();
        }

        #region CRUD operations
        private void UpdateSubjectList()
        {
            _db.Subjects.Load();
            _subjects = _db.Subjects.ToList();

            string request = txtBoxSearch.Text.
                                          Replace(" ", "").
                                          ToLower();

            List<Subject> subjectList = SeacrhSubjects(_subjects, request);

            lViewSubjectList.ItemsSource = subjectList;
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
        #endregion


        #region Search
        /// <summary>
        /// Поиск предмета по коду и названию
        /// </summary>
        /// <param name="subjects">Список предметов для поиска</param>
        /// <param name="request">Поисковый запрос</param>
        /// <returns>Результат поиска</returns>
        private List<Subject> SeacrhSubjects(List<Subject> subjects, string request)
        {
            return subjects.Where(s => (s.SubjectId +
                                        s.SubjectName).
                                            ToLower().
                                            Contains(request)).
                                            ToList();
        }

        private void txtBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateSubjectList();
        }
        #endregion
    }
}