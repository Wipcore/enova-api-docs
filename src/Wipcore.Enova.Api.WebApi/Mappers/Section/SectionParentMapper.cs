using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Core;
using Wipcore.Enova.Generics;

namespace Wipcore.eNova.Api.WebApi.Mappers.Section
{
    /// <summary>
    /// Maps the parent of a productsection. Assumes only one parent to simplify.
    /// </summary>
    public class SectionParentMapper : IPropertyMapper
    {
        public List<string> Names => new List<string>() { "Parent" };
        public Type Type => typeof(EnovaBaseProductSection);
        public bool InheritMapper => true;
        public int Priority => 0;
        public MapType MapType => MapType.MapFromAndToEnovaAllowed;

        public object GetEnovaProperty(BaseObject obj, string propertyName)
        {
            var section = (EnovaBaseProductSection) obj;
            var parent = section.Parent;

            return parent == null ? new { ID = 0, Identifier = "", Name = "" } : 
                new { ID = parent.ID, Identifier = parent.Identifier, Name = parent.Name};
        }

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            var context = obj.GetContext();
            var section = (EnovaBaseProductSection)obj;
            if (String.IsNullOrEmpty(value?.ToString()))
            {
                if(section.Parent != null)
                    section.RemoveParent(section.Parent);
                return;
            }

            var parentModel = JsonConvert.DeserializeAnonymousType(value.ToString(), new { Identifier  = "", ID = 0});
            var parent = context.FindObject<EnovaBaseProductSection>(parentModel.Identifier);
            section.Parent = parent;
        }

        
    }
}
