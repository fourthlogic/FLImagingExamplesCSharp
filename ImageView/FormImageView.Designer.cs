namespace ImageView
{
    partial class FormImageView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormImageView));
            this.buttonOpenView = new System.Windows.Forms.Button();
            this.buttonTerminateView = new System.Windows.Forms.Button();
            this.buttonLoadImage = new System.Windows.Forms.Button();
            this.buttonSaveImage = new System.Windows.Forms.Button();
            this.groupBoxFigureObject = new System.Windows.Forms.GroupBox();
            this.richTextBoxInfo = new System.Windows.Forms.RichTextBox();
            this.labelTemplateType = new System.Windows.Forms.Label();
            this.labelInfo = new System.Windows.Forms.Label();
            this.labelDeclType = new System.Windows.Forms.Label();
            this.buttonPopFront = new System.Windows.Forms.Button();
            this.buttonCreate = new System.Windows.Forms.Button();
            this.comboBoxTemplateType = new System.Windows.Forms.ComboBox();
            this.comboBoxDeclType = new System.Windows.Forms.ComboBox();
            this.groupBoxFigureObject.SuspendLayout();
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
            this.buttonOpenView.Text = "Open Image View";
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
            this.buttonTerminateView.Text = "Terminate Image View";
            this.buttonTerminateView.UseVisualStyleBackColor = true;
            // 
            // buttonLoadImage
            // 
            this.buttonLoadImage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonLoadImage.Font = new System.Drawing.Font("Arial", 9F);
            this.buttonLoadImage.Location = new System.Drawing.Point(12, 39);
            this.buttonLoadImage.Name = "buttonLoadImage";
            this.buttonLoadImage.Size = new System.Drawing.Size(175, 25);
            this.buttonLoadImage.TabIndex = 2;
            this.buttonLoadImage.Text = "Load Image";
            this.buttonLoadImage.UseVisualStyleBackColor = true;
            // 
            // buttonSaveImage
            // 
            this.buttonSaveImage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonSaveImage.Font = new System.Drawing.Font("Arial", 9F);
            this.buttonSaveImage.Location = new System.Drawing.Point(193, 39);
            this.buttonSaveImage.Name = "buttonSaveImage";
            this.buttonSaveImage.Size = new System.Drawing.Size(175, 25);
            this.buttonSaveImage.TabIndex = 3;
            this.buttonSaveImage.Text = "Save Image";
            this.buttonSaveImage.UseVisualStyleBackColor = true;
            // 
            // groupBoxFigureObject
            // 
            this.groupBoxFigureObject.Controls.Add(this.richTextBoxInfo);
            this.groupBoxFigureObject.Controls.Add(this.labelTemplateType);
            this.groupBoxFigureObject.Controls.Add(this.labelInfo);
            this.groupBoxFigureObject.Controls.Add(this.labelDeclType);
            this.groupBoxFigureObject.Controls.Add(this.buttonPopFront);
            this.groupBoxFigureObject.Controls.Add(this.buttonCreate);
            this.groupBoxFigureObject.Controls.Add(this.comboBoxTemplateType);
            this.groupBoxFigureObject.Controls.Add(this.comboBoxDeclType);
            this.groupBoxFigureObject.Font = new System.Drawing.Font("Arial", 9F);
            this.groupBoxFigureObject.Location = new System.Drawing.Point(12, 70);
            this.groupBoxFigureObject.Name = "groupBoxFigureObject";
            this.groupBoxFigureObject.Size = new System.Drawing.Size(356, 155);
            this.groupBoxFigureObject.TabIndex = 4;
            this.groupBoxFigureObject.TabStop = false;
            this.groupBoxFigureObject.Text = "Figure Object";
            // 
            // richTextBoxInfo
            // 
            this.richTextBoxInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBoxInfo.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBoxInfo.Location = new System.Drawing.Point(181, 41);
            this.richTextBoxInfo.Name = "richTextBoxInfo";
            this.richTextBoxInfo.ReadOnly = true;
            this.richTextBoxInfo.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.richTextBoxInfo.Size = new System.Drawing.Size(169, 72);
            this.richTextBoxInfo.TabIndex = 8;
            this.richTextBoxInfo.Text = "";
            // 
            // labelTemplateType
            // 
            this.labelTemplateType.AutoSize = true;
            this.labelTemplateType.Location = new System.Drawing.Point(6, 72);
            this.labelTemplateType.Name = "labelTemplateType";
            this.labelTemplateType.Size = new System.Drawing.Size(86, 15);
            this.labelTemplateType.TabIndex = 7;
            this.labelTemplateType.Text = "Template Type";
            // 
            // labelInfo
            // 
            this.labelInfo.AutoSize = true;
            this.labelInfo.Location = new System.Drawing.Point(179, 22);
            this.labelInfo.Name = "labelInfo";
            this.labelInfo.Size = new System.Drawing.Size(27, 15);
            this.labelInfo.TabIndex = 6;
            this.labelInfo.Text = "Info";
            // 
            // labelDeclType
            // 
            this.labelDeclType.AutoSize = true;
            this.labelDeclType.Location = new System.Drawing.Point(6, 22);
            this.labelDeclType.Name = "labelDeclType";
            this.labelDeclType.Size = new System.Drawing.Size(60, 15);
            this.labelDeclType.TabIndex = 5;
            this.labelDeclType.Text = "Decl Type";
            // 
            // buttonPopFront
            // 
            this.buttonPopFront.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonPopFront.Location = new System.Drawing.Point(181, 121);
            this.buttonPopFront.Name = "buttonPopFront";
            this.buttonPopFront.Size = new System.Drawing.Size(169, 25);
            this.buttonPopFront.TabIndex = 4;
            this.buttonPopFront.Text = "Pop Front";
            this.buttonPopFront.UseVisualStyleBackColor = true;
            // 
            // buttonCreate
            // 
            this.buttonCreate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonCreate.Location = new System.Drawing.Point(8, 121);
            this.buttonCreate.Name = "buttonCreate";
            this.buttonCreate.Size = new System.Drawing.Size(167, 25);
            this.buttonCreate.TabIndex = 3;
            this.buttonCreate.Text = "Create";
            this.buttonCreate.UseVisualStyleBackColor = true;
            // 
            // comboBoxTemplateType
            // 
            this.comboBoxTemplateType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboBoxTemplateType.FormattingEnabled = true;
            this.comboBoxTemplateType.Items.AddRange(new object[] {
            "Int32",
            "Int64",
            "Float",
            "Double"});
            this.comboBoxTemplateType.Location = new System.Drawing.Point(8, 90);
            this.comboBoxTemplateType.Name = "comboBoxTemplateType";
            this.comboBoxTemplateType.Size = new System.Drawing.Size(167, 23);
            this.comboBoxTemplateType.TabIndex = 2;
            this.comboBoxTemplateType.Text = "Double";
            // 
            // comboBoxDeclType
            // 
            this.comboBoxDeclType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboBoxDeclType.FormattingEnabled = true;
            this.comboBoxDeclType.Items.AddRange(new object[] {
            "Point",
            "Line",
            "Rect",
            "Quad",
            "Circle",
            "Ellipse",
            "CubicSpline",
            "Region",
            "ComplexRegion",
            "Doughnut"});
            this.comboBoxDeclType.Location = new System.Drawing.Point(8, 41);
            this.comboBoxDeclType.Name = "comboBoxDeclType";
            this.comboBoxDeclType.Size = new System.Drawing.Size(167, 23);
            this.comboBoxDeclType.TabIndex = 1;
            this.comboBoxDeclType.Text = "Point";
            // 
            // FormImageView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(381, 238);
            this.Controls.Add(this.groupBoxFigureObject);
            this.Controls.Add(this.buttonSaveImage);
            this.Controls.Add(this.buttonLoadImage);
            this.Controls.Add(this.buttonTerminateView);
            this.Controls.Add(this.buttonOpenView);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormImageView";
            this.Text = "FormImageView";
            this.groupBoxFigureObject.ResumeLayout(false);
            this.groupBoxFigureObject.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonOpenView;
        private System.Windows.Forms.Button buttonTerminateView;
        private System.Windows.Forms.Button buttonLoadImage;
        private System.Windows.Forms.Button buttonSaveImage;
        private System.Windows.Forms.GroupBox groupBoxFigureObject;
        private System.Windows.Forms.RichTextBox richTextBoxInfo;
        private System.Windows.Forms.Label labelDeclType;
        private System.Windows.Forms.Label labelInfo;
        private System.Windows.Forms.Label labelTemplateType;
        private System.Windows.Forms.Button buttonCreate;
        private System.Windows.Forms.Button buttonPopFront;
        private System.Windows.Forms.ComboBox comboBoxDeclType;
        private System.Windows.Forms.ComboBox comboBoxTemplateType;
    }
}