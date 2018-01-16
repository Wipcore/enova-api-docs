using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Logging;
using Wipcore.Core;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions.Internal;
using Wipcore.Enova.Api.Abstractions.Models;
using Wipcore.Enova.Api.WebApi.Helpers;
using Wipcore.Enova.Generics;

namespace Wipcore.Enova.Api.WebApi.Services
{
    /// <inheritdoc />
    /// <summary>
    /// CRUD upon enova access rights.
    /// </summary>
    public class AccessRightService : IAccessRightService
    {
        private readonly IContextService _contextService;
        private readonly ILogger _logger;

        public AccessRightService(IContextService contextService, ILoggerFactory loggerFactory)
        {
            _contextService = contextService;
            _logger = loggerFactory.CreateLogger(this.GetType());
        }

        /// <inheritdoc />
        /// <summary>
        /// Get effective access rights on a certain type for a group or a user.
        /// </summary>
        public AccessModel GetAccessToType(string enovaTypeName, BaseObject groupOrUserToCheck)
        {
            var type = ReflectionHelper.GetTypeByName(enovaTypeName);
            if (type == null)
                throw new HttpException(HttpStatusCode.BadRequest, $"Cannot find enovaType {enovaTypeName}");

            var context = _contextService.GetContext();
            
            var access = groupOrUserToCheck == null ? context.GetActualAccess(type) : context.GetActualAccess(groupOrUserToCheck, type);

            var accessModel = MapAccessMaskToModel(access);

            return accessModel;
        }

        /// <inheritdoc />
        /// <summary>
        /// Get effective access rights on a certain object for a group or a user.
        /// </summary>
        public AccessModel GetAccessToObject(int objectId, string enovaTypeName, BaseObject groupOrUserToCheck)
        {
            var type = ReflectionHelper.GetTypeByName(enovaTypeName);
            if (type == null)
                throw new HttpException(HttpStatusCode.BadRequest, $"Cannot find enovaType {enovaTypeName}");

            var context = _contextService.GetContext();
            var obj = context.FindObject(objectId, type);
            
            var access = obj.GetSpecificAccess(groupOrUserToCheck);

            var accessModel = MapAccessMaskToModel(access);

            return accessModel;
        }

        /// <inheritdoc />
        /// <summary>
        /// Set access to a type for a certain group. 
        /// </summary>
        public void SetAccessToType(string enovaTypeName, Group group, AccessModel accessModel)
        {
            var type = ReflectionHelper.GetTypeByName(enovaTypeName);
            if (type == null)
                throw new HttpException(HttpStatusCode.BadRequest, $"Cannot find enovaType {enovaTypeName}");

            var currentAccess = GetAccessToType(enovaTypeName, group);
            var accessMask = MapModelToAccessMask(accessModel, currentAccess);

            var context = _contextService.GetContext();
            context.SetDefaultAccess(group, accessMask, type);

            _logger.LogInformation($"Set access to {accessModel} on type {enovaTypeName} for group with id {group.ID} and identifier {group.Identifier}");
        }

        /// <inheritdoc />
        /// <summary>
        /// Set access to an object for a certain group.
        /// </summary>
        public void SetAccessToObject(int objectId, string enovaTypeName, Group group, AccessModel accessModel)
        {
            var type = ReflectionHelper.GetTypeByName(enovaTypeName);
            if (type == null)
                throw new HttpException(HttpStatusCode.BadRequest, $"Cannot find enovaType {enovaTypeName}");

            var currentAccess = GetAccessToObject(objectId, enovaTypeName, group);
            var accessMask = MapModelToAccessMask(accessModel, currentAccess);
            var context = _contextService.GetContext();

            var obj = context.FindObject(objectId, type);
            obj.SetSpecificAccess(obj, accessMask);

            _logger.LogInformation($"Set access to {accessModel} on object of type {enovaTypeName} with id {objectId} for group with id {group.ID} and identifier {group.Identifier}");
        }

