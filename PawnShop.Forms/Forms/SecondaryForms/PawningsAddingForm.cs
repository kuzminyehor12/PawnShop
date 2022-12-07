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
using PawnShop.Forms.Validation;

namespace PawnShop.Forms.Forms.SecondaryForms
{
    public partial class PawningsAddingForm : Form
    {
        private readonly string PdfPath;
        private readonly BasePdfRenderer _renderer;
        public PawningsAddingForm(BasePdfRenderer renderer, string pdfPath)
        {
            InitializeComponent();
            _renderer = renderer;
            PdfPath = pdfPath;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.OpenChildForm(new PawningsForm(), this);
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (TrySendErrorMessage())
            {
                return;
            }

            if (listBox1.SelectedIndex == -1 || listBox2.SelectedIndex == -1)
            {
                MessageBox.Show("List Boxes element must be chosen by mouse click!", "List Box Error",
                    MessageBoxButtons.OK);
                return;
            }

            StringBuilder html = new StringBuilder();
            html.Append("Table 'Pawnings': <br>'");

            await using IPawningService pawningService = new PawningService(html);
            await using ICategoryService categoryService = new SQLCategoryService(html);
            await using IClientPassportService clientService = new ClientPassportService(html);

            var names = listBox1.Items[listBox1.SelectedIndex].ToString().Split(' ');

            var category = await categoryService
                .GetOneByFIlter(c =>
                    c.Name == listBox2.Items[listBox2.SelectedIndex].ToString());

            var client = await clientService
                .GetClient(c =>
                    c.Surname == names[0] 
                    && c.Name == names[1] 
                    && names.Length == 3 ? c.Patronymic == names[2] : c.Patronymic == null);

            var pawning = new Pawnings
            {
                PawningId = Guid.NewGuid(),
                Description = richTextBox1.Text,
                SubmissionDate = DateTime.Parse(dateTimePicker1.Text),
                ReturnDate = DateTime.Parse(dateTimePicker2.Text),
                Sum = double.Parse(textBox1.Text),
                Commision = double.Parse(textBox2.Text),
                ClientId = client.ClientId,
                CategoryId = category.CategoryId
            };

            await pawningService.AddAsync(pawning);
            html.Append($"Current Client Count: {pawningService.GetCount()}<br>");

            var pdf = _renderer.RenderHtmlAsPdf(html.ToString());
            pdf.AppendPdf(PdfDocument.FromFile(PdfPath));
            PdfExtensions.SaveOrMerge(PdfPath, pdf);

            MessageBox.Show("Data has been added successfully!", "Adding", MessageBoxButtons.OK);
            this.OpenChildForm(new PawningsForm(), this);
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
                    throw new FormValidationException("Some data is invalid");
                }
            }
        }

        private Label[] GetStateLabels()
        {
            return new[]
            {
                label8, label9, label10, label11, label12
            };
        }

        private async void PawningsAddingForm_Load(object sender, EventArgs e)
        {
            await FillClients();
            await FillCategories();
        }

        private async Task FillClients()
        {
            await Task.Run(() =>
            {
                listBox1.Invoke(new MethodInvoker(async () =>
                {
                    StringBuilder html = new StringBuilder();
                    await using IClientPassportService service = new ClientPassportService(html);
                    var clientNames = await service.GetAllAsync();

                    foreach (var name in clientNames
                                 .Select(c => c.FullName)
                                 .OrderBy(n => n))
                    {
                        listBox1.Items.Add(name);
                    }
                }));
            });
        }

        private async Task FillCategories()
        {
            await Task.Run(() =>
            {
                listBox2.Invoke(new MethodInvoker(async () =>
                {
                    await using ICategoryService service = new CategoryService();
                    var categoryNames = await service.GetAllAsync();

                    foreach (var name in categoryNames
                                 .Select(c => c.Name)
                                 .OrderBy(n => n))
                    {
                        listBox2.Items.Add(name);
                    }
                }));
            });
        }

        private void dateTimePicker1_Validating(object sender, CancelEventArgs e)
        {
            if (DateTime.Parse(dateTimePicker1.Text) > DateTime.Parse(dateTimePicker2.Text))
            {
                label8.SetInvalid();
            }
            else
            {
                label8.SetValid();
            }
        }

        private void dateTimePicker2_Validating(object sender, CancelEventArgs e)
        {
            if (DateTime.Parse(dateTimePicker1.Text) > DateTime.Parse(dateTimePicker2.Text))
            {
                label9.SetInvalid();
            }
            else
            {
                label9.SetValid();
            }
        }

        private void textBox1_Validating(object sender, CancelEventArgs e)
        {
            if (double.TryParse(textBox1.Text, out double res) && res > 0)
            {
                label10.SetValid();
            }
            else
            {
                label10.SetInvalid();
            }
        }

        private void textBox2_Validating(object sender, CancelEventArgs e)
        {
            if (double.TryParse(textBox2.Text, out double res) && res > 0)
            {
                label11.SetValid();
            }
            else
            {
                label11.SetInvalid();
            }
        }

        private void richTextBox1_Validating(object sender, CancelEventArgs e)
        {
            if (richTextBox1.Text != string.Empty)
            {
                label12.SetValid();
            }
            else
            {
                label12.SetInvalid();
            }
        }
    }
}
