﻿namespace SceneCommonTools.Forms
{
    partial class frmAddLyrFromMap
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
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSelAll = new System.Windows.Forms.Button();
            this.btnNoSelAll = new System.Windows.Forms.Button();
            this.lstLyrs = new System.Windows.Forms.CheckedListBox();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(120, 283);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(65, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "确定";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(191, 283);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(65, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSelAll
            // 
            this.btnSelAll.Location = new System.Drawing.Point(223, 12);
            this.btnSelAll.Name = "btnSelAll";
            this.btnSelAll.Size = new System.Drawing.Size(39, 23);
            this.btnSelAll.TabIndex = 3;
            this.btnSelAll.Text = "全选";
            this.btnSelAll.UseVisualStyleBackColor = true;
            this.btnSelAll.Click += new System.EventHandler(this.btnSelAll_Click);
            // 
            // btnNoSelAll
            // 
            this.btnNoSelAll.Location = new System.Drawing.Point(223, 41);
            this.btnNoSelAll.Name = "btnNoSelAll";
            this.btnNoSelAll.Size = new System.Drawing.Size(39, 23);
            this.btnNoSelAll.TabIndex = 4;
            this.btnNoSelAll.Text = "反选";
            this.btnNoSelAll.UseVisualStyleBackColor = true;
            this.btnNoSelAll.Click += new System.EventHandler(this.btnNoSelAll_Click);
            // 
            // lstLyrs
            // 
            this.lstLyrs.FormattingEnabled = true;
            this.lstLyrs.Location = new System.Drawing.Point(12, 12);
            this.lstLyrs.Name = "lstLyrs";
            this.lstLyrs.Size = new System.Drawing.Size(205, 260);
            this.lstLyrs.TabIndex = 5;
            // 
            // frmAddLyrFromMap
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(268, 318);
            this.Controls.Add(this.lstLyrs);
            this.Controls.Add(this.btnNoSelAll);
            this.Controls.Add(this.btnSelAll);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "frmAddLyrFromMap";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "选择图层";
            this.Load += new System.EventHandler(this.frmAddLyrFromMap_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSelAll;
        private System.Windows.Forms.Button btnNoSelAll;
        private System.Windows.Forms.CheckedListBox lstLyrs;
    }
}