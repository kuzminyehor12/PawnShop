using System;
using System.Collections.Generic;
using System.Text;
using PawnShop.Business.DTO;
using PawnShop.Data.Models;

namespace PawnShop.Business
{
    public static class Mapper
    {
        public static ClientPassportDTO Map(ClientData client)
        {
            return new ClientPassportDTO
            {
                ClientId = client.ClientId,
                PassportId = client.PassportDataId,
                FullName = client.FullName,
                PassportSeries = client.PassportData.Series,
                PassportNumber = client.PassportData.Number,
                DateOfIssue = client.PassportData.DateOfIssue
            };
        }

        public static IEnumerable<ClientPassportDTO> Map(IEnumerable<ClientData> clients)
        {
            var dtos = new List<ClientPassportDTO>();

            foreach (var c in clients)
            {
                dtos.Add(Map(c));
            }

            return dtos;
        }
    }
}
