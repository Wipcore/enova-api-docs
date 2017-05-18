using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Api.WebApi.Helpers;
using Wipcore.Enova.Core;

namespace Wipcore.eNova.Api.WebApi.Mappers.Customer
{
    public class CustomerGroupsMapper : IPropertyMapper
    {
        public List<string> Names => new List<string>() { "Groups" };
        public Type Type => typeof(EnovaCustomer);
        public bool InheritMapper => true;
        public int Priority => 0;
        public MapType MapType => MapType.MapFromAndToEnovaAllowed;
        public bool PostSaveSet => true;
        public bool FlattenMapping => false;

        public object GetEnovaProperty(BaseObject obj, string propertyName, List<EnovaLanguage> mappingLanguages)
        {
            var groups = new List<object>();
            var customer = (EnovaCustomer) obj;

            foreach (var group in customer.Groups.OfType<CustomerGroup>())
            {
                var customerGroup = new Dictionary<string, object>()
                {
                    {"ID", group.ID},
                    {"Identifier", group.Identifier},
                    {"Type", group.GetType().Name},
                    {"MarkForDelete", false}
                }.MapLanguageProperty("Name", mappingLanguages, group.GetName);

                groups.Add(customerGroup);
            }

            return groups;
        }

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            if (value == null)
                return;

            var customer = (EnovaCustomer)obj;
            var context = obj.GetContext();
            foreach (var group in value as dynamic)
            {
                var groupModel = JsonConvert.DeserializeAnonymousType(group.ToString(), new {ID = 0, Identifier = String.Empty, MarkForDelete = false}) ;
                var enovaGroup = (CustomerGroup) (context.FindObject(groupModel.ID, typeof(CustomerGroup), false) ?? context.FindObject(groupModel.Identifier, typeof(CustomerGroup), true));

                if (groupModel.MarkForDelete)
                    enovaGroup.RemoveUser(customer);
                else if (!enovaGroup.HasUser(customer))
                    enovaGroup.AddUser(customer);
            }
        }

        
    }
}
