using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Api.Abstractions.Internal;
using Wipcore.Enova.Api.WebApi.Helpers;
using Wipcore.Enova.Core;

namespace Wipcore.eNova.Api.WebApi.Mappers.Section
{
    public class SectionChildrenMapper : IPropertyMapper
    {
        public bool PostSaveSet => false;
        public List<string> Names => new List<string>() { "Children" };
        public Type Type => typeof(EnovaBaseProductSection);
        public bool InheritMapper => true;
        public bool FlattenMapping => false;
        public int Priority => 0;
        public MapType MapType => MapType.MapFromAndToEnovaAllowed;

        public object GetEnovaProperty(BaseObject obj, string propertyName, List<EnovaLanguage> mappingLanguages)
        {
            var section = (EnovaBaseProductSection)obj;
            var children = section.Children;

            return children?.OfType<EnovaBaseProductSection>().Select(x => new Dictionary<string, object>()
            {
                {"ID", x.ID}, {"Identifier", x.Identifier}, {"MarkForDelete", false}
            }.MapLanguageProperty("Name", mappingLanguages, x.GetName));
        }

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            if (value == null)
                return;

            var section = (EnovaBaseProductSection)obj;
            
            var childrenModels = JsonConvert.DeserializeAnonymousType(value.ToString(), new[] { new { ID = 0, Identifier = "", MarkForDelete = false } });

            foreach (var childrenModel in childrenModels)
            {
                var child = section.GetContext().Find<EnovaBaseProductSection>(childrenModel.ID, childrenModel.Identifier, true);
                if (childrenModel.MarkForDelete)
                {
                    
                    if (section.HasChild(child))
                        section.RemoveChild(child);
                }
                else
                {
                    if (!section.HasChild(child))
                        section.AddChild(child);
                }
            }
        }
    }

}
