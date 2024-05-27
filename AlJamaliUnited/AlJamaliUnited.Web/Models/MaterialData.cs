using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AlJamaliUnited.Web.Models
{
    public class MaterialData
    {
        public int Id { get; set; }
        public string ItemCode { get; set; }
        public string Description { get; set; }
        public double QtySold { get; set; }
        public double StockQty { get; set; }
        public string BaseUnit { get; set; }
        //public string Unit { get; set; }
        public List<UnitData> Unit { get; set; }
        public double Packing { get; set; }
        //public List<int> Packing { get; set; }
        public int CRT { get; set; }
        public int PCSRoll { get; set; }
        public double Quantity { get; set; }
    }
    public class UnitData
    {
        public int iMasterId { get; set; }
        public string sName { get; set; }
        public double xFactor { get; set; }
    }
}