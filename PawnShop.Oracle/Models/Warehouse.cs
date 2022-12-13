using System;
using System.Collections.Generic;
using System.Text;

namespace PawnShop.Oracle.Models
{
    public class Warehouse : BaseEntity
    {
        public string Address
        {
            get
            {
                return Country + "," + City + "," + Street + "," + Number;
            }
        }
        public int Capacity { get; set; }
        public int Size { get; set; }
        public decimal AddressId { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string Number { get; set; }
    }
}
