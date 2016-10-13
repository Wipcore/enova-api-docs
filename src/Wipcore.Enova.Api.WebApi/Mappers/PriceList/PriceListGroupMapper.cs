using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Core;

namespace Wipcore.eNova.Api.WebApi.Mappers.PriceList
{
    public class PriceListGroupMapper : IPropertyMapper
    {
        private readonly IContextService _contextService;

        public PriceListGroupMapper(IContextService contextService)
        {
            _contextService = contextService;
        }

        public List<string> Names => new List<string>() {"Groups"};
        public Type Type => typeof (EnovaPriceList);
        public bool InheritMapper => true;
        public int Priority => 0;
        public MapType MapType => MapType.MapFromAndToEnovaAllowed;

        public void MapToEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            var priceList = (EnovaPriceList)obj;
            var context = _contextService.GetContext();
            foreach (var i in value as dynamic)
            {
                var item = JsonConvert.DeserializeAnonymousType(i.ToString(), new { ID = 0, MarkForDelete = false});
                var customerGroup = EnovaCustomerGroup.Find(context, item.ID);
                if (item.MarkForDelete == true)
                {
                    priceList.RemoveSpecificAccess(customerGroup);
                    continue;
                }

                if (priceList.GetSpecificAccess(customerGroup) < (BaseObject.AccessUse | BaseObject.AccessRead))
                    priceList.SetSpecificAccess(customerGroup, BaseObject.AccessUse | BaseObject.AccessRead);
            }
        }

        public object MapFromEnovaProperty(BaseObject obj, string propertyName)
        {
            var context = _contextService.GetContext();
            var priceList = (EnovaPriceList) obj;
            var lists = priceList.GetObjectsWithSpecificAccess(context, typeof (EnovaCustomerGroup)).Cast<EnovaCustomerGroup>();

            return lists.Select(x => new
            {
                ID = x.ID,
                Identifier = x.Identifier,
                Name = x.Name,
                MarkForDelete = false
            });
        }
    }
}
