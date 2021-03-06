﻿namespace GeoDataEdit
{
    partial class frmAttributeEdit
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.barTop = new DevComponents.DotNetBar.Bar();
            this.labelItem = new DevComponents.DotNetBar.LabelItem();
            this.comboBoxItem = new DevComponents.DotNetBar.ComboBoxItem();
            this.comboItemTopLay = new DevComponents.Editors.ComboItem();
            this.comboItemVisibleLay = new DevComponents.Editors.ComboItem();
            this.comboItemSelectableLay = new DevComponents.Editors.ComboItem();
            this.comboItemAllLay = new DevComponents.Editors.ComboItem();
            this.barButtom = new DevComponents.DotNetBar.Bar();
            this.labelItemMemo = new DevComponents.DotNetBar.LabelItem();
            this.progressBarItem = new DevComponents.DotNetBar.ProgressBarItem();
            this.labelEditStatus = new DevComponents.DotNetBar.LabelItem();
            this.itemPanel1 = new DevComponents.DotNetBar.ItemPanel();
            this.dataGridViewX = new DevComponents.DotNetBar.Controls.DataGridViewX();
            this.advTree = new DevComponents.AdvTree.AdvTree();
            this.nodeConnector1 = new DevComponents.AdvTree.NodeConnector();
            this.elementStyle1 = new DevComponents.DotNetBar.ElementStyle();
            ((System.ComponentModel.ISupportInitialize)(this.barTop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barButtom)).BeginInit();
            this.itemPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.advTree)).BeginInit();
            this.SuspendLayout();
            // 
            // barTop
            // 
            this.barTop.Items.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.labelItem,
            this.comboBoxItem});
            this.barTop.Location = new System.Drawing.Point(0, 0);
            this.barTop.Name = "barTop";
            this.barTop.Size = new System.Drawing.Size(447, 26);
            this.barTop.Stretch = true;
            this.barTop.Style = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.barTop.TabIndex = 8;
            this.barTop.TabStop = false;
            this.barTop.Visible = false;
            // 
            // labelItem
            // 
            this.labelItem.Name = "labelItem";
            this.labelItem.Text = "选择方式：";
            this.labelItem.Visible = false;
            // 
            // comboBoxItem
            // 
            this.comboBoxItem.ComboWidth = 320;
            this.comboBoxItem.DropDownHeight = 106;
            this.comboBoxItem.Items.AddRange(new object[] {
            this.comboItemTopLay,
            this.comboItemVisibleLay,
            this.comboItemSelectableLay,
            this.comboItemAllLay});
            this.comboBoxItem.Name = "comboBoxItem";
            this.comboBoxItem.Visible = false;
            this.comboBoxItem.SelectedIndexChanged += new System.EventHandler(this.comboBoxItem_SelectedIndexChanged);
            // 
            // comboItemTopLay
            // 
            this.comboItemTopLay.Text = "顶层图层";
            // 
            // comboItemVisibleLay
            // 
            this.comboItemVisibleLay.Text = "可见图层";
            // 
            // comboItemSelectableLay
            // 
            this.comboItemSelectableLay.Text = "可选图层";
            // 
            // comboItemAllLay
            // 
            this.comboItemAllLay.Text = "所有图层";
            // 
            // barButtom
            // 
            this.barButtom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barButtom.Items.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.labelItemMemo,
            this.progressBarItem,
            this.labelEditStatus});
            this.barButtom.Location = new System.Drawing.Point(0, 297);
            this.barButtom.Name = "barButtom";
            this.barButtom.Size = new System.Drawing.Size(447, 19);
            this.barButtom.Stretch = true;
            this.barButtom.Style = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.barButtom.TabIndex = 14;
            this.barButtom.TabStop = false;
            // 
            // labelItemMemo
            // 
            this.labelItemMemo.Name = "labelItemMemo";
            this.labelItemMemo.Width = 120;
            // 
            // progressBarItem
            // 
            this.progressBarItem.ChunkGradientAngle = 0F;
            this.progressBarItem.ColorTable = DevComponents.DotNetBar.eProgressBarItemColor.Paused;
            this.progressBarItem.MenuVisibility = DevComponents.DotNetBar.eMenuVisibility.VisibleAlways;
            this.progressBarItem.Name = "progressBarItem";
            this.progressBarItem.RecentlyUsed = false;
            this.progressBarItem.Visible = false;
            this.progressBarItem.Width = 250;
            // 
            // labelEditStatus
            // 
            this.labelEditStatus.Name = "labelEditStatus";
            this.labelEditStatus.Visible = false;
            // 
            // itemPanel1
            // 
            // 
            // 
            // 
            this.itemPanel1.BackgroundStyle.BackColor = System.Drawing.Color.White;
            this.itemPanel1.BackgroundStyle.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.itemPanel1.BackgroundStyle.BorderBottomWidth = 1;
            this.itemPanel1.BackgroundStyle.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(157)))), ((int)(((byte)(185)))));
            this.itemPanel1.BackgroundStyle.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.itemPanel1.BackgroundStyle.BorderLeftWidth = 1;
            this.itemPanel1.BackgroundStyle.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.itemPanel1.BackgroundStyle.BorderRightWidth = 1;
            this.itemPanel1.BackgroundStyle.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.itemPanel1.BackgroundStyle.BorderTopWidth = 1;
            this.itemPanel1.BackgroundStyle.PaddingBottom = 1;
            this.itemPanel1.BackgroundStyle.PaddingLeft = 1;
            this.itemPanel1.BackgroundStyle.PaddingRight = 1;
            this.itemPanel1.BackgroundStyle.PaddingTop = 1;
            this.itemPanel1.ContainerControlProcessDialogKey = true;
            this.itemPanel1.Controls.Add(this.dataGridViewX);
            this.itemPanel1.Controls.Add(this.advTree);
            this.itemPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.itemPanel1.LayoutOrientation = DevComponents.DotNetBar.eOrientation.Vertical;
            this.itemPanel1.Location = new System.Drawing.Point(0, 0);
            this.itemPanel1.Name = "itemPanel1";
            this.itemPanel1.Size = new System.Drawing.Size(447, 297);
            this.itemPanel1.TabIndex = 15;
            this.itemPanel1.Text = "itemPanel1";
            // 
            // dataGridViewX
            // 
            this.dataGridViewX.AllowUserToAddRows = false;
            this.dataGridViewX.AllowUserToDeleteRows = false;
            this.dataGridViewX.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewX.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewX.DefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewX.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewX.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(215)))), ((int)(((byte)(229)))));
            this.dataGridViewX.Location = new System.Drawing.Point(121, 0);
            this.dataGridViewX.Name = "dataGridViewX";
            this.dataGridViewX.RowHeadersVisible = false;
            this.dataGridViewX.RowTemplate.Height = 23;
            this.dataGridViewX.Size = new System.Drawing.Size(326, 297);
            this.dataGridViewX.TabIndex = 14;
            this.dataGridViewX.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewX_CellValueChanged);
            this.dataGridViewX.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewX_CellDoubleClick);
            this.dataGridViewX.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewX_CellClick);
            // 
            // advTree
            // 
            this.advTree.AccessibleRole = System.Windows.Forms.AccessibleRole.Outline;
            this.advTree.AllowDrop = true;
            this.advTree.BackColor = System.Drawing.SystemColors.Window;
            // 
            // 
            // 
            this.advTree.BackgroundStyle.Class = "TreeBorderKey";
            this.advTree.Dock = System.Windows.Forms.DockStyle.Left;
            this.advTree.Location = new System.Drawing.Point(0, 0);
            this.advTree.Name = "advTree";
            this.advTree.NodesConnector = this.nodeConnector1;
            this.advTree.NodeStyle = this.elementStyle1;
            this.advTree.PathSeparator = ";";
            this.advTree.Size = new System.Drawing.Size(121, 297);
            this.advTree.Styles.Add(this.elementStyle1);
            this.advTree.TabIndex = 13;
            this.advTree.Text = "advTree1";
            this.advTree.NodeDoubleClick += new DevComponents.AdvTree.TreeNodeMouseEventHandler(this.advTree_NodeDoubleClick);
            this.advTree.NodeClick += new DevComponents.AdvTree.TreeNodeMouseEventHandler(this.advTree_NodeClick);
            // 
            // nodeConnector1
            // 
            this.nodeConnector1.LineColor = System.Drawing.SystemColors.ControlText;
            // 
            // elementStyle1
            // 
            this.elementStyle1.Name = "elementStyle1";
            this.elementStyle1.TextColor = System.Drawing.SystemColors.ControlText;
            // 
            // frmAttributeEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(447, 316);
            this.Controls.Add(this.itemPanel1);
            this.Controls.Add(this.barButtom);
            this.Controls.Add(this.barTop);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmAttributeEdit";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "属性编辑";
            ((System.ComponentModel.ISupportInitialize)(this.barTop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barButtom)).EndInit();
            this.itemPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.advTree)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.Bar barTop;
        private DevComponents.DotNetBar.LabelItem labelItem;
        private DevComponents.DotNetBar.ComboBoxItem comboBoxItem;
        private DevComponents.Editors.ComboItem comboItemTopLay;
        private DevComponents.Editors.ComboItem comboItemVisibleLay;
        private DevComponents.Editors.ComboItem comboItemSelectableLay;
        //private DevComponents.Editors.ComboItem comboItemCurEdit;
        private DevComponents.DotNetBar.Bar barButtom;
        private DevComponents.DotNetBar.ItemPanel itemPanel1;
        private DevComponents.DotNetBar.Controls.DataGridViewX dataGridViewX;
        private DevComponents.AdvTree.AdvTree advTree;
        private DevComponents.AdvTree.NodeConnector nodeConnector1;
        private DevComponents.DotNetBar.ElementStyle elementStyle1;
        private DevComponents.DotNetBar.LabelItem labelItemMemo;
        private DevComponents.DotNetBar.ProgressBarItem progressBarItem;
        private DevComponents.Editors.ComboItem comboItemAllLay;
        private DevComponents.DotNetBar.LabelItem labelEditStatus;
    }
}