using System;
using System.Collections.Generic;
using Wipcore.Enova.Api.Abstractions.Attributes;

namespace Wipcore.Enova.Api.Abstractions.Models.EnovaTypes
{
    [GroupPresentation("Setting info", new[] { "Identifier" }, sortOrder: 200)]
    [GroupPresentation("Values", new[] { "Value", "ValueString", "ValueInteger", "ValueBoolean", "ValueDateTime", "ValueFloat", "ValueMoney" }, sortOrder: 300)]
    [IndexModel]
    public class SystemSettingModel : BaseModel
    {

        [PropertyPresentation("String", null, languageDependant: true, isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 300)]
        public string Value { get; set; }

        [PropertyPresentation("String", "Value string", isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 400, description: "Language independent")]
        public string ValueString { get; set; }

        [PropertyPresentation("NumberString", "Value integer", isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 500)]
        public int ValueInteger { get; set; }

        [PropertyPresentation("DateTime", "Value datetime", isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 600)]
        public DateTime ValueDateTime { get; set; } = WipConstants.InvalidDateTime;

        [PropertyPresentation("Boolean", "Value bool", isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 700)]
        public bool ValueBoolean { get; set; }

        [PropertyPresentation("NumberString", "Value double", isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 800)]
        public double ValueFloat { get; set; }

        [PropertyPresentation("NumberString", "Value decimal", isEditable: true, isFilterable: true, isGridColumn: true, sortOrder: 900)]
        public decimal ValueMoney { get; set; }
        
        public override List<string> GetDefaultPropertiesInGrid()
        {
            return new List<string>() { "Identifier", "Value", "ValueInteger", "ValueDateTime", "ValueBoolean", "ValueFloat", "ValueMoney", "ValueString" };
        }
    }
}
