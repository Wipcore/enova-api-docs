using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Core;

namespace Wipcore.eNova.Api.WebApi.Mappers.Promo
{
    public class PromoGroupsMapper : IPropertyMapper
    {
        public List<string> Names => new List<string>() { "Groups" };
        public Type Type => typeof(EnovaPromo);
        public bool InheritMapper => true;
        public int Priority => 0;
        public MapType MapType => MapType.MapFromAndToEnovaAllowed;
        public bool PostSaveSet => false;
        public bool FlattenMapping => false;

        public object GetEnovaProperty(BaseObject obj, string propertyName, List<EnovaLanguage> mappingLanguages)
        {
            var groups = new List<object>();
            var promo = (EnovaPromo) obj;
            var context = promo.GetContext();

            foreach (var group in promo.GetObjectsWithSpecificAccess(context, typeof(EnovaCustomerGroup)).Cast<EnovaCustomerGroup>())
            {
                var customerGroup = new Dictionary<string, object>()
                {
                    {"ID", group.ID},
                    {"Identifier", group.Identifier},
                }.MapLanguageProperty("Name", mappingLanguages, group.GetName);
                groups.Add(customerGroup);
            }

            return groups;
        }

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            if (value == null)
                return;

            var promo = (EnovaPromo)obj;
            var context = promo.GetContext();

            var groups = JsonConvert.DeserializeAnonymousType(value.ToString(), new [] { new {ID = 0, MarkForDelete = false } });
            foreach (var group in groups)
            {
                var customerGroup = EnovaCustomerGroup.Find(context, group.ID);
                if (group.MarkForDelete == true)
                {
                    promo.RemoveSpecificAccess(customerGroup);
                    continue;
                }

                if (promo.GetSpecificAccess(customerGroup) < (BaseObject.AccessUse | BaseObject.AccessRead))
                    promo.SetSpecificAccess(customerGroup, BaseObject.AccessUse | BaseObject.AccessRead);
            }

        }

        
    }
}
