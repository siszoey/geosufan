using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using Fan.Common.Gis;
using Fan.Common.Error;
using Fan.Common.Authorize;
using ESRI.ArcGIS.esriSystem;
using Fan.Common;

namespace GeoUserManager
{
    public partial class SettingControl : UserControl
    {
        //�Ҽ��˵�����
        private Dictionary<string, DevComponents.DotNetBar.ContextMenuBar> _dicContextMenu;
        enumWSType _curWsType = enumWSType.SDE;                               //��ǰ�����ռ�����

        //��ʼ��������
        public SettingControl(string strName, string strCation)
        {
            InitializeComponent();
            //��ʼ�����ö�Ӧ��ͼ�ؼ�
            InitialMainViewControl();
            this.Dock = System.Windows.Forms.DockStyle.Fill;

            this.Name = strName;
            this.Tag = strCation;
            this.roleTree.ImageList = IconContainer;

            ModData.v_AppPrivileges.MainUserControl = this;
            ModData.v_AppPrivileges.MainTree = this.mainTree;
            ModData.v_AppPrivileges.RoleTree = this.roleTree;
            ModData.v_AppPrivileges.UserTree = this.userTree;
            ModData.v_AppPrivileges.PrivilegeTree = this.priTree;

            //��ʼ����ܲ���ؼ�����
            InitialFrmDefControl();
        }

        //��ʼ�����ö�Ӧ��ͼ�ؼ�
        private void InitialMainViewControl()
        {
            frmBarManager newfrmBarManager = new frmBarManager();
            newfrmBarManager.TopLevel = false;
            newfrmBarManager.Dock = DockStyle.Fill;
            newfrmBarManager.Show();
            this.Controls.Add(newfrmBarManager);

            DevComponents.AdvTree.Node node = new DevComponents.AdvTree.Node();
            node.Image = global::GeoUserManager.Properties.Resources.rulemanager;
            node.Text = "��ɫ����";
            mainTree.Nodes.Add(node);
            mainTree.SelectedNode = node;
            node = new DevComponents.AdvTree.Node();
            node.Image = global::GeoUserManager.Properties.Resources.usemanager ;
            node.Text = "�û�����";
            mainTree.Nodes.Add(node);

            //�������ÿ�����ͼ
            DevComponents.DotNetBar.Bar barManagerTree = newfrmBarManager.CreateBar("barManagerTree", enumLayType.FILL);
            barManagerTree.CanHide = false;
            barManagerTree.CanAutoHide = true;
            DevComponents.DotNetBar.PanelDockContainer PanelManagerTree = newfrmBarManager.CreatePanelDockContainer("PanelManagerTree", barManagerTree);
            DockContainerItem TreeContainerItem = newfrmBarManager.CreateDockContainerItem("TreeContainerItem", "Ȩ�޹���", PanelManagerTree, barManagerTree);
            PanelManagerTree.Controls.Add(this.mainTree);
            this.mainTree.Dock = DockStyle.Fill;

            //��������������ͼ
            DevComponents.DotNetBar.Bar barManagerView = newfrmBarManager.CreateBar("barManagerView", enumLayType.FILL);
            barManagerView.CanHide = false;
            DevComponents.DotNetBar.PanelDockContainer PanelManagerView = newfrmBarManager.CreatePanelDockContainer("PanelManagerView", barManagerView);
            DockContainerItem ViewContainerItem = newfrmBarManager.CreateDockContainerItem("TreeContainerItem", "", PanelManagerView, barManagerView);
            PanelManagerView.Controls.Add(this.panel);
            this.panel.Dock = DockStyle.Fill;

            //��������
            newfrmBarManager.MainDotNetBarManager.FillDockSite.GetDocumentUIManager().Dock(barManagerTree, barManagerView, eDockSide.Right);
            newfrmBarManager.MainDotNetBarManager.FillDockSite.GetDocumentUIManager().SetBarWidth(barManagerTree, this.Width / 5);

            //����������ʾ����
            //�û�������
            PanelDockContainer PanelTipData = new PanelDockContainer();
            PanelTipData.Controls.Add(this.tipRichBox);
            this.tipRichBox.Dock = DockStyle.Fill;
            DockContainerItem dockItemData = new DockContainerItem("dockItemData", "��ʾ");
            dockItemData.Control = PanelTipData;
            newfrmBarManager.ButtomBar.Items.Add(dockItemData);
        }

        //��ʼ����ܲ���ؼ�����
        private void InitialFrmDefControl()
        {
            //�õ�Xml��System�ڵ�,����XML���ز������
            string xPath = ".//System[@Name='" + this.Name + "']";
            Fan.Plugin.ModuleCommon.LoadButtonViewByXmlNode(ModData.v_AppPrivileges.ControlContainer, xPath, ModData.v_AppPrivileges);

            _dicContextMenu = ModData.v_AppPrivileges.DicContextMenu;


            //��������Ȩ��
            XmlDocument doc =new XmlDocument();
            doc.Load(ModData.m_SysXmlPath);

            if (doc != null)
            {
                //��xmlȨ���ĵ���ʾ��Ȩ������
                ModuleOperator.DisplayInLstView(doc, priTree);
            }
            //��ͼ��Ŀ¼�ӹ����⿽��������Ŀ¼
            ModuleOperator.CopyLayerTreeXmlFromDataBase(Fan.Plugin.ModuleCommon.TmpWorkSpace,ModData.m_DataXmlPath );
            XmlDocument datadoc = new XmlDocument();
            datadoc.Load(ModData.m_DataXmlPath);
            if (datadoc != null)
            {
                ModuleOperator.DisplayInDataLstView(datadoc, DataTree);
            }

            ModuleOperator.DisplayInDbsourceLstView(this.dbSourceTree);
        }

