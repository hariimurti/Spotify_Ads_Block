﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Harimurti;
using System.Net;

namespace Spotify_Ads_Block
{
    public partial class MainForm : Form
    {
        private static string GITHUB_ADSLIST = "https://raw.githubusercontent.com/hariimurti/Spotify_Ads_Block/master/hosts.txt";
        private static string FILE_ADSLIST = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "hosts.txt");

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            if (!AdminPrivilege.isElevated())
            {
                groupBox1.Enabled = false;
                MessageBox.Show("This application needs admin previlege to modify hosts.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            if (Preferences.isExist())
            {
                textBox1.Text = Preferences.Get_StorageSize();
            }
            else
            {
                groupBox2.Enabled = false;
            }

            try
            {
                using (var client = new WebClient())
                {
                    client.DownloadFile(GITHUB_ADSLIST, FILE_ADSLIST);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Something went wrong...", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (File.Exists(FILE_ADSLIST))
            {
                try
                {
                    List<string> adslist = new List<string>();
                    foreach (string line in File.ReadAllLines(FILE_ADSLIST))
                    {
                        if (!line.StartsWith("#") && (line != string.Empty))
                            adslist.Add(line);
                    }
                    Hosts.Block(adslist);
                    MessageBox.Show("Successfully block ads!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Something went wrong...", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("AdsList (hosts.txt) not found!", "Something went wrong...", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                Hosts.Reset();
                MessageBox.Show("Successfully reset hosts!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Something went wrong...", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (Preferences.Set_StorageSize(textBox1.Text))
            {
                MessageBox.Show($"Successfully changed cache size to {textBox1.Text}MB!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            else
            {
                MessageBox.Show("File prefs not found!", "Something went wrong...", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
