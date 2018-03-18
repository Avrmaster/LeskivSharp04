using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using LeskivSharp04.Annotations;


namespace LeskivSharp04
{
    // ReSharper disable ArrangeAccessorOwnerBody
    class PersonRegisterViewModel : INotifyPropertyChanged
    {
        private readonly Window _parentWindow;

        private string _name;
        private string _surname;
        private string _email;
        private DateTime _birthDate = DateTime.Today;
        private RelayCommand _signInCommand;

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public string Surname
        {
            get
            {
                return _surname;
            }
            set
            {
                _surname = value;
                OnPropertyChanged();
            }
        }

        public string Email
        {
            get
            {
                return _email;
            }
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        public DateTime BirthDate
        {
            get
            {
                return _birthDate;
            }
            set
            {
                _birthDate = value;
                OnPropertyChanged();
            }
        }

        //this is used to keep track of empty birthday field
        public string BirthDateText { get; set; }

        public RelayCommand RegisterCommand
        {
            get
            {
                return _signInCommand ?? (_signInCommand = new RelayCommand(RegisterImpl,
                           o => !string.IsNullOrWhiteSpace(_name) &&
                                !string.IsNullOrWhiteSpace(_surname) &&
                                !string.IsNullOrWhiteSpace(_email) &&
                                !string.IsNullOrWhiteSpace(BirthDateText)
                                ));
            }
        }

        private async void RegisterImpl(object o)
        {
            Person person = null;
            //TODO block "proceed" button here

            await Task.Run((() =>
            {
                try
                {
                    person = new Person(_name, _surname, _email, _birthDate);
                }
                catch (PersonCreationException e)
                {
                    MessageBox.Show(e.Message);
                }
                
                //save to database here :)
            }));
            if (person == null)
                return;

            PersonInfoWindow personInfoWindow = new PersonInfoWindow(person);

            _parentWindow.Hide();
            personInfoWindow.Show();
        }

        internal PersonRegisterViewModel(Window parentWindow)
        {
            _parentWindow = parentWindow;
        }

        #region Implementation
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
