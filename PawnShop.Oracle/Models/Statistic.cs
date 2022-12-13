using System;
using System.Collections.Generic;
using System.Text;

namespace PawnShop.Oracle.Models
{
    public class Statistic
    {
        public string CategoryName { get; set; }
        public decimal Max { get; set; }
        public decimal Min { get; set; }
        public decimal Average { get; set; }
        public int Count { get; set; }
    }
}
