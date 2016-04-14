namespace CV_Evaluator.PeakPicking
{
    partial class frmPeakPickingSetup
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.numSteepness = new jwGraph.GeneralTools.NumericInputbox();
            this.label1 = new System.Windows.Forms.Label();
            this.numMinHeight = new jwGraph.GeneralTools.NumericInputbox();
            this.label2 = new System.Windows.Forms.Label();
            this.numWindow = new jwGraph.GeneralTools.NumericInputbox();
            this.label4 = new System.Windows.Forms.Label();
            this.numStdLimit = new jwGraph.GeneralTools.NumericInputbox();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.numSteepness, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.numMinHeight, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.numWindow, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.numStdLimit, 1, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(5, 5);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(296, 186);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 72);
            this.label3.Margin = new System.Windows.Forms.Padding(3, 10, 3, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(198, 26);
            this.label3.TabIndex = 4;
            this.label3.Text = "Window Size [V] (Ssearch width of local extremum)";
            // 
            // numSteepness
            // 
            this.numSteepness.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.numSteepness.ChangeBackcolor = false;
            this.numSteepness.ChangeForecolor = true;
            this.numSteepness.ForeColor = System.Drawing.Color.Green;
            this.numSteepness.FormatString = "G";
            this.numSteepness.InvalidBackColor = System.Drawing.Color.PaleVioletRed;
            this.numSteepness.InvalidForeColor = System.Drawing.Color.Red;
            this.numSteepness.Location = new System.Drawing.Point(237, 34);
            this.numSteepness.Maximum = 1D;
            this.numSteepness.Minimum = 0D;
            this.numSteepness.Name = "numSteepness";
            this.numSteepness.Size = new System.Drawing.Size(56, 20);
            this.numSteepness.TabIndex = 3;
            this.numSteepness.Text = "0.1";
            this.numSteepness.ValidBackColor = System.Drawing.Color.White;
            this.numSteepness.ValidForeColor = System.Drawing.Color.Green;
            this.numSteepness.Value = 0.1D;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 10);
            this.label1.Margin = new System.Windows.Forms.Padding(3, 10, 3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(184, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Minimum Peak Height (% of Min/Max)";
            // 
            // numMinHeight
            // 
            this.numMinHeight.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.numMinHeight.ChangeBackcolor = false;
            this.numMinHeight.ChangeForecolor = true;
            this.numMinHeight.ForeColor = System.Drawing.Color.Green;
            this.numMinHeight.FormatString = "G";
            this.numMinHeight.InvalidBackColor = System.Drawing.Color.PaleVioletRed;
            this.numMinHeight.InvalidForeColor = System.Drawing.Color.Red;
            this.numMinHeight.Location = new System.Drawing.Point(237, 3);
            this.numMinHeight.Maximum = 1D;
            this.numMinHeight.Minimum = 0D;
            this.numMinHeight.Name = "numMinHeight";
            this.numMinHeight.Size = new System.Drawing.Size(56, 20);
            this.numMinHeight.TabIndex = 1;
            this.numMinHeight.Text = "0.2";
            this.numMinHeight.ValidBackColor = System.Drawing.Color.White;
            this.numMinHeight.ValidForeColor = System.Drawing.Color.Green;
            this.numMinHeight.Value = 0.2D;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 36);
            this.label2.Margin = new System.Windows.Forms.Padding(3, 10, 3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(222, 26);
            this.label2.TabIndex = 2;
            this.label2.Text = "Minimum Steepness (%, use higher to discard broad peaks)";
            // 
            // numWindow
            // 
            this.numWindow.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.numWindow.ChangeBackcolor = false;
            this.numWindow.ChangeForecolor = true;
            this.numWindow.ForeColor = System.Drawing.Color.Green;
            this.numWindow.FormatString = "G";
            this.numWindow.InvalidBackColor = System.Drawing.Color.PaleVioletRed;
            this.numWindow.InvalidForeColor = System.Drawing.Color.Red;
            this.numWindow.Location = new System.Drawing.Point(237, 70);
            this.numWindow.Maximum = 100D;
            this.numWindow.Minimum = 0.001D;
            this.numWindow.Name = "numWindow";
            this.numWindow.Size = new System.Drawing.Size(56, 20);
            this.numWindow.TabIndex = 5;
            this.numWindow.Text = "0.075";
            this.numWindow.ValidBackColor = System.Drawing.Color.White;
            this.numWindow.ValidForeColor = System.Drawing.Color.Green;
            this.numWindow.Value = 0.075D;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 108);
            this.label4.Margin = new System.Windows.Forms.Padding(3, 10, 3, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(202, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Baseline Search Standard Deviation Limit";
            // 
            // numStdLimit
            // 
            this.numStdLimit.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.numStdLimit.ChangeBackcolor = false;
            this.numStdLimit.ChangeForecolor = true;
            this.numStdLimit.ForeColor = System.Drawing.Color.Green;
            this.numStdLimit.FormatString = "G";
            this.numStdLimit.InvalidBackColor = System.Drawing.Color.PaleVioletRed;
            this.numStdLimit.InvalidForeColor = System.Drawing.Color.Red;
            this.numStdLimit.Location = new System.Drawing.Point(237, 101);
            this.numStdLimit.Maximum = 1D;
            this.numStdLimit.Minimum = 1E-20D;
            this.numStdLimit.Name = "numStdLimit";
            this.numStdLimit.Size = new System.Drawing.Size(56, 20);
            this.numStdLimit.TabIndex = 7;
            this.numStdLimit.Text = "5E-09";
            this.numStdLimit.ValidBackColor = System.Drawing.Color.White;
            this.numStdLimit.ValidForeColor = System.Drawing.Color.Green;
            this.numStdLimit.Value = 5E-09D;
            // 
            // frmPeakPickingSetup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(306, 196);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "frmPeakPickingSetup";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.ShowInTaskbar = false;
            this.Text = "Peak Picking Setup";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmPeakPickingSetup_FormClosing);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private jwGraph.GeneralTools.NumericInputbox numMinHeight;
        private System.Windows.Forms.Label label2;
        private jwGraph.GeneralTools.NumericInputbox numSteepness;
        private System.Windows.Forms.Label label3;
        private jwGraph.GeneralTools.NumericInputbox numWindow;
        private System.Windows.Forms.Label label4;
        private jwGraph.GeneralTools.NumericInputbox numStdLimit;
    }
}