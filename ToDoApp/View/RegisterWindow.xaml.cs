using System;
using System.Windows;
using ToDoApp.ViewModel;

namespace ToDoApp.View
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow(RegisterViewModel viewModel)
        {
            viewModel.RegisterSucceeded += RegisterWindow_Succeeded;
            DataContext = viewModel;
            InitializeComponent();
        }

        private void RegisterWindow_Succeeded(object sender, EventArgs e)
        {
            Close();
        }
        
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}