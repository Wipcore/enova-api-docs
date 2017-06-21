using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Api.Abstractions.Internal;
using Wipcore.Enova.Core;
using Wipcore.Enova.Generics;

namespace Wipcore.eNova.Api.WebApi.Mappers.PriceList
{
    public class PriceListGroupMapper : IPropertyMapper
    {
        public bool PostSaveSet => true;
        private readonly IContextService _contextService;

        public PriceListGroupMapper(IContextService contextService)
        {
            _contextService = contextService;
        }

        public List<string> Names => new List<string>() {"Groups"};
        public Type Type => typeof (EnovaPriceList);
        public bool InheritMapper => true;
        public bool FlattenMapping => false;
        public int Priority => 0;
        public MapType MapType => MapType.MapFromAndToEnovaAllowed;

        public object GetEnovaProperty(BaseObject obj, string propertyName, List<EnovaLanguage> mappingLanguages)
        {
            var context = _contextService.GetContext();
            var priceList = (EnovaPriceList)obj;
            var lists = priceList.GetObjectsWithSpecificAccess(context, typeof(CustomerGroup)).Cast<CustomerGroup>();

            return lists.Select(x => new Dictionary<string, object>()
            {
                {"ID", x.ID},
                {"Identifier", x.Identifier},
                {"Type", x.GetType().Name},
                {"MarkForDelete", false}
            }.MapLanguageProperty("Name", mappingLanguages, x.GetName));
        }

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            if(value == null)
                return;

            var priceList = (EnovaPriceList)obj;
            var context = _contextService.GetContext();
            foreach (var i in value as dynamic)
            {
                var item = JsonConvert.DeserializeAnonymousType(i.ToString(), new { ID = 0, Identifier = String.Empty, MarkForDelete = false});
                var customerGroup = (CustomerGroup) (context.FindObject(item.ID, typeof(CustomerGroup), false) ?? context.FindObject(item.Identifier, typeof(CustomerGroup), true));
                if (item.MarkForDelete == true)
                {
                    priceList.RemoveSpecificAccess(customerGroup);
                    continue;
                }

                if (priceList.GetSpecificAccess(customerGroup) < (BaseObject.AccessUse | BaseObject.AccessRead))
                    priceList.SetSpecificAccess(customerGroup, BaseObject.AccessUse | BaseObject.AccessRead);
            }
        }
    }
}
