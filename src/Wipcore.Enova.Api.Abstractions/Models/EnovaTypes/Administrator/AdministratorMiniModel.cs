using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wipcore.Enova.Api.Abstractions.Attributes;

namespace Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Administrator
{
    public class AdministratorMiniModel : BaseModel
    {
        [PropertyPresentation("String", null, isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 300, description: "Login name")]
        public string Alias { get; set; }

        [PropertyPresentation("Password", "Password", isEditable: true, isFilterable: false, isGridColumn: false)]
        public string UserPassword { get; set; }

        [PropertyPresentation("String", "First name", isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 150)]
        public string FirstName { get; set; }

        [PropertyPresentation("String", "Last name", isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 200)]
        public string LastName { get; set; }

        [IgnorePropertyOnIndex]
        public bool MarkForDelete { get; set; }
    }
}
