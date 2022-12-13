using IronPdf;
using IronPdf.Rendering.Abstractions;
using PawnShop.Forms.Extensions;
using PawnShop.Forms.Forms.BaseForms;
using PawnShop.Forms.Validation;
using PawnShop.Oracle.Models;
using PawnShop.Oracle.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PawnShop.Forms.Forms.SecondaryForms
{
    public partial class WarehouseAddingForm : Form
    {
        public string PdfPath { get; }
        private readonly BasePdfRenderer _renderer;
        private readonly WarehouseService _warehouseService;
        public WarehouseAddingForm(
            string path, 
            BasePdfRenderer renderer, 
            WarehouseService warehouseService)
        {
            InitializeComponent();
            _warehouseService = warehouseService;
            PdfPath = path;
            _renderer = renderer;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.OpenChildForm(new WarehouseForm(), this);
        }

        private void textBox1_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text) || textBox1.Text.ContainsDigit() || textBox1.Text.ContainsSpecialChar())
            {
                label1.SetInvalid();
            }
            else
            {
                label1.SetValid();
            }
        }

        private void textBox3_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(textBox3.Text) || textBox3.Text.ContainsDigit() || textBox3.Text.ContainsSpecialChar())
            {
                label3.SetInvalid();
            }
            else
            {
                label3.SetValid();
            }
        }

        private void textBox2_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(textBox2.Text) || textBox2.Text.ContainsDigit() || textBox2.Text.ContainsSpecialChar())
            {
                label2.SetInvalid();
            }
            else
            {
                label2.SetValid();
            }
        }

        private void textBox4_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(textBox4.Text))
            {
                label4.SetInvalid();
            }
            else
            {
                label4.SetValid();
            }
        }

        private void textBox5_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(textBox5.Text) || !int.TryParse(textBox5.Text, out var capacity))
            {
                label5.SetInvalid();
            }
            else
            {
                label5.SetValid();
            }
        }

        private void textBox6_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(textBox6.Text) || !int.TryParse(textBox6.Text, out var size))
            {
                label6.SetInvalid();
            }
            else
            {
                label6.SetValid();
            }
        }

        private bool TryCheckValid()
        {
            try
            {
                CheckValid();
                return true;
            }
            catch (FormValidationException e)
            {
                MessageBox.Show(e.Message, e.GetType().ToString(), MessageBoxButtons.OK);
                return false;
            }
        }

        private void CheckValid()
        {
            foreach (var l in GetStateLabels())
            {
                if (l.Text == "Invalid")
                {
                    throw new FormValidationException("Some data is invalid");
                }
            }
        }

        private Label[] GetStateLabels()
        {
            return new[]
            {
                label1, label2, label3, label4, label5, label6
            };
        }


        private async void button1_Click(object sender, EventArgs e)
        {
            if (!TryCheckValid())
            {
                return;
            }

            try
            {
                _warehouseService.Html.AppendLine("Table 'Warehouses':");
                var address = new Address
                {
                    Country = textBox1.Text,
                    City = textBox2.Text,
                    Street = textBox3.Text,
                    Number = textBox4.Text
                };

                await _warehouseService.AddAddressAsync(address);
                var createdAddressId = await _warehouseService.GetAddressIdByModelAsync(address);

                var warehouse = new Warehouse
                {
                    AddressId = createdAddressId,
                    Capacity = int.Parse(textBox5.Text),
                    Size = int.Parse(textBox6.Text)
                };

                await _warehouseService.AddAsync(warehouse);

                _warehouseService.Html.Append($"Current Warehouses Count: {_warehouseService.GetCount()}<br>");

                var pdf = _renderer.RenderHtmlAsPdf(_warehouseService.Html.ToString());
                pdf.AppendPdf(PdfDocument.FromFile(PdfPath));
                PdfExtensions.SaveOrMerge(PdfPath, pdf);
                _warehouseService.Html.Clear();
                MessageBox.Show("Data has been added successfully!", "Adding", MessageBoxButtons.OK);
                this.OpenChildForm(new WarehouseForm(), this);
            }
            catch (Exception)
            {
                MessageBox.Show("Perhaps this warehouse exist already!");
            }
        }
    }
}
