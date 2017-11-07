using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;
//?using GeoDataCenterFrame;
using System.Xml;
using System.IO;
using System.Data.OleDb;
using GeoDataCenterFunLib;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesGDB;

namespace GeoDBConfigFrame
{
    public partial class FaceControl : UserControl
    {
        //�Ҽ��˵�����  
        //Ŀǰ�Ҽ��˵�������
      // private Dictionary<string, DevComponents.DotNetBar.ContextMenuBar> _dicContextMenu;
        public OleDbDataAdapter m_Adapter;
        public DataTable m_dataTable;
        public string m_connstr;
        public string m_TableName;
        public string m_strLogFilePath = Application.StartupPath + "\\..\\Log\\DataManagerLog.txt";

        //��ͼ�������������
        private Control _MapToolControl;
        public FaceControl(string strName, string strCation)
        {
            InitializeComponent();

          
            //��ʼ�����ö�Ӧ��ͼ�ؼ�
            InitialMainViewControl();
            this.Dock = System.Windows.Forms.DockStyle.Fill;

            this.Name = strName;
            this.Tag = strCation;
            ModFrameData.v_AppPrivileges.MainUserControl = this;
            ModFrameData.v_AppPrivileges.DataTabIndexTree = DataIndexTree;
            ModFrameData.v_AppPrivileges.GridCtrl  = gridControl;
            ModFrameData.v_AppPrivileges.tipRichBox = tipRichBox;
            //��־�ļ�·��
            ModFrameData.v_AppPrivileges.strLogFilePath = m_strLogFilePath;
           // ModFrameData.v_AppPrivileges.MainUserControl = this;

            //����sys�����ļ���Ӳ˵��͹�����
            InitialFrmDefControl();      
        }

        public void InitialMainViewControl()
        {
            frmBarManager newfrmBarManager = new frmBarManager();
            newfrmBarManager.TopLevel = false;
            newfrmBarManager.Dock = DockStyle.Fill;
            newfrmBarManager.Show();
            this.Controls.Add(newfrmBarManager);

            //��ʼ�����ڵ�
            InitIndexTree();
      

            //��ʾ
            //���������ļ���ȡҪ����������Ϣ��

            //��������������������
            DevComponents.DotNetBar.Bar barIndexView = newfrmBarManager.CreateBar("barIndexView", enumLayType.FILL);
            barIndexView.CanHide = false;
            DevComponents.DotNetBar.PanelDockContainer PanelIndexView = newfrmBarManager.CreatePanelDockContainer("PanelIndexView", barIndexView);
            PanelIndexView.Controls.Add(this.tabControlIndex);
            this.tabControlIndex.Dock = DockStyle.Fill;


            //����������ͼ����
            DevComponents.DotNetBar.Bar barMapView = newfrmBarManager.CreateBar("barMapView", enumLayType.FILL);
            barMapView.CanHide = false;
            DevComponents.DotNetBar.PanelDockContainer PanelMapView = newfrmBarManager.CreatePanelDockContainer("PanelMapView", barMapView);
            PanelMapView.Controls.Add(this.tabControlData);
            this.tabControlData.Dock = DockStyle.Fill;
            _MapToolControl = PanelMapView as Control;

            //��������
            newfrmBarManager.MainDotNetBarManager.FillDockSite.GetDocumentUIManager().Dock(barIndexView, barMapView, eDockSide.Right);
            newfrmBarManager.MainDotNetBarManager.FillDockSite.GetDocumentUIManager().SetBarWidth(barIndexView, this.Width / 5);

            //����������ʾ����
            //�û�������
      /*      PanelDockContainer PanelTipData = new PanelDockContainer();
            PanelTipData.Controls.Add(this.tipRichBox);
            this.tipRichBox.Dock = DockStyle.Fill;
            DockContainerItem dockItemData = new DockContainerItem("dockItemData", "��ʾ");
            dockItemData.Control = PanelTipData;
            newfrmBarManager.ButtomBar.Items.Add(dockItemData);   */
        }

