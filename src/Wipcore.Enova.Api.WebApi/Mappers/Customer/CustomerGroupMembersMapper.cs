using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Core;

namespace Wipcore.eNova.Api.WebApi.Mappers.Customer
{
    public class CustomerGroupMembersMapper : IPropertyMapper
    {
        public List<string> Names => new List<string>() { "Members"};
        public Type Type => typeof(EnovaCustomerGroup);
        public bool InheritMapper => true;
        public int Priority => 0;
        public MapType MapType => MapType.MapFromAndToEnovaAllowed;

        public bool PostSaveSet => false;
        public bool FlattenMapping => false;
        public object GetEnovaProperty(BaseObject obj, string propertyName)
        {
            var group = (EnovaCustomerGroup) obj;
            return group.GetUsers(typeof (EnovaCustomer)).Cast<EnovaCustomer>().Select(x => new
            {
               x.ID, x.Identifier, x.Alias, x.FirstName, x.LastName
            });
        }

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            if (value == null)
                return;

            var group = (EnovaCustomerGroup)obj;
            var context = obj.GetContext();
            foreach (var g in value as dynamic)
            {
                var item = JsonConvert.DeserializeAnonymousType(g.ToString(), new { ID = 0, Identifier = "", PriceExclTax = 0m, MarkForDelete = false });
                var customer = EnovaCustomer.Find(context, item.ID);
                if(item.MarkForDelete)
                    group.RemoveUser(customer);
                else if (!group.HasUser(customer))
                    group.AddUser(customer);
            }
        }
    }
}
