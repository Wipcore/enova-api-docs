using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wipcore.Enova.Api.Interfaces
{
    public interface ILocationService
    {
        IDictionary<string, string> GetParametersFromLocationConfiguration(string type, string location);
    }

    //public class LocationConfiguration
    //{
    //    string Name { get; set; }


    //}
}
