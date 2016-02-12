using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Generics;


namespace Wipcore.Enova.Api.WebApi.Services
{
    public class SortService : ISortService
    {
        public BaseObjectList Sort(BaseObjectList objects, string sort)
        {
            if (String.IsNullOrEmpty(sort))
                return objects;

            sort = sort.Replace(',', ';');
            objects.Sort(sort); //TODO handle several properties, asc and desc
            return objects;
        }
    }
}
