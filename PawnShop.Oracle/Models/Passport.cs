using PawnShop.Oracle.Models;
using System;
using System.Collections.Generic;

namespace PawnShop.Data.Models
{
    public partial class Passport : BaseEntity
    {
        public string Number { get; set; }
        public string Series { get; set; }
        public string DateOfIssue { get; set; }
        public decimal ClientId { get; set; }
    }
}
