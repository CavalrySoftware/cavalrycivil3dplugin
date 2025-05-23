using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace CavalryCivil3DPlugin.Consoles
{
    /// <summary>
    /// Interaction logic for ConsoleBasic.xaml
    /// </summary>
    public partial class ConsoleBasic : Window
    {
        public ConsoleBasic(IEnumerable collection)
        {
            InitializeComponent();
            Items.ItemsSource = collection;
            this.Show();
        }

        public ConsoleBasic(string text)
        {
            InitializeComponent();
            Text.Text = text;
            this.Show();
        }

        public ConsoleBasic(int number)
        {
            InitializeComponent();
            Text.Text = number.ToString();
            this.Show();
        }

        public ConsoleBasic(string text, IEnumerable collection)
        {
            InitializeComponent();
            Text.Text = text;
            Items.ItemsSource = collection;
            this.Show();
        }
    }


    public class _Console
    {
        public static void ShowConsole(string message)
        {
            ConsoleBasic consoleBasic = new ConsoleBasic(message);
        }

        public static void ShowConsole(IEnumerable collection)
        {
            ConsoleBasic consoleBasic = new ConsoleBasic(collection);
        }

        public static void ShowConsole(string message, IEnumerable collection)
        {
            ConsoleBasic consoleBasic = new ConsoleBasic(message, collection);
        }

        public static void ShowConsole(int number)
        {
            ConsoleBasic consoleBasic = new ConsoleBasic(number.ToString());
        }

    }

}
