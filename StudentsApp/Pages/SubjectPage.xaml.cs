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
        // Поле фильтрации
        const int FILTER_BY_SUBJECT_ID = 1;     // Фильтр по коду предмета

        // Поле сортировки
        const int SORT_BY_SUBJECT_ID    = 0,    // Сортировка по коду предмета
                  SORT_BY_SUBJECT_NAME  = 1,    // Сортировка по названию предмета
                  SORT_BY_TOTAL_HOURS   = 2;    // Сортировка по количеству часов

        // Тип сортировки
        const int ASC_SORT = 0;     // Сортировка по возрастанию

        StudentsContext _db = new StudentsContext();
        List<Subject> _subjects = new List<Subject>();

        public SubjectPage()
        {
            InitializeComponent();

            cmbBoxFilterField.SelectedIndex = 0;
            cmbBoxFilterType.SelectedIndex = 0;

            cmbBoxSortField.SelectedIndex = 0;
            cmbBoxSortType.SelectedIndex = 0;

            UpdateSubjectList();
        }

        #region CRUD operations
        /// <summary>
        /// Обновление списка предметов на основе сортировки, поиска, фильтрации и деления на страницы
        /// </summary>
        private void UpdateSubjectList()
        {
            _db.Subjects.Load();
            _subjects = _db.Subjects.ToList();

            string request = txtBoxSearch.Text.
                                          Replace(" ", "").
                                          ToLower();

            int subjectId = 0;

            if (cmbBoxFilterType.SelectedIndex > 0)
            {
                int.TryParse(cmbBoxFilterType.SelectedValue.ToString(), out subjectId);
            }

            int filterFiled = cmbBoxFilterField.SelectedIndex;

            List<Subject> subjectList = SearchSubjects(
                                            FilterSubjects(_subjects, 
                                                           filterFiled, 
                                                           subjectId),
                                                       request);

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
        private List<Subject> SearchSubjects(List<Subject> subjects, string request)
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

        #region Filter
        /// <summary>
        /// Очистка данных из ComboBox
        /// </summary>
        /// <param name="comboBox">ComboBox для очистки</param>
        private void ClearComboBox(ComboBox comboBox)
        {
            try
            {
                comboBox.Items.Clear();
            }
            catch (Exception)
            {
                comboBox.ItemsSource = null;
            }
        }

        /// <summary>
        /// <para>Получение списка кодов предметов</para>
        /// <para>Загрузка списка в ComboBox</para>
        /// </summary>
        /// <param name="filterType">ComboBox для загрузки</param>
        private void LoadSubjectIdInComboBox(ComboBox filterType)
        {
            List<string> subjectId = _db.Subjects.Select(s => s.SubjectId).ToList();

            for (int i = 0; i < subjectId.Count; i++)
            {
                subjectId[i] = subjectId[i].Split(".")[0];
            }

            subjectId = subjectId.Distinct().OrderBy(s => s).ToList();
            subjectId.Insert(0, "Все предметы");

            ClearComboBox(filterType);

            filterType.ItemsSource = subjectId;
            filterType.SelectedIndex = 0;
        }

        private List<Subject> FilterSubjects(List<Subject> subjects, int filterField, int subjectId)
        {
            if (subjectId != 0)
            {
                switch (filterField)
                {
                    case FILTER_BY_SUBJECT_ID:
                        subjects = subjects.Where(s => s.SubjectId.Contains(subjectId.ToString())).ToList();

                        break;

                    default:
                        break;
                }
            }

            return subjects;
        }

        private void cmbBoxFilterField_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (cmbBoxFilterField.SelectedIndex)
            {
                case FILTER_BY_SUBJECT_ID:
                    LoadSubjectIdInComboBox(cmbBoxFilterType);
                    break;

                default:
                    ClearComboBox(cmbBoxFilterType);
                    cmbBoxFilterType.Items.Add("Не задано");
                    cmbBoxFilterType.SelectedIndex = 0;
                    break;
            }
        }

        private void cmbBoxFilterType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSubjectList();
        }
        #endregion

        #region Sort
        private void cmbBoxSortField_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cmbBoxSortType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        #endregion
    }
}