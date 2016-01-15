using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wipcore.eNova.Api.WebApi.Models
{
    /// <summary>
    /// Represents a language dependent string
    /// </summary>
    public class LocalizedString
    {
        /// <summary>
        /// Culture name that corresponds to the EnovaLanguage CultureName property
        /// </summary>
        public string Culture { get; set; }

        public string Value { get; set; }
    }
}
