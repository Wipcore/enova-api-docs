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
    /// <summary>
    /// This service seeds default Enova data into a new database.
    /// </summary>
    public class InitializeEnovaService : IInitializeEnovaService
    {
        private readonly IConfigurationRoot _cofigurationRoot;
        private readonly ILogger _log;

        public InitializeEnovaService(IConfigurationRoot cofigurationRoot, ILoggerFactory loggerFactory)
        {
            _cofigurationRoot = cofigurationRoot;
            _log = loggerFactory.CreateLogger(this.GetType());
        }

        /// <summary>
        /// Run Enova initializer. If no context given, a new one will be created on the systemfacade. If forcerun is true it will bypass settings to turn it off.
        /// </summary>
        public void InitializeEnova(Context context = null, bool forceRun = false)
        {
            try
            {
                if (forceRun)
                {
                    _log.LogInformation("Forcerun on RunEnovaEdit is set.");
                }
                else
                {
                    var runInit = _cofigurationRoot.GetValue<bool>("EnovaSettings:RunEnovaInit", false);
                    if (!runInit)
                    {
                        _log.LogTrace("RunEnovaInit set to false. Skipping.");
                        return;
                    }
                }
                
                context = context ?? EnovaSystemFacade.Current.Connection.CreateContext();

                var initSetting = context.FindObject<EnovaGlobalSystemSettings>("EnovaInitialized");
                if (initSetting == null)
                {
                    initSetting = new EnovaGlobalSystemSettings(context) { Identifier = "EnovaInitialized", ValueBoolean = false };
                    initSetting.Save();
                }

                if (initSetting.ValueBoolean && !forceRun)
                {
                    _log.LogTrace("Enova already initialized. Skipping.");
                    return;
                }

                _log.LogInformation("Running enova initializer to seed default data.");
                
                var admin = CreateAdmin(context);
                var (swedish, english) = CreateLanguages(context);
                CreatedAdminGroup(context, admin, swedish, english);
                SetBasicAccess(context);
                var customerGroup = CreateCustomerGroup(context, swedish, english);
                CreatePriceLists(context, swedish, english, customerGroup);
                CreateTax(context, swedish, english);
                CreateInvoiceStatuses(context, swedish, english);
                CreateCurrencies(context, swedish, english);
                CreateCountries(context, swedish, english);
                CreatePayments(context, swedish, english);
                CreateShippingStatuses(context, swedish, english);
                CreateWarehouse(context, swedish, english);

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
            admin = new EnovaAdministrator(context) { Identifier = username, Alias = username, Password = username };
            admin.Save();

            context.Login(username, password);

            return admin;
        }

        private (EnovaLanguage, EnovaLanguage) CreateLanguages(Context context)
        {
            //adding swedish and english by default
            var swedish = context.FindObject<EnovaLanguage>("SV");
            if (swedish == null)
            {
                swedish = new EnovaLanguage(context) { Identifier = "SV", IsoCode = "SV", Culture = "sv-SE" };
                swedish.Save();

                _log.LogInformation("Created language: {0}", swedish.Identifier);
            }

            var english = context.FindObject<EnovaLanguage>("EN");
            if (english == null)
            {
                english = new EnovaLanguage(context) { Identifier = "EN", IsoCode = "EN", Culture = "en-GB" };
                english.Save();

                _log.LogInformation("Created language: {0}", english.Identifier);
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
            if (adminGroup != null)
                return;

            _log.LogInformation("Creating superadmin group {0}", identifier);
            adminGroup = new EnovaAdministratorGroup(context) { Identifier = identifier };
            adminGroup.SetName("Administrators", english);
            adminGroup.SetName("Administratörer", swedish);
            adminGroup.Save();

            // Default should be that everyone who belongs to the group has full access to all objects.
            BaseObject.SetDefaultAccess(context, adminGroup, BaseObject.AccessAll);

            adminGroup.AddUser(admin);
        }

        private EnovaCustomerGroup CreateCustomerGroup(Context context, EnovaLanguage swedish, EnovaLanguage english)
        {
            var defaultGroup = context.FindObject<EnovaCustomerGroup>("DEFAULT");
            if (defaultGroup != null)
                return defaultGroup;

            defaultGroup = new EnovaCustomerGroup(context) { Identifier = "DEFAULT" };
            defaultGroup.SetName("Standardkundgrupp", swedish);
            defaultGroup.SetName("Default customergroup", english);
            defaultGroup.Save();

            _log.LogInformation("Created default customergroup {0}.", defaultGroup.Identifier);

            return defaultGroup;
        }

        private void CreatePriceLists(Context context, EnovaLanguage swedish, EnovaLanguage english, EnovaCustomerGroup customerGroup)
        {
            var standardPriceList = context.FindObject<EnovaPriceList>("STANDARD");
            if (standardPriceList != null)
                return;

            standardPriceList = new EnovaPriceList(context) { Identifier = "STANDARD" };
            standardPriceList.SetName("Standardprislista", swedish);
            standardPriceList.SetName("Standard pricelist", english);

            standardPriceList.Save();

            // The default customer group should be allowed to use the standard price list.
            standardPriceList.SetSpecificAccess(customerGroup, BaseObject.AccessRead | BaseObject.AccessUse);

            _log.LogInformation("Created default pricelist {0} and gave the default customer group access to it.", standardPriceList.Identifier);
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

                _log.LogInformation("Created default zero tax {0}.", noTax.Identifier);
            }


            var standardTax = context.FindObject<EnovaTax>("STANDARD_TAX_SV");
            if (standardTax == null)
            {
                standardTax = new EnovaTax(context)
                {
                    Identifier = "STANDARD_TAX_SV",
                    Rate = 25.0 // 25% moms
                };
                standardTax.SetName("Normalskatt", swedish);
                standardTax.SetName("Swedish normal tax", english);
                standardTax.Save();

                _log.LogInformation("Created default standard tax {0}.", standardTax.Identifier);
            }

            var standardTaxationRule = context.FindObject<EnovaTaxationRule>("STANDARD_TAX");
            if (standardTaxationRule == null)
            {
                standardTaxationRule = new EnovaTaxationRule(context)
                {
                    Identifier = "STANDARD_TAX",
                    DefaultTax = standardTax
                };
                standardTaxationRule.SetName("Normalskattregel", swedish);
                standardTaxationRule.SetName("Swedish normal tax rule", english);
                standardTaxationRule.Save();

                _log.LogInformation("Created default taxation rule {0}.", standardTaxationRule.Identifier);
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
                    status = new EnovaInvoiceStatus(context) { Identifier = statuse.Identifier };
                    status.SetName(statuse.SV, swedish);
                    status.SetName(statuse.EN, english);
                    status.Save();

                    _log.LogInformation("Created invoice status {0}.", status.Identifier);
                }
            }
        }

        private void CreateCountries(Context context, EnovaLanguage swedish, EnovaLanguage english)
        {
            var countryList = new[] { new { Identifier = "SE", SV = "Sverige", EN = "Sweden" } };//TODO way more countries, somehow
            foreach (var c in countryList)
            {
                var country = context.FindObject<EnovaCountry>(c.Identifier);
                if (country == null)
                {
                    country = new EnovaCountry(context)
                    {
                        Identifier = c.Identifier,
                        IsoCode = c.Identifier,
                        Language = swedish,
                        DecimalSeparator = ",",
                        ShortDateFormat = "yyyy-MM-dd",
                        LongDateFormat = "d MMMM yyyy",
                        ShortTimeFormat = "HH:mm",
                        LongTimeFormat = "HH:mm:ss",
                        TaxationRule = context.FindObject<EnovaTaxationRule>("STANDARD_TAX_SV"),
                        Currency = context.FindObject<EnovaCurrency>("SEK")
                    };
                    country.SetName(c.SV, swedish);
                    country.SetName(c.EN, english);
                    country.Save();

                    _log.LogInformation("Created country {0}.", country.Identifier);
                }

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
                    currency = new EnovaCurrency(context) { Identifier = c.Identifier, IsoCode = c.Identifier, Prefix = c.Prefix, Suffix = c.Suffix, ConversionFactor = c.Conversion };
                    currency.Save();

                    _log.LogInformation("Created currency {0}.", currency.Identifier);
                }
            }
        }

        private void CreatePayments(Context context, EnovaLanguage swedish, EnovaLanguage english)
        {
            var paymentSource = context.FindObject<EnovaPaymentSource>("CC");
            if (paymentSource == null)
            {
                paymentSource = new EnovaPaymentSource(context) { Identifier = "CC" };
                paymentSource.SetName("Kreditkort", swedish);
                paymentSource.SetName("Credit card", english);
                paymentSource.Save();

                _log.LogInformation("Created payment source {0}.", paymentSource.Identifier);
            }


            var paymentType = context.FindObject<EnovaPaymentType>("PAYMENT_INVOICE");
            if (paymentType == null)
            {
                paymentType = new EnovaPaymentType(context) { Identifier = "PAYMENT_INVOICE" };
                paymentType.SetName("Faktura", swedish);
                paymentType.SetName("Invoice", english);
                paymentType.Save();

                _log.LogInformation("Created payment type {0}.", paymentType.Identifier);
            }
        }

        private void CreateShippingStatuses(Context context, EnovaLanguage swedish, EnovaLanguage english)
        {
            var shippingType = context.FindObject<EnovaShippingType>("NORMAL");
            if (shippingType == null)
            {
                shippingType = new EnovaShippingType(context) {Identifier = "NORMAL" };
                shippingType.SetName("Normal", swedish);
                shippingType.SetName("Normal", english);
                shippingType.Save();
            }


            //to these list picking/packing etc can be added if needed. Stock increase/decrease and other settings too if/when needed
            var statusList = new[]
            {
                new { Identifier = "NEW_INTERNET", Change = true, Inactive = false, Scrap = false, Reservation = false, SV = "Ny webborder", EN = "New weborder", Dest = "PENDING_PAYMENT,VERIFIED_INTERNET,SCRAP" },
                new { Identifier = "NEW_INTERNAL", Change = true, Inactive = false, Scrap = false, Reservation = true, SV = "Ny internorder", EN = "New internalorder", Dest = "VERIFIED_INTERNET,SCRAP" },
                new { Identifier = "PENDING_PAYMENT", Change = true, Inactive = false, Scrap = false, Reservation = true, SV = "Väntar på betalning, webborder", EN = "Pending payment", Dest = "FAILED_SETTLE,VERIFIED_INTERNET,SCRAP" },
                new { Identifier = "SCRAP", Change = false, Inactive = true, Scrap = true, Reservation = false, SV = "Makulerad", EN = "Scrap", Dest = "" },
                new { Identifier = "VERIFIED_INTERNET", Change = true, Inactive = false, Scrap = false, Reservation = true, SV = "Godkänd webborder", EN = "Verified weborder", Dest = "SCRAP,REST,SHIPPING,DONE" },
                new { Identifier = "REST", Change = true, Inactive = false, Scrap = false, Reservation = true, SV = "Restnoterad", EN = "Backorder", Dest = "SCRAP,SHIPPING,DONE" },
                new { Identifier = "FAILED_SETTLE", Change = false, Inactive = false, Scrap = false, Reservation = false, SV = "Betalning misslyckades", EN = "Failed settle", Dest = "PENDING_PAYMENT,SCRAP" },
                new { Identifier = "SHIPPING", Change = false, Inactive = false, Scrap = false, Reservation = false, SV = "Fraktas", EN = "Shipping", Dest = "DONE" },
                new { Identifier = "DONE", Change = false, Inactive = true, Scrap = false, Reservation = false, SV = "Betald och klar", EN = "Done", Dest = "" },
            };

            //add the statuses
            foreach (var s in statusList)
            {
                var status = context.FindObject<EnovaShippingStatus>(s.Identifier);
                if (status != null)
                    continue;

                status = new EnovaShippingStatus(context)
                {
                    Identifier = s.Identifier,
                    AllowOrderChange = s.Change,
                    InactiveOrder = s.Inactive,
                    IncludeInReservation = s.Reservation,
                    IsScrapStatus = s.Scrap
                };
                status.SetName(s.SV, swedish);
                status.SetName(s.EN, english);
                status.Save();

                _log.LogInformation("Created shipping status {0}.", status.Identifier);
            }

            //then add the connections between them
            foreach (var s in statusList)
            {
                foreach (var destinationIdentifier in s.Dest.Split(',').Where(x => !String.IsNullOrEmpty(x)))
                {
                    var source = context.FindObject<EnovaShippingStatus>(s.Identifier);
                    var destination = context.FindObject<EnovaShippingStatus>(destinationIdentifier);

                    var ruleIdentifier = $"{source.Identifier}:{destination.Identifier}";
                    var rule = context.FindObject<EnovaConfigShippingStatusRule>(ruleIdentifier);
                    if (rule != null)
                        continue;

                    rule = new EnovaConfigShippingStatusRule(context)
                    {
                        Identifier = ruleIdentifier,
                        SourceStatus = source.Identifier,
                        DestinationStatus = destination.Identifier,
                        Allow = true
                    };
                    rule.SetName($"{source.GetName(swedish)} ({source.Identifier}) -> {destination.GetName(swedish)} ({destination.Identifier})", swedish);
                    rule.SetName($"{source.GetName(english)} ({source.Identifier}) -> {destination.GetName(english)} ({destination.Identifier})", english);
                    rule.Save();

                    _log.LogInformation("Created shipping rule {0}.", rule.Identifier);
                }
            }

        }

        private void CreateWarehouse(Context context, EnovaLanguage swedish, EnovaLanguage english)
        {
            var warehouse = context.FindObject<EnovaWarehouse>("DEFAULT_WAREHOUSE");
            if (warehouse == null)
            {
                warehouse = new EnovaWarehouse(context) { Identifier = "DEFAULT_WAREHOUSE" };
                warehouse.SetName("Standardlager", swedish);
                warehouse.SetName("Default warehouse", english);
                warehouse.Save();

                _log.LogInformation("Created default warehouse {0}.", warehouse.Identifier);
            }

            var admin = context.FindObject<EnovaAdministrator>(_cofigurationRoot.GetValue<string>("Enova:Username", "wadmin"));
            if (admin.DefaultWarehouse == null)
            {
                admin.Edit();
                admin.DefaultWarehouseID = warehouse.ID;
                admin.Save();
            }

            var warehouseSetting = context.FindObject<EnovaLocalSystemSettings>("LOCAL_PRIMARY_WAREHOUSE");
            if (warehouseSetting == null)
            {
                warehouseSetting = new EnovaLocalSystemSettings(context) { Identifier = "LOCAL_PRIMARY_WAREHOUSE" };
                warehouseSetting.SetValue("DEFAULT_WAREHOUSE;", swedish);
                warehouseSetting.SetValue("DEFAULT_WAREHOUSE;", english);
                warehouseSetting.Save();
            }
        }

        private void SetBasicAccess(Context context)
        {
            //byt default anyone should be able to register as customer, or create a cart and order
            const int defaultAccess = BaseObject.AccessCreate | BaseObject.AccessCreateLink | BaseObject.AccessRead |
                                      BaseObject.AccessUpdateLink | BaseObject.AccessWrite;

            _log.LogInformation("Setting default read access to everyone on base object.");
            BaseObject.SetDefaultAccess(context, BaseObject.AccessRead);

            _log.LogInformation("Setting default read/create/update access on customer, order, cart and payment.");

            context.SetDefaultAccess(defaultAccess, typeof(EnovaCustomer));
            context.SetDefaultAccess(defaultAccess, typeof(EnovaPayment));

            context.SetDefaultAccess(defaultAccess, typeof(EnovaOrder));
            context.SetDefaultAccess(defaultAccess, typeof(OrderItem));

            context.SetDefaultAccess(defaultAccess, typeof(EnovaCart));
            context.SetDefaultAccess(defaultAccess, typeof(CartItem));
        }
        
    }
}
