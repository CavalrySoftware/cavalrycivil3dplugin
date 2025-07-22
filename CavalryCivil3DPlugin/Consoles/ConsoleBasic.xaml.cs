using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Remoting.Messaging;
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
using Autodesk.Gis.Map;
using DocumentFormat.OpenXml.Spreadsheet;


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

        public void UpdateText(string text_)
        {
            Text.Text = text_;
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

        public static void ShowConsole(Exception ex)
        {
            string message = $"{ex.Message}. \n{ex.ToString()}";
            ConsoleBasic consoleBasic = new ConsoleBasic(message);
        }

        public static void ShowConsole()
        {
            string message = $"I am here!";
            ConsoleBasic consoleBasic = new ConsoleBasic(message);
        }

        public static void ShowConsole(Dictionary<string, string> dictionary)
        {
            string message = "";

            foreach(var key in dictionary.Keys)
            {
                string newMessage = $"{key} : {dictionary[key]}";
                message = message + "\n" + newMessage;
            }
            ConsoleBasic consoleBasic = new ConsoleBasic(message);
        }
    }

    public class StaticConsole : Window
    {
        private string message = "";
        private ConsoleBasic consoleBasic;

        public StaticConsole ()
        {
            message = "";
            consoleBasic = new ConsoleBasic(message);
        }

        public void Print(string _message)
        {
            message = message + "\n" + _message;
            consoleBasic.UpdateText(message);
        }

        public void Print(int _message)
        {
            message = message + "\n" + _message.ToString();
            consoleBasic.UpdateText(message);
        }

        public void Print(IEnumerable collection)
        {
            foreach (var item in collection)
            {
                message = message + "\n" + item;
            }
            
            consoleBasic.UpdateText(message);
        }


    }

}
