using System;
using System.Threading;
using System.Windows.Forms;
using System.Net;
using MaterialSkin;
using MaterialSkin.Controls;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;
using System.ComponentModel;

namespace Lustery_Multitool
{
    public partial class Form1 : MaterialForm
    {
        public Form1()
        {
            InitializeComponent();

            Control.CheckForIllegalCrossThreadCalls = false;

            #region Set theme on load
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Pink400, Primary.Pink500, Primary.Pink600, Accent.Pink200, TextShade.WHITE);
            #endregion

            #region Setup textbox watermark
            materialSingleLineTextField1.Text = "https://lustery.com/videos#video-**";
            this.materialSingleLineTextField1.Leave += new System.EventHandler(this.materialSingleLineTextField1_Leave);
            this.materialSingleLineTextField1.Enter += new System.EventHandler(this.materialSingleLineTextField1_Enter);
            this.ActiveControl = materialLabel1;
            #endregion
        }

        #region Textbox watermark
        private void materialSingleLineTextField1_Leave(object sender, EventArgs e)
        {
            if (materialSingleLineTextField1.Text.Length == 0)
            {
                materialSingleLineTextField1.Text = "https://lustery.com/videos#video-**";
            }
        }
        private void materialSingleLineTextField1_Enter(object sender, EventArgs e)
        {
            if (materialSingleLineTextField1.Text == "https://lustery.com/videos#video-**")
            {
                materialSingleLineTextField1.Text = "";
            }
        }
        #endregion

        #region Stored information
        public class Variables
        {
            /*Quality*/
            private static int quality = 720;
            public static int QUALITY { get { return quality; } set { quality = value; } }

            /*ID*/
            private static string id = "";
            public static string ID { get { return id; } set { id = value; } }

            /*URL*/
            private static string url = "";
            public static string URL { get { return url; } set { url = value; } }

            /*Title*/
            private static string title = "";
            public static string TITLE { get { return title; } set { title = value; } }
        }
        #endregion

        #region Quality
        private void materialRadioButton1_CheckedChanged(object sender, EventArgs e)
        {
            /*Set quality to 720P*/
            Variables.QUALITY = 720;
        }

        private void materialRadioButton2_CheckedChanged(object sender, EventArgs e)
        {
            /*Set quality to 360P*/
            Variables.QUALITY = 360;
        }
        #endregion

        private void materialRaisedButton1_Click(object sender, EventArgs e)
        {
            /*Set file location*/
            FolderBrowserDialog directory = new FolderBrowserDialog();
            directory.ShowDialog();
            materialSingleLineTextField2.Text = directory.SelectedPath + @"\";
        }

        private void materialRaisedButton2_Click(object sender, EventArgs e)
        {
            #region Download button
            /*Check if URL is lustery.com*/
            if (materialSingleLineTextField1.Text.ToLower().Contains("https://lustery.com/videos#video-"))
            {
                /*Get number after -*/
                Variables.ID = materialSingleLineTextField1.Text.Split('-').Last();

                /*Check if ID is a number*/
                double num;
                if (double.TryParse(Variables.ID, out num))
                {
                    /*Save ID as full URL*/
                    Variables.URL = "https://lustery.com/play_video.php/" + Variables.ID + "/";

                    using (var client = new WebClient())
                    {
                        /*Disable buttons etc + get video name from URL*/
                        materialRaisedButton1.Enabled = false;
                        materialRaisedButton3.Enabled = false;
                        materialSingleLineTextField1.Enabled = false;
                        groupBox2.Enabled = false;
                        groupBox3.Enabled = false;

                        /*Shitty regex*/
                        string regex = client.DownloadString(materialSingleLineTextField1.Text);
                        MatchCollection m1 = Regex.Matches(regex, @"<span class=""title"">\s*<a href=""#video-" + Variables.ID + @""" class=""show-video-link show-preview"">\s*(.+?)\s*</a>", RegexOptions.Singleline);
                        foreach (Match fuckregex in m1)
                        {
                            Variables.TITLE = fuckregex.Groups[1].Value;
                        }

                        /*Download video*/
                        try
                        {
                            (new Thread(() =>
                            {
                                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                                client.DownloadFileCompleted += new AsyncCompletedEventHandler(Client_DownloadFileCompleted);
                                client.DownloadFileAsync(new Uri(Variables.URL + Variables.QUALITY), materialSingleLineTextField2.Text + Variables.TITLE + ".mp4");
                                MessageBox.Show("Downloading [" + Variables.TITLE + ".mp4] in " + materialSingleLineTextField2.Text, "Lustery Multitool", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            })).Start();
                        }
                        catch
                        {
                            /*Error downloading video*/
                            materialRaisedButton1.Enabled = true;
                            materialRaisedButton3.Enabled = true;
                            materialSingleLineTextField1.Enabled = true;
                            groupBox2.Enabled = true;
                            groupBox3.Enabled = true;
                            MessageBox.Show("Error downloading video, please check your internet connection and try again.", "Lustery Multitool", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {
                    /*Invalid ID*/
                    MessageBox.Show("Not a valid video ID", "Lustery Multitool", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                /*Invalid URL*/
                MessageBox.Show("Invalid URL", "Lustery Multitool", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            #endregion
        }

        #region Download
        private void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            MessageBox.Show("Finished downloading " + Variables.TITLE + ".mp4", "Lustery Exploit", MessageBoxButtons.OK, MessageBoxIcon.Information);
            materialRaisedButton1.Enabled = true;
            materialRaisedButton3.Enabled = true;
            materialSingleLineTextField1.Enabled = true;
            groupBox2.Enabled = true;
            groupBox3.Enabled = true;
        }

        private void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar1.Maximum = (int)e.TotalBytesToReceive / 100;
            progressBar1.Value = (int)e.BytesReceived / 100;
        }
        #endregion

        private void materialRaisedButton3_Click(object sender, EventArgs e)
        {
            #region Play button
            /*Check if URL is lustery.com*/
            if (materialSingleLineTextField1.Text.ToLower().Contains("https://lustery.com/videos#video-"))
            {
                /*Get number after -*/
                Variables.ID = materialSingleLineTextField1.Text.Split('-').Last();

                /*Check if ID is a number*/
                double num;
                if (double.TryParse(Variables.ID, out num))
                {
                    /*Save ID as full URL*/
                    Variables.URL = "https://lustery.com/play_video.php/" + Variables.ID + "/";

                    /*Play video*/
                    try
                    {
                        this.Hide();
                        Form2 form2 = new Form2(Variables.URL + Variables.QUALITY);
                        form2.ShowDialog();
                        this.Close();
                    }
                    catch
                    {
                        /*Error playing video*/
                        MessageBox.Show("Error playing video", "Lustery Multitool", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    /*Invalid ID*/
                    MessageBox.Show("Not a valid video ID", "Lustery Multitool", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                /*Invalid URL*/
                MessageBox.Show("Invalid URL", "Lustery Multitool", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(linkLabel1.Text);
        }
    }
}