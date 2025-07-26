namespace FigureOperation
{
    partial class FormFigureOperation
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormFigureOperation));
			this.groupBoxFigureObject = new System.Windows.Forms.GroupBox();
			this.buttonClear = new System.Windows.Forms.Button();
			this.labelTemplateType = new System.Windows.Forms.Label();
			this.labelDeclType = new System.Windows.Forms.Label();
			this.buttonCreate = new System.Windows.Forms.Button();
			this.comboBoxTemplateType = new System.Windows.Forms.ComboBox();
			this.comboBoxDeclType = new System.Windows.Forms.ComboBox();
			this.groupBoxOperation = new System.Windows.Forms.GroupBox();
			this.richTextBoxMessage = new System.Windows.Forms.RichTextBox();
			this.labelMessage = new System.Windows.Forms.Label();
			this.labelOperation = new System.Windows.Forms.Label();
			this.comboBoxOperation = new System.Windows.Forms.ComboBox();
			this.buttonExecute = new System.Windows.Forms.Button();
			this.DestinationFigure = new System.Windows.Forms.Label();
			this.labelSourceFigure = new System.Windows.Forms.Label();
			this.comboBoxDst = new System.Windows.Forms.ComboBox();
			this.comboBoxSrc = new System.Windows.Forms.ComboBox();
			this.pictureBoxView = new System.Windows.Forms.PictureBox();
			this.groupBoxFigureObject.SuspendLayout();
			this.groupBoxOperation.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxView)).BeginInit();
			this.SuspendLayout();
			// 
			// groupBoxFigureObject
			// 
			this.groupBoxFigureObject.Controls.Add(this.buttonClear);
			this.groupBoxFigureObject.Controls.Add(this.labelTemplateType);
			this.groupBoxFigureObject.Controls.Add(this.labelDeclType);
			this.groupBoxFigureObject.Controls.Add(this.buttonCreate);
			this.groupBoxFigureObject.Controls.Add(this.comboBoxTemplateType);
			this.groupBoxFigureObject.Controls.Add(this.comboBoxDeclType);
			this.groupBoxFigureObject.Font = new System.Drawing.Font("Arial", 9F);
			this.groupBoxFigureObject.Location = new System.Drawing.Point(560, 10);
			this.groupBoxFigureObject.Name = "groupBoxFigureObject";
			this.groupBoxFigureObject.Size = new System.Drawing.Size(170, 172);
			this.groupBoxFigureObject.TabIndex = 4;
			this.groupBoxFigureObject.TabStop = false;
			this.groupBoxFigureObject.Text = "Figure Object";
			// 
			// buttonClear
			// 
			this.buttonClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonClear.Location = new System.Drawing.Point(8, 139);
			this.buttonClear.Name = "buttonClear";
			this.buttonClear.Size = new System.Drawing.Size(154, 25);
			this.buttonClear.TabIndex = 9;
			this.buttonClear.Text = "Clear";
			this.buttonClear.UseVisualStyleBackColor = true;
			// 
			// labelTemplateType
			// 
			this.labelTemplateType.AutoSize = true;
			this.labelTemplateType.Location = new System.Drawing.Point(6, 63);
			this.labelTemplateType.Name = "labelTemplateType";
			this.labelTemplateType.Size = new System.Drawing.Size(86, 15);
			this.labelTemplateType.TabIndex = 7;
			this.labelTemplateType.Text = "Template Type";
			// 
			// labelDeclType
			// 
			this.labelDeclType.AutoSize = true;
			this.labelDeclType.Location = new System.Drawing.Point(6, 19);
			this.labelDeclType.Name = "labelDeclType";
			this.labelDeclType.Size = new System.Drawing.Size(60, 15);
			this.labelDeclType.TabIndex = 5;
			this.labelDeclType.Text = "Decl Type";
			// 
			// buttonCreate
			// 
			this.buttonCreate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonCreate.Location = new System.Drawing.Point(8, 110);
			this.buttonCreate.Name = "buttonCreate";
			this.buttonCreate.Size = new System.Drawing.Size(154, 25);
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
			this.comboBoxTemplateType.Location = new System.Drawing.Point(8, 81);
			this.comboBoxTemplateType.Name = "comboBoxTemplateType";
			this.comboBoxTemplateType.Size = new System.Drawing.Size(154, 23);
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
			this.comboBoxDeclType.Location = new System.Drawing.Point(8, 37);
			this.comboBoxDeclType.Name = "comboBoxDeclType";
			this.comboBoxDeclType.Size = new System.Drawing.Size(154, 23);
			this.comboBoxDeclType.TabIndex = 1;
			this.comboBoxDeclType.Text = "Point";
			// 
			// groupBoxOperation
			// 
			this.groupBoxOperation.Controls.Add(this.richTextBoxMessage);
			this.groupBoxOperation.Controls.Add(this.labelMessage);
			this.groupBoxOperation.Controls.Add(this.labelOperation);
			this.groupBoxOperation.Controls.Add(this.comboBoxOperation);
			this.groupBoxOperation.Controls.Add(this.buttonExecute);
			this.groupBoxOperation.Controls.Add(this.DestinationFigure);
			this.groupBoxOperation.Controls.Add(this.labelSourceFigure);
			this.groupBoxOperation.Controls.Add(this.comboBoxDst);
			this.groupBoxOperation.Controls.Add(this.comboBoxSrc);
			this.groupBoxOperation.Font = new System.Drawing.Font("Arial", 9F);
			this.groupBoxOperation.Location = new System.Drawing.Point(560, 188);
			this.groupBoxOperation.Name = "groupBoxOperation";
			this.groupBoxOperation.Size = new System.Drawing.Size(170, 317);
			this.groupBoxOperation.TabIndex = 10;
			this.groupBoxOperation.TabStop = false;
			this.groupBoxOperation.Text = "Figure Operation";
			// 
			// richTextBoxMessage
			// 
			this.richTextBoxMessage.Location = new System.Drawing.Point(8, 225);
			this.richTextBoxMessage.Name = "richTextBoxMessage";
			this.richTextBoxMessage.ReadOnly = true;
			this.richTextBoxMessage.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.richTextBoxMessage.Size = new System.Drawing.Size(154, 82);
			this.richTextBoxMessage.TabIndex = 13;
			this.richTextBoxMessage.Text = "";
			// 
			// labelMessage
			// 
			this.labelMessage.AutoSize = true;
			this.labelMessage.Location = new System.Drawing.Point(6, 204);
			this.labelMessage.Name = "labelMessage";
			this.labelMessage.Size = new System.Drawing.Size(58, 15);
			this.labelMessage.TabIndex = 12;
			this.labelMessage.Text = "Message";
			// 
			// labelOperation
			// 
			this.labelOperation.AutoSize = true;
			this.labelOperation.Location = new System.Drawing.Point(9, 121);
			this.labelOperation.Name = "labelOperation";
			this.labelOperation.Size = new System.Drawing.Size(61, 15);
			this.labelOperation.TabIndex = 11;
			this.labelOperation.Text = "Operation";
			// 
			// comboBoxOperation
			// 
			this.comboBoxOperation.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.comboBoxOperation.FormattingEnabled = true;
			this.comboBoxOperation.Items.AddRange(new object[] {
            "Intersection",
            "Union",
            "Subtraction",
            "Exclusive Or"});
			this.comboBoxOperation.Location = new System.Drawing.Point(8, 142);
			this.comboBoxOperation.Name = "comboBoxOperation";
			this.comboBoxOperation.Size = new System.Drawing.Size(154, 23);
			this.comboBoxOperation.TabIndex = 10;
			this.comboBoxOperation.Text = "Intersection";
			// 
			// buttonExecute
			// 
			this.buttonExecute.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonExecute.Location = new System.Drawing.Point(8, 172);
			this.buttonExecute.Name = "buttonExecute";
			this.buttonExecute.Size = new System.Drawing.Size(154, 25);
			this.buttonExecute.TabIndex = 9;
			this.buttonExecute.Text = "Execute";
			this.buttonExecute.UseVisualStyleBackColor = true;
			// 
			// DestinationFigure
			// 
			this.DestinationFigure.AutoSize = true;
			this.DestinationFigure.BackColor = System.Drawing.Color.CornflowerBlue;
			this.DestinationFigure.Location = new System.Drawing.Point(9, 70);
			this.DestinationFigure.Name = "DestinationFigure";
			this.DestinationFigure.Size = new System.Drawing.Size(108, 15);
			this.DestinationFigure.TabIndex = 7;
			this.DestinationFigure.Text = "Destination Figure";
			// 
			// labelSourceFigure
			// 
			this.labelSourceFigure.AutoSize = true;
			this.labelSourceFigure.BackColor = System.Drawing.Color.Salmon;
			this.labelSourceFigure.Location = new System.Drawing.Point(9, 21);
			this.labelSourceFigure.Name = "labelSourceFigure";
			this.labelSourceFigure.Size = new System.Drawing.Size(84, 15);
			this.labelSourceFigure.TabIndex = 5;
			this.labelSourceFigure.Text = "Source Figure";
			// 
			// comboBoxDst
			// 
			this.comboBoxDst.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.comboBoxDst.FormattingEnabled = true;
			this.comboBoxDst.Location = new System.Drawing.Point(8, 93);
			this.comboBoxDst.Name = "comboBoxDst";
			this.comboBoxDst.Size = new System.Drawing.Size(154, 23);
			this.comboBoxDst.TabIndex = 2;
			// 
			// comboBoxSrc
			// 
			this.comboBoxSrc.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.comboBoxSrc.FormattingEnabled = true;
			this.comboBoxSrc.Location = new System.Drawing.Point(9, 42);
			this.comboBoxSrc.Name = "comboBoxSrc";
			this.comboBoxSrc.Size = new System.Drawing.Size(154, 23);
			this.comboBoxSrc.TabIndex = 1;
			// 
			// pictureBoxView
			// 
			this.pictureBoxView.Location = new System.Drawing.Point(12, 12);
			this.pictureBoxView.Name = "pictureBoxView";
			this.pictureBoxView.Size = new System.Drawing.Size(542, 493);
			this.pictureBoxView.TabIndex = 11;
			this.pictureBoxView.TabStop = false;
			// 
			// FormFigureOperation
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(734, 511);
			this.Controls.Add(this.pictureBoxView);
			this.Controls.Add(this.groupBoxOperation);
			this.Controls.Add(this.groupBoxFigureObject);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormFigureOperation";
			this.Text = "FigureOperation";
			this.groupBoxFigureObject.ResumeLayout(false);
			this.groupBoxFigureObject.PerformLayout();
			this.groupBoxOperation.ResumeLayout(false);
			this.groupBoxOperation.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxView)).EndInit();
			this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBoxFigureObject;
        private System.Windows.Forms.Label labelDeclType;
        private System.Windows.Forms.Label labelTemplateType;
        private System.Windows.Forms.ComboBox comboBoxDeclType;
        private System.Windows.Forms.ComboBox comboBoxTemplateType;
        private System.Windows.Forms.Button buttonCreate;
        private System.Windows.Forms.Button buttonClear;
        private System.Windows.Forms.GroupBox groupBoxOperation;
        private System.Windows.Forms.Label DestinationFigure;
        private System.Windows.Forms.Label labelSourceFigure;
        private System.Windows.Forms.ComboBox comboBoxDst;
        private System.Windows.Forms.ComboBox comboBoxSrc;
        private System.Windows.Forms.Label labelOperation;
        private System.Windows.Forms.ComboBox comboBoxOperation;
        private System.Windows.Forms.Button buttonExecute;
        private System.Windows.Forms.Label labelMessage;
        private System.Windows.Forms.RichTextBox richTextBoxMessage;
		private System.Windows.Forms.PictureBox pictureBoxView;
	}
}
