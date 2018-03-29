using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Api.Abstractions.Internal;
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
        public bool FlattenMapping => false;
        public MapType MapType => MapType.MapFromAndToEnovaAllowed;
        
        public object GetEnovaProperty(BaseObject obj, string propertyName, List<EnovaLanguage> mappingLanguages)
        {
            var context = obj.GetContext();
            var status = (EnovaShippingStatus)obj;
            var relations = String.Equals(propertyName, "DestinationStatuses", StringComparison.InvariantCultureIgnoreCase)
                ? status.GetDestinationStatuses()?.Cast<EnovaShippingStatus>() ?? new List<EnovaShippingStatus>()
                : context.Search($"Allow = 1 AND DestinationStatus = '{status.Identifier}' ", typeof (EnovaConfigShippingStatusRule)).Cast<EnovaConfigShippingStatusRule>()
                    .Select(x => context.FindObject<EnovaShippingStatus>(x.SourceStatus)).Where(x => x != null);

            return relations.Select(x => new Dictionary<string, object>() {{"ID", x.ID}, {"Identifier", x.Identifier}, {"MarkForDelete", false}}.MapLanguageProperty("Name", mappingLanguages, x.GetName));
        }

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            if (value == null)
                return;

            var context = obj.GetContext();
            var status = (EnovaShippingStatus)obj;

            if (String.Equals(propertyName, "ArrivalStatuses", StringComparison.InvariantCultureIgnoreCase))
                return; //can't be modified directly

            var statusModels = JsonConvert.DeserializeAnonymousType(value.ToString(), new []{ new { ID = 0, Identifier = String.Empty, MarkForDelete = false} });

            foreach (var statusModel in statusModels)
            {
                var destinationStatus = context.Find<EnovaShippingStatus>(statusModel.ID, statusModel.Identifier, true);
                var rule = context.FindObject<EnovaConfigShippingStatusRule>($"{status.Identifier}:{destinationStatus.Identifier}") ?? 
                    EnovaObjectMakerHelper.CreateNew<EnovaConfigShippingStatusRule>(context);

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
