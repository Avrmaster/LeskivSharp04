using System.Windows;

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
            DataContext = new PersonsBrowseViewModel(
                delegate() { Dispatcher.Invoke(PersonsDataGrid.Items.Refresh); },
                delegate(string s) { Dispatcher.Invoke(() => { return UserShortTextBlock.Text = s; }); });
        }
    }
}