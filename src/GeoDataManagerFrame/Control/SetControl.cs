using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using GeoDataCenterFunLib;
using System.Data.OleDb;
using System.IO;
using System.Xml;
using SysCommon;
using System.Diagnostics;
//using Microsoft.Office.Tools.Word;

using SysCommon.Error;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Output;
using ESRI.ArcGIS.Display;
using GeoProperties;
//using ESRI.ArcGIS.Controls;
//using ESRI.ArcGIS.Geometry; 
//using GeoProperties;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Geometry;



namespace GeoDataManagerFrame
{
    public partial class SetControl : UserControl
    {
        // private Dictionary<string, DevComponents.DotNetBar.ContextMenuBar> _dicContextMenu;
        //  public IFeatureWorkspace m_pFeatureWorkspace;
        public static TreeNode m_tparent;       //��ͼ�����ڵ�
        public TreeNode m_tTextparent;   //�ĵ������ڵ�
        public TreeNode m_selectnode; //ѡ��ڵ����
        public string m_SaveXmlFileName = "";
        public string m_strLogFilePath = Application.StartupPath + "\\..\\Log\\DataCenterLog.txt";
        private static object docPath;//��ʱdoc�ļ�
        private Microsoft.Office.Interop.Word.Application wordApp;//wordӦ��
        private Microsoft.Office.Interop.Word.Document doc;//word�ĵ�
        private static List<int> wordProcess = new List<int>();//word���̼�
        private TreeNode m_LastDragNode = null;
        private short m_transpy = 0;//͸���� 
        private double m_scale = 0;//������
        private TreeNode m_CurEditLayerNode; 
        private TreeNode m_CurEditTopicNode;
        private bool XzqLoad=false;
        private bool FeatureTrue = true;//�ж��ǲ���ʸ������ͼ��2011.0414 xs
        private��bool flag = true;
        private bool flag2 = true;
        private string m_strnew = "";//�ж��Ƿ������ӵ�ͼ��

        //��ͼ�������������
        private Control _MapToolControl;
        private double TempX;
        private double TempY;
        System.Drawing.Point pPoint = new System.Drawing.Point();
        private IGeometry MGeometry = null;//��mapctrl�ϻ��ķ�Χ����������ͼ����ת
        public SetControl(string strName, string strCation)
        {
            InitializeComponent();

            //��ʼ�����ö�Ӧ��ͼ�ؼ�
            InitialMainViewControl();
            this.Dock = System.Windows.Forms.DockStyle.Fill;

            this.Name = strName;
            this.Tag = strCation;

            //���ݲ���
            ModFrameData.v_AppGisUpdate.MainUserControl = this;
            ModFrameData.v_AppGisUpdate.ArcGisMapControl = axMapControl;
         //   ModFrameData.v_AppGisUpdate.TOCControl = axTOCControl.Object as ITOCControlDefault;
            ModFrameData.v_AppGisUpdate.MapControl = axMapControl.Object as IMapControlDefault;
            ModFrameData.v_AppGisUpdate.CurrentControl = axMapControl.Object;
            ModFrameData.v_AppGisUpdate.MapDocTree = MapDocTree;
          //  ModFrameData.v_AppGisUpdate.TextDocTree = TextDoctree;
          //  ModFrameData.v_AppGisUpdate.DataIndexTree = DataIndexTree;
            ModFrameData.v_AppGisUpdate.DataUnitTree = DataUnitTree;
            // ModFrameData.v_AppGisUpdate.UserResultTree = UserResultTree;
            ModFrameData.v_AppGisUpdate.Visible = this.Visible;
            ModFrameData.v_AppGisUpdate.Enabled = this.Enabled;
            ModFrameData.v_AppGisUpdate.CurrentThread = null;
            ModFrameData.v_AppGisUpdate.tipRichBox = tipRichBox;

          //  ModFrameData.v_AppGisUpdate.DocControl = RichBoxWordDoc;
            //��־�ļ�·��
            ModFrameData.v_AppGisUpdate.strLogFilePath = m_strLogFilePath;
            ModFrameData.v_AppGisUpdate.IndextabControl = IndextabControl;


            //����sys�����ļ���Ӳ˵��͹�����
            InitialFrmDefControl();
            InitOutPutResultTree();
            DataUnitTree.ImageList = imageList;

            (axMapControl.ActiveView as IActiveViewEvents_Event).AfterDraw += new IActiveViewEvents_AfterDrawEventHandler(SetControl_mapctrl_afterdraw);
      
        }
        //��ʼ���û��ɹ��б�
        public void InitOutPutResultTree()
        {
            treeViewOutPutResults.Nodes.Clear();
            string strFilePath = Application.StartupPath + "\\..\\Res\\Xml\\OutputResultsTreeIndex.Xml";
            CreatOutPutTree(treeViewOutPutResults, strFilePath);

        }
  
        public void CreatOutPutTree(TreeView toTreeView, string strXmlPath)
        {
            TreeNode tparent;
            tparent = new TreeNode();
            tparent.Text = "�б�";
            tparent.Tag = 0;
            tparent.ImageIndex = 13;
            tparent.SelectedImageIndex = 13;
            toTreeView.Nodes.Add(tparent);
            toTreeView.ExpandAll();


            //�����ӽڵ�
            TreeNode tNewNode;
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();

            //������ȡitemName��Ϣ 
            string strTblName = "";
            string strTblPath = "";
            XmlDocument xmldoc = new XmlDocument();
            if (xmldoc != null)
            {
                if (File.Exists(strXmlPath))
                {
                    xmldoc.Load(strXmlPath);

                    //�޸ĸ��ڵ�ڵ�����
                    string strRootName = "";
                    string strSearchRoot = "//Rootset";
                    string strRootPath = "";
                    XmlNode xmlNodeRoot = xmldoc.SelectSingleNode(strSearchRoot);
                    XmlElement xmlElentRoot = (XmlElement)xmlNodeRoot;
                    strRootName = xmlElentRoot.GetAttribute("Caption");
                    strRootPath = xmlElentRoot.GetAttribute("Path");
                    tparent.Text = strRootName;
                    tparent.Name = strRootPath;
                    //������ӵ�һ���ӽڵ� Childset
                    string strSearch = "//Childset";
                    XmlNode xmlNode = xmldoc.SelectSingleNode(strSearch);
                    XmlNodeList xmlNdList;
                    xmlNdList = xmlNode.ChildNodes;
                    foreach (XmlNode xmlChild in xmlNdList)
                    {
                        strTblName = "";
                        XmlElement xmlElent = (XmlElement)xmlChild;
                        strTblName = xmlElent.GetAttribute("Caption");
                        strTblPath = xmlElent.GetAttribute("Path");
                        tNewNode = new TreeNode();
                        tNewNode.Text = strTblName;
                        tNewNode.Name = strTblPath;
                        tNewNode.Tag = 1;
                        tNewNode.ImageIndex = 11;
                        tNewNode.SelectedImageIndex = 12;
                        tparent.Nodes.Add(tNewNode);
                        tparent.ExpandAll();

                        //��������ӽڵ�
                        AddLeafItemFromFile(tNewNode, xmlChild);
                    }
                }
            }

        }
        public void AddLeafItemFromFile(TreeNode treeNode, XmlNode xmlNode)
        {
            string path;
            path = Application.StartupPath + "\\..\\" + treeNode.Parent.Name + "\\" + treeNode.Name;
            if (Directory.Exists(path))
            {
                TreeNode tChildNode, tChildChildNode;
               // string strTblName = "";
                DirectoryInfo Dinfo = new DirectoryInfo(path);
                foreach (DirectoryInfo eachinfo in Dinfo.GetDirectories())
                {
                    tChildNode = new TreeNode();
                    tChildNode.Text = eachinfo.Name;
                    tChildNode.Name = eachinfo.FullName;
                    tChildNode.ImageIndex = 11;
                    tChildNode.SelectedImageIndex = 12;
                    treeNode.Nodes.Add(tChildNode);
                    foreach (FileInfo Finfo in eachinfo.GetFiles("*.cel"))
                    {
                        tChildChildNode = new TreeNode();
                        tChildChildNode.Text = Finfo.Name.Substring(0,Finfo.Name.IndexOf("."));
                        tChildChildNode.Name = Finfo.FullName;
                        tChildChildNode.ImageIndex = 15;
                        tChildChildNode.SelectedImageIndex =15;
                        tChildNode.Nodes.Add(tChildChildNode);
                    }
                    foreach (FileInfo Finfo in eachinfo.GetFiles("*.mdb"))
                    {
                        tChildChildNode = new TreeNode();
                        tChildChildNode.Text = Finfo.Name.Substring(0, Finfo.Name.IndexOf("."));
                        tChildChildNode.Name = Finfo.FullName;
                        tChildChildNode.ImageIndex = 18;
                        tChildChildNode.SelectedImageIndex = 18;
                        tChildNode.Nodes.Add(tChildChildNode);
                    }
                    foreach (FileInfo Finfo in eachinfo.GetFiles("*.mxd"))
                    {
                        tChildChildNode = new TreeNode();
                        tChildChildNode.Text = Finfo.Name.Substring(0, Finfo.Name.IndexOf("."));
                        tChildChildNode.Name = Finfo.FullName;
                        tChildChildNode.ImageIndex = 17;
                        tChildChildNode.SelectedImageIndex = 17;
                        tChildNode.Nodes.Add(tChildChildNode);
                    }
                }
            }

        }
        public void CreatTreeFromXmlFile(TreeView toTreeView, string strXmlPath)
        {
            TreeNode tparent;
            tparent = new TreeNode();
            tparent.Text = "�б�";
            tparent.Tag = 0;
            tparent.ImageIndex = 13;
            tparent.SelectedImageIndex = 13;
            toTreeView.Nodes.Add(tparent);
            toTreeView.ExpandAll();


            //�����ӽڵ�
            TreeNode tNewNode;
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();

            //������ȡitemName��Ϣ 
            string strTblName = "";
            XmlDocument xmldoc = new XmlDocument();
            if (xmldoc != null)
            {
                if (File.Exists(strXmlPath))
                {
                    xmldoc.Load(strXmlPath);

                    //�޸ĸ��ڵ�ڵ�����
                    string strRootName = "";
                    string strSearchRoot = "//Rootset";
                    XmlNode xmlNodeRoot = xmldoc.SelectSingleNode(strSearchRoot);
                    XmlElement xmlElentRoot = (XmlElement)xmlNodeRoot;
                    strRootName = xmlElentRoot.GetAttribute("sItemName");
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
                        strTblName = xmlElent.GetAttribute("Caption");

                        tNewNode = new TreeNode();
                        tNewNode.Text = strTblName;
                        tNewNode.Tag = 1;
                        tNewNode.ImageIndex = 17;
                        tNewNode.SelectedImageIndex = 17;
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
                    strTblName = xmlElent.GetAttribute("Caption");
                    tNewNode = new TreeNode();
                    tNewNode.Text = strTblName;
                    tNewNode.Tag = 2;
                    tNewNode.ImageIndex = 17;
                    tNewNode.SelectedImageIndex = 17;
                    treeNode.Nodes.Add(tNewNode);
                }
                treeNode.ExpandAll();
            }
        }



        private void InitialMainViewControl()
        {
            frmBarManager newfrmBarManager = new frmBarManager();
            newfrmBarManager.TopLevel = false;
            newfrmBarManager.Dock = DockStyle.Fill;
            newfrmBarManager.Show();
            this.Controls.Add(newfrmBarManager);


            //��������������������
            DevComponents.DotNetBar.Bar barIndexView = newfrmBarManager.CreateBar("barIndexView", enumLayType.FILL);
            barIndexView.CanHide = false;
            DevComponents.DotNetBar.PanelDockContainer PanelIndexView = newfrmBarManager.CreatePanelDockContainer("PanelIndexView", barIndexView);
            PanelIndexView.Controls.Add(this.IndextabControl);
            this.IndextabControl.Dock = DockStyle.Fill;


            //����������ͼ����
     /*       DevComponents.DotNetBar.Bar barMapView = newfrmBarManager.CreateBar("barMapView", enumLayType.FILL);
            barMapView.CanHide = false;
            DevComponents.DotNetBar.PanelDockContainer PanelMapView = newfrmBarManager.CreatePanelDockContainer("PanelMapView", barMapView);
            PanelMapView.Controls.Add(this.panelCenterMain);
            this.panelCenterMain.Dock = DockStyle.Fill;
*/

            //����������ͼ����
            DevComponents.DotNetBar.Bar barMapView = newfrmBarManager.CreateBar("barMapView", enumLayType.FILL);
            DevComponents.DotNetBar.PanelDockContainer PanelMap = newfrmBarManager.CreatePanelDockContainer("PanelMapView", barMapView);
          //  DockContainerItem MapContainerItem = newfrmBarManager.CreateDockContainerItem("TreeContainerItem", "������ͼ", PanelMap, barMapView);
            PanelMap.Controls.Add(this.panelCenterMain);
            this.panelCenterMain.Dock = DockStyle.Fill;
            _MapToolControl = PanelMap as Control;

            //��������
            newfrmBarManager.MainDotNetBarManager.FillDockSite.GetDocumentUIManager().Dock(barIndexView, barMapView, eDockSide.Right);
            newfrmBarManager.MainDotNetBarManager.FillDockSite.GetDocumentUIManager().SetBarWidth(barIndexView, this.Width / 5);

            //����������ʾ����
            //�û�������
            PanelDockContainer PanelTipData = new PanelDockContainer();
            PanelTipData.Controls.Add(this.tipRichBox);
            this.tipRichBox.ContextMenuStrip = contextMenuLog;
            this.tipRichBox.Dock = DockStyle.Fill;
            DockContainerItem dockItemData = new DockContainerItem("dockItemData", "��ʾ");
            dockItemData.Control = PanelTipData;
            newfrmBarManager.ButtomBar.Items.Add(dockItemData);
        }


        //��ʼ����ܲ���ؼ�����
        //����sys�����ļ���Ӳ˵��͹�����
        private void InitialFrmDefControl()
        {
            //�õ�Xml��System�ڵ�,����XML���ز������
            string xPath = ".//System[@Name='" + this.Name + "']";
            ModFrameData.v_AppGisUpdate.ScaleBoxList = new List<ComboBoxItem>();
            Plugin.ModuleCommon.LoadButtonViewByXmlNode(ModFrameData.v_AppGisUpdate.ControlContainer, xPath, ModFrameData.v_AppGisUpdate);

            //��ʼ����ͼ���������
            Plugin.Application.IAppFormRef pAppFrm = ModFrameData.v_AppGisUpdate as Plugin.Application.IAppFormRef;
            XmlNode barXmlNode = pAppFrm.SystemXml.SelectSingleNode(".//ToolBar[@Name='ControlMapToolBar8']");
            if (barXmlNode == null || _MapToolControl == null) return;
            //DevComponents.DotNetBar.Bar mapToolBar = Plugin.ModuleCommon.LoadButtonView(_MapToolControl, barXmlNode, pAppFrm, null, false) as Bar;
            DevComponents.DotNetBar.Bar mapToolBar = Plugin.ModuleCommon.LoadButtonView(_MapToolControl, barXmlNode, pAppFrm, null) as Bar;
            if (mapToolBar != null)
            {
                mapToolBar.AccessibleRole = System.Windows.Forms.AccessibleRole.ToolBar;
                mapToolBar.DockOrientation = DevComponents.DotNetBar.eOrientation.Vertical;
                mapToolBar.DockSide = DevComponents.DotNetBar.eDockSide.Left;
                mapToolBar.GrabHandleStyle = DevComponents.DotNetBar.eGrabHandleStyle.Office2003;
                mapToolBar.Style = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            }
        }




        //���ݵ�Ԫ���ڵ㱻��������Ӧ
        //�������ݵ�Ԫ����Ϣ������ԴĿ¼��
        //Ŀǰֻ��Լ���Ϊ3�Ľڵ㣨�ؼ��ڵ㣩����Ӧ���������Կ��Ƕ��м��ڵ���Ӧ�����ǻ������ԴĿ¼�б�����̫������
        private void DataUnitTree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (DataUnitTree.SelectedNode != e.Node)
            {
                if (DataUnitTree.SelectedNode != null)
                {
                    DataUnitTree.SelectedNode.ForeColor = Color.Black;
                }

                DataUnitTree.SelectedNode = e.Node;
                e.Node.ForeColor = Color.Red;

                string strItemName = DataUnitTree.SelectedNode.Name;
                string strItemText = DataUnitTree.SelectedNode.Text;

                //=================================
                //���ߣ�ϯʤ 
                //ʱ�䣺2011-02-21
                //˵�����жϽڵ㼶��
                //=================================
                //DataUnitTree.SelectedImageIndex = null;
                switch (DataUnitTree.SelectedNode.Level.ToString())
                {
                    case "0":
                        DataUnitTree.SelectedNode.SelectedImageIndex = 0;
                        break;
                    case "1":
                        DataUnitTree.SelectedNode.SelectedImageIndex = 17;
                        break;
                    case "2":
                        DataUnitTree.SelectedNode.SelectedImageIndex = 17;
                        break;
                    case "3":
                        DataUnitTree.SelectedNode.SelectedImageIndex = 17;
                        break;
                    default:
                        break;

                }
                DataUnitTree.Refresh();

                //���ݵ�ѡ�ڵ� ���� AxMapControl ����
                //?     UpdateMapControl(strItemName,strItemText);

                if (DataUnitTree.SelectedNode.Level.Equals(2))//ֻ����ؼ��ڵ�
                {
                    DataUnitTree.SelectedNode.ExpandAll();//չ�����������ڵ�
                }
            }

            //�Ҽ������ʱ�򵯳��Ҽ��˵�
            if (e.Button == MouseButtons.Right)
            {

                System.Drawing.Point ClickPoint = DataUnitTree.PointToScreen(new System.Drawing.Point(e.X, e.Y));
                TreeNode tSelNode;
                tSelNode = e.Node;
                if (tSelNode != null)
                {

                    if (tSelNode.Level == 2)//�������ڵ�
                        DataUnitcontextMenu.Show(ClickPoint);
                    else if (tSelNode.Level == 4)//Ҷ�ӽڵ�
                    {
                        DataIndexTreecontextMenu.Show(ClickPoint);
                        MenuItemExport.Visible = false;
                        MenuItemAddLoadData.Visible = true;
                        MenuItemAtt.Visible = true;
                        MenuItemLoadData.Visible = true;
                        m_selectnode = e.Node;
                    }
                    else if (tSelNode.Level == 3)//ר��ڵ�
                    {
                        DataIndexTreecontextMenu.Show(ClickPoint);
                        MenuItemExport.Visible = true;
                        MenuItemAddLoadData.Visible = false;
                        MenuItemAtt.Visible = false;
                        MenuItemLoadData.Visible = false;
                    }

                }
            }
        }

        //���ݵ�ѡ�ڵ� ���� AxMapControl ����
        public void UpdateMapControl(string strXzqCode, string strXzqName)
        {
            if (strXzqCode.Equals(""))
                return;

            //����û�е�������
            if (MapDocTree.GetNodeCount(true) > 0)
                return;

            axMapControl.ActiveView.Clear();

            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            string mypath = dIndex.GetDbInfo();
            string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
            string strExp = "select �ű��ļ� from ���ݵ�Ԫ�� where �������� = '" + strXzqCode + "'";
            GeoDataCenterDbFun dDbFun = new GeoDataCenterDbFun();
            string strIndexName = dDbFun.GetInfoFromMdbByExp(strCon, strExp);

            //��workspace�л�ȡ��Ӧ��layer
            string dbpath = dIndex.GetDbValue("dbServerPath");

            IWorkspaceFactory2 rasterWorkspaceFactory = new FileGDBWorkspaceFactoryClass();

            IRasterWorkspaceEx rasterWorkspace = rasterWorkspaceFactory.OpenFromFile(dbpath, 0) as IRasterWorkspaceEx;

            //??Ҫ�ж��ļ��Ƿ����
            IWorkspace2 pWorkspace2 = rasterWorkspace as IWorkspace2;
            if (pWorkspace2.get_NameExists(esriDatasetType.esriDTRasterDataset, strIndexName))
            {
                IRasterDataset rasterDataset = rasterWorkspace.OpenRasterDataset(strIndexName);
                IRasterLayer pRasterLayer = new RasterLayerClass();
                pRasterLayer.CreateFromDataset(rasterDataset);
                //axMapControl.Map.AddLayer(pRasterLayer);
                IMapLayers pmaplayers = axMapControl.Map as IMapLayers;
                pmaplayers.InsertLayer(pRasterLayer, false, pmaplayers.LayerCount);
            }
            else
            {
                axMapControl.ActiveView.Clear();
                // MessageBox.Show("���ݵ�Ԫ��" + strXzqName + "����Ӧ�������ļ�������", "ϵͳ��ʾ");   
            }

            axMapControl.ActiveView.Refresh();
        }

        ////��ʼ���������� 
        //public void InitDataIndexTree(string strXzqCode, string strXzqName)
        //{
        //    GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
        //    string mypath = dIndex.GetDbInfo();
        //    string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���

        //    //������������ӵ�ͼ�����Ϣ���л�ȡ��Ӧ�������Ϣ
        //    CreateDataIndexTree(strCon, strXzqCode, strXzqName);
        //}

  

