namespace CV_Evaluator.RandlesSevchik
{
    partial class frmRandlesSevchikResults
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
            this.components = new System.ComponentModel.Container();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.jwgResults = new jwGraph.jwGraph.jwGraph();
            this.tbResults = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(10, 10);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.jwgResults);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tbResults);
            this.splitContainer1.Size = new System.Drawing.Size(716, 442);
            this.splitContainer1.SplitterDistance = 326;
            this.splitContainer1.TabIndex = 0;
            // 
            // jwgResults
            // 
            this.jwgResults.AutoScaleBorder = 0.1D;
            this.jwgResults.BottomRightColor = System.Drawing.Color.LightSteelBlue;
            this.jwgResults.CenterImage = null;
            this.jwgResults.CenterImageMaxSize = new System.Drawing.Size(0, 0);
            this.jwgResults.CenterImageMinSize = new System.Drawing.Size(0, 0);
            this.jwgResults.Cursor = System.Windows.Forms.Cursors.Cross;
            this.jwgResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.jwgResults.EnableAutoscaling = true;
            this.jwgResults.EnableGraphObjects = true;
            this.jwgResults.EnableLegend = true;
            this.jwgResults.EnableMarkers = true;
            this.jwgResults.EraserVisible = false;
            this.jwgResults.FreeMarkerCount = 0;
            this.jwgResults.GraphBackColor = System.Drawing.Color.GhostWhite;
            this.jwgResults.GraphBorder = new System.Windows.Forms.Padding(100, 20, 20, 70);
            this.jwgResults.HighQuality = true;
            this.jwgResults.HorizontalMarkerCount = 0;
            this.jwgResults.IncludeMarkersInScaling = true;
            this.jwgResults.LeftMouseAction = jwGraph.jwGraph.jwGraph.enLeftMouseAction.ZoomIn;
            this.jwgResults.LeftMouseFunctionalityEnabled = true;
            this.jwgResults.LegendAlwaysVisible = false;
            this.jwgResults.LegendPosition = jwGraph.jwGraph.jwGraph.enumLegendPosition.TopRight;
            this.jwgResults.LegendTitle = null;
            this.jwgResults.Location = new System.Drawing.Point(0, 0);
            this.jwgResults.Message = "";
            this.jwgResults.MessageColor = System.Drawing.Color.Black;
            this.jwgResults.MiddleMouseFunctionalityEnabled = true;
            this.jwgResults.MinimumSize = new System.Drawing.Size(160, 105);
            this.jwgResults.MouseWheelZoomEnabled = true;
            this.jwgResults.Name = "jwgResults";
            this.jwgResults.RightMouseFunctionalityEnabled = true;
            this.jwgResults.ScaleProportional = false;
            this.jwgResults.Size = new System.Drawing.Size(714, 324);
            this.jwgResults.TabIndex = 0;
            this.jwgResults.Text = "jwGraph1";
            this.jwgResults.TopLeftColor = System.Drawing.Color.White;
            this.jwgResults.VerticalMarkerCount = 0;
            // 
            // tbResults
            // 
            this.tbResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbResults.Font = new System.Drawing.Font("Lucida Console", 8.25F);
            this.tbResults.Location = new System.Drawing.Point(0, 0);
            this.tbResults.Name = "tbResults";
            this.tbResults.Size = new System.Drawing.Size(714, 110);
            this.tbResults.TabIndex = 0;
            this.tbResults.Text = "";
            // 
            // frmRandlesSevchikResults
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(736, 462);
            this.Controls.Add(this.splitContainer1);
            this.Name = "frmRandlesSevchikResults";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.Text = "Randles Sevchik Results";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private jwGraph.jwGraph.jwGraph jwgResults;
        private System.Windows.Forms.RichTextBox tbResults;
    }
}