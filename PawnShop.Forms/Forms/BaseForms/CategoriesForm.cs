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
    public partial class CategoriesForm : Form
    {
        private readonly CategoriesAddingForm _secondaryForm;
        private readonly BasePdfRenderer _renderer;
        private const string Pdfpath = "../../../../PDFs/Categories.pdf";
        public CategoriesForm()
        {
            InitializeComponent();
            _renderer = new ChromePdfRenderer();
            _secondaryForm = new CategoriesAddingForm(_renderer, Pdfpath);
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                dataGridView1.Invoke(new MethodInvoker(async () =>
                {
                    dataGridView1.Rows.Clear();
                    StringBuilder html = new StringBuilder();
                    html.Append("Table 'Categories': <br>");

                    await using ICategoryService service = new SQLCategoryService(html);
                    var categories = await service.GetAllAsync();

                    foreach (var row in categories.OrderBy(c => c.Name))
                    {
                        dataGridView1.Rows.Add(row.Name, row.Note);
                        html.Append($"Category Name: {row.Name}, Category Note: {row.Note}<br>");
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
                    if (dataGridView1.SelectedRows == null)
                    {
                        MessageBox.Show("There is nothing to delete.", "Delete Message", MessageBoxButtons.OK);
                        return;
                    }

                    StringBuilder html = new StringBuilder();
                    html.Append("Table 'Categories': <br>");

                    await using ICategoryService service = new SQLCategoryService(html);

                    foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                    {
                        var findingCategory = await service
                            .GetOneByFIlter(p => 
                                p.Name == row.Cells["CategoryName"].EditedFormattedValue.ToString());

                        await service.DeleteAsync(findingCategory);
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

            if (!IsUpdateValid(dataRow))
            {
                MessageBox.Show("Data was input incorrectly", "Error", MessageBoxButtons.OK);
                return;
            }

            dataGridView1.Update();
            StringBuilder html = new StringBuilder();
            html.Append("Table 'Categories': <br>");

            await using ICategoryService service = new SQLCategoryService(html);

            var findingCategory = await service
                .GetOneByFIlter(p =>
                    p.Name == dataRow.Cells["CategoryName"].EditedFormattedValue.ToString());

            var category = new Categories
            {
                CategoryId = findingCategory.CategoryId,
                Name = dataRow.Cells["CategoryName"].EditedFormattedValue.ToString(),
                Note = dataRow.Cells["CategoryNote"].EditedFormattedValue.ToString(),
            };

            await service.UpdateAsync(category);
            html.Append($"Current Client Count: {service.GetCount()}<br>");
            html.Append($"<br>{DateTime.Now}<br>");
            var pdf = _renderer.RenderHtmlAsPdf(html.ToString());
            PdfExtensions.SaveOrMerge(Pdfpath, pdf);

            MessageBox.Show("Data has been updated successfully!", "Update", MessageBoxButtons.OK);
        }

        private bool IsUpdateValid(DataGridViewRow dataRow)
        {
            return dataRow.Cells["CategoryName"].EditedFormattedValue.ToString() != string.Empty
                   && dataRow.Cells["CategoryNote"].EditedFormattedValue.ToString() != string.Empty;
        }
    }
}
