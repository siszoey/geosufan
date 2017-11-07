using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Data.OleDb;
using System.Collections;
using DevComponents.DotNetBar;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;

using System.IO;
using SysCommon.Gis;
namespace GeoDBIntegration
{
    /// <summary>
    /// ���ɹ�����ϵͳ����   ���Ƿ� 20100927
    /// </summary>
    public partial class UserControlDBIntegra : UserControl
    {

        //�Ҽ��˵�����
        private Dictionary<string, DevComponents.DotNetBar.ContextMenuBar> _dicContextMenu;

        //��ͼ�������������
        private Control _MapToolControl;
        private Control _DBControl;

        private bool m_bGroupLayerVisible;   //���ƽ��GroupLayer��ʾ����(�����д��Ľ� )
        //��ʼ��������
        public UserControlDBIntegra(string strName, string strCation)
        {

            InitializeComponent();
            //********************************************************************************//
            //��ʼ�����ݿ������
            if (null == ModuleData.v_DataBaseProPanel) ModuleData.v_DataBaseProPanel = new clsDatabasePanel(this.groupPanel1);
            //********************************************************************************//
            //**************************************
            //2010-11-22 guozheng added  ϵͳ������־
            ModuleData.v_SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
            if (SysCommon.Log.Module.SysLog == null)
            {
                SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Initial();
            }
            SysCommon.Log.Module.SysLog.Write("�������ݿ������ϵͳ", null, DateTime.Now);
            //*************************************
            //��ʼ�����ö�Ӧ��ͼ�ؼ�
            InitialMainViewControl();

            this.Name = strName;
            this.Tag = strCation;
            this.Dock = System.Windows.Forms.DockStyle.Fill;

            axMapControl1.Map.Name = "������ͼ";
            axTOCControl1.SetBuddyControl(axMapControl1.Object);
            advTreeProject.ImageList = IconContainer;


            ModuleData.v_AppDBIntegra.MainUserControl = this;
            ModuleData.v_AppDBIntegra.TOCControl = axTOCControl1.Object as ITOCControlDefault;
            ModuleData.v_AppDBIntegra.MapControl = axMapControl1.Object as IMapControlDefault;
            ModuleData.v_AppDBIntegra.ArcGisMapControl = axMapControl1;
            ModuleData.v_AppDBIntegra.ProjectTree = advTreeProject;
            /////*********************************************************
            ////guozheng  2010-10-8
            ModuleData.v_AppDBIntegra.cmbImageDB = this.cmbImageDB;
            ModuleData.v_AppDBIntegra.cmbFileDB = this.cmbFileDB;
            ModuleData.v_AppDBIntegra.cmbFeatureDB = this.cmbFeatureDB;
            ModuleData.v_AppDBIntegra.cmbEntiDB = this.cmbEntiDB;
            ModuleData.v_AppDBIntegra.cmbDemDB = this.cmbDemDB;
            ModuleData.v_AppDBIntegra.cmbAdressDB = this.cmbAdressDB;
            //**********************************************************


            //������Ϣ
            btnDemDB.Enabled = false;
            btnEntiDB.Enabled = false;
            btnFeaDB.Enabled = false;
            btnFileDB.Enabled = false;
            btnImageDB.Enabled = false;
            btnMapDB.Enabled = false;
            btnSubjectDB.Enabled = false;
            bnAddressDB.Enabled = false;


            //��ʼ����ܲ���ؼ�����
            InitialFrmDefControl();

            //��ձ���xml�ļ�����������ȡ���ݿ�ˢ�½���xml�ļ�
            if (File.Exists(ModuleData.v_feaProjectXML))
            {
                //���Ҫ�ؿ���ϵͳ
                File.Delete(ModuleData.v_feaProjectXML);
            }
            if (File.Exists(ModuleData.v_ImageProjectXml))
            {
                //�߳����ݿ���ϵͳ
                File.Delete(ModuleData.v_ImageProjectXml);
            }
            if (File.Exists(ModuleData.v_DEMProjectXml))
            {
                //Ӱ�����ݿ���ϵͳ
                File.Delete(ModuleData.v_DEMProjectXml);
            }


            //��ȡϵͳά�����е��������ݿ���Ϣ�ҽӵ�������
            //cyf 20110602 modify:�޸�ϵͳά����������Ϣ
            Exception ex = null;
            if (File.Exists(ModuleData.v_ConfigPath))
            {
                SysCommon.Gis.SysGisDB vgisDb = new SysCommon.Gis.SysGisDB();
                //���ϵͳά����������Ϣ
                SysCommon.Authorize.AuthorizeClass.GetConnectInfo(ModuleData.v_ConfigPath, out ModuleData.Server, out ModuleData.Instance, out ModuleData.Database, out ModuleData.User, out ModuleData.Password, out ModuleData.Version, out ModuleData.dbType);
                //����ϵͳά��������
                bool blnCanConnect = CanOpenConnect(vgisDb, ModuleData.dbType, ModuleData.Server, ModuleData.Instance, ModuleData.Database, ModuleData.User, ModuleData.Password, ModuleData.Version);
                if (!blnCanConnect)
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "ϵͳά��������ʧ�ܣ����������ã�");
                    return;
                }
                ModuleData.TempWks = vgisDb.WorkSpace;

