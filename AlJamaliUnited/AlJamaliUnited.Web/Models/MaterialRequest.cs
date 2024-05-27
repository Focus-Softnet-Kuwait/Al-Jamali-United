using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AlJamaliUnited.Web.Models
{
    public class MaterialRequest
    {
        public MaterialRequestHeader Header { get; set; }
        public List<MaterialRequestBody> Body { get; set; }
    }
}