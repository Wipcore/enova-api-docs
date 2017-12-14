using System;
using System.Collections.Generic;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Api.Abstractions.Internal;
using Wipcore.Enova.Core;

namespace Wipcore.eNova.Api.WebApi.Mappers.Customer
{
    public class UserCultureMapper : IPropertyMapper
    {
        public List<string> Names => new List<string>() { "Currency", "Country" };
        public Type Type => typeof(User);//covers customers and admins
        public bool InheritMapper => true;
        public int Priority => 0;
        public MapType MapType => MapType.MapFromAndToEnovaAllowed;

        public bool PostSaveSet => false;
        public bool FlattenMapping => false;
        public object GetEnovaProperty(BaseObject obj, string propertyName, List<EnovaLanguage> mappingLanguages)
        {
            var user = (User) obj;
            if (propertyName.Equals("Currency", StringComparison.InvariantCultureIgnoreCase))
                return user.Currency?.Identifier;

            return user.Country?.Identifier;
        }

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            if (value == null)
                return;

            var user = (User)obj;
            
            if (propertyName.Equals("Currency", StringComparison.InvariantCultureIgnoreCase))
                user.Currency = EnovaCurrency.Find(user.GetContext(), value.ToString());
            else
                user.Country = EnovaCountry.Find(user.GetContext(), value.ToString());
        }
    }
}
