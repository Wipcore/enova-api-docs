using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using Swashbuckle.SwaggerGen;
using Wipcore.Enova.Api.Models;

namespace Wipcore.eNova.Api.WebApi.Helpers
{
    /// <summary>
    /// This class is used to iterate parameters in api calls and setting descriptions for them in swagger. 
    /// Descriptions are taken from .xml documents. In a perfect world this should work automatically, but in the current version of swagger it does not.
    /// </summary>
    internal class ComplexModelFilter : IOperationFilter
    {
        private readonly string _docFolder;

        public ComplexModelFilter(string docFolder)
        {
            _docFolder = docFolder;
        }
        
        public void Apply(Operation operation, OperationFilterContext context)
        {
            for (var i = 0; i < context.ApiDescription.ParameterDescriptions.Count; i++)
            {
                var complexType = context.ApiDescription.ParameterDescriptions[i].ModelMetadata?.ContainerType;
                if (complexType == null || complexType.Namespace == "System")
                    continue;

                var xmlMatchingAssemblyName = Path.Combine(_docFolder, complexType.Assembly.ManifestModule.ScopeName.Replace(".dll", ".xml"));

                if(!File.Exists(xmlMatchingAssemblyName))
                    continue;

                var fullPropertyName = $"P:{complexType.FullName}.{context.ApiDescription.ParameterDescriptions[i].Name}";
                var xml = XDocument.Load(xmlMatchingAssemblyName);
                var xmlSummary = xml.XPathEvaluate($"string(/doc/members/member[@name='{fullPropertyName}']/summary)").ToString().Trim();

                operation.Parameters[i].Description = xmlSummary;
                operation.Parameters[i].In = "query";
            }
        }
    }
}
