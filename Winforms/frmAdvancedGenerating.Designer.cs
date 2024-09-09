namespace CodeGen
{
    partial class frmAdvancedGenerating
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
            this.btnGenerateBL = new System.Windows.Forms.Button();
            this.btnGenerateDAL = new System.Windows.Forms.Button();
            this.txtFolderPath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.pbSelectFolder = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pbMinimizeForm = new System.Windows.Forms.PictureBox();
            this.pbCloseForm = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.lblSelectedDatabase = new System.Windows.Forms.Label();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            ((System.ComponentModel.ISupportInitialize)(this.pbSelectFolder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbMinimizeForm)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbCloseForm)).BeginInit();
            this.SuspendLayout();
            // 
            // btnGenerateBL
            // 
            this.btnGenerateBL.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(62)))), ((int)(((byte)(66)))));
            this.btnGenerateBL.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGenerateBL.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(238)))), ((int)(((byte)(238)))));
            this.btnGenerateBL.Location = new System.Drawing.Point(264, 205);
            this.btnGenerateBL.Name = "btnGenerateBL";
            this.btnGenerateBL.Size = new System.Drawing.Size(240, 40);
            this.btnGenerateBL.TabIndex = 9;
            this.btnGenerateBL.Text = "Generate Business Layer";
            this.btnGenerateBL.UseVisualStyleBackColor = false;
            this.btnGenerateBL.Click += new System.EventHandler(this.btnGenerateBL_Click);
            // 
            // btnGenerateDAL
            // 
            this.btnGenerateDAL.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(62)))), ((int)(((byte)(66)))));
            this.btnGenerateDAL.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGenerateDAL.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(238)))), ((int)(((byte)(238)))));
            this.btnGenerateDAL.Location = new System.Drawing.Point(18, 205);
            this.btnGenerateDAL.Name = "btnGenerateDAL";
            this.btnGenerateDAL.Size = new System.Drawing.Size(240, 40);
            this.btnGenerateDAL.TabIndex = 9;
            this.btnGenerateDAL.Text = "Generate Data Access Layer";
            this.btnGenerateDAL.UseVisualStyleBackColor = false;
            this.btnGenerateDAL.Click += new System.EventHandler(this.btnGenerateDAL_Click);
            // 
            // txtFolderPath
            // 
            this.txtFolderPath.Location = new System.Drawing.Point(18, 150);
            this.txtFolderPath.Name = "txtFolderPath";
            this.txtFolderPath.ReadOnly = true;
            this.txtFolderPath.Size = new System.Drawing.Size(442, 27);
            this.txtFolderPath.TabIndex = 10;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 127);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(102, 20);
            this.label1.TabIndex = 11;
            this.label1.Text = "Select Folder:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI Semibold", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(57, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(220, 30);
            this.label2.TabIndex = 11;
            this.label2.Text = "Advanced Generating";
            // 
            // pbSelectFolder
            // 
            this.pbSelectFolder.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbSelectFolder.Image = global::CodeGen.Properties.Resources.open_folder;
            this.pbSelectFolder.Location = new System.Drawing.Point(471, 150);
            this.pbSelectFolder.Name = "pbSelectFolder";
            this.pbSelectFolder.Size = new System.Drawing.Size(33, 29);
            this.pbSelectFolder.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbSelectFolder.TabIndex = 7;
            this.pbSelectFolder.TabStop = false;
            this.pbSelectFolder.Click += new System.EventHandler(this.pbSelectFolder_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::CodeGen.Properties.Resources.thunder;
            this.pictureBox2.Location = new System.Drawing.Point(18, 12);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(33, 29);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 7;
            this.pictureBox2.TabStop = false;
            // 
            // pbMinimizeForm
            // 
            this.pbMinimizeForm.Image = global::CodeGen.Properties.Resources.minus;
            this.pbMinimizeForm.Location = new System.Drawing.Point(427, 13);
            this.pbMinimizeForm.Name = "pbMinimizeForm";
            this.pbMinimizeForm.Size = new System.Drawing.Size(33, 29);
            this.pbMinimizeForm.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbMinimizeForm.TabIndex = 7;
            this.pbMinimizeForm.TabStop = false;
            this.pbMinimizeForm.Click += new System.EventHandler(this.pbMinimizeForm_Click);
            // 
            // pbCloseForm
            // 
            this.pbCloseForm.Image = global::CodeGen.Properties.Resources.remove;
            this.pbCloseForm.Location = new System.Drawing.Point(471, 13);
            this.pbCloseForm.Name = "pbCloseForm";
            this.pbCloseForm.Size = new System.Drawing.Size(33, 29);
            this.pbCloseForm.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbCloseForm.TabIndex = 8;
            this.pbCloseForm.TabStop = false;
            this.pbCloseForm.Click += new System.EventHandler(this.pbCloseForm_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 61);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(76, 20);
            this.label3.TabIndex = 11;
            this.label3.Text = "Database:";
            // 
            // lblSelectedDatabase
            // 
            this.lblSelectedDatabase.AutoSize = true;
            this.lblSelectedDatabase.Font = new System.Drawing.Font("Segoe UI Semibold", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSelectedDatabase.Location = new System.Drawing.Point(13, 81);
            this.lblSelectedDatabase.Name = "lblSelectedDatabase";
            this.lblSelectedDatabase.Size = new System.Drawing.Size(169, 25);
            this.lblSelectedDatabase.TabIndex = 11;
            this.lblSelectedDatabase.Text = "Selected Database";
            // 
            // frmAdvancedGenerating
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(62)))), ((int)(((byte)(66)))));
            this.ClientSize = new System.Drawing.Size(528, 273);
            this.Controls.Add(this.lblSelectedDatabase);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtFolderPath);
            this.Controls.Add(this.pbSelectFolder);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pbMinimizeForm);
            this.Controls.Add(this.pbCloseForm);
            this.Controls.Add(this.btnGenerateDAL);
            this.Controls.Add(this.btnGenerateBL);
            this.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(238)))), ((int)(((byte)(238)))));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "frmAdvancedGenerating";
            this.Opacity = 0.95D;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmAdvancedGenerating";
            this.Load += new System.EventHandler(this.frmAdvancedGenerating_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbSelectFolder)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbMinimizeForm)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbCloseForm)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pbMinimizeForm;
        private System.Windows.Forms.PictureBox pbCloseForm;
        private System.Windows.Forms.Button btnGenerateBL;
        private System.Windows.Forms.Button btnGenerateDAL;
        private System.Windows.Forms.TextBox txtFolderPath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pbSelectFolder;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblSelectedDatabase;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
    }
}