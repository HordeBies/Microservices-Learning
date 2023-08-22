﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discount.DataAccess.DbInitializers
{
    public interface IDbInitializer
    {
        Task Initialize(int retry = 0);
    }
}