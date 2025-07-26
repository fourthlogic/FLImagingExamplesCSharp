namespace FLImagingExamplesCSharp
{
    partial class FormGraphViewContextMenu
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormGraphViewContextMenu));
			this.groupBoxContextMenu = new System.Windows.Forms.GroupBox();
			this.radioButtonNone = new System.Windows.Forms.RadioButton();
			this.radioButtonAll = new System.Windows.Forms.RadioButton();
			this.radioButtonHide = new System.Windows.Forms.RadioButton();
			this.radioButtonShow = new System.Windows.Forms.RadioButton();
			this.panelAvailableContextMenu = new System.Windows.Forms.Panel();
			this.labelUnavailableMenuDisplayOptionvailableContextMenu = new System.Windows.Forms.Label();
			this.buttonApply = new System.Windows.Forms.Button();
			this.labelUnavailableMenuDisplayOption = new System.Windows.Forms.Label();
			this.pictureBoxView = new System.Windows.Forms.PictureBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel2 = new System.Windows.Forms.Panel();
			this.groupBoxContextMenu.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxView)).BeginInit();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBoxContextMenu
			// 
			this.groupBoxContextMenu.Controls.Add(this.panel2);
			this.groupBoxContextMenu.Controls.Add(this.panel1);
			this.groupBoxContextMenu.Controls.Add(this.panelAvailableContextMenu);
			this.groupBoxContextMenu.Controls.Add(this.labelUnavailableMenuDisplayOptionvailableContextMenu);
			this.groupBoxContextMenu.Controls.Add(this.buttonApply);
			this.groupBoxContextMenu.Controls.Add(this.labelUnavailableMenuDisplayOption);
			this.groupBoxContextMenu.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.groupBoxContextMenu.Font = new System.Drawing.Font("Arial", 9F);
			this.groupBoxContextMenu.Location = new System.Drawing.Point(520, 10);
			this.groupBoxContextMenu.Name = "groupBoxContextMenu";
			this.groupBoxContextMenu.Size = new System.Drawing.Size(369, 440);
			this.groupBoxContextMenu.TabIndex = 0;
			this.groupBoxContextMenu.TabStop = false;
			this.groupBoxContextMenu.Text = "Context Menu";
			// 
			// radioButtonNone
			// 
			this.radioButtonNone.AutoSize = true;
			this.radioButtonNone.Location = new System.Drawing.Point(137, 3);
			this.radioButtonNone.Name = "radioButtonNone";
			this.radioButtonNone.Size = new System.Drawing.Size(55, 19);
			this.radioButtonNone.TabIndex = 16;
			this.radioButtonNone.TabStop = true;
			this.radioButtonNone.Text = "None";
			this.radioButtonNone.UseVisualStyleBackColor = true;
			this.radioButtonNone.CheckedChanged += new System.EventHandler(this.radioButtonAll_CheckedChanged);
			// 
			// radioButtonAll
			// 
			this.radioButtonAll.AutoSize = true;
			this.radioButtonAll.Location = new System.Drawing.Point(3, 3);
			this.radioButtonAll.Name = "radioButtonAll";
			this.radioButtonAll.Size = new System.Drawing.Size(38, 19);
			this.radioButtonAll.TabIndex = 15;
			this.radioButtonAll.TabStop = true;
			this.radioButtonAll.Text = "All";
			this.radioButtonAll.UseVisualStyleBackColor = true;
			this.radioButtonAll.CheckedChanged += new System.EventHandler(this.radioButtonAll_CheckedChanged);
			// 
			// radioButtonHide
			// 
			this.radioButtonHide.AutoSize = true;
			this.radioButtonHide.Location = new System.Drawing.Point(137, 3);
			this.radioButtonHide.Name = "radioButtonHide";
			this.radioButtonHide.Size = new System.Drawing.Size(51, 19);
			this.radioButtonHide.TabIndex = 14;
			this.radioButtonHide.TabStop = true;
			this.radioButtonHide.Text = "Hide";
			this.radioButtonHide.UseVisualStyleBackColor = true;
			this.radioButtonHide.CheckedChanged += new System.EventHandler(this.radioButtonShow_CheckedChanged);
			// 
			// radioButtonShow
			// 
			this.radioButtonShow.AutoSize = true;
			this.radioButtonShow.Location = new System.Drawing.Point(3, 3);
			this.radioButtonShow.Name = "radioButtonShow";
			this.radioButtonShow.Size = new System.Drawing.Size(56, 19);
			this.radioButtonShow.TabIndex = 13;
			this.radioButtonShow.TabStop = true;
			this.radioButtonShow.Text = "Show";
			this.radioButtonShow.UseVisualStyleBackColor = true;
			this.radioButtonShow.CheckedChanged += new System.EventHandler(this.radioButtonShow_CheckedChanged);
			// 
			// panelAvailableContextMenu
			// 
			this.panelAvailableContextMenu.AutoScroll = true;
			this.panelAvailableContextMenu.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panelAvailableContextMenu.Location = new System.Drawing.Point(9, 141);
			this.panelAvailableContextMenu.Name = "panelAvailableContextMenu";
			this.panelAvailableContextMenu.Size = new System.Drawing.Size(354, 254);
			this.panelAvailableContextMenu.TabIndex = 2;
			// 
			// labelUnavailableMenuDisplayOptionvailableContextMenu
			// 
			this.labelUnavailableMenuDisplayOptionvailableContextMenu.AutoSize = true;
			this.labelUnavailableMenuDisplayOptionvailableContextMenu.Location = new System.Drawing.Point(6, 86);
			this.labelUnavailableMenuDisplayOptionvailableContextMenu.Name = "labelUnavailableMenuDisplayOptionvailableContextMenu";
			this.labelUnavailableMenuDisplayOptionvailableContextMenu.Size = new System.Drawing.Size(133, 15);
			this.labelUnavailableMenuDisplayOptionvailableContextMenu.TabIndex = 12;
			this.labelUnavailableMenuDisplayOptionvailableContextMenu.Text = "Available Context Menu";
			// 
			// buttonApply
			// 
			this.buttonApply.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonApply.Location = new System.Drawing.Point(9, 402);
			this.buttonApply.Name = "buttonApply";
			this.buttonApply.Size = new System.Drawing.Size(354, 23);
			this.buttonApply.TabIndex = 10;
			this.buttonApply.Text = "Apply";
			this.buttonApply.UseVisualStyleBackColor = true;
			// 
			// labelUnavailableMenuDisplayOption
			// 
			this.labelUnavailableMenuDisplayOption.AutoSize = true;
			this.labelUnavailableMenuDisplayOption.Location = new System.Drawing.Point(6, 22);
			this.labelUnavailableMenuDisplayOption.Name = "labelUnavailableMenuDisplayOption";
			this.labelUnavailableMenuDisplayOption.Size = new System.Drawing.Size(188, 15);
			this.labelUnavailableMenuDisplayOption.TabIndex = 0;
			this.labelUnavailableMenuDisplayOption.Text = "Unavailable Menu Display Option";
			// 
			// pictureBoxView
			// 
			this.pictureBoxView.Location = new System.Drawing.Point(12, 10);
			this.pictureBoxView.Name = "pictureBoxView";
			this.pictureBoxView.Size = new System.Drawing.Size(502, 440);
			this.pictureBoxView.TabIndex = 1;
			this.pictureBoxView.TabStop = false;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.radioButtonHide);
			this.panel1.Controls.Add(this.radioButtonShow);
			this.panel1.Location = new System.Drawing.Point(6, 40);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(200, 28);
			this.panel1.TabIndex = 17;
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.radioButtonAll);
			this.panel2.Controls.Add(this.radioButtonNone);
			this.panel2.Location = new System.Drawing.Point(6, 104);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(200, 31);
			this.panel2.TabIndex = 0;
			// 
			// FormGraphViewContextMenu
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(901, 461);
			this.Controls.Add(this.pictureBoxView);
			this.Controls.Add(this.groupBoxContextMenu);
			this.Font = new System.Drawing.Font("굴림", 9F);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormGraphViewContextMenu";
			this.Text = "FormGraphViewContextMenu";
			this.groupBoxContextMenu.ResumeLayout(false);
			this.groupBoxContextMenu.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxView)).EndInit();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxContextMenu;
        private System.Windows.Forms.Label labelUnavailableMenuDisplayOptionvailableContextMenu;
        private System.Windows.Forms.Button buttonApply;
        private System.Windows.Forms.Label labelUnavailableMenuDisplayOption;
		private System.Windows.Forms.PictureBox pictureBoxView;
		private System.Windows.Forms.RadioButton radioButtonNone;
		private System.Windows.Forms.RadioButton radioButtonAll;
		private System.Windows.Forms.RadioButton radioButtonHide;
		private System.Windows.Forms.RadioButton radioButtonShow;
		private System.Windows.Forms.Panel panelAvailableContextMenu;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Panel panel1;
	}
}