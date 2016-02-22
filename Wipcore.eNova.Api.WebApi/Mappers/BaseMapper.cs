using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Wipcore.Core.SessionObjects;
using Fasterflect;
using System.Reflection;
using Wipcore.Core;
using Wipcore.Enova.Core;
using Wipcore.Enova.Generics;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Wipcore.Enova.Api;
using Wipcore.Enova.Api.Models;
using Wipcore.Enova.Api.WebApi.Helpers;

namespace Wipcore.Enova.Api.WebApi.Mappers
{
    public abstract class BaseMapper<TEnova, TModel>
        where TEnova : BaseObject
        where TModel : BaseModel
    {
        // ReSharper disable StaticFieldInGenericType (Yes we want one static list for every combo of generic types)
        protected static readonly List<string> _propertyCandidatesToEnova = new List<string>();
        protected static readonly List<string> _propertyCandidatesFromEnova = new List<string>();

        protected readonly Func<TModel> _modelMaker;
        private readonly Func<Context, TEnova> _enovaMaker;

        /// <summary>
        /// Check which properties match between the enova object and the model. These can be auto mapped.
        /// </summary>
        static BaseMapper()
        {
            //Get most derived types, i.e. MyEnovaObject and MyModel
            var modelType = typeof(TModel).GetMostDerivedType(ReflectionHelper.GetAllAvailableTypes());
            //var enovaType = typeof(TEnova).GetMostDerivedType(ReflectionHelper.GetAllAvailableTypes());
            var enovaType = typeof(TEnova).GetMostDerivedTypes(ReflectionHelper.GetAllAvailableTypes()).OrderBy<Type, int>(
                x =>
                {
                    if (x.Namespace == typeof(BaseObject).Namespace)
                        return 1000;
                    if (x.Namespace == typeof(EnovaBaseProduct).Namespace)
                        return 100;
                    return 0;
                }).FirstOrDefault();

            //Save the properties which have the same name and the same type
            foreach (var property in modelType.GetProperties())
            {
                var propertyCandidate = enovaType.GetProperties(BindingFlags.Public | BindingFlags.Instance).FirstOrDefault(x => MatchingProperty(x, property));
                if (propertyCandidate == null)
                    continue;

                if (propertyCandidate.CanRead)
                    _propertyCandidatesFromEnova.Add(propertyCandidate.Name);

                if (propertyCandidate.CanWrite)
                    _propertyCandidatesToEnova.Add(propertyCandidate.Name);

            }

            //LogWriter.Current.Trace("AEnovaMapper: Found '{0}' property candidates for '{1}' from '{2}'", _propertyCandidatesFromEnova.Count, typeof(TEnova), typeof(TModel));
            //LogWriter.Current.Trace("AEnovaMapper: Found '{0}' property candidates for '{1}' from '{2}'", _propertyCandidatesToEnova.Count, typeof(TModel), typeof(TEnova));
        }

        private static bool MatchingProperty(PropertyInfo a, PropertyInfo b)
        {
            if (a.Name != b.Name)
                return false;
            //match enova type against same model type, but also against Nullable<type> in the model
            return a.PropertyType == b.PropertyType || a.PropertyType == Nullable.GetUnderlyingType(b.PropertyType);
        }

        protected BaseMapper(Func<TModel> modelMaker, Func<Context, TEnova> enovaMaker)
        {
            // Delegate factory implemented with Autofac
            _modelMaker = modelMaker;
            _enovaMaker = enovaMaker;
        }

        protected Context Context
        {
            get
            {
                return EnovaContextProvider.GetCurrentContext();
            }
        }

        /// <summary>
        /// Maps the model to an enova object. Creates a new object if no current object is found by the models ID or identifier.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual TEnova MapToEnova(TModel model)
        {
            TEnova enovaObject = null;
            //if an ID is specified, then there must be an object that has this ID
            if (model.ID != WipConstants.InvalidId)
            {
                enovaObject = Context.FindObjectThrowException<TEnova>(model.ID);
            }
            //otherwise check by identifier
            else if (!String.IsNullOrEmpty(model.Identifier))
            {
                enovaObject = Context.FindObject<TEnova>(model.Identifier);
            }

            //if no object found, make a new one
            if (enovaObject == null)
                enovaObject = _enovaMaker.Invoke(Context);

            this.MakeSureObjectIsInEditMode(enovaObject);

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

        /// <summary>
        /// Maps an enova object to a model.
        /// </summary>
        /// <param name="enovaObject"></param>
        /// <returns></returns>
        public virtual TModel MapFromEnova(TEnova enovaObject)
        {
            var model = _modelMaker.Invoke();
            //auto map simple properties
            foreach (var propertyCandidate in _propertyCandidatesFromEnova)
            {
                model.TrySetPropertyValue(propertyCandidate, enovaObject.TryGetPropertyValue(propertyCandidate));
            }

            return model;
        }

        protected virtual void MakeSureObjectIsInEditMode(BaseObject enovaObject)
        {
            if (!enovaObject.IsBeingEdited)
                enovaObject.Edit();
        }

        ///// <summary>
        ///// Maps all given models to enova objects. Creates new objects if no current object is found by the models ID or identifier.
        ///// </summary>
        ///// <param name="models"></param>
        ///// <param name="save">True to save the enova objects.</param>
        ///// <returns></returns>
        //public virtual IEnumerable<TEnova> MapToEnova(IEnumerable<TModel> models, bool save = true)
        //{
        //    //return models.Select(x => MapToEnova(x, save));
        //    var bag = new ConcurrentBag<TEnova>();
        //    Parallel.ForEach(models, model => bag.Add(MapToEnova(model, save)));

        //    return bag;
        //}

        ///// <summary>
        ///// Maps all given enova objects to models.
        ///// </summary>
        ///// <param name="enovaObjects"></param>
        ///// <returns></returns>
        //public virtual IEnumerable<TModel> MapFromEnova(IEnumerable<TEnova> enovaObjects)
        //{
        //    //return enovaObjects.Select(MapFromEnova);
        //    var bag = new ConcurrentBag<TModel>();
        //    Parallel.ForEach(enovaObjects, enovaObject => bag.Add(MapFromEnova(enovaObject)));

        //    return bag;
        //}
    }
}