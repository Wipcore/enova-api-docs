using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Wipcore.Core.SessionObjects;
using Wipcore.eNova.Api.WebApi.Mappers;
using Wipcore.Enova.Api.Abstractions.Internal;
using Wipcore.Enova.Api.WebApi.Helpers;
using Wipcore.Enova.Core;

namespace Wipcore.Enova.Api.WebApi.Mappers.Customer
{
    public class GroupsMapper : IPropertyMapper
    {
        public List<string> Names => new List<string>() { "Groups" };
        public Type Type => typeof(User);
        public bool InheritMapper => true;
        public int Priority => 0;
        public MapType MapType => MapType.MapFromAndToEnovaAllowed;
        public bool PostSaveSet => true;
        public bool FlattenMapping => false;

        public object GetEnovaProperty(BaseObject obj, string propertyName, List<EnovaLanguage> mappingLanguages)
        {
            var groups = new List<object>();
            var user = (User)obj;

            foreach (var enovaGroup in user.Groups.OfType<UserGroup>())
            {
                var group = new Dictionary<string, object>()
                {
                    {"ID", enovaGroup.ID},
                    {"Identifier", enovaGroup.Identifier},
                    {"Type", enovaGroup.GetType().Name},
                    {"MarkForDelete", false}
                }.MapLanguageProperty("Name", mappingLanguages, enovaGroup.GetName);

                groups.Add(group);
            }

            return groups;
        }

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            if (value == null)
                return;

            var user = (User)obj;
            var context = obj.GetContext();
            foreach (var group in value as dynamic)
            {
                var groupModel = JsonConvert.DeserializeAnonymousType(group.ToString(), new { ID = 0, Identifier = String.Empty, MarkForDelete = false });
                var enovaGroup = EnovaObjectMakerHelper.Find<UserGroup>(context, groupModel.ID, groupModel.Identifier, true);

                if (groupModel.MarkForDelete)
                    enovaGroup.RemoveUser(user);
                else if (!enovaGroup.HasUser(user))
                    enovaGroup.AddUser(user);
            }
        }


    }
}
