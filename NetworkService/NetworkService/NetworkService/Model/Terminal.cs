using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using NetworkService.Helpers;

namespace NetworkService.Model
{
    public class Terminal : BindableBase
    {
        private string terminalContent;
        private string consoleContent;

        public Terminal()
        {
            TerminalContent = $"New terminal started at {DateTime.Now:yyyy/MM/dd hh:mm:ss}.\n";
            ConsoleContent = string.Empty;
        }
        public string TerminalContent
        {
            get { return terminalContent; }
            set
            {
                terminalContent = value;
                OnPropertyChanged("TerminalContent");
            }
        }
        public string ConsoleContent
        {
            get { return consoleContent; }
            set
            {
                consoleContent = value;
                OnPropertyChanged("ConsoleContent");
            }
        }
    }
}