        private void roleTree_AfterNodeSelect(object sender, DevComponents.AdvTree.AdvTreeNodeEventArgs e)
        {
            Exception eError;
            if (roleTree.SelectedNode == null)
            {
                return;
            }
            DevComponents.AdvTree.Node selectNode = roleTree.SelectedNode;
            SysGisTable sysTable = new SysGisTable(ModData.gisDb);
            bool result = false;
            //�������ݿ��������
            if (ModData.gisDb == null)
            {
                switch (SdeConfig.dbType)
                {
                    case "ORACLE":
                    case "SQLSERVER":
                        _curWsType = enumWSType.SDE;
                        break;
                    case "ACCESS":
                        _curWsType = enumWSType.PDB;
                        break;
                    case "FILE":
                        _curWsType = enumWSType.GDB;
                        break;
                }
                ModData.gisDb = new SysGisDB();
                switch (_curWsType)
                {
                    case enumWSType.SDE:
                        result = ModData.gisDb.SetWorkspace(SdeConfig.Server, SdeConfig.Instance, SdeConfig.Database, SdeConfig.User, SdeConfig.Password, SdeConfig.Version, out eError);
                        break;
                    case enumWSType.PDB:
                    case enumWSType.GDB:
                        result = ModData.gisDb.SetWorkspace(SdeConfig.Server, _curWsType, out eError);
                        break;
                    default:
                        break;
                }
                if (!result) return;
            }
            List<Dictionary<string, object>> lstDicData = sysTable.GetRows("role_pri", "ROLEID='" + selectNode.Name.ToLower() + "'", out eError);
            List<string> lstPrivilege = new List<string>();
            if (lstDicData.Count != 0)
            {
                for (int i = 0; i < lstDicData.Count; i++)
                {
                    lstPrivilege.Add(lstDicData[i]["PRIVILEGE_ID"].ToString());
                }
                for (int i = 0; i < priTree.Nodes.Count; i++)
                {
                    DevComponents.AdvTree.Node treeNode = priTree.Nodes[i];
                    ModuleOperator.IsCheckChanged = false;
                    if (lstPrivilege.Contains(treeNode.Name))
                    {
                        treeNode.Cells[1].Checked = true;
                    }
                    else
                    {
                        treeNode.Cells[1].Checked = false;
                    }
                    if (treeNode.Tag.ToString() == treeNode.Cells[1].Checked.ToString())
                    {
                        ModuleOperator.IsCheckChanged = true;
                    }
                    else
                    {
                        treeNode.Tag = treeNode.Cells[1].Checked;
                    }
                    if (treeNode.HasChildNodes)
                    {
                        ModuleOperator.UpdateRolePrivilege(lstPrivilege, treeNode);
                    }
                }
            }
            else
            {
                for (int i = 0; i < priTree.Nodes.Count; i++)
                {
                    DevComponents.AdvTree.Node treeNode = priTree.Nodes[i];
                    ModuleOperator.IsCheckChanged = false;
                    treeNode.Cells[1].Checked = false;
                    if (treeNode.Tag.ToString() == treeNode.Cells[1].Checked.ToString())
                    {
                        ModuleOperator.IsCheckChanged = true;
                    }
                    else
                    {
                        treeNode.Tag = treeNode.Cells[1].Checked;
                    }
                    if (treeNode.HasChildNodes)
                    {
                        ModuleOperator.UpdateRolePrivilege(lstPrivilege, treeNode);
                    }
                }
            }
            //����Ȩ������
            List<Dictionary<string, object>> lstDicDataData = sysTable.GetRows("role_data", "ROLEID='" + selectNode.Name.ToLower() + "'", out eError);
            List<string> lstdataPrivilege = new List<string>();
            if (lstDicDataData.Count != 0)
            {
                for (int i = 0; i < lstDicDataData.Count; i++)
                {
                    lstdataPrivilege.Add(lstDicDataData[i]["DATAPRI_ID"].ToString());
                }
                for (int i = 0; i < DataTree.Nodes.Count; i++)
                {
                    DevComponents.AdvTree.Node treeNode = DataTree.Nodes[i];
                    ModuleOperator.IsCheckChanged = false;
                    if (lstdataPrivilege.Contains(treeNode.Name))
                    {
                        treeNode.Cells[1].Checked = true;
                    }
                    else
                    {
                        treeNode.Cells[1].Checked = false;
                    }
                    if (treeNode.Tag.ToString() == treeNode.Cells[1].Checked.ToString())
                    {
                        ModuleOperator.IsCheckChanged = true;
                    }
                    else
                    {
                        treeNode.Tag = treeNode.Cells[1].Checked;
                    }
                    if (treeNode.HasChildNodes)
                    {
                        ModuleOperator.UpdateRolePrivilege(lstdataPrivilege, treeNode);
                    }
                }
            }
            else
            {
                for (int i = 0; i < DataTree.Nodes.Count; i++)
                {
                    DevComponents.AdvTree.Node treeNode = DataTree.Nodes[i];
                    ModuleOperator.IsCheckChanged = false;
                    treeNode.Cells[1].Checked = false;
                    if (treeNode.Tag.ToString() == treeNode.Cells[1].Checked.ToString())
                    {
                        ModuleOperator.IsCheckChanged = true;
                    }
                    else
                    {
                        treeNode.Tag = treeNode.Cells[1].Checked;
                    }
                    if (treeNode.HasChildNodes)
                    {
                        ModuleOperator.UpdateRolePrivilege(lstdataPrivilege, treeNode);
                    }
                }
            }
            //����ԴȨ������
            List<Dictionary<string, object>> lstDicDbsource = sysTable.GetRows("role_dbsource", "ROLEID='" + selectNode.Name.ToLower() + "'", out eError);
            List<string> lstdbsource = new List<string>();
            if (lstDicDbsource != null)
            {
                if (lstDicDbsource.Count != 0)
                {
                    for (int i = 0; i < lstDicDbsource.Count; i++)
                    {
                        lstdbsource.Add(lstDicDbsource[i]["DBSOURCE_ID"].ToString());
                    }
                    for (int i = 0; i < dbSourceTree.Nodes.Count; i++)
                    {
                        DevComponents.AdvTree.Node treeNode = dbSourceTree.Nodes[i];
                        ModuleOperator.IsCheckChanged = false;
                        if (lstdbsource.Contains(treeNode.Name))
                        {
                            treeNode.Cells[1].Checked = true;
                        }
                        else
                        {
                            treeNode.Cells[1].Checked = false;
                        }
                        if (treeNode.Tag.ToString() == treeNode.Cells[1].Checked.ToString())
                        {
                            ModuleOperator.IsCheckChanged = true;
                        }
                        else
                        {
                            treeNode.Tag = treeNode.Cells[1].Checked;
                        }
                        if (treeNode.HasChildNodes)
                        {
                            ModuleOperator.UpdateRolePrivilege(lstdbsource, treeNode);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < dbSourceTree.Nodes.Count; i++)
                    {
                        DevComponents.AdvTree.Node treeNode = dbSourceTree.Nodes[i];
                        ModuleOperator.IsCheckChanged = false;
                        treeNode.Cells[1].Checked = false;
                        if (treeNode.Tag.ToString() == treeNode.Cells[1].Checked.ToString())
                        {
                            ModuleOperator.IsCheckChanged = true;
                        }
                        else
                        {
                            treeNode.Tag = treeNode.Cells[1].Checked;
                        }
                        if (treeNode.HasChildNodes)
                        {
                            ModuleOperator.UpdateRolePrivilege(lstdbsource, treeNode);
                        }
                    }
                }
            }
        }


        private void userTree_NodeClick(object sender, DevComponents.AdvTree.TreeNodeMouseEventArgs e)
        {
            Exception eError;
            User user = userTree.SelectedNode.Tag as User;
            if (user != null)
            {
                //���Ȩ����
                lstUserPrivilege.Items.Clear();
                //�󶨿���Ȩ��
                ModuleOperator.DisplayRoleLstView("", lstAllPrivilege, ref ModData.gisDb,out eError);
                if (eError != null)
                {
                    ErrorHandle.ShowInform("��ʾ", eError.Message);
                    return;
                }
                List<string> ids = ModuleOperator.GetRoleIds(user.IDStr, ref ModData.gisDb,out eError);
                string strSql = "";
                if (ids != null && ids.Count > 0)
                {
                    foreach (string id in ids)
                    {
                        if (string.IsNullOrEmpty(strSql))
                        {
                            strSql = "'"+id.ToString()+"'";
                        }
                        else
                        {
                            strSql += ",'" + id.ToString()+"'";
                        }
                    }
                    //strSql = strSql.Remove(strSql.Length - 1);
                    ModuleOperator.DisplayRoleLstView("ROLEID IN (" + strSql + ")", lstUserPrivilege, ref ModData.gisDb, out eError);
                }
                else
                {
                    if (eError != null)
                    {
                        ErrorHandle.ShowInform("��ʾ", eError.Message);
                        return;
                    }
                }
            }
        }

        private void btnPrivilegesAdd_Click(object sender, EventArgs e)
        {
            if (this.lstAllPrivilege.SelectedItems.Count > 0)
            {
                ListViewItem item = this.lstAllPrivilege.SelectedItems[0];
                if (item != null)
                {
                    foreach (ListViewItem pItem in this.lstUserPrivilege.Items)
                    {
                        if (pItem.Text.Equals(item.Text))
                        {
                            return;
                        }
                    }
                    this.lstUserPrivilege.Items.Add(item.Clone() as ListViewItem);
                }
            }
        }

        private void btnPrivilegesRemove_Click(object sender, EventArgs e)
        {
            if (this.lstUserPrivilege.SelectedItems.Count > 0)
            {
                ListViewItem item = this.lstUserPrivilege.SelectedItems[0];
                if (item != null)
                {
                    this.lstUserPrivilege.Items.Remove(item);
                }
            }
        }

        private void lstAllPrivilege_DoubleClick(object sender, EventArgs e)
        {
            btnPrivilegesAdd_Click(null, null);
        }

        private void lstUserPrivilege_DoubleClick(object sender, EventArgs e)
        {
            btnPrivilegesRemove_Click(null, null);
        }

        /// <summary>
        /// �����û��������û���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOk_Click(object sender, EventArgs e)
        {
            User user = userTree.SelectedNode.Tag as User;
            if (user != null)
            {
                Exception exError;
                Dictionary<string, object> dicValues;
                try
                {
                    if (ModData.gisDb == null)
                    {
                        ModData.gisDb.SetWorkspace(SdeConfig.Server, SdeConfig.Instance, SdeConfig.Database, SdeConfig.User, SdeConfig.Password, SdeConfig.Version, out exError);
                    }
                    ModData.gisDb.StartTransaction(out exError);
                    SysGisTable sysTable = new SysGisTable(ModData.gisDb);
                    if (lstUserPrivilege.Items.Count > 0)
                    {
                        sysTable.DeleteRows("user_role", "USERID='" + user.IDStr +"'", out exError);
                        bool result = false;
                        foreach (ListViewItem item in lstUserPrivilege.Items)
                        {
                            dicValues = new Dictionary<string, object>();
                            Role role = item.Tag as Role;
                            if (role != null)
                            {
                                dicValues.Add("userid", user.IDStr );
                                dicValues.Add("roleid", role.IDStr );
                                result=sysTable.NewRow("user_role", dicValues, out exError);
                            }
                        }
                        ModData.gisDb.EndTransaction(result, out exError);
                        if (result)
                        {
                            ErrorHandle.ShowInform("��ʾ", "��ɫ���óɹ���");
                        }
                    }
                }
                catch (Exception ex)
                {
                    exError = ex;
                    ModData.gisDb.EndTransaction(false, out exError);
                    return;
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            userTree_NodeClick(null, null);
        }

        private void userTree_NodeMouseDown(object sender, DevComponents.AdvTree.TreeNodeMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left || _dicContextMenu == null)
                return;
            System.Drawing.Point pPoint = new System.Drawing.Point(e.X, e.Y);
            DevComponents.DotNetBar.ButtonItem item = null;
            if (_dicContextMenu.ContainsKey("ContextMenuTree"))
            {
                if (_dicContextMenu["ContextMenuTree"].Items.Count > 0)
                {
                    item = _dicContextMenu["ContextMenuTree"].Items[0] as DevComponents.DotNetBar.ButtonItem;
                    if (item != null)
                    {
                        item.Popup(userTree.PointToScreen(pPoint));
                    }
                }
            }
        }

        private void roleTree_NodeMouseDown(object sender, DevComponents.AdvTree.TreeNodeMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left || _dicContextMenu == null)
                return;
            System.Drawing.Point pPoint = new System.Drawing.Point(e.X, e.Y);
            DevComponents.DotNetBar.ButtonItem item = null;
            if (_dicContextMenu.ContainsKey("ContextMenuTree"))
            {
                if (_dicContextMenu["ContextMenuTree"].Items.Count > 0)
                {
                    item = _dicContextMenu["ContextMenuTree"].Items[0] as DevComponents.DotNetBar.ButtonItem;
                    if (item != null)
                    {
                        item.Popup(roleTree.PointToScreen(pPoint));
                    }
                }
            }
        }

        private void mainTree_SelectionChanged(object sender, EventArgs e)
        {
            Exception eError;
            if (mainTree.SelectedNode == null) return;
            DevComponents.AdvTree.Node node =mainTree.SelectedNode;
            bool result = false;
            //�������ݿ��������
            if (ModData.gisDb == null)
            {
                switch (SdeConfig.dbType)
                {
                    case "ORACLE":
                    case "SQLSERVER":
                        _curWsType = enumWSType.SDE;
                        break;
                    case "ACCESS":
                        _curWsType = enumWSType.PDB;
                        break;
                    case "FILE":
                        _curWsType = enumWSType.GDB;
                        break;
                }
                ModData.gisDb = new SysGisDB();
                switch (_curWsType)
                {
                    case enumWSType.SDE:
                        result = ModData.gisDb.SetWorkspace(SdeConfig.Server, SdeConfig.Instance, SdeConfig.Database, SdeConfig.User, SdeConfig.Password, SdeConfig.Version, out eError);
                        break;
                    case enumWSType.PDB:
                    case enumWSType.GDB:
                        result = ModData.gisDb.SetWorkspace(SdeConfig.Server, _curWsType, out eError);
                        break;
                    default:
                        break;
                }
                if (!result) return;
            }

            if (node.Text.Equals("��ɫ����"))
            {
                ModuleOperator.DisplayRoleTree("", roleTree, ref ModData.gisDb, out eError);
                if (eError != null)
                {
                    ErrorHandle.ShowInform("��ʾ", eError.Message);
                    return;
                }
                roleManagerPanel.Visible = true;
                userManagerPanel.Visible = false;
                ModData.v_AppPrivileges.CurrentPanel = roleManagerPanel;
            }
            else
            {
                ModuleOperator.DisplayUserTree("", userTree, ref ModData.gisDb, out eError);
                if (eError != null)
                {
                    ErrorHandle.ShowInform("��ʾ", eError.Message);
                    return;
                }
                roleManagerPanel.Visible = false;
                userManagerPanel.Visible = true;
                ModData.v_AppPrivileges.CurrentPanel = userManagerPanel;
            }
        }

        private void SettingControl_Load(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// ��������Ȩ�����ĵ�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataTree_AfterCheck(object sender, DevComponents.AdvTree.AdvTreeCellEventArgs e)
        {
            Exception eError;
            SysGisTable sysTable = new SysGisTable(ModData.gisDb);
            if (ModuleOperator.IsCheckChanged)
            {
                if (roleTree.SelectedNode != null && roleTree.SelectedNode.Text != "��ɫ")
                {
                    DevComponents.AdvTree.Cell cell = e.Cell;
                    if (cell == null) return;
                    DevComponents.AdvTree.Node aNode = e.Cell.Parent;
                    if (aNode != null)
                    {
                        aNode.Tag = aNode.Cells[1].Checked;
                        if (aNode.Cells[1].Checked.ToString().ToUpper() == "TRUE")
                        {
                            //Dictionary<string, object> dicvalues = new Dictionary<string, object>();
                            //dicvalues.Add("ROLEID", roleTree.SelectedNode.Name);
                            //dicvalues.Add("DATAPRI_ID", aNode.Name);
                            //sysTable.NewRow("role_Data", dicvalues, out eError);
                            CheckeddataNode(sysTable, aNode);
                        }
                        else if (aNode.Cells[1].Checked.ToString().ToUpper() == "FALSE")
                        {
                            //sysTable.DeleteRows("role_Data", "ROLEID='" + roleTree.SelectedNode.Name + "' and DATAPRI_ID='" + aNode.Name + "'", out eError);
                            unCheckeddataNode(sysTable, aNode);
                        }
                        else
                        { return; }
                    }
                }
                else
                {
                    ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ѡ���ɫ��");
                }
            }
            else
            {
                ModuleOperator.IsCheckChanged = true;
            }
        }

        /// <summary>
        /// ����Ȩ�����ĵ�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void priTree_AfterCheck(object sender, DevComponents.AdvTree.AdvTreeCellEventArgs e)
        {
            Exception eError;
            SysGisTable sysTable = new SysGisTable(ModData.gisDb);
            if (ModuleOperator.IsCheckChanged)
            {
                if (roleTree.SelectedNode != null && roleTree.SelectedNode.Text != "��ɫ")
                {
                    DevComponents.AdvTree.Cell cell = e.Cell;
                    if (cell == null) return;
                    DevComponents.AdvTree.Node aNode = e.Cell.Parent;
                    if (aNode != null)
                    {
                        aNode.Tag = aNode.Cells[1].Checked;
                        if (aNode.Cells[1].Checked.ToString().ToUpper() == "TRUE")
                        {
                            //Dictionary<string, object> dicvalues = new Dictionary<string, object>();
                            //dicvalues.Add("ROLEID", roleTree.SelectedNode.Name);
                            //dicvalues.Add("PRIVILEGE_ID", aNode.Name);
                            //sysTable.NewRow("role_pri", dicvalues, out eError);
                            CheckedNode(sysTable, aNode);
                        }
                        else if (aNode.Cells[1].Checked.ToString().ToUpper() == "FALSE")
                        {
                            //sysTable.DeleteRows("role_pri", "ROLEID='" + roleTree.SelectedNode.Name + "' and PRIVILEGE_ID='" + aNode.Name + "'", out eError);
                            unCheckedNode(sysTable, aNode);
                        }
                        else
                        { return; }
                    }
                }
                else
                {
                    ErrorHandle.ShowInform("��ʾ", "��ѡ���ɫ��");
                }
            }
            else
            {
                ModuleOperator.IsCheckChanged = true;
            }
        }
        //ѡ��ĳ��Ȩ�޽ڵ㣬�������ڵ㣬ȫ��ѡ��
        private void CheckedNode(SysGisTable sysTable, DevComponents.AdvTree.Node aNode)
        {
            if (roleTree.SelectedNode == null)
            {
                ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ѡ���ɫ��");
                return;
            }
            Exception eError;
            Dictionary<string, object> dicvalues = new Dictionary<string, object>();
            dicvalues.Add("ROLEID", roleTree.SelectedNode.Name);
            dicvalues.Add("PRIVILEGE_ID", aNode.Name);
            sysTable.NewRow("role_pri", dicvalues, out eError);
            if (aNode.Parent != null)
            {
                DevComponents.AdvTree.Node pNodeparent = aNode.Parent;
                if (pNodeparent.Cells[1].Checked == false)
                {
                    pNodeparent.Cells[1].Checked = true;
                    CheckedNode(sysTable, pNodeparent);
                }
            }
        }
        //��ѡĳ��Ȩ�޽ڵ㣬�����ӽڵ㣬ȫ����ѡ
        private void unCheckedNode(SysGisTable sysTable, DevComponents.AdvTree.Node aNode)
        {
            if (roleTree.SelectedNode == null)
            {
                ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ѡ���ɫ��");
                return;
            }
            Exception eError;
            sysTable.DeleteRows("role_pri", "ROLEID='" + roleTree.SelectedNode.Name + "' and PRIVILEGE_ID='" + aNode.Name + "'", out eError);
            if (aNode.Nodes.Count > 0)
            {
                DevComponents.AdvTree.NodeCollection pCollection = aNode.Nodes as DevComponents.AdvTree.NodeCollection;
                foreach (DevComponents.AdvTree.Node eachnode in pCollection)
                {
                    eachnode.Cells[1].Checked = false;
                    unCheckedNode(sysTable, eachnode);
                }
            }
        }
        //ѡ��ĳ������Ȩ�޽ڵ㣬�������ڵ㣬ȫ��ѡ��
        private void CheckeddataNode(SysGisTable sysTable, DevComponents.AdvTree.Node aNode)
        {
            if (roleTree.SelectedNode == null)
            {
                ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ѡ���ɫ��");
                return;
            }
            Exception eError;
            Dictionary<string, object> dicvalues = new Dictionary<string, object>();
            dicvalues.Add("ROLEID", roleTree.SelectedNode.Name);
            dicvalues.Add("DATAPRI_ID", aNode.Name);
            sysTable.NewRow("role_Data", dicvalues, out eError);
            if (aNode.Parent != null)
            {
                DevComponents.AdvTree.Node pNodeparent = aNode.Parent;
                if (pNodeparent.Cells[1].Checked == false)
                {
                    pNodeparent.Cells[1].Checked = true;
                    CheckeddataNode(sysTable, pNodeparent);
                }
            }
        }
        //ѡ��ĳ������ԴȨ�޽ڵ㣬�������ڵ㣬ȫ��ѡ��
        private void CheckeddbsourceNode(SysGisTable sysTable, DevComponents.AdvTree.Node aNode)
        {
            if (roleTree.SelectedNode == null)
            {
                ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ѡ���ɫ��");
                return;
            }
            Exception eError;
            Dictionary<string, object> dicvalues = new Dictionary<string, object>();
            dicvalues.Add("ROLEID", roleTree.SelectedNode.Name);
            dicvalues.Add("DBSOURCE_ID", aNode.Name);
            if (!sysTable.ExistData("role_dbsource", "ROLEID='" + roleTree.SelectedNode.Name + "' and DBSOURCE_ID=" + aNode.Name))
            {
                sysTable.NewRow("role_dbsource", dicvalues, out eError);
            }
            if (aNode.Parent != null)
            {
                DevComponents.AdvTree.Node pNodeparent = aNode.Parent;
                if (pNodeparent.Cells[1].Checked == false)
                {
                    pNodeparent.Cells[1].Checked = true;
                    CheckeddbsourceNode(sysTable, pNodeparent);
                }
            }
        }
        //��ѡĳ������Ȩ�޽ڵ㣬�����ӽڵ㣬ȫ����ѡ
        private void unCheckeddataNode(SysGisTable sysTable, DevComponents.AdvTree.Node aNode)
        {
            if (roleTree.SelectedNode == null)
            {
                ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ѡ���ɫ��");
                return;
            }
            Exception eError;
            sysTable.DeleteRows("role_Data", "ROLEID='" + roleTree.SelectedNode.Name + "' and DATAPRI_ID='" + aNode.Name + "'", out eError);
            if (aNode.Nodes.Count > 0)
            {
                DevComponents.AdvTree.NodeCollection pCollection = aNode.Nodes as DevComponents.AdvTree.NodeCollection;
                foreach (DevComponents.AdvTree.Node eachnode in pCollection)
                {
                    eachnode.Cells[1].Checked = false;
                    unCheckeddataNode(sysTable, eachnode);
                }
            }
        }
        //��ѡĳ������ԴȨ�޽ڵ㣬�����ӽڵ㣬ȫ����ѡ
        private void unCheckeddbsourceNode(SysGisTable sysTable, DevComponents.AdvTree.Node aNode)
        {
            if (roleTree.SelectedNode == null)
            {
                ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ѡ���ɫ��");
                return;
            }
            Exception eError;
            sysTable.DeleteRows("role_Dbsource", "ROLEID='" + roleTree.SelectedNode.Name + "' and DBSOURCE_ID=" + aNode.Name , out eError);
            if (aNode.Nodes.Count > 0)
            {
                DevComponents.AdvTree.NodeCollection pCollection = aNode.Nodes as DevComponents.AdvTree.NodeCollection;
                foreach (DevComponents.AdvTree.Node eachnode in pCollection)
                {
                    eachnode.Cells[1].Checked = false;
                    unCheckeddbsourceNode(sysTable, eachnode);
                }
            }
        }
        private void MenuSelectAll_Click(object sender, EventArgs e)
        {

            SysGisTable sysTable = new SysGisTable(ModData.gisDb);
            DevComponents.AdvTree.Node aNode = priTree.SelectedNode;
            
            SelectAllpriNode(sysTable, aNode);
        }
        //����Ȩ��ȫѡ���ܣ�ѡ�е�ǰ�ڵ㼰�����²�ڵ㣩
        private void SelectAllpriNode(SysGisTable sysTable,DevComponents.AdvTree.Node aNode)
        {
            if (roleTree.SelectedNode == null)
            {
                ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ѡ���ɫ��");
                return;
            }
            if (aNode != null)
            {
                Exception eError;
                Dictionary<string, object> dicvalues = new Dictionary<string, object>();
                if (aNode.Cells[1].Checked == false)
                {
                    //cyf 20110613  :add����Ӷ����ڵ��ѡ��״̬�ı�������Ȼ������
                    if (roleTree.SelectedNode == null)
                    {
                        Fan.Common.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ѡ��һ����ɫ�ڵ㡣");
                        return;
                    }
                    //end
                    dicvalues.Add("ROLEID", roleTree.SelectedNode.Name);
                    dicvalues.Add("PRIVILEGE_ID", aNode.Name);
                    sysTable.NewRow("role_pri", dicvalues, out eError);
                }
                aNode.Cells[1].Checked = true;
                if (aNode.Nodes.Count > 0)
                {
                    DevComponents.AdvTree.NodeCollection pCollection = aNode.Nodes as DevComponents.AdvTree.NodeCollection;
                    foreach (DevComponents.AdvTree.Node eachnode in pCollection)
                    {
                        //eachnode.Cells[1].Checked = true;
                        SelectAllpriNode(sysTable, eachnode);
                    }
                }
            }
           
        }
        //����Ȩ��ȫ��ѡ���ܣ���ѡ��ǰ�ڵ㼰�����²�ڵ㣩
        private void unSelectAllpriNode(SysGisTable sysTable, DevComponents.AdvTree.Node aNode)
        {
            if (aNode != null && roleTree.SelectedNode != null)
            {
                Exception eError;
                Dictionary<string, object> dicvalues = new Dictionary<string, object>();
                if (aNode.Cells[1].Checked == true)
                {
                    sysTable.DeleteRows("role_pri", "ROLEID='" + roleTree.SelectedNode.Name + "' and PRIVILEGE_ID='" + aNode.Name + "'", out eError);

                }
                aNode.Cells[1].Checked = false;
                if (aNode.Nodes.Count > 0)
                {
                    DevComponents.AdvTree.NodeCollection pCollection = aNode.Nodes as DevComponents.AdvTree.NodeCollection;
                    foreach (DevComponents.AdvTree.Node eachnode in pCollection)
                    {
                        //eachnode.Cells[1].Checked = true;
                        unSelectAllpriNode(sysTable, eachnode);
                    }
                }
            }
          
        }
        //����Ȩ�޽ڵ�ȫѡ���ܣ���ǰ�ڵ㼰�����²�ڵ㣩
        private void SelectAlldataNode(SysGisTable sysTable, DevComponents.AdvTree.Node aNode)
        {
            if (roleTree.SelectedNode == null)
            {
                ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ѡ���ɫ��");
                return;
            }
            if (aNode == null)
                return;
            Exception eError;
            Dictionary<string, object> dicvalues = new Dictionary<string, object>();
            if (aNode.Cells[1].Checked == false)
            {
                dicvalues.Add("ROLEID", roleTree.SelectedNode.Name);
                dicvalues.Add("DATAPRI_ID", aNode.Name);
                sysTable.NewRow("role_Data", dicvalues, out eError);
            }
            aNode.Cells[1].Checked = true;
            if (aNode.Nodes.Count > 0)
            {
                DevComponents.AdvTree.NodeCollection pCollection = aNode.Nodes as DevComponents.AdvTree.NodeCollection;
                foreach (DevComponents.AdvTree.Node eachnode in pCollection)
                {
                    //eachnode.Cells[1].Checked = true;
                    SelectAlldataNode(sysTable, eachnode);
                }
            }
        }
        //����Ȩ�޽ڵ�ȫ��ѡ���ܣ���ǰ�ڵ㼰�����²�ڵ㣩
        private void unSelectAlldataNode(SysGisTable sysTable, DevComponents.AdvTree.Node aNode)
        {
            if (roleTree.SelectedNode == null)
            {
                ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ѡ���ɫ��");
                return;
            }
            if (aNode == null)
                return;
            Exception eError;
            Dictionary<string, object> dicvalues = new Dictionary<string, object>();
            if (aNode.Cells[1].Checked == true)
            {
                sysTable.DeleteRows("role_Data", "ROLEID='" + roleTree.SelectedNode.Name + "' and DATAPRI_ID='" + aNode.Name + "'", out eError);

            }
            aNode.Cells[1].Checked = false;
            if (aNode.Nodes.Count > 0)
            {
                DevComponents.AdvTree.NodeCollection pCollection = aNode.Nodes as DevComponents.AdvTree.NodeCollection;
                foreach (DevComponents.AdvTree.Node eachnode in pCollection)
                {
                    //eachnode.Cells[1].Checked = true;
                    unSelectAlldataNode(sysTable, eachnode);
                }
            }
        }
        private void priTree_NodeClick(object sender, DevComponents.AdvTree.TreeNodeMouseEventArgs e)
        {
            priTree.SelectedNode = e.Node;
        }

        private void MenuUnselectall_Click(object sender, EventArgs e)
        {

            SysGisTable sysTable = new SysGisTable(ModData.gisDb);
            DevComponents.AdvTree.Node aNode = priTree.SelectedNode;

            unSelectAllpriNode(sysTable, aNode);
        }

        private void MenudataSelectAll_Click(object sender, EventArgs e)
        {
            SysGisTable sysTable = new SysGisTable(ModData.gisDb);
            DevComponents.AdvTree.Node aNode =DataTree.SelectedNode;

            SelectAlldataNode(sysTable, aNode);
        }

        private void MenuDataunSelectall_Click(object sender, EventArgs e)
        {
            SysGisTable sysTable = new SysGisTable(ModData.gisDb);
            DevComponents.AdvTree.Node aNode = DataTree.SelectedNode;

            unSelectAlldataNode(sysTable, aNode);
        }

        private void dbSourceTree_AfterCheck(object sender, DevComponents.AdvTree.AdvTreeCellEventArgs e)
        {
            Exception eError;
            SysGisTable sysTable = new SysGisTable(ModData.gisDb);
            if (ModuleOperator.IsCheckChanged)
            {
                if (roleTree.SelectedNode != null && roleTree.SelectedNode.Text != "��ɫ")
                {
                    DevComponents.AdvTree.Cell cell = e.Cell;
                    if (cell == null) return;
                    DevComponents.AdvTree.Node aNode = e.Cell.Parent;
                    if (aNode != null)
                    {
                        aNode.Tag = aNode.Cells[1].Checked;
                        if (aNode.Cells[1].Checked.ToString().ToUpper() == "TRUE")
                        {
                            //Dictionary<string, object> dicvalues = new Dictionary<string, object>();
                            //dicvalues.Add("ROLEID", roleTree.SelectedNode.Name);
                            //dicvalues.Add("DATAPRI_ID", aNode.Name);
                            //sysTable.NewRow("role_Data", dicvalues, out eError);
                            CheckeddbsourceNode(sysTable, aNode);
                        }
                        else if (aNode.Cells[1].Checked.ToString().ToUpper() == "FALSE")
                        {
                            //sysTable.DeleteRows("role_Data", "ROLEID='" + roleTree.SelectedNode.Name + "' and DATAPRI_ID='" + aNode.Name + "'", out eError);
                            unCheckeddbsourceNode(sysTable, aNode);
                        }
                        else
                        { return; }
                    }
                }
                else
                {
                    ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ѡ���ɫ��");
                }
            }
            else
            {
                ModuleOperator.IsCheckChanged = true;
            }

        }

        private void MenuDbsourceSelAll_Click(object sender, EventArgs e)
        {
            SysGisTable sysTable = new SysGisTable(ModData.gisDb);
            DevComponents.AdvTree.Node aNode = dbSourceTree.SelectedNode;

            SelectAllpriNode(sysTable, aNode);
        }

        private void MenuDbsourceunSelAll_Click(object sender, EventArgs e)
        {
            SysGisTable sysTable = new SysGisTable(ModData.gisDb);
            DevComponents.AdvTree.Node aNode = dbSourceTree.SelectedNode;

            unSelectAllpriNode(sysTable, aNode);
        }

        //����Ȩ�޽ڵ�ȫѡ���ܣ���ǰ�ڵ㼰�����²�ڵ㣩
        private void SelectAllDbsourceNode(SysGisTable sysTable, DevComponents.AdvTree.Node aNode)
        {
            if (roleTree.SelectedNode == null)
            {
                ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ѡ���ɫ��");
                return;
            }
            if (aNode == null)
                return;
            Exception eError;
            Dictionary<string, object> dicvalues = new Dictionary<string, object>();
            if (aNode.Cells[1].Checked == false)
            {
                dicvalues.Add("ROLEID", roleTree.SelectedNode.Name);
                dicvalues.Add("DBSOURCE_ID", aNode.Name);
                if (!sysTable.ExistData("role_dbsource", "ROLEID='" + roleTree.SelectedNode.Name + "' and DBSOURCE_ID=" + aNode.Name))
                {
                    sysTable.NewRow("ROLE_DBSOURCE", dicvalues, out eError);
                }
            }
            aNode.Cells[1].Checked = true;
            if (aNode.Nodes.Count > 0)
            {
                DevComponents.AdvTree.NodeCollection pCollection = aNode.Nodes as DevComponents.AdvTree.NodeCollection;
                foreach (DevComponents.AdvTree.Node eachnode in pCollection)
                {
                    //eachnode.Cells[1].Checked = true;
                    SelectAlldataNode(sysTable, eachnode);
                }
            }
        }
        //����Ȩ�޽ڵ�ȫ��ѡ���ܣ���ǰ�ڵ㼰�����²�ڵ㣩
        private void unSelectAllDbsourceNode(SysGisTable sysTable, DevComponents.AdvTree.Node aNode)
        {
            if (roleTree.SelectedNode == null)
            {
                ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ѡ���ɫ��");
                return;
            }
            if (aNode == null)
                return;
            Exception eError;
            Dictionary<string, object> dicvalues = new Dictionary<string, object>();
            if (aNode.Cells[1].Checked == true)
            {   //��������ѡ�񣬴Ӹý�ɫ��Ȩ�ޱ����ɾ��������¼
                sysTable.DeleteRows("role_DBSOURCE", "ROLEID='" + roleTree.SelectedNode.Name + "' and DBSOURCE_ID=" + aNode.Name, out eError);

            }
            aNode.Cells[1].Checked = false;
            if (aNode.Nodes.Count > 0)
            {
                DevComponents.AdvTree.NodeCollection pCollection = aNode.Nodes as DevComponents.AdvTree.NodeCollection;
                foreach (DevComponents.AdvTree.Node eachnode in pCollection)
                {
                    //eachnode.Cells[1].Checked = true;
                    //�ݹ�ִ��ȫ��ѡ����
                    unSelectAlldataNode(sysTable, eachnode);
                }
            }
        }

        private void DataTree_NodeClick(object sender, DevComponents.AdvTree.TreeNodeMouseEventArgs e)
        {
            DataTree.SelectedNode = e.Node;
        }

        private void dbSourceTree_NodeClick(object sender, DevComponents.AdvTree.TreeNodeMouseEventArgs e)
        {
            dbSourceTree.SelectedNode = e.Node;
        }

        private void roleTree_NodeClick(object sender, DevComponents.AdvTree.TreeNodeMouseEventArgs e)
        {
            roleTree.SelectedNode = e.Node;
        }
        //ˢ��ͼ��Ŀ¼�б�
        private void MenuDataRefresh_Click(object sender, EventArgs e)
        {
            //��ͼ��Ŀ¼�ӹ����⿽��������Ŀ¼
            ModuleOperator.CopyLayerTreeXmlFromDataBase(Fan.Plugin.ModuleCommon.TmpWorkSpace, ModData.m_DataXmlPath);
            XmlDocument datadoc = new XmlDocument();
            datadoc.Load(ModData.m_DataXmlPath);
            if (datadoc != null)
            {
                ModuleOperator.DisplayInDataLstView(datadoc, DataTree);
            }
            if (roleTree.SelectedNode == null)
                return;
            DevComponents.AdvTree.Node selectNode = roleTree.SelectedNode;

            //����Ȩ������
            Exception eError = null;
            SysGisTable sysTable = new SysGisTable(ModData.gisDb);
            List<Dictionary<string, object>> lstDicDataData = sysTable.GetRows("role_data", "ROLEID='" + selectNode.Name.ToLower() + "'", out eError);
            List<string> lstdataPrivilege = new List<string>();
            if (lstDicDataData.Count != 0)
            {
                for (int i = 0; i < lstDicDataData.Count; i++)
                {
                    lstdataPrivilege.Add(lstDicDataData[i]["DATAPRI_ID"].ToString());
                }
                for (int i = 0; i < DataTree.Nodes.Count; i++)
                {
                    DevComponents.AdvTree.Node treeNode = DataTree.Nodes[i];
                    ModuleOperator.IsCheckChanged = false;
                    if (lstdataPrivilege.Contains(treeNode.Name))
                    {
                        treeNode.Cells[1].Checked = true;
                    }
                    else
                    {
                        treeNode.Cells[1].Checked = false;
                    }
                    if (treeNode.Tag.ToString() == treeNode.Cells[1].Checked.ToString())
                    {
                        ModuleOperator.IsCheckChanged = true;
                    }
                    else
                    {
                        treeNode.Tag = treeNode.Cells[1].Checked;
                    }
                    if (treeNode.HasChildNodes)
                    {
                        ModuleOperator.UpdateRolePrivilege(lstdataPrivilege, treeNode);
                    }
                }
            }
            else
            {
                for (int i = 0; i < DataTree.Nodes.Count; i++)
                {
                    DevComponents.AdvTree.Node treeNode = DataTree.Nodes[i];
                    ModuleOperator.IsCheckChanged = false;
                    treeNode.Cells[1].Checked = false;
                    if (treeNode.Tag.ToString() == treeNode.Cells[1].Checked.ToString())
                    {
                        ModuleOperator.IsCheckChanged = true;
                    }
                    else
                    {
                        treeNode.Tag = treeNode.Cells[1].Checked;
                    }
                    if (treeNode.HasChildNodes)
                    {
                        ModuleOperator.UpdateRolePrivilege(lstdataPrivilege, treeNode);
                    }
                }
            }
        }
        //ˢ������Դ�б�
        private void MenuDbsourceRefresh_Click(object sender, EventArgs e)
        {
            ModuleOperator.DisplayInDbsourceLstView(this.dbSourceTree);
            if(roleTree.SelectedNode==null)
                return;
            //�����ѡ�еĽ�ɫ�ڵ㣬����������Դ�б�ڵ�ѡ��״̬��ʾ��ˢ�º���б���
            DevComponents.AdvTree.Node selectNode=roleTree.SelectedNode;
            //����ԴȨ������
            Exception eError = null;
            SysGisTable sysTable = new SysGisTable(ModData.gisDb);
            //�����ݿ��л�ȡ���µ�ѡ��ID�б�
            List<Dictionary<string, object>> lstDicDbsource = sysTable.GetRows("role_dbsource", "ROLEID='" + selectNode.Name.ToLower() + "'", out eError);
            List<string> lstdbsource = new List<string>();
            if (lstDicDbsource != null)
            {
                if (lstDicDbsource.Count != 0)
                {
                    for (int i = 0; i < lstDicDbsource.Count; i++)
                    {
                        lstdbsource.Add(lstDicDbsource[i]["DBSOURCE_ID"].ToString());
                    }
                    for (int i = 0; i < dbSourceTree.Nodes.Count; i++)
                    {
                        DevComponents.AdvTree.Node treeNode = dbSourceTree.Nodes[i];
                        ModuleOperator.IsCheckChanged = false;
                        if (lstdbsource.Contains(treeNode.Name))
                        {
                            treeNode.Cells[1].Checked = true;
                        }
                        else
                        {
                            treeNode.Cells[1].Checked = false;
                        }
                        if (treeNode.Tag.ToString() == treeNode.Cells[1].Checked.ToString())
                        {
                            ModuleOperator.IsCheckChanged = true;
                        }
                        else
                        {
                            treeNode.Tag = treeNode.Cells[1].Checked;
                        }
                        if (treeNode.HasChildNodes)
                        {
                            ModuleOperator.UpdateRolePrivilege(lstdbsource, treeNode);
                        }
                    }
                }
                else//��û���κ�ѡ�еģ���ȫ������Ϊ��ѡ��״̬
                {
                    for (int i = 0; i < dbSourceTree.Nodes.Count; i++)
                    {
                        DevComponents.AdvTree.Node treeNode = dbSourceTree.Nodes[i];
                        ModuleOperator.IsCheckChanged = false;
                        treeNode.Cells[1].Checked = false;
                        if (treeNode.Tag.ToString() == treeNode.Cells[1].Checked.ToString())
                        {
                            ModuleOperator.IsCheckChanged = true;
                        }
                        else
                        {
                            treeNode.Tag = treeNode.Cells[1].Checked;
                        }
                        if (treeNode.HasChildNodes)
                        {
                            ModuleOperator.UpdateRolePrivilege(lstdbsource, treeNode);
                        }
                    }
                }
            }
        }
 
    }
}
