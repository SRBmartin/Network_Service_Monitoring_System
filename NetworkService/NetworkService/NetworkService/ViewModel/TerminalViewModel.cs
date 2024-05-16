using NetworkService.Helpers;
using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.ViewModel
{
    public class TerminalViewModel : BindableBase
    {
        private Terminal terminal;
        public TerminalViewModel()
        {
            terminal = new Terminal();
        }

        public Terminal Terminal
        {
            get
            {
                return terminal;
            }
            set
            {
                SetProperty(ref terminal, value);
            }
        }
    }
}
