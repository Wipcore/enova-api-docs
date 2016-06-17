using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Core;
using Wipcore.Enova.Generics;

namespace Wipcore.eNova.Api.WebApi.Mappers
{
    /// <summary>
    /// Maps country values for customer.
    /// </summary>
    public class CountryMapper : IPropertyMapper
    {
        public List<string> Names => new List<string>() { "Country", "CountryName", "CountryIsoCode" };
        public Type Type => typeof (EnovaCustomer);
        public bool InheritMapper => true;
        public int Priority => 0;
        public MapType MapType => MapType.MapTo;

        public object MapToEnovaProperty(BaseObject obj, string propertyName, object value)
        {
            var customer = (EnovaCustomer) obj;
            var country = "CountryName".Equals(propertyName, StringComparison.CurrentCultureIgnoreCase) ? 
                obj.GetContext().GetAllObjects<EnovaCountry>().FirstOrDefault(x => x.Name.Equals(value.ToString(), StringComparison.CurrentCultureIgnoreCase)) : 
                EnovaCountry.Find(obj.GetContext(), value.ToString());

            customer.Country = country;
            return value;
        }

        public object MapFromEnovaProperty(BaseObject obj, string propertyName)
        {
            throw new NotImplementedException();
        }
    }
}
