namespace View3DIntoDialog
{
	partial class FormView3DIntoDialog
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
			if(disposing && (components != null))
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormView3DIntoDialog));
			this.buttonGetHeightProfile = new System.Windows.Forms.Button();
			this.richTextBoxInfo = new System.Windows.Forms.RichTextBox();
			this.groupBoxRect = new System.Windows.Forms.GroupBox();
			this.labelHeightProfile = new System.Windows.Forms.Label();
			this.textBoxEndY = new System.Windows.Forms.TextBox();
			this.labelEndY = new System.Windows.Forms.Label();
			this.textBoxEndX = new System.Windows.Forms.TextBox();
			this.labelEndX = new System.Windows.Forms.Label();
			this.textBoxStartY = new System.Windows.Forms.TextBox();
			this.labelStartY = new System.Windows.Forms.Label();
			this.textBoxStartX = new System.Windows.Forms.TextBox();
			this.labelStartX = new System.Windows.Forms.Label();
			this.pictureBoxView = new System.Windows.Forms.PictureBox();
			this.groupBoxRect.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxView)).BeginInit();
			this.SuspendLayout();
			// 
			// buttonGetHeightProfile
			// 
			this.buttonGetHeightProfile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonGetHeightProfile.Font = new System.Drawing.Font("Arial", 9F);
			this.buttonGetHeightProfile.Location = new System.Drawing.Point(8, 92);
			this.buttonGetHeightProfile.Name = "buttonGetHeightProfile";
			this.buttonGetHeightProfile.Size = new System.Drawing.Size(157, 33);
			this.buttonGetHeightProfile.TabIndex = 0;
			this.buttonGetHeightProfile.Text = "Height Profile";
			this.buttonGetHeightProfile.UseVisualStyleBackColor = true;
			// 
			// richTextBoxInfo
			// 
			this.richTextBoxInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.richTextBoxInfo.Font = new System.Drawing.Font("Arial", 9F);
			this.richTextBoxInfo.Location = new System.Drawing.Point(6, 131);
			this.richTextBoxInfo.Name = "richTextBoxInfo";
			this.richTextBoxInfo.ReadOnly = true;
			this.richTextBoxInfo.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.richTextBoxInfo.Size = new System.Drawing.Size(158, 303);
			this.richTextBoxInfo.TabIndex = 4;
			this.richTextBoxInfo.Text = "";
			// 
			// groupBoxRect
			// 
			this.groupBoxRect.Controls.Add(this.labelHeightProfile);
			this.groupBoxRect.Controls.Add(this.textBoxEndY);
			this.groupBoxRect.Controls.Add(this.labelEndY);
			this.groupBoxRect.Controls.Add(this.textBoxEndX);
			this.groupBoxRect.Controls.Add(this.labelEndX);
			this.groupBoxRect.Controls.Add(this.textBoxStartY);
			this.groupBoxRect.Controls.Add(this.labelStartY);
			this.groupBoxRect.Controls.Add(this.textBoxStartX);
			this.groupBoxRect.Controls.Add(this.labelStartX);
			this.groupBoxRect.Controls.Add(this.richTextBoxInfo);
			this.groupBoxRect.Controls.Add(this.buttonGetHeightProfile);
			this.groupBoxRect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.groupBoxRect.Font = new System.Drawing.Font("Arial", 9F);
			this.groupBoxRect.Location = new System.Drawing.Point(560, 10);
			this.groupBoxRect.Name = "groupBoxRect";
			this.groupBoxRect.Size = new System.Drawing.Size(170, 440);
			this.groupBoxRect.TabIndex = 2;
			this.groupBoxRect.TabStop = false;
			// 
			// labelHeightProfile
			// 
			this.labelHeightProfile.AutoSize = true;
			this.labelHeightProfile.Font = new System.Drawing.Font("Arial", 9F);
			this.labelHeightProfile.Location = new System.Drawing.Point(6, 13);
			this.labelHeightProfile.Name = "labelHeightProfile";
			this.labelHeightProfile.Size = new System.Drawing.Size(81, 15);
			this.labelHeightProfile.TabIndex = 13;
			this.labelHeightProfile.Text = "Height Profile";
			// 
			// textBoxEndY
			// 
			this.textBoxEndY.Location = new System.Drawing.Point(120, 65);
			this.textBoxEndY.Name = "textBoxEndY";
			this.textBoxEndY.Size = new System.Drawing.Size(45, 21);
			this.textBoxEndY.TabIndex = 12;
			this.textBoxEndY.Text = "120";
			// 
			// labelEndY
			// 
			this.labelEndY.AutoSize = true;
			this.labelEndY.Location = new System.Drawing.Point(101, 68);
			this.labelEndY.Name = "labelEndY";
			this.labelEndY.Size = new System.Drawing.Size(15, 15);
			this.labelEndY.TabIndex = 11;
			this.labelEndY.Text = " y";
			// 
			// textBoxEndX
			// 
			this.textBoxEndX.Location = new System.Drawing.Point(58, 65);
			this.textBoxEndX.Name = "textBoxEndX";
			this.textBoxEndX.Size = new System.Drawing.Size(37, 21);
			this.textBoxEndX.TabIndex = 10;
			this.textBoxEndX.Text = "104";
			// 
			// labelEndX
			// 
			this.labelEndX.AutoSize = true;
			this.labelEndX.Location = new System.Drawing.Point(6, 68);
			this.labelEndX.Name = "labelEndX";
			this.labelEndX.Size = new System.Drawing.Size(46, 15);
			this.labelEndX.TabIndex = 9;
			this.labelEndX.Text = "End    x";
			// 
			// textBoxStartY
			// 
			this.textBoxStartY.Location = new System.Drawing.Point(122, 35);
			this.textBoxStartY.Name = "textBoxStartY";
			this.textBoxStartY.Size = new System.Drawing.Size(43, 21);
			this.textBoxStartY.TabIndex = 8;
			this.textBoxStartY.Text = "0";
			// 
			// labelStartY
			// 
			this.labelStartY.AutoSize = true;
			this.labelStartY.Location = new System.Drawing.Point(101, 38);
			this.labelStartY.Name = "labelStartY";
			this.labelStartY.Size = new System.Drawing.Size(15, 15);
			this.labelStartY.TabIndex = 7;
			this.labelStartY.Text = " y";
			// 
			// textBoxStartX
			// 
			this.textBoxStartX.Location = new System.Drawing.Point(58, 35);
			this.textBoxStartX.Name = "textBoxStartX";
			this.textBoxStartX.Size = new System.Drawing.Size(37, 21);
			this.textBoxStartX.TabIndex = 6;
			this.textBoxStartX.Text = "0";
			// 
			// labelStartX
			// 
			this.labelStartX.AutoSize = true;
			this.labelStartX.Location = new System.Drawing.Point(6, 38);
			this.labelStartX.Name = "labelStartX";
			this.labelStartX.Size = new System.Drawing.Size(46, 15);
			this.labelStartX.TabIndex = 5;
			this.labelStartX.Text = "Start   x";
			// 
			// pictureBoxView
			// 
			this.pictureBoxView.Location = new System.Drawing.Point(12, 12);
			this.pictureBoxView.Name = "pictureBoxView";
			this.pictureBoxView.Size = new System.Drawing.Size(542, 438);
			this.pictureBoxView.TabIndex = 3;
			this.pictureBoxView.TabStop = false;
			// 
			// FormView3DIntoDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(734, 461);
			this.Controls.Add(this.pictureBoxView);
			this.Controls.Add(this.groupBoxRect);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormView3DIntoDialog";
			this.Text = "View3DIntoDialog";
			this.groupBoxRect.ResumeLayout(false);
			this.groupBoxRect.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxView)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBoxRect;
		private System.Windows.Forms.Button buttonGetHeightProfile;
		private System.Windows.Forms.RichTextBox richTextBoxInfo;
		private System.Windows.Forms.Label labelStartX;
		private System.Windows.Forms.TextBox textBoxStartX;
		private System.Windows.Forms.TextBox textBoxEndY;
		private System.Windows.Forms.Label labelEndY;
		private System.Windows.Forms.TextBox textBoxEndX;
		private System.Windows.Forms.Label labelEndX;
		private System.Windows.Forms.TextBox textBoxStartY;
		private System.Windows.Forms.Label labelStartY;
		private System.Windows.Forms.Label labelHeightProfile;
		private System.Windows.Forms.PictureBox pictureBoxView;
	}
}