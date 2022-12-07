using System;
using System.Collections.Generic;

namespace PawnShop.Data.Models
{
    public partial class ClientData
    {
        public ClientData()
        {
            Pawnings = new HashSet<Pawnings>();
        }

        public string FullName
        {
            get => Surname + " " + Name + " " + (Patronymic == null ? String.Empty : Patronymic);
        }

        public Guid ClientId { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string Patronymic { get; set; }
        public Guid PassportDataId { get; set; }

        public virtual PassportData PassportData { get; set; }
        public virtual ICollection<Pawnings> Pawnings { get; set; }
    }
}
