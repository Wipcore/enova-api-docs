using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fasterflect;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Core;
using Wipcore.Enova.Generics;

namespace Wipcore.Enova.Api.WebApi.EnovaObjectServices
{
    public class AttributeService : IAttributeService
    {
        private readonly IContextService _contextService;

        public AttributeService(IContextService contextService)
        {
            _contextService = contextService;
        }

        /// <summary>
        /// Get attributes of the object with the given identifier.
        /// </summary>
        public BaseObjectList GetAttributes<T>(string identifier) where T : BaseObject
        {
            var context = _contextService.GetContext();
            var obj = context.FindObject(identifier, typeof (T)); //find as most specific type possible, to prevent Enova looking in many tables

            return obj.AttributeValues;
        }

        /// <summary>
        /// Get all objects that has an attributevalue.
        /// </summary>
        public IEnumerable<IDictionary<string, object>> GetObjectsWithAttributeValue(int attributeValueId)
        {
            var context = _contextService.GetContext();
            var attributeValue = EnovaAttributeValue.Find(context, attributeValueId);

            var objects = attributeValue.GetObjects().OfType<BaseObject>();
            
            return objects.Select(x => new Dictionary<string, object>() { { "ID", x.ID }, { "Identifier", x.Identifier }, { "Type", x.GetType().Name }, { "Name",  x.Name} });
        }
    }
}
