namespace CV_Evaluator
{
    partial class frmInterpolateSettings
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
            this.label1 = new System.Windows.Forms.Label();
            this.numdt = new System.Windows.Forms.NumericUpDown();
            this.cbSettZero = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.numSkips = new System.Windows.Forms.NumericUpDown();
            this.cbFromScanrate = new System.Windows.Forms.CheckBox();
            this.lblStats = new System.Windows.Forms.Label();
            this.cbInPlace = new System.Windows.Forms.CheckBox();
            this.cbAllCycles = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.numdt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSkips)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Delta t:";
            // 
            // numdt
            // 
            this.numdt.DecimalPlaces = 15;
            this.numdt.Increment = new decimal(new int[] {
            1,
            0,
            0,
            262144});
            this.numdt.Location = new System.Drawing.Point(59, 23);
            this.numdt.Maximum = new decimal(new int[] {
            -1530494976,
            232830,
            0,
            0});
            this.numdt.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            983040});
            this.numdt.Name = "numdt";
            this.numdt.Size = new System.Drawing.Size(130, 20);
            this.numdt.TabIndex = 1;
            this.numdt.Value = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.numdt.ValueChanged += new System.EventHandler(this.numdt_ValueChanged);
            // 
            // cbSettZero
            // 
            this.cbSettZero.AutoSize = true;
            this.cbSettZero.Location = new System.Drawing.Point(15, 49);
            this.cbSettZero.Name = "cbSettZero";
            this.cbSettZero.Size = new System.Drawing.Size(93, 17);
            this.cbSettZero.TabIndex = 2;
            this.cbSettZero.Text = "Set t_min=0s?";
            this.cbSettZero.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(58, 185);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(140, 185);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 69);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Skips:";
            // 
            // numSkips
            // 
            this.numSkips.Location = new System.Drawing.Point(59, 67);
            this.numSkips.Name = "numSkips";
            this.numSkips.Size = new System.Drawing.Size(217, 20);
            this.numSkips.TabIndex = 6;
            // 
            // cbFromScanrate
            // 
            this.cbFromScanrate.AutoSize = true;
            this.cbFromScanrate.Location = new System.Drawing.Point(195, 24);
            this.cbFromScanrate.Name = "cbFromScanrate";
            this.cbFromScanrate.Size = new System.Drawing.Size(95, 17);
            this.cbFromScanrate.TabIndex = 7;
            this.cbFromScanrate.Text = "From Scanrate";
            this.cbFromScanrate.UseVisualStyleBackColor = true;
            this.cbFromScanrate.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // lblStats
            // 
            this.lblStats.AutoSize = true;
            this.lblStats.Location = new System.Drawing.Point(12, 150);
            this.lblStats.Name = "lblStats";
            this.lblStats.Size = new System.Drawing.Size(27, 13);
            this.lblStats.TabIndex = 8;
            this.lblStats.Text = "asdf";
            // 
            // cbInPlace
            // 
            this.cbInPlace.AutoSize = true;
            this.cbInPlace.Location = new System.Drawing.Point(15, 93);
            this.cbInPlace.Name = "cbInPlace";
            this.cbInPlace.Size = new System.Drawing.Size(130, 17);
            this.cbInPlace.TabIndex = 9;
            this.cbInPlace.Text = "Replace data in place";
            this.cbInPlace.UseVisualStyleBackColor = true;
            // 
            // cbAllCycles
            // 
            this.cbAllCycles.AutoSize = true;
            this.cbAllCycles.Location = new System.Drawing.Point(15, 116);
            this.cbAllCycles.Name = "cbAllCycles";
            this.cbAllCycles.Size = new System.Drawing.Size(71, 17);
            this.cbAllCycles.TabIndex = 10;
            this.cbAllCycles.Text = "All Cycles";
            this.cbAllCycles.UseVisualStyleBackColor = true;
            // 
            // frmInterpolateSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(288, 224);
            this.Controls.Add(this.cbAllCycles);
            this.Controls.Add(this.cbInPlace);
            this.Controls.Add(this.lblStats);
            this.Controls.Add(this.cbFromScanrate);
            this.Controls.Add(this.numSkips);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.cbSettZero);
            this.Controls.Add(this.numdt);
            this.Controls.Add(this.label1);
            this.Name = "frmInterpolateSettings";
            this.Text = "Interpolate Settings";
            this.Load += new System.EventHandler(this.frmInterpolateSettings_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numdt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSkips)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numdt;
        private System.Windows.Forms.CheckBox cbSettZero;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numSkips;
        private System.Windows.Forms.CheckBox cbFromScanrate;
        private System.Windows.Forms.Label lblStats;
        private System.Windows.Forms.CheckBox cbInPlace;
        private System.Windows.Forms.CheckBox cbAllCycles;
    }
}