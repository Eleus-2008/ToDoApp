using System;
using System.Windows;
using ToDoApp.ViewModel;

namespace ToDoApp.View
{
    public partial class LoginWindow : Window
    {
        public LoginWindow(LoginViewModel viewModel)
        {
            viewModel.LoginSucceeded += LoginWindow_Succeeded;
            DataContext = viewModel;
            InitializeComponent();
        }

        private void LoginWindow_Succeeded(object sender, EventArgs e)
        {
            Close();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}