using IronPdf;
using IronPdf.Rendering.Abstractions;
using PawnShop.Forms.Extensions;
using PawnShop.Oracle.Models;
using PawnShop.Oracle.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PawnShop.Forms.Forms.BaseForms
{
    public partial class PackingForm : Form
    {
        private readonly WarehouseService _warehouseService;
        private readonly PawningService _pawningService;
        private const string PdfPath = "../../../../PDFs/Warehouses.pdf";
        private readonly BasePdfRenderer _renderer;
        public PackingForm()
        {
            InitializeComponent();
            _warehouseService = new WarehouseService(ConfigurationManager.ConnectionStrings["PawnShopOracleDb"].ConnectionString);
            _pawningService = new PawningService(ConfigurationManager.ConnectionStrings["PawnShopOracleDb"].ConnectionString);
            _renderer = new ChromePdfRenderer();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                dataGridView1.Invoke(new MethodInvoker(async () =>
                {
                    try
                    {
                        _warehouseService.Html.Append("Table 'Warehouses': <br>");

                        if (dataGridView1.SelectedRows == null)
                        {
                            MessageBox.Show("There is nothing to pack.", "Packing", MessageBoxButtons.OK);
                            return;
                        }

                        var listBoxData = listBox1.Items[listBox1.SelectedIndex].ToString().Split(',');
                        var address = new Address
                        {
                            Country = listBoxData[0],
                            City = listBoxData[1],
                            Street = listBoxData[2],
                            Number = listBoxData[3]
                        };
                        var findingAddressId = await _warehouseService.GetAddressIdByModelAsync(address);
                        var findingWarehouse = await _warehouseService.GetByAddressId(findingAddressId);

                        foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                        {
                            var findingPawning = await _pawningService.GetByIdAsync(Convert.ToDecimal(row.Cells["PawningId"].EditedFormattedValue));
                            await _warehouseService.ExecutePawnPacking(findingWarehouse.Id, findingPawning.Id);
                        }

                        _warehouseService.Html.Append($"Current Warehouses Count: {_warehouseService.GetCount()}<br>");
                        _warehouseService.Html.Append($"<br>{DateTime.Now}<br>");
                        var pdf = _renderer.RenderHtmlAsPdf(_warehouseService.Html.ToString());
                        PdfExtensions.SaveOrMerge(PdfPath, pdf);
                        _warehouseService.Html.Clear();
                        MessageBox.Show("Pawning has been successfully packed!", "Packing", MessageBoxButtons.OK);
                        dataGridView1.Update();
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("The warehouse capacity is full!");
                    }
                }));
            });
        }

        private async void PackingForm_Load(object sender, EventArgs e)
        {
            await FillPawningsWithNoAddress();
            await FillAddresses();
        }
        public async Task FillPawningsWithNoAddress()
        {
            await Task.Run(() =>
            {
                this.dataGridView1.Invoke(new MethodInvoker(async () =>
                {
                    try
                    {
                        _pawningService.Html.AppendLine("Table 'Pawnings':");
                        this.dataGridView1.Rows.Clear();
                        var pawnings = await _pawningService.GetAllWithNoAddress();
                        foreach (var pawning in pawnings)
                        {
                            this.dataGridView1.Rows.Add(pawning.Id, pawning.Description, pawning.SubmissionDate,
                                pawning.ReturnDate, pawning.Sum, pawning.Commision, pawning.CategoryName);
                            _pawningService.Html.AppendLine($"Pawning Id:{pawning.Id}, Description: {pawning.Description}, Submission Date: {pawning.SubmissionDate}," +
                                $"Return Date: {pawning.ReturnDate}, Sum: {pawning.Sum}, Comission: {pawning.Commision}, Category Name: {pawning.CategoryName}");
                        }

                        _pawningService.Html.Append($"<br>{DateTime.Now}<br>");
                        var pdf = _renderer.RenderHtmlAsPdf(_pawningService.Html.ToString());
                        PdfExtensions.SaveOrMerge(PdfPath, pdf);
                        _pawningService.Html.Clear();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }));
            });
        }
        public async Task FillAddresses()
        {
            await Task.Run(() =>
            {
                this.listBox1.Invoke(new MethodInvoker(async () =>
                {
                    try
                    {
                        _warehouseService.Html.Append("Table 'Warehouses': <br>");
                        this.listBox1.Items.Clear();
                        var warehouses = await _warehouseService.GetAllWithAddressAsync();

                        foreach (var warehouse in warehouses)
                        {
                            this.listBox1.Items.Add(warehouse.Address);
                            _warehouseService.Html.AppendLine($"Warehouse Address: {warehouse.Address}");
                        }

                        _warehouseService.Html.Append($"<br>{DateTime.Now}<br>");
                        var pdf = _renderer.RenderHtmlAsPdf(_warehouseService.Html.ToString());
                        PdfExtensions.SaveOrMerge(PdfPath, pdf);
                        _warehouseService.Html.Clear();
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
