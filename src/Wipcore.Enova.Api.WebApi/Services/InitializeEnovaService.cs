using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions;
using Wipcore.Enova.Api.Abstractions.Internal;
using Wipcore.Enova.Api.OAuth;
using Wipcore.Enova.Core;
using Wipcore.Enova.Generics;

namespace Wipcore.Enova.Api.WebApi.Services
{
    public class InitializeEnovaService : IInitializeEnovaService
    {
        private readonly IConfigurationRoot _cofigurationRoot;
        private readonly ILogger _log;

        public InitializeEnovaService(IConfigurationRoot cofigurationRoot, ILoggerFactory loggerFactory)
        {
            _cofigurationRoot = cofigurationRoot;
            _log = loggerFactory.CreateLogger(this.GetType());
        }

        public void InitializeEnova(Context context = null)
        {
            try
            {
                var runInit = _cofigurationRoot.GetValue<bool>("RunEnovaInit:RunEnovaInit", false);
                if (!runInit)
                {
                    _log.LogTrace("RunEnovaInit set to false. Skipping.");
                    return;
                }

                context = context ?? EnovaSystemFacade.Current.Connection.CreateContext();

                var initSetting = context.FindObject<EnovaGlobalSystemSettings>("EnovaInitialized");
                if (initSetting == null)
                {
                    initSetting = new EnovaGlobalSystemSettings(context) { Identifier = "EnovaInitialized", ValueBoolean = false };
                    initSetting.Save();
                }

                if (initSetting.ValueBoolean)
                {
                    _log.LogTrace("Enova already initialized. Skipping.");
                    return;
                }

                _log.LogInformation("Running enova initializer to seed default data.");

                var admin = CreateAdmin(context);
                var (swedish, english) = CreateLanguages(context);
                CreatedAdminGroup(context, admin, swedish, english);
                var customerGroup = CreateCustomerGroup(context, swedish, english);
                CreatePriceLists(context, swedish, english, customerGroup);
                CreateTax(context, swedish, english);
                CreateInvoiceStatuses(context, swedish, english);
                CreateCurrencies(context, swedish, english);
                CreateCountries(context, swedish, english);
                CreatePayments(context, swedish, english);
                CreateShippingStatuses(context, swedish, english);


                //mark as finished!
                initSetting.Edit();
                initSetting.ValueBoolean = true;
                initSetting.Save();

                _log.LogInformation("Finished enova initialization.");
            }
            catch (Exception e)
            {
                _log.LogCritical("Failure while running Enova initialization: {0}", e);
                throw;
            }
            
        }

        private void CreateShippingStatuses(Context context, EnovaLanguage swedish, EnovaLanguage english)
        {
            var statusList = new[]
            {
                new { Identifier = "NEW_INTERNET", Change = true, Inactive = false, Scrap = false, Reservation = false, SV = "Ny webborder", EN = "New weborder" },
                new { Identifier = "CC_PENDING_INTERNET", Change = true, Inactive = false, Scrap = false, Reservation = true, SV = "Väntar på betalning, webborder", EN = "CC pending internet" },
                new { Identifier = "SCRAP", Change = false, Inactive = true, Scrap = true, Reservation = false, SV = "Makulerad", EN = "Scrap" },
                new { Identifier = "NEW_INTERNET", Change = true, Inactive = false, Scrap = false, Reservation = false, SV = "Ny webborder", EN = "New weborder" },
                new { Identifier = "NEW_INTERNET", Change = true, Inactive = false, Scrap = false, Reservation = false, SV = "Ny webborder", EN = "New weborder" },
                new { Identifier = "NEW_INTERNET", Change = true, Inactive = false, Scrap = false, Reservation = false, SV = "Ny webborder", EN = "New weborder" },
                new { Identifier = "NEW_INTERNET", Change = true, Inactive = false, Scrap = false, Reservation = false, SV = "Ny webborder", EN = "New weborder" },
                new { Identifier = "NEW_INTERNET", Change = true, Inactive = false, Scrap = false, Reservation = false, SV = "Ny webborder", EN = "New weborder" },
                new { Identifier = "NEW_INTERNET", Change = true, Inactive = false, Scrap = false, Reservation = false, SV = "Ny webborder", EN = "New weborder" },
                new { Identifier = "NEW_INTERNET", Change = true, Inactive = false, Scrap = false, Reservation = false, SV = "Ny webborder", EN = "New weborder" },
                new { Identifier = "NEW_INTERNET", Change = true, Inactive = false, Scrap = false, Reservation = false, SV = "Ny webborder", EN = "New weborder" },
            };
        }

