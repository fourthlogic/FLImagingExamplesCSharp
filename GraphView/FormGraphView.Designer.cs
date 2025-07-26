namespace GraphView
{
    partial class FormGraphView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormGraphView));
            this.buttonOpenView = new System.Windows.Forms.Button();
            this.buttonTerminateView = new System.Windows.Forms.Button();
            this.buttonLoadGraph = new System.Windows.Forms.Button();
            this.buttonSaveGraph = new System.Windows.Forms.Button();
            this.groupBoxChart = new System.Windows.Forms.GroupBox();
            this.textBoxChartName = new System.Windows.Forms.TextBox();
            this.labelType = new System.Windows.Forms.Label();
            this.labelName = new System.Windows.Forms.Label();
            this.buttonClear = new System.Windows.Forms.Button();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.comboBoxChartType = new System.Windows.Forms.ComboBox();
            this.groupBoxChart.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonOpenView
            // 
            this.buttonOpenView.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonOpenView.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonOpenView.Location = new System.Drawing.Point(12, 10);
            this.buttonOpenView.Name = "buttonOpenView";
            this.buttonOpenView.Size = new System.Drawing.Size(175, 25);
            this.buttonOpenView.TabIndex = 0;
            this.buttonOpenView.Text = "Open Graph View";
            this.buttonOpenView.UseVisualStyleBackColor = true;
            // 
            // buttonTerminateView
            // 
            this.buttonTerminateView.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonTerminateView.Font = new System.Drawing.Font("Arial", 9F);
            this.buttonTerminateView.Location = new System.Drawing.Point(193, 10);
            this.buttonTerminateView.Name = "buttonTerminateView";
            this.buttonTerminateView.Size = new System.Drawing.Size(175, 25);
            this.buttonTerminateView.TabIndex = 1;
            this.buttonTerminateView.Text = "Terminate Graph View";
            this.buttonTerminateView.UseVisualStyleBackColor = true;
            // 
            // buttonLoadGraph
            // 
            this.buttonLoadGraph.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonLoadGraph.Font = new System.Drawing.Font("Arial", 9F);
            this.buttonLoadGraph.Location = new System.Drawing.Point(12, 39);
            this.buttonLoadGraph.Name = "buttonLoadGraph";
            this.buttonLoadGraph.Size = new System.Drawing.Size(175, 25);
            this.buttonLoadGraph.TabIndex = 2;
            this.buttonLoadGraph.Text = "Load Graph";
            this.buttonLoadGraph.UseVisualStyleBackColor = true;
            // 
            // buttonSaveGraph
            // 
            this.buttonSaveGraph.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonSaveGraph.Font = new System.Drawing.Font("Arial", 9F);
            this.buttonSaveGraph.Location = new System.Drawing.Point(193, 39);
            this.buttonSaveGraph.Name = "buttonSaveGraph";
            this.buttonSaveGraph.Size = new System.Drawing.Size(175, 25);
            this.buttonSaveGraph.TabIndex = 3;
            this.buttonSaveGraph.Text = "Save Graph";
            this.buttonSaveGraph.UseVisualStyleBackColor = true;
            // 
            // groupBoxChart
            // 
            this.groupBoxChart.Controls.Add(this.textBoxChartName);
            this.groupBoxChart.Controls.Add(this.labelType);
            this.groupBoxChart.Controls.Add(this.labelName);
            this.groupBoxChart.Controls.Add(this.buttonClear);
            this.groupBoxChart.Controls.Add(this.buttonAdd);
            this.groupBoxChart.Controls.Add(this.comboBoxChartType);
            this.groupBoxChart.Font = new System.Drawing.Font("Arial", 9F);
            this.groupBoxChart.Location = new System.Drawing.Point(12, 70);
            this.groupBoxChart.Name = "groupBoxChart";
            this.groupBoxChart.Size = new System.Drawing.Size(356, 103);
            this.groupBoxChart.TabIndex = 4;
            this.groupBoxChart.TabStop = false;
            this.groupBoxChart.Text = "Chart";
            // 
            // textBoxChartName
            // 
            this.textBoxChartName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxChartName.Location = new System.Drawing.Point(9, 40);
            this.textBoxChartName.Name = "textBoxChartName";
            this.textBoxChartName.Size = new System.Drawing.Size(166, 21);
            this.textBoxChartName.TabIndex = 7;
            // 
            // labelType
            // 
            this.labelType.AutoSize = true;
            this.labelType.Location = new System.Drawing.Point(179, 18);
            this.labelType.Name = "labelType";
            this.labelType.Size = new System.Drawing.Size(32, 15);
            this.labelType.TabIndex = 6;
            this.labelType.Text = "Type";
            // 
            // labelName
            // 
            this.labelName.AutoSize = true;
            this.labelName.Location = new System.Drawing.Point(6, 18);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(41, 15);
            this.labelName.TabIndex = 5;
            this.labelName.Text = "Name";
            // 
            // buttonClear
            // 
            this.buttonClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonClear.Location = new System.Drawing.Point(182, 69);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(166, 25);
            this.buttonClear.TabIndex = 4;
            this.buttonClear.Text = "Clear";
            this.buttonClear.UseVisualStyleBackColor = true;
            // 
            // buttonAdd
            // 
            this.buttonAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonAdd.Location = new System.Drawing.Point(8, 69);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(167, 25);
            this.buttonAdd.TabIndex = 3;
            this.buttonAdd.Text = "Add";
            this.buttonAdd.UseVisualStyleBackColor = true;
            // 
            // comboBoxChartType
            // 
            this.comboBoxChartType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboBoxChartType.FormattingEnabled = true;
            this.comboBoxChartType.Items.AddRange(new object[] {
            "Bar",
            "Line",
            "Scatter"});
            this.comboBoxChartType.Location = new System.Drawing.Point(181, 40);
            this.comboBoxChartType.Name = "comboBoxChartType";
            this.comboBoxChartType.Size = new System.Drawing.Size(167, 23);
            this.comboBoxChartType.TabIndex = 1;
            this.comboBoxChartType.Text = "Bar";
            // 
            // FormGraphView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(381, 185);
            this.Controls.Add(this.groupBoxChart);
            this.Controls.Add(this.buttonSaveGraph);
            this.Controls.Add(this.buttonLoadGraph);
            this.Controls.Add(this.buttonTerminateView);
            this.Controls.Add(this.buttonOpenView);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormGraphView";
            this.Text = "FormGraphView";
            this.groupBoxChart.ResumeLayout(false);
            this.groupBoxChart.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonOpenView;
        private System.Windows.Forms.Button buttonTerminateView;
        private System.Windows.Forms.Button buttonLoadGraph;
        private System.Windows.Forms.Button buttonSaveGraph;
        private System.Windows.Forms.GroupBox groupBoxChart;
        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.Label labelType;
        private System.Windows.Forms.Button buttonAdd;
        private System.Windows.Forms.Button buttonClear;
        private System.Windows.Forms.ComboBox comboBoxChartType;
        private System.Windows.Forms.TextBox textBoxChartName;
    }
}