using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using Wipcore.Core.SessionObjects;
using Fasterflect;
using Wipcore.Enova.Core;
using Wipcore.Enova.Generics;
using Wipcore.Enova.Api.Models;
using Wipcore.Enova.Api.WebApi.Helpers;

namespace Wipcore.Enova.Api.WebApi.Mappers
{
    public abstract class LanguageMapper<TEnova, TModel> : BaseMapper<TEnova, TModel>
        where TEnova : BaseObject
        where TModel : BaseModel 
    {
        protected static readonly List<string> _languagePropertyCandidatesToEnova = new List<string>();
        protected static readonly List<string> _languagePropertyCandidatesFromEnova = new List<string>();

        static LanguageMapper()
        {
            //Get most derived types, i.e. MyEnovaObject and MyModel
            var modelType = typeof(TModel).GetMostDerivedType(ReflectionHelper.GetAllAvailableTypes());
            //var enovaType = typeof(TEnova);//.GetMostDerivedType(ReflectionHelper.GetAllAvailableTypes());
            var enovaType = typeof(TEnova).GetMostDerivedTypes(ReflectionHelper.GetAllAvailableTypes()).OrderBy<Type, int>(
                x =>
                {
                    if (x.Namespace == typeof(BaseObject).Namespace)
                        return 1000;
                    if (x.Namespace == typeof(EnovaBaseProduct).Namespace)
                        return 100;
                    return 0;
                }).FirstOrDefault();


            foreach (var property in modelType.GetProperties().Where(p => p.PropertyType == typeof(List<LocalizedString>)))
            {
                var propertyCandidate = enovaType.GetProperties(BindingFlags.Public | BindingFlags.Instance).FirstOrDefault(x => MatchingProperty(x, property));
                if (propertyCandidate == null)
                    continue;

                if (propertyCandidate.CanRead)
                    _languagePropertyCandidatesFromEnova.Add(propertyCandidate.Name);

                if (propertyCandidate.CanWrite)
                    _languagePropertyCandidatesToEnova.Add(propertyCandidate.Name);
            }

            //LogWriter.Current.Trace("ALanguageEnovaMapper: Found '{0}' property candidates for '{1}' from '{2}'", _languagePropertyCandidatesFromEnova.Count, typeof(TEnova), typeof(TModel));
            //LogWriter.Current.Trace("ALanguageEnovaMapper: Found '{0}' property candidates for '{1}' from '{2}'", _languagePropertyCandidatesToEnova.Count, typeof(TModel), typeof(TEnova));
        }

        private static bool MatchingProperty(PropertyInfo a, PropertyInfo b)
        {
            if (a.Name != b.Name)
                return false;

            return a.PropertyType == typeof(string) && b.PropertyType == typeof(List<LocalizedString>);
        }

        protected LanguageMapper(Func<TModel> modelMaker, Func<Context, TEnova> enovaMaker)
            : base(modelMaker, enovaMaker)
        {
        }

        public override TEnova MapToEnova(TModel model)
        {
            var enovaObject = base.MapToEnova(model);

            base.MakeSureObjectIsInEditMode(enovaObject);

            var languages = Context.GetAllLanguages();
            foreach (var propertyCandidate in _languagePropertyCandidatesToEnova)
            {
                var modelValue = model.TryGetPropertyValue(propertyCandidate);
                if (modelValue == null || modelValue.GetType() != typeof(List<LocalizedString>))
                    continue;

                var localizedStrings = (List<LocalizedString>)modelValue;
                
                foreach (var localizedString in localizedStrings)
                {
                    if (String.IsNullOrEmpty(localizedString.Culture))
                    {
                        //LogWriter.Current.Trace("LanguageEnovaMapper: No language set for '{0}' in model. Setting value for all languages. Value: {1}", propertyCandidate, localizedString.Value);
                        foreach (var lang in languages)
                        {
                            enovaObject.SetProperty(propertyCandidate, localizedString.Value, lang);
                        }
                    }
                    else
                    {
                        EnovaLanguage language = languages.FirstOrDefault(l => l.Culture == localizedString.Culture);

                        if (language == null)
                        {
                            language = languages.FirstOrDefault(l => l.IsoCode.ToLowerInvariant() == localizedString.Culture.ToLowerInvariant());
                        }

                        if (language != null)
                        {
                            //LogWriter.Current.Trace("Trying to set property '{0}' to '{1}' for language '{2}'", propertyCandidate, localizedString.Value, language.Name);
                            enovaObject.SetProperty(propertyCandidate, localizedString.Value, language);
                        }
                        else
                        {
                            //LogWriter.Current.Trace("LanguageEnovaMapper: Language '{0}' not found. Skipping value.", localizedString.Culture);
                        }
                    }
                }
            }

            return enovaObject;
        }

        public override TModel MapFromEnova(TEnova enovaObject)
        {
            var model = base.MapFromEnova(enovaObject);

            var languages = Context.GetAllLanguages();
            foreach (var property in _languagePropertyCandidatesFromEnova)
            {
                var propertyValues = new List<LocalizedString>();
                foreach (var language in languages)
                {
                    var enovaProperty = enovaObject.GetProperty(property, language);

                    var modelProperty = new LocalizedString()
                    {
                        Culture = language.Culture,
                        Value = (string)enovaProperty
                    };

                    propertyValues.Add(modelProperty);
                }

                model.TrySetValue(property, propertyValues);
            }

            return model;
        }
    }
}