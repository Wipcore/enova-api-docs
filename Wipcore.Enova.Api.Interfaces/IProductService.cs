using Wipcore.Core.SessionObjects;

namespace Wipcore.Enova.Api.Interfaces
{
    public interface IProductService
    {
        /// <summary>
        /// Get all members of any variant family the given product is a member of. Null if no family exists.
        /// </summary>
        BaseObjectList GetVariants(string identifier);
    }
}