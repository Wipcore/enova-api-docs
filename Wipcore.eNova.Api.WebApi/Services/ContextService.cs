using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.Configuration;
using Wipcore.Core.SessionObjects;
using Wipcore.eNova.Api.WebApi.Models;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Core;
using Wipcore.Enova.Generics;

namespace Wipcore.eNova.Api.WebApi.Services
{
    

    public class ContextService : IContextService
    {
        public const string ContextModelKey = "requestContext";
        public const string EnovaContextKey = "enovaContext";

        private readonly IHttpContextAccessor _httpAccessor;
        private readonly IConfigurationRoot _configuration;
        private readonly ObjectCache _cache;
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        //private readonly ConcurrentDictionary<string, bool> _adminLogins = new ConcurrentDictionary<string, Context>();


        public ContextService(IHttpContextAccessor httpAccessor, IConfigurationRoot configuration, ObjectCache cache)
        {
            _httpAccessor = httpAccessor;
            _configuration = configuration;
            _cache = cache;
        }

        public Context GetContext()
        {
            //TODO threat safety
            var enovaContext = _httpAccessor.HttpContext.Items[EnovaContextKey] as Context;
            if (enovaContext != null)
                return enovaContext;
            
            enovaContext = EnovaSystemFacade.Current.Connection.CreateContext();
            _httpAccessor.HttpContext.Items[EnovaContextKey] = enovaContext;

            var requestContext = _httpAccessor.HttpContext.Items[ContextModelKey] as ContextModel;
            if (requestContext == null)
                return enovaContext;//TODO default values if non specified?

            //first read any values from market configuration
            if (!String.IsNullOrEmpty(requestContext.Market))
            {
                var config = _configuration.GetSection(requestContext.Market);

                if (config != null)
                {
                    var settings = config.GetChildren().ToList();
                    SetLanguage(enovaContext, settings.FirstOrDefault(x => x.Key == "language")?.Value);
                    SetCurrency(enovaContext, settings.FirstOrDefault(x => x.Key == "currency")?.Value);
                }
            }

            //then login admin
            if (!String.IsNullOrEmpty(requestContext.Admin))
            {
                var previouslyLoggedIn = _cache.Contains(requestContext.Admin +"_loggedin");
                var admin = enovaContext.Login(requestContext.Admin, requestContext.Pass, updateLastLoginDate: !previouslyLoggedIn);
                _cache.Add(requestContext.Admin + "_loggedin", true, DateTimeOffset.Now.AddDays(1));
                //if the admin has specified values, clear other values
                ClearSpecifiedContextValues(enovaContext, admin);
            }

            //then login customer
            if (!String.IsNullOrEmpty(requestContext.Customer))
            {
                var adminAlreadyLoggedIn = enovaContext.IsLoggedIn();
                try
                {
                    if(!adminAlreadyLoggedIn)
                        LoginDefaultAdmin(enovaContext);

                    var customer = EnovaCustomer.Find(enovaContext, requestContext.Customer);
                    enovaContext.CustomerLogin(customer.ID);
                    //if the customer has specified values, clear other values
                    ClearSpecifiedContextValues(enovaContext, customer);
                }
                finally 
                {
                    if(!adminAlreadyLoggedIn)
                        enovaContext.Logout();
                }
            }

            //then override by url specified values
            SetLanguage(enovaContext, requestContext.Language);
            SetCurrency(enovaContext, requestContext.Currency);

            return enovaContext;
        }

        //public Context GetAdminContext(string username = null, string password = null)
        //{
        //    if (String.IsNullOrEmpty(username))
        //    {
        //        var config = _configuration.GetSection("eNova").GetChildren().ToList();
        //        username = config.First(x => x.Key == "Username").Value;
        //        password = config.First(x => x.Key == "Password").Value;
        //    }

        //    var previouslyLoggedIn = _cache.Contains(username);


        //    return adminContext;
        //}

        private void LoginDefaultAdmin(Context enovaContext)
        {
            var config = _configuration.GetSection("eNova").GetChildren().ToList();
            var username = config.First(x => x.Key == "Username").Value;
            var pass = config.First(x => x.Key == "Password").Value;

            enovaContext.Login(username, pass, updateLastLoginDate: false); 
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
