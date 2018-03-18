using System.ComponentModel;
using System.Runtime.CompilerServices;
using LeskivSharp02.Annotations;

namespace LeskivSharp04
{
    // ReSharper disable ArrangeAccessorOwnerBody
    class PersonInfoViewModel : INotifyPropertyChanged
    {
        private readonly Person _person;

        public string Name => $"Your name:\n{_person.Name}";
        public string Surname => $"Your surname:\n{_person.Surname}";
        public string Email => $"Your email:\n{_person.Email}";
        public string BirthDate => $"Your birthday:\n{_person.Birthday.ToShortDateString()}";
        public string SunSign => $"Your sun sign:\n{_person.SunSign}";
        public string ChineseSign => $"Your chinese sign:\n{_person.ChineseSign}";
        public string IsBirthday => $"Today is {(_person.IsBirthday ? "" : "not ")}your birthday";
        public string IsAdult => $"You are {(_person.IsAdult? "" : "not ")}adult";

        public PersonInfoViewModel(Person person)
        {
            _person = person;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        

        #region Implementation
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
