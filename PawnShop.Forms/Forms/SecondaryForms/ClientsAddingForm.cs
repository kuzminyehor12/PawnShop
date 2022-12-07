using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using IronPdf;
using IronPdf.Rendering.Abstractions;
using PawnShop.Business.DTO;
using PawnShop.Business.Interfaces;
using PawnShop.Business.Services;
using PawnShop.Data.Models;
using PawnShop.Forms.Extensions;
using PawnShop.Forms.Validation;

namespace PawnShop.Forms.Forms.SecondaryForms
{
    public partial class ClientsAddingForm : Form
    {
        private readonly string PdfPath;
        private readonly BasePdfRenderer _renderer;
        public ClientsAddingForm(BasePdfRenderer renderer, string pdfPath)
        {
            InitializeComponent();
            _renderer = renderer;
            PdfPath = pdfPath;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.OpenChildForm(new ClientsForm(), this);
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (TrySendErrorMessage())
            {
                return;
            }

            StringBuilder html = new StringBuilder();
            html.Append("Table 'Clients': <br>'");
            await using IClientPassportService service = new ClientPassportService(html);

            var dto = new ClientPassportDTO
            {
                PassportId = Guid.NewGuid(),
                PassportNumber = textBox4.Text,
                PassportSeries = textBox5.Text,
                DateOfIssue = DateTime.Parse(dateTimePicker1.Text),
                ClientId = Guid.NewGuid(),
                FullName = textBox1.Text
            };

            await service.AddAsync(dto);
            html.Append($"Current Client Count: {service.GetCount()}<br>");

            var pdf = _renderer.RenderHtmlAsPdf(html.ToString());
            pdf.AppendPdf(PdfDocument.FromFile(PdfPath));
            PdfExtensions.SaveOrMerge(PdfPath, pdf);

            MessageBox.Show("Data has been added successfully!", "Adding", MessageBoxButtons.OK);
            this.OpenChildForm(new ClientsForm(), this);
        }

        private bool TrySendErrorMessage()
        {
            try
            {
                CheckValid();
                return false;
            }
            catch (FormValidationException e)
            {
                MessageBox.Show(e.Message, e.GetType().ToString(), MessageBoxButtons.OK);
                return true;
            }
        }

        private void CheckValid()
        {
            foreach (var l in GetStateLabels())
            {
                if (l.Text == "Invalid")
                {
                    throw new FormValidationException("Data was input invalid!");
                }
            }
        }
        private Label[] GetStateLabels()
        {
            return new[]
            {
                label2, label3, label7, label8
            };
        }

        private void textBox1_Validating(object sender, CancelEventArgs e)
        {
            if (!textBox1.Text.ContainsDigit() && textBox1.Text.Split(' ').Length >= 2)
            {
                label2.SetValid();
            }
            else
            {
                label2.SetInvalid();
            }
        }

        private void textBox4_Validating(object sender, CancelEventArgs e)
        {
            if (Regex.IsMatch(textBox4.Text, "^[0-9]{6}$"))
            {
                label3.SetValid();
            }
            else
            {
                label3.SetInvalid();
            }
        }

        private void textBox5_Validating(object sender, CancelEventArgs e)
        {
            if (Regex.IsMatch(textBox5.Text, "^[\\p{IsCyrillic}]{2}$"))
            {
                label7.SetValid();
            }
            else
            {
                label7.SetInvalid();
            }
        }

        private void dateTimePicker1_Validating(object sender, CancelEventArgs e)
        {
            if (DateTime.Parse(dateTimePicker1.Text) < DateTime.Now)
            {
                label8.SetValid();
            }
            else
            {
                label8.SetInvalid();
            }
        }
    }
}
