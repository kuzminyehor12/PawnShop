using System;
using System.Collections.Generic;

namespace PawnShop.Data.Models
{
    public partial class Categories
    {
        public Categories()
        {
            Pawnings = new HashSet<Pawnings>();
        }

        public Guid CategoryId { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }

        public virtual ICollection<Pawnings> Pawnings { get; set; }
    }
}
