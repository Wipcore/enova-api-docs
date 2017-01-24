using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Interfaces;
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
        public bool PostSaveSet => false;
        public bool FlattenMapping => false;

        public object GetEnovaProperty(BaseObject obj, string propertyName)
        {
            var groups = new List<object>();
            var customer = (EnovaCustomer) obj;

            foreach (var group in customer.Groups.OfType<EnovaCustomerGroup>())
            {
                var customerGroup = new
                {
                    ID = group.ID,
                    Identifier = group.Identifier,
                    Name = group.Name,
                    MarkForDelete = false
                };
                groups.Add(customerGroup);
            }

            return groups;
        }

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            if (value == null)
                return;

            var customer = (EnovaCustomer)obj;
            dynamic groups = value;
            foreach (var g in groups)
            {
                var group = (Dictionary<string, object>)JsonConvert.DeserializeObject(g.ToString(), typeof(Dictionary<string, object>));
                var enovaGroup = EnovaCustomerGroup.Find(obj.GetContext(), group.GetOrDefault<int>("ID"));

                if (group.GetOrDefault<bool>("MarkForDelete"))
                    enovaGroup.RemoveUser(customer);
                else if (!enovaGroup.HasUser(customer))
                    enovaGroup.AddUser(customer);
            }
        }

        
    }
}
