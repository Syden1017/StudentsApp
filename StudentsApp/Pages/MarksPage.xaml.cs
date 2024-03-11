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
        const int FILTER_BY_MARK = 0;

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

        #region Search
        private void txtBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        #endregion

        #region Filter
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

        private void LoadMarkInComboBox(ComboBox filterType)
        {
            List<string> mark = _db.StudentsSuccesses.Select(s => s.Evaluation.ToString()).ToList();

            mark = mark.Distinct().OrderBy(y => y).ToList();

            ClearComboBox(filterType);

            mark.Insert(0, "Все оценки");
            filterType.ItemsSource = mark;

            filterType.SelectedIndex = 0;
        }

        private List<StudentsSuccess> FilterMark(List<StudentsSuccess> grades, int filterField, int mark)
        {
            if (mark != 0)
            {
                switch (filterField)
                {
                    
                    default:
                        break;
                }
            }
        }

        private void cmbBoxFilterField_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cmbBoxFilterType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

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