        //�������� �˵���Ӧ
        private void MenuItemLoadData_Click(object sender, EventArgs e)
        {

            // TreeNode tCurNode = DataIndexTree.SelectedNode;
            if (m_selectnode == null) return;
            TreeNode tCurNode = m_selectnode;
            //��ȡר������
            string strSubType = tCurNode.Tag.ToString();
            //��ʼ��������
            SysCommon.CProgress vProgress = new SysCommon.CProgress("������");
            vProgress.EnableCancel = false;
            vProgress.ShowDescription = true;
            vProgress.FakeProgress = true;
            vProgress.TopMost = true;
            vProgress.ShowProgress();


            //��ȡģ��·�� 
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            //  string mypath = dIndex.GetDbValue("dbServerPath");
            string mypath = dIndex.GetDbInfo();
            string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
            string strExp = "select �ű��ļ� from ��׼ר����Ϣ�� where ר������ ='" + strSubType + "'";

            //����ר�����ʹӱ�׼ר����Ϣ�� �л�ȡ��Ӧ��ģ���ļ�·��
            GeoDataCenterDbFun dDbFun = new GeoDataCenterDbFun();
            string strModpath = dDbFun.GetInfoFromMdbByExp(strCon, strExp);

            //��ȡģ��·��
            string strModFile = Application.StartupPath + "\\..\\Template\\" + strModpath;

            if (!File.Exists(strModFile))
            {
                vProgress.Close();
                MessageBox.Show("�ű��ļ������ڣ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            //���Ƶ�Temp�ļ�����
            string strWorkFile = Application.StartupPath + "\\..\\Temp\\CurPrj.xml";
            m_SaveXmlFileName = strWorkFile;
            File.Copy(strModFile, strWorkFile, true);

            //��ʼ����ͼ�ĵ���
            string strBuffer = tCurNode.Text;
            TreeNode tParent = tCurNode.Parent;
            TreeNode tRoot = tParent.Parent;
            string strCodeUnitName = tRoot.Text;
            string strSuffixx = strCodeUnitName + strBuffer; //��׺ 

            //��¼��־
            LogFile log = new LogFile(tipRichBox, m_strLogFilePath);
            string strLog = "��ʼ����" + tCurNode.Parent.Text + "_" + strSuffixx;
            if (log != null)
            {
                log.Writelog(strLog);
            }
            vProgress.SetProgress(strLog);

            //��ȡ�����������루���ݵ�Ԫ���룩
            string strCodeUnitCode = tRoot.Name;

            //��ȡ���
            string strYear = strBuffer.Substring(0, 4);

            //��ȡ�����ߴ���
            int iStartPos = strBuffer.IndexOf("��");
            int iEndPos = strBuffer.IndexOf("��");
            int iLength = iEndPos - iStartPos - 1;
            string strScaleName = strBuffer.Substring(iStartPos + 1, iLength);
            strExp = "select ���� from �����ߴ���� where ���� ='" + strScaleName + "'";
            string strScaleCode = dDbFun.GetInfoFromMdbByExp(strCon, strExp);

            //���ǰ�μ��ص�ͼ��
            axMapControl.Map.ClearLayers();

            //��ʼ����ͼ�ĵ���  ���ݵ�Ԫ����   ���  ������  ר������
            InitMapDocTree(strWorkFile, strSuffixx, strCodeUnitCode, strYear, strScaleCode, strSubType, vProgress);

            vProgress.SetProgress("��ʼ���ĵ���");
            //��ʼ���ĵ���
            InitTextDocTree(strWorkFile, strSuffixx, strCodeUnitCode, strYear, strScaleCode, strSubType, vProgress);

            //��ʼ��TOC�ؼ�  wgf
            //?   InitTocTree(strWorkFile, strSuffixx, strCodeUnitCode, strYear, strScaleCode, strSubType);

            vProgress.Close();
            if (!XzqLoad)//��������������
            {
                //��������ת����ͼ�ĵ�����
                IndextabControl.SelectedTab = PageMapDoc;

                //��ͼ������ת��ͼ���������
            //    CenterTabControl.SelectedTab = MapPage;
            }
            InitXZQTree();
            InitJHTBTree();
        }
        //yjl�������ݺ����XZQ������ݵ�Ԫ��ʼ���ұߵ���������
        private void InitXZQTree()
        {
            IMap mMap = axMapControl.Map;
            TreeNode[] mNode = MapDocTree.Nodes.Find("������",true);
            if (mNode.Length == 0)
                return;
            TreeNode city=DataUnitTree.Nodes[0].Nodes[0];
            treeViewXZQ.Nodes.Add(city.Name,city.Text,18,19);
            city = DataUnitTree.Nodes[0].Nodes[0].Nodes[0];
            treeViewXZQ.Nodes[0].Nodes.Add(city.Name, city.Text, 18, 19);


            TreeNode mTN = mNode[0];
            
            IFeatureLayer mFL = GetLayerofTreeNode(mTN) as IFeatureLayer;//call sub proc
            IFeatureClass mFC = mFL.FeatureClass;
            IFeatureCursor mFCs = mFC.Search(null, false);
            IFeature mF = mFCs.NextFeature();
            int fdXZQMC = mFC.Fields.FindField("XZQMC");
            int fdXZQDM = mFC.Fields.FindField("XZQDM");
            while (mF != null)
            {
                try
                {
                    TreeNode tn = new TreeNode();
                    tn.Text = mF.get_Value(fdXZQMC).ToString();
                    tn.Name = mF.get_Value(fdXZQDM).ToString();
                    tn.ImageIndex = 18;
                    tn.SelectedImageIndex = 19;
                    treeViewXZQ.Nodes[0].Nodes[0].Nodes.Add(tn);
                }
                catch
                {
                    
                }
                mF = mFCs.NextFeature();
            }
            treeViewXZQ.ExpandAll();

            
 
        }
        //yjl�������ݺ��ʼ���ұߵĽӺ�ͼ����
        private void InitJHTBTree()
        {
            IMap mMap = axMapControl.Map;
            TreeNode[] mNode = MapDocTree.Nodes.Find("�Ӻ�ͼ��", true);
            if (mNode.Length == 0)
                return;
          
            treeViewJHTable.Nodes.Add("ͼ����", "ͼ����", 18, 19);
   
            TreeNode mTN = mNode[0];

            IFeatureLayer mFL = GetLayerofTreeNode(mTN) as IFeatureLayer;//call sub proc
            IFeatureClass mFC = mFL.FeatureClass;
            IFeatureCursor mFCs = mFC.Search(null, false);
            IFeature mF = mFCs.NextFeature();
            int fdNEWMAPNO = mFC.Fields.FindField("NEWMAPNO");
            //int fdXZQDM = mFC.Fields.FindField("XZQDM");
            while (mF != null)
            {
                try
                {
                    TreeNode tn = new TreeNode();
                    tn.Text = mF.get_Value(fdNEWMAPNO).ToString();
                    tn.Name = tn.Text;//mF.get_Value(fdXZQDM).ToString();
                    tn.ImageIndex = 18;
                    tn.SelectedImageIndex = 19;
                    treeViewJHTable.Nodes[0].Nodes.Add(tn);
                }
                catch
                {

                }
                mF = mFCs.NextFeature();
            }
            treeViewJHTable.ExpandAll();



        }
        //�������� �˵���Ӧ ���ݵ�ͼ�ĵ��ļ�������
        public void LoadDatafromXml(string strXmlFile, SysCommon.CProgress vProgress)
        {

            //���ǰ�μ��ص�ͼ��
            axMapControl.Map.ClearLayers();

            //��ʼ����ͼ�ĵ���  ���ݵ�Ԫ����   ���  ������  ר������
            InitMapDocTreefromXml(strXmlFile, vProgress);


            //��������ת����ͼ�ĵ�����
            IndextabControl.SelectedTab = PageMapDoc;

            //��ͼ������ת��ͼ���������
         //   CenterTabControl.SelectedTab = MapPage;

        }



        //��ʼ��TOC����,���޸�Layer�ڵ��sFile����
        public void InitTocTree(string strModPath, string strSuffixx, string strXzq, string strYear, string strScale, string strSubType)
        {


        }

        //��ʼ���ĵ���
        public void InitTextDocTree(string strModPath, string strSuffixx, string strXzq, string strYear, string strScale, string strSubType, SysCommon.CProgress vProgress)
        {
            //�ж��ļ��Ƿ����
        /*    XmlDocument xmldoc = new XmlDocument();
            if (!File.Exists(strModPath))
                return;
            xmldoc.Load(strModPath);

            //�ӵ�ͼ�����Ϣ���л�ȡ�����������Ϣ��ͼ����ɣ�
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            string mypath = dIndex.GetDbInfo();
            string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���


            //��ȡ�ĵ���
            GeoDataCenterDbFun dDbFun = new GeoDataCenterDbFun();
            string strExp = "select �ĵ��� from ��ͼ�����Ϣ�� where �������� ='" + strXzq + "'" + "And " + " ���='" +
               strYear + "'" + "And " + " ������='" + strScale + "'" + "And " + " ר������='" + strSubType + "'";
            string strDocLib = dDbFun.GetInfoFromMdbByExp(strCon, strExp);


            //������  GisMap ���ڵ� Group ר��ڵ� Layer Ҷ�ӽڵ�
            TextDoctree.Nodes.Clear();

            //���ڵ�
            m_tTextparent = new TreeNode();
            m_tTextparent.Text = "�ĵ�Ŀ¼";
            TextDoctree.Nodes.Add(m_tTextparent);
            TextDoctree.ExpandAll();
            m_tTextparent.ImageIndex = 18;
            m_tTextparent.SelectedImageIndex = 18;

            TreeNode tGroupNode = new TreeNode();
            tGroupNode.Text = strSuffixx;
            tGroupNode.Name = strDocLib;
            tGroupNode.ExpandAll();
            m_tTextparent.Nodes.Add(tGroupNode);
            tGroupNode.ImageIndex = 18;
            tGroupNode.SelectedImageIndex = 18;

            TreeNode tLeafNode;
            string strSearchRoot = "//GisMap";
            XmlNode xmlNodeRoot = xmldoc.SelectSingleNode(strSearchRoot);

            //�����ĵ��������ݾ�����ĵ���
            if (strDocLib != null)
            {
                //����xml�ļ� ����ڵ�
                XmlElement xmlGridGroup = xmldoc.CreateElement("SubGroup");
                xmlGridGroup.SetAttribute("sItemName", "���ĵ����ݡ�");
                xmlGridGroup.SetAttribute("sType", "GROUP");
                xmlGridGroup.SetAttribute("sDataSource", strDocLib);
                xmlGridGroup.SetAttribute("sGroupType", "DOC");
                xmlNodeRoot.AppendChild(xmlGridGroup);

                //����ĵ�Ҷ�ӽڵ�
                strExp = "select * from �ĵ������Ϣ�� where �������� ='" + strXzq + "'" + "And " + " ���='" + strYear + "'" +
                     "And " + " ������='" + strScale + "'" + "And " + " ר������='" + strSubType + "'";
                OleDbConnection mycon = new OleDbConnection(strCon);   //����OleDbConnection����ʵ�����������ݿ�
                OleDbCommand aCommand = new OleDbCommand(strExp, mycon);
                try
                {
                    mycon.Open();

                    //����datareader   ���������ӵ���     
                    OleDbDataReader aReader = aCommand.ExecuteReader();

                    //��� ר�� ����������
                    while (aReader.Read())
                    {
                        string strDocFilePath = strYear + strSubType + strXzq + "\\" + aReader["�ĵ�����"].ToString() + "." + aReader["�ĵ�����"].ToString();
                        XmlElement xmlDocLeaf = xmldoc.CreateElement("Layer");
                        xmlDocLeaf.SetAttribute("sItemName", aReader["�ĵ�����"].ToString());
                        xmlDocLeaf.SetAttribute("sFile", strDocFilePath);
                        xmlDocLeaf.SetAttribute("sType", aReader["�ĵ�����"].ToString());
                        xmlGridGroup.AppendChild(xmlDocLeaf);

                        tLeafNode = new TreeNode();
                        tLeafNode.Text = aReader["�ĵ�����"].ToString();
                        tLeafNode.Name = strDocFilePath;
                        tLeafNode.ImageIndex = 19;
                        tLeafNode.SelectedImageIndex = 19;
                        tGroupNode.Nodes.Add(tLeafNode);

                    }
                    //�ر�reader����     
                    aReader.Close();

                    //�ر�����,�����Ҫ     
                    mycon.Close();
                }
                catch (System.Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                //��ӵ����ؼ���

            }

            //ɾ������ֵΪ�յĽڵ�
            DeleNullElementForXml(xmldoc);
            xmldoc.Save(strModPath);*/
        }

        //׷�ӵ��ĸ����ĵ���
        public void AddUpdateTextDocTree(string strModPath, string strSuffixx, string strXzq, string strYear, string strScale, string strSubType, SysCommon.CProgress vProgress)
        {
            //�ж��ļ��Ƿ����
            XmlDocument xmldoc = new XmlDocument();
            if (!File.Exists(strModPath))
                return;
            xmldoc.Load(strModPath);

            if (m_tTextparent == null)
                return;

            //�ӵ�ͼ�����Ϣ���л�ȡ�����������Ϣ��ͼ����ɣ�
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            string mypath = dIndex.GetDbInfo();
            string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���


            //��ȡ�ĵ���
            GeoDataCenterDbFun dDbFun = new GeoDataCenterDbFun();
            string strExp = "select �ĵ��� from ��ͼ�����Ϣ�� where �������� ='" + strXzq + "'" + "And " + " ���='" +
               strYear + "'" + "And " + " ������='" + strScale + "'" + "And " + " ר������='" + strSubType + "'";
            string strDocLib = dDbFun.GetInfoFromMdbByExp(strCon, strExp);

            //���ڵ�
            TreeNode tGroupNode = new TreeNode();
            tGroupNode.Text = strSuffixx;
            tGroupNode.ExpandAll();
            m_tTextparent.Nodes.Add(tGroupNode);
            tGroupNode.ImageIndex = 18;
            tGroupNode.SelectedImageIndex = 18;

            TreeNode tLeafNode;
            string strSearchRoot = "//GisMap";
            XmlNode xmlNodeRoot = xmldoc.SelectSingleNode(strSearchRoot);

            //�����ĵ��������ݾ�����ĵ���
            if (strDocLib != null)
            {
                //����xml�ļ� ����ڵ�
                XmlElement xmlGridGroup = xmldoc.CreateElement("SubGroup");
                xmlGridGroup.SetAttribute("sItemName", "���ĵ����ݡ�");
                xmlGridGroup.SetAttribute("sType", "GROUP");
                xmlGridGroup.SetAttribute("sDataSource", strDocLib);
                xmlGridGroup.SetAttribute("sGroupType", "DOC");
                xmlNodeRoot.AppendChild(xmlGridGroup);

                //����ĵ�Ҷ�ӽڵ�
                strExp = "select * from �ĵ������Ϣ�� where �������� ='" + strXzq + "'" + "And " + " ���='" + strYear + "'" +
                     "And " + " ������='" + strScale + "'" + "And " + " ר������='" + strSubType + "'";
                OleDbConnection mycon = new OleDbConnection(strCon);   //����OleDbConnection����ʵ�����������ݿ�
                OleDbCommand aCommand = new OleDbCommand(strExp, mycon);
                try
                {
                    mycon.Open();

                    //����datareader   ���������ӵ���     
                    OleDbDataReader aReader = aCommand.ExecuteReader();

                    //��� ר�� ����������
                    while (aReader.Read())
                    {
                        string strDocFilePath = strYear + strSubType + strXzq + "\\" + aReader["�ĵ�����"].ToString() + "." + aReader["�ĵ�����"].ToString();
                        XmlElement xmlDocLeaf = xmldoc.CreateElement("Layer");
                        xmlDocLeaf.SetAttribute("sItemName", aReader["�ĵ�����"].ToString());
                        xmlDocLeaf.SetAttribute("sFile", strDocFilePath);
                        xmlDocLeaf.SetAttribute("sType", aReader["�ĵ�����"].ToString());
                        xmlGridGroup.AppendChild(xmlDocLeaf);

                        tLeafNode = new TreeNode();
                        tLeafNode.Text = aReader["�ĵ�����"].ToString();
                        tLeafNode.Name = strDocFilePath;
                        tLeafNode.ImageIndex = 19;
                        tLeafNode.SelectedImageIndex = 19;
                        tGroupNode.Nodes.Add(tLeafNode);

                    }
                    //�ر�reader����     
                    aReader.Close();

                    //�ر�����,�����Ҫ     
                    mycon.Close();
                }
                catch (System.Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            }

            //ɾ������ֵΪ�յĽڵ�
            DeleNullElementForXml(xmldoc);
            xmldoc.Save(strModPath);

        }

        //��ʼ����ͼ�ĵ���,���޸�Layer�ڵ��sFile����
        public void  InitMapDocTree(string strModPath, string strSuffixx, string strXzq, string strYear, string strScale, string strSubType, SysCommon.CProgress vProgress)
        {
            //�ж��ļ��Ƿ����
            XmlDocument xmldoc = new XmlDocument();
            if (!File.Exists(strModPath))
                return;
            xmldoc.Load(strModPath);

            //�ӵ�ͼ�����Ϣ���л�ȡ�����������Ϣ��ͼ����ɣ�
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            //    string mypath = dIndex.GetDbValue("dbServerPath");
            string mypath = dIndex.GetDbInfo();
            string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
            string strExp = "select ͼ����� from ��ͼ�����Ϣ�� where �������� ='" + strXzq + "'" + "And " + " ���='" +
                strYear + "'" + "And " + " ������='" + strScale + "'" + "And " + " ר������='" + strSubType + "'";
            GeoDataCenterDbFun dDbFun = new GeoDataCenterDbFun();
            string strLayerGroup = dDbFun.GetInfoFromMdbByExp(strCon, strExp);

            //��������
            //strExp = "select �������� from ��ͼ�����Ϣ�� where �������� ='" + strXzq + "'" + "And " + " ���='" +
            //   strYear + "'" + "And " + " ������='" + strScale + "'" + "And " + " ר������='" + strSubType + "'";
            //string strDataType = dDbFun.GetInfoFromMdbByExp(strCon, strExp);

            //ҵ�����  ���� ���� �ػ�
            strExp = "select ҵ����� from ��ͼ�����Ϣ�� where �������� ='" + strXzq + "'" + "And " + " ���='" +
               strYear + "'" + "And " + " ������='" + strScale + "'" + "And " + " ר������='" + strSubType + "'";
            string strBusinesCode = dDbFun.GetInfoFromMdbByExp(strCon, strExp);

            //ͼ������ǰ׺
        //    string strPrefix = strBusinesCode + strYear + strSubType + strXzq + strScale;//wgf 20110411


            //��ȡӰ���
            strExp = "select Ӱ��� from ��ͼ�����Ϣ�� where �������� ='" + strXzq + "'" + "And " + " ���='" +
             strYear + "'" + "And " + " ������='" + strScale + "'" + "And " + " ר������='" + strSubType + "'";
            string strGridLib = dDbFun.GetInfoFromMdbByExp(strCon, strExp);

            //������  GisMap ���ڵ� Group ר��ڵ� Layer Ҷ�ӽڵ�
            MapDocTree.Nodes.Clear();

            //���ڵ�
            m_tparent = new TreeNode();
            m_tparent.Text = "��ͼĿ¼";
            m_tparent.Checked = true;
            MapDocTree.Nodes.Add(m_tparent);
            MapDocTree.ExpandAll();
            m_tparent.ImageIndex = 18;
            m_tparent.SelectedImageIndex = 18;

            TreeNode tMapNode;
            TreeNode tNewNode;
            string strTblName = "";
            string strRootName = "";
            string strSearchRoot = "//GisMap";
            XmlNode xmlNodeRoot = xmldoc.SelectSingleNode(strSearchRoot);
            XmlElement xmlElentRoot = (XmlElement)xmlNodeRoot;
            strRootName = xmlElentRoot.GetAttribute("sItemName");
            xmlElentRoot.SetAttribute("sYear", strYear);
            xmlElentRoot.SetAttribute("sSubject", strSubType);
            xmlElentRoot.SetAttribute("sXzqCode", strXzq);
            xmlElentRoot.SetAttribute("sScale", strScale);

            tMapNode = new TreeNode();
            tMapNode.Text = strRootName + "_" + strSuffixx;
            tMapNode.Name = strSubType;
            tMapNode.Checked = true;
            m_tparent.Nodes.Add(tMapNode);
            m_tparent.ExpandAll();
            tMapNode.ImageIndex = 9;
            tMapNode.SelectedImageIndex = 9;

            //������
            //������ڵ�
            IGroupLayer pGroupFLayer = new GroupLayer();
            pGroupFLayer.Name = tMapNode.Text;
            IMapLayers pmaplayers = axMapControl.Map as IMapLayers;
            pmaplayers.InsertLayer(pGroupFLayer, false, pmaplayers.LayerCount);



            //����Ӱ��������ݾ����Ӱ��ڵ�
            if (strGridLib != "")
            {
                //����xml�ļ� ����ڵ�

                //XmlElement xmlElentRoot = (XmlElement)xmlNodeRoot;
                XmlElement xmlElemGroup = xmldoc.CreateElement("SubGroup");
                string strGroupName = "��Ӱ�����ݡ�";
                xmlElemGroup.SetAttribute("sItemName", strGroupName);
                xmlElemGroup.SetAttribute("sType", "GROUP");
                

                XmlElement xmlElement=xmldoc.CreateElement("Layer");
                xmlElement.SetAttribute("sDemo", "Ӱ������");
                string name = strGridLib.Substring(strGridLib.LastIndexOf("\\") + 1);
                xmlElement.SetAttribute("sItemName", "Ӱ������");
                //xmlElement.SetAttribute("sFile",name);
                xmlElement.SetAttribute("sDataSource", strGridLib);
                xmlElemGroup.AppendChild(xmlElement);

                //string strSearchBuffer = "//SubGroup";
                //XmlNode xmlNdGroupFirst = xmldoc.SelectSingleNode(strSearchBuffer);
                //if (xmlNdGroupFirst != null)
                //{
                //    xmlElentRoot.InsertBefore(xmlElemGroup, xmlNdGroupFirst);

                //}
                //else
                //{
                //    xmlElentRoot.AppendChild(xmlElemGroup);
                //}
                xmlElentRoot.AppendChild(xmlElemGroup);
                // xmldoc.Save(strModPath);
            }


            xmldoc.Save(strModPath);

            //������ӵ�һ���ӽڵ� SubGroup
            string strSearch = "//SubGroup";
            XmlNodeList xmlNdList = xmldoc.SelectNodes(strSearch);
            foreach (XmlNode xmlChild in xmlNdList)
            {
                strTblName = "";
                XmlElement xmlElent = (XmlElement)xmlChild;
                strTblName = xmlElent.GetAttribute("sItemName");

                tNewNode = new TreeNode();
                tNewNode.Text = strTblName;
                tNewNode.Name = xmlElent.GetAttribute("sDataSource");

                tNewNode.Checked = true;
                tMapNode.Nodes.Add(tNewNode);
                tMapNode.ExpandAll();
                tNewNode.ImageIndex =20;
                tNewNode.SelectedImageIndex = 20;

                //��������ӽڵ�
                AddLeafItem(pGroupFLayer,strSubType, tNewNode, xmlChild, strLayerGroup, strYear, strXzq, strScale);

            }


            //ɾ������ֵΪ�յĽڵ�
            DeleNullElementForXml(xmldoc);
            xmldoc.Save(strModPath);

            //��¼��־
            LogFile log = new LogFile(tipRichBox, m_strLogFilePath);
            string strLog = tMapNode.Text + "�������";
            if (log != null)
            {
                log.Writelog(strLog);
            }
            vProgress.SetProgress(strLog);
        }


        public void DeleNullElementForXml(XmlDocument xmldoc)
        {
            if (xmldoc != null)
            {
                string strSearch = "//Layer[@sFile='']";
                XmlNodeList xmlNdList = xmldoc.SelectNodes(strSearch);
                foreach (XmlNode xmlChild in xmlNdList)
                {
                    xmlChild.ParentNode.RemoveChild(xmlChild);
                }
            }
        }

        //ֱ�Ӵ򿪱���ĵ�ͼ�ĵ��ļ���ʼ����ͼ�ĵ���
        public void InitMapDocTreefromXml(string strModPath, SysCommon.CProgress vProgress)
        {
            //�ж��ļ��Ƿ����
        /*    XmlDocument xmldoc = new XmlDocument();
            if (!File.Exists(strModPath))
                return;
            xmldoc.Load(strModPath);

            //������  GisMap ���ڵ� Group ר��ڵ� Layer Ҷ�ӽڵ�
            MapDocTree.Nodes.Clear();

            //���ڵ�
            m_tparent = new TreeNode();
            m_tparent.Text = "��ͼĿ¼";

            m_tparent.Checked = true;
            MapDocTree.Nodes.Add(m_tparent);
            MapDocTree.ExpandAll();
            m_tparent.ImageIndex = 18;
            m_tparent.SelectedImageIndex = 18;

          

            TreeNode tMapNode;
            TreeNode tNewNode;
            string strTblName = "";
            string strMapName = "";
            string strYear = "";
            string strSubType = "";
            string strXzq = "";
            string strScale = "";

            string strSuffixx = "";
            string strSearchRoot = "//GisDoc";
            XmlElement xmlElent;
            XmlNode xmlNodeRoot = xmldoc.SelectSingleNode(strSearchRoot);

            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            string mypath = dIndex.GetDbInfo();
            string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
            string strExp = "";
            GeoDataCenterDbFun dDbFun = new GeoDataCenterDbFun();
            string strCodeUnitName = "";
            string strScaledescribe = "";


            string strSearch = "//GisMap";
            XmlNodeList xmlNdListMap = xmlNodeRoot.SelectNodes(strSearch);
            foreach (XmlNode xmlChildMap in xmlNdListMap)
            {
                xmlElent = (XmlElement)xmlChildMap;
                strMapName = xmlElent.GetAttribute("sItemName");
                strYear = xmlElent.GetAttribute("sYear");
                strSubType = xmlElent.GetAttribute("sSubject");
                strXzq = xmlElent.GetAttribute("sXzqCode");
                strScale = xmlElent.GetAttribute("sScale");


                strExp = "select �������� from ���ݵ�Ԫ�� where �������� ='" + strXzq + "'";
                strCodeUnitName = dDbFun.GetInfoFromMdbByExp(strCon, strExp);
                strExp = "select ���� from �����ߴ���� where ���� ='" + strScale + "'";
                strScaledescribe = dDbFun.GetInfoFromMdbByExp(strCon, strExp);


                strSuffixx = strMapName + "_" + strCodeUnitName + strYear + "��" + "��" + strScaledescribe + "��";


                tMapNode = new TreeNode();
                tMapNode.Text = strSuffixx;
                tMapNode.Name = strSubType;
                tMapNode.Checked = true;


                //������ڵ�
                IGroupLayer pGroupFLayer = new GroupLayer();
                pGroupFLayer.Name = tMapNode.Text;
                IMapLayers pmaplayers = axMapControl.Map as IMapLayers;
                pmaplayers.InsertLayer(pGroupFLayer, false, pmaplayers.LayerCount);

                m_tparent.Nodes.Add(tMapNode);
                m_tparent.ExpandAll();
                tMapNode.ImageIndex = 9;
                tMapNode.SelectedImageIndex =9;

                tTextNode = new TreeNode();
                tTextNode.Text = strSuffixx;
                m_tTextparent.Nodes.Add(tTextNode);
                m_tTextparent.ExpandAll();
                tTextNode.ImageIndex = 9;
                tTextNode.SelectedImageIndex =9;

                //������ӵ�һ���ӽڵ� SubGroup
                //strSearch = "//SubGroup";  //�����ĵ��ڵ�  sGroupType = DOC
                XmlNodeList xmlNdList = xmlChildMap.ChildNodes;
                foreach (XmlNode xmlChild in xmlNdList)
                {
                    strTblName = "";
                    xmlElent = (XmlElement)xmlChild;
                    strTblName = xmlElent.GetAttribute("sItemName");
                    string strGroupType = xmlElent.GetAttribute("sGroupType");
                    if (strGroupType.Equals("DOC"))  //�ĵ�
                    {
                        vProgress.SetProgress("�����ĵ�" + strTblName);
                        //��������ӽڵ�
                        AddLeafItemfromXml(pGroupFLayer,strSubType, tTextNode, xmlChild, strGroupType);
                    }
                    else
                    {
                        tNewNode = new TreeNode();
                        tNewNode.Text = strTblName;

                        tNewNode.Checked = true;
                        tMapNode.Nodes.Add(tNewNode);
                        tMapNode.ExpandAll();
                        tNewNode.ImageIndex = 20;
                        tNewNode.SelectedImageIndex = 20;

                        //��������ӽڵ�
                        AddLeafItemfromXml(pGroupFLayer,strSubType, tNewNode, xmlChild, strGroupType);

                        vProgress.SetProgress(tNewNode.Text + "�������");
                    }

                }

            }
            TextDoctree.Refresh();
            xmldoc.Save(strModPath);*/
        }
        public IWorkspace GetWorkspace(string server, string service, string dataBase, string user, string password, string version, enumWSType wsType, out Exception eError)
        {
            eError = null;
            bool result = false;

            if (ModFrameData.v_AppGisUpdate.gisDataSet == null)
            {
                ModFrameData.v_AppGisUpdate.gisDataSet = new SysCommon.Gis.SysGisDataSet();
            }
            try
            {
                switch (wsType)
                {
                    case enumWSType.SDE:
                        result = ModFrameData.v_AppGisUpdate.gisDataSet.SetWorkspace(server, service, dataBase, user, password, version, out eError);
                        break;
                    case enumWSType.PDB:
                    case enumWSType.GDB:
                        result = ModFrameData.v_AppGisUpdate.gisDataSet.SetWorkspace(server, wsType, out eError);
                        break;
                    default:
                        break;
                }
                if (result)
                {
                    return ModFrameData.v_AppGisUpdate.gisDataSet.WorkSpace;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                eError = ex;
                return null;
            }
        }
        //ֱ�Ӷ�ȡ��ǰ����õ�xml�ļ�
        //strInLibLayer  �Ѿ�����ͼ�����
        //strPrefix      ͼ������ǰ׺  ����������������ʽһ��
        public void AddLeafItemfromXml(IGroupLayer pGroupFLayer,string strSubType, TreeNode treeNode, XmlNode xmlNode, string strGroupType)
        {
            if (treeNode != null && xmlNode != null)
            {
                TreeNode tNewNode;
                if (strGroupType.Equals("DOC"))
                {
                    string strLayerDescribed = ""; //ͼ������ ����ͼ�ߵ�
                    string strNewFile = "";
                    XmlNodeList xmlNdList;
                    xmlNdList = xmlNode.ChildNodes;
                    foreach (XmlNode xmlChild in xmlNdList)
                    {
                        strLayerDescribed = "";
                        XmlElement xmlElent = (XmlElement)xmlChild;
                        strLayerDescribed = xmlElent.GetAttribute("sDemo");
                        strNewFile = xmlElent.GetAttribute("sItemName");
                        //����ͼ������
                        if (!strNewFile.Equals(""))
                        {
                            tNewNode = new TreeNode();
                            tNewNode.Text = strLayerDescribed;
                            tNewNode.Name = strNewFile;
                            treeNode.Nodes.Add(tNewNode);
                            tNewNode.ImageIndex = 19;
                            tNewNode.SelectedImageIndex =19;
                        }
                    }
                }
                else
                {
                    string strLayerDescribed = ""; //ͼ������ ����ͼ�ߵ�
                    string strNewFile = "";
                    XmlNodeList xmlNdList;
                    xmlNdList = xmlNode.ChildNodes;
                    foreach (XmlNode xmlChild in xmlNdList)
                    {
                        strLayerDescribed = "";
                        XmlElement xmlElent = (XmlElement)xmlChild;
                        strLayerDescribed = xmlElent.GetAttribute("sDemo");
                        strNewFile = xmlElent.GetAttribute("sItemName");
                        string Datasource = xmlElent.GetAttribute("sDataSource");
                        if (xmlElent.GetAttribute("sDiaphaneity") != "")
                            m_transpy = short.Parse(xmlElent.GetAttribute("sDiaphaneity"));//͸����
                        string strcale = xmlElent.GetAttribute("sDispScale");//������ 1:10000
                        if (strcale != "")
                        {
                            strcale = strcale.Substring(strcale.LastIndexOf(":") + 1);//�õ� 10000
                            m_scale = double.Parse(strcale);//������ ת��Ϊdouble
                        }

                        //����ͼ������
                        if (!strNewFile.Equals(""))
                        {                            
                            tNewNode = new TreeNode();
                            tNewNode.Checked = true;
                            tNewNode.Text = strLayerDescribed;
                            tNewNode.Name = strNewFile;

                            treeNode.Nodes.Add(tNewNode);
                            tNewNode.ImageIndex = 19;
                            tNewNode.SelectedImageIndex = 19;

                            //����ͼ�����ƻ�ȡpFeatureWorkspace  wgf20110412
                            string strSubject = "";
                            if (treeNode.Parent!= null)
                            {
                                strSubject = treeNode.Parent.Name;
                            }
                            if (strNewFile.Trim() != "Ӱ������")
                            {
                                 IFeatureWorkspace pFeatureWorkspace ;
                                //string strLayerName = GetLayerNameFormNodeName(strNewFile, strSubject); //changed by chulili 2011-04-15
                                string strLayerName = GetLayersFileFormNode(tNewNode);
                                if (m_strnew.Trim() != "")//�����ӵ�
                                {
                                    pFeatureWorkspace = GetWorkspaceByFileName2(strLayerName);
                                    strLayerName = strLayerName.Substring(strLayerName.LastIndexOf("\\") + 1);
                                    strLayerName = System.IO.Path.GetFileNameWithoutExtension(strLayerName);
                                }
                                else
                                {
                                    //����m_strnew== "" ά��ԭ������
                                    pFeatureWorkspace = GetWorkspaceByFileName(strLayerName);
                                }
                                if (pFeatureWorkspace != null)
                                    LoadMapData(pGroupFLayer,strSubType, pFeatureWorkspace, strLayerName, strLayerDescribed);
                                else
                                {
                                    tNewNode.ImageIndex = 14;
                                    tNewNode.SelectedImageIndex = 14;
                                }
                            }
                            else
                            {
                                try
                                {
                                    if (Datasource.Trim() == "")
                                    {
                                        tNewNode.ImageIndex = 14;
                                        tNewNode.SelectedImageIndex = 14;                                        
                                    }
                                    LoadMapRasterData(pGroupFLayer,Datasource);//����Ӱ���
                                    tNewNode.ImageIndex = 19;
                                    tNewNode.SelectedImageIndex = 19;
                                }
                                catch
                                {
                                    tNewNode.ImageIndex = 14;
                                    tNewNode.SelectedImageIndex = 14;
                                }
                            }
                        }
                    }
                }
                treeNode.ExpandAll();
            }
        }


        //added  by  xs 2011.04.18
        public IFeatureWorkspace GetWorkspaceByFileName2(string filename)
        {
            if (filename.Equals(""))
                return null;
            if (filename.Length < 1)
                return null;
            IFeatureWorkspace pFeatureWorkspace;
            int Index = filename.LastIndexOf("\\");
            string filePath = filename.Substring(0, Index);
            try
            {//��shp�ļ�
                if (m_strnew == "shp")
                {
                    string fileName = System.IO.Path.GetFileNameWithoutExtension(filename);
                    //�򿪹����ռ䲢���shp�ļ�
                    IWorkspaceFactory pWorkspaceFactory = new ShapefileWorkspaceFactoryClass();
                    //ע��˴���·���ǲ��ܴ��ļ�����
                    pFeatureWorkspace = (IFeatureWorkspace)pWorkspaceFactory.OpenFromFile(filePath, 0);
                }
                else
                {
                    //��personGeodatabase,�����ͼ��
                    IWorkspaceFactory pAccessWorkspaceFactory = new AccessWorkspaceFactoryClass();
                    //�򿪹����ռ䲢�������ݼ�
                    IWorkspace pWorkspace = pAccessWorkspaceFactory.OpenFromFile(filePath, 0);
                    pFeatureWorkspace = pWorkspace as IFeatureWorkspace;
                }
                return pFeatureWorkspace;
            }
            catch
            {
                return null;
            }
        }
        public IFeatureWorkspace GetWorkspaceByFileName(string filename)
        {
            if (filename.Equals(""))
                return null;
            if (filename.Length < 1)
                return null;
            string[] array = new string[6];//�������ݳ�������ʽ
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            string mypath = dIndex.GetDbInfo();
            string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
            string strExp = "select �ֶ����� from ͼ�����������";
            GeoDataCenterDbFun db = new GeoDataCenterDbFun();
            string strname = db.GetInfoFromMdbByExp(strCon, strExp);
            string[] arrName = strname.Split('+');//�����ֶ�����
            for (int i = 0; i < arrName.Length; i++)
            {
                switch (arrName[i])
                {
                    case "ҵ��������":
                        array[0] = filename.Substring(0, 2);//ҵ��������
                        filename = filename.Remove(0, 2);
                        break;
                    case "���":
                        array[1] = filename.Substring(0, 4);//���
                        filename = filename.Remove(0, 4);
                        break;
                    case "ҵ��С�����":
                        array[2] = filename.Substring(0, 2);//ҵ��С�����
                        filename = filename.Remove(0, 2);
                        break;
                    case "��������":
                        array[3] = filename.Substring(0, 6);//��������
                        filename = filename.Remove(0, 6);
                        break;
                    case "������":
                        array[4] = filename.Substring(0, 1);//������
                        filename = filename.Remove(0, 1);
                        break;
                }
            }
            array[5] = filename;//ͼ�����

            IFeatureWorkspace pFeatureWorkspace = GetWorkspaceByLayerInfo(array[0], array[1], array[2], array[3], array[4], array[5]);
           return pFeatureWorkspace;
        }

        //���Ҷ�ӽڵ�
        //strInLibLayer  �Ѿ�����ͼ�����
        public void AddLeafItem(IGroupLayer pGroupFLayer,string strSubType, TreeNode treeNode, XmlNode xmlNode, string strInLibLayer, string strYear, string strXzq, string strScale)
        {
            if (treeNode != null && xmlNode != null && pGroupFLayer != null)
            {
                //wgf 20110412
              //?  IFeatureWorkspace pFeatureWorkspace = ModFrameData.v_AppGisUpdate.gisDataSet.WorkSpace as IFeatureWorkspace;
                TreeNode tNewNode;
                string strLayerDescribed = ""; //ͼ������ ����ͼ�ߵ�
                string strFileName = "";
                string strNewFile = "";
                string strItemName="";
                string strBigClass = "";
                string strSubClass = "";
                XmlNodeList xmlNdList;
                xmlNdList = xmlNode.ChildNodes;
                foreach (XmlNode xmlChild in xmlNdList)
                {
                    strLayerDescribed = "";
                    XmlElement xmlElent = (XmlElement)xmlChild;
                    strLayerDescribed = xmlElent.GetAttribute("sDemo");//��������
                    strItemName = xmlElent.GetAttribute("sItemName");//����
                    strBigClass = xmlElent.GetAttribute("sBigClass");//ҵ�����
                    strSubClass = xmlElent.GetAttribute("sSubClass");//ҵ��С��
                    strFileName = xmlElent.GetAttribute("sFile");//ͼ�����
                    string strLayerType = xmlElent.GetAttribute("sLayerType");
                    if (xmlElent.GetAttribute("sDiaphaneity") != "")
                        m_transpy = short.Parse(xmlElent.GetAttribute("sDiaphaneity"));//͸����
                    string strcale = xmlElent.GetAttribute("sDispScale");//������ 1:10000
                    if (strcale != "")
                    {
                        strcale = strcale.Substring(strcale.LastIndexOf(":") + 1);//�õ� 10000
                        m_scale = double.Parse(strcale);//������ ת��Ϊdouble
                    }
                    string datasource = xmlElent.GetAttribute("sDataSource");
                    if (strFileName == "")
                    {
                        if (datasource != "")
                        {
                            strNewFile = datasource.Substring(datasource.LastIndexOf("\\") + 1);
                            tNewNode = new TreeNode();
                            tNewNode.Checked = true;
                            tNewNode.Text = strLayerDescribed;
                            tNewNode.Name = strItemName;

                            treeNode.Nodes.Add(tNewNode);
                            try
                            {
                                LoadMapRasterData(pGroupFLayer,datasource);//����Ӱ���
                                tNewNode.ImageIndex = 19;
                                tNewNode.SelectedImageIndex = 19;
                            }
                            catch 
                            {
                                tNewNode.ImageIndex = 14;
                                tNewNode.SelectedImageIndex = 14;
                            }                           
                        }
                    }
                    else
                    {
                        if (strInLibLayer.Contains(strFileName))
                        {
                            //�޸�sfile����

                            strNewFile = GetLayerName(strBigClass, strYear, strSubClass, strXzq, strScale) + strFileName;
                            xmlElent.SetAttribute("sFile", strNewFile);

                            tNewNode = new TreeNode();
                            tNewNode.Checked = true;
                            tNewNode.Text = strLayerDescribed;
                            tNewNode.Name = strItemName;

                            treeNode.Nodes.Add(tNewNode);
                            tNewNode.ImageIndex = 19;
                            tNewNode.SelectedImageIndex = 19;

                            //����ͼ�����ƻ�ȡpFeatureWorkspace  wgf20110412
                            IFeatureWorkspace pFeatureWorkspace = GetWorkspaceByLayerInfo(strBigClass, strYear, strSubClass, strXzq, strScale, strFileName);
                            if (pFeatureWorkspace!=null)//���ǿյĿռ�
                            {//����ͼ������
                                LoadMapData(pGroupFLayer,strSubType, pFeatureWorkspace, strNewFile, strLayerDescribed);
                            }
                            else
                            {
                                tNewNode.ImageIndex = 14;//��ͼ��
                                tNewNode.SelectedImageIndex = 14;//��ͼ��                                
                            }
                        }
                        else
                        {
                            xmlElent.SetAttribute("sFile", "");
                        }
                    }
                }
                treeNode.ExpandAll();
            }
        }

        /// <summary>
        /// ����ÿ��ͼ���Ӧ��������Ϣ ��ȡLayer  ���ר���ÿ��ͼ�㶼�����ڲ�ͬ�����ݿ���
        /// wgf 2011-04-12
        /// strBigClass, ҵ��������
        /// strYear,     ���
        /// strSubClass, ҵ��С�����
        /// strXzq,      ���ݵ�Ԫ���루�ؼ����룩
        /// strScale,    �����ߴ���
        /// strFileName  ͼ������ DLTB
        /// </summary>
        /// <returns></returns>
        public IFeatureWorkspace GetWorkspaceByLayerInfo(string strBigClass, string strYear, string strSubClass, string strXzq, string strScale, string strFileName)
        {
            IFeatureWorkspace pFeatureWorkspace = null;

            //���ݱ��ʽ�� ���ݱ�����л�ȡ����Դ 
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            string mypath = dIndex.GetDbInfo();
            string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
            string strExp = string.Format("select ����Դ���� from ���ݱ���� where ҵ��������='{0}' and ���='{1}' and ҵ��С�����='{2}'and ��������='{3}' and ������='{4}' and ͼ�����='{5}'",
              strBigClass, strYear, strSubClass, strXzq, strScale, strFileName);
     
            GeoDataCenterDbFun db = new GeoDataCenterDbFun();
            string strDataSourceName = db.GetInfoFromMdbByExp(strCon, strExp);
            if (strDataSourceName.Trim() != "")
            {

                //changed by xisheng 2011.04.29
                //    //����������Դ���л�ȡ��Ӧ�����ݿ���Ϣ
                //    strExp = string.Format("select ���ݿ� from ��������Դ�� where ����Դ����='{0}'", strDataSourceName);
                //    string strDataLibName = db.GetInfoFromMdbByExp(strCon, strExp);
                //    if (strDataLibName.Trim() != "")
                //    {
                //        if (!Directory.Exists(strDataLibName) && !File.Exists(strDataLibName))//·��������
                //        {
                //            pFeatureWorkspace = null;
                //        }
                //        else
                //        {//����workspace
                //            IWorkspaceFactory pWorkspaceFactory = new FileGDBWorkspaceFactoryClass();
                //            IWorkspace pWorkspace = pWorkspaceFactory.OpenFromFile(strDataLibName, 0);
                //            pFeatureWorkspace = pWorkspace as IFeatureWorkspace;
                //        }
                //    }

                IWorkspace pWorkspace = GetWorkspace(strDataSourceName);//�ж���GDB����SDE�����ݿ⣬�õ���Ӧ�Ĺ����ռ�
                pFeatureWorkspace = pWorkspace as IFeatureWorkspace;
            }
            return pFeatureWorkspace;

        }
        /// <summary>
        /// �õ����ݿ�ռ� Added by xisheng 2011.04.28
        /// </summary>
        /// <param name="str">����Դ����</param>
        /// <returns>�����ռ�</returns>
        private IWorkspace GetWorkspace(string str)
        {
            try
            {
                IWorkspace pws = null;
                GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
                string mypath = dIndex.GetDbInfo();
                string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
                string strExp = "select * from ��������Դ�� where ����Դ����='" + str + "'";
                GeoDataCenterDbFun db = new GeoDataCenterDbFun();
                DataTable dt = db.GetDataTableFromMdb(strCon, strExp);
                string type = dt.Rows[0]["����Դ����"].ToString();
                if (type.Trim() == "GDB")
                {
                    IWorkspaceFactory pWorkspaceFactory;
                    pWorkspaceFactory = new FileGDBWorkspaceFactoryClass();
                    pws = pWorkspaceFactory.OpenFromFile(dt.Rows[0]["���ݿ�"].ToString(), 0);
                }
                else if (type.Trim() == "SDE")
                {
                    IWorkspaceFactory pWorkspaceFactory;
                    pWorkspaceFactory = new SdeWorkspaceFactoryClass();

                    //PropertySet
                    IPropertySet pPropertySet;
                    pPropertySet = new PropertySetClass();
                    pPropertySet.SetProperty("Server", dt.Rows[0]["������"].ToString());
                    pPropertySet.SetProperty("Database", dt.Rows[0]["���ݿ�"].ToString());
                    pPropertySet.SetProperty("Instance", "5151");//"port:" + txtService.Text
                    pPropertySet.SetProperty("user", dt.Rows[0]["�û�"].ToString());
                    pPropertySet.SetProperty("password", dt.Rows[0]["����"].ToString());
                    pPropertySet.SetProperty("version", "sde.DEFAULT");
                    pws = pWorkspaceFactory.Open(pPropertySet, 0);

                }
                return pws;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// ��֯ǰ׺��added by xs 20110413
        /// </summary>
        /// <param name="str1">ҵ�����</param>
        /// <param name="str2">���</param>
        /// <param name="str3">ҵ��С��</param>
        /// <param name="str4">��������</param>
        /// <param name="str5">������</param>
        /// <returns></returns>
        public string GetLayerName(string str1, string str2, string str3, string str4, string str5)
        {
            string layername="";
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            string mypath = dIndex.GetDbInfo();
            string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
            string strExp = "select �ֶ����� from ͼ�����������";
            GeoDataCenterDbFun db = new GeoDataCenterDbFun();
            string strname = db.GetInfoFromMdbByExp(strCon, strExp);
            string[] arrName = strname.Split('+');//�����ֶ�����
            for (int i = 0; i < arrName.Length; i++)
            {
                switch (arrName[i])
                {
                    case "ҵ��������":
                        layername += str1;
                        break;
                    case "���":
                        layername += str2;
                        break;
                    case "ҵ��С�����":
                        layername += str3;
                        break;
                    case "��������":
                        layername += str4;
                        break;
                    case "������":
                        layername += str5;
                        break;
                }
            }
            return layername;
        }
        /// <summary>
        /// �����ڵ��name�õ�XML��sFile���ͼ������
        /// </summary>
        /// <param name="strItemName">���ڵ�Name</param>
        ///  strSubject   ���ڵ�name ��Ӧxml�ļ��е�GisMap�е�sSubject
        /// <returns></returns>
        public string GetLayerNameFormNodeName(string strItemName, string strSubject)
        {
            if (strItemName.Equals(""))
                return "";
            string layername = "";
            string strCurFile = Application.StartupPath + "\\..\\Temp\\CurPrj.xml";
            if (File.Exists(strCurFile))
            {
                string strSearch = "//GisMap[@sSubject='" + strSubject + "']" + "//Layer[@sItemName='" + strItemName + "']";
                XmlDocument xmlCurdoc = new XmlDocument();
                xmlCurdoc.Load(strCurFile);
                XmlNode xmlnode = xmlCurdoc.SelectSingleNode(strSearch);
                if (xmlnode != null)
                {
                    layername = ((XmlElement)xmlnode).GetAttribute("sFile");
                }

            }
            return layername;
        }
        //added by chulili 2011-4-14
        //�����ڵ�õ�XML��sFile���ͼ������
        //������������ڵ�  ���������ͼ���Ӧmap���ļ���
        public string GetLayersFileFormNode(TreeNode node)
        {
            string strsFile = "";
            m_strnew = "";
            if (node == null)
                return strsFile;
            if (node.Level < 3)
                return strsFile;
            //��ȡ��ǰ�����ļ�
            string strWorkFile = Application.StartupPath + "\\..\\Temp\\CurPrj.xml";
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(strWorkFile);

            TreeNode TopicNode = node.Parent.Parent;    //��ȡ���ڵ�ר��ڵ�
            string strSuffixx = TopicNode.Text;
            //���ݵ�ǰר������ƹ��ɣ���ö�Ӧxml�ڵ�ĸ�����
            string strMapName = strSuffixx.Substring(0, strSuffixx.IndexOf("_"));
            string strYear = strSuffixx.Substring(strSuffixx.LastIndexOf("��") - 4, 4);
            string strScaledescribe = strSuffixx.Substring(strSuffixx.LastIndexOf("��") + 1, strSuffixx.LastIndexOf("��") - strSuffixx.LastIndexOf("��") - 1);
            string strCodeUnitName = strSuffixx.Substring(strSuffixx.IndexOf("_") + 1, strSuffixx.LastIndexOf("��") - 4 - strSuffixx.IndexOf("_") - 1);
            //��ȡ��ǰ��ҵ���mdb
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            string mypath = dIndex.GetDbInfo();
            string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���

            GeoDataCenterDbFun dDbFun = new GeoDataCenterDbFun();
            //�����������ƻ�ȡ��������
            string strExp = "select �������� from ���ݵ�Ԫ�� where �������� ='" + strCodeUnitName + "' and ���ݵ�Ԫ����='3'";
            string strCodeUnit = dDbFun.GetInfoFromMdbByExp(strCon, strExp);
            //���ݱ�����������ȡ�����ߴ���
            strExp = "select ���� from �����ߴ���� where ����='" + strScaledescribe + "'";
            string strScalecode = dDbFun.GetInfoFromMdbByExp(strCon, strExp);
            //strSuffixx = strMapName + "_" + strCodeUnitName + strYear + "��" + "��" + strScaledescribe + "��";

            string strSearchExp = "//GisMap[@sItemName='" + strMapName + "' and @sYear = '" + strYear + "'"
            + "and @sXzqCode ='" + strCodeUnit + "'" + "and @sScale='" + strScalecode + "']";
            //��ȡ�仯ͼ��ͼ��
            XmlNode topicnode = xmldoc.SelectSingleNode(strSearchExp);
            strSearchExp = "//Layer [@sItemName='" + node.Name + "']";
            //��ȡͼ���Ӧ��xmlnode
            XmlNode pXmlnode = topicnode.SelectSingleNode(strSearchExp);
            if (pXmlnode == null)
                return strsFile;
            if (!pXmlnode.NodeType.ToString().Equals("Element"))
                return strsFile;
            XmlElement pEle = pXmlnode as XmlElement;
            //��ȡxmlnode��sFile����
            strsFile = pEle.GetAttribute("sFile");
            m_strnew = pEle.GetAttribute("sNewLoad");
            return strsFile;
        }
        /// <summary>
        /// �����ڵ��name�õ�XML��sDatasource���
        /// </summary>
        /// <param name="strItemName">���ڵ�Name</param>
        ///  strSubject   ���ڵ�name ��Ӧxml�ļ��е�GisMap�е�sSubject
        /// <returns></returns>
        public string GetRasterLayerFormNodeName(string strItemName, string strSubject)
        {
            if (strItemName.Equals(""))
                return "";
            string layername = "";
            string strCurFile = Application.StartupPath + "\\..\\Temp\\CurPrj.xml";
            if (File.Exists(strCurFile))
            {
                if (strItemName.Trim() == "Ӱ������")
                {
                    string strSearch = "//GisMap[@sSubject='" + strSubject + "']" + "//Layer[@sItemName='" + strItemName + "']";
                    XmlDocument xmlCurdoc = new XmlDocument();
                    xmlCurdoc.Load(strCurFile);
                    XmlNode xmlnode = xmlCurdoc.SelectSingleNode(strSearch);
                    if (xmlnode != null)
                    {
                        layername = ((XmlElement)xmlnode).GetAttribute("sDataSource");
                        if (layername != "")
                        {
                            layername=layername.Substring(layername.LastIndexOf("\\")+1);
                        }
                    }
                }

            }
            return layername;
        }

        /// <summary>
        /// ���ݵ�ǰ�ڵ�ݹ�ѡ�и��ڵ� added by xs 2011.04.18
        /// </summary>
        /// <param name="node">��ǰ�ڵ�</param>
        private void ChangeParentCheck(TreeNode node)
        {
            flag2 = false;
            if (node.Parent != null)
            {
                if (node.Checked)
                {
                    node.Parent.Checked = true;
                    ChangeParentCheck(node.Parent);
                }
            }
            flag2 = true;

        }

        /// <summary>
        /// ���ݵ�ǰ�ڵ�ݹ�ѡ�и��ڵ� added by xs 2011.04.19
        /// </summary>
        /// <param name="node">��ǰ�ڵ�</param>
        private void ChangeChildCheck(TreeNode node)
        {
            
            foreach (TreeNode item in node.Nodes)
            {
                flag = false;
                if (item.Level.Equals(3)) flag = true;
                item.Checked = node.Checked;
                ChangeChildCheck(item); 
            }
           

        }
        //��ͼ�ĵ��� ���ڵ�ѡ��״̬�ı�� �ӽڵ�ҲҪ��֮�����ı�
        private void MapDocTree_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (flag2&&flag)
            {
                ChangeParentCheck(e.Node);
                TreeNode node = e.Node;
                //�����Ҷ�ӽڵ�
                if (node.Level.Equals(3)&&flag)
                {
                    ModifyLayerDispStat(node, node.Checked);
                }
                else
                {
                   
                    ChangeChildCheck(node);
                    //����Ǹ��׽ڵ�
                    
                    foreach (TreeNode item in node.Nodes)
                    {
                       
                        if (item.Level.Equals(3))
                        {
                            //pFeatureLayer.Visible = node.Checked;
                            ModifyLayerDispStat(item, item.Checked);
                        }
                    }
   

                }
                axMapControl.ActiveView.Refresh(); //ˢ����ͼ����ʾ����ͼ���Ľ�� 
            }
        }


        //���ݽڵ�ѡ��״̬����ͼ���Ƿ���ʾ
        private void ModifyLayerDispStat(TreeNode node, bool bStat)
        {
            string strSubject = "";
            string strRootMapText = "";

            if (node.Parent != null)
            {
                if (node.Parent.Parent != null)
                {
                    strSubject = node.Parent.Parent.Name;
                    strRootMapText = node.Parent.Parent.Text;
                }
            }
            //string strLayerName = GetLayerNameFormNodeName(node.Name, strSubject);//changed by chulili 2011-04-15
            string strLayerName = GetLayersFileFormNode(node);
           /* IFeatureWorkspace pFeatureWorkspace = ModFrameData.v_AppGisUpdate.gisDataSet.WorkSpace as IFeatureWorkspace;
            Exception exError = null;
            IFeatureClass pFC = ModFrameData.v_AppGisUpdate.gisDataSet.GetFeatureClass(strLayerName, out exError);*/

            int iLayerCount = axMapControl.Map.LayerCount;
            IFeatureLayer pFeatureLayer;
            IRasterLayer pRasterLayer;
            IMap pMap = axMapControl.Map;
            
                if (strLayerName.Trim() != "")//ʸ��ͼ�����״̬
                {
                    for (int ii = 0; ii < iLayerCount; ii++)
                    
                    {ILayer layer = pMap.get_Layer(ii);
                     if (layer is IGroupLayer && layer.Name == strRootMapText)
                     {
                          ICompositeLayer Comlayer = layer as ICompositeLayer;//��һ��������Ĳ����ת����һ����ϲ㣬ʹ�����Ա���
                          for (int c = 0; c < Comlayer.Count; c++)
                          {
                              pFeatureLayer = Comlayer.get_Layer(c) as IFeatureLayer;
                              if (pFeatureLayer == null)
                                  continue;
                              if (pFeatureLayer.FeatureClass == null)
                                  continue;
                              if (pFeatureLayer.Name == node.Name)
                              {
                                  pFeatureLayer.Visible = bStat;
                                  break;
                              }
                          }
                     }
                    }
                   /* pFeatureLayer = pMap.get_Layer(ii) as IFeatureLayer;
                    if (pFeatureLayer == null)
                        continue;
                    if (pFeatureLayer.FeatureClass.FeatureClassID == pFC.FeatureClassID)
                    {
                        pFeatureLayer.Visible = bStat;
                        break;
                    }*/

                }
                else//Ӱ��ͼ����ʾ״̬
                {
                    strLayerName = GetRasterLayerFormNodeName(node.Name, strSubject);
                    for (int ii = 0; ii < iLayerCount; ii++)
                    {
                        ILayer layer = pMap.get_Layer(ii);
                        if (layer is IGroupLayer && layer.Name == strRootMapText)
                        {
                            ICompositeLayer Comlayer = layer as ICompositeLayer;//��һ��������Ĳ����ת����һ����ϲ㣬ʹ�����Ա���
                            for (int c = 0; c < Comlayer.Count; c++)
                            {
                                pRasterLayer = Comlayer.get_Layer(c) as IRasterLayer;
                                if (pRasterLayer == null)
                                    continue;
                                if (pRasterLayer.Name.Substring(0, pRasterLayer.Name.LastIndexOf("\\")).ToUpper() == strLayerName.ToUpper())
                                {
                                    pRasterLayer.Visible = bStat;
                                }
                            }
                        }
                    }

            }
            axMapControl.ActiveView.Refresh(); //ˢ����ͼ����ʾ����ͼ���Ľ�� 
        }

        //����դ������
        public void LoadMapRasterData(IGroupLayer pGroupFLayer, string strFullPath)
        {
            if (strFullPath.Trim() == "")
                return;
            IWorkspaceFactory pWorkspaceFactory;
            //IRasterWorkspace pRasterWorkspace;
            IRasterWorkspaceEx pRasterWorkspaceEx;
            IWorkspace pWorkspace;

            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            string mypath = dIndex.GetDbInfo();
            string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
            if (strFullPath == "") return;
            int Index = strFullPath.LastIndexOf("\\");
            string fileName = strFullPath.Substring(Index + 1);
            string sourcename = strFullPath.Substring(0, Index);
            //string strExp = "select ���ݿ� from ��������Դ�� where ����Դ����='" + sourcename + "'";
            //GeoDataCenterDbFun db = new GeoDataCenterDbFun();
            //string filePath = db.GetInfoFromMdbByExp(strCon, strExp);
            //if (filePath.Trim() == "")
            //{
            //    filePath = sourcename;
            //}
            //if (!Directory.Exists(filePath)) return;
            //pWorkspaceFactory = new FileGDBWorkspaceFactoryClass();
            //pWorkspace = pWorkspaceFactory.OpenFromFile(filePath, 0);
            pWorkspace = GetWorkspace(sourcename);
            if (pWorkspace == null) return;
            pRasterWorkspaceEx = pWorkspace as IRasterWorkspaceEx;
            IRasterCatalog pCatalog = null;
            IEnumDatasetName pEnumDSName = pWorkspace.get_DatasetNames(esriDatasetType.esriDTRasterCatalog);
            IDatasetName pSDEDsName = pEnumDSName.Next();
            while (pSDEDsName != null)
            {
                string SDEDatasetName = pSDEDsName.Name;
                if (fileName.Trim().CompareTo(SDEDatasetName) != 0)
                {
                    pSDEDsName = pEnumDSName.Next();
                    break;
                }
                IRasterDataset2 pRasterDS = (IRasterDataset2)pRasterWorkspaceEx.OpenRasterDataset(SDEDatasetName);

                //IEnumDataset pEnumDataset = pWorkspace.get_Datasets(esriDatasetType.esriDTRasterCatalog);
                //IDataset pDataset = pEnumDataset.Next();
                //while (pDataset != null)
                //{
                //    if (pDataset.Name == fileName)
                //    {
                //        pCatalog = pDataset as IRasterCatalog;
                //        break;
                //    }
                //    pDataset = pEnumDataset.Next();
                //}
                ////pRasterWorkspaceEx = (IRasterWorkspaceEx)pRasterWorkspace;
                //IFeatureClass featureClass = (IFeatureClass)pCatalog;
                //IRasterCatalogItem rasterCatalogItem = (IRasterCatalogItem)featureClass.GetFeature(1);
                //IRasterDataset pRasterDataset = rasterCatalogItem.RasterDataset;
                ////try
                ////{
                ////    pRasterDataset = (IRasterDataset)pRasterWorkspace.OpenRasterDataset(fileName);
                ////}
                ////catch
                ////{
                ////     pRasterDataset = (IRasterDataset)pRasterWorkspaceEx.OpenRasterCatalog(fileName);
                ////}
                IRasterLayer pRasterLayer = new RasterLayerClass();
                pRasterLayer.CreateFromDataset(pRasterDS);
                IMapLayers pmaplayers = axMapControl.Map as IMapLayers;
                //   ModRender.SetRenderByXML(pLayer, log);//���Ż�   //��������д����sytel�ļ���ͼ
                //  pmaplayers.InsertLayer(pLayer, false, pmaplayers.LayerCount);
                //������
                ICompositeLayer Comlayer = pGroupFLayer as ICompositeLayer;
                pmaplayers.InsertLayerInGroup(pGroupFLayer, pRasterLayer, false, Comlayer.Count);
                //axMapControl.Extent = axMapControl.FullExtent;
                axMapControl.ActiveView.Refresh();
                pSDEDsName = pEnumDSName.Next();



            }
        }
        // ���ص�ͼ����
        //strLayerName  ���ݿ����ļ��Ĵ洢����
        //strLayerDescribed �ļ���������
        public void LoadMapData(IGroupLayer pGroupFLayer,string strSubType, IFeatureWorkspace pFeatureWorkspace, string strLayerName, string strLayerDescribed)
        {

            //IEnumDatasetName enumDatasetName = (pFeatureWorkspace as IWorkspace).get_DatasetNames(esriDatasetType.esriDTFeatureClass); 
            //IDatasetName datasetName = enumDatasetName.Next(); 
            //while (datasetName != null) 
            //{
            //    bool nn = (pFeatureWorkspace as IWorkspace2).get_NameExists(esriDatasetType.esriDTFeatureClass, datasetName.Name);
            //    Console.WriteLine(datasetName.Name); datasetName = enumDatasetName.Next(); }



            if (pFeatureWorkspace != null && pGroupFLayer != null)
            {
                //���ж�strLayerNameͼ���Ƿ���� �����������
                //����ÿ��ͼ���¼�����ݿ���Ϣ ��ȡlayer
                IWorkspace2 pWorkspace2 = pFeatureWorkspace as IWorkspace2;

                //IWorkspace workspace = pFeatureWorkspace as IWorkspace;
                //IEnumDatasetName enumDatasetName = workspace.get_DatasetNames(esriDatasetType.esriDTFeatureClass);
                //List<string> tmpL = new List<string>();
                //IDatasetName datasetName = enumDatasetName.Next(); 
                //while (datasetName != null)
                //{
                //    if (pWorkspace2.get_NameExists(esriDatasetType.esriDTFeatureClass, datasetName.Name))
                //    { tmpL.Add(datasetName.Name); }
                //    datasetName = enumDatasetName.Next();
                //}
                //if ((pFeatureWorkspace as IWorkspace).Type == esriWorkspaceType.esriRemoteDatabaseWorkspace)//���SDE���û���ǰ׺
                //{
                //    string username = (pFeatureWorkspace as IWorkspace).ConnectionProperties.GetProperty("USER").ToString().ToUpper();
                //    strLayerName = username + "." + strLayerName;
                //}
                if (pWorkspace2.get_NameExists(esriDatasetType.esriDTFeatureClass, strLayerName))
                {

                    IFeatureClass pFC = pFeatureWorkspace.OpenFeatureClass(strLayerName);

                    //��������ͼ�㣬����GIS����ͼ��ĸ���
                    IFeatureLayer pFLayer = new FeatureLayerClass();
                    if (m_scale != 0)
                    {
                        pFLayer.MaximumScale = m_scale;
                        pFLayer.MinimumScale = m_scale;
                    }
                    ILayerEffects pLyrEffects = pFLayer as ILayerEffects;
                    if (m_transpy != 0)
                        pLyrEffects.Transparency = m_transpy;//����͸����

                    //����ͼ���� 
                    pFLayer.Name = strLayerDescribed;

                    //����ͼ�㣬�����ղŵ�����ͼ��ǿ��ת��Ϊͼ����� 
                    ILayer pLayer = pFLayer as ILayer;

                    //��ȡģ��·�� 
                    GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
                    string mypath = dIndex.GetDbInfo();
                    string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
                    string strExp = "select ��ͼ�����ļ� from ��׼ר����Ϣ�� where ר������ ='" + strSubType + "'";
                    GeoDataCenterDbFun dDbFun = new GeoDataCenterDbFun();
                    string strModFile = dDbFun.GetInfoFromMdbByExp(strCon, strExp);

                    MapDocumentClass pMapDocument = new MapDocumentClass();
                    if (File.Exists(strModFile))
                    {
                        //����ģ�������ͼ����
                        pMapDocument.Open(strModFile, string.Empty);
                        IMap pMap = pMapDocument.get_Map(0);
                        int iLayCount = pMap.LayerCount;
                        ILayer pLayTemp;
                        for (int jj = 0; jj < iLayCount; jj++)
                        {
                            pLayTemp = pMap.get_Layer(jj);
                            if (pLayTemp == null)
                                continue;
                            if (pLayTemp.Name == pLayer.Name)
                            {
                                //��������Դ
                                pLayer = pLayTemp;
                                pLayer.Visible = true;
                                break;
                            }
                        }
                    }
              

                    //����ͼ���������Ϊ�ղŵ�������,�����Ϳ��Խ��������е����ݼ��ص�����ͼ�������
                    (pLayer as IFeatureLayer).FeatureClass = pFC;
                    IMapLayers pmaplayers = axMapControl.Map as IMapLayers;
               //     ModRender.SetRenderByXML(pLayer);//���Ż�   //��������д����sytel�ļ���ͼ
                    //  pmaplayers.InsertLayer(pLayer, false, pmaplayers.LayerCount);
                    //������
                    ICompositeLayer Comlayer = pGroupFLayer as ICompositeLayer;
                    pmaplayers.InsertLayerInGroup(pGroupFLayer, pLayer, false, Comlayer.Count);
                  //?  pMapDocument.Close();

                    //��¼��־
                    LogFile log = new LogFile(tipRichBox, m_strLogFilePath);
                }
                axMapControl.ActiveView.Refresh(); //ˢ����ͼ����ʾ����ͼ���Ľ��   
            }
          
        }



        //׷�ӵ���
        //��CurPrj.xml�ļ�������µ�ר��
        private void MenuItemAddLoadData_Click(object sender, EventArgs e)
        {
            if (m_tparent == null||MapDocTree.Nodes.Count==0)
            {
                MessageBox.Show("��ǰû�е������ݣ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (m_selectnode == null) return;
            TreeNode tCurNode = m_selectnode;
            //TreeNode tCurNode = DataIndexTree.SelectedNode;

            //��ȡר������
            string strSubType = tCurNode.Tag.ToString();
            //��ʼ�������� added by chulili 
            SysCommon.CProgress vProgress = new SysCommon.CProgress("������");
            vProgress.EnableCancel = false;
            vProgress.ShowDescription = true;
            vProgress.FakeProgress = true;
            vProgress.TopMost = true;
            vProgress.ShowProgress();

            //��ȡģ��·�� 
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            //   string mypath = dIndex.GetDbValue("dbServerPath");
            string mypath = dIndex.GetDbInfo();
            string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
            string strExp = "select �ű��ļ� from ��׼ר����Ϣ�� where ר������ ='" + strSubType + "'";

            //����ר�����ʹӱ�׼ר����Ϣ�� �л�ȡ��Ӧ��ģ���ļ�·��
            GeoDataCenterDbFun dDbFun = new GeoDataCenterDbFun();
            string strModpath = dDbFun.GetInfoFromMdbByExp(strCon, strExp);

            //��ȡģ��·��
            string strModFile = Application.StartupPath + "\\..\\Template\\" + strModpath;

            //���Ƶ�Temp�ļ�����
            string strWorkFile = Application.StartupPath + "\\..\\Temp\\SupplementalPrj.xml";
            File.Copy(strModFile, strWorkFile, true);

            //��ʼ����ͼ�ĵ���
            string strBuffer = tCurNode.Text;
            TreeNode tParent = tCurNode.Parent;
            TreeNode tRoot = tParent.Parent;
            string strCodeUnitName = tRoot.Text;
            string strSuffixx = strCodeUnitName + strBuffer; //��׺ 

            //��¼��־
            LogFile log = new LogFile(tipRichBox, m_strLogFilePath);
            string strLog = "��ʼ׷�ӵ���" + tCurNode.Text + "_" + strSuffixx;
            if (log != null)
            {
                log.Writelog(strLog);
            }
            vProgress.SetProgress(strLog);

            //��ȡ�����������루���ݵ�Ԫ���룩
            strExp = "select �������� from ���ݵ�Ԫ�� where �������� ='" + strCodeUnitName + "' and ���ݵ�Ԫ����='3'";
            string strCodeUnitCode = dDbFun.GetInfoFromMdbByExp(strCon, strExp);

            //��ȡ���
            string strYear = strBuffer.Substring(0, 4);

            //��ȡ�����ߴ���
            int iStartPos = strBuffer.IndexOf("��");
            int iEndPos = strBuffer.IndexOf("��");
            int iLength = iEndPos - iStartPos - 1;
            string strScaleName = strBuffer.Substring(iStartPos + 1, iLength);
            strExp = "select ���� from �����ߴ���� where ���� ='" + strScaleName + "'";
            string strScaleCode = dDbFun.GetInfoFromMdbByExp(strCon, strExp);

            //׷�ӵ��ĸ��µ�ͼ�ĵ���  ���ݵ�Ԫ����   ���  ������  ר������
            AddUpdateMapDocTree(strWorkFile, strSuffixx, strCodeUnitCode, strYear, strScaleCode, strSubType, vProgress);

            //׷�Ӹ����ĵ�������
            vProgress.SetProgress("׷�Ӹ����ĵ�������");

            AddUpdateTextDocTree(strWorkFile, strSuffixx, strCodeUnitCode, strYear, strScaleCode, strSubType, vProgress);
            vProgress.Close();

            if (!XzqLoad)
            {
                //��������ת����ͼ�ĵ�����
                IndextabControl.SelectedTab = PageMapDoc;

                //��ͼ������ת��ͼ���������
              //  CenterTabControl.SelectedTab = MapPage;
            }

            //��SupplementalPrj�е�������ӵ�CurPrj.xml��
            UpdateCurPrjXml();

            //ɾ����ʱ�ļ�
            File.Delete(strWorkFile);
        }

        //���µ�ǰ��CurPrj.xml
        public void UpdateCurPrjXml()
        {
            string strCurFile = Application.StartupPath + "\\..\\Temp\\CurPrj.xml";
            string strWorkFile = Application.StartupPath + "\\..\\Temp\\SupplementalPrj.xml";
            XmlDocument xmlWorkdoc = new XmlDocument();
            xmlWorkdoc.Load(strWorkFile);
            string strSearchRoot = "//GisMap";
            XmlNode xmlNodeRoot = xmlWorkdoc.SelectSingleNode(strSearchRoot);

            XmlDocument xmlCurdoc = new XmlDocument();
            xmlCurdoc.Load(strCurFile);
            strSearchRoot = "//GisDoc";
            xmlCurdoc.Save(strCurFile);
            XmlNode xmlNodeAddRoot = xmlCurdoc.SelectSingleNode(strSearchRoot);
            XmlNode xmlNodeAdd = xmlCurdoc.ImportNode(xmlNodeRoot, true);
            xmlNodeAddRoot.AppendChild(xmlNodeAdd);

            xmlCurdoc.Save(strCurFile);
        }

        //׷�ӵ��ĸ��µ�ͼ�ĵ���,���޸�Layer�ڵ��sFile����
        public void AddUpdateMapDocTree(string strModPath, string strSuffixx, string strXzq, string strYear, string strScale, string strSubType, SysCommon.CProgress vProgress)
        {

            //�ж��ļ��Ƿ����
            XmlDocument xmldoc = new XmlDocument();
            if (!File.Exists(strModPath))
                return;
            xmldoc.Load(strModPath);

            //���׷�ӵ���ר�����ڵ��ģ��ⲻ��Ӧ
            string strSearchExp = "//GisMap[@sSubject = '" + strSubType + "'" + "and @sYear = '" + strYear + "'"
            + "and @sXzqCode ='" + strXzq + "'" + "and @sScale='" + strScale + "']";
            string strCurFile = Application.StartupPath + "\\..\\Temp\\CurPrj.xml";
            XmlDocument xmlCurdoc = new XmlDocument();
            if (File.Exists(strCurFile))
            {
                xmlCurdoc.Load(strCurFile);
                XmlNode xmlFindNode = xmlCurdoc.SelectSingleNode(strSearchExp);
                if (xmlFindNode != null)
                    return;
            }


            //�ӵ�ͼ�����Ϣ���л�ȡ�����������Ϣ��ͼ����ɣ�
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            // string mypath = dIndex.GetDbValue("dbServerPath");
            string mypath = dIndex.GetDbInfo();
            string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
            string strExp = "select ͼ����� from ��ͼ�����Ϣ�� where �������� ='" + strXzq + "'" + "And " + " ���='" +
                strYear + "'" + "And " + " ������='" + strScale + "'" + "And " + " ר������='" + strSubType + "'";
            GeoDataCenterDbFun dDbFun = new GeoDataCenterDbFun();
            string strLayerGroup = dDbFun.GetInfoFromMdbByExp(strCon, strExp);

            //��������
            //strExp = "select �������� from ��ͼ�����Ϣ�� where �������� ='" + strXzq + "'" + "And " + " ���='" +
            //   strYear + "'" + "And " + " ������='" + strScale + "'" + "And " + " ר������='" + strSubType + "'";
            //string strDataType = dDbFun.GetInfoFromMdbByExp(strCon, strExp);

            //ҵ�����  ���� ���� �ػ�
            strExp = "select ҵ����� from ��ͼ�����Ϣ�� where �������� ='" + strXzq + "'" + "And " + " ���='" +
               strYear + "'" + "And " + " ������='" + strScale + "'" + "And " + " ר������='" + strSubType + "'";
            string strBusinesCode = dDbFun.GetInfoFromMdbByExp(strCon, strExp);

            //ͼ������ǰ׺
            //string strPrefix = strBusinesCode + strYear + strSubType + strXzq + strScale;

            //��ȡӰ���
            strExp = "select Ӱ��� from ��ͼ�����Ϣ�� where �������� ='" + strXzq + "'" + "And " + " ���='" +
             strYear + "'" + "And " + " ������='" + strScale + "'" + "And " + " ר������='" + strSubType + "'";
            string strGridLib = dDbFun.GetInfoFromMdbByExp(strCon, strExp);

            //������  GisMap ���ڵ� Group ר��ڵ� Layer Ҷ�ӽڵ�
            if (m_tparent == null)
            {
                //���ڵ�
                m_tparent = new TreeNode();
                m_tparent.Text = "��ͼĿ¼";

                MapDocTree.Nodes.Add(m_tparent);
                MapDocTree.ExpandAll();
                m_tparent.ImageIndex = 18;
                m_tparent.SelectedImageIndex = 18;
            }


            TreeNode tMapNode;
            TreeNode tNewNode;
            string strTblName = "";
            string strRootName = "";
            string strSearchRoot = "//GisMap";
            XmlNode xmlNodeRoot = xmldoc.SelectSingleNode(strSearchRoot);
            XmlElement xmlElentRoot = (XmlElement)xmlNodeRoot;
            strRootName = xmlElentRoot.GetAttribute("sItemName");
            xmlElentRoot.SetAttribute("sYear", strYear);
            xmlElentRoot.SetAttribute("sSubject", strSubType);
            xmlElentRoot.SetAttribute("sXzqCode", strXzq);
            xmlElentRoot.SetAttribute("sScale", strScale);
            tMapNode = new TreeNode();
            tMapNode.Text = strRootName + "_" + strSuffixx;
            tMapNode.Name = strSubType;
            tMapNode.Checked = true;

            //������ڵ�
            IGroupLayer pGroupFLayer = new GroupLayer();
            pGroupFLayer.Name = tMapNode.Text;
            IMapLayers pmaplayers = axMapControl.Map as IMapLayers;
            pmaplayers.InsertLayer(pGroupFLayer, false, pmaplayers.LayerCount);


            m_tparent.Nodes.Add(tMapNode);
            m_tparent.ExpandAll();
            tMapNode.ImageIndex = 9;
            tMapNode.SelectedImageIndex = 9;

            //����Ӱ��������ݾ����Ӱ��ڵ�
            if (strGridLib != "")
            {
                //����xml�ļ� ����ڵ�
                XmlElement xmlElemGroup = xmldoc.CreateElement("SubGroup");
                string strGroupName = "��Ӱ�����ݡ�";
                xmlElemGroup.SetAttribute("sItemName", strGroupName);
                xmlElemGroup.SetAttribute("sType", "GROUP");


                XmlElement xmlElement = xmldoc.CreateElement("Layer");
                xmlElement.SetAttribute("sDemo", "Ӱ������");
                string name = strGridLib.Substring(strGridLib.LastIndexOf("\\") + 1);
                xmlElement.SetAttribute("sItemName", "Ӱ������");
                //xmlElement.SetAttribute("sFile",name);
                xmlElement.SetAttribute("sDataSource", strGridLib);
                xmlElemGroup.AppendChild(xmlElement);

                //string strSearchBuffer = "//SubGroup";
                //XmlNode xmlNdGroupFirst = xmldoc.SelectSingleNode(strSearchBuffer);
                //if (xmlNdGroupFirst != null)
                //{
                //    xmlElentRoot.InsertBefore(xmlElemGroup, xmlNdGroupFirst);

                //}
                //else
                //{
                //    xmlElentRoot.AppendChild(xmlElemGroup);
                //}
                xmlElentRoot.AppendChild(xmlElemGroup);
            }

            xmldoc.Save(strModPath);

            //������ӵ�һ���ӽڵ� SubGroup
            string strSearch = "//SubGroup";
            XmlNodeList xmlNdList = xmldoc.SelectNodes(strSearch);
            foreach (XmlNode xmlChild in xmlNdList)
            {
                strTblName = "";
                XmlElement xmlElent = (XmlElement)xmlChild;
                strTblName = xmlElent.GetAttribute("sItemName");

                tNewNode = new TreeNode();
                tNewNode.Text = strTblName;
                tNewNode.Checked = true;

                tMapNode.Nodes.Add(tNewNode);
                tMapNode.ExpandAll();
                tNewNode.SelectedImageIndex = 20;
                tNewNode.ImageIndex = 20;

                //��������ӽڵ�
                AddLeafItem(pGroupFLayer, strSubType, tNewNode, xmlChild, strLayerGroup, strYear, strXzq, strScale);
            }

            //ɾ������ֵΪ�յĽڵ�
            DeleNullElementForXml(xmldoc);
            xmldoc.Save(strModPath);

            //��¼��־
            LogFile log = new LogFile(tipRichBox, m_strLogFilePath);
            string strLog = tMapNode.Text + "׷�ӵ������";
            if (log != null)
            {
                log.Writelog(strLog);
            }
            vProgress.SetProgress(strLog);
        }


        //�Ҽ���Ӧ  ����ͼ��������ڵ��Ҽ��˵�
        private void axMapControl_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e)
        {
            //�Ҽ������ʱ�򵯳��Ҽ��˵�
        /*    if (e.button == 2)
            {
                System.Drawing.Point ClickPoint = axMapControl.PointToScreen(new System.Drawing.Point(e.x, e.y));
                axMapControlcontextMenu.Show(ClickPoint);
            }*/
        }


        //��ͼ�ĵ����Ҽ���Ӧ
        private void MapDocTree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            System.Drawing.Point ClickPoint;
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            string mypath = dIndex.GetDbInfo();
            string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
            GeoDataCenterDbFun db = new GeoDataCenterDbFun();
            //���ڵ�ĵ����¼����任ѡ��������ɫ��ͼ��
            if (MapDocTree.SelectedNode != e.Node)
            {
                if (MapDocTree.SelectedNode != null)
                {
                    //MapDocTree.SelectedNode.ForeColor = Color.Black;
                }

                MapDocTree.SelectedNode = e.Node;
                //e.Node.ForeColor = Color.Red;
            }



            //��Ϊ��ǰ�༭ͼ��
            /*     IFeatureWorkspace pFeatureWorkspace = ModFrameData.v_AppGisUpdate.gisDataSet.WorkSpace as IFeatureWorkspace;
                 Exception exError = null;
                 IFeatureClass pFC = ModFrameData.v_AppGisUpdate.gisDataSet.GetFeatureClass(e.Node.Name.ToString(), out exError);
     */
            //�������ڵ����ƻ�ȡ�ڵ��sFile
            //���ڵ������sSubject
            TreeNode tRootNode;
            string strSubject = "";
            string strRootMapText = "";
            if (MapDocTree.SelectedNode != null)
            {
                if (MapDocTree.SelectedNode.Parent != null)
                {
                    tRootNode = MapDocTree.SelectedNode.Parent.Parent;
                    if (tRootNode != null)
                    {
                        strSubject = tRootNode.Name;
                        strRootMapText = tRootNode.Text;
                    }
                }

            }

            //string sFile = GetLayerNameFormNodeName(MapDocTree.SelectedNode.Name, strSubject);//changed by chulili 2011-04-15
            string sFile = GetLayersFileFormNode(MapDocTree.SelectedNode);
            if (sFile != "")
            {
                /*     IFeatureClass pFC = pFeatureWorkspace.OpenFeatureClass(sFile);
                     if (pFC != null)
                     {
                  
                          int iLayerCount = axMapControl.Map.LayerCount;
                         IFeatureLayer pFeatureLayer;
                         IMap pMap = axMapControl.Map;
                         for (int ii = 0; ii < iLayerCount; ii++)
                         {
                             pFeatureLayer = pMap.get_Layer(ii) as IFeatureLayer;
                             if (pFeatureLayer == null)
                                 continue;
                             if (pFeatureLayer.FeatureClass == null)
                                 continue;
                             if (pFeatureLayer.FeatureClass.FeatureClassID == pFC.FeatureClassID)
                             {
                                 axMapControl.CustomProperty = pFeatureLayer;
                                 FeatureTrue = true;
                                 //�Ҽ������ʱ�򵯳��Ҽ��˵�
                                 if (e.Button == MouseButtons.Right)
                                 {

                                     //����ѡ�е���Ҷ�ӽڵ�
                                     ClickPoint = MapDocTree.PointToScreen(new System.Drawing.Point(e.X, e.Y));
                                     MapDocTreecontextMenu.Show(ClickPoint);//changed by xs 20100415 ��ʾʸ�����ݲ˵�
                                 }
                                 break;
                             }
                         }
                     }*/

                //���ݸ��׽ڵ��ȡ��ڵ������  strRootMapText
                int iLayerCount = axMapControl.Map.LayerCount; ;
                IFeatureLayer pFeatureLayer;
                IMap pMap = axMapControl.Map;
                for (int n = 0; n < pMap.LayerCount; n++)
                {
                    ILayer layer = pMap.get_Layer(n);
                    if (layer is IGroupLayer && layer.Name == strRootMapText)
                    {
                        ICompositeLayer Comlayer = layer as ICompositeLayer;//��һ��������Ĳ����ת����һ����ϲ㣬ʹ�����Ա���
                        for (int c = 0; c < Comlayer.Count; c++)
                        {
                            pFeatureLayer = Comlayer.get_Layer(c) as IFeatureLayer;
                            if (pFeatureLayer == null)
                                continue;
                            if (pFeatureLayer.FeatureClass == null)
                                continue;
                            if (pFeatureLayer.Name == MapDocTree.SelectedNode.Name)
                            {
                                axMapControl.CustomProperty = pFeatureLayer;
                                FeatureTrue = true;
                                //�Ҽ������ʱ�򵯳��Ҽ��˵�
                                if (e.Button == MouseButtons.Right)
                                {

                                    //����ѡ�е���Ҷ�ӽڵ�
                                    ClickPoint = MapDocTree.PointToScreen(new System.Drawing.Point(e.X, e.Y));
                                    MapDocTreecontextMenu.Show(ClickPoint);//changed by xs 20100415 ��ʾʸ�����ݲ˵�
                                }
                                break;
                            }
                        }
                    }
                }

            }
            else//Ӱ��ͼ����ʾ״̬
            {
                sFile = GetRasterLayerFormNodeName(MapDocTree.SelectedNode.Name, strSubject);
                int iLayerCount = axMapControl.Map.LayerCount;
                IRasterLayer pRasterLayer;
                IMap pMap = axMapControl.Map;
                for (int ii = 0; ii < iLayerCount; ii++)
                {
                    ILayer layer = pMap.get_Layer(ii);
                    if (layer is IGroupLayer && layer.Name == strRootMapText)
                    {
                        ICompositeLayer Comlayer = layer as ICompositeLayer;//��һ��������Ĳ����ת����һ����ϲ㣬ʹ�����Ա���
                        for (int c = 0; c < Comlayer.Count; c++)
                        {
                            pRasterLayer = Comlayer.get_Layer(c) as IRasterLayer;
                            if (pRasterLayer == null)
                                continue;
                            if (pRasterLayer.Name.Substring(0, pRasterLayer.Name.LastIndexOf("\\")).ToUpper() == sFile.ToUpper())
                            {
                                axMapControl.CustomProperty = pRasterLayer;
                                FeatureTrue = false;
                                //�Ҽ������ʱ�򵯳��Ҽ��˵�
                                if (e.Button == MouseButtons.Right)
                                {
                                    //����ѡ�е���Ҷ�ӽڵ�
                                    ClickPoint = MapDocTree.PointToScreen(new System.Drawing.Point(e.X, e.Y));
                                    MapDocTreeRastercontextMenu.Show(ClickPoint);//changed by xs 20100415 ��ʾӰ�����ݲ˵�

                                }
                                break;
                            }
                        }
                    }
                }
            }
        }


   
        //��ͼ�ĵ����Ҽ��˵�������Ӧ
        #region
        private void ClearSeletable()
        {
            int iLayerCount = axMapControl.Map.LayerCount;
            IFeatureLayer pFeatureLayer; 
            IMap pMap = axMapControl.Map;
            for (int n = 0; n < iLayerCount; n++)
            {
                ILayer layer = pMap.get_Layer(n);
                if (layer is IGroupLayer)
                {
                    ICompositeLayer Comlayer = layer as ICompositeLayer;//��һ��������Ĳ����ת����һ����ϲ㣬ʹ�����Ա���
                    for (int c = 0; c < Comlayer.Count; c++)
                    {
                        pFeatureLayer = Comlayer.get_Layer(c) as IFeatureLayer;
                        if (pFeatureLayer != null) pFeatureLayer.Selectable = false;

                    }
                }
            }
        }
        private void SetAllSeletable()
        {
            int iLayerCount = axMapControl.Map.LayerCount;
            IFeatureLayer pFeatureLayer;
            IMap pMap = axMapControl.Map;
            for (int n = 0; n < iLayerCount; n++)
            {
                ILayer layer = pMap.get_Layer(n);
                if (layer is IGroupLayer)
                {
                    ICompositeLayer Comlayer = layer as ICompositeLayer;//��һ��������Ĳ����ת����һ����ϲ㣬ʹ�����Ա���
                    for (int c = 0; c < Comlayer.Count; c++)
                    {
                        pFeatureLayer = Comlayer.get_Layer(c) as IFeatureLayer;
                        if (pFeatureLayer != null) pFeatureLayer.Selectable = true;

                    }
                }
            }
        }
        //����ͼ��Ϊ��ǰ�༭��
        private void MenuItemSetEditFlag_Click(object sender, EventArgs e)
        {
            TreeNode tSelNode; IFeatureLayer pOldLayer, pNewLayer;
            tSelNode = MapDocTree.SelectedNode;
            if (m_CurEditLayerNode != null)
            {
                m_CurEditLayerNode.ForeColor = Color.Black;
                pOldLayer = GetLayerofTreeNode(m_CurEditLayerNode) as IFeatureLayer ;
                if (pOldLayer != null) pOldLayer.Selectable = false;
            }
            else
            {
                ClearSeletable();
            }
            tSelNode.ForeColor = Color.Red;
            m_CurEditLayerNode = tSelNode;
            m_CurEditTopicNode = tSelNode.Parent.Parent;
            pNewLayer = GetLayerofTreeNode(tSelNode) as IFeatureLayer;
            if (pNewLayer!=null) pNewLayer.Selectable = true;
        }

        //ȡ����ǰͼ��ĵ�ǰ�༭״̬
        private void MenuItemReMoveEditFalg_Click(object sender, EventArgs e)
        {
            TreeNode tSelNode;
            tSelNode = MapDocTree.SelectedNode;
            if (tSelNode.ForeColor == Color.Red)
                tSelNode.ForeColor = Color.Black;
            if (m_CurEditLayerNode == null)
                return;
            if (m_CurEditLayerNode.Equals(tSelNode))
            {
                m_CurEditLayerNode = null;
                m_CurEditTopicNode = null;
                SetAllSeletable();
            }
            
        }


        //��ͼ���ŵ���ǰͼ�㷶Χ
        private void MenuItemZoomToLayer_Click(object sender, EventArgs e)
        {
            ILayer pLayer = null;
            pLayer = (ILayer)axMapControl.CustomProperty;
            if (pLayer == null)
                return;
            IActiveView pActiveView = axMapControl.ActiveView;
            pActiveView.Extent = pLayer.AreaOfInterest;
            pActiveView.Refresh();
        }

        //��������
        private void MenuItemSetSymbol_Click(object sender, EventArgs e)
        {
            IFeatureLayer pLayer = (IFeatureLayer)axMapControl.CustomProperty;
            if (pLayer == null)
                return;

            try
            {
                GeoSymbology.frmSymbology frm = new GeoSymbology.frmSymbology(pLayer);
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    ESRI.ArcGIS.Carto.IGeoFeatureLayer pGeoLayer = pLayer as ESRI.ArcGIS.Carto.IGeoFeatureLayer;
                    pGeoLayer.Renderer = frm.FeatureRenderer();
                    axMapControl.ActiveView.Refresh();
                    // _AppHk.TOCControl.Update();

                    //���浱ǰ�ķ�����Ϣ
                    string strLyrName = pLayer.Name;
                    if (pGeoLayer.FeatureClass != null)
                    {
                        IDataset pDataset = pGeoLayer.FeatureClass as IDataset;
                        strLyrName = pDataset.Name;
                    }
                    strLyrName = strLyrName.Substring(strLyrName.IndexOf('.') + 1);

                    /*  XmlDocument vDoc = new XmlDocument();
                      vDoc.Load(System.Windows.Forms.Application.StartupPath + "\\..\\Template\\SymbolInfo.xml");
                      UpdateSymbolInfo(strLyrName, vDoc, pGeoLayer.Renderer);
                      vDoc.Save(System.Windows.Forms.Application.StartupPath + "\\..\\Template\\SymbolInfo.xml");*/
                }
            }
            catch
            {
                return;
            }
        }

        //ͼ�����Լ�¼��ֵ
        private void MenuItemOpenLayerAtt_Click(object sender, EventArgs e)
        {
            ILayer pLayer = axMapControl.CustomProperty as ILayer;
            frmAttributeTable frm = new frmAttributeTable(axMapControl);
            frm.CreateAttributeTable(pLayer);
            frm.ShowDialog();

            string strName = "�鿴" + pLayer.Name + "ͼ��������Ϣ";
            LogFile log = new LogFile(tipRichBox, m_strLogFilePath);
            if (log != null)
            {
                log.Writelog(strName);
            }
        }

        //ͼ��������Ϣ
        private void MenuItemLayerAtt_Click(object sender, EventArgs e)
        {
            ILayer pLayer = axMapControl.CustomProperty as ILayer;
            if (pLayer != null)
            {

                string strName = "";
                if (FeatureTrue)//ʸ������
                {
                    strName = "�鿴" + pLayer.Name + "ͼ������ֵ��Ϣ";
                }
                else//դ������
                {
                    strName = "�鿴Ӱ������ͼ������ֵ��Ϣ";
                }
                LogFile log = new LogFile(tipRichBox, m_strLogFilePath);
                if (log != null)
                {
                    log.Writelog(strName);
                }
                frmLayerProperties layerProDialog = new frmLayerProperties(pLayer, axMapControl.ActiveView,FeatureTrue);
                layerProDialog.Show();
            }
        }

        //��������        
        #region
        //==================================================
        //���ߣ�ϯʤ
        //���ڣ�2011.03.02
        //��������  �������Ϊmdb\shp��ʽ
        //==================================================
        private void MenuItemOutputLayer_Click(object sender, EventArgs e)
        {
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = "";
            dlg.Filter = "mdb����|*.mdb|shp����|*.shp";
            dlg.FilterIndex = 1;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                if (1 == dlg.FilterIndex)
                {
                    ExportMdb(dlg);
                }
                else if (2 == dlg.FilterIndex)
                {
                    ExportShp(dlg);
                }
            }
        }

        //����MDB����
        private void ExportMdb(SaveFileDialog dlg)
        {
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            //��ȡģ��·��
            string sourcefilename = Application.StartupPath + "\\..\\Template\\DATATEMPLATE.mdb";
            //��workspace�л�ȡ��Ӧ��layer
         /*   string dbpath = dIndex.GetDbValue("dbServerPath");
            IWorkspaceFactory pWorkspaceFactory = new FileGDBWorkspaceFactoryClass();
            IWorkspace pWorkspace = pWorkspaceFactory.OpenFromFile(dbpath, 0);*/
            string strSubject = "";
            if (MapDocTree.SelectedNode != null)
            {
                if (MapDocTree.SelectedNode.Parent != null)
                {
                    if (MapDocTree.SelectedNode.Parent.Parent != null)
                    {
                        strSubject = MapDocTree.SelectedNode.Parent.Parent.Name;
                    }
                }

            }

            //string strLayerName = GetLayerNameFormNodeName(MapDocTree.SelectedNode.Name, strSubject);//changed by chulili 2011-04-15
            string strLayerName = GetLayersFileFormNode(MapDocTree.SelectedNode);
            IFeatureWorkspace pFeaWorkspace;
            //���m_strnew !="" �����ӵ�
            if (m_strnew.Trim() != "")
            {
                pFeaWorkspace = GetWorkspaceByFileName2(strLayerName);
                strLayerName = strLayerName.Substring(strLayerName.LastIndexOf("\\") + 1);
                strLayerName = System.IO.Path.GetFileNameWithoutExtension(strLayerName);
            }
            else
            {
                //����m_strnew== "" ά��ԭ������
                pFeaWorkspace = GetWorkspaceByFileName(strLayerName);
            }
            IWorkspace pWorkspace = pFeaWorkspace as IWorkspace;
            try
            {
                if (File.Exists(sourcefilename))//ԭģ�����
                {
                   // DialogResult result = MessageBox.Show("�����Ƿ�ȥ��ǰ׺��", "��ʾ", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    //�������mdb,�滻�ļ�������ģ�嵽ָ��·��
                    //�������mdb�����滻����׷�ӵ�����ļ�
                    File.Copy(sourcefilename, dlg.FileName, true);
                    IWorkspaceFactory Pwf = new AccessWorkspaceFactoryClass();
                    IWorkspace pws = (IWorkspace)(Pwf.OpenFromFile(dlg.FileName, 0));
                    IWorkspace2 pws2 = (IWorkspace2)pws;
                    ILayer pLayer = axMapControl.CustomProperty as ILayer;
                    if (pLayer != null)
                    {
                        IFeatureLayer pFeatureLayer = pLayer as IFeatureLayer;
                        if (pFeatureLayer.Visible)
                        {
                            string cellvalue;
                            if(m_strnew.Trim() != "")
                                cellvalue=strLayerName;
                            else{
                                cellvalue = pFeatureLayer.FeatureClass.AliasName;
                            }
                           // if (result == DialogResult.Yes) cellvalue = cellvalue.Substring(15);//ȥ��ǰ׺
                            if (pws2.get_NameExists(esriDatasetType.esriDTFeatureClass, cellvalue))
                            {
                                IFeatureClass tmpfeatureclass;
                                IFeatureWorkspace pFeatureWorkspace = (IFeatureWorkspace)pws;
                                tmpfeatureclass = pFeatureWorkspace.OpenFeatureClass(cellvalue);
                                IDataset set = tmpfeatureclass as IDataset;
                                set.CanDelete();
                                set.Delete();
                            }
                            IFeatureDataConverter_ConvertFeatureClass(pWorkspace, pws, cellvalue, cellvalue);
                        }
                    }
                    MessageBox.Show("�����ɹ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LogFile log = new LogFile(tipRichBox, m_strLogFilePath);
                    if (log != null)
                    {
                        log.Writelog("����ͼ�㵽" + dlg.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        //����SHP����
        private void ExportShp(SaveFileDialog dlg)
        {
            string file = dlg.FileName.Substring(0, dlg.FileName.LastIndexOf('\\'));
            if (!System.IO.Directory.Exists(file))
            {
                System.IO.Directory.CreateDirectory(file);
            }
            try
            {
                ILayer pLayer = axMapControl.CustomProperty as ILayer;
                if (pLayer != null)
                {
                    IFeatureLayer pFeatureLayer = pLayer as IFeatureLayer;
                    if (pFeatureLayer.Visible)
                    {
                        ExportFeature(pFeatureLayer.FeatureClass, dlg.FileName);
                    }
                }
                MessageBox.Show("�����ɹ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch
            {
                MessageBox.Show("����ʧ��!", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        //����shp����
        public void ExportFeature(IFeatureClass pInFeatureClass, string pPath)
        {

            // create a new Access workspace factory  
            IWorkspaceFactory pWorkspaceFactory = new ShapefileWorkspaceFactoryClass();//��ҪESRI.ArcGIS.DataSourcesFile�����ռ�;
            string parentPath = pPath.Substring(0, pPath.LastIndexOf('\\'));
            string fileName = pPath.Substring(pPath.LastIndexOf('\\') + 1, pPath.Length - pPath.LastIndexOf('\\') - 1);
            IWorkspaceName pWorkspaceName = pWorkspaceFactory.Create(parentPath, fileName, null, 0);
            // Cast for IName        
            IName name = (IName)pWorkspaceName;

            //Open a reference to the access workspace through the name object        
            IWorkspace pOutWorkspace = (IWorkspace)name.Open();

            IDataset pInDataset = pInFeatureClass as IDataset;
            IFeatureClassName pInFCName = pInDataset.FullName as IFeatureClassName;
            IWorkspace pInWorkspace = pInDataset.Workspace;
            IDataset pOutDataset = pOutWorkspace as IDataset;
            IWorkspaceName pOutWorkspaceName = pOutDataset.FullName as IWorkspaceName;
            IFeatureClassName pOutFCName = new FeatureClassNameClass();
            IDatasetName pDatasetName = pOutFCName as IDatasetName;
            pDatasetName.WorkspaceName = pOutWorkspaceName;
            pDatasetName.Name = pInFeatureClass.AliasName;
            IFieldChecker pFieldChecker = new FieldCheckerClass();
            pFieldChecker.InputWorkspace = pInWorkspace;
            pFieldChecker.ValidateWorkspace = pOutWorkspace;
            IFields pFields = pInFeatureClass.Fields;
            IFields pOutFields;
            IEnumFieldError pEnumFieldError;
            pFieldChecker.Validate(pFields, out pEnumFieldError, out pOutFields);
            IFeatureDataConverter pFeatureDataConverter = new FeatureDataConverterClass();
            pFeatureDataConverter.ConvertFeatureClass(pInFCName, null, null, pOutFCName, null, pOutFields, "", 100, 0);
        }

        /// <summary>
        /// ����jpgͼƬ��ʽ����
        /// </summary>
        /// <param name="dlg"></param>
        #region
        private void ExportJpg(SaveFileDialog dlg)
        {
            try
            {
                IExport pExport = new ExportJPEGClass();//IExport��Ҫusing ESRI.ArcGIS.Output�����ռ�;
                pExport.ExportFileName = dlg.FileName;

                //���ò��� Ĭ�Ͼ���
                int reslution = 96;
                pExport.Resolution = reslution;
                //��ȡ������Χ
                tagRECT exportRect = axMapControl.ActiveView.ExportFrame;//tagRECT��Ҫ ESRI.ArcGIS.Display;
                ESRI.ArcGIS.Geometry.IEnvelope Env = new ESRI.ArcGIS.Geometry.EnvelopeClass();//tagRECT��Ҫ ESRI.ArcGIS.Geometry;
                Env.PutCoords(exportRect.left, exportRect.top, exportRect.right, exportRect.bottom);
                pExport.PixelBounds = Env;
                //��ʼ����,get DC  
                int hDC = pExport.StartExporting();
                axMapControl.ActiveView.Output(hDC, reslution, ref exportRect, null, null);
                pExport.FinishExporting();
                pExport.Cleanup();
                MessageBox.Show("�����ɹ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch
            {
                MessageBox.Show("����ʧ��!", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }
        #endregion
        #endregion

        //�һ��������������ԶԻ���
        private void MenuItemAtt_Click(object sender, EventArgs e)
        {
            frmMapProperty frm = new frmMapProperty();
            frm.ThisNode = DataUnitTree.SelectedNode;
            if (frm.ThisNode != null)
            {
                string strName = "�鿴" + frm.ThisNode.Parent.Text + frm.ThisNode.Text + "����������Ϣ";
                LogFile log = new LogFile(tipRichBox, m_strLogFilePath);
                if (log != null)
                {
                    log.Writelog(strName);
                }

                frm.Show();
            }
        }

        #endregion


        //��ͼ��������Ҽ��˵���ʵ��        
        #region
        ///<summary>
        ///˵����ͼ���Ҽ��˵���ʵ��
        ///���ߣ�ϯʤ
        ///���ڣ�2011-03-01
        ///</summary>

        //�Ҽ�����Ŵ�
        private void MapZoomInMenuItem_Click(object sender, EventArgs e)
        {
            ESRI.ArcGIS.SystemUI.ICommand pCommand;
            pCommand = new ESRI.ArcGIS.Controls.ControlsMapZoomInToolClass();
            pCommand.OnCreate(axMapControl.Object);
            axMapControl.CurrentTool = pCommand as ESRI.ArcGIS.SystemUI.ITool;
        }

        //�Ҽ������С
        private void ZoomOutMenuItem_Click(object sender, EventArgs e)
        {
            ESRI.ArcGIS.SystemUI.ICommand pCommand;
            pCommand = new ESRI.ArcGIS.Controls.ControlsMapZoomOutToolClass();
            pCommand.OnCreate(axMapControl.Object);
            axMapControl.CurrentTool = pCommand as ESRI.ArcGIS.SystemUI.ITool;
        }

        //�Ҽ��������
        private void MapPanMenuItem_Click(object sender, EventArgs e)
        {

            ESRI.ArcGIS.SystemUI.ICommand pCommand;
            pCommand = new ESRI.ArcGIS.Controls.ControlsMapPanToolClass();
            pCommand.OnCreate(axMapControl.Object);
            axMapControl.CurrentTool = pCommand as ESRI.ArcGIS.SystemUI.ITool;
        }

        //�Ҽ����������С
        private void MapZoomOutFixedMenuItem_Click(object sender, EventArgs e)
        {
            ESRI.ArcGIS.SystemUI.ICommand pCommand;
            pCommand = new ESRI.ArcGIS.Controls.ControlsMapZoomOutFixedCommandClass();
            pCommand.OnCreate(axMapControl.Object);
            pCommand.OnClick();
        }

        //�Ҽ�������ķŴ�
        private void MapZoomInFixedMenuItem_Click(object sender, EventArgs e)
        {
            ESRI.ArcGIS.SystemUI.ICommand pCommand;
            pCommand = new ESRI.ArcGIS.Controls.ControlsMapZoomInFixedCommandClass();
            pCommand.OnCreate(axMapControl.Object);
            pCommand.OnClick();
        }

        //�Ҽ����ˢ��
        private void MapRefreshMenuItem_Click(object sender, EventArgs e)
        {
            ESRI.ArcGIS.SystemUI.ICommand pCommand;
            pCommand = new ESRI.ArcGIS.Controls.ControlsMapRefreshViewCommandClass();
            pCommand.OnCreate(axMapControl.Object);
            pCommand.OnClick();
        }

        //�Ҽ����ȫ��
        private void MapFullExtentMenuItem_Click(object sender, EventArgs e)
        {
            ESRI.ArcGIS.SystemUI.ICommand pCommand;
            pCommand = new ESRI.ArcGIS.Controls.ControlsMapFullExtentCommandClass();
            pCommand.OnCreate(axMapControl.Object);
            pCommand.OnClick();
        }

        //�Ҽ������
        private void BackCommandMenuItem_Click(object sender, EventArgs e)
        {
            ESRI.ArcGIS.SystemUI.ICommand pCommand;
            pCommand = new ESRI.ArcGIS.Controls.ControlsMapZoomToLastExtentBackCommandClass();
            pCommand.OnCreate(axMapControl.Object);
            pCommand.OnClick();
        }

        //�Ҽ����ǰ��
        private void ForwardCommandMenuItem_Click(object sender, EventArgs e)
        {
            ESRI.ArcGIS.SystemUI.ICommand pCommand;
            pCommand = new ESRI.ArcGIS.Controls.ControlsMapZoomToLastExtentForwardCommandClass();
            pCommand.OnCreate(axMapControl.Object);
            pCommand.OnClick();
        }

        #endregion

        private void ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmDetailLog frm = new frmDetailLog(m_strLogFilePath);
            frm.Show();
        }

        //˫����ͼ�ĵ���
        private void MapDocTree_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeNode tSelNode;
            tSelNode = e.Node;
            if (tSelNode != null && tSelNode.Level == 3)
            {
                if (tSelNode.Parent.Text != "��Ӱ�����ݡ�")
                {
                    if (m_CurEditLayerNode != null)
                    {
                        if (m_CurEditLayerNode.Equals(tSelNode))
                        {
                            m_CurEditLayerNode.ForeColor = Color.Black;
                            m_CurEditLayerNode = null;
                            m_CurEditTopicNode = null;
                            SetAllSeletable();
                        }
                        else
                        {
                            m_CurEditLayerNode.ForeColor = Color.Black;
                            IFeatureLayer pOldEditLayer = GetLayerofTreeNode(m_CurEditLayerNode) as IFeatureLayer ;
                            if (pOldEditLayer!=null) pOldEditLayer.Selectable = false;
                            tSelNode.ForeColor = Color.Red;
                            m_CurEditLayerNode = tSelNode;
                            m_CurEditTopicNode = tSelNode.Parent.Parent;
                            IFeatureLayer pNewEditLayer = GetLayerofTreeNode(tSelNode) as IFeatureLayer;
                            if (pNewEditLayer!=null) pNewEditLayer.Selectable = true;
                        }
                    }
                    else
                    {
                        ClearSeletable();
                        tSelNode.ForeColor  = Color.Red;
                        m_CurEditLayerNode = tSelNode;
                        m_CurEditTopicNode = tSelNode.Parent.Parent;
                        IFeatureLayer pNewEditLayer = GetLayerofTreeNode(tSelNode) as IFeatureLayer;
                        if (pNewEditLayer!=null) pNewEditLayer.Selectable = true;
                    }
                }
            }
        }

        private void WordToRtf(object docPath)
        {
            try
            {
                System.IO.FileStream fs = null;
                if (!File.Exists((string)docPath))
                {
                    fs = new System.IO.FileStream((string)docPath, FileMode.Create, FileAccess.Write);
                }
                // File.SetAttributes((string)docPath, FileAttributes.Hidden);
                //BinaryWriter bw = new BinaryWriter(fs);
                //bw.Write((Source);

                // ָ��ԭ�ļ���Ŀ���ļ�
                object Target = Application.StartupPath + "\\temp.rtf";
                // ȱʡ����  
                object Unknown = Type.Missing;
                object saveChanges = false;
                object readOnly = true;
                wordApp = new Microsoft.Office.Interop.Word.ApplicationClass();
                // ��doc�ļ�
                doc = wordApp.Documents.Open(ref docPath, ref Unknown,
                      ref readOnly, ref Unknown, ref Unknown,
                      ref Unknown, ref Unknown, ref Unknown,
                      ref Unknown, ref Unknown, ref Unknown,
                      ref Unknown, ref Unknown, ref Unknown,
                      ref Unknown, ref Unknown);
                doc.Activate();

                // ָ�����Ϊ��ʽ(rtf)
                object format = Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatRTF;

                // ת����ʽ
                doc.SaveAs(ref Target, ref format,
                           ref Unknown, ref Unknown, ref Unknown,
                           ref Unknown, ref Unknown, ref Unknown,
                           ref Unknown, ref Unknown, ref Unknown,
                           ref Unknown, ref Unknown, ref Unknown,
                           ref Unknown, ref Unknown);

                // �ر��ĵ���Word����
                doc.Close(ref saveChanges, ref Unknown, ref Unknown);
                object o = Microsoft.Office.Interop.Word.WdOriginalFormat.wdOriginalDocumentFormat;
                System.Runtime.InteropServices.Marshal.ReleaseComObject(wordApp);
                wordApp = null;

                #region ����������wordӦ�ý���
                List<int> arr = new List<int>();
                GetWordApp(arr);
                foreach (int i in arr)
                {
                    if (!wordProcess.Exists(delegate(int a) { return a == i; }))
                    {
                        KillOwnWordApp(i);
                    }
                }
                #endregion
            }
            catch
            {

                #region ����������wordӦ�ý���
                List<int> arr = new List<int>();
                GetWordApp(arr);
                foreach (int i in arr)
                {
                    if (!wordProcess.Exists(delegate(int a) { return a == i; }))
                    {
                        KillOwnWordApp(i);
                    }
                }
                #endregion
            }
        }

        //��ȡword����Pid
        private void GetWordApp(List<int> T)
        {
            Process[] ps = Process.GetProcessesByName("WINWORD");
            foreach (Process process in ps)
            {
                T.Add(process.Id);
            }
        }

        //����ָ��word����
        private void KillOwnWordApp(int Pid)
        {
            Process ps = Process.GetProcessById(Pid);
            ps.Kill();
            GC.Collect();
        }

        //�ⲿ���ĵ�
        private void MenuItemOutopen_Click(object sender, EventArgs e)
        {
            object oMissing = System.Reflection.Missing.Value;
            wordApp = new Microsoft.Office.Interop.Word.ApplicationClass();
            wordApp.Visible = true;
            object fileName = @docPath;
            doc = wordApp.Documents.Open(ref fileName,
            ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
            ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
            ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing);
            doc.Activate();

        }


        //���Ϊ
        private void MenuItemSaveas_Click(object sender, EventArgs e)
        {
            string Targetpath = "";
            string Sourcepath = docPath.ToString();
            string name = Sourcepath.Substring(Sourcepath.LastIndexOf("\\") + 1);
            FolderBrowserDialog dg = new FolderBrowserDialog();
            if (dg.ShowDialog() == DialogResult.OK)
            {
                Targetpath = dg.SelectedPath + "\\" + name;


                if (File.Exists(Sourcepath))
                {
                    File.Copy(Sourcepath, Targetpath, true);
                    MessageBox.Show("���Ϊ�ɹ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    string strName = "����ĵ�" + name + ",Ŀ��·��Ϊ:" + Targetpath;
                    LogFile log = new LogFile(tipRichBox, m_strLogFilePath);
                    if (log != null)
                    {
                        log.Writelog(strName);
                    }
                }
            }
        }
        //�ĵ�����
        private void MenuItemDocatt_Click(object sender, EventArgs e)
        {

        }



        //�Ƴ�ͼ��
        private void MenuItemRemovelayer_Click(object sender, EventArgs e)
        {
            //ɾ�����ڵ�
            if (MapDocTree.SelectedNode != null)
            {
                //��¼��־
                LogFile log = new LogFile(tipRichBox, m_strLogFilePath);
                string strLog = "�Ƴ�ͼ��" + MapDocTree.SelectedNode.Text;
                if (log != null)
                {
                    log.Writelog(strLog);
                }


                MapDocTree.SelectedNode.Remove();
                // MapDocTree.update();
            }
            ILayer pLayer = null;
            pLayer = axMapControl.CustomProperty as ILayer;
            if (pLayer == null)
                return;

            //ɾ��workspace�е�featureclass
            axMapControl.Map.DeleteLayer(pLayer);

            axMapControl.ActiveView.Refresh();
        }

        //���ñ�ע
        private void MenuItemSetLabel_Click(object sender, EventArgs e)
        {
            frmSetLabel pFrmSetLabel = new frmSetLabel();
            ILayer pLayer = null;
            pLayer = axMapControl.CustomProperty as ILayer;
            if (pLayer == null)
                return;
            if (pLayer is IGeoFeatureLayer)
            {
                pFrmSetLabel.GeoFeatLayer = pLayer as IGeoFeatureLayer;
                pFrmSetLabel.MapControl = axMapControl.Object as IMapControlDefault;
                pFrmSetLabel.ShowDialog();
            }
        }

        //�Ƴ���ע
        private void MenuItemRemoveLabel_Click(object sender, EventArgs e)
        {
            ILayer pLayer = null;
            pLayer = axMapControl.CustomProperty as ILayer;
            if (pLayer == null)
                return;
            IGeoFeatureLayer pGeoFeatureLayer = null;
            if (pLayer is IGeoFeatureLayer)
            {
                pGeoFeatureLayer = pLayer as IGeoFeatureLayer;
                if (pGeoFeatureLayer.DisplayAnnotation == true)
                {
                    pGeoFeatureLayer.DisplayAnnotation = false;
                }
                axMapControl.ActiveView.Refresh();
            }
        }
        //��ȡĳ��node�ڵ�����Ӧ��ͼ��
        private ILayer GetLayerofTreeNode(TreeNode tnode)
        {
            if (tnode == null) return null;
            if (tnode.Level < 3) return null;

            Exception exError = null;

            IMap pMap = axMapControl.Map;

            string strRootMapText = "";
            //��ȡר������
            if (tnode.Parent != null)
            {
                if (tnode.Parent.Parent != null)
                {
                    strRootMapText = tnode.Parent.Parent.Text;
                }
            }
             
            for (int n = 0; n < pMap.LayerCount; n++)
            {
                ILayer layer = pMap.get_Layer(n);
                if (layer is IGroupLayer && layer.Name == strRootMapText)
                {
                    ICompositeLayer Comlayer = layer as ICompositeLayer;//��һ��������Ĳ����ת����һ����ϲ㣬ʹ�����Ա���
                    for (int c = 0; c < Comlayer.Count; c++)
                    {
                        IFeatureLayer  pFeatureLayer = Comlayer.get_Layer(c) as IFeatureLayer;
                        if (pFeatureLayer == null)
                            continue;
                        if (pFeatureLayer.FeatureClass == null)
                            continue;
                        if (pFeatureLayer.Name == tnode.Name)
                        {
                            return pFeatureLayer as ILayer;
                        }
                    }
                }
            }
            return null;
        }
        //��ȡĳ��node�ڵ�����Ӧ��ͼ���index
        private int GetLayerIndexofTreeNode(TreeNode tnode)
        {
            if (tnode == null) return -1;
            if (tnode.Level < 3) return -1;
            Exception exError = null;

            int iLayerCount = axMapControl.Map.LayerCount;
            IFeatureLayer pFeatureLayer; ILayer pLayer;
            IMap pMap = axMapControl.Map;

            string strRootMapText = "";

            if (tnode.Parent != null)
            {
                if (tnode.Parent.Parent != null)
                {
                    strRootMapText = tnode.Parent.Parent.Text;
                }
            }
            for (int n = 0; n < pMap.LayerCount; n++)
            {
                ILayer layer = pMap.get_Layer(n);
                if (layer is IGroupLayer && layer.Name == strRootMapText)
                {
                    ICompositeLayer Comlayer = layer as ICompositeLayer;//��һ��������Ĳ����ת����һ����ϲ㣬ʹ�����Ա���
                    for (int c = 0; c < Comlayer.Count; c++)
                    {
                        pFeatureLayer = Comlayer.get_Layer(c) as IFeatureLayer;
                        if (pFeatureLayer == null)
                            continue;
                        if (pFeatureLayer.FeatureClass == null)
                            continue;
                        if (pFeatureLayer.Name == tnode.Name)
                        {
                            return c;
                        }
                    }
                }
            }

            return -1;
        }

        //�ڵ���ק
        private void MapDocTree_DragDrop(object sender, DragEventArgs e)
        {
            //����Ϸ��еĽڵ� 
            if (m_LastDragNode != null)
            {
                m_LastDragNode.BackColor = SystemColors.Window;//ȡ����һ�������õĽڵ������ʾ   

                m_LastDragNode.ForeColor = SystemColors.WindowText;
                m_LastDragNode = null;
            }

            TreeNode moveNode = null;
            if (e.Data.GetDataPresent(typeof(TreeNode)))
            {
                moveNode = (TreeNode)(e.Data.GetData(typeof(TreeNode)));
            }
            else
            {
                MessageBox.Show("error","ϵͳ��ʾ",MessageBoxButtons.OK ,MessageBoxIcon.Information );
            }
            if (moveNode.Level < 3)
                return;

            //�����������ȷ��Ҫ�ƶ�����Ŀ��ڵ�  
            System.Drawing.Point pt;
            TreeNode targeNode;
            pt = ((TreeView)(sender)).PointToClient(new System.Drawing.Point(e.X, e.Y));
            targeNode = MapDocTree.GetNodeAt(pt);
            if (targeNode.Level < 2)
                return;
            if (targeNode.Level == 2)
            {
                if (!moveNode.Parent.Parent.Equals(targeNode.Parent))
                    return;
            }
            else
            {
                if (!moveNode.Parent.Parent.Equals(targeNode.Parent.Parent))
                    return;
            }
            IMap pMap = axMapControl.Map;
            string GroupLayerName = moveNode.Parent.Parent.Text;
            IGroupLayer pGroupLayer = GetGroupLayerByName(GroupLayerName) as IGroupLayer ;
            //���Ŀ��ڵ����ӽڵ������Ϊͬ���ڵ�,��֮��ӵ��¼��ڵ��δ��  
            TreeNode NewMoveNode = (TreeNode)moveNode.Clone();
            int newindex, oldindex; ILayer pmovelayer;
            oldindex = GetLayerIndexofTreeNode(moveNode);  //�Ͻڵ���map�е�������
            newindex = GetLayerIndexofTreeNode(targeNode); //�½ڵ���map�е�������
            pmovelayer = GetLayerofTreeNode(moveNode);
            int newnodeindex, oldnodeindex;
            oldnodeindex = GetAbsoluteIndexofNode(moveNode);
            newnodeindex = GetAbsoluteIndexofNode(targeNode);
            IMapLayers pmaplayers = axMapControl.Map as IMapLayers;
            TreeNode tmpnode;
            switch (targeNode.Level)
            {
                case 3:     //���Ŀ��ڵ���Ҷ�ӽڵ�
                    if (targeNode.Parent.Parent != moveNode.Parent.Parent)
                        return;
                    if (oldindex < newindex)
                    {
                        targeNode.Parent.Nodes.Insert(targeNode.Index + 1, NewMoveNode);
                    }
                    else
                    {
                        targeNode.Parent.Nodes.Insert(targeNode.Index, NewMoveNode);
                    }
                    moveNode.Remove();      //ɾ���ɽڵ�

                    //axMapControl.MoveLayerTo(oldindex, newindex);   //�޸�ͼ��˳��
                    
                    pmaplayers.MoveLayerEx(pGroupLayer, pGroupLayer, pmovelayer, newindex);
                    break;
                case 2:   //���Ŀ��ڵ���2���ڵ�
                    if (targeNode.Nodes.Count == 0)   //����ø��ڵ�û���ӽڵ�
                    {
                        tmpnode = GetprevLeafNode(targeNode);
                        if (tmpnode != null)
                        {
                            newindex = GetLayerIndexofTreeNode(tmpnode);
                            if (newindex < oldindex)
                                newindex = newindex + 1;
                        }
                        else
                        {
                            newindex = 0;
                        }
                    }
                    else  //����ø��ڵ����ӽڵ�
                    {
                        newindex = GetLayerIndexofTreeNode(targeNode.LastNode);
                        if (newindex < oldindex)
                            newindex = newindex + 1;
                    }

                    targeNode.Nodes.Insert(targeNode.Nodes.Count, NewMoveNode); //����½ڵ�
                    moveNode.Remove();          //ɾ���ɽڵ�
                    //axMapControl.MoveLayerTo(oldindex, newindex);       //�޸�ͼ��˳��
                    pmaplayers.MoveLayerEx(pGroupLayer, pGroupLayer, pmovelayer, newindex);
                    break;


            }
            //}

            //���µ�ǰ�϶��Ľڵ�ѡ��  
            MapDocTree.SelectedNode = NewMoveNode;

            //չ��Ŀ��ڵ�,������ʾ�Ϸ�Ч��  
            targeNode.Expand();
            //�Ƴ��ϷŵĽڵ�  

            //ˢ�µ�ǰ��ͼ
            axMapControl.ActiveView.Refresh();

        }
        //���������ڵ����ڵ���һ��Ҷ�ӽڵ�
        private TreeNode GetnextLeafNode(TreeNode pnode)
        {
            const int leafLevel = 3;
            if (pnode.NextNode == null)
                if (pnode.Level > 0)
                    return GetnextLeafNode(pnode.Parent);
                else
                    return null;
            switch (pnode.Level)
            {
                case leafLevel://3
                    if (pnode.NextNode != null)
                        return pnode.NextNode;
                    break;
                case leafLevel - 1://2
                    if (pnode.NextNode != null)
                        if (pnode.NextNode.Nodes.Count > 0)
                            return pnode.NextNode.FirstNode;
                        else
                            return GetnextLeafNode(pnode.NextNode);
                    break;
                case leafLevel - 2://1
                    if (pnode.NextNode != null)
                    {
                        if (pnode.NextNode.Nodes.Count > 0)
                            if (pnode.NextNode.FirstNode.Nodes.Count > 0)
                                return pnode.NextNode.FirstNode.FirstNode;
                            else
                                return GetnextLeafNode(pnode.NextNode.FirstNode);
                        else
                            return GetnextLeafNode(pnode.NextNode);

                    }
                    break;
                case 0:
                    return null;

            }
            return null;
        }
        //���������ڵ����ڵ���һ��Ҷ�ӽڵ�
        private TreeNode GetprevLeafNode(TreeNode pnode)
        {
            const int leafLevel = 3;
            if (pnode.PrevNode == null)
                if (pnode.Level > 0)
                    return GetprevLeafNode(pnode.Parent);
                else
                    return null;
            switch (pnode.Level)
            {
                case leafLevel://3
                    if (pnode.PrevNode != null)
                        return pnode.PrevNode;
                    break;
                case leafLevel - 1://2
                    if (pnode.PrevNode != null)
                        if (pnode.PrevNode.Nodes.Count > 0)
                            return pnode.PrevNode.LastNode;
                        else
                            return GetprevLeafNode(pnode.PrevNode);
                    break;
                case leafLevel - 2://1
                    if (pnode.PrevNode != null)
                    {
                        if (pnode.PrevNode.Nodes.Count > 0)
                            if (pnode.PrevNode.LastNode.Nodes.Count > 0)
                                return pnode.PrevNode.LastNode.LastNode;
                            else
                                return GetprevLeafNode(pnode.PrevNode.LastNode);
                        else
                            return GetprevLeafNode(pnode.PrevNode);

                    }
                    break;
                case 0:
                    return null;

            }
            return null;
        }
        private void MapDocTree_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(TreeNode)))
            {
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void MapDocTree_ItemDrag(object sender, ItemDragEventArgs e)
        {

            if (e.Button == MouseButtons.Left)
            {
                DoDragDrop(e.Item, DragDropEffects.Move);
            }

        }

        private void MapDocTree_DragOver(object sender, DragEventArgs e)
        {
            //�������ͣ�� TreeView �ؼ���ʱ��չ���ÿؼ��е� TreeNode   
            TreeNode moveNode = null;
            if (e.Data.GetDataPresent(typeof(TreeNode)))
            {
                moveNode = (TreeNode)(e.Data.GetData(typeof(TreeNode)));
            }
            else
            {
                MessageBox.Show("error","ϵͳ��ʾ",MessageBoxButtons.OK ,MessageBoxIcon.Information );
            }
            if (moveNode.Level != 3)
                return;

            System.Drawing.Point p = this.MapDocTree.PointToClient(new System.Drawing.Point(e.X, e.Y));

            TreeNode tn = this.MapDocTree.GetNodeAt(p);

            //�����Ϸ�Ŀ��TreeNode�ı���ɫ   


            if (m_LastDragNode != null && tn != m_LastDragNode)
            {
                m_LastDragNode.BackColor = SystemColors.Window;//ȡ����һ�������õĽڵ������ʾ   

                m_LastDragNode.ForeColor = SystemColors.WindowText;
            }
            else
            {
                moveNode.BackColor = SystemColors.Window;//ȡ����һ�������õĽڵ������ʾ   

                moveNode.ForeColor = SystemColors.WindowText;
            }

            if (m_LastDragNode != tn)
            {
                tn.BackColor = SystemColors.Highlight;

                tn.ForeColor = SystemColors.HighlightText;

            }
            m_LastDragNode = tn;

        }


        private void axMapControl_OnExtentUpdated(object sender, IMapControlEvents2_OnExtentUpdatedEvent e)
        {
            if (ModFrameData.v_AppGisUpdate.ScaleBoxList.Count > 0)
            {
                foreach (ComboBoxItem item in ModFrameData.v_AppGisUpdate.ScaleBoxList)
                {
                    if (item.Name.Equals("scale"))
                    {
                        item.ComboBoxEx.Text = "1:" + axMapControl.Map.MapScale.ToString("0.00");
                        break;
                    }
                }
            }
        }
        private int GetAbsoluteIndexofNode(TreeNode node)
        {
            if (node.Level != 3)
                return -1;
            int ind0, ind1, ind2, ind3; //��������
            int tmpind0, tmpind1, tmpind2, tmpind3;
            ind0 = node.Parent.Parent.Parent.Index;
            ind1 = node.Parent.Parent.Index;
            ind2 = node.Parent.Index;
            ind3 = node.Index;
            TreeNode node0;
            node0 = node.Parent.Parent.Parent;
            int AbsoluteIndex = 0;
            foreach (TreeNode node1 in node0.Nodes)
            {
                if (node1.Index >= ind1)
                    continue;
                foreach (TreeNode node2 in node1.Nodes)
                {
                    //AbsoluteIndex = AbsoluteIndex + node2.Nodes.Count;
                    foreach (TreeNode node3 in node2.Nodes)
                    {
                        AbsoluteIndex = AbsoluteIndex + 1;
                    }
                }

            }
            TreeNode node01;
            node01 = node.Parent.Parent;
            foreach (TreeNode node2 in node01.Nodes)
            {
                if (node2.Index >= ind2)
                    continue;
                //AbsoluteIndex = AbsoluteIndex + node2.Nodes.Count;
                foreach (TreeNode node3 in node2.Nodes)
                {
                    AbsoluteIndex = AbsoluteIndex + 1;
                }
            }
            TreeNode node02 = node.Parent;
            foreach (TreeNode node3 in node02.Nodes)
            {
                if (node3.Index >= ind3)
                    continue;
                AbsoluteIndex = AbsoluteIndex + 1;
            }
            //AbsoluteIndex = AbsoluteIndex + ind3;

            return AbsoluteIndex;

        }

        //==============�ĵ�Ŀ¼���===============================
        //�ĵ�Ŀ¼��
        private void TextDoctree_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Level == 2)
            {
                string strsouce = e.Node.Parent.Name;
                string strdir = e.Node.Name;
                string strExp = "";
                GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
                // string mypath = dIndex.GetDbValue("dbServerPath");
                string mypath = dIndex.GetDbInfo();
                GeoDataCenterDbFun db = new GeoDataCenterDbFun();
                string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���

                if (strsouce != "")
                {
                    strExp = "select ·�� from �ĵ�����Դ��Ϣ�� where ����Ŀ¼��='" + strsouce + "'";
                    strsouce = db.GetInfoFromMdbByExp(strCon, strExp);

                    docPath = strsouce + "\\" + strdir;
             //       RichBoxWordDoc.Clear();
                    this.Cursor = Cursors.WaitCursor;
                    GetWordApp(wordProcess);//��ȡ�ͻ������е�word����
                    try
                    {
                        WordToRtf(@docPath);//Source����Ϊ�����ݿ��ȡ��word�Ķ�����������
                 //       RichBoxWordDoc.LoadFile(Application.StartupPath + "\\temp.rtf");//richtextbox�ؼ�����temp.rtf

                //        this.CenterTabControl.SelectedTab = DocPage;
                        Cursor = Cursors.Default;
                    }
                    catch
                    {

                        Cursor = Cursors.Default;
                    }
                }
            }

        }

     

        //������ǰר������
        private void MenuItemExport_Click(object sender, EventArgs e)
        {
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            string mypath = dIndex.GetDbInfo();
            string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
            GeoDataCenterDbFun db = new GeoDataCenterDbFun();

            TreeNode tTypeNode = DataUnitTree.SelectedNode;
            string strExp = "select ר������ from ��׼ר����Ϣ�� where ����= '" + tTypeNode.Text + "'";
            string strtype = db.GetInfoFromMdbByExp(strCon, strExp);//ר������
            strExp = "select �������� from ���ݵ�Ԫ�� where ��������='" + tTypeNode.Parent.Text + "' and ���ݵ�Ԫ����='3'";
            string strarea = db.GetInfoFromMdbByExp(strCon, strExp);//��������
            strExp = "select * from ��ͼ�����Ϣ�� where ר������='" + strtype + "' and ��������='" + strarea + "'";
            DataTable dt = db.GetDataTableFromMdb(strCon, strExp);

            strExp = "select �ֶ����� from ͼ�����������";
            string strname = db.GetInfoFromMdbByExp(strCon, strExp);
            string[] arrName = strname.Split('+');//�����ֶ�����
            List<string> namelist = new List<string>();
            for (int j = 0; j < dt.Rows.Count; j++)
            {
                string[] layers = dt.Rows[j]["ͼ�����"].ToString().Split('/');
                for (int k = 0; k < layers.Length; k++)
                {
                    string layer = layers[k];
                    if (layer != "")//ͼ����ɲ�Ϊ��
                    {
                        strExp = "select ҵ��������,ҵ��С����� from ��׼ͼ����Ϣ�� where ����='" + layer + "'";
                        DataTable dt2 = db.GetDataTableFromMdb(strCon, strExp);
                        if (dt2.Rows.Count > 0)//��ͼ�����ڱ�׼ͼ���
                        {
                            string strBig = dt2.Rows[0]["ҵ��������"].ToString();
                            string strSub = dt2.Rows[0]["ҵ��С�����"].ToString();
                            strname = "";
                            for (int i = 0; i < arrName.Length; i++)
                            {
                                switch (arrName[i])
                                {
                                    case "ҵ��������":
                                        strname += strBig;
                                        break;
                                    case "���":
                                        strname += dt.Rows[j]["���"].ToString();
                                        break;
                                    case "ҵ��С�����":
                                        strname += strSub;
                                        break;
                                    case "��������":
                                        strname += dt.Rows[j]["��������"].ToString();
                                        break;
                                    case "������":
                                        strname += dt.Rows[j]["������"].ToString();
                                        break;
                                }
                            }
                            strname += layer;
                            namelist.Add(strname);
                        }
                    }
                }
            }
            if (namelist.Count == 0)
            {
                MessageBox.Show("û��ͼ����Ե���!", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            //��ȡģ��·��
            string sourcefilename = Application.StartupPath + "\\..\\Template\\DATATEMPLATE.mdb";
            SysCommon.CProgress vProgress = new SysCommon.CProgress("���ڵ�������,���Ժ�");
            try
            {
                if (File.Exists(sourcefilename))//ԭģ�����
                {
                    SaveFileDialog dlg = new SaveFileDialog();
                    dlg.Filter = "MDB����|*.mdb";
                    dlg.OverwritePrompt = false;
                    dlg.Title = "���浽MDB";
                    DialogResult result = MessageBox.Show("�����Ƿ�ȥ��ǰ׺��", "��ʾ", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        
                        vProgress.EnableCancel = false;
                        vProgress.ShowDescription = false;
                        vProgress.FakeProgress = true;
                        vProgress.TopMost = true;
                        vProgress.ShowProgress();
                       
                        //�������mdb,�滻�ļ�������ģ�嵽ָ��·��
                        //�������mdb�����滻����׷�ӵ�����ļ�
                        File.Copy(sourcefilename, dlg.FileName, true);
                        IWorkspaceFactory Pwf = new AccessWorkspaceFactoryClass();
                        IWorkspace pws = (IWorkspace)(Pwf.OpenFromFile(dlg.FileName, 0));
                        IWorkspace2 pws2 = (IWorkspace2)pws;
                        for (int i = 0; i < namelist.Count; i++)
                        {
                            //��workspace�л�ȡ��Ӧ��layer
                            IWorkspace  pWorkspace=GetWorkspaceByFileName(namelist[i]) as IWorkspace;
                            string cellvalue = namelist[i];
                            if (result == DialogResult.Yes) cellvalue = cellvalue.Substring(15);//ȥ��ǰ׺
                            if (pws2.get_NameExists(esriDatasetType.esriDTFeatureClass, cellvalue))
                            {
                                IFeatureClass tmpfeatureclass;
                                IFeatureWorkspace pFeatureWorkspace = (IFeatureWorkspace)pws;
                                tmpfeatureclass = pFeatureWorkspace.OpenFeatureClass(cellvalue);
                                IDataset set = tmpfeatureclass as IDataset;
                                set.CanDelete();
                                set.Delete();
                            }
                            if(pWorkspace!=null)//�ռ��Ƿ����
                            IFeatureDataConverter_ConvertFeatureClass(pWorkspace, pws, namelist[i], cellvalue);
                        }
                        vProgress.Close();
                        MessageBox.Show("�����ɹ���", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LogFile log = new LogFile(tipRichBox, m_strLogFilePath);
                        if (log != null)
                        {
                            log.Writelog("����MDB���ݵ�" + dlg.FileName);
                        }
                    }
                    
                }
            }
            catch
            {
                vProgress.Close();
                MessageBox.Show("������������!", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }
        /// <summary>
        /// ��һ��Ҫ�����һ�������ռ�ת�Ƶ�����һ�������ռ�
        /// ע��Ŀ�깤���ռ䲻���и�Ҫ���࣬���������  
        /// </summary>
        /// <param name="sourceWorkspace">Դ�����ռ�</param>
        /// <param name="targetWorkspace">Ŀ�깤���ռ�</param>
        /// <param name="nameOfSourceFeatureClass">ԴҪ������</param>
        /// <param name="nameOfTargetFeatureClass">Ŀ��Ҫ������</param>
        public void IFeatureDataConverter_ConvertFeatureClass(IWorkspace sourceWorkspace, IWorkspace targetWorkspace, string nameOfSourceFeatureClass, string nameOfTargetFeatureClass)
        {
            //create source workspace name   
            IDataset sourceWorkspaceDataset = (IDataset)sourceWorkspace;
            IWorkspaceName sourceWorkspaceName = (IWorkspaceName)sourceWorkspaceDataset.FullName;
            //create source dataset name   
            IFeatureClassName sourceFeatureClassName = new FeatureClassNameClass();
            IDatasetName sourceDatasetName = (IDatasetName)sourceFeatureClassName;
            sourceDatasetName.WorkspaceName = sourceWorkspaceName;
            sourceDatasetName.Name = nameOfSourceFeatureClass;
            //create target workspace name   
            IDataset targetWorkspaceDataset = (IDataset)targetWorkspace;
            IWorkspaceName targetWorkspaceName = (IWorkspaceName)targetWorkspaceDataset.FullName;
            //create target dataset name   
            IFeatureClassName targetFeatureClassName = new FeatureClassNameClass();
            IDatasetName targetDatasetName = (IDatasetName)targetFeatureClassName;
            targetDatasetName.WorkspaceName = targetWorkspaceName;
            targetDatasetName.Name = nameOfTargetFeatureClass;
            //Open input Featureclass to get field definitions.   
            ESRI.ArcGIS.esriSystem.IName sourceName = (ESRI.ArcGIS.esriSystem.IName)sourceFeatureClassName;
            IFeatureClass sourceFeatureClass = (IFeatureClass)sourceName.Open();
            //Validate the field names because you are converting between different workspace types.   
            IFieldChecker fieldChecker = new FieldCheckerClass();
            IFields targetFeatureClassFields;
            IFields sourceFeatureClassFields = sourceFeatureClass.Fields;
            IEnumFieldError enumFieldError;
            // Most importantly set the input and validate workspaces!     
            fieldChecker.InputWorkspace = sourceWorkspace;
            fieldChecker.ValidateWorkspace = targetWorkspace;
            fieldChecker.Validate(sourceFeatureClassFields, out enumFieldError, out targetFeatureClassFields);
            // Loop through the output fields to find the geomerty field   
            IField geometryField;
            for (int i = 0; i < targetFeatureClassFields.FieldCount; i++)
            {
                if (targetFeatureClassFields.get_Field(i).Type == esriFieldType.esriFieldTypeGeometry)
                {
                    geometryField = targetFeatureClassFields.get_Field(i);
                    // Get the geometry field's geometry defenition            
                    IGeometryDef geometryDef = geometryField.GeometryDef;
                    //Give the geometry definition a spatial index grid count and grid size        
                    IGeometryDefEdit targetFCGeoDefEdit = (IGeometryDefEdit)geometryDef;
                    targetFCGeoDefEdit.GridCount_2 = 1;
                    targetFCGeoDefEdit.set_GridSize(0, 0);
                    //Allow ArcGIS to determine a valid grid size for the data loaded      
                    targetFCGeoDefEdit.SpatialReference_2 = geometryField.GeometryDef.SpatialReference;
                    // we want to convert all of the features   
                    IQueryFilter queryFilter = new QueryFilterClass();
                    queryFilter.WhereClause = "";
                    // Load the feature class     
                    IFeatureDataConverter fctofc = new FeatureDataConverterClass();
                    IEnumInvalidObject enumErrors = fctofc.ConvertFeatureClass(sourceFeatureClassName, queryFilter, null, targetFeatureClassName, geometryDef, targetFeatureClassFields, "", 1000, 0);
                    break;
                }
            }
        }

        private void OpenFile_Click(object sender, EventArgs e)
        {
            if (treeViewOutPutResults.SelectedNode == null)
                return;
            if (treeViewOutPutResults.SelectedNode.Level <2)
                return;
            string filepath = treeViewOutPutResults.SelectedNode.Name;
            switch (treeViewOutPutResults.SelectedNode.Level.ToString())
            {
                case "2":
                    if (Directory.Exists(filepath))
                    {
                        System.Diagnostics.Process.Start("explorer.exe", filepath);
                        //��¼��־
                        LogFile log = new LogFile(tipRichBox, m_strLogFilePath);
                        string strLog = "���ļ���'" + treeViewOutPutResults.SelectedNode.Text + "'";
                        if (log != null)
                        {
                            log.Writelog(strLog);
                        }
                    }
                    break;
                case "3":
                    if (File.Exists(filepath)  )
                    {
                        if (filepath.Substring(filepath.Length - 3, 3).ToLower().Equals( "cel"))
                        {
                            FormFlexcell frm = new FormFlexcell();
                            frm.OpenFlexcellFile(filepath);
                            //��¼��־
                            LogFile log = new LogFile(tipRichBox, m_strLogFilePath);
                            string strLog = "���ļ�'" + treeViewOutPutResults.SelectedNode.Text + "'";
                            if (log != null)
                            {
                                log.Writelog(strLog);
                            }
                            frm.Show();
                        }
                        if (filepath.Substring(filepath.Length - 3, 3).ToLower().Equals( "mxd"))
                        {
                            //��¼��־
                            LogFile log = new LogFile(tipRichBox, m_strLogFilePath);
                            string strLog = "���ļ�'" + treeViewOutPutResults.SelectedNode.Text + "'";
                            if (log != null)
                            {
                                log.Writelog(strLog);
                            }
                        // ?   GeoPageLayout.FrmPageLayout fmPageLayout = new GeoPageLayout.FrmPageLayout(filepath);
                         //?   fmPageLayout.ShowDialog();
                        }


                    }
                    break;
                default:
                    break;

            }

        }

        private void DeleteFile_Click(object sender, EventArgs e)
        {
            try
            {
                if (treeViewOutPutResults.SelectedNode == null)
                    return;
                if (treeViewOutPutResults.SelectedNode.Level < 2)
                    return;
                string filepath = treeViewOutPutResults.SelectedNode.Name;
                switch (treeViewOutPutResults.SelectedNode.Level.ToString())
                {
                    case "2":
                        if (Directory.Exists(filepath))
                        {
                            Directory.Delete(filepath, true);
                            //��¼��־
                            LogFile log = new LogFile(tipRichBox, m_strLogFilePath);
                            string strLog = "ɾ���ļ���'" + treeViewOutPutResults.SelectedNode.Text + "'";
                            if (log != null)
                            {
                                log.Writelog(strLog);
                            }
                            treeViewOutPutResults.SelectedNode.Remove();
                        }
                        break;
                    case "3":
                        if (File.Exists(filepath))
                        {
                            File.Delete(filepath);
                            //��¼��־
                            LogFile log = new LogFile(tipRichBox, m_strLogFilePath);
                            string strLog = "ɾ���ļ�'" + treeViewOutPutResults.SelectedNode.Text + "'";
                            if (log != null)
                            {
                                log.Writelog(strLog);
                            }
                            treeViewOutPutResults.SelectedNode.Remove();
                        }
                        break;
                    default:
                        break;

                }
            }
            catch
            { 
            }

        }

        private void LocateFile_Click(object sender, EventArgs e)
        {
            if (treeViewOutPutResults.SelectedNode == null)
                return;
            if (treeViewOutPutResults.SelectedNode.Level <2)
                return;
            string filepath = treeViewOutPutResults.SelectedNode.Name;
            switch (treeViewOutPutResults.SelectedNode.Level.ToString())
            {
                case "2":
                    if (Directory.Exists(filepath))
                    {
                        System.Diagnostics.Process.Start("explorer.exe", filepath);
                        //��¼��־
                        LogFile log = new LogFile(tipRichBox, m_strLogFilePath);
                        string strLog = "��λ���ļ���'" + treeViewOutPutResults.SelectedNode.Text + "'";
                        if (log != null)
                        {
                            log.Writelog(strLog);
                        }
                    }
                    break;
                case "3":
                    string dirpath = filepath.Substring(0, filepath.LastIndexOf("\\"));
                    if (File.Exists(filepath))
                    {
                        System.Diagnostics.Process.Start("explorer.exe", dirpath);
                        //��¼��־
                        LogFile log = new LogFile(tipRichBox, m_strLogFilePath);
                        string strLog = "��λ���ļ�'" + treeViewOutPutResults.SelectedNode.Text + "'";
                        if (log != null)
                        {
                            log.Writelog(strLog);
                        }
                    }
                    break;
                default:
                    break;

            }

        }
        //������������������
        private void MenuItemLoadDataByUnit_Click(object sender, EventArgs e)
        {
            XzqLoad=true;
            int count = 0;
            foreach (TreeNode node in DataUnitTree.SelectedNode.Nodes)//�õ�ר��ڵ�
            {
               
                foreach (TreeNode childnode in node.Nodes)//�õ�ר��ڵ��ӽڵ�
                {
                    count++;
                        if (m_tparent==null||MapDocTree.Nodes.Count==0)
                        {
                            m_selectnode = childnode;
                            MenuItemLoadData_Click(sender, e);//����

                        }
                        else
                        {
                            m_selectnode = childnode;
                            MenuItemAddLoadData_Click(sender, e);//׷�ӵ���
                        }
                }
            }
            XzqLoad = false;
            if (count > 0)
            {
                //��������ת����ͼ�ĵ�����
                IndextabControl.SelectedTab = PageMapDoc;

                //��ͼ������ת��ͼ���������
              //  CenterTabControl.SelectedTab = MapPage;
            }
            

        }
        //�������ܣ�ѡ��ͼ������   ���������������  �����������
        public void SelectJudge(CProgress vProgress)
        {
            //�жϵ�ǰ�����Ƿ����
            if (m_CurEditTopicNode == null)
                return;
            vProgress.SetProgress("�ӵ�ǰר���ȡ����");
            //��ȡ��ǰר�������
            string strSuffixx = m_CurEditTopicNode.Text;

            ILayer bhtbLayer = GetLayerByName(strSuffixx, "�仯ͼ��");
            //��ȡ����ͼ��ͼ��
            ILayer pcbpLayer = GetLayerByName(strSuffixx, "���α�������");
            //��ȡ��;ͼ��ͼ��
            ILayer tdytLayer = GetLayerByName(strSuffixx, "ɭ����;");
            //��ȡ����ͼ��ͼ��
            ILayer dltbLayer = GetLayerByName(strSuffixx, "����ͼ��");
            //ִ��ѡ��ͼ�����й���������
            ChangeJudge.DoJudgeBySelect(bhtbLayer as IFeatureLayer, pcbpLayer as IFeatureLayer, tdytLayer as IFeatureLayer, dltbLayer as IFeatureLayer, vProgress);
            //treeViewOutPutResults.Nodes.Clear();
            InitOutPutResultTree();
        }
        //����ר������ͼ������ȡͼ��
        private ILayer GetLayerByName(string GroupName, string LayerName)
        {
            if (GroupName.Equals("")) return null;
            if (LayerName.Equals("")) return null;

            Exception exError = null;

            IMap pMap = axMapControl.Map;

            for (int n = 0; n < pMap.LayerCount; n++)
            {
                ILayer layer = pMap.get_Layer(n);
                if (layer is IGroupLayer && layer.Name == GroupName)//ͨ���ȶ��ҵ�ר���Ӧͼ����
                {
                    ICompositeLayer Comlayer = layer as ICompositeLayer;//��һ��������Ĳ����ת����һ����ϲ㣬ʹ�����Ա���
                    for (int c = 0; c < Comlayer.Count; c++)
                    {
                        IFeatureLayer  pFeatureLayer = Comlayer.get_Layer(c) as IFeatureLayer;
                        if (pFeatureLayer == null)
                            continue;
                        if (pFeatureLayer.FeatureClass == null)
                            continue;
                        if (pFeatureLayer.Name == LayerName)    //��ͼ�����ڣ�ͨ���ȶ��ҵ�ͼ��
                        {
                            return pFeatureLayer as ILayer;
                        }
                    }
                }
            }
            return null;
        }
        //����ר������ȡͼ����
        private ILayer GetGroupLayerByName(string GroupName)
        {
            if (GroupName.Equals("")) return null;

            Exception exError = null;

            IMap pMap = axMapControl.Map;

            for (int n = 0; n < pMap.LayerCount; n++)
            {
                ILayer layer = pMap.get_Layer(n);
                if (layer is IGroupLayer && layer.Name == GroupName)//ͨ���ȶ��ҵ�ר���Ӧͼ����
                {
                    return layer;
                }
            }
            return null;
        }
        private void tabControlRight_Click(object sender, EventArgs e)
        {

        }

        private void CenterTabControl_Click(object sender, EventArgs e)
        {

        }

        private void axMapControl_OnMouseMove(object sender, IMapControlEvents2_OnMouseMoveEvent e)
        {
            string xStr = e.mapX.ToString("0.00");
            string yStr = e.mapY.ToString("0.00");
            TempX = double.Parse(xStr);
            TempY = double.Parse(yStr);
            ModFrameData.v_AppGisUpdate.CoorXY = "X:" + xStr.PadRight(14, ' ') + "   Y:" + yStr.PadRight(14, ' ');
          //  ModFrameData.v_QueryResult.Hide();
        }


        //
        private void MenuItemAddlayer_Click(object sender, EventArgs e)
        {
            string strWorkFile = Application.StartupPath + "\\..\\Temp\\CurPrj.xml";
            //�ж��ļ��Ƿ����
            XmlDocument xmldoc = new XmlDocument();
            if (!File.Exists(strWorkFile))
                return;
            xmldoc.Load(strWorkFile);
            TreeNode node = MapDocTree.SelectedNode;
            frmAddNewFile frm = new frmAddNewFile();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                string strfollow = frm.strPath.Substring(frm.strPath.LastIndexOf(".") + 1).ToLower();
                //����XML
                string strSearchRoot = "//SubGroup[@sItemName='" + node.Parent.Text + "']";
                XmlNode xmlNodeRoot = xmldoc.SelectSingleNode(strSearchRoot);
                string strSearchnode = "//Layer[@sItemName='" + frm.strName + "']";
                XmlNode xmlCheck = xmldoc.SelectSingleNode(strSearchnode);
                if (xmlCheck != null)
                {
                    MessageBox.Show("ͼ�������Ѵ��ڣ�����������", "ϵͳ��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                XmlElement xmlElentRoot = (XmlElement)xmlNodeRoot;
                XmlElement xmlElemt = xmldoc.CreateElement("Layer");
                xmlElemt.SetAttribute("sDemo", frm.strDescri);
                xmlElemt.SetAttribute("sItemName", frm.strName);
                if (strfollow == "mdb")
                {
                    xmlElemt.SetAttribute("sFile", frm.strPath + "\\" + frm.strFeauture);
                    xmlElemt.SetAttribute("sNewLoad", "mdb");
                }
                else
                {
                    xmlElemt.SetAttribute("sFile", frm.strPath);
                    xmlElemt.SetAttribute("sNewLoad", "shp");
                }

                xmlElentRoot.AppendChild(xmlElemt);
                xmldoc.Save(strWorkFile);

                //������
                TreeNode Newnode = new TreeNode();
                Newnode.Text = frm.strDescri;
                Newnode.Name = frm.strName;
                Newnode.Checked = true;
                node.Parent.Nodes.Add(Newnode);

                //���µ�ͼ
                ILayer pLayer;
                try
                {
                    if (strfollow == "shp")
                    {
                        int Index = frm.strPath.LastIndexOf("\\");
                        string filePath = frm.strPath.Substring(0, Index);
                        string fileName = frm.strPath.Substring(Index + 1);
                        //�򿪹����ռ䲢���shp�ļ�
                        IWorkspaceFactory pWorkspaceFactory = new ShapefileWorkspaceFactoryClass();
                        //ע��˴���·���ǲ��ܴ��ļ�����
                        IFeatureWorkspace pFeatureWorkspace = (IFeatureWorkspace)pWorkspaceFactory.OpenFromFile(filePath, 0);
                        IFeatureLayer pFeatureLayer = new FeatureLayerClass();
                        //ע��������ļ����ǲ��ܴ�·����
                        pFeatureLayer.FeatureClass = pFeatureWorkspace.OpenFeatureClass(fileName);
                        pFeatureLayer.Name = frm.strName;
                        pLayer = pFeatureLayer as ILayer;
                    }
                    else
                    {
                        //��personGeodatabase,�����ͼ��
                        IWorkspaceFactory pAccessWorkspaceFactory = new AccessWorkspaceFactoryClass();
                        //�򿪹����ռ䲢�������ݼ�
                        IWorkspace pWorkspace = pAccessWorkspaceFactory.OpenFromFile(frm.strPath, 0);
                        IFeatureWorkspace pFeatureWorkspace = pWorkspace as IFeatureWorkspace;
                        IFeatureLayer pFeatureLayer = new FeatureLayerClass();
                        pFeatureLayer.FeatureClass = pFeatureWorkspace.OpenFeatureClass(frm.strFeauture);
                        pFeatureLayer.Name = frm.strName;
                        pLayer = pFeatureLayer as ILayer;

                    }

                    IGroupLayer pGroupFLayer = GetGroupLayerByName(node.Parent.Parent.Text) as IGroupLayer;
                    IMapLayers pmaplayers = axMapControl.Map as IMapLayers;
                    ICompositeLayer Comlayer = pGroupFLayer as ICompositeLayer;
                    pmaplayers.InsertLayerInGroup(pGroupFLayer, pLayer, false, Comlayer.Count);
                    Newnode.ImageIndex = 19;
                    Newnode.SelectedImageIndex = 19;
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message, "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Newnode.ImageIndex = 14;
                    Newnode.SelectedImageIndex = 14;
                }
                
            }
        }

        private void treeViewOutPutResults_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (treeViewOutPutResults.SelectedNode != e.Node)
            {
                treeViewOutPutResults.SelectedNode = e.Node;
            }
        }

        private void MenuItemStatSum_Click(object sender, EventArgs e)
        {
            IFeatureLayer pFeatureLayer;
            TreeNode tSelNode = MapDocTree.SelectedNode;
            pFeatureLayer = GetLayerofTreeNode(tSelNode) as IFeatureLayer;
            if (pFeatureLayer != null)
            {
                FormStatCustomize frm = new FormStatCustomize();
                frm.InitForm(this.axMapControl.Map, pFeatureLayer);
                frm.ShowDialog();
            }
        }

        private void RefreshResult_Click(object sender, EventArgs e)
        {
            this.InitOutPutResultTree();
        }

        private void MapDocTree_ControlRemoved(object sender, ControlEventArgs e)
        {

        }

        private void treeViewXZQ_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (treeViewXZQ.SelectedNode != e.Node)
            {
                if (treeViewXZQ.SelectedNode != null)
                {
                    treeViewXZQ.SelectedNode.ForeColor = Color.Black;
                }

                treeViewXZQ.SelectedNode = e.Node;
                e.Node.ForeColor = Color.Red;

                string strItemName = treeViewXZQ.SelectedNode.Name;
                string strItemText = treeViewXZQ.SelectedNode.Text;

           
                treeViewXZQ.Refresh();

                //���ݵ�ѡ�ڵ� ���� AxMapControl ����
                //?     UpdateMapControl(strItemName,strItemText);

                if (treeViewXZQ.SelectedNode.Level.Equals(2))//ֻ��Դ缶�ڵ�
                {
                    treeViewXZQ.SelectedNode.ExpandAll();//չ�����������ڵ�
                }
            }

            //�Ҽ������ʱ�򵯳��Ҽ��˵�
            if (e.Button == MouseButtons.Right)
            {

                System.Drawing.Point ClickPoint = treeViewXZQ.PointToScreen(new System.Drawing.Point(e.X, e.Y));
                TreeNode tSelNode;
                tSelNode = e.Node;
                if (tSelNode != null)
                {

                    if (tSelNode.Level == 2)//�������ڵ�
                        XZQcontextMenu.Show(ClickPoint);
                   

                }
            }
        }

        private void jmpXZQ_Click(object sender, EventArgs e)
        {
            if (treeViewXZQ.SelectedNode == null)
                return;
            IGeometry mGeometry = null;
            TreeNode county = treeViewXZQ.SelectedNode;
            TreeNode[] layers=MapDocTree.Nodes.Find("������",true);
            if (layers.Length == 0)
                return;
            TreeNode layer = layers[0];

            IFeatureClass mFC = (GetLayerofTreeNode(layer) as IFeatureLayer).FeatureClass;

            IFeatureCursor mFCs = mFC.Search(null, false);
            IFeature mF = mFCs.NextFeature();
            int fdXZQMC = mFC.Fields.FindField("XZQMC");
            int fdXZQDM = mFC.Fields.FindField("XZQDM");
            while (mF != null)
            {
                try
                {
                    if (mF.get_Value(fdXZQDM).ToString() == county.Name)
                    {
                        MGeometry=mF.ShapeCopy;
                        break;
                    }
                 
                }
                catch
                {

                }
                mF = mFCs.NextFeature();
            }
            ///ZQ 20111020 ��λ��Χ����1.5
            IEnvelope pExtent = MGeometry.Envelope;
            SysCommon.ModPublicFun.ResizeEnvelope(pExtent, 1.5);
            axMapControl.ActiveView.Extent = pExtent;
            //drawgeometryXOR(MGeometry as IPolygon, axMapControl.ActiveView.ScreenDisplay);
            axMapControl.ActiveView.Refresh();
            Application.DoEvents();
            
        }
        //��mapcontrol�ϻ������
        private void drawgeometryXOR(IPolygon pPolygon, IScreenDisplay pScreenDisplay)
        {
            if (pPolygon == null) 
                return;
            ISimpleFillSymbol pFillSymbol = new SimpleFillSymbolClass();
            ISimpleLineSymbol pLineSymbol = new SimpleLineSymbolClass();

            try
            {
                //��ɫ����
                IRgbColor pRGBColor = new RgbColorClass();
                pRGBColor.UseWindowsDithering = false;
                ISymbol pSymbol = (ISymbol)pFillSymbol;
                //pSymbol.ROP2 = esriRasterOpCode.esriROPNotXOrPen;

                pRGBColor.Red = 255;
                pRGBColor.Green = 170;
                pRGBColor.Blue = 0;
                pLineSymbol.Color = pRGBColor;

                pLineSymbol.Width = 0.8;
                pLineSymbol.Style = esriSimpleLineStyle.esriSLSSolid;
                pFillSymbol.Outline = pLineSymbol;

                pFillSymbol.Color = pRGBColor;
                pFillSymbol.Style = esriSimpleFillStyle.esriSFSDiagonalCross;

                pScreenDisplay.StartDrawing(pScreenDisplay.hDC, -1);  //esriScreenCache.esriNoScreenCache -1
                pScreenDisplay.SetSymbol(pSymbol);

                //�������ѻ����Ķ����
                if (pPolygon != null)
                {
                    pScreenDisplay.DrawPolygon(pPolygon);
                    //m_Polygon = pPolygon;
                }
                //�����ѻ����Ķ����
                //else
                //{
                //    if (m_Polygon != null)
                //    {
                //        pScreenDisplay.DrawPolygon(m_Polygon);
                //    }
                //}

                pScreenDisplay.FinishDrawing();
            }
            catch (Exception ex)
            {
                MessageBox.Show("���Ʒ�Χ����:" + ex.Message, "ϵͳ��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                pFillSymbol = null;
            }
        }

        private void drawgeometryXOR1(IPolygon pPolygon, IScreenDisplay pScreenDisplay)
        {
            if (pPolygon == null)
                return;
            ISimpleFillSymbol pFillSymbol = new SimpleFillSymbolClass();
            ISimpleLineSymbol pLineSymbol = new SimpleLineSymbolClass();

            try
            {
                //��ɫ����
                IRgbColor pRGBColor = new RgbColorClass();
                pRGBColor.UseWindowsDithering = false;
                ISymbol pSymbol = (ISymbol)pFillSymbol;
                //pSymbol.ROP2 = esriRasterOpCode.esriROPNotXOrPen;

                pRGBColor.Red = 255;
                pRGBColor.Green = 170;
                pRGBColor.Blue = 0;
                pLineSymbol.Color = pRGBColor;

                pLineSymbol.Width = 0.8;
                pLineSymbol.Style = esriSimpleLineStyle.esriSLSSolid;
                pFillSymbol.Outline = pLineSymbol;

                pFillSymbol.Color = pRGBColor;
                pFillSymbol.Style = esriSimpleFillStyle.esriSFSDiagonalCross;

                //pScreenDisplay.StartDrawing(pScreenDisplay.hDC, -1);  //esriScreenCache.esriNoScreenCache -1
                pScreenDisplay.SetSymbol(pSymbol);

                //�������ѻ����Ķ����
                if (pPolygon != null)
                {
                    pScreenDisplay.DrawPolygon(pPolygon);
                    //m_Polygon = pPolygon;
                }
                //�����ѻ����Ķ����
                //else
                //{
                //    if (m_Polygon != null)
                //    {
                //        pScreenDisplay.DrawPolygon(m_Polygon);
                //    }
                //}

                //pScreenDisplay.FinishDrawing();
            }
            catch (Exception ex)
            {
                MessageBox.Show("���Ʒ�Χ����:" + ex.Message, "ϵͳ��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                pFillSymbol = null;
            }
        }

        private void treeViewJHTable_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (treeViewJHTable.SelectedNode != e.Node)
            {
                if (treeViewJHTable.SelectedNode != null)
                {
                    treeViewJHTable.SelectedNode.ForeColor = Color.Black;
                }

                treeViewJHTable.SelectedNode = e.Node;
                e.Node.ForeColor = Color.Red;

                string strItemName = treeViewJHTable.SelectedNode.Name;
                string strItemText = treeViewJHTable.SelectedNode.Text;


                treeViewJHTable.Refresh();

                //���ݵ�ѡ�ڵ� ���� AxMapControl ����
                //?     UpdateMapControl(strItemName,strItemText);

                //if (treeViewJHTable.SelectedNode.Level.Equals(1))//ֻ��Դ缶�ڵ�
                //{
                //    treeViewJHTable.SelectedNode.ExpandAll();//չ�����������ڵ�
                //}
            }

            //�Ҽ������ʱ�򵯳��Ҽ��˵�
            if (e.Button == MouseButtons.Right)
            {

                System.Drawing.Point ClickPoint = treeViewJHTable.PointToScreen(new System.Drawing.Point(e.X, e.Y));
                TreeNode tSelNode;
                tSelNode = e.Node;
                if (tSelNode != null)
                {

                    if (tSelNode.Level == 1)//�������ڵ�
                        JHTBcontextMenu.Show(ClickPoint);


                }
            }
           
        }

        private void JHTBjmpStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (treeViewJHTable.SelectedNode == null)
                return;
            IGeometry mGeometry = null;
            TreeNode tfh = treeViewJHTable.SelectedNode;
            TreeNode[] layers = MapDocTree.Nodes.Find("�Ӻ�ͼ��", true);
            if (layers.Length == 0)
                return;
            TreeNode layer = layers[0];

            IFeatureClass mFC = (GetLayerofTreeNode(layer) as IFeatureLayer).FeatureClass;

            IFeatureCursor mFCs = mFC.Search(null, false);
            IFeature mF = mFCs.NextFeature();
            int NEWMAPNO = mFC.Fields.FindField("NEWMAPNO");
            //int fdXZQDM = mFC.Fields.FindField("XZQDM");
            while (mF != null)
            {
                try
                {
                    if (mF.get_Value(NEWMAPNO).ToString() == tfh.Name)
                    {
                        MGeometry = mF.ShapeCopy;
                        break;
                    }

                }
                catch
                {

                }
                mF = mFCs.NextFeature();
            }
            ///ZQ 20111020 ��λ��Χ����1.5��
            IEnvelope pExtent = MGeometry.Envelope;
            SysCommon.ModPublicFun.ResizeEnvelope(pExtent, 1.5);
            axMapControl.ActiveView.Extent = pExtent;
            //drawgeometryXOR(MGeometry as IPolygon, axMapControl.ActiveView.ScreenDisplay);
            axMapControl.ActiveView.Refresh();
            Application.DoEvents();
            

        }
        private void SetControl_mapctrl_afterdraw(IDisplay Display, esriViewDrawPhase phase)
        {

            if (phase == esriViewDrawPhase.esriViewForeground) drawgeometryXOR1(MGeometry as IPolygon, axMapControl.ActiveView.ScreenDisplay);
        }

        private void cancelXZQMenuItem_Click(object sender, EventArgs e)
        {
            MGeometry = null;
            axMapControl.ActiveView.Refresh();
        }

        private void cancelJHTBMenuItem_Click(object sender, EventArgs e)
        {
            MGeometry = null;
            axMapControl.ActiveView.Refresh();
        }

        private void axMapControl_OnViewRefreshed(object sender, IMapControlEvents2_OnViewRefreshedEvent e)
        {
            //drawgeometryXOR(MGeometry as IPolygon, axMapControl.ActiveView.ScreenDisplay);

        }


    }       
    
}