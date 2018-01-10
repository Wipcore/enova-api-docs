using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wipcore.Enova.Api.Abstractions.Attributes;

namespace Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Administrator
{
    [GroupPresentation("Group info", new string[] { "Identifier", "Name" }, sortOrder: 100)]
    [GroupPresentation("Group members", new string[] { "Users" }, sortOrder: 200)]
    [IndexModel]
    public class AdministratorGroupModel : BaseModel
    {
        [PropertyPresentation("String", null, languageDependant: true, isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 300)]
        public string Name { get; set; }

        [PropertyPresentation("GroupMembers", null, isEditable: true, isFilterable: false, isGridColumn: true, sortOrder: 300)]
        public List<AdministratorMiniModel> Users { get; set; }
    }
}
