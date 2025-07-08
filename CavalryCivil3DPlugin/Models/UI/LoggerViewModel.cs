using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace CavalryCivil3DPlugin.Models.UI
{
    public class LoggerViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));


        private Brush _logTextBrush;
        public Brush LogTextBrush
        {
            get => _logTextBrush;
            set
            {
                _logTextBrush = value;
                OnPropertyChanged(nameof(LogTextBrush));
            }
        }


        private string ErrorColor = "#e74c3c";
        private string ValidColor = "#138d75";


        private string _MainMessage;

        public string MainMessage
		{
			get { return _MainMessage; }
			set { _MainMessage = value; OnPropertyChanged(nameof(MainMessage)); }
		}


		public LoggerViewModel()
		{
            _logTextBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ValidColor));
		}


		public void AddLog(string message)
		{
			MainMessage = MainMessage + "\n" + message;
		}

        public void AddLog(double message)
        {
            MainMessage = MainMessage + "\n" + message.ToString();
        }


        public void ReplaceLog(string message)
		{
			MainMessage = message;
		}


        public void SetErrorMessage(string message)
        {
            LogTextBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ErrorColor));
            MainMessage = message;
        }


        public void SetLogMessage(string message)
        {
            LogTextBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ValidColor));
            MainMessage = message;
        }
	}
}
