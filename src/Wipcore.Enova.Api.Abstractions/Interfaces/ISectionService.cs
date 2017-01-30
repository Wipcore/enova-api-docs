using Wipcore.Core.SessionObjects;

namespace Wipcore.Enova.Api.Abstractions.Interfaces
{
    public interface ISectionService
    {
        /// <summary>
        /// Get any sub-sections to the section with given identifier.
        /// </summary>
        BaseObjectList GetSubSections(string identifier);

        /// <summary>
        /// Get any products beloning to the section with given identifier.
        /// </summary>
        BaseObjectList GetProducts(string identifier);
    }
}