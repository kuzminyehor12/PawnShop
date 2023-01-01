using PawnShop.Oracle.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PawnShop.Forms.Forms.BaseForms
{
    public partial class UserForm : Form
    {
        public string ConnectionString { get; }
        private readonly IdentityService _identityService;
        public UserForm()
        {
            InitializeComponent();
            ConnectionString = ConfigurationManager.ConnectionStrings["PawnShopOracleDb"].ConnectionString;
            _identityService = new IdentityService(ConnectionString);
        }

        private async void UserForm_Load(object sender, EventArgs e)
        {
            await FillUsersAndRoles();
        }

        private async Task FillUsersAndRoles()
        {
            await Task.Run(() =>
            {
                dataGridView1.Invoke(new MethodInvoker(async () =>
                {
                    try
                    {
                        var users = await _identityService.GetAllWithRolesAsync();

                        foreach (var user in users)
                        {
                            dataGridView1.Rows.Add(user.FirstName + " " + user.LastName, user.Email, user.Role);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }));
            });
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            AuthorizationForm authForm = new AuthorizationForm();
            authForm.Show();
            Visible = false;
        }
    }
}
