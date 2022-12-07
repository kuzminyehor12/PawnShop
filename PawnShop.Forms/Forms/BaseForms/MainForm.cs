using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using PawnShop.Forms.Extensions;
using PawnShop.Forms.Forms.BaseForms;

namespace PawnShop.Forms.Forms
{
    public partial class MainForm : Form
    {
        private readonly ClientsForm _clientsForm;
        private readonly CategoriesForm _categoriesForm;
        private readonly PawningsForm _pawningsForm;
        private readonly AnalyticsForm _analyticsForm;
        public MainForm()
        {
            InitializeComponent();
            _clientsForm = new ClientsForm();
            _categoriesForm = new CategoriesForm();
            _pawningsForm = new PawningsForm();
            _analyticsForm = new AnalyticsForm();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.OpenChildForm(_clientsForm, splitContainer1.Panel2);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.OpenChildForm(_categoriesForm, splitContainer1.Panel2);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.OpenChildForm(_pawningsForm, splitContainer1.Panel2);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.OpenChildForm(_analyticsForm, splitContainer1.Panel2);
        }
    }
}
