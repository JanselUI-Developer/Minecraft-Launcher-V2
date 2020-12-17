using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CmlLib.Utils;
using CmlLib.Launcher;
using System.Threading;

namespace Minecraft_Launcher_V2
{
    public partial class Form1 : Form
    {
        private MProfileInfo[] versions;
        private bool allowOffline = true;
        private MSession session;

        public Form1()
        {
            InitializeComponent();
            HomeGB.Hide();
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            GrBx1.Hide();
            HomeGB.Show();
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            GrBx1.Show();
            HomeGB.Hide();
        }

        private void guna2Button4_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void guna2Button1_Click(object sender, EventArgs e) //Login Button
        {
            guna2Button1.Enabled = false;
            if (password.Text == "")
            {
                if (allowOffline)
                {
                    if (usermail.Text == "")
                    {
                        MessageBox.Show("Please enter a Username");
                        guna2Button1.Enabled = true;
                    }
                    else
                    {
                        session = MSession.GetOfflineSession(usermail.Text);
                        MessageBox.Show("Offline/Cracked Login succeeded: " + usermail.Text);
                        usermail.Text = "";
                    }
                }
                else
                {
                    MessageBox.Show("Password was empty. Please try again.");
                    guna2Button1.Enabled = true;
                }
                return;
            }
            new Thread((ThreadStart)delegate
            {
                MSession mSession = new MLogin().Authenticate(usermail.Text, password.Text);
                if (mSession.Result == MLoginResult.Success)
                {
                    MessageBox.Show("Login Success : " + mSession.Username);
                    session = mSession;
                    usermail.Text = "";
                    password.Text = "";
                }
                else
                {
                    MessageBox.Show(mSession.Result.ToString() + "\n" + mSession.Message);
                    Invoke((MethodInvoker)delegate
                    {
                        guna2Button1.Enabled = true;
                    });
                }
            }).Start();
        }

    }
}
