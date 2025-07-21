namespace ImageView
{
	partial class FormImageViewIntoDialog
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormImageViewIntoDialog));
			this.buttonCreate = new System.Windows.Forms.Button();
			this.buttonPopFront = new System.Windows.Forms.Button();
			this.groupBoxRectFigureObject = new System.Windows.Forms.GroupBox();
			this.richTextBoxFigureInfo = new System.Windows.Forms.RichTextBox();
			this.labelInfo = new System.Windows.Forms.Label();
			this.pictureBoxView = new System.Windows.Forms.PictureBox();
			this.groupBoxRectFigureObject.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxView)).BeginInit();
			this.SuspendLayout();
			// 
			// buttonCreate
			// 
			this.buttonCreate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonCreate.Font = new System.Drawing.Font("Arial", 9F);
			this.buttonCreate.Location = new System.Drawing.Point(8, 20);
			this.buttonCreate.Name = "buttonCreate";
			this.buttonCreate.Size = new System.Drawing.Size(156, 23);
			this.buttonCreate.TabIndex = 0;
			this.buttonCreate.Text = "Create";
			this.buttonCreate.UseVisualStyleBackColor = true;
			// 
			// buttonPopFront
			// 
			this.buttonPopFront.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonPopFront.Font = new System.Drawing.Font("Arial", 9F);
			this.buttonPopFront.Location = new System.Drawing.Point(8, 49);
			this.buttonPopFront.Name = "buttonPopFront";
			this.buttonPopFront.Size = new System.Drawing.Size(156, 23);
			this.buttonPopFront.TabIndex = 1;
			this.buttonPopFront.Text = "Pop Front";
			this.buttonPopFront.UseVisualStyleBackColor = true;
			// 
			// groupBoxRectFigureObject
			// 
			this.groupBoxRectFigureObject.Controls.Add(this.richTextBoxFigureInfo);
			this.groupBoxRectFigureObject.Controls.Add(this.buttonCreate);
			this.groupBoxRectFigureObject.Controls.Add(this.labelInfo);
			this.groupBoxRectFigureObject.Controls.Add(this.buttonPopFront);
			this.groupBoxRectFigureObject.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.groupBoxRectFigureObject.Font = new System.Drawing.Font("Arial", 9F);
			this.groupBoxRectFigureObject.Location = new System.Drawing.Point(560, 10);
			this.groupBoxRectFigureObject.Name = "groupBoxRectFigureObject";
			this.groupBoxRectFigureObject.Size = new System.Drawing.Size(170, 440);
			this.groupBoxRectFigureObject.TabIndex = 2;
			this.groupBoxRectFigureObject.TabStop = false;
			this.groupBoxRectFigureObject.Text = "RectFigure Object";
			// 
			// richTextBoxFigureInfo
			// 
			this.richTextBoxFigureInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.richTextBoxFigureInfo.Font = new System.Drawing.Font("Arial", 9F);
			this.richTextBoxFigureInfo.Location = new System.Drawing.Point(6, 93);
			this.richTextBoxFigureInfo.Name = "richTextBoxFigureInfo";
			this.richTextBoxFigureInfo.ReadOnly = true;
			this.richTextBoxFigureInfo.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.richTextBoxFigureInfo.Size = new System.Drawing.Size(158, 341);
			this.richTextBoxFigureInfo.TabIndex = 4;
			this.richTextBoxFigureInfo.Text = "";
			// 
			// labelInfo
			// 
			this.labelInfo.AutoSize = true;
			this.labelInfo.Font = new System.Drawing.Font("Arial", 9F);
			this.labelInfo.Location = new System.Drawing.Point(6, 75);
			this.labelInfo.Name = "labelInfo";
			this.labelInfo.Size = new System.Drawing.Size(27, 15);
			this.labelInfo.TabIndex = 3;
			this.labelInfo.Text = "Info";
			// 
			// pictureBoxView
			// 
			this.pictureBoxView.Location = new System.Drawing.Point(12, 12);
			this.pictureBoxView.Name = "pictureBoxView";
			this.pictureBoxView.Size = new System.Drawing.Size(542, 438);
			this.pictureBoxView.TabIndex = 3;
			this.pictureBoxView.TabStop = false;
			// 
			// FormImageViewIntoDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(734, 461);
			this.Controls.Add(this.pictureBoxView);
			this.Controls.Add(this.groupBoxRectFigureObject);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormImageViewIntoDialog";
			this.Text = "ImageViewInToDialog";
			this.groupBoxRectFigureObject.ResumeLayout(false);
			this.groupBoxRectFigureObject.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxView)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button buttonCreate;
		private System.Windows.Forms.Button buttonPopFront;
		private System.Windows.Forms.GroupBox groupBoxRectFigureObject;
		private System.Windows.Forms.RichTextBox richTextBoxFigureInfo;
		private System.Windows.Forms.Label labelInfo;
		private System.Windows.Forms.PictureBox pictureBoxView;
	}
}