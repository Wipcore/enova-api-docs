﻿using System;
using System.Collections.Generic;
using System.Linq;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Core;

namespace Wipcore.eNova.Api.WebApi.Mappers.CompanyAndGroup
{
    public class GroupPriceListsMapper : IPropertyMapper
    {
        public List<string> Names => new List<string>() { "PriceLists" };
        public Type Type => typeof(CustomerGroup);
        public bool InheritMapper => true;
        public int Priority => 0;
        public MapType MapType => MapType.MapFromEnovaAllowed;
        public bool PostSaveSet => false;
        public bool FlattenMapping => false;

        public object GetEnovaProperty(BaseObject obj, string propertyName)
        {
            var priceListModels = new List<object>();
            var group = (CustomerGroup)obj;
            var priceLists = group.GetObjectsWithSpecificAccessRights(typeof(EnovaPriceList)).OfType<EnovaPriceList>();

            foreach (var priceList in priceLists)
            {
                var priceModel = new
                {
                    ID = priceList.ID,
                    Identifier = priceList.Identifier,
                    Name = priceList.Name
                };
                priceListModels.Add(priceModel);
            }

            return priceListModels;
        }

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            throw new InvalidOperationException();
        }
    }
}
