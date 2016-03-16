using Wipcore.Core.SessionObjects;

namespace Wipcore.eNova.Api.WebApi.Services
{
    public interface IProductService
    {
        BaseObjectList GetVariants(string identifier);
    }
}