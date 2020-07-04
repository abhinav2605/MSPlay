using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Web;

namespace MSPlay.Models
{
    public class Booking
    {
        public int SlNo { get; set; }
        [Required]
        public string VenueID { get; set; }
        [Required]
        public DateTime InTime { get; set; }
        [Required]
        public DateTime OutTime { get; set; }
        [Required]
        public string EmployeeAlias { get; set; }
        public string EmployeeName { get; set; }
        public List<Booking> GetBookingList { get; set; }
    }
}