using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PawnShop.Business.Interfaces;
using PawnShop.Business.Services;

namespace PawnShop.Forms.Forms.BaseForms
{
    public partial class AnalyticsForm : Form
    {
        public AnalyticsForm()
        {
            InitializeComponent();
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                dataGridView1.Invoke(new MethodInvoker(async () =>
                {
                    dataGridView1.Rows.Clear();

                    await using ICategoryService service = new CategoryService();
                    var categories = await service.GetAllWithDetails();

                    foreach (var elem in categories)
                    {
                        if (elem.Pawnings.Any())
                        {
                            dataGridView1.Rows.Add(elem.Name, elem.Pawnings.Max(p => p.Sum),
                                elem.Pawnings.Min(p => p.Sum), elem.Pawnings.Count);
                        }
                    }
                }));
            });
        }
    }
}
