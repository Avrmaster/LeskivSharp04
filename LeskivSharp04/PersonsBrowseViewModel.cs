using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using LeskivSharp04.Annotations;

namespace LeskivSharp04
{
    class PersonsBrowseViewModel : INotifyPropertyChanged
    {
        private Person _selectedPerson;
        private readonly Action _updateUsersGrid;
        private readonly Action<string> _updateUserInfoInfoAction;
        private RelayCommand _deleteCommand;
        private RelayCommand _editCommand;
        private RelayCommand _registerCommand;

        private RelayCommand _sortCommand;
        private RelayCommand _filterCommand;

        public List<Person> PersonsList { get; }

        public Person SelectedPerson
        {
            get => _selectedPerson;
            set
            {
                _selectedPerson = value;
                if (_selectedPerson != null)
                    _updateUserInfoInfoAction($"{_selectedPerson.Name} {_selectedPerson.Surname}");
            }
        }

        public RelayCommand DeleteCommand =>
            _deleteCommand ?? (_deleteCommand = new RelayCommand(DeleteImpl, o => _selectedPerson != null));

        private async void DeleteImpl(object o)
        {
            await Task.Run((() =>
            {
                //since it's exactly the same object
                PersonsList.Remove(SelectedPerson);
                _updateUsersGrid();
            }));
        }

        public RelayCommand EditCommand =>
            _editCommand ?? (_editCommand = new RelayCommand(EditImpl, o => _selectedPerson != null));

        private void EditImpl(object o)
        {
            var personToEdit = _selectedPerson;
            var editWindow = new PersonRegisterEditWindow(delegate(Person edited)
            {
                personToEdit.CopyFrom(edited);
                _updateUsersGrid();
            }, _selectedPerson);
            editWindow.Show();
        }

        public RelayCommand RegisterCommand =>
            _registerCommand ?? (_registerCommand = new RelayCommand(RegisterImpl, o => true));

        private void RegisterImpl(object o)
        {
            var registerWindow = new PersonRegisterEditWindow(delegate(Person newPerson)
            {
                PersonsList.Add(newPerson);
                _updateUsersGrid();
            });
            registerWindow.Show();
        }

        public PersonsBrowseViewModel(Action updateGrid, Action<string> updateUserInfo)
        {
            PersonsList = new List<Person>();
            Person.LoadAllInto(PersonsList, updateGrid);

            _updateUsersGrid = () =>
            {
                Person.SaveAll(PersonsList);
                updateGrid();
            };
            _updateUserInfoInfoAction = updateUserInfo;
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