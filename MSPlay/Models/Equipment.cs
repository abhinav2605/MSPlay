using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSPlay.Models
{
    public class Equipment
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public int Quantity { get; set; }
        public List<Equipment> GetEquipmentList { get; set; }
    }
}