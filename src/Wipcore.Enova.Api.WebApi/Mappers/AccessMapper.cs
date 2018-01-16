using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Api.Abstractions.Internal;
using Wipcore.Enova.Api.Abstractions.Models;
using Wipcore.Enova.Core;

namespace Wipcore.eNova.Api.WebApi.Mappers
{
    /// <summary>
    /// This mapper is used to map access rights for some commonly used enova types.
    /// </summary>
    public class AccessMapper : IPropertyMapper
    {
        private readonly IAccessRightService _accessRightService;
        private readonly List<string> _typeNames;

        public AccessMapper(IConfigurationRoot configurationRoot, IAccessRightService accessRightService)
        {
            _accessRightService = accessRightService;
            var setting = configurationRoot["ApiSettings:AccessRightsTypesToMap"] ?? "EnovaAdministrator,EnovaAdministratorGroup,EnovaAttributeType,EnovaCart,EnovaCompany," +
                          "EnovaCustomer,EnovaCustomerGroup,EnovaTextDocument,EnovaManufacturer,EnovaOrder,EnovaPriceList,EnovaBaseProduct,EnovaPromo,EnovaBaseProductSection," +
                          "EnovaShippingStatus,EnovaGlobalSystemSettings,EnovaSystemText";
            _typeNames = setting.Split(',').Select(x => x.Trim()).Distinct().ToList();
        }

        public bool PostSaveSet => true;
        public List<string> Names { get; } = new List<string>() { "AccessRights" };
        public Type Type => typeof(UserGroup);
        public bool InheritMapper => true;
        public int Priority => 0;
        public bool FlattenMapping => false;
        public MapType MapType => MapType.MapFromAndToEnovaAllowed;
        
        public object GetEnovaProperty(BaseObject obj, string propertyName, List<EnovaLanguage> mappingLanguages)
        {
            var userGroup = (UserGroup) obj;
            var accessRights = new Dictionary<string, AccessModel>();
            foreach (var typeName in _typeNames)
            {
                var accessRight = _accessRightService.GetAccessToType(typeName, userGroup);
                accessRights.Add(typeName, accessRight);
            }

            return accessRights;
        }

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            var userGroup = (UserGroup)obj;
            var accessRights = (Dictionary<string, AccessModel>) value;

            foreach (var accessRight in accessRights)
            {
                _accessRightService.SetAccessToType(accessRight.Key, userGroup, accessRight.Value);
            }
        }
    }
}
