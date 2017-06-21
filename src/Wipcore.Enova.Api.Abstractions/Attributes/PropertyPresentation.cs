using System;

namespace Wipcore.Enova.Api.Abstractions.Attributes
{
    /// <summary>
    /// This attribute is used to give information of how a property should be presented in the admin.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    public class PropertyPresentation : Attribute
    {
        /// <summary>
        /// The type of html control that should be used for this property. I.e. texbox, label, etc.
        /// </summary>
        public string ControlType { get; set; }

        /// <summary>
        /// The label used to describe this property.
        /// </summary>
        public string ControlLabel { get; set; }

        /// <summary>
        /// The description for this property, if additional information beside the label is needed.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Whether this property can be edited by the user.
        /// </summary>
        public bool IsEditable { get; set; }

        /// <summary>
        /// Whether this property can be used in a filter.
        /// </summary>
        public bool IsFilterable { get; set; }

         /// <summary>
        /// Whether this property can be shown in the grid view.
        /// </summary>
        public bool IsGridColumn { get; set; }

        /// <summary>
        /// The sort order for this property to show up as a filter option.
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// Whether this property changes depending on the language.
        /// </summary>
        public bool LanguageDependant { get; set; }

        public string ApiQueryName { get; set; }

        public PropertyPresentation(string controlType, string controlLabel = null, bool isEditable = true, bool isFilterable = true, bool isGridColumn = true, 
            int sortOrder = int.MaxValue, string description = null, bool languageDependant = false, string apiQueryName = null)
        {
            ControlType = controlType;
            ControlLabel = controlLabel;
            IsEditable = isEditable;
            IsFilterable = isFilterable;
            IsGridColumn = isGridColumn;
            SortOrder = sortOrder;
            Description = description;
            LanguageDependant = languageDependant;
            ApiQueryName = apiQueryName;
        }
    }
}
