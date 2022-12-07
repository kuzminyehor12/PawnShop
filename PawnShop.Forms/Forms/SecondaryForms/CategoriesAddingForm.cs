using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using IronPdf;
using IronPdf.Rendering.Abstractions;
using PawnShop.Business.Interfaces;
using PawnShop.Business.Services;
using PawnShop.Data.Models;
using PawnShop.Forms.Extensions;
using PawnShop.Forms.Validation;

namespace PawnShop.Forms.Forms.SecondaryForms
{
    public partial class CategoriesAddingForm : Form
    {
        private readonly string PdfPath;
        private readonly BasePdfRenderer _renderer;
        public CategoriesAddingForm(BasePdfRenderer renderer, string pdfPath)
        {
            InitializeComponent();
            _renderer = renderer;
            PdfPath = pdfPath;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (TrySendErrorMessage())
            {
                return;
            }

            StringBuilder html = new StringBuilder();
            html.Append("Table 'Categories': <br>'");
            await using ICategoryService service = new SQLCategoryService(html);

            var category = new Categories
            {
                CategoryId = Guid.NewGuid(),
                Name = textBox1.Text,
                Note = richTextBox1.Text,
            };

            await service.AddAsync(category);
            html.Append($"Current Client Count: {service.GetCount()}<br>");

            var pdf = _renderer.RenderHtmlAsPdf(html.ToString());
            pdf.AppendPdf(PdfDocument.FromFile(PdfPath));
            PdfExtensions.SaveOrMerge(PdfPath, pdf);

            MessageBox.Show("Data has been added successfully!", "Adding", MessageBoxButtons.OK);
            this.OpenChildForm(new CategoriesForm(), this);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.OpenChildForm(new CategoriesForm(), this);
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
                label2, label3
            };
        }

        private void textBox1_Validating(object sender, CancelEventArgs e)
        {
            if (textBox1.Text != string.Empty)
            {
                label3.SetValid();
            }
            else
            {
                label3.SetInvalid();
            }
        }

        private void richTextBox1_Validating(object sender, CancelEventArgs e)
        {
            if (richTextBox1.Text != string.Empty)
            {
                label3.SetValid();
            }
            else
            {
                label3.SetInvalid();
            }
        }
    }
}