        //��ʼ���������ڵ�
        public void InitIndexTree()
        {
            this.DataIndexTree.Nodes.Clear();
            TreeNode tparent;
            tparent = new TreeNode();
            tparent.Text = "��������";
            tparent.Tag = 0;
            tparent.ImageIndex = 0;
            this.DataIndexTree.Nodes.Add(tparent);
            this.DataIndexTree.ExpandAll();
  

            //�����ӽڵ�
            TreeNode tNewNode;
            GetDataTreeInitIndex  dIndex = new GetDataTreeInitIndex();
        
           //������ȡitemName��Ϣ 
            string  strTblName = "";
            XmlDocument xmldoc = new XmlDocument();
            if (xmldoc != null)
            {
                if (File.Exists(dIndex.m_strInitXmlPath))
                {
                    xmldoc.Load(dIndex.m_strInitXmlPath);

                    //�޸ĸ��ڵ�ڵ�����
                    string strRootName = "";
                    string strSearchRoot = "//Rootset";
                    XmlNode xmlNodeRoot = xmldoc.SelectSingleNode(strSearchRoot);
                    XmlElement xmlElentRoot = (XmlElement)xmlNodeRoot;
                    strRootName = xmlElentRoot.GetAttribute("ItemName");
                    tparent.Text = strRootName;

                    //������ӵ�һ���ӽڵ� Childset
                    string strSearch = "//Childset";
                    XmlNode xmlNode = xmldoc.SelectSingleNode(strSearch);
                    XmlNodeList xmlNdList;
                    xmlNdList = xmlNode.ChildNodes;
                    foreach (XmlNode xmlChild in xmlNdList)
                    {
                        strTblName = "";
                        XmlElement xmlElent = (XmlElement)xmlChild;
                        strTblName = xmlElent.GetAttribute("ItemName");

                        tNewNode = new TreeNode();
                        tNewNode.Text = strTblName;
                        tNewNode.Tag = 1;
                        tNewNode.ImageIndex =0;
                        tparent.Nodes.Add(tNewNode);
                        tparent.ExpandAll();

                        //��������ӽڵ�
                        AddLeafItem(tNewNode, xmlChild);
                    }
                }
            }  
        }

        //���Ҷ�ӽڵ�
        public void AddLeafItem(TreeNode treeNode, XmlNode xmlNode)
        {
            if (treeNode != null && xmlNode != null)
            {
                 TreeNode tNewNode;
                 string strTblName = "";
    
                 XmlNodeList xmlNdList;
                 xmlNdList = xmlNode.ChildNodes;
                 foreach (XmlNode xmlChild in xmlNdList)
                 {
                     strTblName = "";
                     XmlElement xmlElent = (XmlElement)xmlChild;
                     strTblName = xmlElent.GetAttribute("ItemName");
                     tNewNode = new TreeNode();
                     tNewNode.Text = strTblName;
                     tNewNode.Tag = 2;
                     tNewNode.ImageIndex = 1;
                     treeNode.Nodes.Add(tNewNode);              
                }
                treeNode.ExpandAll();
            }
        }

        //��ʼ����ܲ���ؼ�����
        //����sys�����ļ���Ӳ˵��͹�����
        public void InitialFrmDefControl()
        {
            ////�õ�Xml��System�ڵ�,����XML���ز������
            string xPath = ".//System[@Name='" + this.Name + "']";
            Plugin.ModuleCommon.LoadButtonViewByXmlNode(ModFrameData.v_AppPrivileges.ControlContainer, xPath, ModFrameData.v_AppPrivileges);

            ////�Ҽ��˵�
         //   _dicContextMenu = ModFrameData.v_AppPrivileges.DicContextMenu;
            //��ʼ����ͼ���������
            //Plugin.Application.IAppFormRef pAppFrm = ModFrameData.v_AppPrivileges as Plugin.Application.IAppFormRef;
            //XmlNode barXmlNode = pAppFrm.SystemXml.SelectSingleNode(".//ToolBar[@Name='ControlMapToolBar9']");
            //if (barXmlNode == null || _MapToolControl == null) return;
            ////DevComponents.DotNetBar.Bar mapToolBar = Plugin.ModuleCommon.LoadButtonView(_MapToolControl, barXmlNode, pAppFrm, null, false) as Bar;
            //DevComponents.DotNetBar.Bar mapToolBar = Plugin.ModuleCommon.LoadButtonView(_MapToolControl, barXmlNode, pAppFrm, null) as Bar;
            //if (mapToolBar != null)
            //{
            //    mapToolBar.AccessibleRole = System.Windows.Forms.AccessibleRole.ToolBar;
            //    mapToolBar.DockOrientation = DevComponents.DotNetBar.eOrientation.Vertical;
            //    mapToolBar.DockSide = DevComponents.DotNetBar.eDockSide.Left;
            //    mapToolBar.GrabHandleStyle = DevComponents.DotNetBar.eGrabHandleStyle.Office2003;
            //    mapToolBar.Style = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            //}


            //��ʼ����ͼ���������
           Plugin.Application.IAppFormRef pAppFrm = ModFrameData.v_AppPrivileges as Plugin.Application.IAppFormRef;
            XmlNode barXmlNode = pAppFrm.SystemXml.SelectSingleNode(".//ToolBar[@Name='ControlMapToolBar4']");
            if (barXmlNode == null || _MapToolControl == null)
                return;
             DevComponents.DotNetBar.Bar mapToolBar = Plugin.ModuleCommon.LoadButtonView(_MapToolControl, barXmlNode, pAppFrm, null) as Bar;
            if (mapToolBar != null)
            {
                mapToolBar.AccessibleRole = System.Windows.Forms.AccessibleRole.ToolBar;
                mapToolBar.DockOrientation = DevComponents.DotNetBar.eOrientation.Vertical;
                mapToolBar.DockSide = DevComponents.DotNetBar.eDockSide.Left;
                mapToolBar.GrabHandleStyle = DevComponents.DotNetBar.eGrabHandleStyle.None;
                mapToolBar.Style = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
                mapToolBar.RoundCorners = false;
                mapToolBar.SendToBack();
            }
        }



