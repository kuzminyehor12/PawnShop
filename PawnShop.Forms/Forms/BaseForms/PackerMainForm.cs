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
    public partial class PackerMainForm : Form
    {
        private readonly WarehouseForm _warehouseForm;
        private readonly PackingForm _packingForm;
        public PackerMainForm()
        {
            InitializeComponent();
            _warehouseForm = new WarehouseForm();
            _packingForm = new PackingForm();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.OpenChildForm(_warehouseForm, splitContainer1.Panel2);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.OpenChildForm(_packingForm, splitContainer1.Panel2);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            AuthorizationForm authorizationForm = new AuthorizationForm();
            authorizationForm.Show();
            Visible = false;
        }
    }
}
