using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;
using Fasterflect;
using IdentityModel;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.Configuration;
using Wipcore.Core;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Models;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Api.OAuth;
using Wipcore.Enova.Api.WebApi.Helpers;
using Wipcore.Enova.Core;
using Wipcore.Enova.Generics;

namespace Wipcore.Enova.Api.WebApi.Services
{
    public class ContextService : IContextService
    {
        private readonly IHttpContextAccessor _httpAccessor;
        private readonly IConfigurationRoot _configuration;
        private readonly IAuthService _authService;
        private readonly MethodInfo _loginMethod = typeof(CmoContext).GetMethod("AdministratorLogin", BindingFlags.NonPublic | BindingFlags.Instance);


        public ContextService(IHttpContextAccessor httpAccessor, IConfigurationRoot configuration, IAuthService authService)
        {
            _httpAccessor = httpAccessor;
            _configuration = configuration;
            _authService = authService;
        }

        /// <summary>
        /// Get an EnovaContext for the current user.
        /// </summary>
        public Context GetContext()
        {
            //if a context has already been created for the request, then use that one
            var enovaContext = _httpAccessor.HttpContext.Items[ContextConstants.EnovaContextKey] as Context;
            if (enovaContext != null)
                return enovaContext;
            
            enovaContext = EnovaSystemFacade.Current.Connection.CreateContext();
            _httpAccessor.HttpContext.Items[ContextConstants.EnovaContextKey] = enovaContext;

            var requestContext = _httpAccessor.HttpContext.Items[ContextConstants.ContextModelKey] as ContextModel;
            if (requestContext == null)
                return enovaContext;
            
            //first read any values from market configuration
            if (!String.IsNullOrEmpty(requestContext.Market))
            {
                var config = _configuration.GetSection(requestContext.Market);

                if (config != null)
                {
                    var settings = config.GetChildren().ToList();
                    SetLanguage(enovaContext, settings.FirstOrDefault(x => x.Key == "language")?.Value);
                    SetCurrency(enovaContext, settings.FirstOrDefault(x => x.Key == "currency")?.Value);
                    SetTaxRule(enovaContext, settings.FirstOrDefault(x => x.Key == "taxrule")?.Value);
                }
            }
            
            //then login admin or customer
            if (_authService.IsLoggedInAsAdmin())
            {
                var alias = _authService.GetLoggedInAlias();
                var hash = _authService.GetPasswordHash();
                var cmoAdmin = _loginMethod.Invoke(enovaContext.GetCmoContext(), new object[]{alias, hash, null, CmoAdministrator.AdministratorRole.Any, null, null, false});
                var admin = EnovaObjectCreationHelper.CreateNew<EnovaAdministrator>(enovaContext, cmoAdmin);
                ClearSpecifiedContextValues(enovaContext, admin);//if the admin has specified values, clear other values
            }
            else if (_authService.IsLoggedInAsCustomer())
            {
                var adminAlreadyLoggedIn = enovaContext.IsLoggedIn();
                try
                {
                    if (!adminAlreadyLoggedIn)
                        LoginDefaultAdmin(enovaContext);

                    var identifier = _authService.GetLoggedInIdentifier();
                    var customer = EnovaCustomer.Find(enovaContext, identifier);
                    enovaContext.CustomerLogin(customer.ID);
                    ClearSpecifiedContextValues(enovaContext, customer);//if the customer has specified values, clear other values
                }
                finally
                {
                    if (!adminAlreadyLoggedIn)
                        enovaContext.Logout();
                }
            }
            
            //then override by url specified values
            SetLanguage(enovaContext, requestContext.Language);
            SetCurrency(enovaContext, requestContext.Currency);

            return enovaContext;
        }
        
        private void LoginDefaultAdmin(Context enovaContext)
        {
            var config = _configuration.GetSection("Enova").GetChildren().ToList();
            var username = config.First(x => x.Key == "Username").Value;
            var pass = config.First(x => x.Key == "Password").Value;

            enovaContext.Login(username, pass, updateLastLoginDate: false); 
        }

        private void SetTaxRule(Context enovaContext, string taxruleIdentifier)
        {
            if (String.IsNullOrEmpty(taxruleIdentifier))
                return;

            var taxrule = EnovaTaxationRule.Find(enovaContext, taxruleIdentifier);
            enovaContext.DefaultTaxationRule = taxrule;
        }

        private void SetLanguage(Context enovaContext, string languageIdentifier)
        {
            if (String.IsNullOrEmpty(languageIdentifier))
                return;

            var language = EnovaLanguage.Find(enovaContext, languageIdentifier);
            enovaContext.CurrentLanguage = language;
        }

        private void SetCurrency(Context enovaContext, string currencyIdentifier)
        {
            if (String.IsNullOrEmpty(currencyIdentifier))
                return;

            var currency = EnovaCurrency.Find(enovaContext, currencyIdentifier);
            enovaContext.CurrentCurrency = currency;
        }

        private void ClearSpecifiedContextValues(Context enovaContext, User user)
        {
            if (user.Language != null)
                enovaContext.CurrentLanguage = null;
            if (user.Currency != null)
                enovaContext.CurrentCurrency = null;
        }
    }
}
