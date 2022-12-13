using IronPdf;
using IronPdf.Rendering.Abstractions;
using PawnShop.Forms.Extensions;
using PawnShop.Forms.Forms.SecondaryForms;
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
    public partial class WarehouseForm : Form
    {
        private readonly WarehouseAddingForm _secondaryForm;
        private readonly WarehouseService _warehouseService;
        private readonly BasePdfRenderer _renderer;
        private const string Pdfpath = "../../../../PDFs/Warehouses.pdf";
        public WarehouseForm()
        {
            InitializeComponent();
            _warehouseService = new WarehouseService(ConfigurationManager.ConnectionStrings["PawnShopOracleDb"].ConnectionString);
            _renderer = new ChromePdfRenderer();
            _secondaryForm = new WarehouseAddingForm(Pdfpath, _renderer, _warehouseService);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            this.OpenChildForm(_secondaryForm, this);
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
                        _warehouseService.Html.Append("Table 'Warehouses': <br>");

                        var warehouses = await _warehouseService.GetAllWithAddressAsync();

                        foreach (var row in warehouses)
                        {
                            dataGridView1.Rows.Add(row.Id, row.Country, row.City,
                                row.Street, row.Number, row.Capacity, row.Size);

                            _warehouseService.Html.Append($"Warehouse Id: {row.Id}, Country: {row.Country}, City: {row.City}, Street: {row.Street}," +
                                        $"Number: {row.Number}, Capacity: {row.Capacity}, Size: {row.Size}<br>");
                        }

                        _warehouseService.Html.Append($"<br>{DateTime.Now}<br>");
                        var pdf = _renderer.RenderHtmlAsPdf(_warehouseService.Html.ToString());
                        PdfExtensions.SaveOrMerge(Pdfpath, pdf);
                        _warehouseService.Html.Clear();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }));
            });
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                dataGridView1.Invoke(new MethodInvoker(async () =>
                {
                    _warehouseService.Html.Append("Table 'Warehouses': <br>");

                    if (dataGridView1.SelectedRows == null)
                    {
                        MessageBox.Show("There is nothing to delete.", "Delete Message", MessageBoxButtons.OK);
                        return;
                    }

                    try
                    {
                        foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                        {
                            var address = new Address
                            {
                                Country = row.Cells["Country"].EditedFormattedValue.ToString(),
                                City = row.Cells["City"].EditedFormattedValue.ToString(),
                                Street = row.Cells["Street"].EditedFormattedValue.ToString(),
                                Number = row.Cells["Number"].EditedFormattedValue.ToString(),
                            };

                            var addressId = await _warehouseService.GetAddressIdByModelAsync(address);
                            var findingAddress = await _warehouseService.GetAddressByIdAsync(addressId);
                            await _warehouseService.DeleteAddressAsync(findingAddress);
                        }

                        _warehouseService.Html.Append($"Current Warehouse Count: {_warehouseService.GetCount()}<br>");
                        _warehouseService.Html.Append($"<br>{DateTime.Now}<br>");
                        var pdf = _renderer.RenderHtmlAsPdf(_warehouseService.Html.ToString());
                        PdfExtensions.SaveOrMerge(Pdfpath, pdf);
                        MessageBox.Show("Data has been deleted successfully!", "Delete", MessageBoxButtons.OK);
                        _warehouseService.Html.Clear();
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

            dataGridView1.Update();
            _warehouseService.Html.Append("Table 'Warehouses': <br>");

            try
            {
                var findingWarehouse = await _warehouseService.GetByIdAsync(Convert.ToDecimal(dataRow.Cells["WarehouseId"].EditedFormattedValue));
                var findingAddress = await _warehouseService.GetAddressByIdAsync(findingWarehouse.AddressId);

                var updatingAddress = new Address
                {
                    Id = findingAddress.Id,
                    Country = dataRow.Cells["Country"].EditedFormattedValue.ToString(),
                    City = dataRow.Cells["City"].EditedFormattedValue.ToString(),
                    Street = dataRow.Cells["Street"].EditedFormattedValue.ToString(),
                    Number = dataRow.Cells["Number"].EditedFormattedValue.ToString()
                };

                await _warehouseService.UpdateAddressAsync(updatingAddress);

                var warehouse = new Warehouse
                {
                    Id = findingWarehouse.Id,
                    AddressId = findingWarehouse.AddressId,
                    Capacity = Convert.ToInt32(dataRow.Cells["Capacity"].EditedFormattedValue),
                    Size = Convert.ToInt32(dataRow.Cells["Size"].EditedFormattedValue)
                };

                await _warehouseService.UpdateAsync(warehouse);

                _warehouseService.Html.Append($"Current Warehouse Count: {_warehouseService.GetCount()}<br>");
                _warehouseService.Html.Append($"<br>{DateTime.Now}<br>");
                var pdf = _renderer.RenderHtmlAsPdf(_warehouseService.Html.ToString());
                PdfExtensions.SaveOrMerge(Pdfpath, pdf);

                MessageBox.Show("Data has been updated successfully!", "Update", MessageBoxButtons.OK);
                _warehouseService.Html.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private bool IsUpdateInvalid(DataGridViewRow dataRow)
        {
            return !dataRow.Cells["Country"].EditedFormattedValue.ToString().ContainsDigitOrSpecialChar()
                   && !dataRow.Cells["City"].EditedFormattedValue.ToString().ContainsDigitOrSpecialChar()
                   && !dataRow.Cells["Street"].EditedFormattedValue.ToString().ContainsDigitOrSpecialChar()
                   && !dataRow.Cells["Number"].EditedFormattedValue.ToString().ContainsDigitOrSpecialChar()
                   && int.TryParse(dataRow.Cells["Capacity"].EditedFormattedValue.ToString(), out var capacity)
                   && int.TryParse(dataRow.Cells["Size"].EditedFormattedValue.ToString(), out var size)
                   && capacity < size && capacity < 0 && size < 0;
        }

    }
}
