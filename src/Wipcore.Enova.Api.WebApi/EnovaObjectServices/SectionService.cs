using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Core;

namespace Wipcore.Enova.Api.WebApi.EnovaObjectServices
{
    public class SectionService : ISectionService
    {
        private readonly IContextService _contextService;

        public SectionService(IContextService contextService)
        {
            _contextService = contextService;
        }

        /// <summary>
        /// Get any sub-sections to the section with given identifier.
        /// </summary>
        public BaseObjectList GetSubSections(string identifier)
        {
            var context = _contextService.GetContext();
            var section = EnovaBaseProductSection.Find(context, identifier);

            return section.GetChildren(typeof (EnovaBaseProductSection), false, true);
        }

        /// <summary>
        /// Get any products beloning to the section with given identifier.
        /// </summary>
        public BaseObjectList GetProducts(string identifier)
        {
            var context = _contextService.GetContext();
            var section = EnovaBaseProductSection.Find(context, identifier);
            return section.GetItems(typeof(EnovaBaseProduct));
        }
    }
}
