using System;
using System.ComponentModel;
using System.Net.Http;
using System.Runtime.CompilerServices;
using ToDoApp.Annotations;
using ToDoApp.Model;
using ToDoApp.Model.Interfaces;

namespace ToDoApp.ViewModel
{
    public class RegisterViewModel : INotifyPropertyChanged
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

        private string _email;
        public string Email
        {
            get => _email;
            set
            {
                _email = value;
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
        
        public bool IsSuccess { get; private set; }

        public RegisterViewModel(IAuthentication authentication)
        {
            _authentication = authentication;
        }

        private AsyncRelayCommand<object> _registerCommand;

        public AsyncRelayCommand<object> RegisterCommand
        {
            get
            {
                return _registerCommand ??
                       (_registerCommand = new AsyncRelayCommand<object>(async obj =>
                       {
                           try
                           {
                               var result = await _authentication.Register(Username, Email, Password);
                               IsSuccess = result;
                           }
                           catch
                           {
                               IsSuccess = false;
                           }
                       }));
            }
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}