using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Core;

namespace Wipcore.eNova.Api.WebApi.Mappers.Section
{
    public class SectionChildrenMapper : IPropertyMapper
    {
        public List<string> Names => new List<string>() { "Children" };
        public Type Type => typeof(EnovaBaseProductSection);
        public bool InheritMapper => true;
        public int Priority => 0;
        public MapType MapType => MapType.MapFromAndToEnovaAllowed;

        public object GetEnovaProperty(BaseObject obj, string propertyName)
        {
            var section = (EnovaBaseProductSection)obj;
            var children = section.Children;

            return children?.OfType<EnovaBaseProductSection>().Select(x =>
                new { ID = x.ID, Identifier = x.Identifier, Name = x.Name, MarkForDelete = false });
        }

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            var section = (EnovaBaseProductSection)obj;
            
            var childrenModels = JsonConvert.DeserializeAnonymousType(value.ToString(), new[] { new { Identifier = "", MarkForDelete = false } });

            foreach (var childrenModel in childrenModels)
            {
                var child = EnovaBaseProductSection.Find(section.GetContext(), childrenModel.Identifier);
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
