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
using System.Diagnostics;
using System.IO;

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

        private void Form1_Shown(object sender, EventArgs e)
        {
            Path.Text = Environment.GetEnvironmentVariable("appdata") + "\\.minecraft";
            new Thread((ThreadStart)delegate
            {
                Minecraft.Initialize(Path.Text);
                versions = MProfileInfo.GetProfiles();
                Invoke((MethodInvoker)delegate
                {
                    MProfileInfo[] array = versions;
                    foreach (MProfileInfo mProfileInfo in array)
                    {
                        Cb_Version.Items.Add(mProfileInfo.Name);
                    }
                });
            }).Start();
        }

        private void Launch_Click(object sender, EventArgs e)
        {
            if (session == null)
            {
                MessageBox.Show("Please login first.");
            }
            else
            {
                if (Cb_Version.Text == "")
                {
                    return;
                }
                Cb_Version.Enabled = false;
                Launch.Enabled = false;
                string startVersion = Cb_Version.Text;
                string javaPath = Txt_Java.Text;
                string xmx = "2048";
                new Thread((ThreadStart)delegate
                {
                    MProfile mProfile = MProfile.FindProfile(versions, startVersion);
                    DownloadGame(mProfile);
                    MLaunchOption mLaunchOption = new MLaunchOption
                    {
                        StartProfile = mProfile,
                        JavaPath = javaPath,
                        MaximumRamMb = int.Parse(xmx),
                        Session = session
                    };

                    Process process = new MLaunch(mLaunchOption).GetProcess();
                    Invoke((MethodInvoker)delegate
                    {
                        Launch.Enabled = true;
                        Cb_Version.Enabled = true;
                    });
                    DebugProcess(process);
                }).Start();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            MJava mJava = new MJava(Minecraft.DefaultPath + "\\runtime");
            if (!mJava.CheckJavaw())
            {
                rundown form = new rundown();
                form.Show();
                bool iscom = false;
                mJava.DownloadProgressChanged += delegate (object s, ProgressChangedEventArgs v)
                {
                    form.ChangeProgress(v.ProgressPercentage);
                };
                mJava.UnzipCompleted += delegate
                {
                    form.Close();
                    Show();
                    iscom = true;

                };
                mJava.DownloadJavaAsync();
                while (!iscom)
                {
                    Application.DoEvents();
                }
            }
            Txt_Java.Text = Minecraft.DefaultPath + "\\runtime\\bin\\javaw.exe";
        }

        private void Downloader_ChangeProgress(object sender, ProgressChangedEventArgs e)
        {
            Invoke((MethodInvoker)delegate
            {
                launchbar2.Value = e.ProgressPercentage;
            });
        }

        private void DownloaderChangeFile(DownloadFileChangedEventArgs e)
        {
            Invoke((MethodInvoker)delegate
            {
                Lv_Status.Text = e.FileKind.ToString() + ": " + e.FileName;
                launchbar1.Maximum = e.TotalFileCount;
                launchbar1.Value = e.ProgressedFileCount;
            });
        }

        private void DownloadGame(MProfile profile, bool downloadResource = true)
        {
            MDownloader mDownloader = new MDownloader(profile);
            mDownloader.ChangeFile += DownloaderChangeFile;
            mDownloader.ChangeProgress += Downloader_ChangeProgress;
            mDownloader.DownloadAll(downloadResource);
        }

        private void DebugProcess(Process process)
        {
            File.WriteAllText("launcher.txt", process.StartInfo.Arguments);
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.EnableRaisingEvents = true;
            process.Start();
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();
        }
    }
}
