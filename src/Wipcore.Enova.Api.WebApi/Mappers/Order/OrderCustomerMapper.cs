using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Core;

namespace Wipcore.eNova.Api.WebApi.Mappers.Order
{
    public class OrderCustomerMapper : IPropertyMapper
    {
        public List<string> Names => new List<string>() { "CustomerID", "CustomerIdentifier" };
        public Type Type => typeof(EnovaOrder);
        public bool InheritMapper => true;
        public int Priority => 0;
        public MapType MapType => MapType.MapFromAndToEnovaAllowed;
        public bool PostSaveSet => false;
        public object GetEnovaProperty(BaseObject obj, string propertyName)
        {
            var order = (EnovaOrder)obj;

            if (propertyName.Equals("CustomerID", StringComparison.InvariantCultureIgnoreCase))
                return order.Customer?.ID;

            return order.Customer?.Identifier;
        }

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            var order = (EnovaOrder)obj;
            var context = order.GetContext();

            if (propertyName.Equals("CustomerID", StringComparison.InvariantCultureIgnoreCase))
            {
                var id = Convert.ToInt32(value);
                if (id == 0)
                    order.Customer = null;
                else if(order.Customer?.ID != id)
                {
                    order.Customer = EnovaCustomer.Find(context, id);
                }
            }
            else
            {
                var identifier = value.ToString();
                if (String.IsNullOrEmpty(identifier))
                    order.Customer = null;
                else if (order.Customer?.Identifier != identifier)
                {
                    order.Customer = EnovaCustomer.Find(context, identifier);
                }
            }
          
        }
    }

}
