﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Interfaces;
using Wipcore.Enova.Core;

namespace Wipcore.eNova.Api.WebApi.Mappers.Customer
{
    public class CustomerPasswordMapper : IPropertyMapper
    {
        public List<string> Names => new List<string>() { "UserPassword" };
        public Type Type => typeof(EnovaCustomer);
        public bool InheritMapper => true;
        public int Priority => 0;
        public MapType MapType => MapType.MapFromAndToEnovaAllowed;


        public object GetEnovaProperty(BaseObject obj, string propertyName)
        {
            return String.Empty; //passwords can't be retrived
        }

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            if(value == null)
                return;

            var customer = (EnovaCustomer) obj;
            customer.Password = value.ToString();
        }
     
    }
}
