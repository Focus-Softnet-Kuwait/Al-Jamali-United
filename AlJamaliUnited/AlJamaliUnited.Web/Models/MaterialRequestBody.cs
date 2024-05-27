using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AlJamaliUnited.Web.Models
{
    public class MaterialRequestBody
    {
        public string ItemCode { get; set; }
        public string Description { get; set; }
        public double QtySold { get; set; }
        public double StockQty { get; set; }
        public string Unit { get; set; }
        public string Unit2 { get; set; }
        public int Packing { get; set; }
        public int CRT { get; set; }
        public int PCSRoll { get; set; }
        public double Quantity { get; set; }
    }
}