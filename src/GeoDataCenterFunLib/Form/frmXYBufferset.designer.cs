﻿namespace GeoDataCenterFunLib
{
    partial class frmXYBufferset
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
            this.groupBoxLocation = new System.Windows.Forms.GroupBox();
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.textBoxY = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.textBoxX = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.labelY = new DevComponents.DotNetBar.LabelX();
            this.labelX = new DevComponents.DotNetBar.LabelX();
            this.buttonOK = new DevComponents.DotNetBar.ButtonX();
            this.buttonCancel = new DevComponents.DotNetBar.ButtonX();
            this.Error_Lable = new System.Windows.Forms.Label();
            this.groupBoxLocation.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxLocation
            // 
            this.groupBoxLocation.Controls.Add(this.labelX1);
            this.groupBoxLocation.Controls.Add(this.textBoxY);
            this.groupBoxLocation.Controls.Add(this.textBoxX);
            this.groupBoxLocation.Controls.Add(this.labelY);
            this.groupBoxLocation.Controls.Add(this.labelX);
            this.groupBoxLocation.Location = new System.Drawing.Point(12, 12);
            this.groupBoxLocation.Name = "groupBoxLocation";
            this.groupBoxLocation.Size = new System.Drawing.Size(353, 111);
            this.groupBoxLocation.TabIndex = 0;
            this.groupBoxLocation.TabStop = false;
            this.groupBoxLocation.Text = "请输入缓冲点坐标";
            // 
            // labelX1
            // 
            this.labelX1.Location = new System.Drawing.Point(10, 24);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(338, 15);
            this.labelX1.TabIndex = 4;
            this.labelX1.Text = "中国地理范围:(东经73.66°-135.04° 北纬3.86°-53.55°)";
            // 
            // textBoxY
            // 
            // 
            // 
            // 
            this.textBoxY.Border.Class = "TextBoxBorder";
            this.textBoxY.Location = new System.Drawing.Point(48, 78);
            this.textBoxY.Name = "textBoxY";
            this.textBoxY.Size = new System.Drawing.Size(286, 21);
            this.textBoxY.TabIndex = 2;
            this.textBoxY.Text = "0";
            this.textBoxY.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmXYLocation_KeyDown);
            this.textBoxY.TextChanged += new System.EventHandler(this.textBoxY_TextChanged);
            // 
            // textBoxX
            // 
            // 
            // 
            // 
            this.textBoxX.Border.Class = "TextBoxBorder";
            this.textBoxX.Location = new System.Drawing.Point(48, 48);
            this.textBoxX.Name = "textBoxX";
            this.textBoxX.Size = new System.Drawing.Size(286, 21);
            this.textBoxX.TabIndex = 1;
            this.textBoxX.Text = "0";
            this.textBoxX.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmXYLocation_KeyDown);
            this.textBoxX.TextChanged += new System.EventHandler(this.textBoxX_TextChanged);
            // 
            // labelY
            // 
            this.labelY.Location = new System.Drawing.Point(11, 78);
            this.labelY.Name = "labelY";
            this.labelY.Size = new System.Drawing.Size(48, 23);
            this.labelY.TabIndex = 0;
            this.labelY.Text = "纬度:";
            // 
            // labelX
            // 
            this.labelX.Location = new System.Drawing.Point(11, 49);
            this.labelX.Name = "labelX";
            this.labelX.Size = new System.Drawing.Size(48, 23);
            this.labelX.TabIndex = 0;
            this.labelX.Text = "经度:";
            // 
            // buttonOK
            // 
            this.buttonOK.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.buttonOK.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.buttonOK.Location = new System.Drawing.Point(210, 136);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 3;
            this.buttonOK.Text = "确定";
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.buttonCancel.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.buttonCancel.Location = new System.Drawing.Point(291, 136);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "取消";
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // Error_Lable
            // 
            this.Error_Lable.AutoSize = true;
            this.Error_Lable.Location = new System.Drawing.Point(12, 138);
            this.Error_Lable.Name = "Error_Lable";
            this.Error_Lable.Size = new System.Drawing.Size(41, 12);
            this.Error_Lable.TabIndex = 9;
            this.Error_Lable.Text = "label1";
            this.Error_Lable.Visible = false;
            // 
            // frmXYBufferset
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(377, 164);
            this.Controls.Add(this.Error_Lable);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.groupBoxLocation);
            this.DoubleBuffered = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmXYBufferset";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "输入坐标缓冲查询";
            this.Load += new System.EventHandler(this.frmXYBufferset_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmXYLocation_FormClosed);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmXYLocation_KeyDown);
            this.groupBoxLocation.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxLocation;
        private DevComponents.DotNetBar.Controls.TextBoxX textBoxY;
        private DevComponents.DotNetBar.Controls.TextBoxX textBoxX;
        private DevComponents.DotNetBar.LabelX labelY;
        private DevComponents.DotNetBar.LabelX labelX;
        private DevComponents.DotNetBar.ButtonX buttonOK;
        private DevComponents.DotNetBar.ButtonX buttonCancel;
        private System.Windows.Forms.Label Error_Lable;
        private DevComponents.DotNetBar.LabelX labelX1;
    }
}