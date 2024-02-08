using Microsoft.EntityFrameworkCore;

using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

using StudentsApp.Models;

namespace StudentsApp.Windows
{
    /// <summary>
    /// Логика взаимодействия для AddEditSubjectWindow.xaml
    /// </summary>
    public partial class AddEditSubjectWindow : Window
    {
        StudentsContext _db = new StudentsContext();

        Subject _currentSubject = new Subject(),
                _subject;

        public AddEditSubjectWindow(Subject selectedSubject)
        {
            InitializeComponent();

            _subject = selectedSubject;

            if (selectedSubject != null)
            {
                _currentSubject = selectedSubject;
            }
            else
            {
                addEditWindow.Title = "Добавление предмета";
            }

            DataContext = _currentSubject;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            #region Код предмета
            if (string.IsNullOrWhiteSpace(_currentSubject.SubjectId) ||
                string.IsNullOrEmpty(_currentSubject.SubjectId))
            {
                errors.AppendLine("Введите корректный код предмета!");
            }
            else if (_db.Subjects.Local.Select(subject => subject.SubjectId).ToList().
                        Contains(_currentSubject.SubjectId))
            {
                errors.AppendLine("Предмет с таким кодом уже существует!");
            }
            #endregion

            #region Название предмета
            if (string.IsNullOrWhiteSpace(_currentSubject.SubjectName) ||
                string.IsNullOrEmpty(_currentSubject.SubjectName))
            {
                errors.AppendLine("Введите корректное название предмета!");
            }
            else if (!Regex.IsMatch(_currentSubject.SubjectName, @"^[А-ЯA-ZЁ][а-яa-zё]+"))
            {
                errors.AppendLine("Название предмета должно начинаться с заглавной буквы!");
            }
            #endregion

            #region Количество часов
            if (string.IsNullOrWhiteSpace(_currentSubject.TotalHours.ToString()) ||
                string.IsNullOrEmpty(_currentSubject.TotalHours.ToString()))
            {
                errors.AppendLine("Введите корректное количество часов!");
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

            if (_subject != _currentSubject)
            {
                _db.Subjects.Add(_currentSubject);
            }
            else
            {
                _db.Entry(_currentSubject).State = EntityState.Modified;
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
                    ex.Message.ToString(),
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