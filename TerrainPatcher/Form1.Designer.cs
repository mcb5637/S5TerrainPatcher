namespace TerrainPatcher
{
    partial class Form1
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.btnChooseFileSrc = new System.Windows.Forms.Button();
            this.tbSrcMap = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnRead = new System.Windows.Forms.Button();
            this.pnlRegions = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.cbTextures = new System.Windows.Forms.CheckBox();
            this.cbHeights = new System.Windows.Forms.CheckBox();
            this.cbEntities = new System.Windows.Forms.CheckBox();
            this.cbVertexColors = new System.Windows.Forms.CheckBox();
            this.cbWaterTypes = new System.Windows.Forms.CheckBox();
            this.cbWaterLevels = new System.Windows.Forms.CheckBox();
            this.btnGetLuaFile = new System.Windows.Forms.Button();
            this.btnHelp = new System.Windows.Forms.Button();
            this.pnlRegions.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnChooseFileSrc
            // 
            this.btnChooseFileSrc.Location = new System.Drawing.Point(255, 16);
            this.btnChooseFileSrc.Name = "btnChooseFileSrc";
            this.btnChooseFileSrc.Size = new System.Drawing.Size(74, 22);
            this.btnChooseFileSrc.TabIndex = 0;
            this.btnChooseFileSrc.Tag = "";
            this.btnChooseFileSrc.Text = ".s5x wählen";
            this.btnChooseFileSrc.UseVisualStyleBackColor = true;
            this.btnChooseFileSrc.Click += new System.EventHandler(this.btnChooseFile_Click);
            // 
            // tbSrcMap
            // 
            this.tbSrcMap.Location = new System.Drawing.Point(89, 18);
            this.tbSrcMap.Name = "tbSrcMap";
            this.tbSrcMap.Size = new System.Drawing.Size(160, 20);
            this.tbSrcMap.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Patch-Map:";
            // 
            // btnRead
            // 
            this.btnRead.Location = new System.Drawing.Point(12, 53);
            this.btnRead.Name = "btnRead";
            this.btnRead.Size = new System.Drawing.Size(323, 25);
            this.btnRead.TabIndex = 6;
            this.btnRead.Text = "Einlesen";
            this.btnRead.UseVisualStyleBackColor = true;
            this.btnRead.Click += new System.EventHandler(this.btnRead_Click);
            // 
            // pnlRegions
            // 
            this.pnlRegions.AutoScroll = true;
            this.pnlRegions.Controls.Add(this.label3);
            this.pnlRegions.Location = new System.Drawing.Point(12, 84);
            this.pnlRegions.Name = "pnlRegions";
            this.pnlRegions.Size = new System.Drawing.Size(323, 159);
            this.pnlRegions.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 11);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(81, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Nichts geladen!";
            // 
            // cbTextures
            // 
            this.cbTextures.AutoSize = true;
            this.cbTextures.Checked = true;
            this.cbTextures.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbTextures.Location = new System.Drawing.Point(12, 249);
            this.cbTextures.Name = "cbTextures";
            this.cbTextures.Size = new System.Drawing.Size(68, 17);
            this.cbTextures.TabIndex = 8;
            this.cbTextures.Text = "Texturen";
            this.cbTextures.UseVisualStyleBackColor = true;
            // 
            // cbHeights
            // 
            this.cbHeights.AutoSize = true;
            this.cbHeights.Checked = true;
            this.cbHeights.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbHeights.Location = new System.Drawing.Point(139, 249);
            this.cbHeights.Name = "cbHeights";
            this.cbHeights.Size = new System.Drawing.Size(58, 17);
            this.cbHeights.TabIndex = 9;
            this.cbHeights.Text = "Höhen";
            this.cbHeights.UseVisualStyleBackColor = true;
            // 
            // cbEntities
            // 
            this.cbEntities.AutoSize = true;
            this.cbEntities.Checked = true;
            this.cbEntities.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbEntities.Location = new System.Drawing.Point(261, 249);
            this.cbEntities.Name = "cbEntities";
            this.cbEntities.Size = new System.Drawing.Size(68, 17);
            this.cbEntities.TabIndex = 10;
            this.cbEntities.Text = "Entitäten";
            this.cbEntities.UseVisualStyleBackColor = true;
            // 
            // cbVertexColors
            // 
            this.cbVertexColors.AutoSize = true;
            this.cbVertexColors.Checked = true;
            this.cbVertexColors.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbVertexColors.Location = new System.Drawing.Point(12, 272);
            this.cbVertexColors.Name = "cbVertexColors";
            this.cbVertexColors.Size = new System.Drawing.Size(92, 17);
            this.cbVertexColors.TabIndex = 11;
            this.cbVertexColors.Text = "Vertex Farben";
            this.cbVertexColors.UseVisualStyleBackColor = true;
            // 
            // cbWaterTypes
            // 
            this.cbWaterTypes.AutoSize = true;
            this.cbWaterTypes.Checked = true;
            this.cbWaterTypes.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbWaterTypes.Location = new System.Drawing.Point(261, 272);
            this.cbWaterTypes.Name = "cbWaterTypes";
            this.cbWaterTypes.Size = new System.Drawing.Size(88, 17);
            this.cbWaterTypes.TabIndex = 12;
            this.cbWaterTypes.Text = "Wassertypen";
            this.cbWaterTypes.UseVisualStyleBackColor = true;
            // 
            // cbWaterLevels
            // 
            this.cbWaterLevels.AutoSize = true;
            this.cbWaterLevels.Checked = true;
            this.cbWaterLevels.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbWaterLevels.Location = new System.Drawing.Point(139, 272);
            this.cbWaterLevels.Name = "cbWaterLevels";
            this.cbWaterLevels.Size = new System.Drawing.Size(91, 17);
            this.cbWaterLevels.TabIndex = 13;
            this.cbWaterLevels.Text = "Wasser Level";
            this.cbWaterLevels.UseVisualStyleBackColor = true;
            // 
            // btnGetLuaFile
            // 
            this.btnGetLuaFile.Location = new System.Drawing.Point(12, 296);
            this.btnGetLuaFile.Name = "btnGetLuaFile";
            this.btnGetLuaFile.Size = new System.Drawing.Size(177, 27);
            this.btnGetLuaFile.TabIndex = 14;
            this.btnGetLuaFile.Text = "Comfort-Skript abspeichern";
            this.btnGetLuaFile.UseVisualStyleBackColor = true;
            this.btnGetLuaFile.Click += new System.EventHandler(this.btnGetLuaFile_Click);
            // 
            // btnHelp
            // 
            this.btnHelp.Location = new System.Drawing.Point(195, 296);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(140, 27);
            this.btnHelp.TabIndex = 15;
            this.btnHelp.Text = "Info";
            this.btnHelp.UseVisualStyleBackColor = true;
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // Form1
            // 
            this.AcceptButton = this.btnRead;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(347, 335);
            this.Controls.Add(this.btnHelp);
            this.Controls.Add(this.btnGetLuaFile);
            this.Controls.Add(this.cbWaterLevels);
            this.Controls.Add(this.cbWaterTypes);
            this.Controls.Add(this.cbVertexColors);
            this.Controls.Add(this.cbEntities);
            this.Controls.Add(this.cbHeights);
            this.Controls.Add(this.cbTextures);
            this.Controls.Add(this.pnlRegions);
            this.Controls.Add(this.btnRead);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbSrcMap);
            this.Controls.Add(this.btnChooseFileSrc);
            this.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(363, 374);
            this.MinimumSize = new System.Drawing.Size(363, 374);
            this.Name = "Form1";
            this.Text = "TerrainPatcher";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.pnlRegions.ResumeLayout(false);
            this.pnlRegions.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnChooseFileSrc;
        private System.Windows.Forms.TextBox tbSrcMap;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnRead;
        private System.Windows.Forms.Panel pnlRegions;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox cbTextures;
        private System.Windows.Forms.CheckBox cbHeights;
        private System.Windows.Forms.CheckBox cbEntities;
        private System.Windows.Forms.CheckBox cbVertexColors;
        private System.Windows.Forms.CheckBox cbWaterTypes;
        private System.Windows.Forms.CheckBox cbWaterLevels;
        private System.Windows.Forms.Button btnGetLuaFile;
        private System.Windows.Forms.Button btnHelp;
    }
}

