using System;
using System.Windows;
using System.Windows.Controls;

namespace LeskivSharp04
{
    /// <summary>
    /// Interaction logic for PersonsBrowseWindow.xaml
    /// </summary>
    public partial class PersonsBrowseWindow : Window
    {
        public PersonsBrowseWindow()
        {
            InitializeComponent();
            DataContext = new PersonsBrowseViewModel(delegate() { Dispatcher.Invoke(PersonsDataGrid.Items.Refresh); });
        }
        
        private void PersonsDataGrid_OnBeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            MessageBox.Show("Beginning editing");
        }

        private void PersonsDataGrid_OnSorting(object sender, DataGridSortingEventArgs e)
        {
            MessageBox.Show("On sorting");
        }
    }
}