using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Api.Abstractions.Internal;
using Wipcore.Enova.Core;

namespace Wipcore.eNova.Api.WebApi.Mappers.CompanyAndGroup
{
    public class UsersMapper : IPropertyMapper
    {
        public List<string> Names => new List<string>() { "Users" };
        public Type Type => typeof(UserGroup);
        public bool InheritMapper => true;
        public int Priority => 0;
        public MapType MapType => MapType.MapFromAndToEnovaAllowed;

        public bool PostSaveSet => false;
        public bool FlattenMapping => false;

        public object GetEnovaProperty(BaseObject obj, string propertyName, List<EnovaLanguage> mappingLanguages)
        {
            var group = (UserGroup)obj;
            var type = group is EnovaAdministratorGroup ? typeof(EnovaAdministrator) : typeof(EnovaCustomer);
            var users = group.GetUsers(type).Cast<User>().Select(x => new {x.ID, x.Identifier, x.FirstName, x.LastName, x.Alias});
            return users.ToList();
        }

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            if(value == null)
                return;

            var context = obj.GetContext();
            var group = (UserGroup)obj;
            var isAdminGroup = group is AdministratorGroup;//otherwise customergroup

            var userModels = JsonConvert.DeserializeAnonymousType(value.ToString(), new [] {new {ID = 0, Identifier = String.Empty, MarkForDelete = false} });

            foreach (var userModel in userModels)
            {
                User user;
                if(isAdminGroup)
                    user = userModel.ID > 0 ? EnovaAdministrator.Find(context, userModel.ID) : EnovaAdministrator.Find(context, userModel.Identifier);
                else
                    user = userModel.ID > 0 ? EnovaCustomer.Find(context, userModel.ID) : EnovaCustomer.Find(context, userModel.Identifier);
                
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
