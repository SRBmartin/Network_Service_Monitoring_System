﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Helpers.Interface
{
    public interface IConfirmationService
    {
        bool Confirm(string ConfirmationContent);
    }
}
