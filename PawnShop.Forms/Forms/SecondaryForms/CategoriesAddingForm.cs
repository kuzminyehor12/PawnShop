using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using IronPdf;
using IronPdf.Rendering.Abstractions;
using PawnShop.Data.Models;
using PawnShop.Forms.Extensions;
using PawnShop.Forms.Validation;
using PawnShop.Oracle.Services;

namespace PawnShop.Forms.Forms.SecondaryForms
{
    public partial class CategoriesAddingForm : Form
    {
        private readonly string PdfPath;
        private readonly BasePdfRenderer _renderer;
        private readonly CategoryService _categoryService;
        public CategoriesAddingForm(CategoryService categoryService, BasePdfRenderer renderer, string pdfPath)
        {
            InitializeComponent();
            _renderer = renderer;
            PdfPath = pdfPath;
            _categoryService = categoryService;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (TrySendErrorMessage())
            {
                return;
            }

            try
            {
                _categoryService.Html.Append("Table 'Categories': <br>'");

                var category = new Category
                {
                    Name = textBox1.Text,
                    Description = richTextBox1.Text,
                };

                await _categoryService.AddAsync(category);
                _categoryService.Html.Append($"Current Category Count: {_categoryService.GetCount()}<br>");

                var pdf = _renderer.RenderHtmlAsPdf(_categoryService.Html.ToString());
                pdf.AppendPdf(PdfDocument.FromFile(PdfPath));
                PdfExtensions.SaveOrMerge(PdfPath, pdf);

                MessageBox.Show("Data has been added successfully!", "Adding", MessageBoxButtons.OK);
                _categoryService.Html.Clear();
                this.OpenChildForm(new CategoriesForm(), this);
            }
            catch (Exception)
            {
                MessageBox.Show("Perhaps this category exist already!");
            }
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
