using System;
using System.Collections.Generic;
using System.Text;
using PawnShop.Data.Models;

namespace PawnShop.Business.DTO
{
    public class ClientPassportDTO
    {
        public Guid PassportId { get; set; }
        public Guid ClientId { get; set; }
        public string PassportNumber { get; set; }
        public string PassportSeries { get; set; }
        public DateTime DateOfIssue { get; set; }
        public string FullName { get; set; }

    }
}
