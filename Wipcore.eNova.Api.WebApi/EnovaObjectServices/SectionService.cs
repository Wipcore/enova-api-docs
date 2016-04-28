using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Core;

namespace Wipcore.eNova.Api.WebApi.EnovaObjectServices
{
    public class SectionService : ISectionService
    {
        private readonly IContextService _contextService;

        public SectionService(IContextService contextService)
        {
            _contextService = contextService;
        }

        public BaseObjectList GetSubSections(string identifier)
        {
            var context = _contextService.GetContext();
            var section = EnovaBaseProductSection.Find(context, identifier);

            return section.GetChildren(typeof (EnovaBaseProductSection), false, true);
        }

        public BaseObjectList GetProducts(string identifier)
        {
            var context = _contextService.GetContext();
            var section = EnovaBaseProductSection.Find(context, identifier);
            return section.GetItems(typeof(EnovaBaseProduct));
        }
    }
}
