using System.Windows;
using ToDoApp.ViewModel;

namespace ToDoApp.View
{
    public partial class LoginWindow : Window
    {
        public LoginWindow(LoginViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (((LoginViewModel)DataContext).IsSuccess)
            {
                Close();
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}