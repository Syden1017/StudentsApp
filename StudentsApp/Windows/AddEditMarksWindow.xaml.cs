using Microsoft.EntityFrameworkCore;

using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Text.RegularExpressions;

using StudentsApp.Models;

namespace StudentsApp.Windows
{
    /// <summary>
    /// Логика взаимодействия для AddEditMarksWindow.xaml
    /// </summary>
    public partial class AddEditMarksWindow : Window
    {
        StudentsContext _db = new StudentsContext();

        StudentsSuccess _currentMark = new StudentsSuccess(),
                        _mark;

        public AddEditMarksWindow(StudentsSuccess selectedmark)
        {
            InitializeComponent();

            _mark = selectedmark;

            if (selectedmark != null) 
            {
                _currentMark = selectedmark;

                txtBoxStudentId.IsEnabled = false;
            }
            else
            {
                addEditWindow.Title = "Добавление оценки";
            }
            DataContext = _currentMark;
        }

        private void addEditWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (dPicExamDate.SelectedDate == DateTime.MinValue)
            {
                dPicExamDate.SelectedDate = new DateTime(2000, 1, 1);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            #region № студенческого билета
            if (string.IsNullOrWhiteSpace(_currentMark.StudentId) ||
                string.IsNullOrEmpty(_currentMark.StudentId))
            {
                errors.AppendLine("Введите корректный № студенческого билета!");
            }
            else if (!_db.StudentsSuccesses.Local.Select(mark => mark.StudentId).ToList().
                        Contains(_currentMark.StudentId))
            {
                errors.AppendLine("Такого студента не существует!");
            }
            else if (!Regex.IsMatch(_currentMark.StudentId, @"^\d{3}-\d{2}$"))
            {
                errors.AppendLine("№ студенческого билета не соответствует шаблону \"000-00\"!");
            }
            #endregion

            #region Предмет
            if (string.IsNullOrWhiteSpace(_currentMark.SubjectId) ||
                string.IsNullOrEmpty(_currentMark.SubjectId))
            {
                errors.AppendLine("Введите корректный код предмета!");
            }
            else if (_db.StudentsSuccesses.Local.Select(mark => mark.SubjectId).ToList().
                        Contains(_currentMark.SubjectId))
            {
                errors.AppendLine("Такого предмета не существует!");
            }
            #endregion

            #region Оценка
            else if (Convert.ToInt32(_currentMark.Evaluation) < 1 &&
                     Convert.ToInt32(_currentMark.Evaluation) > 5)
            {
                errors.AppendLine("Введите оценку по русской системе исчисления!");
            }
            #endregion

            #region Дата экзамена
            if (string.IsNullOrWhiteSpace(_currentMark.ExamDate.ToString()) ||
                string.IsNullOrEmpty(_currentMark.ExamDate.ToString()))
            {
                errors.AppendLine("Выберите дату!");
            }
            else if (_currentMark.ExamDate.Year >= DateTime.Now.Year)
            {
                errors.AppendLine("Выберите корректную дату!");
            }
            #endregion

            if (errors.Length > 0)
            {
                MessageBox.Show(
                    errors.ToString(),
                    "Заполните поля",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                    );

                return;
            }

            if (_mark != _currentMark)
            {
                _db.StudentsSuccesses.Add(_mark);
            }
            else
            {
                _db.Entry(_currentMark).State = EntityState.Modified;
            }

            try
            {
                _db.SaveChanges();

                MessageBox.Show(
                    "Информация сохранена",
                    "Сохранение",
                     MessageBoxButton.OK,
                     MessageBoxImage.Information
                    );

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Системная ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                    );
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}