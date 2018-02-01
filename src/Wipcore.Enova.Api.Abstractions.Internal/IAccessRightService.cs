using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions.Models;

namespace Wipcore.Enova.Api.Abstractions.Internal
{
    /// <summary>
    /// CRUD upon enova access rights.
    /// </summary>
    public interface IAccessRightService
    {
        /// <summary>
        /// Get effective access rights on a certain type for a group.
        /// </summary>
        AccessModel GetGroupAccessToType(string enovaTypeName, UserGroup group, bool includeEveryonesRights = true);


        /// <summary>
        /// Get effective access rights on a certain type for a user.
        /// </summary>
        AccessModel GetUserAccessToType(string enovaTypeName, User user, bool includeEveryonesRights = true);

        /// <summary>
        /// Get effective access rights on a certain object for a group or a user.
        /// </summary>
        AccessModel GetAccessToObject(int objectId, string enovaTypeName, BaseObject groupOrUserToCheck);


        /// <summary>
        /// Set access to a type for a certain group. 
        /// </summary>
        void SetAccessToType(UserGroup group, AccessModel accessModel);


        /// <summary>
        /// Set access to an object for a certain group.
        /// </summary>
        void SetAccessToObject(int objectId, UserGroup group, AccessModel accessModel);

        /// <summary>
        /// Removes access to a type for a group. NOTE: removing is not the same as denying access.
        /// </summary>
        void RemoveAccessToType(string enovaTypeName, UserGroup group);

        /// <summary>
        /// Removes access to an object for a group. NOTE: removing is not the same as denying access.
        /// </summary>
        void RemoveAccessToObject(int objectId, string enovaTypeName, UserGroup group);

    }
}
