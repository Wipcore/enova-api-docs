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
        /// Get effective access rights on a certain type for a group or a user.
        /// </summary>
        AccessModel GetAccessToType(string enovaTypeName, BaseObject groupOrUserToCheck);

        /// <summary>
        /// Get effective access rights on a certain object for a group or a user.
        /// </summary>
        AccessModel GetAccessToObject(int objectId, string enovaTypeName, BaseObject groupOrUserToCheck);


        /// <summary>
        /// Set access to a type for a certain group. 
        /// </summary>
        void SetAccessToType(string enovaTypeName, Group group, AccessModel accessModel);


        /// <summary>
        /// Set access to an object for a certain group.
        /// </summary>
        void SetAccessToObject(int objectId, string enovaTypeName, Group group, AccessModel accessModel);

        /// <summary>
        /// Removes access to a type for a group. NOTE: removing is not the same as denying access.
        /// </summary>
        void RemoveAccessToType(string enovaTypeName, Group group);

        /// <summary>
        /// Removes access to an object for a group. NOTE: removing is not the same as denying access.
        /// </summary>
        void RemoveAccessToObject(int objectId, string enovaTypeName, Group group);

    }
}
