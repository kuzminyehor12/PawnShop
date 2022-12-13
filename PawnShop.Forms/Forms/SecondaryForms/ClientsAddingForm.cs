using System;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using IronPdf;
using IronPdf.Rendering.Abstractions;
using PawnShop.Data.Models;
using PawnShop.Forms.Extensions;
using PawnShop.Forms.Validation;
using PawnShop.Oracle.Services;

namespace PawnShop.Forms.Forms.SecondaryForms
{
    public partial class ClientsAddingForm : Form
    {
        private readonly string PdfPath;
        private readonly BasePdfRenderer _renderer;
        private readonly ClientService _clientService;
        public ClientsAddingForm(ClientService clientService, BasePdfRenderer renderer, string pdfPath)
        {
            InitializeComponent();
            _clientService = clientService;
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

            try
            {
                _clientService.Html.Append("Table 'Clients': <br>'");

                var names = textBox1.Text.Split(' ');

                var client = new Client
                {
                    FirstName = names[0],
                    LastName = names[1]
                };

                await _clientService.AddAsync(client);

                var createdClient = await _clientService.GetByFullName(client.FullName);
                var passport = new Passport
                {
                    Number = textBox4.Text,
                    Series = textBox5.Text,
                    DateOfIssue = dateTimePicker1.Text,
                    ClientId = createdClient.Id
                };

                await _clientService.AddPassportAsync(passport);
                _clientService.Html.Append($"Current Client Count: {_clientService.GetCount()}<br>");

                var pdf = _renderer.RenderHtmlAsPdf(_clientService.Html.ToString());
                pdf.AppendPdf(PdfDocument.FromFile(PdfPath));
                PdfExtensions.SaveOrMerge(PdfPath, pdf);
                _clientService.Html.Clear();

                MessageBox.Show("Data has been added successfully!", "Adding", MessageBoxButtons.OK);
                this.OpenChildForm(new ClientsForm(), this);
            }
            catch (Exception)
            {
                MessageBox.Show("Client with those credentials already exists.");
            }
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
            if (!textBox1.Text.ContainsDigit() && textBox1.Text.Split(' ').Length == 2)
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
