using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ToDoApp.View
{
    partial class AppDictionary : ResourceDictionary
    {
        public AppDictionary()
        {
            InitializeComponent();
        }

        private void ToDoListItemTextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var nameTextBox = sender as TextBox;
            nameTextBox.IsReadOnly = false;
        }

        private void ToDoListItemTextBox_LostKeyboardFocus(object sender, RoutedEventArgs e)
        {
            var nameTextBox = sender as TextBox;
            nameTextBox.IsReadOnly = true;
        }
    }
}
