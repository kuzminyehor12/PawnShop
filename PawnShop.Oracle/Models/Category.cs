using PawnShop.Oracle.Models;
using System;
using System.Collections.Generic;

namespace PawnShop.Data.Models
{
    public partial class Category : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
