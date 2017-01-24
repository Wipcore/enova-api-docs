﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Core;

namespace Wipcore.eNova.Api.WebApi.Mappers.Attribute
{
    public class AttributeAsPropertyMapper : IPropertyMapper
    {
        public List<string> Names => new List<string>() {"AttributesAsProperties"};
        public Type Type => typeof(BaseObject);
        public bool InheritMapper => true;
        public int Priority => 1;
        public MapType MapType => MapType.MapFromEnovaAllowed;
        public bool PostSaveSet => false;
        public bool FlattenMapping => true;
        public object GetEnovaProperty(BaseObject obj, string propertyName)
        {
            var values = new Dictionary<string, object>();
            foreach (var attributeValue in obj.AttributeValues.OfType<EnovaAttributeValue>())
            {
                var value = !String.IsNullOrEmpty(attributeValue.ValueCode) ? attributeValue.ValueCode : attributeValue.Name;
                var type = attributeValue.AttributeType?.Identifier ?? String.Empty;
                values.Add(type, value);
            }
            return values;
        }

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            
        }

    }
}
