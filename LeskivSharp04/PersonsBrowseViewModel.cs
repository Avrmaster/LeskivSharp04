using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using LeskivSharp04.Annotations;

namespace LeskivSharp04
{
    class PersonsBrowseViewModel : INotifyPropertyChanged
    {
        public readonly List<Person> PersonsList;

        public PersonsBrowseViewModel()
        {
            PersonsList = new List<Person>();
            Person.LoadAllInto(PersonsList);
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