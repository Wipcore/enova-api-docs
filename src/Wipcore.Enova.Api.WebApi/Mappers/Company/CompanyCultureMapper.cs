using System;
using System.Collections.Generic;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Core;
using Wipcore.Enova.Generics;

namespace Wipcore.eNova.Api.WebApi.Mappers
{
    public class CompanyCultureMapper : IPropertyMapper
    {
        public bool PostSaveSet => false;
        public List<string> Names => new List<string>() { "Country" };
        public Type Type => typeof(EnovaCompany);
        public bool InheritMapper => true;
        public int Priority => 0;
        public MapType MapType => MapType.MapFromAndToEnovaAllowed;


        public object GetEnovaProperty(BaseObject obj, string propertyName)
        {
            var company = (EnovaCompany)obj;
            return company.CountryIsoCode;
        }

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            var company = (EnovaCompany)obj;
            var country = obj.GetContext().FindObject<EnovaCountry>(value?.ToString());
            company.CountryIsoCode = country?.Identifier;
            company.CountryName = country?.Name;
        }
    }
}
