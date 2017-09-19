using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Wipcore.Core;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Api.Abstractions.Internal;
using Wipcore.Enova.Api.Abstractions.Models;
using Wipcore.Enova.Api.OAuth;
using Wipcore.Enova.Api.WebApi.Helpers;
using Wipcore.Enova.Core;
using WipConstants = Wipcore.Enova.Api.Abstractions.WipConstants;

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
            var enovaContext = _httpAccessor.HttpContext.Items[WipConstants.EnovaContextKey] as Context;
            if (enovaContext != null)
                return enovaContext;
            
            enovaContext = EnovaSystemFacade.Current.Connection.CreateContext();
            _httpAccessor.HttpContext.Items[WipConstants.EnovaContextKey] = enovaContext;

            var defaultGroups = _configuration["EnovaSettings:DefaultContextGroups"]?.Split(',').Select(x => x.Trim()).Where(x => x != String.Empty).ToList();
            defaultGroups?.ForEach(x => enovaContext.AddGroup(x.Trim()));
            
            var requestContext = _httpAccessor.HttpContext.Items[WipConstants.ContextModelKey] as ContextModel;
            
            //first read any values from market configuration
            if (!String.IsNullOrEmpty(requestContext?.Market))
            {
                var config = _configuration.GetSection(requestContext.Market);

                if (config != null)
                {
                    var settings = config.GetChildren().ToList();
                    SetLanguage(enovaContext, settings.FirstOrDefault(x => x.Key == "language")?.Value);
                    SetCurrency(enovaContext, settings.FirstOrDefault(x => x.Key == "currency")?.Value);
                    SetTaxRule(enovaContext, settings.FirstOrDefault(x => x.Key == "taxrule")?.Value);
                    SetThousandsSeparator(enovaContext, settings.FirstOrDefault(x => x.Key == "thousandseparator")?.Value);
                    SetDecimalSeparator(enovaContext, settings.FirstOrDefault(x => x.Key == "decimalseparator")?.Value);
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
            
            if (requestContext != null)
            {
                //then override by url specified values
                SetLanguage(enovaContext, requestContext.Language);
                SetCurrency(enovaContext, requestContext.Currency);
                SetThousandsSeparator(enovaContext, requestContext.ThousandSeparator);
                SetDecimalSeparator(enovaContext, requestContext.DecimalSeparator);
            }

            return enovaContext;
        }
        
        private void LoginDefaultAdmin(Context enovaContext)
        {
            var config = _configuration.GetSection("Enova").GetChildren().ToList();
            var alias = config.First(x => x.Key == "Username").Value;
            var pass = config.First(x => x.Key == "Password").Value;

            enovaContext.Login(alias, pass, updateLastLoginDate: false); 
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

        private void SetDecimalSeparator(Context enovaContext, string separator)
        {
            if (String.IsNullOrEmpty(separator))
                return;

            enovaContext.CurrentDecimalSeparator = separator;
        }

        private void SetThousandsSeparator(Context enovaContext, string separator)
        {
            if (String.IsNullOrEmpty(separator))
                return;

            enovaContext.CurrentThousandsSeparator = separator;
        }

        private void ClearSpecifiedContextValues(Context enovaContext, User user)
        {
            if (user.Language != null)
                enovaContext.CurrentLanguage = null;
            if (user.Currency != null)
                enovaContext.CurrentCurrency = null;
            if (!String.IsNullOrEmpty(user.DecimalSeparator))
                enovaContext.CurrentDecimalSeparator = null;
            if (!String.IsNullOrEmpty(user.ThousandsSeparator))
                enovaContext.CurrentThousandsSeparator = null;
        }
    }
}
