using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using StudentsApp.Models;

namespace StudentsApp.Windows
{
    /// <summary>
    /// Логика взаимодействия для AddEditStudentWindow.xaml
    /// </summary>
    public partial class AddEditStudentWindow : Window
    {
        StudentsContext _db = new StudentsContext();

        Student _currentStudent = new Student(),
                _student;

        static string _workingDirectory = Directory.GetParent(
                                    Directory.GetParent(
                                        Directory.GetParent(
                                            Environment.CurrentDirectory).
                                                            FullName).
                                                        FullName).
                                                       FullName;

        BitmapImage _currentImage,
                    _defaultImage = new BitmapImage(new Uri(_workingDirectory +
                                                            @"\Images\student.png"));

        byte[] _imageData;

        public AddEditStudentWindow(Student selectedStudent)
        {
            InitializeComponent();

            _student = selectedStudent;

            _currentImage = _defaultImage;

            if (selectedStudent != null)
            {
                _currentStudent = selectedStudent;

                _imageData = _currentStudent.Photo;

                if (_currentStudent.Photo != null)
                {
                    ReadBitmapImageFromArray(new MemoryStream(_currentStudent.Photo), out _currentImage);
                }

                txtBoxStudentId.IsEnabled = false;
            }
            else
            {
                addEditWindow.Title = "Добавление студента";
            }

            DataContext = _currentStudent;
        }

        /// <summary>
        /// Загрузка окна
        /// </summary>
        private void addEditWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (dPicBirthDate.SelectedDate == DateTime.MinValue)
            {
                dPicBirthDate.SelectedDate = new DateTime(2000, 1, 1);
            }
        }

        #region Validation
        /// <summary>
        /// Валидация TextBox в соответствии с шаблоном
        /// </summary>
        /// <param name="textBox">TextBox для валидации</param>
        /// <param name="regexPattern">Шаблон для проверки</param>
        private static void ValidateText(TextBox textBox, string regexPattern)
        {
            if (Regex.IsMatch(textBox.Text, regexPattern))
            {
                textBox.BorderBrush = Brushes.Green;
            }
            else
            {
                textBox.BorderBrush = Brushes.Red;
            }
        }

        /// <summary>
        /// Валидация TextBox на пустую строку
        /// </summary>
        /// <param name="textBox">TextBox для валидации</param>
        private static void ValidateText(TextBox textBox)
        {
            if (!string.IsNullOrEmpty(textBox.Text) &&
                !string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.BorderBrush = Brushes.Green;
            }
            else
            {
                textBox.BorderBrush = Brushes.Red;
            }
        }

        /// <summary>
        /// Валидация DatePicker на соответствие ограничению
        /// </summary>
        /// <param name="datePicker">DatePicker для валидации</param>
        private static void ValidateText(DatePicker datePicker)
        {
            if (datePicker.SelectedDate >= new DateTime(2000, 1, 1))
            {
                datePicker.BorderBrush = Brushes.Green;
            }
            else
            {
                datePicker.BorderBrush = Brushes.Red;
            }
        }

