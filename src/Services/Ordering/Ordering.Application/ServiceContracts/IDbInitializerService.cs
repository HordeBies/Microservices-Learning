﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Application.ServiceContracts
{
    public interface IDbInitializerService
    {
        Task Initialize(int retry = 0);
    }
}