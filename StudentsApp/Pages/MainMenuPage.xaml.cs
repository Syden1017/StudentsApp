using System.Windows;
using System.Windows.Controls;

namespace StudentsApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для MainMenuPage.xaml
    /// </summary>
    public partial class MainMenuPage : Page
    {
        Frame _frame;

        public MainMenuPage(Frame frmMain)
        {
            InitializeComponent();

            _frame = frmMain;
        }

        private void btnStudent_Click(object sender, RoutedEventArgs e)
        {
            _frame.Navigate(new StudentPage());
        }

        private void btnSubject_Click(object sender, RoutedEventArgs e)
        {
            _frame.Navigate(new SubjectPage());
        }

        private void btnMarks_Click(object sender, RoutedEventArgs e)
        {
            _frame.Navigate(new MarksPage());
        }

        private void btnQueries_Click(object sender, RoutedEventArgs e)
        {
            _frame.Navigate(new QueriesPage());
        }
    }
}