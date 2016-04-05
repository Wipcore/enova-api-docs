﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Enova.Api.Models.Interfaces;

namespace Wipcore.Enova.Api.Interfaces
{
    public interface ILocationService
    {
        IGetParametersModel GetParametersFromLocationConfiguration(Type type, IGetParametersModel parameters);
    }
}
