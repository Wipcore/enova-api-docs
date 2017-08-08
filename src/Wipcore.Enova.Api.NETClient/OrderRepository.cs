﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Api.Abstractions.Models;
using Wipcore.Enova.Api.Abstractions.Models.EnovaTypes.Order;

namespace Wipcore.eNova.Api.NETClient
{
    public class OrderRepository<TOrderModel> where TOrderModel : OrderModel
    {
        private readonly IApiRepository _apiRepository;

        public OrderRepository(IApiRepository apiRepository)
        {
            _apiRepository = apiRepository;
        }

        public TOrderModel GetSavedOrder(int id, QueryModel queryModel = null, ContextModel contextModel = null)
        {
            return _apiRepository.GetObject<TOrderModel>(id, queryModel, contextModel);
        }

        public TOrderModel GetSavedOrder(string identifier, QueryModel queryModel = null, ContextModel contextModel = null)
        {
            return _apiRepository.GetObject<TOrderModel>(identifier, queryModel, contextModel);
        }

        public TOrderModel CreateOrUpdateOrder(TOrderModel order, ContextModel contextModel = null, bool verifyIdentifierNotTaken = true)
        {
            return (TOrderModel)_apiRepository.SaveObject<TOrderModel>(JObject.FromObject(order), contextModel: contextModel, verifyIdentifierNotTaken: verifyIdentifierNotTaken);
        }

        public bool DeleteOrder(string orderIdentifier) => _apiRepository.DeleteObject<TOrderModel>(orderIdentifier);

        public bool DeleteOrder(int orderId) => _apiRepository.DeleteObject<TOrderModel>(orderId);

    }
}
