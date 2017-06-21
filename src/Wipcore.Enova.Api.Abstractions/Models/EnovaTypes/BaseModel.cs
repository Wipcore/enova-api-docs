using System;
using System.Collections.Generic;
using Wipcore.Enova.Api.Abstractions.Attributes;

namespace Wipcore.Enova.Api.Abstractions.Models.EnovaTypes
{
    [GroupPresentation("System information", new[] { "ID", "Enabled", "ModifiedAt", "CreatedAt", "SortOrder" }, sortOrder: 1000)]
    public class BaseModel
    {
        [PropertyPresentation("NumberString", "Id", isEditable: false, isFilterable: true, isGridColumn : true, sortOrder : 105 )]
        public int ID { get; set; }

        [PropertyPresentation("String", null, isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 100)]
        public string Identifier { get; set; }

        [PropertyPresentation("DateTime", "Modified at", isEditable: false, isFilterable: true, isGridColumn: true, sortOrder: 300)]
        public DateTime ModifiedAt { get; set; }

        [PropertyPresentation("DateTime", "Created at", isEditable: false, isFilterable: true, isGridColumn: true, sortOrder: 300)]
        public DateTime CreatedAt { get; set; }

        [PropertyPresentation("Boolean", "Enabled", isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 110)]
        public bool Enabled { get; set; } = true;

        [PropertyPresentation("NumberString", "Sort order", isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 310)]
        public int SortOrder { get; set; }


        /// <summary>
        /// Get the names of the properties that should be shown in the default 'all' tab in the admin.
        /// </summary>
        /// <returns></returns>
        public virtual List<string> GetDefaultPropertiesInGrid() => new List<string>(){"ID", "Identifier", "ModifiedAt", "CreatedAt"};


        public virtual string GetResourceName() => this.GetType().Name.Replace("Model", "s").ToLower();

    }
}
