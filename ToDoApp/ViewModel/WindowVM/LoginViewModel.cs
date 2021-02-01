using System;
using System.ComponentModel;
using System.Net.Http;
using System.Runtime.CompilerServices;
using ToDoApp.Annotations;
using ToDoApp.Model;
using ToDoApp.Model.Interfaces;

namespace ToDoApp.ViewModel
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly IAuthentication _authentication;
        
        private string _username;
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }
        
        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }
        public LoginViewModel(IAuthentication authentication)
        {
            _authentication = authentication;
        }

        private AsyncRelayCommand<object> _loginCommand;

        public AsyncRelayCommand<object> LoginCommand
        {
            get
            {
                return _loginCommand ??
                       (_loginCommand = new AsyncRelayCommand<object>(async obj =>
                       {
                           try
                           {
                               var result = await _authentication.Login(Username, Password);
                               if (result.isSuccess)
                               {
                                   OnLoginSucceeded();
                               }
                           }
                           catch
                           {
                               // ignored
                           }
                       }));
            }
        }

        public event EventHandler LoginSucceeded;
        
        protected virtual void OnLoginSucceeded()
        {
            LoginSucceeded?.Invoke(this, EventArgs.Empty);
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}