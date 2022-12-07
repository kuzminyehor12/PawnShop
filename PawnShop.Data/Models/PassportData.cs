using System;
using System.Collections.Generic;

namespace PawnShop.Data.Models
{
    public partial class PassportData
    {
        public PassportData()
        {
            ClientData = new HashSet<ClientData>();
        }

        public Guid PassportIdataId { get; set; }
        public string Number { get; set; }
        public string Series { get; set; }
        public DateTime DateOfIssue { get; set; }

        public virtual ICollection<ClientData> ClientData { get; set; }
    }
}
