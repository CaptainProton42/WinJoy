namespace WinJoy_HidAPI
{
    partial class WinJoy
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WinJoy));
            this.buttonStart = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBoxL = new System.Windows.Forms.GroupBox();
            this.buttonCalL = new System.Windows.Forms.Button();
            this.labelLatL = new System.Windows.Forms.Label();
            this.labelBatteryL = new System.Windows.Forms.Label();
            this.labelNameL = new System.Windows.Forms.Label();
            this.pictureBoxL = new System.Windows.Forms.PictureBox();
            this.groupBoxR = new System.Windows.Forms.GroupBox();
            this.buttonCalR = new System.Windows.Forms.Button();
            this.labelLatR = new System.Windows.Forms.Label();
            this.pictureBoxR = new System.Windows.Forms.PictureBox();
            this.labelBatteryR = new System.Windows.Forms.Label();
            this.labelNameR = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.groupBoxL.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxL)).BeginInit();
            this.groupBoxR.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxR)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(428, 190);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(128, 59);
            this.buttonStart.TabIndex = 1;
            this.buttonStart.Text = "Start";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(347, 190);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 59);
            this.button2.TabIndex = 4;
            this.button2.Text = "Combine";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // groupBoxL
            // 
            this.groupBoxL.Controls.Add(this.buttonCalL);
            this.groupBoxL.Controls.Add(this.labelLatL);
            this.groupBoxL.Controls.Add(this.labelBatteryL);
            this.groupBoxL.Controls.Add(this.labelNameL);
            this.groupBoxL.Controls.Add(this.pictureBoxL);
            this.groupBoxL.Controls.Add(this.pictureBox2);
            this.groupBoxL.Location = new System.Drawing.Point(12, 12);
            this.groupBoxL.Name = "groupBoxL";
            this.groupBoxL.Size = new System.Drawing.Size(552, 83);
            this.groupBoxL.TabIndex = 5;
            this.groupBoxL.TabStop = false;
            // 
            // buttonCalL
            // 
            this.buttonCalL.Location = new System.Drawing.Point(482, 13);
            this.buttonCalL.Name = "buttonCalL";
            this.buttonCalL.Size = new System.Drawing.Size(64, 64);
            this.buttonCalL.TabIndex = 8;
            this.buttonCalL.Text = "Calibrate";
            this.buttonCalL.UseVisualStyleBackColor = true;
            this.buttonCalL.Click += new System.EventHandler(this.buttonCalL_Click);
            // 
            // labelLatL
            // 
            this.labelLatL.AutoSize = true;
            this.labelLatL.Location = new System.Drawing.Point(76, 64);
            this.labelLatL.Name = "labelLatL";
            this.labelLatL.Size = new System.Drawing.Size(50, 13);
            this.labelLatL.TabIndex = 10;
            this.labelLatL.Text = "labelLatL";
            // 
            // labelBatteryL
            // 
            this.labelBatteryL.AutoSize = true;
            this.labelBatteryL.Location = new System.Drawing.Point(76, 38);
            this.labelBatteryL.Name = "labelBatteryL";
            this.labelBatteryL.Size = new System.Drawing.Size(68, 13);
            this.labelBatteryL.TabIndex = 9;
            this.labelBatteryL.Text = "labelBatteryL";
            // 
            // labelNameL
            // 
            this.labelNameL.AutoSize = true;
            this.labelNameL.Location = new System.Drawing.Point(76, 13);
            this.labelNameL.Name = "labelNameL";
            this.labelNameL.Size = new System.Drawing.Size(63, 13);
            this.labelNameL.TabIndex = 8;
            this.labelNameL.Text = "labelNameL";
            // 
            // pictureBoxL
            // 
            this.pictureBoxL.Image = global::WinJoy_HidAPI.Properties.Resources.joycon_l;
            this.pictureBoxL.InitialImage = null;
            this.pictureBoxL.Location = new System.Drawing.Point(6, 13);
            this.pictureBoxL.Name = "pictureBoxL";
            this.pictureBoxL.Size = new System.Drawing.Size(33, 64);
            this.pictureBoxL.TabIndex = 2;
            this.pictureBoxL.TabStop = false;
            // 
            // groupBoxR
            // 
            this.groupBoxR.Controls.Add(this.pictureBox1);
            this.groupBoxR.Controls.Add(this.buttonCalR);
            this.groupBoxR.Controls.Add(this.labelLatR);
            this.groupBoxR.Controls.Add(this.pictureBoxR);
            this.groupBoxR.Controls.Add(this.labelBatteryR);
            this.groupBoxR.Controls.Add(this.labelNameR);
            this.groupBoxR.Location = new System.Drawing.Point(12, 101);
            this.groupBoxR.Name = "groupBoxR";
            this.groupBoxR.Size = new System.Drawing.Size(552, 83);
            this.groupBoxR.TabIndex = 6;
            this.groupBoxR.TabStop = false;
            // 
            // buttonCalR
            // 
            this.buttonCalR.Location = new System.Drawing.Point(482, 13);
            this.buttonCalR.Name = "buttonCalR";
            this.buttonCalR.Size = new System.Drawing.Size(64, 64);
            this.buttonCalR.TabIndex = 11;
            this.buttonCalR.Text = "Calibrate";
            this.buttonCalR.UseVisualStyleBackColor = true;
            this.buttonCalR.Click += new System.EventHandler(this.buttonCalR_Click);
            // 
            // labelLatR
            // 
            this.labelLatR.AutoSize = true;
            this.labelLatR.Location = new System.Drawing.Point(76, 64);
            this.labelLatR.Name = "labelLatR";
            this.labelLatR.Size = new System.Drawing.Size(52, 13);
            this.labelLatR.TabIndex = 13;
            this.labelLatR.Text = "labelLatR";
            // 
            // pictureBoxR
            // 
            this.pictureBoxR.Image = global::WinJoy_HidAPI.Properties.Resources.joycon_r;
            this.pictureBoxR.InitialImage = null;
            this.pictureBoxR.Location = new System.Drawing.Point(6, 13);
            this.pictureBoxR.Name = "pictureBoxR";
            this.pictureBoxR.Size = new System.Drawing.Size(64, 64);
            this.pictureBoxR.TabIndex = 1;
            this.pictureBoxR.TabStop = false;
            // 
            // labelBatteryR
            // 
            this.labelBatteryR.AutoSize = true;
            this.labelBatteryR.Location = new System.Drawing.Point(76, 38);
            this.labelBatteryR.Name = "labelBatteryR";
            this.labelBatteryR.Size = new System.Drawing.Size(70, 13);
            this.labelBatteryR.TabIndex = 12;
            this.labelBatteryR.Text = "labelBatteryR";
            // 
            // labelNameR
            // 
            this.labelNameR.AutoSize = true;
            this.labelNameR.Location = new System.Drawing.Point(76, 13);
            this.labelNameR.Name = "labelNameR";
            this.labelNameR.Size = new System.Drawing.Size(65, 13);
            this.labelNameR.TabIndex = 11;
            this.labelNameR.Text = "labelNameR";
            // 
            // pictureBox1
            // 
            this.pictureBox1.InitialImage = null;
            this.pictureBox1.Location = new System.Drawing.Point(6, 13);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(33, 64);
            this.pictureBox1.TabIndex = 12;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.pictureBox2.InitialImage = null;
            this.pictureBox2.Location = new System.Drawing.Point(6, 13);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(64, 64);
            this.pictureBox2.TabIndex = 14;
            this.pictureBox2.TabStop = false;
            // 
            // WinJoy
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(576, 258);
            this.Controls.Add(this.groupBoxR);
            this.Controls.Add(this.groupBoxL);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.buttonStart);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "WinJoy";
            this.Text = "WinJoy";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.WinJoy_FormClosed);
            this.Load += new System.EventHandler(this.WinJoy_Load);
            this.Shown += new System.EventHandler(this.WinJoy_Shown);
            this.groupBoxL.ResumeLayout(false);
            this.groupBoxL.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxL)).EndInit();
            this.groupBoxR.ResumeLayout(false);
            this.groupBoxR.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxR)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.GroupBox groupBoxL;
        private System.Windows.Forms.GroupBox groupBoxR;
        private System.Windows.Forms.PictureBox pictureBoxR;
        private System.Windows.Forms.Button buttonCalL;
        private System.Windows.Forms.Label labelLatL;
        private System.Windows.Forms.Label labelBatteryL;
        private System.Windows.Forms.Label labelNameL;
        private System.Windows.Forms.PictureBox pictureBoxL;
        private System.Windows.Forms.Button buttonCalR;
        private System.Windows.Forms.Label labelLatR;
        private System.Windows.Forms.Label labelBatteryR;
        private System.Windows.Forms.Label labelNameR;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}