        private void CreatePayments(Context context, EnovaLanguage swedish, EnovaLanguage english)
        {
            var paymentSource = context.FindObject<EnovaPaymentSource>("CC");
            if (paymentSource == null)
            {
                paymentSource = new EnovaPaymentSource(context) {Identifier = "CC" };
                paymentSource.SetName("Kreditkort", swedish);
                paymentSource.SetName("Credit card", english);
                paymentSource.Save();
            }


            var paymentType = context.FindObject<EnovaPaymentType>("PAYMENT_INVOICE");
            if (paymentType == null)
            {
                paymentType = new EnovaPaymentType(context) {Identifier = "PAYMENT_INVOICE" };
                paymentType.SetName("Faktura", swedish);
                paymentType.SetName("Invoice", english);
                paymentType.Save();
            }
        }

        private void CreateCurrencies(Context context, EnovaLanguage swedish, EnovaLanguage english)
        {
            var currencyList = new[]
            {
                new { Identifier = "SEK", Prefix = "", Suffix = "kr", Conversion = 1d },
                new { Identifier = "NOK", Prefix = "", Suffix = "kr", Conversion = 0.8d },
                new { Identifier = "DKK", Prefix = "", Suffix = "kr", Conversion = 0.67d },
                new { Identifier = "EURO", Prefix = "", Suffix = "kr", Conversion = 0.09d },
                new { Identifier = "GBP", Prefix = "£", Suffix = "", Conversion = 0.08d },
                new { Identifier = "USD", Prefix = "$", Suffix = "", Conversion = 0.11d }
            };

            foreach (var c in currencyList)
            {
                var currency = context.FindObject<EnovaCurrency>(c.Identifier);
                if (currency == null)
                {
                    currency = new EnovaCurrency(context) {Identifier = c.Identifier, IsoCode = c.Identifier, Prefix = c.Prefix, Suffix = c.Suffix, ConversionFactor = c.Conversion};
                    currency.Save();
                }
            }
        }

        private void CreateCountries(Context context, EnovaLanguage swedish, EnovaLanguage english)
        {
            var countryList = new []{new {Identifier = "SE", SV = "Sverige", EN = "Sweden" } };//TODO way more countries, somehow
            foreach (var c in countryList)
            {
                var country = context.FindObject<EnovaCountry>(c.Identifier);
                if (country == null)
                {
                    country = new EnovaCountry(context)
                    {
                        Identifier = c.Identifier, IsoCode = c.Identifier, Language = swedish, DecimalSeparator = ",", ShortDateFormat = "yyyy-MM-dd",
                        LongDateFormat = "d MMMM yyyy", ShortTimeFormat = "HH:mm", LongTimeFormat = "HH:mm:ss",
                        TaxationRule = context.FindObject<EnovaTaxationRule>("STANDARD_TAX_SV"), Currency = context.FindObject<EnovaCurrency>("SEK")
                };
                    country.SetName(c.SV, swedish);
                    country.SetName(c.EN, english);
                    country.Save();
                }
                
            }
        }

        private void CreateInvoiceStatuses(Context context, EnovaLanguage swedish, EnovaLanguage english)
        {
            var statuses = new[] { new { Identifier = "PAID", SV = "Betald", EN = "Paid" },
                new { Identifier = "NOT_PAID", SV = "Ej betald", EN = "Not paid" }, new { Identifier = "PARTLY_PAID", SV = "Delbetald", EN = "Partly paid" } };

            foreach (var statuse in statuses)
            {
                var status = context.FindObject<EnovaInvoiceStatus>(statuse.Identifier);
                if (status == null)
                {
                    status = new EnovaInvoiceStatus(context) {Identifier = statuse.Identifier};
                    status.SetName(statuse.SV, swedish);
                    status.SetName(statuse.EN, english);
                    status.Save();
                }
            }
        }

