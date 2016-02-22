using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wipcore.Enova.Api.Models
{
    public class BaseModel
    {
        public BaseModel()
        {
            Enabled = true;
        }

        public int ID { get; set; }

        public string Identifier { get; set; }

        public int SortOrder { get; set; }

        public bool Enabled { get; set; }
    }
}
