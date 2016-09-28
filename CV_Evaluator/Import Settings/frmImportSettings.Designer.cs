namespace CV_Evaluator.Import_Settings
{
    partial class frmImportSettings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pgSettings = new System.Windows.Forms.PropertyGrid();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnDefault = new System.Windows.Forms.Button();
            this.btnComsol = new System.Windows.Forms.Button();
            this.btnBiologicSingle = new System.Windows.Forms.Button();
            this.btnBiologicMulti = new System.Windows.Forms.Button();
            this.btnZahner = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pgSettings
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.pgSettings, 2);
            this.pgSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgSettings.HelpVisible = false;
            this.pgSettings.Location = new System.Drawing.Point(3, 3);
            this.pgSettings.Name = "pgSettings";
            this.pgSettings.Size = new System.Drawing.Size(468, 311);
            this.pgSettings.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.pgSettings, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnOK, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.btnCancel, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 2, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(10, 10);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(713, 353);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(159, 327);
            this.btnOK.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(240, 327);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.btnDefault);
            this.flowLayoutPanel1.Controls.Add(this.btnComsol);
            this.flowLayoutPanel1.Controls.Add(this.btnBiologicSingle);
            this.flowLayoutPanel1.Controls.Add(this.btnBiologicMulti);
            this.flowLayoutPanel1.Controls.Add(this.btnZahner);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(477, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.tableLayoutPanel1.SetRowSpan(this.flowLayoutPanel1, 3);
            this.flowLayoutPanel1.Size = new System.Drawing.Size(233, 347);
            this.flowLayoutPanel1.TabIndex = 5;
            // 
            // btnDefault
            // 
            this.btnDefault.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDefault.Location = new System.Drawing.Point(3, 3);
            this.btnDefault.Name = "btnDefault";
            this.btnDefault.Size = new System.Drawing.Size(75, 23);
            this.btnDefault.TabIndex = 3;
            this.btnDefault.Text = "Default";
            this.btnDefault.UseVisualStyleBackColor = true;
            this.btnDefault.Click += new System.EventHandler(this.btnDefault_Click);
            // 
            // btnComsol
            // 
            this.btnComsol.Location = new System.Drawing.Point(84, 3);
            this.btnComsol.Name = "btnComsol";
            this.btnComsol.Size = new System.Drawing.Size(75, 23);
            this.btnComsol.TabIndex = 4;
            this.btnComsol.Text = "Comsol";
            this.btnComsol.UseVisualStyleBackColor = true;
            this.btnComsol.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnBiologicSingle
            // 
            this.btnBiologicSingle.Location = new System.Drawing.Point(3, 32);
            this.btnBiologicSingle.Name = "btnBiologicSingle";
            this.btnBiologicSingle.Size = new System.Drawing.Size(99, 23);
            this.btnBiologicSingle.TabIndex = 5;
            this.btnBiologicSingle.Text = "BiologicSingle";
            this.btnBiologicSingle.UseVisualStyleBackColor = true;
            this.btnBiologicSingle.Click += new System.EventHandler(this.btnBiologicSingle_Click);
            // 
            // btnBiologicMulti
            // 
            this.btnBiologicMulti.Location = new System.Drawing.Point(108, 32);
            this.btnBiologicMulti.Name = "btnBiologicMulti";
            this.btnBiologicMulti.Size = new System.Drawing.Size(99, 23);
            this.btnBiologicMulti.TabIndex = 6;
            this.btnBiologicMulti.Text = "Biologic Multi";
            this.btnBiologicMulti.UseVisualStyleBackColor = true;
            this.btnBiologicMulti.Click += new System.EventHandler(this.btnBiologicMulti_Click);
            // 
            // btnZahner
            // 
            this.btnZahner.Location = new System.Drawing.Point(3, 61);
            this.btnZahner.Name = "btnZahner";
            this.btnZahner.Size = new System.Drawing.Size(99, 23);
            this.btnZahner.TabIndex = 7;
            this.btnZahner.Text = "Zahner";
            this.btnZahner.UseVisualStyleBackColor = true;
            this.btnZahner.Click += new System.EventHandler(this.btnZahner_Click);
            // 
            // frmImportSettings
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(733, 373);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "frmImportSettings";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.Text = "Import Setup";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PropertyGrid pgSettings;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnDefault;
        private System.Windows.Forms.Button btnComsol;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button btnBiologicSingle;
        private System.Windows.Forms.Button btnBiologicMulti;
        private System.Windows.Forms.Button btnZahner;
    }
}