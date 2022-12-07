using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IronPdf;
using IronPdf.Rendering.Abstractions;
using Microsoft.EntityFrameworkCore;
using PawnShop.Business.Interfaces;
using PawnShop.Business.Services;
using PawnShop.Data.Models;
using PawnShop.Forms.Extensions;
using PawnShop.Forms.Forms.SecondaryForms;

namespace PawnShop.Forms.Forms
{
    public partial class PawningsForm : Form
    {
        private readonly PawningsAddingForm _secondaryForm;
        private readonly BasePdfRenderer _renderer;
        private const string Pdfpath = "../../../../PDFs/Pawnings.pdf";
        public PawningsForm()
        {
            InitializeComponent();
            _renderer = new ChromePdfRenderer();
            _secondaryForm = new PawningsAddingForm(_renderer, Pdfpath);
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                dataGridView1.Invoke(new MethodInvoker(async () =>
                {
                    dataGridView1.Rows.Clear();
                    StringBuilder html = new StringBuilder();
                    html.Append("Table 'Pawnings': <br>");

                    await using IPawningService service = new PawningService(html);

                    var pawnings = await service.GetAllBySelection(p => new
                    {
                        Description = p.Description,
                        SubmissionDate = p.SubmissionDate,
                        ReturnDate = p.ReturnDate,
                        Sum = p.Sum.ToString() + " грн",
                        Comission = p.Commision.ToString() + "%",
                        OwnerName = p.Client.FullName,
                        CategoryName = p.Category.Name
                    });

                    foreach (var row in pawnings.OrderBy(p => p.ReturnDate))
                    {
                        dataGridView1.Rows.Add(row.Description, row.SubmissionDate,
                            row.ReturnDate, row.Sum, row.Comission, row.OwnerName, row.CategoryName);

                        html.Append($"Description: {row.Description}, Submission Date: {row.SubmissionDate}, Return Date: {row.ReturnDate}," +
                                    $"Sum: {row.Sum}, Commission: {row.Comission}, Owner Name: {row.OwnerName}, Category Name: {row.CategoryName}<br>");
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

        private async void button3_Click(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                dataGridView1.Invoke(new MethodInvoker(async () =>
                {
                    StringBuilder html = new StringBuilder();
                    html.Append("Table 'Pawnings': <br>");
                    await using IPawningService service = new PawningService(html);

                    if (dataGridView1.SelectedRows == null)
                    {
                        MessageBox.Show("There is nothing to delete.", "Delete Message", MessageBoxButtons.OK);
                        return;
                    }

                    foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                    {
                        var names = row.Cells["OwnerName"].EditedFormattedValue.ToString().Split(' ');
                        var findingPawning = await service
                            .GetOneByFIlter(p =>
                                p.Client.Surname == names[0] 
                                && p.Client.Name == names[1]
                                && names.Length == 2 ? p.Client.Patronymic == null : p.Client.Patronymic == names[2]
                                );

                        await service.DeleteAsync(findingPawning);
                    }

                    html.Append($"Current Client Count: {service.GetCount()}<br>");
                    html.Append($"<br>{DateTime.Now}<br>");
                    var pdf = _renderer.RenderHtmlAsPdf(html.ToString());
                    PdfExtensions.SaveOrMerge(Pdfpath, pdf);

                    MessageBox.Show("Data has been deleted successfully!", "Delete", MessageBoxButtons.OK);
                }));
            });
        }

        private async void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1) return;
            var dataRow = dataGridView1.Rows[e.RowIndex];

            if (IsUpdateInvalid(dataRow))
            {
                MessageBox.Show("Data was input incorrectly", "Error", MessageBoxButtons.OK);
                return;
            }

            dataGridView1.Update();
            StringBuilder html = new StringBuilder();
            html.Append("Table 'Pawnings': <br>");

            await using IPawningService service = new PawningService(html);

            var findingPawning = await service
                .GetOneByFIlter(p =>
                    p.Description == dataRow.Cells["Description"].EditedFormattedValue.ToString());

            var pawning = new Pawnings
            {
                PawningId = findingPawning.PawningId,
                Description = dataRow.Cells["Description"].EditedFormattedValue.ToString(),
                SubmissionDate = DateTime.Parse(dataRow.Cells["SubmissionDate"].EditedFormattedValue.ToString()),
                ReturnDate = DateTime.Parse(dataRow.Cells["ReturnDate"].EditedFormattedValue.ToString()),
                Sum = double.Parse(dataRow.Cells["Sum"].EditedFormattedValue.ToString()),
                Commision = double.Parse(dataRow.Cells["Comission"].EditedFormattedValue.ToString())
            };

            await service.UpdateAsync(pawning);

            html.Append($"Current Client Count: {service.GetCount()}<br>");
            html.Append($"<br>{DateTime.Now}<br>");
            var pdf = _renderer.RenderHtmlAsPdf(html.ToString());
            PdfExtensions.SaveOrMerge(Pdfpath, pdf);

            MessageBox.Show("Data has been updated successfully!", "Update", MessageBoxButtons.OK);
        }

        private bool IsUpdateInvalid(DataGridViewRow dataRow)
        {
            return dataRow.Cells[0].EditedFormattedValue.ToString() == string.Empty
                   || DateTime.Parse(dataRow.Cells["SubmissionDate"].EditedFormattedValue.ToString()) > DateTime.Parse(
                       dataRow.Cells["ReturnDate"].EditedFormattedValue.ToString())
                   || !double.TryParse(dataRow.Cells["Sum"].EditedFormattedValue.ToString(), out double sum)
                   || sum < 0
                   || !double.TryParse(dataRow.Cells["Comission"].EditedFormattedValue.ToString(), out double com)
                   || com < 0;
        }
    }
}
