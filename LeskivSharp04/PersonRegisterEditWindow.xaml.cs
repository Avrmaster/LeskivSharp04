using System;
using System.Windows;

namespace LeskivSharp04
{
    /// <summary>
    /// Interaction logic for PersonRegisterEditWindow.xaml
    /// </summary>
    public partial class PersonRegisterEditWindow : Window
    {
        public PersonRegisterEditWindow(Action<Person> onRegisterAction, Person person = null)
        {
            InitializeComponent();
            DataContext = new PersonRegisterEditViewModel(person, delegate(Person newPerson)
            {
                Close();
                onRegisterAction(newPerson);
            });
        }
    }
}
