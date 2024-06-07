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

        public static readonly string AddCommandHelp = "[USAGE]~ add [type] [id] [name]\nType => 0 - Interval Meter, 1 - Smart meter\n";
        public static readonly string DeleteCommandHelp = "[USAGE]~ delete [id]\n";
        public static readonly string NavCommandHelp = "[USAGE]~ nav [tab]\nTab => 0 - Network Entities, 1 - Network Display, 2 - Measurement Graph\n";
        public static readonly string FilterOffCommandHelp = "[USAGE]~ filteroff\n";
        public static readonly string FilterOnCommandHelp = "[USAGE]~ filter [type] [(<|>) | =] [id]\nType => -1 No type, 0 Interval Meter, 1 Smart Meter (this is optional)\nId is optional (if <, >, or = is not selected)\n";
        public Terminal()
        {
            TerminalContent = $"New terminal started at {DateTime.Now:yyyy/MM/dd HH:mm:ss}.\n";
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
                OnPropertyChanged(nameof(ConsoleContent));
            }
        }
        public bool ConsumeTerminalCommand()
        {
            if (consoleContent.Trim().Length > 0)
            {
                TerminalContent += $"admin@root:~$ {ConsoleContent}\n";
                return true;
            }
            return false;
        }
        public void ClearTerminalConsole()
        {
            ConsoleContent = string.Empty;
        }
    }
}
