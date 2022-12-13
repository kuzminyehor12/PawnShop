using System;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IronPdf;
using IronPdf.Rendering.Abstractions;
using PawnShop.Data.Models;
using PawnShop.Forms.Extensions;
using PawnShop.Forms.Forms.SecondaryForms;
using PawnShop.Oracle.Services;

namespace PawnShop.Forms.Forms
{
    public partial class PawningsForm : Form
    {
        private readonly PawningsAddingForm _secondaryForm;
        private readonly BasePdfRenderer _renderer;
        private readonly PawningService _pawningService;
        private readonly CategoryService _categoryService;
        private readonly ClientService _clientService;
        private const string Pdfpath = "../../../../PDFs/Pawnings.pdf";
        public PawningsForm()
        {
            InitializeComponent();
            _renderer = new ChromePdfRenderer();
            _pawningService = new PawningService(ConfigurationManager.ConnectionStrings["PawnShopOracleDb"].ConnectionString);
            _clientService = new ClientService(ConfigurationManager.ConnectionStrings["PawnShopOracleDb"].ConnectionString);
            _categoryService = new CategoryService(ConfigurationManager.ConnectionStrings["PawnShopOracleDb"].ConnectionString);
            _secondaryForm = new PawningsAddingForm(_pawningService, _categoryService, _clientService, _renderer, Pdfpath);
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                dataGridView1.Invoke(new MethodInvoker(async () =>
                {
                    try
                    {
                        dataGridView1.Rows.Clear();
                        _pawningService.Html.Append("Table 'Pawnings': <br>");

                        var pawnings = await _pawningService.GetAllWithDetailsAsync();

                        foreach (var row in pawnings)
                        {
                            dataGridView1.Rows.Add(row.Id, row.Description, row.SubmissionDate,
                                row.ReturnDate, row.Sum, row.Commision, row.WarehouseAddress, row.OwnerName, row.CategoryName);

                            _pawningService.Html.Append($"Pawning Id: {row.Id}, Description: {row.Description}, Submission Date: {row.SubmissionDate}, Return Date: {row.ReturnDate}," +
                                        $"Sum: {row.Sum}, Commission: {row.Commision}, Warehouse Address: {row.WarehouseAddress}, " +
                                        $"Owner Name: {row.OwnerName}, Category Name: {row.CategoryName}<br>");
                        }

                        _pawningService.Html.Append($"<br>{DateTime.Now}<br>");
                        var pdf = _renderer.RenderHtmlAsPdf(_pawningService.Html.ToString());
                        PdfExtensions.SaveOrMerge(Pdfpath, pdf);
                        _pawningService.Html.Clear();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
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
                    try
                    {
                        _pawningService.Html.Append("Table 'Pawnings': <br>");

                        if (dataGridView1.SelectedRows == null)
                        {
                            MessageBox.Show("There is nothing to delete.", "Delete Message", MessageBoxButtons.OK);
                            return;
                        }

                        foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                        {
                            var findingPawning = await _pawningService.GetByIdAsync(Convert.ToDecimal(row.Cells["PawningId"].EditedFormattedValue));
                            await _pawningService.DeleteAsync(findingPawning);
                        }

                        _pawningService.Html.Append($"Current Pawnings Count: {_pawningService.GetCount()}<br>");
                        _pawningService.Html.Append($"<br>{DateTime.Now}<br>");
                        var pdf = _renderer.RenderHtmlAsPdf(_pawningService.Html.ToString());
                        PdfExtensions.SaveOrMerge(Pdfpath, pdf);
                        MessageBox.Show("Data has been deleted successfully!", "Delete", MessageBoxButtons.OK);
                        _pawningService.Html.Clear();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
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

            try
            {
                dataGridView1.Update();
                _pawningService.Html.Append("Table 'Pawnings': <br>");

                var findingPawning = await _pawningService.GetByIdAsync(Convert.ToDecimal(dataRow.Cells["PawningId"].EditedFormattedValue));
                var pawning = new Pawning
                {
                    Id = findingPawning.Id,
                    CategoryId = findingPawning.CategoryId,
                    WarehouseId = findingPawning.WarehouseId,
                    ClientId = findingPawning.ClientId,
                    Description = dataRow.Cells["Description"].EditedFormattedValue.ToString(),
                    SubmissionDate = dataRow.Cells["SubmissionDate"].EditedFormattedValue.ToString(),
                    ReturnDate = dataRow.Cells["ReturnDate"].EditedFormattedValue.ToString(),
                    Sum = Convert.ToDecimal(dataRow.Cells["Sum"].EditedFormattedValue),
                    Commision = Convert.ToDecimal(dataRow.Cells["Comission"].EditedFormattedValue)
                };

                await _pawningService.UpdateAsync(pawning);

                _pawningService.Html.Append($"Current Pawnings Count: {_pawningService.GetCount()}<br>");
                _pawningService.Html.Append($"<br>{DateTime.Now}<br>");
                var pdf = _renderer.RenderHtmlAsPdf(_pawningService.Html.ToString());
                PdfExtensions.SaveOrMerge(Pdfpath, pdf);
                MessageBox.Show("Data has been updated successfully!", "Update", MessageBoxButtons.OK);
                _pawningService.Html.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private bool IsUpdateInvalid(DataGridViewRow dataRow)
        {
            return dataRow.Cells[1].EditedFormattedValue.ToString() == string.Empty
                   || !DateTime.TryParse(dataRow.Cells["SubmissionDate"].EditedFormattedValue.ToString(), out var submissionDate)
                   || !DateTime.TryParse(dataRow.Cells["ReturnDate"].EditedFormattedValue.ToString(), out var returnDate)
                   || submissionDate > returnDate
                   || !double.TryParse(dataRow.Cells["Sum"].EditedFormattedValue.ToString(), out double sum)
                   || sum < 0
                   || !double.TryParse(dataRow.Cells["Comission"].EditedFormattedValue.ToString(), out double com)
                   || com < 0;
        }
    }
}
