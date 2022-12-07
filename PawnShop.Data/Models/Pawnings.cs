using System;
using System.Collections.Generic;

namespace PawnShop.Data.Models
{
    public partial class Pawnings
    {
        public Guid PawningId { get; set; }
        public string Description { get; set; }
        public DateTime SubmissionDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public double Sum { get; set; }
        public double? Commision { get; set; }
        public Guid ClientId { get; set; }
        public Guid? CategoryId { get; set; }

        public virtual Categories Category { get; set; }
        public virtual ClientData Client { get; set; }
    }
}
