namespace AssignDirectlyToAnalystBasic
{
    partial class AssignForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AssignForm));
            this.butCancel = new System.Windows.Forms.Button();
            this.butOK = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.textComment = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.comboAnalysts = new System.Windows.Forms.ComboBox();
            this.textDefault = new System.Windows.Forms.TextBox();
            this.checkBypassComment = new System.Windows.Forms.CheckBox();
            this.labelTier = new System.Windows.Forms.Label();
            this.comboTier = new System.Windows.Forms.ComboBox();
            this.comboTierGuids = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // butCancel
            // 
            this.butCancel.Location = new System.Drawing.Point(512, 222);
            this.butCancel.Name = "butCancel";
            this.butCancel.Size = new System.Drawing.Size(75, 23);
            this.butCancel.TabIndex = 5;
            this.butCancel.Text = "&Cancel";
            this.butCancel.UseVisualStyleBackColor = true;
            this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
            // 
            // butOK
            // 
            this.butOK.Enabled = false;
            this.butOK.Location = new System.Drawing.Point(431, 222);
            this.butOK.Name = "butOK";
            this.butOK.Size = new System.Drawing.Size(75, 23);
            this.butOK.TabIndex = 4;
            this.butOK.Text = "&OK";
            this.butOK.UseVisualStyleBackColor = true;
            this.butOK.Click += new System.EventHandler(this.butOK_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 73);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 18;
            this.label2.Text = "Comment:";
            // 
            // textComment
            // 
            this.textComment.Location = new System.Drawing.Point(12, 98);
            this.textComment.MaxLength = 1024;
            this.textComment.Multiline = true;
            this.textComment.Name = "textComment";
            this.textComment.Size = new System.Drawing.Size(578, 106);
            this.textComment.TabIndex = 2;
            this.textComment.TextChanged += new System.EventHandler(this.textComment_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 16;
            this.label1.Text = "Analyst:";
            // 
            // comboAnalysts
            // 
            this.comboAnalysts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboAnalysts.FormattingEnabled = true;
            this.comboAnalysts.Location = new System.Drawing.Point(12, 36);
            this.comboAnalysts.Name = "comboAnalysts";
            this.comboAnalysts.Size = new System.Drawing.Size(265, 21);
            this.comboAnalysts.Sorted = true;
            this.comboAnalysts.TabIndex = 0;
            this.comboAnalysts.SelectedIndexChanged += new System.EventHandler(this.comboAnalysts_SelectedIndexChanged);
            // 
            // textDefault
            // 
            this.textDefault.Location = new System.Drawing.Point(177, 224);
            this.textDefault.Name = "textDefault";
            this.textDefault.Size = new System.Drawing.Size(100, 20);
            this.textDefault.TabIndex = 21;
            this.textDefault.Visible = false;
            // 
            // checkBypassComment
            // 
            this.checkBypassComment.AutoSize = true;
            this.checkBypassComment.Location = new System.Drawing.Point(12, 226);
            this.checkBypassComment.Name = "checkBypassComment";
            this.checkBypassComment.Size = new System.Drawing.Size(133, 17);
            this.checkBypassComment.TabIndex = 3;
            this.checkBypassComment.Text = "No Comment Required";
            this.checkBypassComment.UseVisualStyleBackColor = true;
            this.checkBypassComment.CheckedChanged += new System.EventHandler(this.checkBypassComment_CheckedChanged);
            // 
            // labelTier
            // 
            this.labelTier.AutoSize = true;
            this.labelTier.Location = new System.Drawing.Point(321, 18);
            this.labelTier.Name = "labelTier";
            this.labelTier.Size = new System.Drawing.Size(28, 13);
            this.labelTier.TabIndex = 22;
            this.labelTier.Text = "Tier:";
            this.labelTier.Visible = false;
            // 
            // comboTier
            // 
            this.comboTier.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboTier.FormattingEnabled = true;
            this.comboTier.Location = new System.Drawing.Point(324, 36);
            this.comboTier.Name = "comboTier";
            this.comboTier.Size = new System.Drawing.Size(265, 21);
            this.comboTier.TabIndex = 1;
            this.comboTier.Visible = false;
            this.comboTier.KeyUp += new System.Windows.Forms.KeyEventHandler(this.comboTier_KeyUp);
            // 
            // comboTierGuids
            // 
            this.comboTierGuids.FormattingEnabled = true;
            this.comboTierGuids.Location = new System.Drawing.Point(294, 224);
            this.comboTierGuids.Name = "comboTierGuids";
            this.comboTierGuids.Size = new System.Drawing.Size(121, 21);
            this.comboTierGuids.TabIndex = 24;
            this.comboTierGuids.Visible = false;
            // 
            // AssignForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(601, 261);
            this.Controls.Add(this.comboTierGuids);
            this.Controls.Add(this.comboTier);
            this.Controls.Add(this.labelTier);
            this.Controls.Add(this.checkBypassComment);
            this.Controls.Add(this.textDefault);
            this.Controls.Add(this.butCancel);
            this.Controls.Add(this.butOK);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textComment);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboAnalysts);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AssignForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Assign Incident Directly To Analyst";
            this.Load += new System.EventHandler(this.AssignForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button butCancel;
        private System.Windows.Forms.Button butOK;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.TextBox textComment;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.ComboBox comboAnalysts;
        public System.Windows.Forms.TextBox textDefault;
        public System.Windows.Forms.CheckBox checkBypassComment;
        private System.Windows.Forms.Label labelTier;
        public System.Windows.Forms.ComboBox comboTier;
        public System.Windows.Forms.ComboBox comboTierGuids;
    }
}