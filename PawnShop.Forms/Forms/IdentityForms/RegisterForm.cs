﻿using Oracle.ManagedDataAccess.Client;
using PawnShop.Forms.Extensions;
using PawnShop.Forms.Forms.BaseForms;
using PawnShop.Forms.Validation;
using PawnShop.Oracle.Extensions;
using PawnShop.Oracle.Models;
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

namespace PawnShop.Forms.Forms.IdentityForms
{
    public partial class RegisterForm : Form
    {
        private string ConnectionString { get; }
        private readonly IdentityService _identityService;
        public RegisterForm()
        {
            InitializeComponent();
            ConnectionString = ConfigurationManager.ConnectionStrings["PawnShopOracleDb"].ConnectionString;
            _identityService = new IdentityService(ConnectionString);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var authForm = new AuthorizationForm();
            authForm.Show();
            Visible = false;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (!TryCheckValid())
            {
                return;
            }

            try
            {
                var user = new User
                {
                    FirstName = textBox2.Text,
                    LastName = textBox1.Text,
                    DateOfBirth = dateTimePicker1.Text,
                    Email = textBox5.Text,
                    Password = PasswordSecurity.Hash(textBox3.Text)
                };

                await _identityService.AddAsync(user);

                var createdUser = await _identityService.FindByEmailAsync(user.Email);
                var role = await _identityService.GetRoleByName(listBox1.Items[listBox1.SelectedIndex].ToString());
                var userRole = new UserRole
                {
                    UserId = createdUser.Id,
                    RoleId = role.Id
                };

                await _identityService.AddUserRole(userRole);

                var authForm = new AuthorizationForm();
                authForm.Show();
                Visible = false;
            }
            catch (Exception)
            {
                MessageBox.Show("User with entered email currently exists. Try to authorize!");
            }
        }

        private bool TryCheckValid()
        {
            try
            {
                CheckValid();
                return true;
            }
            catch (FormValidationException e)
            {
                MessageBox.Show(e.Message, e.GetType().ToString(), MessageBoxButtons.OK);
                return false;
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
                label2, label3, label6, label8, label5, label4
            };
        }


        private void textBox2_Validating(object sender, CancelEventArgs e)
        {
            if(string.IsNullOrEmpty(textBox2.Text) || textBox2.Text.ContainsDigit() || textBox2.Text.ContainsSpecialChar())
            {
                label2.SetInvalid();
            }
            else
            {
                label2.SetValid();
            }
        }

        private void textBox1_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text) || textBox1.Text.ContainsDigit() || textBox2.Text.ContainsSpecialChar())
            {
                label3.SetInvalid();
            }
            else
            {
                label3.SetValid();
            }
        }

        private void textBox5_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(textBox5.Text) || !textBox5.Text.IsEmail())
            {
                label6.SetInvalid();
            }
            else
            {
                label6.SetValid();
            }
        }

        private void dateTimePicker1_Validating(object sender, CancelEventArgs e)
        {
            if (DateTime.Now.Year - DateTime.Parse(dateTimePicker1.Text).Year >= 18)
            {
                label8.SetValid();
            }
            else
            {
                label8.SetInvalid();
            }
        }

        private void textBox3_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(textBox3.Text) || textBox3.Text.IsPasswordValid(out var errorMessage))
            {
                label4.SetValid();
            }
            else
            {
                MessageBox.Show(errorMessage);
                label4.SetInvalid();
            }
        }

        private void textBox4_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(textBox4.Text) || !textBox4.Text.Equals(textBox3.Text))
            {
                label5.SetInvalid();
            }
            else
            {
                label5.SetValid();
            }
        }

        private void listBox1_Validating(object sender, CancelEventArgs e)
        {

        }

        private async void RegisterForm_Load(object sender, EventArgs e)
        {
            await FillRoles();
        }

        public async Task FillRoles()
        {
            await Task.Run(() =>
            {
                listBox1.Invoke(new MethodInvoker(async () =>
                {
                    var roles = await _identityService.GetRolesAsync();
                    foreach (var role in roles)
                    {
                        listBox1.Items.Add(role.ShortName);
                    }
                }));
            });
        }
    }
}
