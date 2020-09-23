using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ToDoApp.ViewModel;

namespace ToDoApp
{
    /// <summary>
    /// Логика взаимодействия для TaskDateForm.xaml
    /// </summary>
    public partial class DateWindow : Window
    {
        public DateWindow(DateViewModel task)
        {
            InitializeComponent();

            DataContext = task;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void DatePicker_DateValidationError(object sender, DatePickerDateValidationErrorEventArgs e)
        {
            (sender as DatePicker).SelectedDate = null;
        }

    }
}
