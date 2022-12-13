using IronPdf;
using IronPdf.Rendering.Abstractions;
using PawnShop.Forms.Extensions;
using PawnShop.Oracle.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PawnShop.Forms.Forms.BaseForms
{
    public partial class AnalyticsForm : Form
    {
        private readonly PawningService _pawningService;
        private readonly BasePdfRenderer _renderer;
        private const string Pdfpath = "../../../../PDFs/Pawnings.pdf";
        public AnalyticsForm()
        {
            InitializeComponent();
            _pawningService = new PawningService(ConfigurationManager.ConnectionStrings["PawnShopOracleDb"].ConnectionString);
            _renderer = new ChromePdfRenderer();
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                dataGridView1.Invoke(new MethodInvoker(async () =>
                {
                    try
                    {
                        dataGridView1.Rows.Clear();
                        _pawningService.Html.AppendLine("Table 'Pawnings':");
                        var statistics = await _pawningService.GetCategoriesStatistic();

                        foreach (var stat in statistics)
                        {
                            dataGridView1.Rows.Add(stat.CategoryName, stat.Max, stat.Min, stat.Average, stat.Count);
                            _pawningService.Html.AppendLine($"Category Name: {stat.CategoryName}, Max: {stat.Max}, Min: {stat.Min}, Average: {stat.Average}, Count: {stat.Count}");
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
    }
}
