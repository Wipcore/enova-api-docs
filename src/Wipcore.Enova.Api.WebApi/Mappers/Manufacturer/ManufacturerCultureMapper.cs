using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Core;

namespace Wipcore.eNova.Api.WebApi.Mappers.Manufacturer
{
    public class ManufacturerCultureMapper : IPropertyMapper
    {
        public List<string> Names => new List<string>() { "Country" };
        public Type Type => typeof(EnovaManufacturer);
        public bool InheritMapper => true;
        public int Priority => 0;
        public MapType MapType => MapType.MapFromAndToEnovaAllowed;


        public object GetEnovaProperty(BaseObject obj, string propertyName)
        {
            var manufacturer = (EnovaManufacturer)obj;
            return manufacturer.CountryIsoCode;
        }

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            var manufacturer = (EnovaManufacturer)obj;
            var country = EnovaCountry.Find(manufacturer.GetContext(), value.ToString());
            manufacturer.CountryIsoCode = country.Identifier;
            manufacturer.CountryName = country.Name;
        }

    }
}
