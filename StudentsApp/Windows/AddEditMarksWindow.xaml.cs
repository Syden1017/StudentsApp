using StudentsApp.Models;
using System.Windows;

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

        private void addEditWindow_Loaded(object sender, RoutedEventArgs e)
        {

        }

        public AddEditMarksWindow(StudentsSuccess selectedmark)
        {
            InitializeComponent();

            _mark = selectedmark;

            if (selectedmark != null) 
            {
                _currentMark = selectedmark;


            }
            else
            {
                addEditWindow.Title = "Добавление оценки";
            }
            DataContext = _currentMark;
        }
    }
}