        //=================================
        //���ߣ�ϯʤ 
        //ʱ�䣺2011-02-21
        //˵���������ѡ���ڵ���Ӧ
        //=================================
        public void DataIndexTree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            //�ж����ڵ㼶��
            if (DataIndexTree.SelectedNode != e.Node)
            {
                if (DataIndexTree.SelectedNode != null)
                {
                    DataIndexTree.SelectedNode.ForeColor = Color.Black;

                }

                DataIndexTree.SelectedNode = e.Node;
                e.Node.ForeColor = Color.Red;
                switch (DataIndexTree.SelectedNode.Tag.ToString())
                {
                    case "0":
                        DataIndexTree.SelectedNode.SelectedImageIndex = 0;
                        break;
                    case "1":
                        DataIndexTree.SelectedNode.SelectedImageIndex = 0;
                        break;
                    case "2":
                        DataIndexTree.SelectedNode.SelectedImageIndex = 2;
                        break;
                }
            }
            string  strItemName = "";
            string  strTblName = "";
            strItemName = e.Node.Text;
            
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            strTblName = dIndex.GetTblNameByItemName(strItemName);
            m_TableName = strTblName;
            //����tblName��ȡ��Ӧ��������Ϣ��䵽�б���
            InitDataInfoList(strTblName);  

            //�޸�tabҳ�����ʾ����
             this.tabItemData.Text = strItemName;
            
        }
        public bool getEditable()
        {
            TreeNode pSelectNode = DataIndexTree.SelectedNode;
            if (pSelectNode == null)
                return false;
            if (pSelectNode.Level == 2)
            {
                if (pSelectNode.Parent.Index == 0)
                    return true;
                else
                    return false;
            }
            return false;
        }

