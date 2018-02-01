using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wipcore.Enova.Api.Abstractions.Models
{
    /// <summary>
    /// A model representing access rights on Enova objects.
    /// </summary>
    public class AccessModel
    {
        /// <summary>
        /// Read access.
        /// </summary>
        public bool? Read { get; set; }

        /// <summary>
        /// Write access.
        /// </summary>
        public bool? Write { get; set; }

        /// <summary>
        /// Create access.
        /// </summary>
        public bool? Create { get; set; }

        /// <summary>
        /// Delete access.
        /// </summary>
        public bool? Delete { get; set; }

        /// <summary>
        /// Use access. Set to give access to promos, pricelists etc.
        /// </summary>
        public bool? Use { get; set; }

        /// <summary>
        /// Permission to create links to other objects.
        /// </summary>
        public bool? CreateLink { get; set; }

        /// <summary>
        /// Permission to update links to other objects.
        /// </summary>
        public bool? UpdateLink { get; set; }

        /// <summary>
        /// Permission to delete links to other objects.
        /// </summary>
        public bool? DeleteLink { get; set; }

        /// <summary>
        /// Permission to change other access rights.
        /// </summary>
        public bool? SetAccess { get; set; }

        /// <summary>
        /// Permission to add and remove properties from a type.
        /// </summary>
        public bool? ModifyDatabase { get; set; }

        /// <summary>
        /// Denied access.
        /// </summary>
        public bool? NoAccess { get; set; }

        /// <summary>
        /// The type the permissions apply to. 
        /// </summary>
        public string EnovaType { get; set; }


        public override string ToString() => $"(EnovaType: {EnovaType}, Read: {Read}, Write: {Write}, Delete: {Delete}, Use: {Use}, CreateLink: {CreateLink}, " +
                                             $"UpdateLink: {UpdateLink}, DeleteLink: {DeleteLink}, SetAccess: {SetAccess}, ModifyDatabase {ModifyDatabase})";

    }
}
