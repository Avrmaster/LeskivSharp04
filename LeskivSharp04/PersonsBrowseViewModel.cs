using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using LeskivSharp04.Annotations;

namespace LeskivSharp04
{
    class PersonsBrowseViewModel : INotifyPropertyChanged
    {
        public List<Person> PersonsList { get; }

        public PersonsBrowseViewModel(Action updateGrid)
        {
            PersonsList = new List<Person>();
            Person.LoadAllInto(PersonsList, updateGrid);
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