using System;
using System.Collections.Generic;
using System.Text;

namespace PawnShop.Oracle.Models
{
    public class Address : BaseEntity
    {
        public string Country { get; set; } = "Ukraine";
        public string City { get; set; }
        public string Street { get; set; }
        public string Number { get; set; }
    }
}
