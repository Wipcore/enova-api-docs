using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Caching;
using Wipcore.Core;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Api.Abstractions.Interfaces;
using Wipcore.Enova.Core;
using Wipcore.Enova.Generics;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NLog.Internal;
using Wipcore.eNova.Api.WebApi.Mappers;

namespace Wipcore.Enova.Api.WebApi.Mappers.Product
{
    public class ProductImageMapper : IPropertyMapper
    {
        public bool PostSaveSet => false;

        public List<string> Names => new List<string>() { "Images" };
        public Type Type => typeof(EnovaBaseProduct);
        public bool InheritMapper => true;
        public bool FlattenMapping => false;

        public int Priority => 0;
        public MapType MapType => MapType.MapFromAndToEnovaAllowed;


        public object GetEnovaProperty(BaseObject obj, string propertyName, List<EnovaLanguage> mappingLanguages)
        {
            var product = (EnovaBaseProduct)obj;

            var pictureFamily = obj.GetContext().FindObject<EnovaPictureLinkFamily>(product.Identifier + "_images");
            if (pictureFamily == null)
                return new ArrayList();

            return pictureFamily.GetObjects().Cast<EnovaPictureLinkObject>().Select(x => new Dictionary<string, object>()
            {
                {"ID", x.ID},
                {"Identifier", x.Identifier},
                {"Path", x.ImageFilePath}
            }.MapLanguageProperty("Name", mappingLanguages, x.GetName));
        }

        public void SetEnovaProperty(BaseObject obj, string propertyName, object value, IDictionary<string, object> otherValues)
        {
            if (value == null)
                return;

            var product = (EnovaBaseProduct)obj;

            var context = product.GetContext();
            var pictureFamily = context.FindObject<EnovaPictureLinkFamily>(product.Identifier + "_images");
            if (pictureFamily == null)
            {
                pictureFamily = new EnovaPictureLinkFamily(context) { Identifier = product.Identifier + "_images", ProductID = product.ID };
                pictureFamily.Save();
            }

            foreach (var i in value as dynamic)
            {
                var imageModel = JsonConvert.DeserializeAnonymousType(i.ToString(), new { ID = 0, Identifier = String.Empty, MarkForDelete = false, Name = String.Empty, Path = String.Empty });
                var pictureObj = context.FindObject(imageModel.ID, typeof(EnovaPictureLinkObject), false) ?? context.FindObject(imageModel.Identifier, typeof(EnovaPictureLinkObject), false);

                if (pictureObj == null)
                {
                    if (imageModel.MarkForDelete)
                        continue;

                    pictureObj = new EnovaPictureLinkObject(context) { Identifier = imageModel.Identifier ?? String.Empty, Name = imageModel.Name ?? String.Empty };
                }
                else if (imageModel.MarkForDelete)
                {
                    pictureFamily.RemoveObject(pictureObj);
                    pictureObj.Delete();
                    continue;
                }


                pictureObj.Edit();
                pictureObj.Identifier = imageModel.Identifier;
                pictureObj.Name = imageModel.Name;
                pictureObj.ImageFilePath = imageModel.Path;
                pictureObj.Save();

                if (pictureFamily.GetObjects(typeof(EnovaPictureLinkObject)).Search($"ID = {pictureObj.ID}").Count == 0)
                {
                    pictureFamily.AddObject(pictureObj);
                }

            }

        }


    }
}
