using NetworkService.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.ViewModel
{
    public class StartingViewModel : BindableBase
    {
        private string applicationTitle;
        private string applicationSubtitle;

        public StartingViewModel()
        {
            applicationTitle = "Monitoring App";
            applicationSubtitle = "Fast and easy way to monitor energy consumption";
        }
        public string ApplicationTitle
        {
            get
            {
                return applicationTitle;
            }
            set
            {
                applicationTitle = value;
                OnPropertyChanged("ApplicationTitle");
            }
        }
        public string ApplicationSubtitle
        {
            get
            {
                return applicationSubtitle;
            }
            set
            {
                applicationSubtitle = value;
                OnPropertyChanged("ApplicationSubtitle");
            }
        }
    }
}
