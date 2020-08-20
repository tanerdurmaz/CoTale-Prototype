using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoTale.Models
{
    public class colAdventure
    {
        public IEnumerable<CoTale.Models.Adventure> Adventures { get; set; }
        public CoTale.Models.Adventure Adventure { get; set; }
    }
}