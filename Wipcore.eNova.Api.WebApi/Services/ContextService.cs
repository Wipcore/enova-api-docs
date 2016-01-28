using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Wipcore.Core.SessionObjects;
using Wipcore.eNova.Api.WebApi.Models;
using Wipcore.Enova.Core;
using Wipcore.Enova.Generics;

namespace Wipcore.eNova.Api.WebApi.Services
{
    public class ContextService
    {
        public const string ContextModelKey = "requestContext";
        public const string EnovaContextKey = "enovaContext";

        private readonly IHttpContextAccessor _httpAccessor;


        public ContextService(IHttpContextAccessor httpAccessor)
        {
            _httpAccessor = httpAccessor;
        }

        public Context GetContext()
        {
            //TODO threat safety
            var enovaContext = _httpAccessor.HttpContext.Items[EnovaContextKey] as Context;
            if (enovaContext != null)
                return enovaContext;
            
            enovaContext = EnovaSystemFacade.Current.Connection.CreateContext();
            _httpAccessor.HttpContext.Items[EnovaContextKey] = enovaContext;

            var requestContext = _httpAccessor.HttpContext.Items[ContextModelKey] as ContextModel;
            if (requestContext == null)
                return enovaContext;//TODO default values if non specified?

            if (requestContext.Language != null)
            {
                var language = enovaContext.FindObject<EnovaLanguage>(requestContext.Language);
                if (language != null)
                    enovaContext.CurrentLanguage = language;
            }
            

            return enovaContext;
        }
    }
}
