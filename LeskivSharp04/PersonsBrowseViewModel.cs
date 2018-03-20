using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Data;
using LeskivSharp04.Annotations;

namespace LeskivSharp04
{
    class PersonsBrowseViewModel : INotifyPropertyChanged
    {        
        private List<Person> _personsList;
        private Person _selectedPerson;
        private readonly Action _refreshPersonsAction;
        private string _filterQuery;
        private bool _sortingAsc = true;

        private RelayCommand _deleteCommand;
        private RelayCommand _editCommand;
        private RelayCommand _registerCommand;
        private RelayCommand _sortCommand;
        private RelayCommand _clearFilterCommand;

        private static CollectionView _sortFilterOptionsCollection;

        public string SelectedSoftFilterProperty { get; set; }

        public string SelectedPersonShort { get; private set; }
        
        public static CollectionView SortFilterOptions => _sortFilterOptionsCollection ??
                                                          (_sortFilterOptionsCollection =
                                                              new CollectionView(SortExtension.SortFiltertOptions));

        public string FilterQuery
        {
            get => _filterQuery;
            set
            {
                _filterQuery = value;
                SelectedPerson = null;
                UpdateUsersGrid();
            }
        }

        public List<Person> PersonsListToShow =>
            (string.IsNullOrEmpty(SelectedSoftFilterProperty) || string.IsNullOrEmpty(FilterQuery))
                ? _personsList
                : _personsList.FilterByProperty(SelectedSoftFilterProperty, FilterQuery);

        public Person SelectedPerson
        {
            get => _selectedPerson;
            set
            {
                _selectedPerson = value;
                if (_selectedPerson == null) return;
                SelectedPersonShort = $"{_selectedPerson.Name} {_selectedPerson.Surname}";
                OnPropertyChanged($"SelectedPersonShort");
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
                UpdateUsersGrid();
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
                UpdateUsersGrid();
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
                UpdateUsersGrid();
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
                _personsList = _personsList.SortByProperty(SelectedSoftFilterProperty, _sortingAsc);
                _sortingAsc = !_sortingAsc;
                UpdateUsersGrid();
            });
        }

        public RelayCommand ClearFilterCommand =>
            _clearFilterCommand ?? (_clearFilterCommand = new RelayCommand((o) =>
                {
                    FilterQuery = "";
                    OnPropertyChanged($"FilterQuery");
                },
                o => !string.IsNullOrEmpty(FilterQuery)));

        private void UpdateUsersGrid()
        {
            Person.SaveAll(_personsList);
            OnPropertyChanged($"PersonsListToShow");
            _refreshPersonsAction();
        }

        public PersonsBrowseViewModel(Action updateGridItems)
        {
            _refreshPersonsAction = updateGridItems;
            _personsList = new List<Person>();
            Person.LoadAllInto(PersonsListToShow, UpdateUsersGrid);
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