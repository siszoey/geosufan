﻿namespace GeoPageLayout
{
    partial class FrmSheetMapSetExtent_ZT
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
            this.label1 = new DevComponents.DotNetBar.LabelX();
            this.btnOK = new DevComponents.DotNetBar.ButtonX();
            this.btnCancel = new DevComponents.DotNetBar.ButtonX();
            this.cBoxScale = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.Item10000 = new DevComponents.Editors.ComboItem();
            this.Item25000 = new DevComponents.Editors.ComboItem();
            this.Item50000 = new DevComponents.Editors.ComboItem();
            this.txtMapNO = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.label4 = new System.Windows.Forms.Label();
            this.txtTitle = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.cBoxSecret = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.comboItem1 = new DevComponents.Editors.ComboItem();
            this.comboItem9 = new DevComponents.Editors.ComboItem();
            this.comboItem10 = new DevComponents.Editors.ComboItem();
            this.comboItem11 = new DevComponents.Editors.ComboItem();
            this.txtJTBWN = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.txtJTBN = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.txtJTBEN = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.txtJTBE = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.txtJTBWS = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.txtJTBS = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.txtJTBES = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.txtJTBW = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.txtTime = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.txtCartoGroup = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.cBoxCoordinate = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.comboItem12 = new DevComponents.Editors.ComboItem();
            this.cBoxElevation = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.groupPanel1 = new DevComponents.DotNetBar.Controls.GroupPanel();
            this.txtSubTitle = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.btnTFH = new DevComponents.DotNetBar.ButtonX();
            this.cBoxHasBootLine = new System.Windows.Forms.CheckBox();
            this.btnShowPgSym = new DevComponents.DotNetBar.ButtonX();
            this.btnSetLabel = new DevComponents.DotNetBar.ButtonX();
            this.cBoxSubHead = new System.Windows.Forms.CheckBox();
            this.chkBoxSecurity = new System.Windows.Forms.CheckBox();
            this.checkBoxTuli = new System.Windows.Forms.CheckBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.txtContourIntvl = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.groupPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Location = new System.Drawing.Point(305, 262);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(19, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "1:";
            // 
            // btnOK
            // 
            this.btnOK.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnOK.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(438, 360);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "确定";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnCancel.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnCancel.Location = new System.Drawing.Point(519, 360);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "取消";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // cBoxScale
            // 
            this.cBoxScale.DisplayMember = "Text";
            this.cBoxScale.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cBoxScale.Enabled = false;
            this.cBoxScale.FormattingEnabled = true;
            this.cBoxScale.ItemHeight = 15;
            this.cBoxScale.Items.AddRange(new object[] {
            this.Item10000,
            this.Item25000,
            this.Item50000});
            this.cBoxScale.Location = new System.Drawing.Point(328, 260);
            this.cBoxScale.Name = "cBoxScale";
            this.cBoxScale.Size = new System.Drawing.Size(121, 21);
            this.cBoxScale.TabIndex = 4;
            this.cBoxScale.Text = "10000";
            this.cBoxScale.SelectedIndexChanged += new System.EventHandler(this.cBoxScale_SelectedIndexChanged);
            // 
            // Item10000
            // 
            this.Item10000.Text = "10000";
            // 
            // Item25000
            // 
            this.Item25000.Text = "25000";
            // 
            // Item50000
            // 
            this.Item50000.Text = "50000";
            // 
            // txtMapNO
            // 
            // 
            // 
            // 
            this.txtMapNO.Border.Class = "TextBoxBorder";
            this.txtMapNO.Location = new System.Drawing.Point(316, 57);
            this.txtMapNO.Name = "txtMapNO";
            this.txtMapNO.Size = new System.Drawing.Size(142, 21);
            this.txtMapNO.TabIndex = 2;
            this.txtMapNO.Text = "F50E006001";
            this.txtMapNO.TextChanged += new System.EventHandler(this.txtMapNO_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Location = new System.Drawing.Point(253, 10);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 12);
            this.label4.TabIndex = 8;
            this.label4.Text = "图  名:";
            // 
            // txtTitle
            // 
            // 
            // 
            // 
            this.txtTitle.Border.Class = "TextBoxBorder";
            this.txtTitle.Location = new System.Drawing.Point(316, 6);
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.Size = new System.Drawing.Size(142, 21);
            this.txtTitle.TabIndex = 7;
            this.txtTitle.Text = "森林资源现状图";
            // 
            // cBoxSecret
            // 
            this.cBoxSecret.DisplayMember = "Text";
            this.cBoxSecret.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cBoxSecret.FormattingEnabled = true;
            this.cBoxSecret.ItemHeight = 15;
            this.cBoxSecret.Items.AddRange(new object[] {
            this.comboItem1,
            this.comboItem9,
            this.comboItem10,
            this.comboItem11});
            this.cBoxSecret.Location = new System.Drawing.Point(510, 57);
            this.cBoxSecret.Name = "cBoxSecret";
            this.cBoxSecret.Size = new System.Drawing.Size(75, 21);
            this.cBoxSecret.TabIndex = 11;
            this.cBoxSecret.Text = "内部用图";
            // 
            // comboItem1
            // 
            this.comboItem1.Text = "内部用图";
            // 
            // comboItem9
            // 
            this.comboItem9.Text = "秘密";
            // 
            // comboItem10
            // 
            this.comboItem10.Text = "机密";
            // 
            // comboItem11
            // 
            this.comboItem11.Text = "绝密";
            // 
            // txtJTBWN
            // 
            // 
            // 
            // 
            this.txtJTBWN.Border.Class = "TextBoxBorder";
            this.txtJTBWN.Location = new System.Drawing.Point(29, 15);
            this.txtJTBWN.Name = "txtJTBWN";
            this.txtJTBWN.Size = new System.Drawing.Size(70, 21);
            this.txtJTBWN.TabIndex = 12;
            this.txtJTBWN.Text = "F50E005024";
            // 
            // txtJTBN
            // 
            // 
            // 
            // 
            this.txtJTBN.Border.Class = "TextBoxBorder";
            this.txtJTBN.Location = new System.Drawing.Point(99, 15);
            this.txtJTBN.Name = "txtJTBN";
            this.txtJTBN.Size = new System.Drawing.Size(70, 21);
            this.txtJTBN.TabIndex = 13;
            // 
            // txtJTBEN
            // 
            // 
            // 
            // 
            this.txtJTBEN.Border.Class = "TextBoxBorder";
            this.txtJTBEN.Location = new System.Drawing.Point(169, 15);
            this.txtJTBEN.Name = "txtJTBEN";
            this.txtJTBEN.Size = new System.Drawing.Size(70, 21);
            this.txtJTBEN.TabIndex = 14;
            // 
            // txtJTBE
            // 
            // 
            // 
            // 
            this.txtJTBE.Border.Class = "TextBoxBorder";
            this.txtJTBE.Location = new System.Drawing.Point(169, 36);
            this.txtJTBE.Name = "txtJTBE";
            this.txtJTBE.Size = new System.Drawing.Size(70, 21);
            this.txtJTBE.TabIndex = 15;
            // 
            // txtJTBWS
            // 
            // 
            // 
            // 
            this.txtJTBWS.Border.Class = "TextBoxBorder";
            this.txtJTBWS.Location = new System.Drawing.Point(29, 57);
            this.txtJTBWS.Name = "txtJTBWS";
            this.txtJTBWS.Size = new System.Drawing.Size(70, 21);
            this.txtJTBWS.TabIndex = 16;
            // 
            // txtJTBS
            // 
            // 
            // 
            // 
            this.txtJTBS.Border.Class = "TextBoxBorder";
            this.txtJTBS.Location = new System.Drawing.Point(99, 57);
            this.txtJTBS.Name = "txtJTBS";
            this.txtJTBS.Size = new System.Drawing.Size(70, 21);
            this.txtJTBS.TabIndex = 17;
            // 
            // txtJTBES
            // 
            // 
            // 
            // 
            this.txtJTBES.Border.Class = "TextBoxBorder";
            this.txtJTBES.Location = new System.Drawing.Point(169, 57);
            this.txtJTBES.Name = "txtJTBES";
            this.txtJTBES.Size = new System.Drawing.Size(70, 21);
            this.txtJTBES.TabIndex = 18;
            // 
            // txtJTBW
            // 
            // 
            // 
            // 
            this.txtJTBW.Border.Class = "TextBoxBorder";
            this.txtJTBW.Location = new System.Drawing.Point(29, 36);
            this.txtJTBW.Name = "txtJTBW";
            this.txtJTBW.Size = new System.Drawing.Size(70, 21);
            this.txtJTBW.TabIndex = 19;
            // 
            // txtTime
            // 
            // 
            // 
            // 
            this.txtTime.Border.Class = "TextBoxBorder";
            this.txtTime.Location = new System.Drawing.Point(26, 260);
            this.txtTime.Name = "txtTime";
            this.txtTime.Size = new System.Drawing.Size(200, 21);
            this.txtTime.TabIndex = 20;
            this.txtTime.Text = "XXXX年X月航摄，XXXX年X月调绘";
            // 
            // txtCartoGroup
            // 
            // 
            // 
            // 
            this.txtCartoGroup.Border.Class = "TextBoxBorder";
            this.txtCartoGroup.Location = new System.Drawing.Point(3, 139);
            this.txtCartoGroup.Multiline = true;
            this.txtCartoGroup.Name = "txtCartoGroup";
            this.txtCartoGroup.Size = new System.Drawing.Size(20, 109);
            this.txtCartoGroup.TabIndex = 22;
            this.txtCartoGroup.Text = "XX市林业资源局";
            // 
            // cBoxCoordinate
            // 
            this.cBoxCoordinate.DisplayMember = "Text";
            this.cBoxCoordinate.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cBoxCoordinate.FormattingEnabled = true;
            this.cBoxCoordinate.ItemHeight = 15;
            this.cBoxCoordinate.Items.AddRange(new object[] {
            this.comboItem12});
            this.cBoxCoordinate.Location = new System.Drawing.Point(26, 281);
            this.cBoxCoordinate.Name = "cBoxCoordinate";
            this.cBoxCoordinate.Size = new System.Drawing.Size(200, 21);
            this.cBoxCoordinate.TabIndex = 24;
            this.cBoxCoordinate.Text = "1980年西安坐标系";
            // 
            // comboItem12
            // 
            this.comboItem12.Text = "西安80,高斯-克吕格投影";
            // 
            // cBoxElevation
            // 
            this.cBoxElevation.DisplayMember = "Text";
            this.cBoxElevation.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cBoxElevation.FormattingEnabled = true;
            this.cBoxElevation.ItemHeight = 15;
            this.cBoxElevation.Location = new System.Drawing.Point(26, 302);
            this.cBoxElevation.Name = "cBoxElevation";
            this.cBoxElevation.Size = new System.Drawing.Size(117, 21);
            this.cBoxElevation.TabIndex = 25;
            this.cBoxElevation.Text = "1985国家高程基准";
            // 
            // groupPanel1
            // 
            this.groupPanel1.CanvasColor = System.Drawing.SystemColors.Control;
            this.groupPanel1.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.groupPanel1.Controls.Add(this.txtSubTitle);
            this.groupPanel1.Controls.Add(this.btnTFH);
            this.groupPanel1.Controls.Add(this.cBoxHasBootLine);
            this.groupPanel1.Controls.Add(this.btnShowPgSym);
            this.groupPanel1.Controls.Add(this.btnSetLabel);
            this.groupPanel1.Controls.Add(this.cBoxSubHead);
            this.groupPanel1.Controls.Add(this.chkBoxSecurity);
            this.groupPanel1.Controls.Add(this.checkBoxTuli);
            this.groupPanel1.Controls.Add(this.pictureBox2);
            this.groupPanel1.Controls.Add(this.label10);
            this.groupPanel1.Controls.Add(this.label9);
            this.groupPanel1.Controls.Add(this.txtContourIntvl);
            this.groupPanel1.Controls.Add(this.label1);
            this.groupPanel1.Controls.Add(this.txtMapNO);
            this.groupPanel1.Controls.Add(this.cBoxElevation);
            this.groupPanel1.Controls.Add(this.cBoxScale);
            this.groupPanel1.Controls.Add(this.cBoxCoordinate);
            this.groupPanel1.Controls.Add(this.txtTime);
            this.groupPanel1.Controls.Add(this.txtJTBW);
            this.groupPanel1.Controls.Add(this.txtJTBES);
            this.groupPanel1.Controls.Add(this.txtCartoGroup);
            this.groupPanel1.Controls.Add(this.txtJTBS);
            this.groupPanel1.Controls.Add(this.txtTitle);
            this.groupPanel1.Controls.Add(this.txtJTBWS);
            this.groupPanel1.Controls.Add(this.label4);
            this.groupPanel1.Controls.Add(this.txtJTBE);
            this.groupPanel1.Controls.Add(this.txtJTBEN);
            this.groupPanel1.Controls.Add(this.cBoxSecret);
            this.groupPanel1.Controls.Add(this.txtJTBN);
            this.groupPanel1.Controls.Add(this.txtJTBWN);
            this.groupPanel1.Location = new System.Drawing.Point(0, 0);
            this.groupPanel1.Name = "groupPanel1";
            this.groupPanel1.Size = new System.Drawing.Size(596, 354);
            // 
            // 
            // 
            this.groupPanel1.Style.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.groupPanel1.Style.BackColorGradientAngle = 90;
            this.groupPanel1.Style.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.groupPanel1.Style.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel1.Style.BorderBottomWidth = 1;
            this.groupPanel1.Style.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.groupPanel1.Style.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel1.Style.BorderLeftWidth = 1;
            this.groupPanel1.Style.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel1.Style.BorderRightWidth = 1;
            this.groupPanel1.Style.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.groupPanel1.Style.BorderTopWidth = 1;
            this.groupPanel1.Style.CornerDiameter = 4;
            this.groupPanel1.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded;
            this.groupPanel1.Style.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Center;
            this.groupPanel1.Style.TextColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.groupPanel1.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near;
            this.groupPanel1.TabIndex = 28;
            this.groupPanel1.Text = "地图信息";
            this.groupPanel1.Click += new System.EventHandler(this.groupPanel1_Click);
            // 
            // txtSubTitle
            // 
            // 
            // 
            // 
            this.txtSubTitle.Border.Class = "TextBoxBorder";
            this.txtSubTitle.Location = new System.Drawing.Point(316, 33);
            this.txtSubTitle.Name = "txtSubTitle";
            this.txtSubTitle.Size = new System.Drawing.Size(142, 21);
            this.txtSubTitle.TabIndex = 53;
            // 
            // btnTFH
            // 
            this.btnTFH.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnTFH.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnTFH.Location = new System.Drawing.Point(255, 57);
            this.btnTFH.Name = "btnTFH";
            this.btnTFH.Size = new System.Drawing.Size(55, 22);
            this.btnTFH.TabIndex = 52;
            this.btnTFH.Text = "旧图幅号";
            this.btnTFH.Click += new System.EventHandler(this.btnTFH_Click);
            // 
            // cBoxHasBootLine
            // 
            this.cBoxHasBootLine.AutoSize = true;
            this.cBoxHasBootLine.BackColor = System.Drawing.Color.Transparent;
            this.cBoxHasBootLine.Checked = true;
            this.cBoxHasBootLine.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cBoxHasBootLine.Location = new System.Drawing.Point(430, 304);
            this.cBoxHasBootLine.Name = "cBoxHasBootLine";
            this.cBoxHasBootLine.Size = new System.Drawing.Size(60, 16);
            this.cBoxHasBootLine.TabIndex = 51;
            this.cBoxHasBootLine.Text = "加引线";
            this.cBoxHasBootLine.UseVisualStyleBackColor = false;
            this.cBoxHasBootLine.CheckedChanged += new System.EventHandler(this.cBoxHasBootLine_CheckedChanged);
            // 
            // btnShowPgSym
            // 
            this.btnShowPgSym.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnShowPgSym.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnShowPgSym.Location = new System.Drawing.Point(496, 276);
            this.btnShowPgSym.Name = "btnShowPgSym";
            this.btnShowPgSym.Size = new System.Drawing.Size(75, 22);
            this.btnShowPgSym.TabIndex = 50;
            this.btnShowPgSym.Text = "显示面符号";
            this.btnShowPgSym.Click += new System.EventHandler(this.btnShowPgSym_Click);
            // 
            // btnSetLabel
            // 
            this.btnSetLabel.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnSetLabel.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnSetLabel.Location = new System.Drawing.Point(496, 301);
            this.btnSetLabel.Name = "btnSetLabel";
            this.btnSetLabel.Size = new System.Drawing.Size(75, 22);
            this.btnSetLabel.TabIndex = 49;
            this.btnSetLabel.Text = "设置标注";
            this.btnSetLabel.Click += new System.EventHandler(this.btnSetLabel_Click);
            // 
            // cBoxSubHead
            // 
            this.cBoxSubHead.AutoSize = true;
            this.cBoxSubHead.BackColor = System.Drawing.Color.Transparent;
            this.cBoxSubHead.Location = new System.Drawing.Point(250, 38);
            this.cBoxSubHead.Name = "cBoxSubHead";
            this.cBoxSubHead.Size = new System.Drawing.Size(60, 16);
            this.cBoxSubHead.TabIndex = 48;
            this.cBoxSubHead.Text = "副标题";
            this.cBoxSubHead.UseVisualStyleBackColor = false;
            this.cBoxSubHead.CheckedChanged += new System.EventHandler(this.cBoxSubHead_CheckedChanged);
            // 
            // chkBoxSecurity
            // 
            this.chkBoxSecurity.AutoSize = true;
            this.chkBoxSecurity.BackColor = System.Drawing.Color.Transparent;
            this.chkBoxSecurity.Checked = true;
            this.chkBoxSecurity.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBoxSecurity.Location = new System.Drawing.Point(462, 61);
            this.chkBoxSecurity.Name = "chkBoxSecurity";
            this.chkBoxSecurity.Size = new System.Drawing.Size(48, 16);
            this.chkBoxSecurity.TabIndex = 46;
            this.chkBoxSecurity.Text = "密级";
            this.chkBoxSecurity.UseVisualStyleBackColor = false;
            this.chkBoxSecurity.CheckedChanged += new System.EventHandler(this.chkBoxSecurity_CheckedChanged);
            // 
            // checkBoxTuli
            // 
            this.checkBoxTuli.AutoSize = true;
            this.checkBoxTuli.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxTuli.Checked = true;
            this.checkBoxTuli.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxTuli.Location = new System.Drawing.Point(500, 254);
            this.checkBoxTuli.Name = "checkBoxTuli";
            this.checkBoxTuli.Size = new System.Drawing.Size(72, 16);
            this.checkBoxTuli.TabIndex = 44;
            this.checkBoxTuli.Text = "生成图例";
            this.checkBoxTuli.UseVisualStyleBackColor = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::GeoPageLayout.Properties.Resources.GeoPageLayout_gzdt;
            this.pictureBox2.Location = new System.Drawing.Point(29, 85);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(542, 163);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox2.TabIndex = 33;
            this.pictureBox2.TabStop = false;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.BackColor = System.Drawing.Color.Transparent;
            this.label10.Location = new System.Drawing.Point(232, 306);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(17, 12);
            this.label10.TabIndex = 32;
            this.label10.Text = "米";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.BackColor = System.Drawing.Color.Transparent;
            this.label9.Location = new System.Drawing.Point(146, 306);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(53, 12);
            this.label9.TabIndex = 31;
            this.label9.Text = "等高距为";
            // 
            // txtContourIntvl
            // 
            // 
            // 
            // 
            this.txtContourIntvl.Border.Class = "TextBoxBorder";
            this.txtContourIntvl.Location = new System.Drawing.Point(199, 302);
            this.txtContourIntvl.Name = "txtContourIntvl";
            this.txtContourIntvl.Size = new System.Drawing.Size(27, 21);
            this.txtContourIntvl.TabIndex = 30;
            this.txtContourIntvl.Text = "1";
            // 
            // FrmSheetMapSetExtent_ZT
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(596, 393);
            this.Controls.Add(this.groupPanel1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmSheetMapSetExtent_ZT";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "森林资源现状分幅图";
            this.Load += new System.EventHandler(this.FrmSheetMapSet_ZT_Load);
            this.groupPanel1.ResumeLayout(false);
            this.groupPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.LabelX label1;
        private DevComponents.DotNetBar.ButtonX btnOK;
        private DevComponents.DotNetBar.ButtonX btnCancel;
        private DevComponents.DotNetBar.Controls.TextBoxX txtMapNO;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cBoxScale;
        private DevComponents.Editors.ComboItem Item10000;
        private System.Windows.Forms.Label label4;
        private DevComponents.DotNetBar.Controls.TextBoxX txtTitle;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cBoxSecret;
        private DevComponents.Editors.ComboItem comboItem9;
        private DevComponents.Editors.ComboItem comboItem10;
        private DevComponents.Editors.ComboItem comboItem11;
        private DevComponents.DotNetBar.Controls.TextBoxX txtJTBWN;
        private DevComponents.DotNetBar.Controls.TextBoxX txtJTBN;
        private DevComponents.DotNetBar.Controls.TextBoxX txtJTBEN;
        private DevComponents.DotNetBar.Controls.TextBoxX txtJTBE;
        private DevComponents.DotNetBar.Controls.TextBoxX txtJTBWS;
        private DevComponents.DotNetBar.Controls.TextBoxX txtJTBS;
        private DevComponents.DotNetBar.Controls.TextBoxX txtJTBES;
        private DevComponents.DotNetBar.Controls.TextBoxX txtJTBW;
        private DevComponents.DotNetBar.Controls.TextBoxX txtTime;
        private DevComponents.DotNetBar.Controls.TextBoxX txtCartoGroup;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cBoxCoordinate;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cBoxElevation;
        private DevComponents.DotNetBar.Controls.GroupPanel groupPanel1;
        private System.Windows.Forms.Label label9;
        private DevComponents.DotNetBar.Controls.TextBoxX txtContourIntvl;
        private DevComponents.Editors.ComboItem comboItem12;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.PictureBox pictureBox2;
        private DevComponents.Editors.ComboItem Item25000;
        private DevComponents.Editors.ComboItem Item50000;
        private System.Windows.Forms.CheckBox checkBoxTuli;
        private DevComponents.Editors.ComboItem comboItem1;
        private System.Windows.Forms.CheckBox chkBoxSecurity;
        private System.Windows.Forms.CheckBox cBoxSubHead;
        private DevComponents.DotNetBar.ButtonX btnShowPgSym;
        private DevComponents.DotNetBar.ButtonX btnSetLabel;
        private System.Windows.Forms.CheckBox cBoxHasBootLine;
        private DevComponents.DotNetBar.ButtonX btnTFH;
        private DevComponents.DotNetBar.Controls.TextBoxX txtSubTitle;
    }
}