using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading;
using System.Windows.Forms;

namespace TerrainPatcher
{
    public partial class Form1 : Form
    {
        private MainIO main;

        public Form1()
        {
            InitializeComponent();

            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            btnChooseFileSrc.Tag = tbSrcMap;

            main = new MainIO(false);
        }
        
        private void btnChooseFile_Click(object sender, EventArgs e)
        {
            TextBox outTb = (sender as Button).Tag as TextBox;
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Siedler 5 Maps (*.s5x)|*.s5x|Alle Dateien (*.*)|*.*";
            if (ofd.ShowDialog() == DialogResult.OK)
                outTb.Text = ofd.FileName;
        }

        private void btnRead_Click(object sender, EventArgs e)
        {
            if (!File.Exists(tbSrcMap.Text))
            {
                MessageBox.Show("Datei nicht gefunden!");
                return;
            }
            
            main.readMapFile(tbSrcMap.Text);

            pnlRegions.Controls.Clear();
            if (main.getRegions().Count == 0)
            {
                Label lblRegion = new Label();
                lblRegion.AutoSize = true;
                lblRegion.Text = "Keine Regionen gefunden!";
                lblRegion.Location = new Point(12, 11);
                pnlRegions.Controls.Add(lblRegion);
                return;
            }
            int num = 0;
            foreach (KeyValuePair<string, PatchRegion> kvp in main.getRegions())
            {
                Label lblRegion = new Label();
                lblRegion.AutoSize = true;
                lblRegion.Text = kvp.Key;
                lblRegion.Location = new Point(12, 11 + 25 * num);
                Button btnSave = new Button();
                btnSave.Text = "Speichern";
                btnSave.Tag = kvp.Key;
                btnSave.Click += btnSave_Click;
                btnSave.Location = new Point(220, 6 + 25 * num);
                pnlRegions.Controls.Add(lblRegion);
                pnlRegions.Controls.Add(btnSave);

                num++;
            }
        }
        
        void btnSave_Click(object sender, EventArgs e)
        {
            string prKey = (sender as Button).Tag as string;

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Lua Dateien (*.lua)|*.lua|Alle Dateien (*.*)|*.*";
            sfd.FileName = prKey.ToLower() + ".lua";
            if (sfd.ShowDialog() != DialogResult.OK)
                return;

            
            bool entities = cbEntities.Checked, textures = cbTextures.Checked, heights = cbHeights.Checked, vertexColors = cbVertexColors.Checked, waterTypes = cbWaterTypes.Checked, waterLevels = cbWaterLevels.Checked;

            main.createRegionPatchFile(prKey, sfd.FileName, prKey, entities, textures, heights, vertexColors, waterTypes, waterLevels);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            main.close();
        }

        private void btnGetLuaFile_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Lua Dateien (*.lua)|*.lua|Alle Dateien (*.*)|*.*";
            sfd.FileName = "terrainpatcher.lua";
            if (sfd.ShowDialog() != DialogResult.OK)
                return;
            Assembly localAssembly = Assembly.GetExecutingAssembly();

            try
            {
                Stream resourceStream = localAssembly.GetManifestResourceStream("TerrainPatcher.Comfort.terrainpatcher.lua");
                using (FileStream fs = new FileStream(sfd.FileName, FileMode.Create))
                {
                    resourceStream.CopyTo(fs);
                }
            }
            catch { }
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo(main.getTmpPath() + "/readme.txt"));
        }




    }


    public static class MyExtensions
    {
        // Only useful before .NET 4
        public static void CopyTo(this Stream input, Stream output)
        {
            byte[] buffer = new byte[16 * 1024]; // Fairly arbitrary size
            int bytesRead;

            while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, bytesRead);
            }
        }
    }
}
