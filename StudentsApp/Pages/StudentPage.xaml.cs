using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using StudentsApp.Models;
using StudentsApp.Windows;

namespace StudentsApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для StudentPage.xaml
    /// </summary>
    public partial class StudentPage : Page
    {
        // Поле фильтрации
        const int FILTER_BY_ADMISSION_YEAR = 1;     // Фильтр по году поступления
        const int FILTER_BY_BIRTH_YEAR = 2;     // Фильтр по году рождения

        // Поле сортировки
        const int SORT_BY_STUDENT_ID = 0,        // Сортировка по № студенческого билета
                  SORT_BY_LAST_NAME = 1,        // Сортировка по фамилии
                  SORT_BY_FIRST_NAME = 2,        // Соортировка по имени
                  SORT_BY_MIDDLE_NAME = 3,        // Сортировка по отчеству
                  SORT_BY_BIRTH_DATE = 4;        // Сортировка по дате рождения

        // Тип сортировки
        const int ASC_SORT = 0;    // Сортировка по возрастанию

        StudentsContext _db = new StudentsContext();
        List<Student> _students = new List<Student>();
        int _studentCount = 0;

        // Распределение по страницам
        int _currentPage = 1,
            _maxPage = 0;

        public StudentPage()
        {
            InitializeComponent();

            cmbBoxFilterField.SelectedIndex = 0;
            cmbBoxFilterType.SelectedIndex = 0;

            cmbBoxSortField.SelectedIndex = 0;
            cmbBoxSortType.SelectedIndex = 0;

            cmbBoxStudentCount.SelectedIndex = 0;

            UpdateStudentList();
        }

        #region CRUD operations
        /// <summary>
        /// Обновление списка студентов на основе сортировки, поиска, фильтрации и деления на страницы
        /// </summary>
        private void UpdateStudentList()
        {
            _db.Students.Load();
            _students = _db.Students.ToList();
            _studentCount = _students.Count;

            string request = txtBoxSearch.Text.
                                          Replace(" ", "").
                                          ToLower();

            int year = 0;

            if (cmbBoxFilterType.SelectedIndex > 0)
            {
                int.TryParse(cmbBoxFilterType.SelectedValue.ToString(), out year);
            }

            // Список формировать в порядке
            // сортировка -> поиск -> фильтрация -> деление на страницы
            int filterField = cmbBoxFilterField.SelectedIndex,
                sortField = cmbBoxSortField.SelectedIndex,
                sortType = cmbBoxSortType.SelectedIndex;

            List<Student> studentList = SortStudents(
                                            SearchStudents(
                                                FilterStudents(
                                                    GetPages(_students),
                                                                filterField,
                                                                year),
                                                          request),
                                                     sortField,
                                                     sortType);

            txtBoxCurrentPage.Text = _currentPage.ToString();
            txtBoxCurrentPage.MaxLength = _maxPage.ToString().Length;
            txtBoxTotalPage.Text = _maxPage.ToString();

            lViewStudents.ItemsSource = studentList;

            txtBlockStudentCount.Text = _studentCount.ToString();
        }

        /// <summary>
        /// Вывод окна добавления / редактирования студентов
        /// </summary>
        /// <param name="student">Информация о студенте</param>
        private void ShowAddOrEditWindows(Student student)
        {
            AddEditStudentWindow addEditStudentWindow = new AddEditStudentWindow(student);
            addEditStudentWindow.ShowDialog();

            UpdateStudentList();
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

            if (result == MessageBoxResult.Yes)
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

            UpdateStudentList();
        }

        private void btnMarks_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region Search
        /// <summary>
        /// Поиск студента по ФИО
        /// </summary>
        /// <param name="students">Список студентов для поиска</param>
        /// <param name="request">Поисковый запрос</param>
        /// <returns>Результаты поиска</returns>
        private List<Student> SearchStudents(List<Student> students, string request)
        {
            return students.Where(s => (s.LastName +
                                        s.FirstName +
                                        s.MiddleName).
                                             ToLower().
                                             Contains(request)).
                                             ToList();
        }

        /// <summary>
        /// Изменение поискового запроса
        /// </summary>
        private void txtBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateStudentList();
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

            ClearComboBox(filterType);

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

            ClearComboBox(filterType);

            filterType.ItemsSource = birthYears;
            filterType.SelectedIndex = 0;
        }

        /// <summary>
        /// Фильтрация списка студентов по году поступления / году рождения
        /// </summary>
        /// <param name="students">Список студентов для фильтрации</param>
        /// <param name="filterField">Номер поля для фильтрации</param>
        /// <param name="year">Значение года фильтрации</param>
        /// <returns>Результаты фильтрации</returns>
        private List<Student> FilterStudents(List<Student> students, int filterField, int year)
        {
            if (year != 0)
            {
                switch (filterField)
                {
                    case FILTER_BY_ADMISSION_YEAR:
                        students = students.Where(s => s.StudentId.Contains("-" + year.ToString().
                                                                                       Remove(0, 2))).ToList();

                        break;

                    case FILTER_BY_BIRTH_YEAR:
                        students = students.Where(s => s.BirthDate.Year == year).ToList();

                        break;

                    default:
                        break;
                }
            }

            return students;
        }

        /// <summary>
        /// Выбор поля для фильтрации
        /// </summary>
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
                    ClearComboBox(cmbBoxFilterType);
                    cmbBoxFilterType.Items.Add("Не задано");
                    cmbBoxFilterType.SelectedIndex = 0;
                    break;
            }
        }

        /// <summary>
        /// Выбор типа значения фильтрации
        /// </summary>
        private void cmbBoxFilterType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateStudentList();
        }
        #endregion

        #region Sort
        /// <summary>
        /// Сортировка списка студентов
        /// </summary>
        /// <param name="students">Список студентов для сортировки</param>
        /// <param name="sortField">Номер поля сортировки</param>
        /// <param name="sortType">Номер типа сортировки</param>
        /// <returns>Отсортированный список</returns>
        private List<Student> SortStudents(List<Student> students, int sortField, int sortType)
        {
            Func<Student, object> sortExpression;

            switch (sortField)
            {
                case SORT_BY_STUDENT_ID:
                    sortExpression = s => s.StudentId;
                    break;

                case SORT_BY_LAST_NAME:
                    sortExpression = s => s.LastName;
                    break;

                case SORT_BY_FIRST_NAME:
                    sortExpression = s => s.FirstName;
                    break;

                case SORT_BY_MIDDLE_NAME:
                    sortExpression = s => s.MiddleName;
                    break;

                case SORT_BY_BIRTH_DATE:
                    sortExpression = s => s.BirthDate;
                    break;

                default:
                    sortExpression = s => s.StudentId;
                    break;
            }

            return sortType == ASC_SORT ? students.OrderBy(sortExpression).ToList() :
                                          students.OrderByDescending(sortExpression).ToList();
        }

        /// <summary>
        /// Выбор поля для сортировки
        /// </summary>
        private void cmbBoxSortField_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateStudentList();
        }

        /// <summary>
        /// Выбор типа сортировки
        /// </summary>
        private void cmbBoxSortType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateStudentList();
        }
        #endregion

        #region Pages
        /// <summary>
        /// Деление списка студентов на страницы
        /// </summary>
        /// <param name="students">Список студентов</param>
        /// <returns>Страница из списка студентов</returns>
        private List<Student> GetPages(List<Student> students)
        {
            int pageSize = _studentCount;

            if (cmbBoxStudentCount.SelectedIndex != 0)
            {
                pageSize = cmbBoxStudentCount.SelectedIndex * 4;
            }

            _maxPage = (int)Math.Ceiling(
                       (double)_db.Students.Count() / pageSize);

            return students.Skip((_currentPage - 1) * pageSize).
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

            UpdateStudentList();
        }

        /// <summary>
        /// Выбор количества студентов на странице
        /// </summary>
        private void cmbBoxStudentCount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GoToPage(1);
        }

        /// <summary>
        /// Переход на первую страницу
        /// </summary>
        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            GoToPage(1);
        }

        /// <summary>
        /// Переход на последнюю страницу
        /// </summary>
        private void btnEnd_Click(object sender, RoutedEventArgs e)
        {
            GoToPage(_maxPage);
        }

        /// <summary>
        /// Переход на предыдущую страницу
        /// </summary>
        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage - 1 < 1)
            {
                return;
            }

            GoToPage(--_currentPage);
        }

        /// <summary>
        /// Переход на следующую страницу
        /// </summary>
        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage + 1 > _maxPage)
            {
                return;
            }

            GoToPage(++_currentPage);
        }

        /// <summary>
        /// Изменение номера текущей страницы
        /// </summary>
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