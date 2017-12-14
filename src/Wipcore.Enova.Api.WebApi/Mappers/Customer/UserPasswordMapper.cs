using System;
using System.Collections.Generic;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Api.Abstractions.Internal;
using Wipcore.Enova.Core;

namespace Wipcore.eNova.Api.WebApi.Mappers.Customer
{
    public class UserPasswordMapper : IPropertyMapper
    {
        public bool PostSaveSet => false;
        public List<string> Names => new List<string>() { "UserPassword" };
        public Type Type => typeof(User);//covers customers and admins
        public bool InheritMapper => true;
        public int Priority => 0;
        public MapType MapType => MapType.MapFromAndToEnovaAllowed;
        public bool FlattenMapping => false;

        public object GetEnovaProperty(BaseObject obj, string propertyName, List<EnovaLanguage> mappingLanguages)
        {
            return String.Empty; //passwords can't be retrived
        }

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            var password = value?.ToString();
            if (String.IsNullOrEmpty(password))
                return;

            var user = (User) obj;
            user.Password = password;
        }
     
    }
}
