using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Generics;

namespace Wipcore.eNova.Api.WebApi.EnovaObjectServices
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
    }
}
