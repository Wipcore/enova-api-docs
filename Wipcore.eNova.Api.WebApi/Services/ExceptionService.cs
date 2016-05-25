using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Wipcore.Core;
using Wipcore.eNova.Api.WebApi.Helpers;
using Wipcore.Enova.Api.Interfaces;

namespace Wipcore.eNova.Api.WebApi.Services
{
    public class ExceptionService : IExceptionService
    {
        private readonly IHostingEnvironment _environment;
        private readonly ILogger _log;

        public ExceptionService(IHostingEnvironment environment, ILoggerFactory loggerFactory)
        {
            _environment = environment;
            _log = loggerFactory.CreateLogger(GetType().Namespace);
        }


        public void HandleControllerException(ActionExecutedContext context)
        {
            if (context.Exception == null || context.ExceptionHandled)
                return;

            var statusCode = GetStatusCodeForException(context.Exception);
            var isDevelopment = _environment.IsDevelopment();
            if (isDevelopment) //if in development, only set status code, as full error page is shown by default
            {
                context.HttpContext.Response.StatusCode = (int)statusCode;
                return;
            }
                

            _log.LogError(context.Exception.ToString());

            context.ExceptionHandled = true;
            context.Result = new ContentResult() {Content = $"{context.Exception.GetType().Name}\n{context.Exception.Message}",
                StatusCode = (int)statusCode };
        }

        public HttpStatusCode GetStatusCodeForException(Exception exception)
        {
            //ObjectHasBeenModifiedException TODO might need special handling for this one

            var statusCode = HttpStatusCode.InternalServerError; //everything but known errors below are treated as server errors

            switch (exception.GetType().Name)
            {
                case "HttpException":
                    statusCode = ((HttpException) exception).StatusCode;
                    break;
                case "MissingMemberException":
                case "PropertyCannotBeSetException":
                case "IdentifierNotUniqueException":
                case "ParserErrorException":
                case "PropertyDoesntExistException":
                case "PropertyCannotBeReadException":
                case "NameNotDefinedException":
                case "AdministratorAliasNotUniqueException":
                case "CustomerAliasNotUniqueException":
                case "CustomerAliasPasswordCombinationNotUniqueException":
                case "CartItemsMustBeDeletedFirstException":
                case "OrderItemsMustBeDeletedFirstException":
                case "AddressesMustBeDeletedFirstException":
                case "ContactsMustBeDeletedFirstException":
                case "AttributeValuesMustBeDeletedFirstException":
                case "ShippingTypeCostsMustBeDeletedFirstException":
                case "PaymentTypeCostsMustBeDeletedFirstException":
                case "WarehouseCompartmentsMustBeDeletedFirstException":
                case "PromoConditionsMustBeDeletedFirstException":
                case "PromoResultsMustBeDeletedFirstException":
                case "LinkedPriceListsMustBeDeletedFirstException":
                case "GiftVoucherAlreadyUsedException":
                case "InvalidProductQuantityException":
                case "InvalidAmountStringException":
                case "RecursionDepthExceededException":
                        //login-related errors
                case "UserIsDisabledException":
                case "PasswordHasExpiredException":
                case "OldPasswordNotAllowedException":
                case "IncorrectPasswordException":
                case "UserNotFoundOrIncorrectPasswordException":
                case "UserNotUniqueException":
                    statusCode = HttpStatusCode.BadRequest;
                    break;
                case "UserNotFoundException":
                case "ObjectNotFoundException":
                case "ObjectHasBeenDeletedException":
                case "PriceListProductNotFoundException":
                    statusCode = HttpStatusCode.NotFound;
                    break;
                case "AccessDeniedException":
                case "ObjectTypeAccessDeniedException":
                case "ObjectHasBeenDisabledException":
                case "ParentObjectAccessDeniedException":
                case "ObjectIsNotVisibleException":
                case "ObjectIsAPreviewObjectException":
                case "NotLoggedInException":
                    statusCode = HttpStatusCode.Unauthorized;
                    break;
            }
            return statusCode;
        }
    }
}
