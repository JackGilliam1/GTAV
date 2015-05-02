using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using GTA.V.Formats.Save;
using GTA.V.Utils.Security;
using System.Security.Cryptography;
using Methods;
using GTA_Save_Block.Forms;
namespace GTA_Save_Block
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
            byte[] key = KeyUtils.GetKey(Application.StartupPath + "/" + Properties.Settings.Default.keyFileName);
            this.key = key;
            if (key == null)
            {
                MessageBox.Show("No key found", "Key - Not Found", MessageBoxButtons.OK);
                openButton.Enabled = false;
       
            }
            if (!File.Exists(Application.StartupPath + "/" + Properties.Settings.Default.keyFileName))
            {
                File.Create(Application.StartupPath + "/" + Properties.Settings.Default.keyFileName).Close();
                File.WriteAllText(Application.StartupPath + "/" + Properties.Settings.Default.keyFileName, "GTAV=");

                MessageBox.Show("key file generated", "File - Generated", MessageBoxButtons.OK);
                return;
            }
            if (key != null)
            {
                MD5 md = MD5.Create();
                byte[] hash = md.ComputeHash(key);
                string hashstr = BitConverter.ToString(hash).Replace("-", "").ToLower();
                if (hashstr != "e68d4cce4c80e77a4f98df9f02fad801")
                {
                    MessageBox.Show("Invalid Key", "Key - Invalid", MessageBoxButtons.OK);
                }
            }
            
        }

        #region Properties
        private Stream xMain;
        private V GTA;
        private byte[] key;
        private string fileName;
        #endregion

        #region Tool Strip Handlers
        private void infoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("GTA V Block Editor - Version 0.0.3\nDeveloped by XBLToothPik\n\nSpecial Thanks to:\nkill_seth\nJappi88", "Info - About", MessageBoxButtons.OK);
        }

        #endregion

        #region Buttons
        private void openButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog OFD = new OpenFileDialog();
            OFD.Filter = "All Files|*.*|Bin|*.bin";
            OFD.Title = "Open GTA V Save (X360)";
            if (OFD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.xMain = File.Open(OFD.FileName, FileMode.Open);
                this.GTA = new V(xMain, this.key, Properties.Settings.Default.autoDecrypt);
                this.fileName = Path.GetFileName(OFD.FileName);
                LoadForm();
            }
        }
        private void closeButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to close this file?", "Close - Confirm", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                CloseForm();
        }
        private void saveAButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog SFD = new SaveFileDialog();
            SFD.Title = "Choose output file...";
            SFD.Filter = "|*.*";
            if (SFD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Stream xOut = File.Create(SFD.FileName);
                this.GTA.Write(xOut, Properties.Settings.Default.autoDecrypt);
                xOut.Close();
                MessageBox.Show("File created, saved & rebuilt!", "File Save - Success", MessageBoxButtons.OK);

            }
        }
        #endregion

        #region Helpers
        private void LoadForm()
        {
            for (int i = 0; i < GTA.Body.Entries.Count; i++)
            {
                ListViewItem LVI = new ListViewItem();
                LVI.Text = GTA.Body.Entries[i].Name;
                LVI.SubItems.Add(GTA.Body.Entries[i].DataLen.ToString());
                LVI.SubItems.Add(GTA.Body.Entries[i].DataOffset.ToString());

                fileView.Items.Add(LVI);
            }
            closeButton.Enabled = true;
            openButton.Enabled = true;
            mainPanel.Enabled = true;
            saveAButton.Enabled = true;
            saveButton.Enabled = true;
            fileLabel.Text = "File: " + this.fileName;

        }
        private void CloseForm()
        {
            closeButton.Enabled = false;
            saveAButton.Enabled = false;
            saveButton.Enabled = false;
            mainPanel.Enabled = false;
            mainPanel.Enabled = false;
            fileLabel.Text = "File: N/A";
            xMain.Close();
            if (GTA.Body.Entries.Count > 0)
                for (int i = 0; i < GTA.Body.Entries.Count; i++)
                    if (GTA.Body.Entries[i].CustomStream != null)
                        GTA.Body.Entries[i].CustomStream.Close();
            fileView.Items.Clear();
        }

        private void ReloadNewSettings()
        {
            byte[] key = KeyUtils.GetKey(Application.StartupPath + "/" + Properties.Settings.Default.keyFileName);
            this.key = key;
        }
        #endregion

        #region ListView Stuff
        private void extractrToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Int32 xInt in this.fileView.SelectedIndices)
            {
                if (fileView.Items[xInt].Text != "...")
                {
                    SaveFileDialog SFD = new SaveFileDialog();
                    SFD.Title = "Choose Output...";
                    SFD.FileName = fileView.Items[xInt].Text;
                    if (SFD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        Stream xOut = File.Create(SFD.FileName);
                        GTA.GetEntryByName(fileView.Items[xInt].Text).ExtractToStream(xOut);
                        xOut.Close();
                        MessageBox.Show("File Extracted!", "Extraction - Success", MessageBoxButtons.OK);
                    }
                }
            }
        }
        private void replaceButton_Click(object sender, EventArgs e)
        {
            foreach (Int32 xInt in this.fileView.SelectedIndices)
            {
                if (fileView.Items[xInt].Text != "...")
                {
                    if (replaceButton.Text == "Un-Replace")
                    {
                        GTA.GetEntryByName(fileView.Items[xInt].Text).CustomStream = null;
                        if (!fileView.VirtualMode)
                        {
                            fileView.Items[xInt].BackColor = SystemColors.Window;
                            fileView.Items[xInt].SubItems[1].Text = GTA.GetEntryByName(fileView.Items[xInt].Text).DataLen.ToString();

                        }
                    }
                    else
                    {
                        OpenFileDialog OFD = new OpenFileDialog();
                        OFD.Filter = "|*.*";
                        OFD.FileName = fileView.Items[xInt].Text;
                        OFD.Title = string.Format("Choose file to replace {0} with...", fileView.Items[xInt].Text);
                        if (OFD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            Stream xIn = File.Open(OFD.FileName, FileMode.Open);
                            GTA.GetEntryByName(fileView.Items[xInt].Text).CustomStream = xIn;
                            fileView.Items[xInt].BackColor = Color.OrangeRed;
                            if (!fileView.VirtualMode)
                                fileView.Items[xInt].SubItems[1].Text = xIn.Length.ToString();
                        }
                    }
                }
            }
        }
        #endregion

        private void saveButton_Click(object sender, EventArgs e)
        {
            string tempFilePath = Application.StartupPath + "/" + "temp.sav";
            Stream tempStream = File.Create(tempFilePath);
            this.GTA.Write(tempStream, Properties.Settings.Default.autoDecrypt);
            xMain.SetLength(0);
            tempStream.Position = 0;
            StreamUtils.CopyStream(tempStream, this.xMain);

            tempStream.Close();
            File.Delete(tempFilePath);

            MessageBox.Show("File saved to original file!", "File Save - Success", MessageBoxButtons.OK);
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings settings = new Settings();
            settings.ShowDialog();
            ReloadNewSettings();

            if (key != null)
            {
                MD5 md = MD5.Create();
                byte[] hash = md.ComputeHash(key);
                string hashstr = BitConverter.ToString(hash).Replace("-", "").ToLower();
                if (hashstr != "e68d4cce4c80e77a4f98df9f02fad801")
                {
                    openButton.Enabled = false;
                    MessageBox.Show("New key is invalid", "Key - Invalid", MessageBoxButtons.OK);
                }
                else
                {

                    openButton.Enabled = true;
                }
            }
            else
            {
                MessageBox.Show("No key found", "Key - Not Found", MessageBoxButtons.OK);
                openButton.Enabled = false;
            }
           
        }
    }
}
