using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LeskivSharp04
{
    /// <summary>
    /// Interaction logic for PersonRegisterWindow.xaml
    /// </summary>
    public partial class PersonRegisterWindow : Window
    {
        public PersonRegisterWindow(Action<Person> onRegisterAction)
        {
            InitializeComponent();
            DataContext = new PersonRegisterViewModel(delegate(Person person)
            {
                Close();
                onRegisterAction(person);
            });
        }
    }
}
