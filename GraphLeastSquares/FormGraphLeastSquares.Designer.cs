namespace FLImagingExamplesCSharp
{
    partial class FormGraphLeastSquares
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormGraphLeastSquares));
			this.groupBoxEqation = new System.Windows.Forms.GroupBox();
			this.richTextBoxInfo = new System.Windows.Forms.RichTextBox();
			this.labelInfo = new System.Windows.Forms.Label();
			this.buttonClear = new System.Windows.Forms.Button();
			this.buttonAdd = new System.Windows.Forms.Button();
			this.textBoxDegree = new System.Windows.Forms.TextBox();
			this.labelDegree = new System.Windows.Forms.Label();
			this.textBoxName = new System.Windows.Forms.TextBox();
			this.labelName = new System.Windows.Forms.Label();
			this.pictureBoxView = new System.Windows.Forms.PictureBox();
			this.groupBoxEqation.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxView)).BeginInit();
			this.SuspendLayout();
			// 
			// groupBoxEqation
			// 
			this.groupBoxEqation.Controls.Add(this.richTextBoxInfo);
			this.groupBoxEqation.Controls.Add(this.labelInfo);
			this.groupBoxEqation.Controls.Add(this.buttonClear);
			this.groupBoxEqation.Controls.Add(this.buttonAdd);
			this.groupBoxEqation.Controls.Add(this.textBoxDegree);
			this.groupBoxEqation.Controls.Add(this.labelDegree);
			this.groupBoxEqation.Controls.Add(this.textBoxName);
			this.groupBoxEqation.Controls.Add(this.labelName);
			this.groupBoxEqation.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.groupBoxEqation.Font = new System.Drawing.Font("Arial", 9F);
			this.groupBoxEqation.Location = new System.Drawing.Point(560, 10);
			this.groupBoxEqation.Name = "groupBoxEqation";
			this.groupBoxEqation.Size = new System.Drawing.Size(160, 440);
			this.groupBoxEqation.TabIndex = 0;
			this.groupBoxEqation.TabStop = false;
			this.groupBoxEqation.Text = "Quartic Eqation";
			// 
			// richTextBoxInfo
			// 
			this.richTextBoxInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.richTextBoxInfo.Location = new System.Drawing.Point(9, 161);
			this.richTextBoxInfo.Name = "richTextBoxInfo";
			this.richTextBoxInfo.ReadOnly = true;
			this.richTextBoxInfo.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.richTextBoxInfo.Size = new System.Drawing.Size(141, 269);
			this.richTextBoxInfo.TabIndex = 13;
			this.richTextBoxInfo.Text = "";
			// 
			// labelInfo
			// 
			this.labelInfo.AutoSize = true;
			this.labelInfo.Location = new System.Drawing.Point(6, 143);
			this.labelInfo.Name = "labelInfo";
			this.labelInfo.Size = new System.Drawing.Size(27, 15);
			this.labelInfo.TabIndex = 12;
			this.labelInfo.Text = "Info";
			// 
			// buttonClear
			// 
			this.buttonClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonClear.Location = new System.Drawing.Point(9, 103);
			this.buttonClear.Name = "buttonClear";
			this.buttonClear.Size = new System.Drawing.Size(141, 23);
			this.buttonClear.TabIndex = 11;
			this.buttonClear.Text = "Clear";
			this.buttonClear.UseVisualStyleBackColor = true;
			// 
			// buttonAdd
			// 
			this.buttonAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonAdd.Location = new System.Drawing.Point(9, 74);
			this.buttonAdd.Name = "buttonAdd";
			this.buttonAdd.Size = new System.Drawing.Size(141, 23);
			this.buttonAdd.TabIndex = 10;
			this.buttonAdd.Text = "Add";
			this.buttonAdd.UseVisualStyleBackColor = true;
			// 
			// textBoxDegree
			// 
			this.textBoxDegree.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textBoxDegree.Location = new System.Drawing.Point(53, 47);
			this.textBoxDegree.Name = "textBoxDegree";
			this.textBoxDegree.Size = new System.Drawing.Size(97, 21);
			this.textBoxDegree.TabIndex = 7;
			this.textBoxDegree.Text = "4";
			// 
			// labelDegree
			// 
			this.labelDegree.AutoSize = true;
			this.labelDegree.Location = new System.Drawing.Point(6, 49);
			this.labelDegree.Name = "labelDegree";
			this.labelDegree.Size = new System.Drawing.Size(48, 15);
			this.labelDegree.TabIndex = 6;
			this.labelDegree.Text = "Degree";
			// 
			// textBoxName
			// 
			this.textBoxName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textBoxName.Location = new System.Drawing.Point(53, 20);
			this.textBoxName.Name = "textBoxName";
			this.textBoxName.Size = new System.Drawing.Size(97, 21);
			this.textBoxName.TabIndex = 1;
			this.textBoxName.Text = "Example";
			// 
			// labelName
			// 
			this.labelName.AutoSize = true;
			this.labelName.Location = new System.Drawing.Point(6, 22);
			this.labelName.Name = "labelName";
			this.labelName.Size = new System.Drawing.Size(41, 15);
			this.labelName.TabIndex = 0;
			this.labelName.Text = "Name";
			// 
			// pictureBoxView
			// 
			this.pictureBoxView.Location = new System.Drawing.Point(12, 10);
			this.pictureBoxView.Name = "pictureBoxView";
			this.pictureBoxView.Size = new System.Drawing.Size(542, 440);
			this.pictureBoxView.TabIndex = 1;
			this.pictureBoxView.TabStop = false;
			// 
			// FormGraphLeastSquares
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(734, 461);
			this.Controls.Add(this.pictureBoxView);
			this.Controls.Add(this.groupBoxEqation);
			this.Font = new System.Drawing.Font("굴림", 9F);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormGraphLeastSquares";
			this.Text = "FormGraphLeastSquares";
			this.groupBoxEqation.ResumeLayout(false);
			this.groupBoxEqation.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxView)).EndInit();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxEqation;
        private System.Windows.Forms.RichTextBox richTextBoxInfo;
        private System.Windows.Forms.Label labelInfo;
        private System.Windows.Forms.Button buttonClear;
        private System.Windows.Forms.Button buttonAdd;
        private System.Windows.Forms.TextBox textBoxDegree;
        private System.Windows.Forms.Label labelDegree;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.Label labelName;
		private System.Windows.Forms.PictureBox pictureBoxView;
	}
}