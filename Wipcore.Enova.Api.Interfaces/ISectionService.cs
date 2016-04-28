using Wipcore.Core.SessionObjects;

namespace Wipcore.Enova.Api.Interfaces
{
    public interface ISectionService
    {
        BaseObjectList GetSubSections(string identifier);

        BaseObjectList GetProducts(string identifier);
    }
}