using PawnShop.Oracle.Models;
using System;
using System.Collections.Generic;

namespace PawnShop.Data.Models
{
    public partial class Pawning : BaseEntity
    {
        public string Description { get; set; }
        public string SubmissionDate { get; set; }
        public string ReturnDate { get; set; }
        public decimal Sum { get; set; }
        public decimal? Commision { get; set; }
        public decimal? ClientId { get; set; }
        public decimal? CategoryId { get; set; }
        public decimal? WarehouseId { get; set; }
        public string OwnerName { get; set; }
        public string CategoryName { get; set; }
        public string WarehouseAddress { get; set; }
    }
}
