using System;

namespace Wipcore.Enova.Api.Abstractions.Attributes
{
    /// <summary>
    /// This attribute specifies that a model should be auto-indexed by elastic search
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class IndexModelAttribute : System.Attribute
    {

    }

    /// <summary>
    /// Specifies that the property should not be indexed in elastic search.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class IgnorePropertyOnIndex : System.Attribute
    {

    }
}
