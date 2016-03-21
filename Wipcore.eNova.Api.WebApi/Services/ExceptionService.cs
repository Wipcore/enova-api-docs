using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Filters;
using Wipcore.Core;
using Wipcore.Enova.Api.Interfaces;

namespace Wipcore.eNova.Api.WebApi.Services
{
    public class ExceptionService : IExceptionService
    {
        private readonly IHostingEnvironment _environment;

        public ExceptionService(IHostingEnvironment environment)
        {
            _environment = environment;
        }


        public void HandleControllerException(ActionExecutedContext context)
        {
            if (context.Exception == null || context.ExceptionHandled)
                return;

            var isDevelopment = _environment.IsDevelopment();
            if (isDevelopment) //if in development, full error page should be shown
                return;

            var statusCode = GetStatusCodeForException(context.Exception);

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
