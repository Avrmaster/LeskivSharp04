using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
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

        private static CollectionView _sortFilterOptionsCollection;

        public static CollectionView SortFilterOptions => _sortFilterOptionsCollection ??
                                                          (_sortFilterOptionsCollection =
                                                              new CollectionView(SortExtension.SortFiltertOptions));

        private string _filterQuery;

        public string SelectedSoftFilterProperty { get; set; }

        public string FilterQuery
        {
            get => _filterQuery;
            set
            {
                _filterQuery = value;
                SelectedPerson = null;
                _updateUsersGrid();
            }
        }

        private RelayCommand _sortCommand;
        private RelayCommand _clearFilterCommand;

        private List<Person> _personsList;

        public List<Person> PersonsListToShow =>
            (string.IsNullOrEmpty(SelectedSoftFilterProperty) || string.IsNullOrEmpty(FilterQuery))
                ? _personsList
                : _personsList.FilterBy(SelectedSoftFilterProperty, FilterQuery);

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
                _personsList.Remove(SelectedPerson);
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
                PersonsListToShow.Add(newPerson);
                _updateUsersGrid();
            });
            registerWindow.Show();
        }

        public RelayCommand SortCommand =>
            _sortCommand ?? (_sortCommand =
                new RelayCommand(SortImpl, o => !string.IsNullOrEmpty(SelectedSoftFilterProperty)));

        private async void SortImpl(object o)
        {
            await Task.Run(() =>
            {
                _personsList = _personsList.SortBy(SelectedSoftFilterProperty);
                _updateUsersGrid();
            });
        }

        public RelayCommand ClearFilterCommand =>
            _clearFilterCommand ?? (_clearFilterCommand = new RelayCommand((o) =>
                {
                    FilterQuery = "";
                    OnPropertyChanged($"FilterQuery");
                },
                o => !string.IsNullOrEmpty(FilterQuery)));

        public PersonsBrowseViewModel(Action updateGrid, Action<string> updateUserInfo)
        {
            _personsList = new List<Person>();
            Person.LoadAllInto(PersonsListToShow, updateGrid);

            _updateUsersGrid = () =>
            {
                Person.SaveAll(_personsList);
                OnPropertyChanged($"PersonsListToShow");
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