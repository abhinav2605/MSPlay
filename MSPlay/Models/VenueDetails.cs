using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSPlay.Models
{
    public class VenueDetails
    {
        public int ID { get; set; }
        public string VenueName { get; set; }
        public string VenueID { get; set; }
        public string Category { get; set; }
        public List<VenueDetails> GetVenueList { get; set; }
    }
}