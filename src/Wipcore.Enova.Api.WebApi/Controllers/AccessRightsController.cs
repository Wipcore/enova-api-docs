using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions.Internal;
using Wipcore.Enova.Api.Abstractions.Models;
using Wipcore.Enova.Api.OAuth;
using Wipcore.Enova.Core;

namespace Wipcore.Enova.Api.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class AccessRightsController : EnovaApiController
    {
        private readonly IAccessRightService _accessRightService;
        private readonly IContextService _contextService;
        private readonly IAuthService _authService;

        public AccessRightsController(EnovaApiControllerDependencies dependencies, IAccessRightService accessRightService, IContextService contextService) : base(dependencies)
        {
            _accessRightService = accessRightService;
            _contextService = contextService;
            _authService = dependencies.AuthService;
        }

        #region typeaccess

        /// <summary>
        /// Get access info on a type for the current logged in user.
        /// </summary>
        /// <param name="enovaType">The name of the enova type.</param>
        [HttpGet("LoggedInUserTypeAccess")]
        [Authorize]
        public AccessModel UserTypeAccess(string enovaType)
        {
            var userIdentifier = _authService.GetLoggedInIdentifier();
            var user = _contextService.GetContext().FindObject(userIdentifier, typeof(User));
            return _accessRightService.GetAccessToType(enovaType, user);
        }

        /// <summary>
        /// Get access to a type for a user.
        /// </summary>
        /// <param name="enovaType">The name of the enova type.</param>
        /// <param name="userIdentifier">Identifier of the user to check.</param>
        [HttpGet("UserTypeAccess/{userIdentifier}")]
        [Authorize(Roles = AuthService.AdminRole)]
        public AccessModel UserTypeAccess(string enovaType, string userIdentifier)
        {
            var user = _contextService.GetContext().FindObject(userIdentifier, typeof(User));
            return _accessRightService.GetAccessToType(enovaType, user);
        }

        /// <summary>
        /// Get access to a type for a user. 
        /// </summary>
        /// <param name="enovaType">The name of the enova type.</param>
        /// <param name="userId">Id of the user to check.</param>
        [HttpGet("UserTypeAccess/id-{userId}")]
        [Authorize(Roles = AuthService.AdminRole)]
        public AccessModel UserTypeAccess(string enovaType, int userId)
        {
            var user = _contextService.GetContext().FindObject(userId, typeof(User));
            return _accessRightService.GetAccessToType(enovaType, user);
        }

        /// <summary>
        /// Get access to a type for a group. 
        /// </summary>
        /// <param name="enovaType">The name of the enova type.</param>
        /// <param name="groupIdentifier">Identifier of the group to check.</param>
        [HttpGet("GroupTypeAccess/{groupIdentifier}")]
        [Authorize(Roles = AuthService.AdminRole)]
        public AccessModel GroupTypeAccess(string enovaType, string groupIdentifier)
        {
            var group = _contextService.GetContext().FindObject(groupIdentifier, typeof(Group));
            return _accessRightService.GetAccessToType(enovaType, group);
        }

        /// <summary>
        /// Get access to a type for a group.  
        /// </summary>
        /// <param name="enovaType">The name of the enova type.</param>
        /// <param name="groupId">Id of the group to check.</param>
        [HttpGet("GroupTypeAccess/id-{groupId}")]
        [Authorize(Roles = AuthService.AdminRole)]
        public AccessModel GroupTypeAccess(string enovaType, int groupId)
        {
            var group = _contextService.GetContext().FindObject(groupId, typeof(Group));
            return _accessRightService.GetAccessToType(enovaType, group);
        }
        #endregion

        #region objectaccess

        /// <summary>
        /// Get access info on an object for the current logged in user.
        /// </summary>
        /// <param name="objectId">ID of the object to check access to.</param>
        /// <param name="enovaType">The name of the enova type.</param>
        [HttpGet("LoggedInUserObjectAccess")]
        [Authorize]
        public AccessModel UserObjectAccess(int objectId, string enovaType)
        {
            var userIdentifier = _authService.GetLoggedInIdentifier();
            var user = _contextService.GetContext().FindObject(userIdentifier, typeof(User));
            return _accessRightService.GetAccessToObject(objectId, enovaType, user);
        }

        /// <summary>
        /// Get access info on an object for a specific user.
        /// </summary>
        /// <param name="objectId">ID of the object to check access to.</param>
        /// <param name="enovaType">The name of the enova type.</param>
        /// <param name="userIdentifier"></param>
        [HttpGet("UserObjectAccess/{userIdentifier}")]
        [Authorize(Roles = AuthService.AdminRole)]
        public AccessModel UserObjectAccess(int objectId, string enovaType, string userIdentifier)
        {
            var user = _contextService.GetContext().FindObject(userIdentifier, typeof(User));
            return _accessRightService.GetAccessToObject(objectId, enovaType, user);
        }

        /// <summary>
        /// Get access info on an object for a specific user. 
        /// </summary>
        /// <param name="objectId">ID of the object to check access to.</param>
        /// <param name="enovaType">The name of the enova type.</param>
        /// <param name="userId">Id of the user to check.</param>
        [HttpGet("UserObjectAccess/id-{userId}")]
        [Authorize(Roles = AuthService.AdminRole)]
        public AccessModel UserObjectAccess(int objectId, string enovaType, int userId)
        {
            var user = _contextService.GetContext().FindObject(userId, typeof(User));
            return _accessRightService.GetAccessToObject(objectId, enovaType, user);
        }

        /// <summary>
        /// Get access info on an object for a specific group.
        /// </summary>
        /// <param name="objectId">ID of the object to check access to.</param>
        /// <param name="enovaType">The name of the enova type.</param>
        /// <param name="groupIdentifier">Identifier of the group to check.</param>
        [HttpGet("GroupObjectAccess/{groupIdentifier}")]
        [Authorize(Roles = AuthService.AdminRole)]
        public AccessModel GroupObjectAccess(int objectId, string enovaType, string groupIdentifier)
        {
            var group = _contextService.GetContext().FindObject(groupIdentifier, typeof(Group));
            return _accessRightService.GetAccessToObject(objectId, enovaType, group);
        }

        /// <summary>
        /// Get access info on an object for a specific group.
        /// </summary>
        /// <param name="objectId">ID of the object to check access to.</param>
        /// <param name="enovaType">The name of the enova type.</param>
        /// <param name="groupId">Id of the group to check.</param>
        [HttpGet("GroupObjectAccess/id-{groupId}")]
        [Authorize(Roles = AuthService.AdminRole)]
        public AccessModel GroupObjectAccess(int objectId, string enovaType, int groupId)
        {
            var group = _contextService.GetContext().FindObject(groupId, typeof(Group));
            return _accessRightService.GetAccessToObject(objectId, enovaType, group);
        }
        #endregion

        #region setaccess
        /// <summary>
        /// Set access for a group on a type.
        /// </summary>
        /// <param name="enovaType">The name of the enova type.</param>
        /// <param name="groupId">Id of the group to check.</param>
        /// <param name="accessModel">The accessrights to set.</param>
        /// <returns></returns>
        [HttpPut("TypeAccess/id-{groupId}")]
        [Authorize(Roles = AuthService.AdminRole)]
        public AccessModel SetTypeAccess(string enovaType, int groupId, AccessModel accessModel)
        {
            var group = _contextService.GetContext().FindObject(groupId, typeof(Group)) as Group;
            _accessRightService.SetAccessToType(enovaType, group, accessModel);
            return _accessRightService.GetAccessToType(enovaType, group);
        }

        /// <summary>
        /// Set access for a group on a type.
        /// </summary>
        /// <param name="enovaType">The name of the enova type.</param>
        /// <param name="groupIdentifier">Identifier of the group to check.</param>
        /// <param name="accessModel">The accessrights to set.</param>
        /// <returns></returns>
        [HttpPut("TypeAccess/{groupIdentifier}")]
        [Authorize(Roles = AuthService.AdminRole)]
        public AccessModel SetTypeAccess(string enovaType, string groupIdentifier, AccessModel accessModel)
        {
            var group = _contextService.GetContext().FindObject(groupIdentifier, typeof(Group)) as Group;
            _accessRightService.SetAccessToType(enovaType, group, accessModel);
            return _accessRightService.GetAccessToType(enovaType, group);
        }

        /// <summary>
        /// Set access for a group on an object.
        /// </summary>
        /// <param name="objectId">ID of the object to check access to.</param>
        /// <param name="enovaType">The name of the enova type.</param>
        /// <param name="groupId">Id of the group to check.</param>
        /// <param name="accessModel">The accessrights to set.</param>
        /// <returns></returns>
        [HttpPut("ObjectAccess/id-{groupId}")]
        [Authorize(Roles = AuthService.AdminRole)]
        public AccessModel SetObjectAccess(int objectId, string enovaType, int groupId, AccessModel accessModel)
        {
            var group = _contextService.GetContext().FindObject(groupId, typeof(Group)) as Group;
            _accessRightService.SetAccessToObject(objectId, enovaType, group, accessModel);
            return _accessRightService.GetAccessToType(enovaType, group);
        }

        /// <summary>
        /// Set access for a group on an object. 
        /// </summary>
        /// <param name="objectId">ID of the object to check access to.</param>
        /// <param name="enovaType">The name of the enova type.</param>
        /// <param name="groupIdentifier">Identifier of the group to check.</param>
        /// <param name="accessModel">The accessrights to set.</param>
        /// <returns></returns>
        [HttpPut("ObjectAccess/{groupIdentifier}")]
        [Authorize(Roles = AuthService.AdminRole)]
        public AccessModel SetObjectAccess(int objectId, string enovaType, string groupIdentifier, AccessModel accessModel)
        {
            var group = _contextService.GetContext().FindObject(groupIdentifier, typeof(Group)) as Group;
            _accessRightService.SetAccessToObject(objectId, enovaType, group, accessModel);
            return _accessRightService.GetAccessToType(enovaType, group);
        }

        #endregion

        #region removeaccess
        /// <summary>
        /// Remove access to a specific type for a group. This does not deny access, it only removes specific set rights so the system falls back on default rights.
        /// </summary>
        /// <param name="enovaType">The name of the enova type.</param>
        /// <param name="groupIdentifier">Identifier of the group to check.</param>
        [HttpDelete("TypeAccess/{groupIdentifier}")]
        [Authorize(Roles = AuthService.AdminRole)]
        public ActionResult RemoveTypeAccess(string enovaType, string groupIdentifier)
        {
            var group = _contextService.GetContext().FindObject(groupIdentifier, typeof(Group)) as Group;
            _accessRightService.RemoveAccessToType(enovaType, group);
            return Ok();
        }

        /// <summary>
        /// Remove access to a specific type for a group. This does not deny access, it only removes specific set rights so the system falls back on default rights.
        /// </summary>
        /// <param name="enovaType">The name of the enova type.</param>
        /// <param name="groupId">Id of the group to check.</param>
        [HttpDelete("TypeAccess/{groupId}")]
        [Authorize(Roles = AuthService.AdminRole)]
        public ActionResult RemoveTypeAccess(string enovaType, int groupId)
        {
            var group = _contextService.GetContext().FindObject(groupId, typeof(Group)) as Group;
            _accessRightService.RemoveAccessToType(enovaType, group);
            return Ok();
        }

        /// <summary>
        /// Remove access to a specific object for a group. This does not deny access, it only removes specific set rights so the system falls back on default rights.
        /// </summary>
        /// <param name="objectId">ID of the object to check access to.</param>
        /// <param name="enovaType">The name of the enova type.</param>
        /// <param name="groupId">Id of the group to check.</param>
        [HttpDelete("ObjectAccess/id-{groupId}")]
        [Authorize(Roles = AuthService.AdminRole)]
        public AccessModel RemoveObjectAccess(int objectId, string enovaType, int groupId)
        {
            var group = _contextService.GetContext().FindObject(groupId, typeof(Group)) as Group;
            _accessRightService.RemoveAccessToObject(objectId, enovaType, group);
            return _accessRightService.GetAccessToType(enovaType, group);
        }

        /// <summary>
        /// Remove access to a specific object for a group. This does not deny access, it only removes specific set rights so the system falls back on default rights.
        /// </summary>
        /// <param name="objectId">ID of the object to check access to.</param>
        /// <param name="enovaType">The name of the enova type.</param>
        /// <param name="groupIdentifier">Identifier of the group to check.</param>
        [HttpDelete("ObjectAccess/groupIdentifier")]
        [Authorize(Roles = AuthService.AdminRole)]
        public AccessModel RemoveObjectAccess(int objectId, string enovaType, string groupIdentifier)
        {
            var group = _contextService.GetContext().FindObject(groupIdentifier, typeof(Group)) as Group;
            _accessRightService.RemoveAccessToObject(objectId, enovaType, group);
            return _accessRightService.GetAccessToType(enovaType, group);
        }

        #endregion
    }
}
