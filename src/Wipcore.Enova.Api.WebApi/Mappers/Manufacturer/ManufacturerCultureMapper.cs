using System;
using System.Collections.Generic;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Core;
using Wipcore.Enova.Generics;

namespace Wipcore.eNova.Api.WebApi.Mappers.Manufacturer
{
    public class ManufacturerCultureMapper : IPropertyMapper
    {
        public List<string> Names => new List<string>() { "Country" };
        public Type Type => typeof(EnovaManufacturer);
        public bool InheritMapper => true;
        public int Priority => 0;
        public MapType MapType => MapType.MapFromAndToEnovaAllowed;
        public bool PostSaveSet => false;
        public bool FlattenMapping => false;
        public object GetEnovaProperty(BaseObject obj, string propertyName, List<EnovaLanguage> mappingLanguages)
        {
            var manufacturer = (EnovaManufacturer)obj;
            return manufacturer.CountryIsoCode;
        }

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            if(String.IsNullOrEmpty(value?.ToString()))
                return;

            var manufacturer = (EnovaManufacturer)obj;
            var country = EnovaCountry.Find(obj.GetContext(), value.ToString());
            
            manufacturer.CountryIsoCode = country.Identifier;
            manufacturer.CountryName = country.Name;
        }

    }
}
