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
    /// Логика взаимодействия для MarksPage.xaml
    /// </summary>
    public partial class MarksPage : Page
    {
        // Поле фильтрации
        const int FILTER_BY_MARK = 1;       // Фильтр по оценке

        // Поле сортировки
        const int SORT_BY_STUDENT_ID = 0,   // Сортировка по номеру студенческого
                  SORT_BY_SUBJECT_ID = 1,   // Сортировка по коду предмета
                  SORT_BY_MARK       = 2,   // Сортировка по оценке
                  SORT_BY_EXAM_DATE  = 3;   // Сортировка по дате экзамена

        // Тип сортировки
        const int ASC_SORT = 0;    // Сортировка по возрастанию

        StudentsContext _db = new StudentsContext();
        List<StudentsSuccess> _grades = new List<StudentsSuccess>();
        int _marksCount = 0;

        // Распределение по страницам
        int _currentPage = 1,
            _maxPage = 0;

        public MarksPage()
        {
            InitializeComponent();

            cmbBoxFilterField.SelectedIndex = 0;
            cmbBoxFilterType.SelectedIndex = 0;

            cmbBoxSortField.SelectedIndex = 0;
            cmbBoxSortType.SelectedIndex = 0;

            cmbBoxMarksCount.SelectedIndex = 0;

            UpdateMarks();
        }

        #region CRUD operations
        /// <summary>
        /// Обновление списка оценок на основе сортировки, поиска и фильтрации
        /// </summary>
        private void UpdateMarks()
        {
            _db.StudentsSuccesses.Load();
            _grades = _db.StudentsSuccesses.ToList();
            _marksCount = _grades.Count;

            string request = txtBoxSearch.Text.
                                          Replace(" ", "").
                                          ToLower();

            int mark = 0;

            if (cmbBoxFilterType.SelectedIndex > 0)
            {
                int.TryParse(cmbBoxFilterType.SelectedValue.ToString(), out mark);
            }

            // Список формировать в порядке
            // сортировка -> поиск -> фильтрация -> деление на страницы
            int filterField = cmbBoxFilterField.SelectedIndex,
                sortField = cmbBoxSortField.SelectedIndex,
                sortType = cmbBoxSortType.SelectedIndex;

            List<StudentsSuccess> marksList = SortMarks(
                                                  SearchMarks(
                                                       FilterMark(
                                                           GetPages(_grades),
                                                                  filterField,
                                                                  mark),
                                                              request),
                                                        sortField,
                                                        sortType);

            txtBoxCurrentPage.Text = _currentPage.ToString();
            txtBoxCurrentPage.MaxLength = _maxPage.ToString().Length;
            txtBoxTotalPage.Text = _maxPage.ToString();

            dGridMarks.ItemsSource = marksList;

            txtBlockMarksCount.Text = _marksCount.ToString();
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

        #region Search
        /// <summary>
        /// Поиск студента по ФИО
        /// </summary>
        /// <param name="marks">Список оценок для поиска</param>
        /// <param name="request">Поисковый запрос</param>
        /// <returns>Результаты поиска</returns>
        private List<StudentsSuccess> SearchMarks(List<StudentsSuccess> marks, string request)
        {
            return marks.Where(s => (s.StudentId +
                                        s.SubjectId +
                                        s.Evaluation +
                                        s.ExamDate).
                                             ToLower().
                                             Contains(request)).
                                             ToList();
        }

        private void txtBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateMarks();
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
        /// <para>Получение списка оценок студентов</para>
        /// <para>Загрузка списка в ComboBox</para>
        /// </summary>
        /// <param name="filterType">ComboBox для загрузки</param>
        private void LoadMarkInComboBox(ComboBox filterType)
        {
            List<string> mark = _db.StudentsSuccesses.Select(s => s.Evaluation.ToString()).ToList();

            mark = mark.Distinct().OrderBy(y => y).ToList();

            ClearComboBox(filterType);

            mark.Insert(0, "Все оценки");
            filterType.ItemsSource = mark;

            filterType.SelectedIndex = 0;
        }

        /// <summary>
        /// Фильтрация списка оценок по полученным оценкам
        /// </summary>
        /// <param name="grades">Список оценок для фильтрации</param>
        /// <param name="filterField">Номер поля для фильтрации</param>
        /// <param name="mark">Значение оценки фильтрации</param>
        /// <returns>Результаты фильтрации</returns>
        private List<StudentsSuccess> FilterMark(List<StudentsSuccess> grades, int filterField, int mark)
        {
            if (mark != 0)
            {
                switch (filterField)
                {
                    case FILTER_BY_MARK:
                        grades = grades.Where(s => s.Evaluation == mark).ToList();
                        break;

                    default:
                        break;
                }
            }

            return grades;
        }

        private void cmbBoxFilterField_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (cmbBoxFilterField.SelectedIndex)
            {
                case FILTER_BY_MARK:
                    LoadMarkInComboBox(cmbBoxFilterType);
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
            UpdateMarks();
        }
        #endregion

        #region Sort
        /// <summary>
        /// Сортировка списка оценок
        /// </summary>
        /// <param name="marks">Список оценок для сортировки</param>
        /// <param name="sortField">Номер поля сортировки</param>
        /// <param name="sortType">Номер типа сортировки</param>
        /// <returns>Отсортированный список</returns>
        private List<StudentsSuccess> SortMarks(List<StudentsSuccess> marks, int sortField, int sortType)
        {
            Func<StudentsSuccess, object> sortExpression;

            switch (sortField)
            {
                case SORT_BY_STUDENT_ID:
                    sortExpression = s => s.StudentId;
                    break;

                case SORT_BY_SUBJECT_ID:
                    sortExpression= s => s.SubjectId;
                    break;

                case SORT_BY_MARK:
                    sortExpression = s => s.Evaluation;
                    break;

                case SORT_BY_EXAM_DATE:
                    sortExpression = s => s.ExamDate;
                    break;

                default:
                    sortExpression = s => s.StudentId;
                    break;
            }

            return sortType == ASC_SORT ? marks.OrderBy(sortExpression).ToList() :
                                          marks.OrderByDescending(sortExpression).ToList();
        }

        private void cmbBoxSortField_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateMarks();
        }

        private void cmbBoxSortType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateMarks();
        }
        #endregion

        #region Pages
        /// <summary>
        /// Деление списка студентов на страницы
        /// </summary>
        /// <param name="students">Список студентов</param>
        /// <returns>Страница из списка студентов</returns>
        private List<StudentsSuccess> GetPages(List<StudentsSuccess> marks)
        {
            int pageSize = _marksCount;

            if (cmbBoxMarksCount.SelectedIndex != 0)
            {
                pageSize = cmbBoxMarksCount.SelectedIndex * 5;
            }

            _maxPage = (int)Math.Ceiling(
                       (double)_db.Students.Count() / pageSize);

            return marks.Skip((_currentPage - 1) * pageSize).
                            Take(pageSize).
                            ToList();
        }

        /// <summary>
        /// Переход на заданную страницу
        /// </summary>
        /// <param name="page">Номер страницы</param>
        private void GoToPage(int page)
        {
            _currentPage = page;

            UpdateMarks();
        }

        private void cmbBoxMarksCount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GoToPage(1);
        }

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            GoToPage(1);
        }

        private void btnEnd_Click(object sender, RoutedEventArgs e)
        {
            GoToPage(_maxPage);
        }

        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage - 1 < 1)
            {
                return;
            }

            GoToPage(--_currentPage);
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage + 1 > _maxPage)
            {
                return;
            }

            GoToPage(++_currentPage);
        }

        private void txtBoxCurrentPage_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_currentPage >= 1 &&
                _currentPage <= _maxPage &&
                txtBoxCurrentPage.Text != string.Empty)
            {
                if (!int.TryParse(txtBoxCurrentPage.Text, out _currentPage))
                {
                    _currentPage = 1;

                    MessageBox.Show(
                        "Номер страницы введён некорректно.\n" +
                        "Осуществлено перенаправление в начало списка.",
                        "Внимание!",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                        );
                }
                else if (_currentPage > _maxPage)
                {
                    _currentPage = _maxPage;

                    MessageBox.Show(
                        "Номер страницы не найден.\n" +
                        "Осуществлено перенаправление в конец списка.",
                        "Внимание!",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                        );
                }
            }

            GoToPage(_currentPage);
        }
        #endregion
    }
}