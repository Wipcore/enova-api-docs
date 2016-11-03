using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Api.WebApi.Helpers;
using Wipcore.Enova.Core;
using Wipcore.Enova.Generics;

namespace Wipcore.eNova.Api.WebApi.Mappers
{
    public class ShippingStatusConnectionMapper : IPropertyMapper
    {
        public bool PostSaveSet => false;
        public List<string> Names => new List<string>() { "DestinationStatuses", "ArrivalStatuses" };
        public Type Type => typeof(EnovaShippingStatus);
        public bool InheritMapper => true;
        public int Priority => 0;
        public MapType MapType => MapType.MapFromAndToEnovaAllowed;
        
        public object GetEnovaProperty(BaseObject obj, string propertyName)
        {
            var context = obj.GetContext();
            var status = (EnovaShippingStatus)obj;
            var relations = String.Equals(propertyName, "DestinationStatuses", StringComparison.InvariantCultureIgnoreCase)
                ? status.GetDestinationStatuses()?.Cast<EnovaShippingStatus>() ?? new List<EnovaShippingStatus>()
                : context.Search($"Allow = 1 AND DestinationStatus = '{status.Identifier}' ", typeof (EnovaConfigShippingStatusRule)).Cast<EnovaConfigShippingStatusRule>()
                    .Select(x => context.FindObject<EnovaShippingStatus>(x.SourceStatus)).Where(x => x != null);

            return relations.Select(x => new {x.ID, x.Identifier, x.Name, MarkForDelete = false});
        }

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            if (value == null)
                return;

            var context = obj.GetContext();
            var status = (EnovaShippingStatus)obj;

            if (String.Equals(propertyName, "ArrivalStatuses", StringComparison.InvariantCultureIgnoreCase))
                return; //can't be modified directly

            var statusModels = JsonConvert.DeserializeAnonymousType(value.ToString(), new []{ new { ID = 0, MarkForDelete = false} });

            foreach (var statusModel in statusModels)
            {
                var destinationStatus = EnovaShippingStatus.Find(context, statusModel.ID);
                var rule = context.FindObject<EnovaConfigShippingStatusRule>($"{status.Identifier}:{destinationStatus.Identifier}") ?? 
                    EnovaObjectCreationHelper.CreateNew<EnovaConfigShippingStatusRule>(context);

                if (rule.ID == 0)
                {
                    rule.Identifier = $"{status.Identifier}:{destinationStatus.Identifier}";
                    rule.Name = $"{status.Name} ({status.Identifier}) -> {destinationStatus.Name} ({destinationStatus.Identifier})";
                    rule.SourceStatus = status.Identifier;
                    rule.DestinationStatus = destinationStatus.Identifier;
                }
                else
                {
                    rule.Edit();
                }

                rule.Allow = !statusModel.MarkForDelete;
                rule.Save();
            }
        }

    }
}
