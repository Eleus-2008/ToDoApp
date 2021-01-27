using System.Windows;
using ToDoApp.ViewModel;

namespace ToDoApp.View
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow(RegisterViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            if (((RegisterViewModel)DataContext).IsSuccess)
            {
                Close();
            }
        }
    }
}