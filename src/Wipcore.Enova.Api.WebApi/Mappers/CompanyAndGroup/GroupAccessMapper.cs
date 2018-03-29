using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions.Internal;
using Wipcore.Enova.Api.Abstractions.Models;
using Wipcore.Enova.Api.WebApi.Mappers.Customer;
using Wipcore.Enova.Core;

namespace Wipcore.Enova.Api.WebApi.Mappers.CompanyAndGroup
{
    /// <summary>
    /// This mapper is used to map access rights for some commonly used enova types.
    /// </summary>
    public class GroupAccessMapper : IPropertyMapper
    {
        private readonly IAccessRightService _accessRightService;
        private readonly List<string> _typeNames;

        public GroupAccessMapper(IConfigurationRoot configurationRoot, IAccessRightService accessRightService)
        {
            _accessRightService = accessRightService;
            var setting = configurationRoot["ApiSettings:AccessRightsTypesToMap"] ?? UserAccessMapper.DefaultAccessTypesToMap;
            _typeNames = setting.Split(',').Select(x => x.Trim()).Distinct().ToList();
        }

        public bool PostSaveSet => true;
        public List<string> Names { get; } = new List<string>() { "AccessRights", "ExplicitAccessRights" };
        public Type Type => typeof(UserGroup);
        public bool InheritMapper => true;
        public int Priority => 0;
        public bool FlattenMapping => false;
        public MapType MapType => MapType.MapFromAndToEnovaAllowed;
        
        public object GetEnovaProperty(BaseObject obj, string propertyName, List<EnovaLanguage> mappingLanguages)
        {
            var includeEveryonesRights = String.Equals(propertyName, "AccessRights", StringComparison.InvariantCultureIgnoreCase);
            var group = (UserGroup)obj;

            return _typeNames.Select(typeName => _accessRightService.GetGroupAccessToType(typeName, group, includeEveryonesRights));
        }

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            if(value == null)
                return;

            var userGroup = (UserGroup)obj;
            var accessRights = JsonConvert.DeserializeObject<List<AccessModel>>(value.ToString());

            foreach (var accessRight in accessRights)
            {
                _accessRightService.SetAccessToType(userGroup, accessRight);
            }
        }
    }
}
