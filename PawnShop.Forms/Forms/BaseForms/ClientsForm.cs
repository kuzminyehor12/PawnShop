using System;
using System.Configuration;
using System.Text;
using System.Text.RegularExpressions;
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
    public partial class ClientsForm : Form
    {
        private readonly ClientsAddingForm _secondaryForm;
        private readonly ClientService _clientService;
        private readonly BasePdfRenderer _renderer;
        private const string Pdfpath = "../../../../PDFs/Clients.pdf";
        public ClientsForm()
        {
            InitializeComponent();
            _renderer = new ChromePdfRenderer();
            _clientService = new ClientService(ConfigurationManager.ConnectionStrings["PawnShopOracleDb"].ConnectionString);
            _secondaryForm = new ClientsAddingForm(_clientService, _renderer, Pdfpath);
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
                        _clientService.Html.Append("Table 'Clients': <br>");
                        var clients = await _clientService.GetAllWithPassportAsync();

                        foreach (var row in clients)
                        {
                            dataGridView1.Rows.Add(row.Id, row.FullName, row.Number, row.Series, row.DateOfIssue);
                            _clientService.Html.Append(
                               $"Client Id: {row.Id}, Full Name: {row.FullName}, Passport Number: {row.Number}, Passport Series: {row.Series}, Date of Issue: {row.DateOfIssue}<br>");
                        }

                        _clientService.Html.Append($"<br>{DateTime.Now}<br>");
                        var pdf = _renderer.RenderHtmlAsPdf(_clientService.Html.ToString());
                        PdfExtensions.SaveOrMerge(Pdfpath, pdf);
                        _clientService.Html.Clear();
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

        private async void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1) return;
            var dataRow = dataGridView1.Rows[e.RowIndex];

            if (!IsUpdateValid(dataRow))
            {
                MessageBox.Show("Data was input incorrectly", "Error", MessageBoxButtons.OK);
                return;
            }

            _clientService.Html.Append("Table 'Clients': <br>'");

            dataGridView1.Update();

            try
            {
                var findingPassport = await _clientService.GetByPassportNumber(dataRow.Cells["PassportNumber"].EditedFormattedValue.ToString());
                var findingClient = await _clientService.GetByIdAsync(Convert.ToDecimal(dataRow.Cells["ClientId"].EditedFormattedValue));
                var names = dataRow.Cells["FullName"].ToString().Split(' ');

                var client = new Client
                {
                    Id = findingClient.Id,
                    FirstName = names[0],
                    LastName = names[1]
                };

                var passport = new Passport
                {
                    Id = findingPassport.Id,
                    ClientId = findingPassport.ClientId,
                    Number = dataRow.Cells["PassportNumber"].EditedFormattedValue.ToString(),
                    Series = dataRow.Cells["PassportSeries"].EditedFormattedValue.ToString(),
                    DateOfIssue = dataRow.Cells["DateOfIssue"].EditedFormattedValue.ToString()
                };

                await _clientService.UpdateAsync(findingClient);
                await _clientService.UpdatePassportAsync(findingPassport);
                _clientService.Html.Append($"Current Client Count: {_clientService.GetCount()}<br>");
                _clientService.Html.Append($"<br>{DateTime.Now}<br>");

                var pdf = _renderer.RenderHtmlAsPdf(_clientService.Html.ToString());
                PdfExtensions.SaveOrMerge(Pdfpath, pdf);
                MessageBox.Show("Data has been updated successfully!", "Update", MessageBoxButtons.OK);
                _clientService.Html.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private bool IsUpdateValid(DataGridViewRow dataRow)
        {
            return Regex.IsMatch(dataRow.Cells["PassportNumber"].EditedFormattedValue.ToString(), "^[0-9]{6}$")
                   && Regex.IsMatch(dataRow.Cells["PassportSeries"].EditedFormattedValue.ToString(),
                       "^[\\p{IsCyrillic}]{2}$")
                   && !dataRow.Cells["FullName"].EditedFormattedValue.ToString().ContainsDigit()
                   && dataRow.Cells["FullName"].EditedFormattedValue.ToString().Split(' ').Length == 2
                   && DateTime.TryParse(dataRow.Cells["DateOfIssue"].EditedFormattedValue.ToString(), out var dateOfIssue);
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                dataGridView1.Invoke(new MethodInvoker(async () =>
                {
                    try
                    {
                        _clientService.Html.Append("Table 'Clients': <br>'");

                        if (dataGridView1.SelectedRows == null)
                        {
                            MessageBox.Show("There is nothing to delete.", "Delete Message", MessageBoxButtons.OK);
                            return;
                        }

                        foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                        {
                            var findingClient = await _clientService.GetByIdAsync(Convert.ToDecimal(row.Cells["ClientId"].EditedFormattedValue));
                            await _clientService.DeleteAsync(findingClient);
                        }

                        _clientService.Html.Append($"Current Client Count: {_clientService.GetCount()}<br>");
                        _clientService.Html.Append($"<br>{DateTime.Now}<br>");

                        var pdf = _renderer.RenderHtmlAsPdf(_clientService.Html.ToString());
                        PdfExtensions.SaveOrMerge(Pdfpath, pdf);
                        MessageBox.Show("Data has been deleted successfully!", "Delete", MessageBoxButtons.OK);
                        _clientService.Html.Clear();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }));
            });
        }
    }
}