        //��ITableת��ΪDataTable
        private DataTable Transfer(ITable table)
        {
            DataTable dt = new DataTable();
            try
            {
                IQueryFilter queryFilter = new QueryFilterClass();
                ICursor pCursor = table.Search(queryFilter, true);
                IRow pRow = pCursor.NextRow();
                if (pRow != null)
                {
                    DataColumn dataColumn = null;
                    for (int i = 0; i < pRow.Fields.FieldCount; i++)
                    {
                       
                        if (pRow.Fields.get_Field(i).AliasName != "OBJECTID")
                        {
                            dataColumn = dt.Columns.Add(pRow.Fields.get_Field(i).AliasName);
                            dataColumn.ReadOnly = true;
                        }
                        else
                        {
                            dt.Columns.Add(pRow.Fields.get_Field(i).AliasName);
                            //dataColumn.Unique = true;
                        }
                    }
                    while (pRow != null)
                    {
                        DataRow pDataRow = dt.NewRow();
                        for (int j = 0; j < pCursor.Fields.FieldCount; j++)
                        {
                            pDataRow[j] = pRow.get_Value(j);
                        }
                        dt.Rows.Add(pDataRow);
                        pRow = pCursor.NextRow();
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("ת������" + ex.Message, "��ʾ");
            }
            return dt;
        }

          //���أ�added by xisheng 2011.06.16
        public void InitDataInfoList(string strTblName, bool boolreturn)
        {
            if (strTblName.Equals(""))
                return;
            IWorkspace pTmpWorkSpace = Plugin.ModuleCommon.TmpWorkSpace;
            if (pTmpWorkSpace == null)
            {
                return;
            }
            ModDBOperate.boolreturn = boolreturn;
            DataTable pDatatable = ModDBOperate.GetQueryTable(pTmpWorkSpace as IFeatureWorkspace, strTblName, "");
            m_dataTable = pDatatable;
            this.gridControl.DataSource = null;
            this.gridControl.DataSource = pDatatable;
        }

        //����tblName��ȡ��Ӧ��������Ϣ��䵽listview��
        public void InitDataInfoList(string strTblName)
        {
            if (strTblName.Equals(""))
                return;
            IWorkspace pTmpWorkSpace = Plugin.ModuleCommon.TmpWorkSpace;
            if (pTmpWorkSpace == null)
            {
                return;
            }
           /* SysCommon.Gis.SysGisTable pSystable = new SysCommon.Gis.SysGisTable(pTmpWorkSpace);
            Exception eError = null;
            ITable pTable = pSystable.OpenTable(strTblName, out eError);
            if (pTable == null)
            {
                return;
            }
            DataTable pDatatable = new DataTable();
            DataRow pRow = pDatatable.NewRow();
            DataColumn pColumn = new DataColumn();

            this.gridControl.DataSource = null;
            this.gridControl.DataSource = pTable;*/
            DataTable pDatatable = ModDBOperate.GetQueryTable(pTmpWorkSpace as IFeatureWorkspace, strTblName, "");
            m_dataTable = pDatatable;
            this.gridControl.DataSource = null;
            this.gridControl.DataSource = pDatatable;
            
            //Ŀǰֱ�Ӷ�ȡmdb�ļ�����Ҫ�޸�Ϊ����ģʽ
/*            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            string mypath = dIndex.GetDbInfo();
            //string  mypath = dIndex.GetDbValue("dbServerPath");
            string constr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
            m_connstr = constr;
            OleDbConnection mycon = new OleDbConnection(constr);   //����OleDbConnection����ʵ�����������ݿ�
            string strExp = "";
            strExp = "select * from " + strTblName;  
            OleDbCommand aCommand = new OleDbCommand(strExp, mycon);     
            try
            {
                mycon.Open();

                //����datareader   ���������ӵ���     
                OleDbDataReader aReader = aCommand.ExecuteReader();
                DataTable dt = new DataTable();
                OleDbDataAdapter da = new OleDbDataAdapter(strExp, constr);
                m_Adapter = null;
                m_dataTable = null;
                m_Adapter = da;
                m_dataTable = dt;
                da.Fill(dt);
                this.gridControl.DataSource = null;
                this.gridControl.DataSource = dt;
                
                
                //�ر�reader����     
                aReader.Close();

                //�ر�����,�����Ҫ     
                mycon.Close();    

            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
            }
            */

            //��ȡ���ݿ��е�����


        }

        //��Ԫ�������
        private void gridControl_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {

            //�Ҽ������ʱ�򵯳��Ҽ��˵�
            //changed by chulili 2011-02-22 gridControl�е�����(e.X,e.Y)������ڱ���Ԫ������꣬����Ҫ����ת��
            if (e.Button == MouseButtons.Right)
            {
                if (e.RowIndex <= -1)
                    return;
  
                if (gridControl.Rows[e.RowIndex].Selected !=true)
                {
                    gridControl.ClearSelection();
                    if (e.RowIndex > -1)
                    {
                        gridControl.Rows[e.RowIndex].Selected = true;   //�ı�ѡ���е�ָ����
                        DataGridViewRow cr = gridControl.Rows[e.RowIndex];
                        if (e.ColumnIndex > -1)
                        {
                            gridControl.CurrentCell = cr.Cells[e.ColumnIndex];    //added by chulili 2011-02-25�ı����׷���
                        }
                    }                    
                    
                }
                Point p = new Point();
                if (e.RowIndex > -1)
                    p.Y = e.Y + e.RowIndex * this.gridControl.Rows[0].Height + this.gridControl.ColumnHeadersHeight;
                else
                    p.Y = e.Y;

                if (e.ColumnIndex > -1)
                    p.X = e.X + e.ColumnIndex * this.gridControl.Columns[0].Width + this.gridControl.RowHeadersWidth;
                else
                    p.X = e.X;

                Point ClickPoint = this.gridControl.PointToScreen(p);

               //? contextMenuStrip.Show(ClickPoint);
            }
        }

        private void MenuItemAddRcd_Click(object sender, EventArgs e)
        {   //added by chulili 2011-02-24 ��Ӽ�¼����

            TableForm myTableForm = new TableForm(gridControl, m_connstr, m_TableName);
            myTableForm.InitForm("ADD");
            myTableForm.ShowDialog();
            OleDbCommandBuilder builder = new OleDbCommandBuilder(m_Adapter);
            //�������ύ�����ݿ����

            try
            {
                m_Adapter.Update(m_dataTable);
                //�ύ�����»�ȡ���ݣ�����ID�е�ֵ�����ݿ��в�һ��
                m_dataTable.Clear();
                m_Adapter.Fill(m_dataTable);
                gridControl.DataSource = null;
                gridControl.DataSource = m_dataTable;
                //gridControl.Update();
            }
            catch (System.Exception m)
            {
                Console.WriteLine(m.Message);
            }

            

        }

        private void MenuItemModifyRcd_Click(object sender, EventArgs e)
        {   //added by chulili 2011-02-24 �޸ļ�¼����
            if (gridControl.SelectedRows.Count ==0)
            {
                MessageBox.Show("δѡ�м�¼��");
                return;
            }
            if (gridControl.SelectedRows.Count > 1)
            {
                MessageBox.Show("�����޸�һ�У�");
                return;
            }

            TableForm myTableForm = new TableForm(gridControl, m_connstr, m_TableName);
            myTableForm.InitForm("MODIFY");
            myTableForm.ShowDialog();

            InitDataInfoList(m_TableName);

            //OleDbCommandBuilder builder = new OleDbCommandBuilder(m_Adapter);
            //////�������ύ�����ݿ����

            //try
            //{
            //    m_Adapter.Update(m_dataTable);
            //    //�ύ�����»�ȡ���ݣ�����ID�е�ֵ�����ݿ��в�һ��
            //    m_dataTable.Clear();
            //    m_Adapter.Fill(m_dataTable);
            //    gridControl.DataSource = null;
            //    gridControl.DataSource = m_dataTable;
            //}
            //catch (System.Exception m)
            //{
            //    Console.WriteLine(m.Message);
            //}

                     

        }

        private void MenuItemDelRcd_Click(object sender, EventArgs e)
        {   //added by chulili 2011-02-24ɾ��ָ����¼����
            int k = gridControl.SelectedRows.Count;
            if (gridControl.SelectedRows.Count ==0)
            {
                MessageBox.Show("δѡ�м�¼��");
                return;
            }
            if (this.gridControl.SelectedRows.Count > 0)
            {
                if (MessageBox.Show("ȷ��Ҫɾ��ѡ�еļ�¼��", "��ʾ", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    return;
            }
            OleDbConnection mycon = new OleDbConnection(m_connstr);   //����OleDbConnection����ʵ�����������ݿ�
            string strExp = "";
            OleDbCommand aCommand = new OleDbCommand(strExp, mycon);
            mycon.Open();
            int i = 0, j = 0;
            if (gridControl.Rows.Count > 0)
            {
                for (j = k; j >= 1; j--)//��������ɾ������ɳ©ЧӦ
                {
                    strExp = "delete from  " + m_TableName + " where";
                    for (i = 0; i < gridControl.ColumnCount; i++)
                    {
                        if (gridControl.SelectedRows[j - 1].Cells[i].Value.ToString().Equals(""))
                            strExp = strExp + " (" + gridControl.Columns[i].Name + "='' or " + gridControl.Columns[i].Name + " is null) and";
                        else 
                            strExp = strExp + " " + gridControl.Columns[i].Name + "='" + gridControl.SelectedRows[j-1].Cells[i].Value.ToString() + "' and";
                    }
                    strExp = strExp.Substring(0, strExp.Length - 3);
                    aCommand.CommandText = strExp;
                    try
                    {
                        aCommand.ExecuteNonQuery();
                    }
                    catch (System.Exception err)
                    {
                        Console.WriteLine(err.Message);
                    }
                 
                    
                }
            }
            else
            {
                gridControl.Rows.Clear();
            }
            mycon.Close();
            InitDataInfoList(m_TableName);
            //OleDbCommandBuilder builder = new OleDbCommandBuilder(m_Adapter);
            ////�������ύ�����ݿ����

            //try
            //{
            //    m_Adapter.Update(m_dataTable);
            //    gridControl.Update();
            //}
            //catch (System.Exception m)
            //{
            //    Console.WriteLine(m.Message);
            //}

            
        }

        private void MenuItemDelAll_Click(object sender, EventArgs e)
        {   //added by chulili 2011-02-25ɾ��ȫ����¼����
            if (MessageBox.Show("ȷ��Ҫȫ��ɾ����ɾ���󲻿ɻָ���", "��ʾ", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;
            OleDbConnection mycon = new OleDbConnection(m_connstr);   //����OleDbConnection����ʵ�����������ݿ�
            string strExp = "delete from " + m_TableName ;
            OleDbCommand aCommand = new OleDbCommand(strExp, mycon);
            mycon.Open();
            aCommand.ExecuteNonQuery();
            mycon.Close();
            InitDataInfoList(m_TableName);
            // int k = gridControl.Rows.Count;
            // if (gridControl.Rows.Count > 0)
            // {
            //     for (int i = k; i >= 1; i--)//��������ɾ������ɳ©ЧӦ
            //     {
            //         gridControl.Rows.RemoveAt(gridControl.Rows[i-1].Index);
                     
            //     }
            // }
            // else
            // {
            //     gridControl.Rows.Clear();
            // }
            ////���Ķ��ύ�����ݿ�
            // OleDbCommandBuilder builder = new OleDbCommandBuilder(m_Adapter);
            // try
            // {
            //     m_Adapter.Update(m_dataTable);
            //     gridControl.Update();
            // }
            // catch (System.Exception m)
            // {
            //     Console.WriteLine(m.Message);
            // }
        }

        private void ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmDetailLog frm = new frmDetailLog(m_strLogFilePath);
            frm.Show();
        }
        /// <summary>
        /// ����˫�������༭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridControl_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (gridControl.DataSource == null)
                return;
            //���δѡ�м�¼������
            if (gridControl.SelectedRows.Count == 0)
            {
                MessageBox.Show("δѡ�м�¼��");
                return;
            }
            //���ѡ�ж��м�¼������
            if (gridControl.SelectedRows.Count > 1)
            {
                MessageBox.Show("�����޸�һ�У�");
                return;
            }
            int idx = gridControl.SelectedRows[0].Index;//yjl20111103 add
            //��ʼ���޸ļ�¼�Ի���
            //��ʼ����Ӽ�¼�Ի���
            TableForm myTableForm = new TableForm(gridControl, m_connstr, m_TableName);
            myTableForm.InitForm("MODIFY");
            myTableForm.ShowDialog();
            //��¼�޸ĺ��ٴγ�ʼ��dataview�ؼ�
            InitDataInfoList(m_TableName);
            ////ZQ   20111017   add  ��ʱ���������ֵ�
            switch (m_TableName)
            {
                case "���Զ��ձ�":
                    SysCommon.ModField.InitNameDic(Plugin.ModuleCommon.TmpWorkSpace, SysCommon.ModField._DicFieldName, "���Զ��ձ�");
                    break;
                case "��׼ͼ������":
                    SysCommon.ModField.InitLayerNameDic(Plugin.ModuleCommon.TmpWorkSpace, SysCommon.ModField._DicLayerName);
                    break;
                //default:
                //    ///ZQ 20111020 add ����������ʾ
                //    MessageBox.Show("��ӵļ�¼ֻ��Ӧ��ϵͳ�����Ժ������Ч��","��ʾ��");
                //    break;
            }
            //yjl20111103 add 
            if (gridControl.Rows.Count > idx)
            {
                //ȡ��Ĭ�ϵĵ�1��ѡ��
                gridControl.Rows[0].Selected = false;
                //����ѡ����
                gridControl.Rows[idx].Selected = true;
                //���õ�ǰ��
                gridControl.CurrentCell = gridControl.Rows[idx].Cells[0];
            }
            /////

            Plugin.LogTable.Writelog("�޸ļ�¼");//xisheng 2011.07.09 ������־
        }

    }
}