using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using NetworkService.Helpers.Interface;

namespace NetworkService.Helpers.Common
{
    public class ConfirmationService : IConfirmationService
    {
        public bool Confirm(string ConfirmationContent)
        {
            return MessageBox.Show(ConfirmationContent, "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
        }
    }
}
