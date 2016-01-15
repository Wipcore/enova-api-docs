using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fasterflect;
using Wipcore.Core;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Core;
using Wipcore.Enova.Generics;
using Wipcore.eNova.Api.WebApi.Models;

namespace Wipcore.eNova.Api.WebApi.Mappers
{
    public class NewProductMapper
    {
        protected static readonly List<string> _propertyCandidatesToEnova = new List<string>();
        protected static readonly List<string> _propertyCandidatesFromEnova = new List<string>();

        protected Context Context
        {
            get
            {
                return EnovaContextProvider.GetCurrentContext();
            }
        }

        public EnovaBaseProduct MapToEnova(ProductModel model)
        {
            //var product = base.MapToEnova(model);

            EnovaBaseProduct enovaObject = null;
            //if an ID is specified, then there must be an object that has this ID
            if (model.ID != WipConstants.InvalidId)
            {
                enovaObject = Context.FindObjectThrowException<EnovaBaseProduct>(model.ID);
            }
            //otherwise check by identifier
            else if (!String.IsNullOrEmpty(model.Identifier))
            {
                enovaObject = Context.FindObject<EnovaBaseProduct>(model.Identifier);
            }

            //if no object found, make a new one
            if (enovaObject == null)
                enovaObject = new EnovaBaseProduct(Context);

            enovaObject.Edit();
            //this.MakeSureObjectIsInEditMode(enovaObject);

            //auto map simple properties
            foreach (var propertyCandidate in _propertyCandidatesToEnova)
            {
                var modelValue = model.TryGetPropertyValue(propertyCandidate);
                if (modelValue != null)
                {
                    //LogWriter.Current.Trace("Trying to set property '{0}' to '{1}'", propertyCandidate, modelValue.ToString());
                    enovaObject.TrySetPropertyValue(propertyCandidate, modelValue);
                }
            }

            return enovaObject;
        }

        public ProductModel MapFromEnova(EnovaBaseProduct product)
        {
            //throw new NotImplementedException();
            //TODO: not complete
            ProductModel model = new ProductModel();

            //auto map simple properties
            foreach (var propertyCandidate in _propertyCandidatesFromEnova)
            {
                model.TrySetPropertyValue(propertyCandidate, product.TryGetPropertyValue(propertyCandidate));
            }
            //model.Identifier = product.Identifier;
            //model.Name = new List<LocalizedString>();
            //model.Name.Add(new LocalizedString { Culture = "en-GB", Value = product.Name });
            return model;
        }
    }
}