                clsAddAppDBConnection addAppDB = new clsAddAppDBConnection();
                //�ж�ϵͳά�����������Ϣ�Ƿ���ȷ
                addAppDB.JudgeAppDbConfiguration(out ex);
                if (ex != null)
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�����ʼ����ʧ�ܣ�\nԭ��:" + ex.Message);
                    return;
                }
                #region cyf 20110627 add:��ʼ��������ͼ
                IFeatureWorkspace pFeaWS = ModuleData.TempWks as IFeatureWorkspace;
                if (pFeaWS == null)
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "����ϵͳά����ʧ�ܣ�");
                    return;
                }
                string pTableName = "DATABASETYPEMD";
                string pFieldNames = "DATABASETYPE";
                ICursor pCursor = ModDBOperate.GetCursor(pFeaWS, pTableName, pFieldNames, "", out ex);
                if (ex != null || pCursor == null)
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ѯ���ݿ�ϵͳά�����е����ݿ����ͱ�ʧ�ܣ�");
                    return;
                }
                IRow pRow = pCursor.NextRow();
                //�����У������ڵ��������ͼ��
                while (pRow != null)
                {
                    string pDBType = pRow.get_Value(0).ToString();  //���ݿ�����
                    DevComponents.AdvTree.Node pNode = new DevComponents.AdvTree.Node();
                    pNode.Expanded = true;
                    pNode.Name = "node2";
                    pNode.TagString = "Database";
                    pNode.Text = pDBType;
                    pNode.Image = IconContainer.Images[1];  //cyf 20110711 ���ͼ��
                    this.node1.Nodes.Add(pNode);
                    pRow = pCursor.NextRow();
                }
                //�ͷ��α�
                System.Runtime.InteropServices.Marshal.ReleaseComObject(pCursor);   //cyf 20110713 add
                this.advTreeProject.Refresh();
                #endregion

                //ˢ�½���
                while (!addAppDB.refurbish(out ex))
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�����ʼ����ʧ�ܣ�\nԭ��:" + ex.Message);
                    return; 
                }
            }
            else
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "ϵͳά�������������ļ������ڣ����飡");
                return;
            }
            #region ԭ�д���
            //*******************************************************//
            //guozheng 2010-09-28 ��ȡϵͳά����������Ϣ��
            //��ȡϵͳά�����е��������ݿ���Ϣ�ҽӵ�������
            //Exception ex = null;
            //clsAddAppDBConnection addAppDB = new clsAddAppDBConnection();
            //string sConnect = addAppDB.GetAppDBConInfo(out ex);
            //if (string.IsNullOrEmpty(sConnect))
            //{
            //    sConnect = addAppDB.SetAppDBConInfo(out ex);
            //}
            //if (!string.IsNullOrEmpty(sConnect))
            //{
            //    //�ж�ϵͳά�����������Ϣ�Ƿ���ȷ
            //addAppDB.JudgeAppDbConfiguration(sConnect, out ex);
            //    if (ex != null)
            //    {
            //        if (SysCommon.Error.ErrorHandle.ShowFrmInformation("��", "��", "ϵͳά�������ṹ����" + ex.Message + ",\n�Ƿ���������ϵͳά����������Ϣ��"))
            //        {
            //            sConnect = addAppDB.SetAppDBConInfo(out ex);
            //        }
            //        else
            //            return;
            //    }

            //    while (!addAppDB.refurbish(sConnect, out ex))
            //    {

            //        if (SysCommon.Error.ErrorHandle.ShowFrmInformation("��", "��", "�����ʼ����ʧ�ܣ�\nԭ��:" + ex.Message + ",\n�Ƿ���������ϵͳά����������Ϣ��"))
            //        {
            //            sConnect = addAppDB.SetAppDBConInfo(out ex);
            //            /////�������ַ�����¼����
            //            ModuleData.v_AppConnStr = sConnect;
            //        }
            //        else
            //        {
            //            break;
            //        }
            //    }
            //    /////�������ַ�����¼����
            //    ModuleData.v_AppConnStr = sConnect;
            //}

            //******************************************************//
            #endregion
        }
        //yjl20120808 add ���ų�ʼ��������ͼ�����������ݿ����ͺ� ��Ҫ��ʼ��������ͼ
        public void InitProjectTree()
        {
            Exception ex;
            clsAddAppDBConnection addAppDB = new clsAddAppDBConnection();
            //�ж�ϵͳά�����������Ϣ�Ƿ���ȷ
            addAppDB.JudgeAppDbConfiguration(out ex);
            if (ex != null)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�����ʼ����ʧ�ܣ�\nԭ��:" + ex.Message);
                return;
            }
            #region cyf 20110627 add:��ʼ��������ͼ
            
            IFeatureWorkspace pFeaWS = ModuleData.TempWks as IFeatureWorkspace;
            if (pFeaWS == null)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "����ϵͳά����ʧ�ܣ�");
                return;
            }
            string pTableName = "DATABASETYPEMD";
            string pFieldNames = "DATABASETYPE";
            ICursor pCursor = ModDBOperate.GetCursor(pFeaWS, pTableName, pFieldNames, "", out ex);
            if (ex != null || pCursor == null)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ѯ���ݿ�ϵͳά�����е����ݿ����ͱ�ʧ�ܣ�");
                return;
            }
            this.node1.Nodes.Clear();
            IRow pRow = pCursor.NextRow();
            //�����У������ڵ��������ͼ��
            while (pRow != null)
            {
                string pDBType = pRow.get_Value(0).ToString();  //���ݿ�����
                DevComponents.AdvTree.Node pNode = new DevComponents.AdvTree.Node();
                pNode.Expanded = true;
                pNode.Name = "node2";
                pNode.TagString = "Database";
                pNode.Text = pDBType;
                pNode.Image = IconContainer.Images[1];  //cyf 20110711 ���ͼ��
                this.node1.Nodes.Add(pNode);
                pRow = pCursor.NextRow();
            }
            //�ͷ��α�
            System.Runtime.InteropServices.Marshal.ReleaseComObject(pCursor);   //cyf 20110713 add
            this.advTreeProject.Refresh();
            #endregion
            //ˢ�½���
            while (!addAppDB.refurbish(out ex))
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "�����ʼ����ʧ�ܣ�\nԭ��:" + ex.Message);
                return;
            }

        }
        //����������Ϣ�Ƿ����
        public static bool CanOpenConnect(SysCommon.Gis.SysGisDB vgisDb, string strType, string strServer, string strService, string strDatabase, string strUser, string strPassword, string strVersion)
        {
            bool blnOpen = false;

            Exception Err;

            if (strType.ToUpper() == "ORACLE" || strType.ToUpper() == "SQLSERVER")
            {
                blnOpen = vgisDb.SetWorkspace(strServer, strService, strDatabase, strUser, strPassword, strVersion, out Err);
            }
            else if (strType.ToUpper() == "ACCESS")
            {
                blnOpen = vgisDb.SetWorkspace(strServer, SysCommon.enumWSType.PDB, out Err);
            }
            else if (strType.ToUpper() == "FILE")
            {
                blnOpen = vgisDb.SetWorkspace(strServer, SysCommon.enumWSType.GDB, out Err);
            }

            return blnOpen;

        }
        //��ʼ�����ö�Ӧ��ͼ�ؼ�
        private void InitialMainViewControl()
        {
            frmBarManager newfrmBarManager = new frmBarManager();
            newfrmBarManager.TopLevel = false;
            newfrmBarManager.Dock = DockStyle.Fill;
            newfrmBarManager.Show();
            this.Controls.Add(newfrmBarManager);

            //�������ÿ�����ͼ
            DevComponents.DotNetBar.Bar barTree = newfrmBarManager.CreateBar("barTree", enumLayType.FILL);
            barTree.CanHide = false;
            barTree.CanAutoHide = true;
            DevComponents.DotNetBar.PanelDockContainer PanelTree = newfrmBarManager.CreatePanelDockContainer("PanelTree", barTree);
            DockContainerItem TreeContainerItem = newfrmBarManager.CreateDockContainerItem("TreeContainerItem", "���ݹ���", PanelTree, barTree);
            PanelTree.Controls.Add(this.advTreeProject);//.tabTreeControl
            this.advTreeProject.Dock = DockStyle.Fill;

            //����ͼ����ʾ  cyf 20110711 modify 
            //DevComponents.DotNetBar.PanelDockContainer PanelTree1 = newfrmBarManager.CreatePanelDockContainer("PanelTree1", barTree);
            //DockContainerItem TreeContainerItem1 = newfrmBarManager.CreateDockContainerItem("TreeContainerItem1", "ͼ����ʾ", PanelTree1, barTree);
            //PanelTree1.Controls.Add(this.axTOCControl1);//.tabTreeControl
            //this.axTOCControl1.Dock = DockStyle.Fill;
            //end

            //�����������ݿ�ռ����
            DevComponents.DotNetBar.Bar barMap = newfrmBarManager.CreateBar("barMap", enumLayType.FILL);
            barMap.CanHide = false;
            DevComponents.DotNetBar.PanelDockContainer PanelMap = newfrmBarManager.CreatePanelDockContainer("PanelMap", barMap);
            DockContainerItem MapContainerItem = newfrmBarManager.CreateDockContainerItem("MapContainerItem", "���ݿ����", PanelMap, barMap);
            PanelMap.Controls.Add(this.groupPanel1);//.tabMapControl
            this.groupPanel1.Dock = DockStyle.Fill;
            _DBControl = PanelMap as Control;
            //����������ͼ  cyf 20110711 modify
            //DevComponents.DotNetBar.PanelDockContainer PanelMap1 = newfrmBarManager.CreatePanelDockContainer("PanelMap1", barMap);
            //DockContainerItem MapContainerItem1 = newfrmBarManager.CreateDockContainerItem("MapContainerItem", "ͼ�����", PanelMap1, barMap);
            //PanelMap1.Controls.Add(this.axMapControl1);//.tabMapControl
            //this.axMapControl1.Dock = DockStyle.Fill;
            //_MapToolControl = PanelMap1 as Control;
            //end

            //��������
            newfrmBarManager.MainDotNetBarManager.FillDockSite.GetDocumentUIManager().Dock(barTree, barMap, eDockSide.Right);
            newfrmBarManager.MainDotNetBarManager.FillDockSite.GetDocumentUIManager().SetBarWidth(barTree, this.Width / 5);

            //cyf 20110610 modify:���ε���صĴ���ؼ�
            //����������ʾ����
            //������Ϣ
            PanelDockContainer PanelTipConn = new PanelDockContainer();
            PanelTipConn.Controls.Add(this.dgConnecInfo);
            this.dgConnecInfo.Dock = DockStyle.Fill;
            DockContainerItem dockItemConn = new DockContainerItem("dockItemConn", "���ݿ�����״̬");
            dockItemConn.Control = PanelTipConn;
            //dockItemConn.Visible = false;
            newfrmBarManager.ButtomBar.Items.Add(dockItemConn);

            //������Ϣ��ʾ
            PanelDockContainer PanelTipPara = new PanelDockContainer();
            PanelTipPara.Controls.Add(this.dgParaInfo);
            this.dgParaInfo.Dock = DockStyle.Fill;
            DockContainerItem dockItemPara = new DockContainerItem("dockItemPara", "������Ϣ");
            dockItemPara.Control = PanelTipPara;
            newfrmBarManager.ButtomBar.Items.Add(dockItemPara);

            ////��ѯ�����ʾ
            //PanelDockContainer PanelTipQuery = new PanelDockContainer();
            //PanelTipQuery.Controls.Add(this.dgQueryRes);
            //this.dgQueryRes.Dock = DockStyle.Fill;
            //DockContainerItem dockItemQuery = new DockContainerItem("dockItemQuery", "��ѯ���");
            //dockItemQuery.Control = PanelTipQuery;
            //newfrmBarManager.ButtomBar.Items.Add(dockItemQuery);
            //end
        }

        //��ʼ����ܲ���ؼ�����
        private void InitialFrmDefControl()
        {
            //�õ�Xml��System�ڵ�,����XML���ز������
            string xPath = ".//System[@Name='" + this.Name + "']";
            Plugin.ModuleCommon.LoadButtonViewByXmlNode(ModuleData.v_AppDBIntegra.ControlContainer, xPath, ModuleData.v_AppDBIntegra);

            _dicContextMenu = ModuleData.v_AppDBIntegra.DicContextMenu;

            ////��ʼ����ͼ���������
            Plugin.Application.IAppFormRef pAppFrm = ModuleData.v_AppDBIntegra as Plugin.Application.IAppFormRef;
            XmlNode barXmlNode = pAppFrm.SystemXml.SelectSingleNode(".//ToolBar[@Name='ControlMapToolBar4']");
            if (barXmlNode == null || _MapToolControl == null) return;
            DevComponents.DotNetBar.Bar mapToolBar = Plugin.ModuleCommon.LoadButtonView(_MapToolControl, barXmlNode, pAppFrm, null) as Bar;
            if (mapToolBar != null)
            {
                mapToolBar.AccessibleRole = System.Windows.Forms.AccessibleRole.ToolBar;
                mapToolBar.DockOrientation = DevComponents.DotNetBar.eOrientation.Horizontal;//.Vertical;
                mapToolBar.DockSide = DevComponents.DotNetBar.eDockSide.Top;//.Left;
                mapToolBar.GrabHandleStyle = DevComponents.DotNetBar.eGrabHandleStyle.Office2003;
                mapToolBar.Style = DevComponents.DotNetBar.eDotNetBarStyle.Office2003;
                _MapToolControl.Controls.Remove(mapToolBar);
                _MapToolControl.Controls.Add(mapToolBar);
            }

            foreach (KeyValuePair<string, DevComponents.DotNetBar.ContextMenuBar> keyValue in _dicContextMenu)
            {
                this.Controls.Add(keyValue.Value);
            }
        }


        //ͼ������Ҽ��˵�
        private void axTOCControl1_OnMouseDown(object sender, ITOCControlEvents_OnMouseDownEvent e)
        {
            IBasicMap pMap = null;
            ILayer pLayer = null;
            System.Object other = null;
            System.Object LayerIndex = null;
            System.Drawing.Point pPoint = new System.Drawing.Point(e.x, e.y);

            esriTOCControlItem TOCItem = esriTOCControlItem.esriTOCControlItemNone;
            ITOCControl2 tocControl = (ITOCControl2)axTOCControl1.Object;

            tocControl.HitTest(e.x, e.y, ref TOCItem, ref pMap, ref pLayer, ref other, ref LayerIndex);

            if (e.button == 1)
            {
                //���ƽ��GroupLayer��ʾ����  
                if (TOCItem == esriTOCControlItem.esriTOCControlItemLayer && pLayer is IGroupLayer)
                {
                    if (pLayer.Visible)
                    {
                        m_bGroupLayerVisible = false;
                    }
                    else
                    {
                        m_bGroupLayerVisible = true;
                    }
                }
                return;
            }

            if (e.button == 2 && _dicContextMenu != null)
            {
                DevComponents.DotNetBar.ButtonItem item = null;
                switch (TOCItem)
                {
                    case esriTOCControlItem.esriTOCControlItemMap:
                        if (_dicContextMenu.ContainsKey("TOCControlContextMenu4"))
                        {
                            if (_dicContextMenu["TOCControlContextMenu4"] != null)
                            {
                                if (_dicContextMenu["TOCControlContextMenu4"].Items.Count > 0)
                                {
                                    item = _dicContextMenu["TOCControlContextMenu4"].Items[0] as DevComponents.DotNetBar.ButtonItem;
                                    if (item != null)
                                    {
                                        item.Popup(axTOCControl1.PointToScreen(pPoint));
                                    }
                                }
                            }
                        }
                        break;
                    case esriTOCControlItem.esriTOCControlItemLayer:
                        {
                            if (!(pLayer is IGroupLayer || pLayer is IFeatureLayer || pLayer is IDataLayer || pLayer is IDynamicLayer)) return;
                            if (_dicContextMenu.ContainsKey("TOCControlLayerContextMenu4"))
                            {
                                if (_dicContextMenu["TOCControlLayerContextMenu4"] != null)
                                {
                                    if (_dicContextMenu["TOCControlLayerContextMenu4"].Items.Count > 0)
                                    {
                                        item = _dicContextMenu["TOCControlLayerContextMenu4"].Items[0] as DevComponents.DotNetBar.ButtonItem;
                                        if (item != null)
                                        {
                                            item.Popup(axTOCControl1.PointToScreen(pPoint));
                                        }
                                    }
                                }
                            }

                            if (axTOCControl1.Buddy is IPageLayoutControl2)
                            {
                                IPageLayoutControl2 pPageLayoutControl = axTOCControl1.Buddy as IPageLayoutControl2;
                                pPageLayoutControl.CustomProperty = pLayer;

                            }

                            else if (axTOCControl1.Buddy is IMapControl3)
                            {
                                IMapControl3 pMapcontrol = axTOCControl1.Buddy as IMapControl3;
                                pMapcontrol.CustomProperty = pLayer;
                            }

                        }

                        break;
                }
            }
        }

        //ͼ������Ҽ��˵�
        private void axMapControl1_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e)
        {
            //timerShow.Enabled = false;  //���в���ʱ��������ʷ��ѯ������

            if (e.button == 1 || _dicContextMenu == null)
                return;
            System.Drawing.Point pPoint = new System.Drawing.Point(e.x, e.y);
            DevComponents.DotNetBar.ButtonItem item = null;
            if (_dicContextMenu.ContainsKey("MapControlContextMenu4"))
            {
                if (_dicContextMenu["MapControlContextMenu4"].Items.Count > 0)
                {
                    item = _dicContextMenu["MapControlContextMenu4"].Items[0] as DevComponents.DotNetBar.ButtonItem;
                    if (item != null)
                    {
                        item.Popup(axMapControl1.PointToScreen(pPoint));
                    }
                }
            }
        }

        //���ô��¼���Ͼֲ�����m_bGroupLayerVisible���ƽ��GroupLayer��ʾ����
        private void axTOCControl1_OnMouseUp(object sender, ITOCControlEvents_OnMouseUpEvent e)
        {
            IBasicMap pMap = null;
            ILayer pLayer = null;
            System.Object other = null;
            System.Object LayerIndex = null;
            System.Drawing.Point pPoint = new System.Drawing.Point(e.x, e.y);

            esriTOCControlItem TOCItem = esriTOCControlItem.esriTOCControlItemNone;
            ITOCControl2 tocControl = (ITOCControl2)axTOCControl1.Object;

            tocControl.HitTest(e.x, e.y, ref TOCItem, ref pMap, ref pLayer, ref other, ref LayerIndex);
            if (e.button == 1)
            {
                if (TOCItem == esriTOCControlItem.esriTOCControlItemLayer && pLayer is IGroupLayer)
                {
                    if (m_bGroupLayerVisible != pLayer.Visible)
                    {
                        IMapControlDefault pMapcontrol = axTOCControl1.Buddy as IMapControlDefault;
                        pMapcontrol.ActiveView.Refresh();
                    }
                }
            }
        }

        private void axMapControl1_OnAfterDraw(object sender, IMapControlEvents2_OnAfterDrawEvent e)
        {
            try
            {
                ModuleData.v_AppDBIntegra.RefScaleCmb.ControlText = (sender as AxMapControl).Map.ReferenceScale.ToString().Trim();
                ModuleData.v_AppDBIntegra.CurScaleCmb.ControlText = (sender as AxMapControl).Map.MapScale.ToString().Trim();
                ModuleData.v_AppDBIntegra.CurScaleCmb.Tooltip = axMapControl1.Map.MapScale.ToString().Trim();
            }
            catch
            {
            }
        }

        private void axMapControl_OnMouseMove(object sender, IMapControlEvents2_OnMouseMoveEvent e)
        {
            //ModData.v_AppSMPD.CoorTxt.ControlText = "X:" + e.mapX + ";Y:" + e.mapY;
            ModuleData.v_AppDBIntegra.OperatorTips = "X:" + e.mapX + ";Y:" + e.mapY;
        }

        /// <summary>
        /// �����ݿ�IDд��xml��  ���Ƿ����20100930
        /// </summary>
        /// <param name="pDBID">���ݿ�ID</param>
        private void SaveIDToXml(string pDBID, string xmlCur, string xmlTemp)
        {
            try
            {
                Convert.ToInt32(pDBID);
                if (!File.Exists(xmlCur))
                {
                    File.Copy(xmlTemp, xmlCur);
                }
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlCur);
                XmlElement pElem = xmlDoc.SelectSingleNode(".//���̹���") as XmlElement;
                pElem.SetAttribute("��ǰ������", pDBID);
                xmlDoc.Save(xmlCur);
            }
            catch (Exception eError)
            {
                //****************************************************
                if (ModuleData.v_SysLog != null)
                    ModuleData.v_SysLog.Write(eError, null, DateTime.Now);
                //****************************************************
                //SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "���ȴ������ݿ����ӹ���");
                return;
            }

        }

        private void cmbFileDB_SelectedIndexChanged(object sender, EventArgs e)
        {
            //�ļ������
            //����ǰ���ݿ�IDд��xml��
            if (cmbFileDB.SelectedValue != null)
            {
                Exception ex = null;
                string pDBID = cmbFileDB.SelectedValue.ToString();
                string sDBName = cmbFileDB.Text.Trim();
                if (pDBID == "System.Data.DataRowView" || sDBName == "System.Data.DataRowView") return;
                clsFTPOper FTPOper = new clsFTPOper();
                FTPOper.SaveProjectXML(pDBID, sDBName, out ex);
                if (ex != null)
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", ex.Message);
                    return;
                }
            }

        }

        private void cmbFeatureDB_SelectedIndexChanged(object sender, EventArgs e)
        {
            //���Ҫ�ؽ���
            //����ǰ���ݿ�IDд��xml��
            if (cmbFeatureDB.SelectedValue != null)
            {
                string pDBID = cmbFeatureDB.SelectedValue.ToString();//��ǰҪ�����Ĺ���ID
                //����ǰ���ݿ�IDд��xml��
                SaveIDToXml(pDBID, ModuleData.v_feaProjectXML, ModuleData.v_feaProjectXMLTemp);

            }
        }

        private void cmbImageDB_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Ӱ�����ݿ����
            //����ǰ���ݿ�IDд��xml��
            if (cmbImageDB.SelectedValue != null)
            {
                string pDBID = cmbImageDB.SelectedValue.ToString();
                SaveIDToXml(pDBID, ModuleData.v_ImageProjectXml, ModuleData.v_ImageProjectXmlTemp);
            }
        }

        private void cmbDemDB_SelectedIndexChanged(object sender, EventArgs e)
        {
            //�߳����ݿ����
            //����ǰ���ݿ�IDд��xml��
            if (cmbDemDB.SelectedValue != null)
            {
                string pDBID = cmbDemDB.SelectedValue.ToString();
                SaveIDToXml(pDBID, ModuleData.v_DEMProjectXml, ModuleData.v_DEMProjectXmlTemp);
            }
        }

        private void cmbSubjectDB_SelectedIndexChanged(object sender, EventArgs e)
        {
            //ר�����ݿ����
            //����ǰ���ݿ�IDд��xml��
            if (cmbSubjectDB.SelectedValue != null)
            {
                string pDBID = cmbSubjectDB.SelectedValue.ToString();
                //SaveIDToXml(pDBID);
            }
        }

        private void cmbAdressDB_SelectedIndexChanged(object sender, EventArgs e)
        {
            //�������ݿ����
            //����ǰ���ݿ�IDд��xml��
            if (cmbAdressDB.SelectedValue != null)
            {
                string pDBID = cmbAdressDB.SelectedValue.ToString();
                //SaveIDToXml(pDBID);
            }
        }

        private void cmbEntiDB_SelectedIndexChanged(object sender, EventArgs e)
        {
            //����ʵ�����ݿ����
            //����ǰ���ݿ�IDд��xml��
            if (cmbEntiDB.SelectedValue != null)
            {
                string pDBID = cmbEntiDB.SelectedValue.ToString();
                //SaveIDToXml(pDBID);
            }
        }

        private void cmbMapDB_SelectedIndexChanged(object sender, EventArgs e)
        {
            //���ӵ�ͼ���ݿ����
            //����ǰ���ݿ�IDд��xml��
            if (cmbMapDB.SelectedValue != null)
            {
                string pDBID = cmbMapDB.SelectedValue.ToString();
                //SaveIDToXml(pDBID);
            }
        }

        private void btnFeaDB_Click(object sender, EventArgs e)
        {

            if (cmbFeatureDB.SelectedValue == null)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "����������ݿ����ӹ��̣����������ݿ⣡");
                return;
            }

            string pDBID = "";  //��ǰҪ�����Ĺ���ID
            pDBID = cmbFeatureDB.SelectedValue.ToString();

            //����ǰ���ݿ�IDд��xml��
            SaveIDToXml(pDBID, ModuleData.v_feaProjectXML, ModuleData.v_feaProjectXMLTemp);

            for (int i = 0; i < advTreeProject.SelectedNode.Nodes.Count; i++)
            {
                if (advTreeProject.SelectedNode.Nodes[i].DataKey.ToString() != pDBID) continue;

                //==============================================================================================================================================
                //chenyafei  modify 20100215 ���ϵͳ������ص�����
                //
                //
                string pSysName = "";   //��ϵͳ����
                string pSysCaption = ""; //��ϵͳ����

                XmlElement pElem = advTreeProject.SelectedNode.Nodes[i].Tag as XmlElement;  //���ݿ�ƽ̨�ڵ�
                string ptStr = pElem.GetAttribute("���ݿ�ƽ̨");   //���ݿ�ƽ̨��Ϣ
                if (ptStr == enumInterDBFormat.ARCGISGDB.ToString() || ptStr == enumInterDBFormat.ARCGISPDB.ToString() || ptStr == enumInterDBFormat.ARCGISSDE.ToString())
                {
                    //����ArcGIsƽ̨
                    pSysName = "GeoDBATool.ControlDBATool";    //Name
                }
                else if (ptStr == enumInterDBFormat.GEOSTARACCESS.ToString() || ptStr == enumInterDBFormat.GEOSTARORACLE.ToString() || ptStr == enumInterDBFormat.GEOSTARORSQLSERVER.ToString())
                {
                    //����Geostarƽ̨
                    pSysName = "GeoStarTest.ControlTest";       //Name
                }
                else if (ptStr == enumInterDBFormat.ORACLESPATIAL.ToString())
                {
                    //����oraclespatialƽ̨
                    pSysName = "OracleSpatialDBTool.ControlOracleSpatialDBTool";    //Name
                }

                //����Name�����ϵͳ��caption
                XmlDocument sysXml = new XmlDocument();
                sysXml.Load(ModuleData.m_SysXmlPath);
                XmlNode sysNode = sysXml.SelectSingleNode("//Main//System[@Name='" + pSysName + "']");
                if (sysNode == null)
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "������NameΪ" + pSysName + "��ϵͳ");
                    return;
                }
                pSysCaption = (sysNode as XmlElement).GetAttribute("Caption").Trim();  //caption

                //������ϵͳ����
                ModDBOperate.InitialForm(pSysName, pSysCaption);

                //===================================================================================================================================================


                //*********************************************************************
                //guozheng added enter feature Db Log
                if (ModuleData.v_SysLog != null)
                {
                    List<string> Pra = new List<string>();
                    Pra.Add(pElem.GetAttribute("���ݿ⹤����"));
                    Pra.Add(pElem.GetAttribute("���ݿ�ƽ̨"));
                    Pra.Add(pElem.GetAttribute("���ݿ�����"));
                    Pra.Add(pElem.GetAttribute("���ݿ�������Ϣ"));
                    ModuleData.v_SysLog.Write("������Ҫ�ؿ�", Pra, DateTime.Now);
                }
                //*********************************************************************
                break;
            }
        }

        private void btnFileDB_Click(object sender, EventArgs e)
        {
            //����ǰ���ݿ�IDд��xml��
            if (cmbFileDB.SelectedValue != null)
            {
                Exception ex = null;
                string pDBID = cmbFileDB.SelectedValue.ToString();
                string sDBName = cmbFileDB.Text.Trim();
                if (pDBID == "System.Data.DataRowView" || sDBName == "System.Data.DataRowView") return;
                clsFTPOper FTPOper = new clsFTPOper();
                FTPOper.SaveProjectXML(pDBID, sDBName, out ex);
                if (ex != null)
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", ex.Message);
                    return;
                }
            }

            //�����ļ������
            //==================================================================================
            //  chenayfei  modify 20110215  ������ϵͳ�����޸�
            //
            //
            string pSysName = "";   //��ϵͳ����
            string pSysCaption = ""; //��ϵͳ����
            //����Name�����ϵͳ��caption

            pSysName = "FileDBTool.ControlFileDBTool";    //Name

            XmlDocument sysXml = new XmlDocument();
            sysXml.Load(ModuleData.m_SysXmlPath);
            XmlNode sysNode = sysXml.SelectSingleNode("//Main//System[@Name='" + pSysName + "']");
            if (sysNode == null)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "������NameΪ" + pSysName + "��ϵͳ");
                return;
            }
            pSysCaption = (sysNode as XmlElement).GetAttribute("Caption").Trim();  //caption

            //������ϵͳ����
            ModDBOperate.InitialForm(pSysName, pSysCaption);

            //========================================================================

        }

        private void btnSubjectDB_Click(object sender, EventArgs e)
        {
            //����ǰ���ݿ�IDд��xml��
            if (cmbSubjectDB.SelectedValue != null)
            {
                string pDBID = cmbSubjectDB.SelectedValue.ToString();
                //SaveIDToXml(pDBID);
            }

            //����ר�����ݿ����

            //Form pForm = (ModuleData.v_AppDBIntegra as Plugin.Application.IAppFormRef).MainForm;
            //pForm.Controls.Clear();
            //ModDBOperate.IntialSysFrm(ModuleData.m_SubjectSysXmlPath, pForm);
        }

        private void bnAddressDB_Click(object sender, EventArgs e)
        {
            //����ǰ���ݿ�IDд��xml��
            if (cmbAdressDB.SelectedValue != null)
            {
                string pDBID = cmbAdressDB.SelectedValue.ToString();
                //SaveIDToXml(pDBID);
            }

            //�������ݿ����
            //==================================================================================
            //  chenayfei  modify 20110215  ������ϵͳ�����޸�
            //
            //
            string pSysName = "";   //��ϵͳ����
            string pSysCaption = ""; //��ϵͳ����
            //����Name�����ϵͳ��caption

            pSysName = "GeoDBAddress.ControlDBAddressTool";    //Name

            XmlDocument sysXml = new XmlDocument();
            sysXml.Load(ModuleData.m_SysXmlPath);
            XmlNode sysNode = sysXml.SelectSingleNode("//Main//System[@Name='" + pSysName + "']");
            if (sysNode == null)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "������NameΪ" + pSysName + "��ϵͳ");
                return;
            }
            pSysCaption = (sysNode as XmlElement).GetAttribute("Caption").Trim();  //caption

            //������ϵͳ����
            //ModDBOperate.InitialForm(pSysName, pSysCaption);

            //========================================================================
        }

        private void btnImageDB_Click(object sender, EventArgs e)
        {
            //����ǰ���ݿ�IDд��xml��
            if (cmbImageDB.SelectedValue != null)
            {
                string pDBID = cmbImageDB.SelectedValue.ToString();
                SaveIDToXml(pDBID, ModuleData.v_ImageProjectXml, ModuleData.v_ImageProjectXmlTemp);
            }

            //Ӱ�����ݿ����
            //==================================================================================
            //  chenayfei  modify 20110215  ������ϵͳ�����޸�
            //
            //
            string pSysName = "";   //��ϵͳ����
            string pSysCaption = ""; //��ϵͳ����
            //����Name�����ϵͳ��caption

            pSysName = "GeoDBATool.ControlDBATool";    //Name

            XmlDocument sysXml = new XmlDocument();
            sysXml.Load(ModuleData.m_SysXmlPath);
            XmlNode sysNode = sysXml.SelectSingleNode("//Main//System[@Name='" + pSysName + "']");
            if (sysNode == null)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "������NameΪ" + pSysName + "��ϵͳ");
                return;
            }
            pSysCaption = (sysNode as XmlElement).GetAttribute("Caption").Trim();  //caption

            //������ϵͳ����
            ModDBOperate.InitialForm(pSysName, pSysCaption);
            //==========================================================================================
        }

        private void btnEntiDB_Click(object sender, EventArgs e)
        {
            //����ǰ���ݿ�IDд��xml��
            if (cmbEntiDB.SelectedValue != null)
            {
                string pDBID = cmbEntiDB.SelectedValue.ToString();
                //SaveIDToXml(pDBID);
            }

            //�������ʵ�����ݿ����
            //==================================================================================
            //  chenayfei  modify 20110215  ������ϵͳ�����޸�
            //
            //
            string pSysName = "";   //��ϵͳ����
            string pSysCaption = ""; //��ϵͳ����
            //����Name�����ϵͳ��caption

            pSysName = "GeoDBEntity.ControlDBEntityTool";    //Name

            XmlDocument sysXml = new XmlDocument();
            sysXml.Load(ModuleData.m_SysXmlPath);
            XmlNode sysNode = sysXml.SelectSingleNode("//Main//System[@Name='" + pSysName + "']");
            if (sysNode == null)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "������NameΪ" + pSysName + "��ϵͳ");
                return;
            }
            pSysCaption = (sysNode as XmlElement).GetAttribute("Caption").Trim();  //caption

            //������ϵͳ����
            //ModDBOperate.InitialForm(pSysName, pSysCaption);

            //========================================================================
        }

        private void btnDemDB_Click(object sender, EventArgs e)
        {
            //����ǰ���ݿ�IDд��xml��
            if (cmbDemDB.SelectedValue != null)
            {
                string pDBID = cmbDemDB.SelectedValue.ToString();
                SaveIDToXml(pDBID, ModuleData.v_DEMProjectXml, ModuleData.v_DEMProjectXmlTemp);
            }

            //�߳����ݿ����
            //==================================================================================
            //  chenayfei  modify 20110215  ������ϵͳ�����޸�
            //
            //
            string pSysName = "";   //��ϵͳ����
            string pSysCaption = ""; //��ϵͳ����
            //����Name�����ϵͳ��caption

            pSysName = "GeoDBATool.ControlDBATool";    //Name

            XmlDocument sysXml = new XmlDocument();
            sysXml.Load(ModuleData.m_SysXmlPath);
            XmlNode sysNode = sysXml.SelectSingleNode("//Main//System[@Name='" + pSysName + "']");
            if (sysNode == null)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "������NameΪ" + pSysName + "��ϵͳ");
                return;
            }
            pSysCaption = (sysNode as XmlElement).GetAttribute("Caption").Trim();  //caption

            //������ϵͳ����
            ModDBOperate.InitialForm(pSysName, pSysCaption);
            //==========================================================================================

        }

        private void btnMapDB_Click(object sender, EventArgs e)
        {
            //������ӵ�ͼ���ݿ����
            //Form pForm = (ModuleData.v_AppDBIntegra as Plugin.Application.IAppFormRef).MainForm;
            //pForm.Controls.Clear();
            //ModDBOperate.IntialSysFrm(ModuleData.m_MapSysXmlPath, pForm);
        }

        private void advTreeProject_NodeClick(object sender, DevComponents.AdvTree.TreeNodeMouseEventArgs e)
        {
            DevComponents.AdvTree.AdvTree aTree = sender as DevComponents.AdvTree.AdvTree;
            if (aTree.SelectedNode == null) return;
            if (aTree.SelectedNode.Tag == null) return;
            try
            {
                XmlElement DbInfoEle = aTree.SelectedNode.Tag as XmlElement;
                //string sDataBaseType = DbInfoEle.GetAttribute("���ݿ�����"); if (string.IsNullOrEmpty(sDataBaseType)) return;
                
                /********���Ӳ�����Ϣ xisheng 20111103***************************/
                dgParaInfo.DataSource = null;
                if (DbInfoEle != null)
                {
                    DataTable dt = new DataTable();
                    for (int i = 0; i < DbInfoEle.Attributes.Count; i++)
                    {
                        if (DbInfoEle.Attributes[i].Name.EndsWith("ID")) continue;
                        if (DbInfoEle.Attributes[i].Name == "���ݿ�������Ϣ") continue;
                        if (DbInfoEle.Attributes[i].Name == "���ݿ����") continue;
                        dt.Columns.Add(DbInfoEle.Attributes[i].Name);
                    }
                    DataRow row = dt.NewRow();
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {

                        row[i] = DbInfoEle.GetAttribute(dt.Columns[i].ColumnName);
                    }

                    string[] arrConect = DbInfoEle.GetAttribute("���ݿ�������Ϣ").Split('|');
                    if (arrConect[0] == "")
                    {
                        dt.Columns.Add("���ݿ�");
                        row["���ݿ�"] = arrConect[2];
                        dt.Columns.Add("���ݼ�");
                        row["���ݼ�"] = arrConect[6];
                    }
                    else
                    {
                        dt.Columns.Add("������");
                        row["������"] = arrConect[0];
                        dt.Columns.Add("����˿�");
                        row["����˿�"] = arrConect[1];
                        dt.Columns.Add("���ݿ�");
                        row["���ݿ�"] = arrConect[2];
                        dt.Columns.Add("�û���");
                        row["�û���"] = arrConect[3];
                        //*******************������Ϣ����ʾ���� xisheng 20111117***********//
                        //dt.Columns.Add("����");
                        //row["����"] = arrConect[4];
                        //end*******************������Ϣ����ʾ���� xisheng 20111117***********//
                        dt.Columns.Add("�汾");
                        row["�汾"] = arrConect[5];
                        dt.Columns.Add("���ݼ�");
                        row["���ݼ�"] = arrConect[6];
                    }

                    dt.Rows.Add(row);
                    dgParaInfo.DataSource = dt;
                    dgParaInfo.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                    dgParaInfo.AllowUserToAddRows = false;
                    dgParaInfo.ReadOnly = true;
                    /******** end ���Ӳ�����Ϣ xisheng 20111103***************************/
                }
                //long lDBID = -1; lDBID = (long)aTree.SelectedNode.DataKey; if (lDBID < 0) return;
                ModuleData.v_DataBaseProPanel.SelectButton(aTree.SelectedNode);
            }
            catch
            {
            }
            //if (aTree.SelectedNode.Tag.ToString() == "Database")
            //{
            //    if (aTree.SelectedNode.Text == enumInterDBType.���Ҫ�����ݿ�.ToString())
            //    {
            //        btnFeaDB.Enabled = true;
            //        btnDemDB.Enabled = false;
            //        btnEntiDB.Enabled = false;
            //        btnFileDB.Enabled = false;
            //        btnImageDB.Enabled = false;
            //        btnMapDB.Enabled = false;
            //        btnSubjectDB.Enabled = false;
            //        bnAddressDB.Enabled = false;
            //    }
            //    if (aTree.SelectedNode.Text == enumInterDBType.�߳����ݿ�.ToString())
            //    {
            //        btnDemDB.Enabled = true;
            //        btnEntiDB.Enabled = false;
            //        btnFeaDB.Enabled = false;
            //        btnFileDB.Enabled = false;
            //        btnImageDB.Enabled = false;
            //        btnMapDB.Enabled = false;
            //        btnSubjectDB.Enabled = false;
            //        bnAddressDB.Enabled = false;
            //    }
            //    if (aTree.SelectedNode.Text == enumInterDBType.Ӱ�����ݿ�.ToString())
            //    {
            //        btnImageDB.Enabled = true;
            //        btnDemDB.Enabled = false;
            //        btnEntiDB.Enabled = false;
            //        btnFeaDB.Enabled = false;
            //        btnFileDB.Enabled = false;
            //        btnMapDB.Enabled = false;
            //        btnSubjectDB.Enabled = false;
            //        bnAddressDB.Enabled = false;
            //    }
            //    if (aTree.SelectedNode.Text == enumInterDBType.�ɹ��ļ����ݿ�.ToString())
            //    {
            //        btnFileDB.Enabled = true;
            //        btnDemDB.Enabled = false;
            //        btnEntiDB.Enabled = false;
            //        btnFeaDB.Enabled = false;
            //        btnImageDB.Enabled = false;
            //        btnMapDB.Enabled = false;
            //        btnSubjectDB.Enabled = false;
            //        bnAddressDB.Enabled = false;
            //    }
            //    if (aTree.SelectedNode.Text == enumInterDBType.�������ݿ�.ToString())
            //    {
            //        bnAddressDB.Enabled = true;
            //        btnDemDB.Enabled = false;
            //        btnEntiDB.Enabled = false;
            //        btnFeaDB.Enabled = false;
            //        btnFileDB.Enabled = false;
            //        btnImageDB.Enabled = false;
            //        btnMapDB.Enabled = false;
            //        btnSubjectDB.Enabled = false;
            //    }
            //    if (aTree.SelectedNode.Text == enumInterDBType.����������ݿ�.ToString())
            //    {
            //        btnEntiDB.Enabled = true;
            //        btnDemDB.Enabled = false;
            //        btnFeaDB.Enabled = false;
            //        btnFileDB.Enabled = false;
            //        btnImageDB.Enabled = false;
            //        btnMapDB.Enabled = false;
            //        btnSubjectDB.Enabled = false;
            //        bnAddressDB.Enabled = false;
            //    }
            //    if (aTree.SelectedNode.Text == enumInterDBType.ר��Ҫ�����ݿ�.ToString())
            //    {
            //        btnSubjectDB.Enabled = true;
            //        btnDemDB.Enabled = false;
            //        btnEntiDB.Enabled = false;
            //        btnFeaDB.Enabled = false;
            //        btnFileDB.Enabled = false;
            //        btnImageDB.Enabled = false;
            //        btnMapDB.Enabled = false;
            //        bnAddressDB.Enabled = false;
            //    }
            //    if (aTree.SelectedNode.Text == enumInterDBType.���ӵ�ͼ���ݿ�.ToString())
            //    {
            //        btnMapDB.Enabled = true;
            //        btnDemDB.Enabled = false;
            //        btnEntiDB.Enabled = false;
            //        btnFeaDB.Enabled = false;
            //        btnFileDB.Enabled = false;
            //        btnImageDB.Enabled = false;
            //        btnSubjectDB.Enabled = false;
            //        bnAddressDB.Enabled = false;
            //    }
            //}
            //else
            //{
            //    //��ť������
            //    btnDemDB.Enabled = false;
            //    btnEntiDB.Enabled = false;
            //    btnFeaDB.Enabled = false;
            //    btnFileDB.Enabled = false;
            //    btnImageDB.Enabled = false;
            //    btnMapDB.Enabled = false;
            //    btnSubjectDB.Enabled = false;
            //    bnAddressDB.Enabled = false;
            //}
        }

        private void advTreeProject_NodeMouseDown(object sender, DevComponents.AdvTree.TreeNodeMouseEventArgs e)
        {
            ////////////////////////////////////////���ݿ⹤����ͼ���Ҽ��˵�//////////////////////////
            if (e.Button == MouseButtons.Right && advTreeProject.SelectedNode != null && advTreeProject.SelectedNode.Level==2)
            {
                advTreeProject.SelectedNode.ContextMenu = contextDataSource;
            }
            if (e.Button != MouseButtons.Right || _dicContextMenu == null) return;
            DevComponents.AdvTree.AdvTree aTree = sender as DevComponents.AdvTree.AdvTree;
            if (aTree.SelectedNode == null) return;

            if (aTree.SelectedNode.Text!="���ݿ������") return;  //cyf 20110713 add

            DevComponents.DotNetBar.ButtonItem item = null;
            if (_dicContextMenu.ContainsKey("ContextMenuProTree4"))
            {
                if (_dicContextMenu["ContextMenuProTree4"].Items.Count > 0)
                {
                    item = _dicContextMenu["ContextMenuProTree4"].Items[0] as DevComponents.DotNetBar.ButtonItem;
                    if (item != null)
                    {
                        aTree.SelectedNode.ContextMenu = item;
                    }
                }
            }
        }

        private void MenuItemAttr_Click(object sender, EventArgs e)
        {
            if(advTreeProject.SelectedNode!=null)
                browseDSAttr(advTreeProject.SelectedNode);
        }
        //yjl20111016 add ����Դ����
        private void browseDSAttr(DevComponents.AdvTree.Node inNode)
        {
            string DSname = inNode.Text;
            IWorkspace pWorkspace = Plugin.ModuleCommon.TmpWorkSpace;
            IFeatureWorkspace pFeatureWorkspace = pWorkspace as IFeatureWorkspace;
            ITable pTable = pFeatureWorkspace.OpenTable("DATABASEMD");
            IQueryFilter pQF = new QueryFilterClass();
            pQF.WhereClause = "DATABASENAME='" + DSname + "'";
            ICursor pCursor = pTable.Search(pQF, false);
            if (pCursor == null)
                return;
            IRow pRow = pCursor.NextRow();
            int fdx = pTable.FindField("CONNECTIONINFO");
            if (pRow != null && fdx != -1)
            {
                string value = pRow.get_Value(fdx).ToString();
                frmDSAttribute fmDSAttr = new frmDSAttribute(value,pWorkspace);
                //   fmDSAttr.m_strValue = value;
                if (fmDSAttr.ShowDialog() == DialogResult.OK)
                {
                    //�޸ļ�¼  m_strValue �޸����ݿ��ж�Ӧ�ı��¼
                    Exception eError;
                    SysGisTable sysTable = new SysGisTable(pWorkspace);
                    Dictionary<string, object> dicData = new Dictionary<string, object>();
                    dicData.Add("CONNECTIONINFO", fmDSAttr.m_strValue);
                    if (!sysTable.UpdateRow("DATABASEMD", pQF.WhereClause, dicData, out eError))
                    {
                        MessageBox.Show("����ʧ��: " + eError.Message);
                        return;
                    }


                }

            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(pCursor);
            pCursor = null;
            pFeatureWorkspace = null;
            pWorkspace = null;
        }
        /// <summary>
        /// ZQ 20111119 add
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void groupPanel1_SizeChanged(object sender, EventArgs e)
        {

        }

    }
}
