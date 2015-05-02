using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GTA_Save_Block.Forms
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
            LoadSettings();
        }

        private void label3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("If you are going to be using this tool for gamesaves, then leave this\noption checked.  Otherwise if you are going to use it for the xmt file\nin the RPF files, uncheck it", "Info - Auto Crypt", MessageBoxButtons.OK);
        }

        private void SaveSettings()
        {
            Properties.Settings.Default.autoDecrypt = autoDecryptBox.Checked;
            Properties.Settings.Default.keyFileName = x360Box.Text;
            Properties.Settings.Default.Save();
        }
        private void LoadSettings()
        {
            this.autoDecryptBox.Checked = Properties.Settings.Default.autoDecrypt;
            this.x360Box.Text = Properties.Settings.Default.keyFileName;
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            SaveSettings();
        }

   

    }
}