        private void CreateTax(Context context, EnovaLanguage swedish, EnovaLanguage english)
        {
            var noTax = context.FindObject<EnovaTax>("VAT00");
            if (noTax == null)
            {
                noTax = new EnovaTax(context)
                {
                    Identifier = "VAT00",
                    Rate = 0 
                };
                noTax.SetName("Ingen skatt", swedish);
                noTax.SetName("No tax", english);
                noTax.Save();
            }


            var standardTax = context.FindObject<EnovaTax>("STANDARD_SV");
            if (standardTax == null)
            {
                standardTax = new EnovaTax(context)
                {
                    Identifier = "STANDARD_SV",
                    Rate = 25.0 // 25% moms
                };
                standardTax.SetName("Normalskatt", swedish);
                standardTax.SetName("Swedish normal tax", english);
                standardTax.Save();
            }

            var standardTaxationRule = context.FindObject<EnovaTaxationRule>("STANDARD_TAX_SV");
            if (standardTaxationRule == null)
            {
                standardTaxationRule = new EnovaTaxationRule(context)
                {
                    Identifier = "STANDARD_TAX_SV",
                    DefaultTax = standardTax
                };
                standardTaxationRule.SetName("Normalskattregel", swedish);
                standardTaxationRule.SetName("Swedish normal tax rule", english);
                standardTaxationRule.Save();
            }
        }

        private void CreatePriceLists(Context context, EnovaLanguage swedish, EnovaLanguage english, EnovaCustomerGroup customerGroup)
        {
            var standardPriceList = context.FindObject<EnovaPriceList>("STANDARD");
            if(standardPriceList == null)
                return;

            standardPriceList = new EnovaPriceList(context) {Identifier = "STANDARD" };
            standardPriceList.SetName("Standardprislista", swedish);
            standardPriceList.SetName("Standard pricelist", english);

            standardPriceList.Save();

            // The default customer group should be allowed to use the standard price list.
            standardPriceList.SetSpecificAccess(customerGroup, BaseObject.AccessRead | BaseObject.AccessUse);
        }

        private EnovaCustomerGroup CreateCustomerGroup(Context context, EnovaLanguage swedish, EnovaLanguage english)
        {
            var defaultGroup = context.FindObject<EnovaCustomerGroup>("DEFAULT");
            if(defaultGroup != null)
                return defaultGroup;

            defaultGroup = new EnovaCustomerGroup(context) {Identifier = "DEFAULT" };
            defaultGroup.SetName("Standardkundgrupp", swedish);
            defaultGroup.SetName("Default customergroup", english);
            defaultGroup.Save();

            return defaultGroup;
        }

        private (EnovaLanguage, EnovaLanguage) CreateLanguages(Context context)
        {
            //adding swedish and english by default
            var swedish = context.FindObject<EnovaLanguage>("SV");
            if (swedish == null)
            {
                swedish = new EnovaLanguage(context) {Identifier = "SV", IsoCode = "SV", Culture = "sv-SE" };
                swedish.Save();
            }

            var english = context.FindObject<EnovaLanguage>("EN");
            if (english == null)
            {
                english = new EnovaLanguage(context) {Identifier = "EN", IsoCode = "EN", Culture = "en-GB" };
                english.Save();
            }

            swedish.Edit();
            swedish.SetName("Svenska", swedish);
            swedish.SetName("Swedish", english);
            swedish.SecondaryLanguage = english;
            swedish.Save();

            english.Edit();
            english.SetName("Engelska", swedish);
            english.SetName("English", english);
            english.Save();

            return (swedish, english);
        }

        private void CreatedAdminGroup(Context context, EnovaAdministrator admin, EnovaLanguage swedish, EnovaLanguage english)
        {
            var identifier = "ADMINISTRATORS";
            var adminGroup = context.FindObject<EnovaAdministratorGroup>(identifier);
            if(adminGroup != null)
                return;

            _log.LogInformation("Creating admin group {0}", identifier);
            adminGroup = new EnovaAdministratorGroup(context) {Identifier = identifier } ;
            adminGroup.SetName("Administrators", english);
            adminGroup.SetName("Administratörer", swedish);
            adminGroup.Save();

            // Default should be that everyone who belongs to the group has full access to all objects.
            BaseObject.SetDefaultAccess(context, adminGroup, BaseObject.AccessAll);

            adminGroup.AddUser(admin);
        }

        private EnovaAdministrator CreateAdmin(Context context)
        {
            var username = _cofigurationRoot.GetValue<string>("Enova:Username", "wadmin");
            var password = _cofigurationRoot.GetValue<string>("Enova:Password", "wadmin");

            var admin = context.FindObject<EnovaAdministrator>(username);
            if (admin != null)
            {
                context.Login(username, password);
                return admin;
            }

            _log.LogInformation("Creating admin {0}", username);
            admin = new EnovaAdministrator(context) {Identifier = username, Alias = username, Password = username};
            admin.Save();

            context.Login(username, password);

            _log.LogInformation("Setting default read access to everyone on base object");
            BaseObject.SetDefaultAccess(context, BaseObject.AccessRead);

            return admin;
        }
    }
}
