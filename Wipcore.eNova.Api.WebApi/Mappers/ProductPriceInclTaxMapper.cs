﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Core.SessionObjects;
using Wipcore.eNova.Api.Interfaces;
using Wipcore.Enova.Core;

namespace Wipcore.eNova.Api.WebApi.Mappers
{
    public class ProductPriceInclTaxMapper : IPropertyMapper
    {
        public bool InheritMapper => true;

        public int Priority => 0;

        public string Name => "priceincltax";


        public Type Type => typeof(EnovaBaseProduct);
        

        public object Map(BaseObject obj)
        {
            var product = (EnovaBaseProduct)obj;
            var price = product.GetPrice(includeTax:false);
            return price;
        }
    }
}
