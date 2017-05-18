using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Core;

namespace Wipcore.eNova.Api.WebApi.Mappers.CompanyAndGroup
{
    public class CompanyUsersMapper : IPropertyMapper
    {
        public List<string> Names => new List<string>() { "Users" };
        public Type Type => typeof(CustomerGroup);
        public bool InheritMapper => true;
        public int Priority => 0;
        public MapType MapType => MapType.MapFromAndToEnovaAllowed;

        public bool PostSaveSet => false;
        public bool FlattenMapping => false;

        public object GetEnovaProperty(BaseObject obj, string propertyName, List<EnovaLanguage> mappingLanguages)
        {
            var group = (CustomerGroup)obj;
            var users = group.GetUsers(typeof (EnovaCustomer)).Cast<EnovaCustomer>().Select(x => new {x.ID, x.Identifier, x.FirstName, x.LastName, x.Alias});
            return users.ToList();
        }

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            if(value == null)
                return;

            var context = obj.GetContext();
            var group = (CustomerGroup)obj;
            var userModels = JsonConvert.DeserializeAnonymousType(value.ToString(), new [] {new {ID = 0, MarkForDelete = false} });

            foreach (var userModel in userModels)
            {
                var user = EnovaCustomer.Find(context, userModel.ID);
                if (userModel.MarkForDelete)
                {
                    if(group.HasUser(user))
                        group.RemoveUser(user);
                }
                else
                {
                    if (!group.HasUser(user))
                        group.AddUser(user);
                }
            }
        }
    }
}