        /// <inheritdoc />
        /// <summary>
        /// Removes access to a type for a group. NOTE: removing is not the same as denying access.
        /// </summary>
        public void RemoveAccessToType(string enovaTypeName, Group group)
        {
            var type = ReflectionHelper.GetTypeByName(enovaTypeName);
            if (type == null)
                throw new HttpException(HttpStatusCode.BadRequest, $"Cannot find enovaType {enovaTypeName}");

            var context = _contextService.GetContext();
            context.RemoveDefaultAccess(group, type);

            _logger.LogInformation($"Removed specific access on type {enovaTypeName} for group with id {group.ID} and identifier {group.Identifier}");
        }

        /// <inheritdoc />
        /// <summary>
        /// Removes access to an object for a group. NOTE: removing is not the same as denying access.
        /// </summary>
        public void RemoveAccessToObject(int objectId, string enovaTypeName, Group group)
        {
            var type = ReflectionHelper.GetTypeByName(enovaTypeName);
            if (type == null)
                throw new HttpException(HttpStatusCode.BadRequest, $"Cannot find enovaType {enovaTypeName}");

            var context = _contextService.GetContext();

            var obj = context.FindObject(objectId, type);
            obj.RemoveSpecificAccess(obj);

            _logger.LogInformation($"Removed specific access on object of type {enovaTypeName} and id {objectId} for group with id {group.ID} and identifier {group.Identifier}");
        }

        private static int MapModelToAccessMask(AccessModel newAccess, AccessModel currentAccess)
        {
            //build a model where rights on new are taken if they exists, otherwise it defaults to current
            var accessModel = new AccessModel()
            {
                Read = newAccess.Read ?? currentAccess.Read,
                Write = newAccess.Write ?? currentAccess.Write,
                Delete = newAccess.Delete ?? currentAccess.Delete,
                Use = newAccess.Use ?? currentAccess.Use,
                CreateLink = newAccess.CreateLink ?? currentAccess.CreateLink,
                UpdateLink = newAccess.UpdateLink ?? currentAccess.UpdateLink,
                DeleteLink = newAccess.DeleteLink ?? currentAccess.DeleteLink,
                SetAccess = newAccess.SetAccess ?? currentAccess.SetAccess,
                ModifyDatabase = newAccess.ModifyDatabase ?? currentAccess.ModifyDatabase,
            };

            var accessMask = 0;
            if (accessModel.Read == true)
                accessMask += WipConstants.AccessRead;
            if (accessModel.Write == true)
                accessMask += WipConstants.AccessWrite;
            if (accessModel.Delete == true)
                accessMask += WipConstants.AccessDelete;
            if (accessModel.Use == true)
                accessMask += WipConstants.AccessUse;
            if (accessModel.CreateLink == true)
                accessMask += WipConstants.AccessCreateLink;
            if (accessModel.UpdateLink == true)
                accessMask += WipConstants.AccessUpdateLink;
            if (accessModel.DeleteLink == true)
                accessMask += WipConstants.AccessDeleteLink;
            if (accessModel.SetAccess == true)
                accessMask += WipConstants.AccessSetAccess;
            if (accessModel.ModifyDatabase == true)
                accessMask += WipConstants.AccessModifyDatabase;

            return accessMask;
        }
        
        private static AccessModel MapAccessMaskToModel(int access)
        {
            var accessModel = new AccessModel
            {
                Read = (access & WipConstants.AccessRead) == WipConstants.AccessRead,
                Write = (access & WipConstants.AccessWrite) == WipConstants.AccessWrite,
                Delete = (access & WipConstants.AccessDelete) == WipConstants.AccessDelete,
                Use = (access & WipConstants.AccessUse) == WipConstants.AccessUse,
                CreateLink = (access & WipConstants.AccessCreateLink) == WipConstants.AccessCreateLink,
                UpdateLink = (access & WipConstants.AccessUpdateLink) == WipConstants.AccessUpdateLink,
                DeleteLink = (access & WipConstants.AccessDeleteLink) == WipConstants.AccessDeleteLink,
                SetAccess = (access & WipConstants.AccessSetAccess) == WipConstants.AccessSetAccess,
                ModifyDatabase = (access & WipConstants.AccessModifyDatabase) == WipConstants.AccessModifyDatabase,
            };
            return accessModel;
        }
    }
}