        /// <summary>
        /// Изменение текста TextBox
        /// </summary>
        private void txtBoxStudentId_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidateText(txtBoxStudentId, @"^\d{3}-\d{2}$");
        }

        /// <summary>
        /// Изменение текста TextBox
        /// </summary>
        private void txtBoxLastName_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidateText(txtBoxLastName, @"^[А-ЯA-ZЁ][а-яa-zё]+");
        }

        /// <summary>
        /// Изменение текста TextBox
        /// </summary>
        private void txtBoxFirstName_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidateText(txtBoxFirstName, @"^[А-ЯA-ZЁ][а-яa-zё]+");
        }

        /// <summary>
        /// Изменение текста TextBox
        /// </summary>
        private void txtBoxMiddleName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtBoxMiddleName.Text != string.Empty)
            {
                ValidateText(txtBoxMiddleName, @"^[А-ЯA-ZЁ][а-яa-zё]+");
            }
            else
            {
                txtBoxMiddleName.ClearValue(BorderBrushProperty);
            }
        }

        /// <summary>
        /// Изменение даты DatePicker
        /// </summary>
        private void dPicBirthDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            ValidateText(dPicBirthDate);
        }

        /// <summary>
        /// Изменение текста TextBox
        /// </summary>
        private void txtBoxAddress_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidateText(txtBoxAddress);
        }

        /// <summary>
        /// Изменение текста TextBox
        /// </summary>
        private void txtBoxPhoneNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidateText(txtBoxPhoneNumber, @"^\+7\(9\d{3}\)\d{3}-\d{2}-\d{2}$");
        }
        #endregion

        #region Save button
        /// <summary>
        /// Кнопка "Сохранить"
        /// </summary>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            #region № студенческого билета
            if (string.IsNullOrWhiteSpace(_currentStudent.StudentId) ||
                string.IsNullOrEmpty(_currentStudent.StudentId))
            {
                errors.AppendLine("Введите корректный № студенческого билета!");
            }
            else if (_db.Students.Local.Select(student => student.StudentId).ToList().
                        Contains(_currentStudent.StudentId))
            {
                errors.AppendLine("Студент с таким № студенческого билета уже существует!");
            }
            else if (!Regex.IsMatch(_currentStudent.StudentId, @"^\d{3}-\d{2}$"))
            {
                errors.AppendLine("№ студенческого билета не соответствует шаблону \"000-00\"!");
            }
            #endregion

            #region Фамилия
            if (string.IsNullOrWhiteSpace(_currentStudent.LastName) ||
                string.IsNullOrEmpty(_currentStudent.LastName))
            {
                errors.AppendLine("Введите корректную фамилия!");
            }
            else if (!Regex.IsMatch(_currentStudent.LastName, @"^[А-ЯA-ZЁ][а-яa-zё]+"))
            {
                errors.AppendLine("Фамилия должна начинаться с заглавной буквы!");
            }
            #endregion

            #region Имя
            if (string.IsNullOrWhiteSpace(_currentStudent.FirstName) ||
                string.IsNullOrEmpty(_currentStudent.FirstName))
            {
                errors.AppendLine("Введите корректное имя!");
            }
            else if (!Regex.IsMatch(_currentStudent.FirstName, @"^[А-ЯA-ZЁ][а-яa-zё]+"))
            {
                errors.AppendLine("Имя должно начинаться с заглавной буквы!");
            }
            #endregion

            #region Отчество
            if (string.IsNullOrWhiteSpace(_currentStudent.MiddleName) ||
                string.IsNullOrEmpty(_currentStudent.MiddleName))
            {
                _currentStudent.MiddleName = "Отсутствует";
            }
            else if (!Regex.IsMatch(_currentStudent.MiddleName, @"^[А-ЯA-ZЁ][а-яa-zё]+"))
            {
                errors.AppendLine("Отчество должно начинаться с заглавной буквы!");
            }
            #endregion

            #region Дата рождения
            if (string.IsNullOrWhiteSpace(_currentStudent.BirthDate.ToString()) ||
                string.IsNullOrEmpty(_currentStudent.BirthDate.ToString()))
            {
                errors.AppendLine("Выберите дату!");
            }
            else if (_currentStudent.BirthDate.Year >= DateTime.Now.Year)
            {
                errors.AppendLine("Выберите корректную дату!");
            }
            #endregion

            #region Адрес
            if (string.IsNullOrWhiteSpace(_currentStudent.Address) ||
                string.IsNullOrEmpty(_currentStudent.Address))
            {
                errors.AppendLine("Введите адрес!");
            }
            #endregion

            #region Номер телефона
            if (_currentStudent.PhoneNumber != null)
            {
                _currentStudent.PhoneNumber = Regex.Replace(_currentStudent.PhoneNumber, @"\D", "");
                _currentStudent.PhoneNumber = _currentStudent.PhoneNumber.TrimStart('7');
            }

            if (string.IsNullOrWhiteSpace(_currentStudent.PhoneNumber) ||
                string.IsNullOrEmpty(_currentStudent.PhoneNumber))
            {
                errors.AppendLine("Номер телефона не может содержать пробел, символ табуляции и т.д.");
            }
            else if (_db.Students.Local.Select(student => student.PhoneNumber).ToList().
                        Contains(_currentStudent.PhoneNumber))
            {
                errors.AppendLine("Студент с таким номером телефона уже существует!");
            }
            else if (!Regex.IsMatch(_currentStudent.PhoneNumber, @"^9\d{9}$"))
            {
                errors.AppendLine("Номер телефона не соответствует шаблону \"9000000000\"!");
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

            if (_student != _currentStudent)
            {
                _db.Students.Add(_currentStudent);
            }
            else
            {
                _db.Entry(_currentStudent).State = EntityState.Modified;
            }

            try
            {
                _currentStudent.Photo = _imageData;

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
        #endregion

        #region Image Work
        /// <summary>
        /// Считывание изображения из массива байт
        /// </summary>
        /// <param name="byteStream">Поток на основе массива байт</param>
        /// <param name="photo">Формируемое изображение</param>
        private void ReadBitmapImageFromArray(MemoryStream byteStream, out BitmapImage image)
        {
            image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = byteStream;
            image.EndInit();
        }

        /// <summary>
        /// Загрузка изображения
        /// </summary>
        /// <param name="imageData">Массив байт для записи изображения</param>
        /// <param name="image">Загруженное изображение</param>
        private void OpenImage(ref byte[] imageData, ref BitmapImage image)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Выбор фото";
            openFileDialog.Filter = "Image files (*.jpeg;*.jpg;*.png)|*.jpeg;*.jpg;*.png|" +
                                    "Jpg files (*.jpeg;*.jpg)|*.jpeg;*.jpg|" +
                                    "Png files (*.png)|*.png";
            openFileDialog.FilterIndex = 1;

            if (openFileDialog.ShowDialog().Value)
            {
                try
                {
                    using (FileStream fileStream = new FileStream(openFileDialog.FileName, FileMode.Open))
                    {
                        imageData = new byte[fileStream.Length];
                        fileStream.Read(imageData, 0, imageData.Length);
                    }
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

                ReadBitmapImageFromArray(new MemoryStream(imageData), out image);
            }
        }

        /// <summary>
        /// Получение изображения
        /// </summary>
        /// <returns>Загруженное изображение</returns>
        private BitmapImage GetImage()
        {            
            OpenImage(ref _imageData, ref _currentImage);

            return _currentImage;
        }

        /// <summary>
        /// Обработка нажатия правой кнопки мыши на Image
        /// </summary>
        private void imgPhoto_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            imgPhoto.Source = _defaultImage;
            _imageData = null;
        }

        /// <summary>
        /// Обработка нажатия левой кнопки мыши на Image
        /// </summary>
        private void imgPhoto_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            BitmapImage photo = GetImage();

            if (photo != null)
            {
                imgPhoto.Source = photo;
            }
        }
        #endregion

        /// <summary>
        /// Кнопка "Отмена"
        /// </summary>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}