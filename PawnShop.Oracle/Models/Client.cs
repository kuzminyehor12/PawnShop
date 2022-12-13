using PawnShop.Oracle.Models;
using System;
using System.Collections.Generic;

namespace PawnShop.Data.Models
{
    public partial class Client : BaseEntity
    {
        public string FullName
        {
            get => FirstName + " " + LastName;
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Number { get; set; }
        public string Series { get; set; }
        public string DateOfIssue { get; set; }
    }
}
