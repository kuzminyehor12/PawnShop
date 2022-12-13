using System;
using System.Configuration;
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
    public partial class CategoriesForm : Form
    {
        private readonly CategoriesAddingForm _secondaryForm;
        private readonly CategoryService _categoryService;
        private readonly BasePdfRenderer _renderer;
        private const string Pdfpath = "../../../../PDFs/Categories.pdf";
        public CategoriesForm()
        {
            InitializeComponent();
            _renderer = new ChromePdfRenderer();
            _categoryService = new CategoryService(ConfigurationManager.ConnectionStrings["PawnShopOracleDb"].ConnectionString);
            _secondaryForm = new CategoriesAddingForm(_categoryService, _renderer, Pdfpath);
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
                        _categoryService.Html.Append("Table 'Categories': <br>");

                        var categories = await _categoryService.GetAllAsync();

                        foreach (var row in categories)
                        {
                            dataGridView1.Rows.Add(row.Id, row.Name, row.Description);
                            _categoryService.Html.Append($"Category Id: {row.Id}, Category Name: {row.Name}, Category Description: {row.Description}<br>");
                        }

                        _categoryService.Html.Append($"<br>{DateTime.Now}<br>");
                        var pdf = _renderer.RenderHtmlAsPdf(_categoryService.Html.ToString());
                        PdfExtensions.SaveOrMerge(Pdfpath, pdf);
                        _categoryService.Html.Clear();
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
                    if (dataGridView1.SelectedRows == null)
                    {
                        MessageBox.Show("There is nothing to delete.", "Delete Message", MessageBoxButtons.OK);
                        return;
                    }

                    try
                    {
                        _categoryService.Html.Append("Table 'Categories': <br>");

                        foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                        {
                            var findingCategory = await _categoryService.GetByIdAsync(Convert.ToDecimal(row.Cells["CategoryId"].EditedFormattedValue));
                            await _categoryService.DeleteAsync(findingCategory);
                        }

                        _categoryService.Html.Append($"Current Category Count: {_categoryService.GetCount()}<br>");
                        _categoryService.Html.Append($"<br>{DateTime.Now}<br>");
                        var pdf = _renderer.RenderHtmlAsPdf(_categoryService.Html.ToString());
                        PdfExtensions.SaveOrMerge(Pdfpath, pdf);
                        MessageBox.Show("Data has been deleted successfully!", "Delete", MessageBoxButtons.OK);
                        _categoryService.Html.Clear();
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

            if (!IsUpdateValid(dataRow))
            {
                MessageBox.Show("Data was input incorrectly", "Error", MessageBoxButtons.OK);
                return;
            }

            try
            {
                dataGridView1.Update();
                _categoryService.Html.Append("Table 'Categories': <br>");

                var findingCategory = await _categoryService.GetByIdAsync(Convert.ToDecimal(dataRow.Cells["CategoryId"].EditedFormattedValue));

                var category = new Category
                {
                    Id = findingCategory.Id,
                    Name = dataRow.Cells["CategoryName"].EditedFormattedValue.ToString(),
                    Description = dataRow.Cells["CategoryNote"].EditedFormattedValue.ToString(),
                };

                await _categoryService.UpdateAsync(category);
                _categoryService.Html.Append($"Current Category Count: {_categoryService.GetCount()}<br>");
                _categoryService.Html.Append($"<br>{DateTime.Now}<br>");
                var pdf = _renderer.RenderHtmlAsPdf(_categoryService.Html.ToString());
                PdfExtensions.SaveOrMerge(Pdfpath, pdf);
                MessageBox.Show("Data has been updated successfully!", "Update", MessageBoxButtons.OK);
                _categoryService.Html.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private bool IsUpdateValid(DataGridViewRow dataRow)
        {
            return dataRow.Cells["CategoryName"].EditedFormattedValue.ToString() != string.Empty
                   && dataRow.Cells["CategoryNote"].EditedFormattedValue.ToString() != string.Empty;
        }
    }
}
