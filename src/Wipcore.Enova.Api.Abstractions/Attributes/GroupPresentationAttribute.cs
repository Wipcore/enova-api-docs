using System;

namespace Wipcore.Enova.Api.Abstractions.Attributes
{
    /// <summary>
    /// This attribute is used to give information of how a properties should be grouped.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class GroupPresentationAttribute : Attribute
    {
        /// <summary>
        /// The name of this group of properties.
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// The names of the properties belonging to this group.
        /// </summary>
        public string[] PropertyNames { get; set; } 

        /// <summary>
        /// Whether this group should be visible or not.
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// The order in which this group should appear.
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// The priority for choosing this group if there are several by the same name. Higher means liklier. 
        /// </summary>
        public int Priority { get; set; }

        public GroupPresentationAttribute(string groupName, string[] propertyNames, bool visible = true, int sortOrder = 1, int priority = 0)
        {
            GroupName = groupName;
            PropertyNames = propertyNames;
            Visible = visible;
            SortOrder = sortOrder;
            Priority = priority;
        }
    }
}
