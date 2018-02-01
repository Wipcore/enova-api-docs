using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions.Internal;
using Wipcore.Enova.Api.Abstractions.Models;
using Wipcore.Enova.Core;

namespace Wipcore.Enova.Api.WebApi.Mappers.Customer
{
    /// <summary>
    /// This mapper is used to map access rights for some commonly used enova types.
    /// </summary>
    public class UserAccessMapper : IPropertyMapper
    {
        public const string DefaultAccessTypesToMap =
            "EnovaAdministrator,EnovaAdministratorGroup,EnovaAttributeType,EnovaCart,EnovaCompany," +
            "EnovaCustomer,EnovaCustomerGroup,EnovaTextDocument,EnovaManufacturer,EnovaOrder,EnovaPriceList,EnovaBaseProduct,EnovaPromo,EnovaBaseProductSection," +
            "EnovaShippingStatus,EnovaGlobalSystemSettings,EnovaSystemText";

        private readonly IAccessRightService _accessRightService;
        private readonly List<string> _typeNames;

        public UserAccessMapper(IConfigurationRoot configurationRoot, IAccessRightService accessRightService)
        {
            _accessRightService = accessRightService;
            var setting = configurationRoot["ApiSettings:AccessRightsTypesToMap"] ?? DefaultAccessTypesToMap;
            _typeNames = setting.Split(',').Select(x => x.Trim()).Distinct().ToList();
        }

        public bool PostSaveSet => true;
        public List<string> Names { get; } = new List<string>() { "AccessRights", "ExplicitAccessRights" };
        public Type Type => typeof(User);
        public bool InheritMapper => true;
        public int Priority => 0;
        public bool FlattenMapping => false;
        public MapType MapType => MapType.MapFromAndToEnovaAllowed;

        public object GetEnovaProperty(BaseObject obj, string propertyName, List<EnovaLanguage> mappingLanguages)
        {
            var includeEveryonesRights = String.Equals(propertyName, "AccessRights", StringComparison.InvariantCultureIgnoreCase);
            var user = (User) obj;
            
            return _typeNames.Select(typeName => _accessRightService.GetUserAccessToType(typeName, user, includeEveryonesRights));
        }

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            throw new NotImplementedException();//discourage setting access rights on user level. Set on usergroup instead.
        }
    }
}
