using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AlJamaliUnited.Web.Models
{
    public class DataTableResult
    {
        public int draw { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public List<MaterialData> data { get; set; }
        public int start { get; set; }
        public int length { get; set; }
    }
}