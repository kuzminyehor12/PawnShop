using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using IronPdf;
using IronPdf.Rendering.Abstractions;
using Microsoft.EntityFrameworkCore;
using PawnShop.Business.DTO;
using PawnShop.Business.Interfaces;
using PawnShop.Business.Services;
using PawnShop.Data.Models;
using PawnShop.Forms.Extensions;
using PawnShop.Forms.Forms.SecondaryForms;

namespace PawnShop.Forms.Forms
{
    public partial class ClientsForm : Form
    {
        private readonly ClientsAddingForm _secondaryForm;
        private readonly BasePdfRenderer _renderer;
        private const string Pdfpath = "../../../../PDFs/Clients.pdf";
        public ClientsForm()
        {
            InitializeComponent();
            _renderer = new ChromePdfRenderer();
            _secondaryForm = new ClientsAddingForm(_renderer, Pdfpath);
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                dataGridView1.Invoke(new MethodInvoker(async () =>
                {
                    dataGridView1.Rows.Clear();
                    using PawnShopDbContext context = new PawnShopDbContext();

                    StringBuilder html = new StringBuilder();
                    html.Append("Table 'Clients': <br>");
                    await using IClientPassportService service = new ClientPassportService(html);
                    var clients = await service.GetAllAsync();

                    foreach (var row in clients.OrderBy(e => e.FullName))
                    {
                        dataGridView1.Rows.Add(row.FullName, row.PassportNumber, row.PassportSeries, row.DateOfIssue);
                        html.Append(
                            $"Full Name: {row.FullName}, Passport Number: {row.PassportNumber}, Passport Series: {row.PassportSeries}, Date of Issue: {row.DateOfIssue}<br>");
                    }

                    html.Append($"<br>{DateTime.Now}<br>");
                    var pdf = _renderer.RenderHtmlAsPdf(html.ToString());
                    PdfExtensions.SaveOrMerge(Pdfpath, pdf);
                }));
            });
        }

        private void button2_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            this.OpenChildForm(_secondaryForm, this);
        }

        private async void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1) return;
            var dataRow = dataGridView1.Rows[e.RowIndex];

            if (!IsUpdateValid(dataRow))
            {
                MessageBox.Show("Data was input incorrectly", "Error", MessageBoxButtons.OK);
                return;
            }

            StringBuilder html = new StringBuilder();
            html.Append("Table 'Clients': <br>'");

            dataGridView1.Update();
            await using IClientPassportService service = new ClientPassportService(html);

            var findingPassport = await service
                .GetPassport((p) => 
                    p.Number == dataRow.Cells["PassportNumber"].EditedFormattedValue.ToString());

            var findingClient = await service
                .GetClient(c => 
                    c.PassportData == findingPassport);

            var dto = new ClientPassportDTO
            {
                PassportId = findingPassport.PassportIdataId,
                PassportNumber = dataRow.Cells["PassportNumber"].EditedFormattedValue.ToString(),
                PassportSeries = dataRow.Cells["PassportSeries"].EditedFormattedValue.ToString(),
                DateOfIssue = DateTime.Parse(dataRow.Cells["DateOfIssue"].EditedFormattedValue
                    .ToString()),
                ClientId = findingClient.ClientId,
                FullName = findingClient.FullName
            };

            await service.UpdateAsync(dto);
            html.Append($"Current Client Count: {service.GetCount()}<br>");
            html.Append($"<br>{DateTime.Now}<br>");

            var pdf = _renderer.RenderHtmlAsPdf(html.ToString());
            PdfExtensions.SaveOrMerge(Pdfpath, pdf);

            MessageBox.Show("Data has been updated successfully!", "Update", MessageBoxButtons.OK);
        }

        private bool IsUpdateValid(DataGridViewRow dataRow)
        {
            return Regex.IsMatch(dataRow.Cells["PassportNumber"].EditedFormattedValue.ToString(), "^[0-9]{6}$")
                   && Regex.IsMatch(dataRow.Cells["PassportSeries"].EditedFormattedValue.ToString(),
                       "^[\\p{IsCyrillic}]{2}$")
                   && !dataRow.Cells["FullName"].EditedFormattedValue.ToString().ContainsDigit()
                   && dataRow.Cells["FullName"].EditedFormattedValue.ToString().Split(' ').Length >= 2;
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                dataGridView1.Invoke(new MethodInvoker(async () =>
                {
                    StringBuilder html = new StringBuilder();
                    html.Append("Table 'Clients': <br>'");
                    await using IClientPassportService service = new ClientPassportService(html);

                    if (dataGridView1.SelectedRows == null)
                    {
                        MessageBox.Show("There is nothing to delete.", "Delete Message", MessageBoxButtons.OK);
                        return;
                    }

                    foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                    {
                        var findingClient = await service
                            .GetClient(c => 
                                c.PassportData.Number == row.Cells["PassportNumber"].EditedFormattedValue.ToString());

                        await service.DeleteAsync(findingClient);
                    }

                    html.Append($"Current Client Count: {service.GetCount()}<br>");
                    html.Append($"<br>{DateTime.Now}<br>");

                    var pdf = _renderer.RenderHtmlAsPdf(html.ToString());
                    PdfExtensions.SaveOrMerge(Pdfpath, pdf);

                    MessageBox.Show("Data has been deleted successfully!", "Delete", MessageBoxButtons.OK);
                }));
            });
        }
    }
}
