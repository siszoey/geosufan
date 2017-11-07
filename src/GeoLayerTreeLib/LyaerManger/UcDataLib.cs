using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Xml;

using DevComponents.AdvTree;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Controls;

using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;
using SysCommon.Gis;


namespace GeoLayerTreeLib.LayerManager
{

    public partial class UcDataLib : UserControl
    {
        //��ǰMap���Ѿ���ӵ�ͼ��
        private  IDictionary<string, ILayer> _DicAddLyrs = new Dictionary<string, ILayer>();
        //��ǰMap����Ӻ���ɾ����ͼ��
        private  IDictionary<string, ILayer> _DicDelLyrs = new Dictionary<string, ILayer>();
        //added by chulili��ǰMap���Ѿ���ӵ�ͼ����
        private IDictionary<string, IGroupLayer> _DicAddGroupLyrs = new Dictionary<string, IGroupLayer>();
        //added by chulili ��ǰMap����Ӻ���ɾ����ͼ����
        private IDictionary<string, IGroupLayer> _DicDelGroupLyrs = new Dictionary<string, IGroupLayer>();
        private IDictionary<string, object > _LayerDicOfGroupLayer = new Dictionary<string,object>();
        private bool _isLayerVisible = true;//����ͼ��Ŀ¼ʱ���ж���ͼ�Ƿ��������ͼ������������Ϊfalse����Ϊtrue
        private DevComponents.DotNetBar.ContextMenuBar _MapContextMenu = null;
        private DevComponents.DotNetBar.ContextMenuBar _LayerContextMenu = null;
        private DevComponents.DotNetBar.ContextMenuBar _LayerTreeManageContextMenu = null;


        //added by chulili 20111119 Ϊ�������ͼ����ӵı���
        private DevComponents.AdvTree.Node _CopyLayerNode = null;
        //end added by chulili 20111119
        //added by chulili 20111119 �Ƿ����ճ��ͼ��
        public bool CanPasteLayer()
        {
            if (_CopyLayerNode == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public AdvTree DataTree//yjl20110915 add for pagelayout
        {
            get { return TreeDataLib; }
        }
        public bool LayerVisible
        {
            set { _isLayerVisible = value; }
        }
        public DevComponents.DotNetBar.ContextMenuBar MapContextMenu
        {
            set { _MapContextMenu = value; }
        }
        public DevComponents.DotNetBar.ContextMenuBar LayerContextMenu
        {
            set { _LayerContextMenu = value; }
        }
        public DevComponents.DotNetBar.ContextMenuBar LayerTreeManageContextMenu
        {
            set { _LayerTreeManageContextMenu = value; }
        }
        public IDictionary<string, ILayer> DicAddLyrs
        {
            set { _DicAddLyrs = value; }
        }
        public IDictionary<string, ILayer> DicDelLyrs
        {
            set { _DicDelLyrs = value; }
        }
        //ZQ  20110827   add   �ж�ͼ��Ŀ¼���Ƿ����ק
        public bool isDragDrop
        {
            set { this.TreeDataLib.DragDropEnabled = value; }
        }
        //
        public UcDataLib()
        {
            InitializeComponent();
            InitUC();
        }
        private bool _isLayerConfig=false;//�Ƿ�������ͼ��Ŀ¼��Ĭ�Ϸ����ã�������չʾ��ϵͳ
        public bool isLayerConfig
        {
            set { _isLayerConfig = value; }
        }

        //��ǰMap
        private IMapControl2 m_pMapCtl;
        public IMapControl2 CurMap
        {
            set { m_pMapCtl = value; }
        }
        private ITOCControlDefault _TocControl;
        public ITOCControlDefault TocControl
        {
            set { _TocControl = value; }
        }
        //��ǰToc
        //private ITocControl m_pToc;
        //public ITocControl CurToc
        //{
        //    set { m_pToc = value; }
        //}

        private IWorkspace m_pWks;
        public IWorkspace CurWks
        {
            set { m_pWks = value; }
        }
        private String _LayerXmlPath;
        public String LayerXmlPath
        {
            set { _LayerXmlPath = value; }
        }
        //changed by chulili �Ƴ�����m_vDataViewXml����ʹ��xmlȫ·����ʾ����
        //private XmlDocument m_vDataViewXml;
        //public XmlDocument DataViewXml
        //{
        //    set { m_vDataViewXml = value; }
        //    get { return m_vDataViewXml; }
        //}

        public Dictionary<string, DevComponents.DotNetBar.ContextMenuBar> DicMenu
        {
            get{ return m_vDicMenu;}
            set { m_vDicMenu = value; }
        }

        //�Ҽ��˵�����
        private Dictionary<string, DevComponents.DotNetBar.ContextMenuBar> m_vDicMenu = null;
        private void BoundMenu()
        {

        }
        
        private void InitUC()
        {
            

            this.TreeDataLib.Nodes.Clear();
            this.TreeDataLib.ImageList = this.ImageList;
            

            //
        }

        public void InitDataView()
        {
            //if (m_vDataViewXml == null) return;
            //changed by chulili �޸��жϷ�ʽ���Ƴ�����m_vDataViewXml
            if (!System.IO.File.Exists(_LayerXmlPath)) return;
        }
        //��ʼ��ͼ�����ֵ䣨Ӣ��������������ӳ�䣩
        public void InitLayerDic()
        {
           SysCommon.ModField.InitLayerNameDic(Plugin.ModuleCommon.TmpWorkSpace, SysCommon.ModField._DicLayerName);
            
         }
        #region ��ʼ����ͼ����
        public void InitDBsource()
        {
            ModuleMap._DicDataLibWks.Clear();
            //���xml
            ModuleMap.InitDBSourceDic(Plugin.ModuleCommon.TmpWorkSpace, ModuleMap._DicDataLibWks);
        }
        public void LoadDataByMap(XmlDocument pDataXmlDoc, IMap pMXDMap)
        {
            if (pMXDMap == null) { return; }
            //��ʼ��Ŀ¼�ڵ�
            LoadTreeByXml(pDataXmlDoc, TreeDataLib, true, false);
            IObjectCopy pOC = null;
            if (pMXDMap.LayerCount > 0)
            {
                for (int x = pMXDMap.LayerCount - 1; x >= 0; x--)
                {
                    try
                    {
                        pOC = new ObjectCopyClass();
                        ILayer pLayer = pOC.Copy(pMXDMap.get_Layer(x)) as ILayer;
                        ILayerGeneralProperties pLayerGenPro = pLayer as ILayerGeneralProperties;
                        //��ȡͼ�������
                        string strNodeXml = pLayerGenPro.LayerDescription;
                        XmlDocument pXmlDoc = new XmlDocument();
                        pXmlDoc.LoadXml(strNodeXml);
                        //��ȡ�ڵ��NodeKey��Ϣ
                        XmlNode pxmlnode = pXmlDoc.SelectSingleNode("//Layer");
                        if (pxmlnode == null)
                        {
                            continue;
                        }
                        string strNodeKey = pxmlnode.Attributes["NodeKey"].Value.ToString();
                        //����xml�ڵ㣬����NodeKey�ڽڵ����ѯ
                        string strSearch = "//Layer[@NodeKey=" + "'" + strNodeKey + "'" + "]";
                        XmlNode pNode = pDataXmlDoc.SelectSingleNode(strSearch);
                        if (pNode != null && pLayer.Valid && pLayer.Visible)
                        {
                            m_pMapCtl.Map.AddLayer(pLayer);
                        }
                    }
                    catch { }
                }
                //ѭ������map�е�ͼ��
                for (int i = m_pMapCtl.Map.LayerCount - 1; i >= 0; i--)
                {
                    ILayer pLayer = m_pMapCtl.Map.get_Layer(i);
                    OpenMxdDocDealLayer(pLayer);
                }
            }
        }
        public void LoadDataByMxd(XmlDocument pXmldoc, string strLayerTreeMxdPath)
        {
            
            if (System.IO.File.Exists(strLayerTreeMxdPath)) //�ж��ļ��Ƿ����
            {
                //��ʼ��Ŀ¼�ڵ�
                LoadTreeByXml(pXmldoc, TreeDataLib, true, false);
                if (m_pMapCtl.CheckMxFile(strLayerTreeMxdPath)) //�ļ��Ƿ�����ȷ��
                {
                    //m_pMapCtl.LoadMxFile(strLayerTreeMxdPath, "", "");  //�����ļ�
                    IMapDocument pMapdoc = new MapDocumentClass();
                    pMapdoc.Open(strLayerTreeMxdPath, "");
                    IObjectCopy pOC = new ObjectCopyClass();
                    if (pMapdoc.MapCount > 0)
                    {
                        IMap pMap = pMapdoc.get_Map(0);
                        m_pMapCtl.Map = pOC.Copy(pMap) as IMap;
                    }
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(pMapdoc);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(pOC);
                    pMapdoc = null;
                    pOC = null;
                    //��Ŀ¼�ڵ�״̬����ͬ��
                    //��սڵ��״̬
                    //for (int i = 0; i < this.TreeDataLib.Nodes.Count; i++)
                    //{
                    //    DevComponents.AdvTree.Node pNode = TreeDataLib.Nodes[i];
                    //    UnSelectAllAdvNode(pNode);
                    //}

                    //ѭ������map�е�ͼ��
                    for (int i = m_pMapCtl.Map.LayerCount-1; i>=0; i--)
                    {
                        ILayer pLayer = m_pMapCtl.Map.get_Layer(i);
                        OpenMxdDocDealLayer(pLayer);
                    }
                }
            }           
        }
        public void RefreshDataByMap(XmlDocument pDoc,IMap pMap)
        {
            _DicAddLyrs.Clear();
            _DicDelLyrs.Clear();
            _DicAddGroupLyrs.Clear();
            _DicDelGroupLyrs.Clear();
            _LayerDicOfGroupLayer.Clear();
            m_pMapCtl.ClearLayers();
            LoadDataByMap(pDoc, pMap);
        }
        public void LoadData()
        {
            _DicAddLyrs.Clear();            
            _DicDelLyrs.Clear();
            _DicAddGroupLyrs.Clear();
            _DicDelGroupLyrs.Clear();
            _LayerDicOfGroupLayer.Clear();
            m_pMapCtl.ClearLayers();
            //m_vDicMenu = ModData.v_AppGisUpdate.DicContextMenu;
            
            XmlDocument xmldoc = new XmlDocument();
            if (!System.IO.File.Exists(_LayerXmlPath)) return;
            xmldoc.Load(_LayerXmlPath);

            bool bReadMxd = false;
            string LayerTreeMxdPath = Application.StartupPath + "\\..\\Template\\չʾͼ����.mxd";
            if (!_isLayerConfig)    //չʾϵͳ���ö�ȡMXD��ģʽ������ϵͳ���ö�ȡXML��ģʽ chulili 20111105
            {
                bReadMxd = SysCommon.ModSysSetting.CopySysSettingtoFile(Plugin.ModuleCommon.TmpWorkSpace, "��ʼ���ص�ͼ�ĵ�", LayerTreeMxdPath);
            }
            if (bReadMxd)   //��ȡͼ��Ŀ¼��MXD�ļ��ɹ�
            {
                LoadDataByMxd(xmldoc, LayerTreeMxdPath);
            }
            else
            {
                ModuleMap.ChangeLableEngine2MapLex(m_pMapCtl as IMapControlDefault);
                if (_TocControl != null)
                {
                    _TocControl.SetBuddyControl(null);//yjl20111102 add ��������ǰ ���TOC
                }
                m_pMapCtl.ActiveView.Deactivate();  //ȡ��ˢ�� added by chulili 20111102
                LoadTreeByXml(xmldoc, this.TreeDataLib, true,true);
                ModuleMap.LayersComposeEx(m_pMapCtl as IMapControlDefault);

                m_pMapCtl.ActiveView.Activate(m_pMapCtl.hWnd);  //����ˢ�� added by chulili 20111102
                if (_TocControl != null)
                {
                    _TocControl.SetBuddyControl(m_pMapCtl);//yjl20111102 add �������ݺ� ��TOC��200ͼ��Ч�ʿ�1����
                }
                //added by chulili 20110628ͼ�������ˢ��һ��toc�ؼ� 
                if (_TocControl != null)
                {
                    _TocControl.Update();
                }
                //ICommand _cmd = new ESRI.ArcGIS.Controls.ControlsMapFullExtentCommand();
                //_cmd.OnCreate(m_pMapCtl);
                //_cmd.OnClick();
                IEnvelope pEnv = SysCommon.ModSysSetting.GetFullExtent(m_pWks); //changed by chulili 20111107ʹ��ϵͳ��������ȫ��
                if (pEnv != null)
                {
                    m_pMapCtl.Extent = pEnv;
                }
                else
                {
                    ICommand _cmd = new ESRI.ArcGIS.Controls.ControlsMapFullExtentCommand();
                    _cmd.OnCreate(m_pMapCtl);
                    _cmd.OnClick();
                    _cmd = null;
                }
                pEnv = null;
            }
            
            //�˵���
            BoundMenu();
            //��д�����ⲿ���ݴ�������ͼ�¼�
            if (!_isLayerConfig)
            {
                IMap pMap = m_pMapCtl.Map;//����ͼ
                IActiveViewEvents_Event pAE = pMap as IActiveViewEvents_Event;
                pAE.ItemAdded += new IActiveViewEvents_ItemAddedEventHandler(this.OnLayerAdded);
            }
        }
        private void OnLayerAdded(object item)
        {
            ILayer pLayer;
            pLayer = item as ILayer;
            ILayerGeneralProperties pLayerGenPro = pLayer as ILayerGeneralProperties;
            //��ȡ��ͼ���������Ϣ��ת��xml�ڵ�
            string strNodeXml = pLayerGenPro.LayerDescription;
            if (!strNodeXml.Contains("NodeKey"))
            {//this.TreeDataLib
                //�������趨ͼ��ڵ�
                DevComponents.AdvTree.Node treenodeOutLayer = new DevComponents.AdvTree.Node();
                treenodeOutLayer.Name = "OutLayer";
                treenodeOutLayer.Text = pLayer.Name;
                treenodeOutLayer.Tag = "OutLayer";
                treenodeOutLayer.DataKey = pLayer as object;
                treenodeOutLayer.Expanded = true;
                if (pLayer is IFeatureLayer)
                {
                    IFeatureLayer pFeaLayer = pLayer as IFeatureLayer;
                    if (pFeaLayer != null)
                    {
                        if (pFeaLayer.FeatureClass != null)
                        {
                            if (pFeaLayer.FeatureClass.FeatureType == esriFeatureType.esriFTAnnotation)
                                treenodeOutLayer.Image = this.ImageList.Images["_annotation"];
                            else if(pFeaLayer.FeatureClass.FeatureType==esriFeatureType.esriFTDimension)
                                treenodeOutLayer.Image = this.ImageList.Images["_Dimension"];
                            else
                            {
                                switch (pFeaLayer.FeatureClass.ShapeType)
                                {
                                    case esriGeometryType.esriGeometryPoint:
                                        treenodeOutLayer.Image = this.ImageList.Images["_point"];
                                        break;
                                    case esriGeometryType.esriGeometryPolyline:
                                        treenodeOutLayer.Image = this.ImageList.Images["_line"];
                                        break;
                                    case esriGeometryType.esriGeometryPolygon:
                                        treenodeOutLayer.Image = this.ImageList.Images["_polygon"];
                                        break;
                                    case esriGeometryType.esriGeometryMultiPatch:
                                        treenodeOutLayer.Image = this.ImageList.Images["_MultiPatch"];
                                        break;
                                    default:
                                        treenodeOutLayer.Image = this.ImageList.Images["Layer"];
                                        break;
                                }
                            }
                        }
                    }
                }
                else 
                {
                    treenodeOutLayer.Image = this.ImageList.Images["Layer"];
                }
                this.TreeDataLib.Nodes[0].Nodes.Add(treenodeOutLayer);
            }

        }
        private XmlDocument GetXmlDoc()
        {
            if (m_pWks == null) return null;

            SysCommon.Gis.SysGisTable systable = new SysCommon.Gis.SysGisTable(m_pWks);
            Exception ex = null;

            object objtemp = systable.GetFieldValue("SysSetting", "SettingValue2", "SettingName='DataViewXml'", out ex);
            systable = null;
            if (objtemp == null) return null ;

            return objtemp as XmlDocument;
        }
        //�۵�
        public void CollapseAllNode()
        {
            this.TreeDataLib.CollapseAll();
        }
        //չ��
        public void ExpendAllNode()
        {
            this.TreeDataLib.ExpandAll();
        }
        //�ƹ����Ĵ��� ��Ҫ��֤
        public void LoadTreeByXml(XmlDocument xmldoc,AdvTree Tree, bool IsInConfig,bool LoadData)
        {
            if (xmldoc == null) return;
            Tree.Nodes.Clear();

            //��ȡXml�ĸ��ڵ㲢��Ϊ���ڵ�ӵ�UltraTree��
            XmlNode xmlnodeRoot = xmldoc.DocumentElement;
            XmlElement xmlelementRoot = xmlnodeRoot as XmlElement;

            xmlelementRoot.SetAttribute("NodeKey", "Root");
            string sNodeText = xmlelementRoot.GetAttribute("NodeText");

            //�������趨���ĸ��ڵ�
            DevComponents.AdvTree.Node treenodeRoot = new DevComponents.AdvTree.Node();
            treenodeRoot.Name="Root";
            treenodeRoot.Text=sNodeText;

            if (IsInConfig)
            {
                //treenodeRoot.Override.NodeStyle = NodeStyle.CheckBox;

                if (xmlelementRoot.Attributes["IsCollection"] != null &&
                    xmlelementRoot.Attributes["IsCollection"].Value.ToLower() == "true" && LoadData)
                    treenodeRoot.CheckState = CheckState.Checked;
                else
                    treenodeRoot.CheckState = CheckState.Unchecked;
            }
            treenodeRoot.Tag = "Root";
            treenodeRoot.DataKey = xmlelementRoot;
            treenodeRoot.Expanded = true;
            Tree.Nodes.Add(treenodeRoot);

            //treenodeRoot.LeftImages.Add("Root");
            treenodeRoot.Image = treenodeRoot.TreeControl.ImageList.Images["Root"];
            LoadTreeNodeByXmlNode(treenodeRoot, xmlnodeRoot, IsInConfig, LoadData);
            //if (IsInConfig)
            //    LoadTreeNodeByXmlNode(treenodeRoot, xmlnodeRoot, IsInConfig);
            //else
            //    LoadTreeNodeByXmlNode(treenodeRoot, xmlnodeRoot, Tree, clsMain);
        }

        public void LoadTreeNodeByXmlNode(DevComponents.AdvTree.Node treenode, XmlNode xmlnode, bool IsInConfig,bool LoadData)
        {

            for (int iChildIndex = 0; iChildIndex < xmlnode.ChildNodes.Count; iChildIndex++)
            {
                XmlElement xmlElementChild = xmlnode.ChildNodes[iChildIndex] as XmlElement;
                if (xmlElementChild == null)
                {
                    continue;
                }
                if (IsInConfig)
                {
                    if (xmlElementChild.Attributes["Enabled"] == null ||
                        xmlElementChild.Attributes["Enabled"].Value.ToLower() == "false")
                        continue;
                }
                else
                {
                    if (xmlElementChild.Attributes["IsCollection"] == null ||
                        xmlElementChild.Attributes["IsCollection"].Value.ToLower() == "false")
                        continue;
                }

                //��Xml�ӽڵ��"NodeKey"��"NodeText"�������������ӽڵ�
                string sNodeKey = xmlElementChild.GetAttribute("NodeKey");
                if (!_isLayerConfig)
                {
                    if (!Plugin.ModuleCommon.AppUser.Name.Equals("admin"))
                    {
                        if (Plugin.ModuleCommon.ListUserdataPriID == null)//changed by xisheng 2011.06.29
                        {
                            continue;
                        }
                        if (!Plugin.ModuleCommon.ListUserdataPriID.Contains(sNodeKey))
                        {
                            continue;
                        }
                    }
                }
                string sNodeText = xmlElementChild.GetAttribute("NodeText");

                DevComponents.AdvTree.Node treenodeChild = new DevComponents.AdvTree.Node();
                treenodeChild.Name = sNodeKey;
                treenodeChild.Text = sNodeText;

                if (IsInConfig || xmlElementChild.Name == "DataDIR")
                {
                    //treenodeChild.Override.NodeStyle = NodeStyle.CheckBox;
                    treenodeChild.CheckBoxVisible = true;

                    if (xmlElementChild.Attributes["IsCollection"] != null &&
                    xmlElementChild.Attributes["IsCollection"].Value.ToLower() == "true" && LoadData)
                        treenodeChild.CheckState = CheckState.Checked;
                    else
                        treenodeChild.CheckState = CheckState.Unchecked;
                }
                
                treenodeChild.DataKey = xmlElementChild;
                treenodeChild.Tag = xmlElementChild.Name;
                if (xmlElementChild.Attributes["Expand"] != null &&
                    xmlElementChild.Attributes["Expand"].Value.ToLower() == "true")
                {
                    treenodeChild.Expanded = true;
                }

                treenode.Nodes.Add(treenodeChild);
                //changed by chulili 20110708ɾ���������ݼ��Ĵ��룬һ��ʼ�����أ�����ӽڵ���������
                //if (xmlElementChild.Name == "DataDIR")
                //{
                //    //������ڵ�
                //    IGroupLayer pGroupFLayer = new GroupLayer();                    
                //    XmlElement pele = xmlnode.ChildNodes[iChildIndex] as XmlElement ;

                //    pGroupFLayer.Name = sNodeText;
                //    _DicAddGroupLyrs.Add(sNodeKey, pGroupFLayer);
                //    pGroupFLayer.Visible = _isLayerVisible;
                //    m_pMapCtl.AddLayer(pGroupFLayer, m_pMapCtl.LayerCount);
                //}
                //�������� ������δ�����Ҫ���������
                bool NodeVisible = false;
                if (LoadData)
                {
                    #region ����ͼ��
                    if (xmlElementChild.Name == "Layer")
                    {

                        ILayer pLyr = null;
                        bool blnLoad = XmlOperation.GetBoolean(xmlnode.ChildNodes[iChildIndex], false, "Load");
                        if (blnLoad)
                        {
                            IGroupLayer pParentGroupLayer = null;
                            string sParentNodeKey = "";
                            XmlNode pParePareXmlnode = xmlnode.ParentNode;
                            XmlNode pDirNode = null;
                            XmlNode pGroupLayerXmlNode = null;
                            if (xmlnode.Name == "DataDIR" && pParePareXmlnode.Name == "DIR")
                            {
                                pDirNode = pParePareXmlnode;
                                pGroupLayerXmlNode = xmlnode;
                            }
                            else
                            {
                                pDirNode = pParePareXmlnode.ParentNode;
                                pGroupLayerXmlNode = pParePareXmlnode;
                            }
                            #region ͼ���鴦����ע�͵�
                            //if (pGroupLayerXmlNode.Name == "DataDIR")
                            //{
                            //    sParentNodeKey = pGroupLayerXmlNode.Attributes["NodeKey"].Value.ToString();
                            //    if (_DicAddGroupLyrs.Keys.Contains(sParentNodeKey))
                            //    {
                            //        pParentGroupLayer = _DicAddGroupLyrs[sParentNodeKey];
                            //    }
                            //    else
                            //    {
                            //       //������ڵ�
                            //        IGroupLayer pGroupFLayer = new GroupLayerClass ();
                            //        string sPareNodeText = pGroupLayerXmlNode.Attributes["NodeText"].Value.ToString();
                            //        //changed by chulili 20110728 ��������ݿ�����
                            //        if (pDirNode != null)
                            //        {
                            //            if (pDirNode.Name.Equals("DIR"))
                            //            {
                            //                sPareNodeText = sPareNodeText + "_" + pDirNode.Attributes["NodeText"].Value;
                            //            }
                            //        }
                            //        //end changed by chulili 20110728 ��������ݿ�����
                            //        pGroupFLayer.Name = sPareNodeText;
                            //        _DicAddGroupLyrs.Add(sParentNodeKey, pGroupFLayer);
                            //        pGroupFLayer.Visible = _isLayerVisible;
                            //        //added by chulili 20110728 ���ͼ������
                            //        ILayerGeneralProperties layerProerties = (ILayerGeneralProperties)pGroupFLayer;
                            //        layerProerties.LayerDescription = pGroupLayerXmlNode.OuterXml;
                            //        //end added by chulili
                            //        m_pMapCtl.AddLayer(pGroupFLayer, m_pMapCtl.LayerCount);
                            //        pParentGroupLayer = _DicAddGroupLyrs[sParentNodeKey];
                            //    }
                            //}
                            # endregion
                            Exception errLayer = null;
                            bool boolLoad = XmlOperation.GetBoolean(xmlnode.ChildNodes[iChildIndex], false, "Load");
                            if (boolLoad)
                            {
                                pLyr = ModuleMap.AddLayerFromXml(ModuleMap._DicDataLibWks, xmlnode.ChildNodes[iChildIndex], m_pWks, "", null, out errLayer);
                            } 
                                //���ص�Map��ȥ
                            
                            if (pLyr != null && boolLoad)
                            {
                                //�����ݼӵ�ͼ�㼯����
                                if (!_DicAddLyrs.Keys.Contains(sNodeKey))
                                {
                                    _DicAddLyrs.Add(sNodeKey, pLyr);
                                }

                                //added by chulili 2011-06-11����ͼ���Ƿ�ɼ�(Ĭ�Ͽɼ�������ͼ��Ŀ¼ʱ�������óɲ��ɼ�)
                                if (_isLayerConfig)
                                {
                                    pLyr.Visible = _isLayerVisible;
                                }
                                #region ͼ���鴦����ע�͵�
                                //if (pParentGroupLayer != null)
                                //{
                                //    IMapLayers pmaplayers = m_pMapCtl.Map as IMapLayers;
                                //    ICompositeLayer Comlayer = pParentGroupLayer as ICompositeLayer;
                                //    pmaplayers.InsertLayerInGroup(pParentGroupLayer, pLyr, false, Comlayer.Count);
                                //    IDictionary<string, ILayer> pDic = null;
                                //    if (_LayerDicOfGroupLayer.Keys.Contains(sParentNodeKey))
                                //    {
                                //        pDic = _LayerDicOfGroupLayer[sParentNodeKey] as IDictionary<string,ILayer >;
                                //        pDic.Add(sNodeKey, pLyr);
                                //    }
                                //    else
                                //    {
                                //        pDic = new Dictionary<string, ILayer >();
                                //        pDic.Add(sNodeKey, pLyr);
                                //        _LayerDicOfGroupLayer.Add(sParentNodeKey, pDic as object );
                                //    }
                                //}
                                //else
                                //{
                                #endregion
                                m_pMapCtl.AddLayer(pLyr, m_pMapCtl.LayerCount);
                                //}
                                m_pMapCtl.ActiveView.Refresh();
                                //�ı���ͼ״̬
                                treenodeChild.CheckState = CheckState.Checked;
                                NodeVisible =ModuleMap.GetVisibleOfLayer(m_pMapCtl.Map.MapScale, pLyr);
                            }
                        }
                    }
                    #endregion
                }

                if (LoadData)
                {
                    //�������ø�ѡ��
                    SetParentCheckState(treenodeChild);
                }

                //�ݹ�
                LoadTreeNodeByXmlNode(treenodeChild, xmlElementChild as XmlNode, IsInConfig, LoadData);
                if (LoadData)
                {
                    if (xmlElementChild.Name.CompareTo("DataDIR") == 0)
                    {
                        SetUltraTreeNodeImage(treenodeChild);
                    }
                }
                InitializeNodeImage(treenodeChild, NodeVisible);
            }

        }
        public void UpdateLayerNodeImage()
        {
            if (m_pMapCtl == null)
            {
                return;
            }
            for (int i = 0; i < m_pMapCtl.LayerCount; i++)
            {
                ILayer pLayer = m_pMapCtl.get_Layer(i);
                if (pLayer is IFeatureLayer || pLayer is IRasterLayer || pLayer is IRasterCatalog)
                {
                    ILayerGeneralProperties pLayerGenPro = pLayer as ILayerGeneralProperties;
                    //��ȡͼ���������ת��xml�ڵ�
                    string strNodeXml = pLayerGenPro.LayerDescription;

                    if (strNodeXml.Equals(""))
                    {
                        continue ;
                    }
                    XmlDocument pXmldoc = new XmlDocument();
                    pXmldoc.LoadXml(strNodeXml);
                    //��ȡ�ڵ��NodeKey��Ϣ
                    XmlNode pxmlnode = pXmldoc.SelectSingleNode("//Layer");
                    if (pxmlnode == null)
                    { 
                        continue;
                    }
                    string strNodeKey = pxmlnode.Attributes["NodeKey"].Value.ToString();
                    pXmldoc = null;
                    //��ȡͼ���Ӧ�Ľڵ�
                    DevComponents.AdvTree.Node pnode = SearchLayerNodebyName(strNodeKey);
                    //��ȡͼ���Ƿ������ɼ�
                    bool nodeVisible = ModuleMap.GetVisibleOfLayer(m_pMapCtl.Map.MapScale, pLayer);
                    //�޸�ͼ���Ӧ�ڵ��ͼ��
                    InitializeNodeImage(pnode, nodeVisible);
                }
            }
        }
    
        /// <summary>  
        /// �趨DataDIR�ڵ��ͼ�꣬ע��Ӹ�DataDIR�ڵ��𣬲��ÿ�������DataDIR�ڵ�
        /// �㷨������ӦDataDIR�µ�Layer����DataDIR�ڵ����ֵ���ӽڵ���Ŀ�Ƚ�
        /// �����Ϊȫ����������Ϊ�رգ�����Ϊ�뿪
        /// ��ֵ���㷨Ϊ����Ϊ��һ����Ϊ��һ���뿪����DtatDIR��Ϊ��
        /// </summary>
        /// <param name="nodeDataDIR"></param>
        private static void SetUltraTreeNodeImage(DevComponents.AdvTree.Node nodeDataDIR)
        {
            int iNumber = 0;
            foreach (DevComponents.AdvTree.Node nodeChildTree in nodeDataDIR.Nodes)
            {

                if (nodeChildTree.HasChildNodes == false)
                {
                    if (nodeChildTree.CheckBoxVisible ==true)
                    {
                        if (nodeChildTree.CheckState == CheckState.Checked)
                            iNumber++;  //�ӽڵ�ѡ�У���һ
                        else if (nodeChildTree.CheckState == CheckState.Unchecked)
                            iNumber--;  //�ӽڵ�δѡ�У���һ
                    }
                    else
                        iNumber++;  //�������,DataDIR�ڵ����ӽڵ�
                }
                else
                {
                    SetUltraTreeNodeImage(nodeChildTree);
                    if (("DataDIR&AllOpened").CompareTo(nodeChildTree.Tag) == 0)
                        iNumber++;          //��dataDIRȫ������һ
                    else if (("DataDIR&Closed").CompareTo(nodeChildTree.Tag) == 0)
                        iNumber--;          //��dataDIR�أ���һ
                    else
                        iNumber = iNumber + 0;  //��dataDIR�뿪������
                }
            }
            if (iNumber == nodeDataDIR.Nodes.Count)
            {
                nodeDataDIR.Tag = "DataDIR&AllOpened";
            }
            else if ((iNumber + nodeDataDIR.Nodes.Count) == 0)
            {
                nodeDataDIR.Tag = "DataDIR&Closed";
            }
            else
            {
                nodeDataDIR.Tag = "DataDIR&HalfOpened";
            }
        }
        //added by chulili 20111119 ��������
        public static void InitializeNodeImage(DevComponents.AdvTree.Node treenode)
        {
            InitializeNodeImage(treenode, false);
        }
        /// <summary>
        /// ͨ������ڵ��tag��ѡ���Ӧ��ͼ��        
        /// </summary>
        /// <param name="treenode"></param>
        /// changed by chulili 20111119 ���һ���������ýڵ��Ƿ�ɼ���������ͼ��ڵ㣩
        public static void InitializeNodeImage(DevComponents.AdvTree.Node treenode,bool NodeVisible)
        {
            switch (treenode.Tag.ToString())
            {
                case "Root":
                    treenode.Image = treenode.TreeControl.ImageList.Images["Root"];
                    treenode.CheckBoxVisible = false;
                    break;
                case "SDE":
                    treenode.Image = treenode.TreeControl.ImageList.Images["SDE"];
                    break;
                case "PDB":
                    treenode.Image = treenode.TreeControl.ImageList.Images["PDB"];
                    break;
                case "FD":
                    treenode.Image = treenode.TreeControl.ImageList.Images["FD"];
                    break;
                case "FC":
                    treenode.Image = treenode.TreeControl.ImageList.Images["FC"];
                    break;
                case "TA":
                    treenode.Image = treenode.TreeControl.ImageList.Images["TA"];
                    break;
                case "DIR":
                    treenode.Image = treenode.TreeControl.ImageList.Images["DIR"];
                    //treenode.CheckBoxVisible = false;
                    break;
                case "DataDIR":
                    treenode.Image = treenode.TreeControl.ImageList.Images["DataDIRClosed"];
                    break;
                case "DataDIR&AllOpened":
                    treenode.Image = treenode.TreeControl.ImageList.Images["DataDIROpen"];
                    break;
                case "DataDIR&Closed":
                    treenode.Image = treenode.TreeControl.ImageList.Images["DataDIRClosed"];
                    break;
                case "DataDIR&HalfOpened":
                    treenode.Image = treenode.TreeControl.ImageList.Images["DataDIRHalfOpen"];
                    break;
                case "Layer":
                    XmlNode xmlnodeChild = (XmlNode)treenode.DataKey;
                    if (xmlnodeChild != null && xmlnodeChild.Attributes["FeatureType"] != null)
                    {
                        string strFeatureType = xmlnodeChild.Attributes["FeatureType"].Value;
                        if (NodeVisible)
                        {
                            switch (strFeatureType)
                            {
                                case "esriGeometryPoint":
                                    treenode.Image = treenode.TreeControl.ImageList.Images["point_v"];
                                    break;
                                case "esriGeometryPolyline":
                                    treenode.Image = treenode.TreeControl.ImageList.Images["line_v"];
                                    break;
                                case "esriGeometryPolygon":
                                    treenode.Image = treenode.TreeControl.ImageList.Images["polygon_v"];
                                    break;
                                case "esriFTAnnotation":
                                    treenode.Image = treenode.TreeControl.ImageList.Images["annotation_v"];
                                    break;
                                case "esriFTDimension":
                                    treenode.Image = treenode.TreeControl.ImageList.Images["dimension_v"];
                                    break;
                                case "esriGeometryMultiPatch":
                                    treenode.Image = treenode.TreeControl.ImageList.Images["multipatch_v"];
                                    break;
                                default:
                                    treenode.Image = treenode.TreeControl.ImageList.Images["layer_v"];
                                    break;
                            }
                        }
                        else
                        {
                            switch (strFeatureType)
                            {
                                case "esriGeometryPoint":
                                    treenode.Image = treenode.TreeControl.ImageList.Images["point"];
                                    break;
                                case "esriGeometryPolyline":
                                    treenode.Image = treenode.TreeControl.ImageList.Images["line"];
                                    break;
                                case "esriGeometryPolygon":
                                    treenode.Image = treenode.TreeControl.ImageList.Images["polygon"];
                                    break;
                                case "esriFTAnnotation":
                                    treenode.Image = treenode.TreeControl.ImageList.Images["annotation"];
                                    break;
                                case "esriFTDimension":
                                    treenode.Image = treenode.TreeControl.ImageList.Images["dimension"];
                                    break;
                                case "esriGeometryMultiPatch":
                                    treenode.Image = treenode.TreeControl.ImageList.Images["multipatch"];
                                    break;
                                default:
                                    treenode.Image = treenode.TreeControl.ImageList.Images["layer"];
                                    break;
                            }
                        }
                    }
                    else
                    {
                        treenode.Image = treenode.TreeControl.ImageList.Images["Layer"];
                    }
                    break;
                case "RC":
                    if (NodeVisible)
                    {
                        treenode.Image = treenode.TreeControl.ImageList.Images["layer_v"];//RC->Layer  û������ΪRC��ͼƬ
                    }
                    else
                    {
                        treenode.Image = treenode.TreeControl.ImageList.Images["layer"];//RC->Layer  û������ΪRC��ͼƬ
                    }
                    break;
                case "RD":
                    if (NodeVisible)
                    {
                        treenode.Image = treenode.TreeControl.ImageList.Images["layer_v"];//RD->Layer  û������ΪRD��ͼƬ
                    }
                    else
                    {
                        treenode.Image = treenode.TreeControl.ImageList.Images["layer"];//RD->Layer  û������ΪRD��ͼƬ
                    }
                    break;
                case "SubType":
                    treenode.Image = treenode.TreeControl.ImageList.Images["SubType"];
                    break;
                default:
                    break;
            }//end switch
        }
        #endregion

        private void TreeDataLib_NodeClick(object sender, TreeNodeMouseEventArgs e)
        {
            DevComponents.AdvTree.Node vNode = e.Node as DevComponents.AdvTree.Node;
            TreeDataLib.SelectedNode = e.Node;
            if (e.Button == MouseButtons.Right)
            {
                if (_isLayerConfig)//�����������Ŀ¼�����������Ҽ��˵�
                {
                    return;
                }
                PopMenu(e.Node);
            }
            else if (e.Button == MouseButtons.Left)
            {
                
            }
        }

        //�����Ҽ��˵�
        private void PopMenu(DevComponents.AdvTree.Node vNode)
        {
            if (m_vDicMenu == null) return;
            DevComponents.DotNetBar.ContextMenuBar tempBar = null;
            switch (vNode.Tag.ToString())
            {
                case "Root":
                    m_vDicMenu.TryGetValue("ContextMenuDataViewTreeRoot", out tempBar);
                    break;
                case "Layer":
                    break;
            }

            if (tempBar == null) return;
            if (tempBar.Items.Count < 1) return;

            //this.Controls.Add(tempBar);
            //vNode.ContextMenu = this.contextMenuBar1;
            //this.contextMenuBar1.Show();
        }

        //�Ƴ�ĳ��ͼ�㣨ͨ���û��޸�ͼ����Ϣʱ���Ƚ���ͼ���Ƴ����ٽ��޸ĺ����ͼ����룩
        public void RemoveLayer(DevComponents.AdvTree.Node pNode)
        {
            if (pNode == null)
            {
                return;
            }
            if (!pNode.Tag.ToString().Equals("Layer"))
            {
                return;
            }
            XmlNode layerNode = pNode.DataKey as XmlNode;
            if (layerNode == null)
            {
                return;
            }
            string nodeKey = layerNode.Attributes["NodeKey"].Value;

            if (_DicDelLyrs.ContainsKey(nodeKey))
                _DicDelLyrs.Remove(nodeKey);

            if (_DicAddLyrs.ContainsKey(nodeKey))
            {
                
                ILayer removeLayer = _DicAddLyrs[nodeKey];
                m_pMapCtl.Map.DeleteLayer(removeLayer);
                _DicAddLyrs.Remove(nodeKey);
            }


        }
        //�Ƴ�ĳ��ͼ����
        private void RemoveDataDIRfromMap(DevComponents.AdvTree.Node pNode)
        {
            if (pNode == null)
            {
                return;
            }
            if (!pNode.Tag.ToString().Contains("DataDIR"))
            {
                return;
            }
            XmlNode layerNode = pNode.DataKey as XmlNode;
            if (layerNode == null)
            {
                return;
            }
            string nodeKey = layerNode.Attributes["NodeKey"].Value;

            if (_DicDelGroupLyrs.ContainsKey(nodeKey))
                _DicDelGroupLyrs.Remove(nodeKey);

            if (_DicAddGroupLyrs.ContainsKey(nodeKey))
            {

                ILayer removeLayer = _DicAddGroupLyrs[nodeKey];
                m_pMapCtl.Map.DeleteLayer(removeLayer);
                _DicAddGroupLyrs.Remove(nodeKey);
            }


        }
        //����ͼ�������ӻ�ж��ͼ��
        private void AddOrDelLyr(DevComponents.AdvTree.Node checkNode, bool blnChecked)
        {
            //added by chulili 20110722 ��ӽ�����
            SysCommon.CProgress vProgress = null;

            XmlNode layerNode = checkNode.DataKey as XmlNode;
            XmlElement Layerele = layerNode as XmlElement;
            XmlDocument pXmldoc = null;
            //added by chulili 20110708 ��ȡ����xml�����ļ�
            pXmldoc = new XmlDocument();
            pXmldoc.Load(_LayerXmlPath);
            //add end
            string nodeKey = layerNode.Attributes["NodeKey"].Value;
            //added by chulili 20110708 ������xml�ļ����ҵ���ǰͼ��ڵ�
            string strSearch = "//" + layerNode.Name + "[@NodeKey='" + nodeKey + "']";
            XmlNode pdocLayerNode = pXmldoc.SelectSingleNode(strSearch);

            IGroupLayer pParentGroupLayer = null;
            ICompositeLayer Comlayer = null;
            XmlNode GroupNode = layerNode.ParentNode;
            IDictionary<string, ILayer> pDic = null;
            string sParentNodeKey = "";
            //if (GroupNode.Name == "DataDIR")
            //{
            //    XmlElement pParentele = GroupNode as XmlElement;
            //    sParentNodeKey = pParentele.GetAttribute("NodeKey");
            //    if (_DicAddGroupLyrs.Keys.Contains(sParentNodeKey))
            //    {
            //        pParentGroupLayer = _DicAddGroupLyrs[sParentNodeKey];
            //    }
            //    else
            //    {
            //        if (_DicDelGroupLyrs.Keys.Contains(sParentNodeKey))
            //        {
            //            pParentGroupLayer = _DicDelGroupLyrs[sParentNodeKey];
            //        }
            //        else
            //        {
            //            if (blnChecked)
            //            {
            //                //������ڵ�
            //                IGroupLayer pGroupFLayer = new GroupLayerClass ();
            //                string sPareNodeText = pParentele.GetAttribute("NodeText");
            //                //changed by chulili 20110728 ��������ݿ�����
            //                XmlNode pDIRnode = pParentele.ParentNode;
            //                if (pDIRnode != null)
            //                {
            //                    if (pDIRnode.Name.Equals("DIR"))
            //                    {
            //                        sPareNodeText = sPareNodeText + "_" + pDIRnode.Attributes["NodeText"].Value;
            //                    }
            //                }
            //                //end changed by chulili 20110728 ��������ݿ�����
            //                pGroupFLayer.Name = sPareNodeText;
            //                _DicAddGroupLyrs.Add(sParentNodeKey, pGroupFLayer);
            //                pGroupFLayer.Visible = _isLayerVisible;
            //                //added by chulili 20110728 ���ͼ������
            //                ILayerGeneralProperties layerProerties = (ILayerGeneralProperties)pGroupFLayer;
            //                layerProerties.LayerDescription = pParentele.OuterXml;
            //                //end added by chulili
            //                m_pMapCtl.AddLayer(pGroupFLayer, m_pMapCtl.LayerCount);
            //                pParentGroupLayer = _DicAddGroupLyrs[sParentNodeKey];
            //            }
            //        }
            //    }
            //    if (pParentGroupLayer != null)
            //    {
            //        Comlayer = pParentGroupLayer as ICompositeLayer;                 
            //    }

            //    if (_LayerDicOfGroupLayer.Keys.Contains(sParentNodeKey))
            //    {
            //        pDic = _LayerDicOfGroupLayer[sParentNodeKey] as IDictionary<string, ILayer>;
            //    }
            //    else
            //    {
            //        pDic = new Dictionary<string, ILayer>();
            //        _LayerDicOfGroupLayer.Add(sParentNodeKey, pDic as object);
            //    }
            //}
            if (blnChecked)
            {
                vProgress = new SysCommon.CProgress("����'" + checkNode.Text + "'");
                vProgress.EnableCancel = false;
                vProgress.ShowDescription = true;
                vProgress.ShowProgressNumber = true;
                vProgress.TopMost = true;
                vProgress.ShowProgress();
            }
            //end added by chulili 
            if (blnChecked)
            {
                //if (pParentGroupLayer != null)//added by chulili ��bug
                //{
                //    if (!_DicAddGroupLyrs.Keys.Contains(sParentNodeKey))
                //    {

                //        m_pMapCtl.AddLayer(pParentGroupLayer, 0);
                //        IMapLayers pmaplayers = m_pMapCtl.Map as IMapLayers;
                //        foreach (string strkey in pDic.Keys)
                //        {
                //            ILayer player = pDic[strkey];
                //            pmaplayers.DeleteLayer(player);
                //        }
                //        pDic.Clear();
                //        _DicAddGroupLyrs.Add(sParentNodeKey, pParentGroupLayer);
                //    }
                //}
                //if (_DicDelGroupLyrs.Keys.Contains(sParentNodeKey))
                //{
                //    _DicDelGroupLyrs.Remove(sParentNodeKey);
                //}
                XmlElement pLayerEle = layerNode as XmlElement;
                if (pLayerEle != null)
                {
                    pLayerEle.SetAttribute("Load", "1");
                }
                //layerNode.Attributes["Load"].Value = "1";


                //layerNode.Attributes["View"].Value = "1";
                //added by chulili 20110708 �޸�����xml�ļ�
                XmlElement pDocLayerEle = pdocLayerNode as XmlElement;
                if (pDocLayerEle != null)
                {
                    pDocLayerEle.SetAttribute("Load", "1");
                }
                //pdocLayerNode.Attributes["Load"].Value = "1";

                //pdocLayerNode.Attributes["View"].Value = "1";
                vProgress.SetProgress(50, "���ڼ���...");//added by chulili 20110722
                //add end 
                ILayer addLayer = null;
                Exception errLayer = null;
                if (_DicDelLyrs.ContainsKey(nodeKey))
                    addLayer = _DicDelLyrs[nodeKey];
                else
                    addLayer = ModuleMap.AddLayerFromXml(ModuleMap._DicDataLibWks, layerNode, m_pWks, "", null,out errLayer );
                //shduan 20110801 �˳�ʱ�رս�����
                if (addLayer == null)
                {
                    vProgress.Close();
                    return;
                }
                int insertIndex = 0;// ModuleMap.GetControlLayerIndex(layerNode, m_SystemData);
                //����ͼ���Ƿ�ɼ���Ĭ�Ͽɼ���������ͼ��Ŀ¼ʱ�������óɲ��ɼ�������ˢ�£�
                if (_isLayerConfig)
                {
                    addLayer.Visible = _isLayerVisible;
                }
                //if (pParentGroupLayer != null)
                //{
                //    IMapLayers pmaplayers = m_pMapCtl.Map as IMapLayers;
                //    pmaplayers.InsertLayerInGroup(pParentGroupLayer, addLayer, false, Comlayer.Count );
                //    //���ڸ�ͼ������ֵ��м����ͼ��
                //    if (!pDic.Keys.Contains(nodeKey))
                //    {
                //        pDic.Add(nodeKey, addLayer);
                //    }
                //}
                //else
                //{
                    m_pMapCtl.AddLayer(addLayer, insertIndex);
                //}
                ModuleMap.DealOrderOfNewLayer(m_pMapCtl as IMapControlDefault,addLayer);

                //added by chulili 20110628ͼ�������ˢ��һ��toc�ؼ� 
                if (_TocControl != null)
                {
                    _TocControl.Update();
                }
                if (_DicDelLyrs.ContainsKey(nodeKey))
                    _DicDelLyrs.Remove(nodeKey);
                if (_DicAddLyrs.ContainsKey(nodeKey) == false)
                    _DicAddLyrs.Add(nodeKey, addLayer);

            }
            else
            {
                //layerNode.Attributes["Load"].Value = "0";

                XmlElement tmpLayerEle = layerNode as XmlElement;   //�滻��ֵ���� chulili 20111103
                tmpLayerEle.SetAttribute("Load", "0");

                //layerNode.Attributes["View"].Value = "0";
                //added by chulili 20110708 �޸�����xml�ļ�

                XmlElement tmpDocLayerEle = pdocLayerNode as XmlElement;    //�滻��ֵ���� chulili 20111103
                tmpDocLayerEle.SetAttribute("Load", "0");

                //pdocLayerNode.Attributes["Load"].Value = "0";
                //pdocLayerNode.Attributes["View"].Value = "0";
                //add end 
                if (_DicAddLyrs.ContainsKey(nodeKey))
                {
                    ILayer removeLayer = _DicAddLyrs[nodeKey];
                    m_pMapCtl.Map.DeleteLayer(removeLayer);

                    _DicAddLyrs.Remove(nodeKey);

                    //(removeLayer as ILayerGeneralProperties).LayerDescription = layerNode.OuterXml;
                    if (_DicDelLyrs.ContainsKey(nodeKey) == false)
                        _DicDelLyrs.Add(nodeKey, removeLayer);
                    //if (pDic != null)
                    //{
                    //    if (pDic.Keys.Contains(nodeKey))
                    //    {
                    //        pDic.Remove(nodeKey);
                    //    }
                    //}
                }
            }
            InitializeNodeImage(checkNode, blnChecked);
            pXmldoc.Save(_LayerXmlPath);
			if (_isLayerConfig)
            {
                SysCommon.ModSysSetting.IsLayerTreeChanged = true;
            }
            
            SetParentCheckState(checkNode.Parent);
            if (blnChecked)
            {   //added by chulili 20110722�رս�����
                vProgress.SetProgress(100, "�������");
                vProgress.Close();
            }
            //added by chulili 20110714 �жϸ��ڵ��Ƿ����toc��ɾ��
            if (!blnChecked && !checkNode.Parent.Checked)
            {
                if (checkNode.Parent.Tag != "Root")
                {
                    if (checkNode.Parent.DataKey != null)
                    {
                        AddOrDelDataDir(checkNode.Parent, checkNode.Parent.CheckState);
                    }
                }
            }
            //end add
        }
        public void AddDataDir(string strNodeKey)
        {
            if(strNodeKey.Equals(""))
            {
                return;
            }
            //����NodeKey���ҽڵ�
            DevComponents.AdvTree.Node pDataDirNode= SearchLayerNodebyName(strNodeKey);
            if (pDataDirNode == null)
                return;
            //����޷��ӽڵ��ȡxml��Ϣ
            if (pDataDirNode.DataKey == null)
            {
                return;
            }
            if (pDataDirNode.Tag.ToString().Contains("DataDIR") == false) 
                return;
            //���ú���,��Ӹ����ݼ��ڵ�
            if (!pDataDirNode.Checked)
            {
                pDataDirNode.Checked = true;
                AddOrDelDataDir(pDataDirNode, pDataDirNode.CheckState );
            }
        }
        //����DataDir
        private void AddOrDelDataDir(DevComponents.AdvTree.Node checkNode, CheckState checkState)
        {
            //if (checkNode.Tag.ToString().Contains("DataDIR") == false) return;
            //if (checkNode.Tag.ToString().Contains("DIR") == false) return;

            //deleted by chulili checkState���ⲿ���룬���ؽ����޸�
            //if (IsChildAllUnCheck(checkNode))
            //    checkState = CheckState.Checked;
            //else
            //    checkState = CheckState.Unchecked;
            //end delete
            //���ϡ��������ýڵ�״̬
            //deleted by chulili 20111117  �û���;����ȡ�����ػ�ж�ع��̣�����һ��ʼ��Ҫȫ���޸Ľڵ��Check״̬
            //if (checkState==CheckState.Unchecked)
            //{
            //    WhileNodeStateChange(checkNode, checkState.ToString());
            //    SetChildCheckState(checkNode);
            //    SetParentCheckState(checkNode.Parent);
            //}
            //end deleted by chulili 
            XmlNode xmlNode = checkNode.DataKey as XmlNode;
            if (xmlNode != null)
            {
                xmlNode.OwnerDocument.Load(_LayerXmlPath);
            }
            XmlElement xmlele = xmlNode as XmlElement;
            string GroupNodeKey = xmlele.Attributes["NodeKey"].Value;
            //added by chulili 20110708 ��ȡ����xml�����ļ�
            XmlDocument  pXmldoc = new XmlDocument();
            pXmldoc.Load(_LayerXmlPath);
            XmlNode pDocLayerNode = null;
            //add end
             //��ӻ�ɾ��ͼ����ڵ�
            //IGroupLayer DealGroupLayer = null;
            //if (checkState == CheckState.Checked)
            //{
                //if (_DicDelGroupLyrs.ContainsKey(GroupNodeKey))
                //    DealGroupLayer = _DicDelGroupLyrs[GroupNodeKey];
                //else
                //{
                //    DealGroupLayer = new GroupLayerClass();
                //    //changed by chulili 20110728 ��������ݿ�����
                //    XmlNode pDIRnode = xmlele.ParentNode;

                //    string GroupLayertxt = xmlele.Attributes["NodeText"].Value;
                //    if (pDIRnode != null)
                //    {
                //        if (pDIRnode.Name.Equals("DIR"))
                //        {
                //            GroupLayertxt = GroupLayertxt + "_" + pDIRnode.Attributes["NodeText"].Value ;
                //        }
                //    }
                //    //end changed by chulili 20110728 ��������ݿ�����
                //    DealGroupLayer.Name = GroupLayertxt;
                //}
                //int insertIndex = 0;
                //if (DealGroupLayer == null) return;                
                //added by chulili 20110728 ���ͼ������
                //ILayerGeneralProperties layerProerties = (ILayerGeneralProperties)DealGroupLayer;
                //layerProerties.LayerDescription = xmlele.OuterXml;
                //end added by chulili
                //����ͼ���Ƿ�ɼ���Ĭ�Ͽɼ�������ͼ��Ŀ¼ʱ�������óɲ��ɼ�������ˢ�£�
                //DealGroupLayer.Visible = _isLayerVisible;
                //m_pMapCtl.AddLayer(DealGroupLayer, insertIndex);

                //if (_DicDelGroupLyrs.ContainsKey(GroupNodeKey))
                //    _DicDelGroupLyrs.Remove(GroupNodeKey);
                //if (_DicAddGroupLyrs.ContainsKey(GroupNodeKey) == false)
                //    _DicAddGroupLyrs.Add(GroupNodeKey, DealGroupLayer);
            //}
            //else
            //{
                //if (_DicAddGroupLyrs.ContainsKey(GroupNodeKey))
                //{
                //    DealGroupLayer = _DicAddGroupLyrs[GroupNodeKey];
                //    m_pMapCtl.Map.DeleteLayer(DealGroupLayer);

                //    _DicAddGroupLyrs.Remove(GroupNodeKey);

                //    (DealGroupLayer as ILayerGeneralProperties).LayerDescription = xmlele.OuterXml;
                //    if (_DicDelGroupLyrs.ContainsKey(GroupNodeKey) == false)
                //        _DicDelGroupLyrs.Add(GroupNodeKey, DealGroupLayer);
                //}
            //}
            //IDictionary<string, ILayer> pDic = null;
            //if (_LayerDicOfGroupLayer.Keys.Contains(GroupNodeKey))
            //{
            //    pDic = _LayerDicOfGroupLayer[GroupNodeKey] as IDictionary<string, ILayer>;
            //}
            //else
            //{
            //    pDic = new Dictionary<string, ILayer>();
            //    _LayerDicOfGroupLayer.Add(GroupNodeKey, pDic as object);
            //}
            //ICompositeLayer Comlayer = DealGroupLayer as ICompositeLayer;
            XmlNodeList layerList = xmlNode.SelectNodes(".//Layer");
            IMapLayers pmaplayers = m_pMapCtl.Map as IMapLayers;
            //added by chulili 20110722 ��ӽ�����
            SysCommon.CProgress vProgress = null;
            string strLoadInfo = "";
            if (checkState == CheckState.Checked)
            {
                if (_TocControl != null)
                {
                    _TocControl.SetBuddyControl(null);//yjl20111102 add ��������ǰ ���TOC
                }
                SysCommon.ModSysSetting.WriteLog("Deactivate start");
                m_pMapCtl.ActiveView.Deactivate();
                SysCommon.ModSysSetting.WriteLog("Deactivate end");
                strLoadInfo = "����";
            }
            else
            {
                strLoadInfo = "ж��";
            }
            //changed by chulili 20111117 ���ػ�ж�ض�Ҫ������
            vProgress = new SysCommon.CProgress(strLoadInfo+"'" + checkNode.Text + "'");
            //vProgress.EnableCancel = false;
            vProgress.ShowDescription = true;
            vProgress.ShowProgressNumber = true;
            vProgress.TopMost = true;
            vProgress.EnableCancel = true;
            vProgress.EnableUserCancel(true);
            vProgress.ShowProgress();
            List<string> ListLayerError = new List<string>();
            //end added by chulili 
            //ͼ�����
            for (int i = 0; i < layerList.Count; i++)
            {
                //����
                //LinkNodeToControlTree(layerList[i], checkState);
                //LinkNodeToFavoriteTree(layerList[i]);

                string nodeKey = layerList[i].Attributes["NodeKey"].Value;                
                string strSearch = "//" + layerList[i].Name + "[@NodeKey='" + nodeKey + "']";
                pDocLayerNode = pXmldoc.SelectSingleNode(strSearch );
                if (!_isLayerConfig)
                {
                    if (!Plugin.ModuleCommon.AppUser.Name.Equals("admin"))
                    {
                        if (Plugin.ModuleCommon.ListUserdataPriID == null)//changed by xisheng 2011.06.29
                        {
                            continue;
                        }
                        if (!Plugin.ModuleCommon.ListUserdataPriID.Contains(nodeKey))
                        {
                            continue;
                        }
                    }
                }
                if (vProgress.UserAskCancel)
                {
                    break;
                }
                string layertxt = "";
                if (pDocLayerNode != null)
                {
                    layertxt = pDocLayerNode.Attributes["NodeText"].Value.ToString();
                }
                //added by chulili 20110722
                double di = double.Parse((i + 1).ToString());

                vProgress.SetProgress(Convert.ToInt32(di / (layerList.Count) * 100), strLoadInfo+ "'" + layertxt + "'ͼ��");
                DevComponents.AdvTree.Node pCurrentNode = GetLayerNodeByNodeKey(checkNode, nodeKey);
                if (checkState == CheckState.Checked)               
                {
                    SysCommon.ModSysSetting.WriteLog("��ʼ����" + layertxt);                  
                    
                    XmlElement tmpLayeritemEle = layerList[i] as XmlElement;    //�滻��ֵ���� chulili 20111103
                    if (tmpLayeritemEle != null)
                    {
                        tmpLayeritemEle.SetAttribute("Load", "1");
                    }

                    //layerList[i].Attributes["Load"].Value = "1";
                    //layerList[i].Attributes["View"].Value = "1";
                    //added by chulili 20110708 �޸�����xml�ļ�

                    XmlElement tmpDocLayerEle = pDocLayerNode as XmlElement;    //�滻��ֵ����,chulili 20111103
                    if (tmpDocLayerEle != null)
                    {
                        tmpDocLayerEle.SetAttribute("Load", "1");
                    }
                    //if (pDocLayerNode != null)
                    //{
                    //    pDocLayerNode.Attributes["Load"].Value = "1";
                    //}
                    //pDocLayerNode.Attributes["View"].Value = "1";
                    //add end 
                    ILayer addLayer = null;
                    
                    //fmPgs.lblOut.Text = "����'" + layertxt + "'ͼ��";
                    Exception errLayer=null;
                    if (_DicDelLyrs.ContainsKey(nodeKey))
                    {
                        addLayer = _DicDelLyrs[nodeKey];
                    }
                    else
                    {   
                        addLayer = ModuleMap.AddLayerFromXml(ModuleMap._DicDataLibWks, layerList[i], m_pWks, "", null,out errLayer);
                    }
                    if (addLayer == null)
                    {
                        if(errLayer!=null)
                        {
                            ListLayerError.Add(layertxt+"����ʧ�ܣ�ʧ��ԭ��"+errLayer.Message);
                        }
                        if (pCurrentNode != null)
                        {
                            pCurrentNode.Checked = false;
                            SetParentCheckState(pCurrentNode);
                        }
                        continue;
                    }
                    //moved by chulili 20111118 ����Ų��λ�ã�ȷ���Ѿ���ȡ��ͼ������ڽڵ��ϴ�
                    if (pCurrentNode != null)
                    {
                        pCurrentNode.Checked = true;
                        SetParentCheckState(pCurrentNode);
                    }
                    //end moved by chulili 20111118
                    if (_isLayerConfig)
                    {
                        addLayer.Visible = _isLayerVisible;
                    }
                    SysCommon.ModSysSetting.WriteLog("GetIndexOfNewLayer start");
                    //changed by chulili 20111117 �Ѽ��غ͵���˳�����ϳ�һ�����裬����ˢ��
                    int NewLayerIndex = ModuleMap.GetIndexOfNewLayer(m_pMapCtl as IMapControlDefault, addLayer);
                    SysCommon.ModSysSetting.WriteLog("AddLayer start");
                    m_pMapCtl.AddLayer(addLayer, NewLayerIndex);
                    //end changed by chulili 20111117
                    //ModuleMap.DealOrderOfNewLayer(m_pMapCtl as IMapControlDefault, addLayer);

                    //m_SystemData.frmProgress.ShowProgress("���ڼ���" + addLayer.Name + "����...", (int)((i + 1) * 100 / layerList.Count));

                    if (_DicDelLyrs.ContainsKey(nodeKey))
                        _DicDelLyrs.Remove(nodeKey);
                    if (_DicAddLyrs.ContainsKey(nodeKey) == false)
                        _DicAddLyrs.Add(nodeKey, addLayer);
                    
                    SysCommon.ModSysSetting.WriteLog("�������" + layertxt);
                    InitializeNodeImage(pCurrentNode, true);
                }
                else
                {
                    if (pCurrentNode != null)
                    {
                        pCurrentNode.Checked = false;
                        SetParentCheckState(pCurrentNode);
                    }
                    if (vProgress.UserAskCancel)
                    {
                        break;
                    }
                    XmlElement tmpLayeritemEle = layerList[i] as XmlElement;    //�滻��ֵ���� chulili 20111103
                    if (tmpLayeritemEle != null)
                    {
                        tmpLayeritemEle.SetAttribute("Load", "0");
                    }

                    //layerList[i].Attributes["Load"].Value = "0";
                    //layerList[i].Attributes["View"].Value = "0";
                    //added by chulili 20110708 �޸�����xml�ļ�

                    XmlElement tmpDocLayerEle = pDocLayerNode as XmlElement;    //�滻��ֵ����,chulili 20111103
                    if (tmpDocLayerEle != null)
                    {
                        tmpDocLayerEle.SetAttribute("Load", "0");
                    }

                    //if (pDocLayerNode != null)
                    //{
                    //    pDocLayerNode.Attributes["Load"].Value = "0";
                    //}
                    //pDocLayerNode.Attributes["View"].Value = "0";
                    //add end
                    if (_DicAddLyrs.ContainsKey(nodeKey))
                    {
                        ILayer removeLayer = _DicAddLyrs[nodeKey];
                        m_pMapCtl.Map.DeleteLayer(removeLayer);

                        //m_SystemData.frmProgress.ShowProgress("�����Ƴ�" + removeLayer.Name + "����...", (int)((i + 1) * 100 / layerList.Count));
                        _DicAddLyrs.Remove(nodeKey);

                        //(removeLayer as ILayerGeneralProperties).LayerDescription = layerList[i].OuterXml;
                        if (_DicDelLyrs.ContainsKey(nodeKey) == false)
                            _DicDelLyrs.Add(nodeKey, removeLayer);
                    }
                    InitializeNodeImage(pCurrentNode, false);
                }
                
            }
            //if (checkState == CheckState.Checked)
            //{
            //    SetParentCheckState(checkNode);     //added by chulili 20111028
            //}
			pXmldoc.Save(_LayerXmlPath);//added by chulili 20110708
            if (_isLayerConfig)
            {
                SysCommon.ModSysSetting.IsLayerTreeChanged = true;
            }
            if (checkState == CheckState.Checked)
            {//���ͼ�������
                //ModuleMap.LayersCompose(m_pMapCtl as IMapControlDefault);
                //added by chulili 20110628ͼ�������ˢ��һ��toc�ؼ� 
                m_pMapCtl.ActiveView.Activate(m_pMapCtl.hWnd);
                
                if (_TocControl != null)
                {
                    _TocControl.SetBuddyControl(m_pMapCtl);//yjl20111102 add �������ݺ� ��TOC��200ͼ��Ч�ʿ�1����
                    _TocControl.Update();
                }
                
                //if (fmPgs != null && !fmPgs.IsDisposed)
                //    fmPgs.Dispose();
            }
            vProgress.Close();  //changed by chulili 20111117 ���غ�ж�ض�Ҫ������
            if (ListLayerError.Count > 0)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandleEx("������ʾ", "����ͼ���������´���", ListLayerError);
            }
            ListLayerError = null;
            //m_SystemData.TocTreeESRI.SetBuddyControl(m_SystemData.MainMap);
            //GeoFrameBase.ModuleControl.TreeRedraw(ptr, true);
            //GeoFrameBase.ModuleControl.CollapseTocLayer(m_SystemData.MainMap.Map, m_SystemData.TocTreeESRI, false);
            //m_SystemData.frmProgress.Visible = false;
            //changed by chulili 20110718 �û��޸Ľڵ�check״̬ʱ���������޸�չ��״̬
            //���ƽڵ��Ƿ�չ��  
            //if (XmlOperation.GetBoolean(xmlNode, true, "Expand") == false)
            //    checkNode.Expanded = false;
            //end changed by chulili 
        }
        //added by chulili 20111118 ����NodeKey���ҽڵ�
        public DevComponents.AdvTree.Node GetNodeByNodeKey( string strNodeKey)
        {
            if (TreeDataLib.Nodes.Count == 0)
            {
                return null;
            }
            DevComponents.AdvTree.Node pNode = TreeDataLib.Nodes[0];
            return GetLayerNodeByNodeKey(pNode, strNodeKey);
        }
        //����NodeKey���ҽڵ㣬�ݹ���ú���
        //�������壺���ҽڵ㷶Χ��NodeKeyֵ
        public DevComponents.AdvTree.Node GetLayerNodeByNodeKey(DevComponents.AdvTree.Node pNode,string strNodeKey)
        {
            DevComponents.AdvTree.Node pTmpnode = null;
            if (pNode.Name.Equals(strNodeKey))
                return pNode;
            if (pNode.Nodes.Count > 0)
            {
                for (int i = 0; i < pNode.Nodes.Count; i++)
                {
                    DevComponents.AdvTree.Node pChildNode = pNode.Nodes[i];
                    pTmpnode = GetLayerNodeByNodeKey(pChildNode,strNodeKey);
                    if (pTmpnode != null)
                    {
                        return pTmpnode;
                    }
                }
            }
            return null;
        }
        //��ӻ�ж��ͼ��
        private void TreeDataLib_AfterCheck(object sender, AdvTreeCellEventArgs e)
        {
            if (e.Action == eTreeAction.Mouse)
            {
                DevComponents.AdvTree.Node vNode = e.Cell.Parent;
                if (vNode.DataKey == null) return;
                XmlNode layerNode = vNode.DataKey as XmlNode;
                if (layerNode.Name == "DataDIR" || layerNode.Name == "Root")
                {
                    AddOrDelDataDir(vNode, vNode.CheckState);
                }
                else if (layerNode.Name == "DIR")
                {
                    AddOrDelDataDir(vNode, vNode.CheckState);
                    //return;
                }
                else
                {
                    AddOrDelLyr(vNode, vNode.Checked);
                }
            }
            
        }

        //�ж��Ƿ������ӽڵ��CheckedState��ΪChecked
        protected bool IsChildAllCheck(DevComponents.AdvTree.Node treenode)
        {
            if (treenode.Nodes.Count == 0)
                return false;
            for (int i = 0; i < treenode.Nodes.Count; i++)
            {
                if (treenode.Nodes[i].CheckState == CheckState.Unchecked)
                    return false;
            }
            return true;
        }
        //�ж��Ƿ������ӽڵ��CheckedState��ΪUnChecked
        protected bool IsChildAllUnCheck(DevComponents.AdvTree.Node treenode)
        {
            if (treenode.Nodes.Count == 0)
                return false;
            for (int i = 0; i < treenode.Nodes.Count; i++)
            {
                if (treenode.Nodes[i].CheckState == CheckState.Checked)
                    return false;
            }
            return true;
        }

        //�������ø�ѡ��
        protected void SetChildCheckState(DevComponents.AdvTree.Node nodeCurrent)
        {
            for (int i = 0; i < nodeCurrent.Nodes.Count; i++)
            {
                nodeCurrent.Nodes[i].CheckState = nodeCurrent.CheckState;
                WhileNodeStateChange(nodeCurrent.Nodes[i], nodeCurrent.CheckState.ToString());
                SetChildCheckState(nodeCurrent.Nodes[i]);
            }
        }

        //�������ø�ѡ��
        protected void SetParentCheckState(DevComponents.AdvTree.Node nodeParent)
        {
            if (nodeParent == null) return;

            if (nodeParent.Tag.ToString() == "Layer")
            {//deleted by chulili 20110708ͼ��ڵ�û���ӽڵ㣬���Բ��ؽ����жϣ�Checked״̬������xml��Load��������
                //if (IsChildAllCheck(nodeParent))
                //{
                //    nodeParent.CheckState = CheckState.Checked;
                //    WhileNodeStateChange(nodeParent, CheckState.Checked.ToString());
                //}
                //else if (IsChildAllUnCheck(nodeParent))
                //{
                //    nodeParent.CheckState = CheckState.Unchecked;
                //    WhileNodeStateChange(nodeParent, CheckState.Unchecked.ToString());
                //}
                //else
                //{
                //    nodeParent.CheckState = CheckState.Checked;
                //    WhileNodeStateChange(nodeParent, "DataDIR&HalfOpened");
                //}
                SetParentCheckState(nodeParent.Parent);
            }
            else if (nodeParent.Tag.ToString().Contains("DataDIR"))
            {
                if (IsChildAllCheck(nodeParent))
                {
                    nodeParent.CheckState = CheckState.Checked;
                    WhileNodeStateChange(nodeParent, CheckState.Checked.ToString());
                }
                else if (IsChildAllUnCheck(nodeParent))
                {
                    nodeParent.CheckState = CheckState.Unchecked;
                    WhileNodeStateChange(nodeParent, CheckState.Unchecked.ToString());
                }
                else
                {
                    nodeParent.CheckState = CheckState.Checked;
                    WhileNodeStateChange(nodeParent, "DataDIR&HalfOpened");
                }

                SetParentCheckState(nodeParent.Parent);
            }
            else if (nodeParent.Tag.ToString()=="DIR")  //added by chulili 20111009֧��ר��ڵ㸴ѡ��
            {
                if (IsChildAllCheck(nodeParent))
                {
                    nodeParent.CheckState = CheckState.Checked;
                }
                else if (IsChildAllUnCheck(nodeParent))
                {
                    nodeParent.CheckState = CheckState.Unchecked;
                }
                else
                {
                    nodeParent.CheckState = CheckState.Checked;
                }
                SetParentCheckState(nodeParent.Parent);
            }
        }

        //�������ڵ��tagֵ��ͼƬ
        private void SetTreeNodeImage(DevComponents.AdvTree.Node treenode, string tagStr)
        {
            if (treenode.Tag == null) return;
            if (treenode.Tag.ToString().Contains("DataDIR") == false) return;
            treenode.Image = null;
            string tagString = tagStr;
            switch (tagStr)
            {
                case "DataDIR&AllOpened":
                    tagString = "DataDIROpen";
                    break;
                case "DataDIR&Closed":
                    tagString = "DataDIRClosed";
                    break;
                case "DataDIR&HalfOpened":
                    tagString = "DataDIRHalfOpen";
                    break;
                case "Checked":
                    tagString = "DataDIROpen";
                    break;
                case "Unchecked":
                    tagString = "DataDIRClosed";
                    break;
            }
            //treenode.Tag = tagString;//shduan20110612
            //treenode.LeftImages.Add(tagString);
            treenode.Image = treenode.TreeControl.ImageList.Images[tagString];
        }

        //��ѡ���ѡ��״̬�ı�ʱ����Ҫ��������
        protected void WhileNodeStateChange(DevComponents.AdvTree.Node treenode, string checkState)
        {
            SetTreeNodeImage(treenode, checkState);
        }

        private void UcDataLib_Load(object sender, EventArgs e)
        {
            //���������ͼ��Ŀ¼ʱʹ�ã���������Ӧ���Ҽ��˵�
            if (_isLayerConfig == true)
            {
                //this.barMenu.Visible = true;                    
                TreeDataLib.ContextMenuStrip = this.contextMenuLayerTree;
                this.contextMenuLayerTree.Visible = true;
                //this.checkBoxShow.Checked = false;
                _isLayerVisible = false;
                ModuleMap.SetLayersVisibleAttri(m_pMapCtl as IMapControlDefault, _isLayerVisible);
            }
        }
        public void AddFolder()
        {
            DevComponents.AdvTree.Node pNode = TreeDataLib.SelectedNode;// layerTree.SelectedNode;
            if (pNode == null)
                return;
            //�Դ�������޸ģ��ϸ�������������ļ��еĽڵ�����
            if (!pNode.Tag.ToString().Equals("DIR") && !pNode.Tag.ToString().Contains("Root"))
                return;  
            try
            {
                //�����Ի���
                FormAddFolder pFrm = new FormAddFolder(Plugin.ModuleCommon.TmpWorkSpace );
                if (pFrm.ShowDialog() != DialogResult.OK)
                    return;
                //���ݴӶԻ��򷵻ص�����ֵ�����ýڵ�
                DevComponents.AdvTree.Node addNode = new DevComponents.AdvTree.Node();
                addNode.Text = pFrm._Foldername;
                addNode.Tag = "DIR";
                addNode.Image = this.ImageList.Images["DIR"];
                addNode.CheckBoxVisible = true;
                addNode.CheckState = CheckState.Checked;
                string nodekey = Guid.NewGuid().ToString();
                addNode.Name = nodekey;
                pNode.Nodes.Add(addNode);
                //�����Ӧ��xml�ڵ�
                XmlDocument XMLDoc = new XmlDocument();
                XMLDoc.Load(_LayerXmlPath);
                //shduan 20110612 �޸�*******************************************************************************************
                //string strSearch = "//" + pNode.Tag + "[@NodeKey='" + pNode.Name + "']";
                string strSearch = "";
                if (pNode.Tag.ToString().Contains("DataDIR"))
                {
                    strSearch = "//DataDIR" + "[@NodeKey='" + pNode.Name + "']";
                }
                else
                {
                    strSearch = "//" + pNode.Tag + "[@NodeKey='" + pNode.Name + "']";
                }
                //****************************************************************************************************************
                XmlNode pxmlnode = XMLDoc.SelectSingleNode(strSearch);
                XmlElement childele = XMLDoc.CreateElement("DIR");

                childele.SetAttribute("NodeKey", nodekey);
                childele.SetAttribute("NodeText", pFrm._Foldername);
                childele.SetAttribute("Description", pFrm._FolderScrip);
                childele.SetAttribute("DataScale", pFrm._Scale);
                childele.SetAttribute("DataType", pFrm._DataType);
                childele.SetAttribute("Year", pFrm._Year);
                childele.SetAttribute("DIRType", pFrm._DIRType);
                //deleted by chulili 20110921 ��һ��ר��ڵ�û��������
                //childele.SetAttribute("XZQCode", pFrm._XZQCode);
                childele.SetAttribute("Enabled", "true");
                childele.SetAttribute("Expand", "100");
                pxmlnode.AppendChild(childele as XmlNode);
                XMLDoc.Save(_LayerXmlPath);
                //addNode.DataKey = childele as object;
                ModuleMap.SetDataKey(addNode, childele as XmlNode);
                SysCommon.ModSysSetting.IsLayerTreeChanged = true;
            }
            catch
            { }
        }
        //����ļ���
        private void MenuItemAddFolder_Click(object sender, EventArgs e)
        {
            AddFolder();
        }
//�򿪵�ͼ�ĵ��ĺ������� added by chulili 20110728
        public void OpenMxdDocDeal()
        {
            //��սڵ��״̬
            for (int i = 0; i < this.TreeDataLib.Nodes.Count; i++)
            {
                DevComponents.AdvTree.Node pNode = TreeDataLib.Nodes[i];
                UnSelectAllAdvNode(pNode);
            }
            //��ո����ֵ�
            _DicAddLyrs.Clear();
            _DicDelLyrs.Clear();
            _DicAddGroupLyrs.Clear();
            _DicDelGroupLyrs.Clear();
            _LayerDicOfGroupLayer.Clear();

            //ѭ������map�е�ͼ��
            for ( int i = m_pMapCtl.Map.LayerCount-1;i>=0; i--)
            {
                ILayer pLayer = m_pMapCtl.Map.get_Layer(i);
                OpenMxdDocDealLayer(pLayer);
            }
        }
        //�ݹ麯���������еĽڵ㲻ѡ�� added by chulili 20110728
        private void UnSelectAllAdvNode(DevComponents.AdvTree.Node pNode)
        {
            pNode.Checked = false;
            if (pNode.Nodes.Count > 0)
            {
                for (int i = 0; i < pNode.Nodes.Count; i++)
                {
                    DevComponents.AdvTree.Node pTmpNode = pNode.Nodes[i];
                    UnSelectAllAdvNode(pTmpNode );
                }
            }
        }
        //�򿪵�ͼ�ĵ����ͼ���������  �ݹ����added by chulili 20110728
        private void OpenMxdDocDealLayer(ILayer pLayer)
        {
            if (pLayer is IGroupLayer)
            {
                #region ��GroupLayer��֧�֣���ע�͵���Ŀǰû��GroupLayer chulili 20111103
                //ILayerGeneralProperties pLayerGenPro = pLayer as ILayerGeneralProperties;
                ////��ȡ��ͼ���������Ϣ��ת��xml�ڵ�
                //string strNodeXml = pLayerGenPro.LayerDescription;

                //if (strNodeXml.Equals(""))
                //{
                //    return;
                //}
                //XmlDocument pXmldoc = new XmlDocument();
                //pXmldoc.LoadXml(strNodeXml);
                ////��ȡ�ڵ��NodeKey��Ϣ
                //XmlNode pxmlnode = pXmldoc.SelectSingleNode("//DataDIR");
                //if (pxmlnode == null)
                //{
                //    pXmldoc = null;
                //    return;
                //}
                //string strNodeKey = pxmlnode.Attributes["NodeKey"].Value.ToString();
                //DevComponents.AdvTree.Node pnode = SearchLayerNodebyName(strNodeKey);
                //if (pnode == null)
                //{
                //    pXmldoc = null;
                //    return;
                //}
                ////�洢��ͼ�����ֵ���
                //if (!_DicAddGroupLyrs.Keys.Contains(strNodeKey)) 
                //{
                //    _DicAddGroupLyrs.Add(strNodeKey, pLayer as IGroupLayer );
                //}
                ////��Ӧ�ڵ��
                //pnode.Checked = true;
                ////���δ����ӽڵ�
                //ICompositeLayer pCompoLayer = pLayer as ICompositeLayer;
                //for (int i = 0; i < pCompoLayer.Count; i++)
                //{
                //    ILayer pTmpLayer = pCompoLayer.get_Layer(i);
                //    OpenMxdDocDealLayer(pTmpLayer);
                //}
                #endregion
            }
            else if (pLayer is IFeatureLayer || pLayer is IRasterLayer || pLayer is IRasterCatalog)
            {
                IFeatureLayer pFeaLayer = pLayer as IFeatureLayer;
                ILayerGeneralProperties pLayerGenPro = pLayer as ILayerGeneralProperties;
                //��ȡͼ���������ת��xml�ڵ�
                string strNodeXml = pLayerGenPro.LayerDescription;
                
                if (strNodeXml.Equals(""))
                {
                    return;
                }
                XmlDocument pXmldoc = new XmlDocument();
                pXmldoc.LoadXml(strNodeXml );
                //��ȡ�ڵ��NodeKey��Ϣ
                XmlNode pxmlnode = pXmldoc.SelectSingleNode("//Layer");
                if (pxmlnode == null)
                { return; }
                string strNodeKey = pxmlnode.Attributes["NodeKey"].Value.ToString();
                pXmldoc = null;
                if (!Plugin.ModuleCommon.AppUser.Name.Equals("admin"))
                {
                    if (Plugin.ModuleCommon.ListUserdataPriID == null)//changed by xisheng 2011.06.29
                    {
                        m_pMapCtl.Map.DeleteLayer(pLayer);
                        return;
                    }
                    if (!Plugin.ModuleCommon.ListUserdataPriID.Contains(strNodeKey))
                    {
                        m_pMapCtl.Map.DeleteLayer(pLayer);
                        return;
                    }
                }
                ModuleMap.SetFieldVisibleOfLayer(pLayer,pxmlnode);
                //��ͼ�㱣�浽ͼ���ֵ���
                DevComponents.AdvTree.Node pnode = SearchLayerNodebyName(strNodeKey);
                if (pnode == null)
                {
                    return; 
                }
                if (!_DicAddLyrs.Keys.Contains(strNodeKey))
                {
                    _DicAddLyrs.Add(strNodeKey, pLayer);
                }
                //����Ŀ¼����Ӧ�ڵ��                
                pnode.Checked = true;
                SetParentCheckState(pnode);
                #region ��GroupLayer��֧�֣���ע�͵���Ŀǰû��GroupLayer chulili 20111103
                //������ڵ������ݼ��ڵ�
                //DevComponents.AdvTree.Node pParentNode = pnode.Parent;
                //if (!pParentNode.Tag.ToString().Contains("DataDIR"))
                //{
                //    return;
                //}
                
                //string sParentNodeKey = pParentNode.Name;
                ////���ڵ���ӽڵ���ֵ��ﱣ���ͼ��
                //IDictionary<string, ILayer> pDic = null;
                //if (_LayerDicOfGroupLayer.Keys.Contains(sParentNodeKey))
                //{
                //    pDic = _LayerDicOfGroupLayer[sParentNodeKey] as IDictionary<string, ILayer>;
                //}
                //else
                //{
                //    pDic = new Dictionary<string, ILayer>();
                //    _LayerDicOfGroupLayer.Add(sParentNodeKey, pDic as object);
                //}
                ////���ڸ�ͼ������ֵ��м����ͼ��
                //if (!pDic.Keys.Contains(strNodeKey))
                //{
                //    pDic.Add(strNodeKey, pLayer );
                //} 
                #endregion
            }
        }

        //�Ƴ�ĳ��ͼ�㣬�������ⲿ���أ��������ڲ�����
        public void Removelayer(ILayer player)
        {
            string nodekey = "";
            //�ڲ�����
            if (_DicAddLyrs.Values.Contains(player))
            {
                foreach (string strkey in _DicAddLyrs.Keys)
                {
                    ILayer ptmplayer = _DicAddLyrs[strkey];
                    if (ptmplayer.Equals(player))
                    {   //���ҵ����ڵ�����(���ڵ����xml�нڵ��nodekey)
                        nodekey = strkey;
                        break;
                    }
                }
                //�������ڵ����ƣ����Ҷ�Ӧ���ڵ�
                DevComponents.AdvTree.Node pnode = SearchLayerNodebyName(nodekey);
                //������ڲ����ص�ͼ�㣬�ҵ���Ӧ�����ڵ㣬�����ڵ�AfterCheck����
                if (pnode != null)
                {
                    pnode.Checked = false;
                    AddOrDelLyr(pnode, false);
                }
            }
            //�ⲿ����
            else
            {
                m_pMapCtl.Map.DeleteLayer(player );
            }
        }
        //�Ƴ�ĳ��ͼ�㣬�������ⲿ���أ��������ڲ�����
        public void Removelayer()
        {
            DevComponents.AdvTree.Node pNode = TreeDataLib.SelectedNode;// layerTree.SelectedNode;
            if (pNode == null)
                return;
            //�Դ�������޸ģ��ϸ�������������ļ��еĽڵ�����
            if (!pNode.Tag.ToString().Contains ("Layer"))
                return;
            string strTag = pNode.Tag.ToString();
            switch (strTag)
            {
                case "Layer":
                    pNode.Checked = false;
                    AddOrDelLyr(pNode, false);
                    break;
                case "OutLayer":
                    ILayer pLayer = pNode.DataKey as ILayer;
                    m_pMapCtl.Map.DeleteLayer(pLayer);
                    pNode.Remove();
                    break;

            }
        }
        //added by chulili 20110714 ��֪�ڵ����ƣ����ҽڵ㣨����Ψһ��
        private DevComponents.AdvTree.Node  SearchLayerNodebyName(string nodename)
        {
            if (TreeDataLib.Nodes.Count == 0)
                return null;
            DevComponents.AdvTree.Node pSearchNode = null;
            for (int i = 0; i < TreeDataLib.Nodes.Count; i++)
            {
                DevComponents.AdvTree.Node pNode = TreeDataLib.Nodes[i];
                if (pNode.Name == nodename)
                    return pNode;
                //���õݹ麯�������ҽڵ�
                pSearchNode = SearchLayerNodebyName(pNode, nodename);
                if (pSearchNode != null)
                    return pSearchNode;

            }
            return null;
        }//added by chulili 20110714 ��֪�ڵ����ƣ����ҽڵ㣨����Ψһ�����ݹ����
        private DevComponents.AdvTree.Node SearchLayerNodebyName(DevComponents.AdvTree.Node pNode, string nodename)
        {
            if (pNode.Name == nodename)
                return pNode;
            if(pNode.Nodes.Count == 0)
                return null;
            DevComponents.AdvTree.Node pSearchNode = null;
            //�����ӽڵ�
            for (int i = 0; i < pNode.Nodes.Count; i++)
            {
                DevComponents.AdvTree.Node ptmpNode = pNode.Nodes[i];
                if (ptmpNode.Name == nodename)
                    return ptmpNode;
                //�ݹ���ò���
                pSearchNode = SearchLayerNodebyName(ptmpNode,nodename);
                //������ҵ����򷵻�
                if (pSearchNode != null)
                    return pSearchNode;

            }
            return null;
        }
        public void AddDataSet()
        {
            DevComponents.AdvTree.Node pNode = TreeDataLib.SelectedNode;// layerTree.SelectedNode;

            if (pNode == null)
                return;
            //�޸Ĵ��룬������������ݼ��Ľڵ����ͽ����ϸ��޶�
            if (!pNode.Tag.ToString().Equals("DIR") && !pNode.Tag.ToString().Contains("Root") && !pNode.Tag.ToString().Contains("DataDIR"))
                return;
            try
            {
                FormAddLayer pFrm = new FormAddLayer(null,_LayerXmlPath, m_pWks, pNode, true, this.ImageList, 2);
                pFrm.ShowDialog();
                //added by chulili 20110712 ���ݽڵ��ѡ��״̬�ж��Ƿ����
                DevComponents.AdvTree.Node pAddnode = pFrm.CurNode;
                if (pAddnode.Checked)
                {
                    m_pMapCtl.ActiveView.Deactivate();  //��ˢ��
                    AddNodeToMap(pAddnode);
                    m_pMapCtl.ActiveView.Activate(m_pMapCtl.hWnd);  //ˢ��
                    //m_pMapCtl.ActiveView.Refresh();
                }
                //end add
            }
            catch
            { }
        }
        //������ݼ�
        private void MenuItemAddDataset_Click(object sender, EventArgs e)
        {

            AddDataSet();
               
            //shduan 20110612����
            //if (pNode.Tag.ToString().Equals("Layer") || pNode.Tag.ToString().Contains ("DataDIR"))
            //    return;
            //******************************************************************************************
            //FormAddDataSet pFrm = new FormAddDataSet();
            //if (pFrm.ShowDialog() != DialogResult.OK)
            //    return;
            //DevComponents.AdvTree.Node addNode = new DevComponents.AdvTree.Node();
            //addNode.Text = pFrm._DataSetname;
            //addNode.Tag = "DataDIR";
            //addNode.Image = this.ImageList.Images["DataDIROpen"];
            //addNode.CheckBoxVisible = true;
            //addNode.CheckState = CheckState.Checked;
            //string nodekey = Guid.NewGuid().ToString();
            //addNode.Name = nodekey;
            //pNode.Nodes.Add(addNode);
            //string strTag = pNode.Tag.ToString();
            //string NodeName = strTag;
            //if (strTag.Contains("DataDIR"))
            //{
            //    NodeName = "DataDIR";
            //}
            //XmlDocument XMLDoc = new XmlDocument();
            //XMLDoc.Load(_LayerXmlPath);
            //string strSearch = "//" + NodeName + "[@NodeKey='" + pNode.Name + "']";
            //XmlNode pxmlnode = XMLDoc.SelectSingleNode(strSearch);
            //XmlElement childele = XMLDoc.CreateElement("DataDIR");

            //childele.SetAttribute("NodeKey", nodekey);
            //childele.SetAttribute("NodeText", pFrm._DataSetname);
            //childele.SetAttribute("Description", pFrm._DataSetScrip);
            //childele.SetAttribute("Enabled", "true");
            //childele.SetAttribute("Expand", "100");
            //pxmlnode.AppendChild(childele as XmlNode);
            //XMLDoc.Save(_LayerXmlPath);

        }
        public void AddWMSserviceLayer()
        {
            DevComponents.AdvTree.Node pNode = TreeDataLib.SelectedNode;// layerTree.SelectedNode;
            if (pNode == null)
                return;

            //�޸Ĵ��룬���������ͼ��Ľڵ�����ϸ��޶�
            if (!pNode.Tag.ToString().Equals("DIR") && !pNode.Tag.ToString().Contains("Root") && !pNode.Tag.ToString().Contains("DataDIR"))
                return;
            try
            {
                FormAddServiceLayer2 pFrm = new FormAddServiceLayer2(null,_LayerXmlPath, m_pWks, pNode, true, this.ImageList,"WMS");
                pFrm.ShowDialog();//FormAddServiceLayer yjl20120814 modify
                //added by chulili 20110712 ���ݽڵ��ѡ��״̬�ж��Ƿ����
                DevComponents.AdvTree.Node pAddnode = pFrm.CurNode;
                if (pAddnode.Checked)
                {
                    //m_pMapCtl.ActiveView.Deactivate();  //��ˢ��

                    AddNodeToMap(pAddnode);

                    //m_pMapCtl.ActiveView.Activate(m_pMapCtl.hWnd);  //ˢ��

                    m_pMapCtl.ActiveView.Refresh();
                }
                //end add
            }
            catch
            { }
        }
        public void AddLayer()
        {
            DevComponents.AdvTree.Node pNode = TreeDataLib.SelectedNode;// layerTree.SelectedNode;
            if (pNode == null)
                return;

            //�޸Ĵ��룬���������ͼ��Ľڵ�����ϸ��޶�
            if (!pNode.Tag.ToString().Equals("DIR") && !pNode.Tag.ToString().Contains("Root") && !pNode.Tag.ToString().Contains("DataDIR"))
                return;
            try
            {
                FormAddLayer pFrm = new FormAddLayer(null,_LayerXmlPath, m_pWks, pNode, true, this.ImageList, 1);
                pFrm.ShowDialog();
                //added by chulili 20110712 ���ݽڵ��ѡ��״̬�ж��Ƿ����
                DevComponents.AdvTree.Node pAddnode = pFrm.CurNode;
                if (pAddnode.Checked)
                {
                    //m_pMapCtl.ActiveView.Deactivate();  //��ˢ��

                    AddNodeToMap(pAddnode);

                    //m_pMapCtl.ActiveView.Activate(m_pMapCtl.hWnd);  //ˢ��

                    m_pMapCtl.ActiveView.Refresh();
                }
                //end add
            }
            catch
            { }
        }
        //���ͼ��
        private void MenuItemAddLayer_Click(object sender, EventArgs e)
        {
            AddLayer();
        }
        public string GetTagOfSeletedNode()
        {
            //��ȡ�û�ѡ�еĽڵ�
            DevComponents.AdvTree.Node pnode = TreeDataLib.SelectedNode;// layerTree.SelectedNode;
            if (pnode == null)
                return "";
            string strTag = pnode.Tag.ToString();
            string NodeName = strTag;
            //�������ݼ��ڵ㣬�򽫽ڵ�����ת��һ��
            if (strTag.Contains("DataDIR"))
            {
                NodeName = "DataDIR";
            }
            return NodeName;
        }
        public void ModifyNode()
        {
            //��ȡ�û�ѡ�еĽڵ�
            DevComponents.AdvTree.Node pnode = TreeDataLib.SelectedNode;// layerTree.SelectedNode;
            if (pnode == null)
                return;
            string strTag = pnode.Tag.ToString();
            string NodeName = strTag;
            //�������ݼ��ڵ㣬�򽫽ڵ�����ת��һ��
            if (strTag.Contains("DataDIR"))
            {
                NodeName = "DataDIR";
            }
            //���ú����Խڵ�����޸�
            switch (NodeName)
            {
                case "Root":
                    ModifyRoot(pnode);
                    break;
                case "DIR":
                    ModifyFolder(pnode);
                    break;
                case "DataDIR":
                    ModifyDataset(pnode);
                    break;
                case "Layer":
                    ModifyLayer(pnode);
                    break;
                default:
                    break;

            }
        }
        //�޸Ľڵ�
        private void MenuItemModify_Click(object sender, EventArgs e)
        {
            ModifyNode();
        }
        //ɾ���ڵ�
        private void MenuDelete_Click(object sender, EventArgs e)
        {
            DeleteTreeNode();
        }
        //added by chulili 2012-11-09 ��ȡɾ�������ݽڵ��б�
        private void GetDeleteNodeKeys(DevComponents.AdvTree.Node pNode, ref List<string> ListDeleteNodeKeys)
        {
            string strtag = pNode.Tag.ToString();
            if (pNode.Nodes.Count > 0)
            {
                for (int i = 0; i < pNode.Nodes.Count; i++)
                {
                    GetDeleteNodeKeys(pNode.Nodes[i],ref ListDeleteNodeKeys);
                }
            }
            string strName = pNode.Name;
            ListDeleteNodeKeys.Add(strName);
            if (pNode.Tag.ToString() == "Layer")
            {
                //������xml�ж�Ӧ�ڵ�
                XmlDocument XMLDoc = new XmlDocument();
                XMLDoc.Load(_LayerXmlPath);
                string strSearch = "//Layer[@NodeKey='" + strName + "']";
                XmlNode pxmlnode = XMLDoc.SelectSingleNode(strSearch);
                if (pxmlnode.ChildNodes.Count > 0)
                {
                    foreach (XmlNode pChildnode in pxmlnode.ChildNodes)
                    {
                        if (pChildnode.Name == "Field")
                        {
                            try
                            {
                                string strFieldNodeKey = (pChildnode as XmlElement).GetAttribute("NodeKey");
                                ListDeleteNodeKeys.Add(strFieldNodeKey);
                            }
                            catch
                            { }
                        }
                    }
                }
                XMLDoc = null;
            }

        }
        public void GetDeleteNodeKeys(ref List<string> ListDeleteNodeKeys)
        {
            DevComponents.AdvTree.Node pnode = TreeDataLib.SelectedNode;
            GetDeleteNodeKeys(pnode, ref ListDeleteNodeKeys);

        }
        //changed by chulili 2012-11-09  ɾ���ڵ��ͬʱ��ɾ������Ȩ���б������Ӧ�ļ�¼
        public void DeleteTreeNode()
        {
            DevComponents.AdvTree.Node pnode = TreeDataLib.SelectedNode;// layerTree.SelectedNode;
            if (pnode == null)
            {
                return;
            }
            if (MessageBox.Show("ȷ��Ҫɾ���ڵ�'" + pnode.Text + "'��", "ѯ��", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;
            string strTag = pnode.Tag.ToString();
            string NodeName = strTag;
            //�������ݼ��ڵ㣬�򽫽ڵ�����ת��һ��
            if (strTag.Contains("DataDIR"))
            {
                NodeName = "DataDIR";
            }
            switch (NodeName)
            {
                case "Root":
                    //���ڵ㲻����ɾ��
                    break;
                case "DIR":
                    Deletenode(pnode);
                    break;
                case "DataDIR":
                    Deletenode(pnode);
                    break;
                case "Layer":
                    Deletenode(pnode);
                    break;
                default:
                    break;
            }
        }
        //�޸��ļ���
        private void ModifyFolder(DevComponents.AdvTree.Node pNode)
        {
            //������xml�ж�Ӧ�ڵ�
            XmlDocument XMLDoc = new XmlDocument();
            XMLDoc.Load(_LayerXmlPath);
            string strSearch = "//" + pNode.Tag + "[@NodeKey='" + pNode.Name + "']";
            XmlNode pxmlnode = XMLDoc.SelectSingleNode(strSearch);
            if (pxmlnode == null)
                return;
            XmlElement pELe = pxmlnode as XmlElement;
            //��ȡxml�нڵ�����
            string Foldername = pELe.GetAttribute("NodeText");
            string FolderScrip = pELe.GetAttribute("Description");
            string strScale = "";
            if (pELe.HasAttribute("DataScale"))
            {
                strScale = pELe.GetAttribute("DataScale");
            }
            string strDataType = "";
            if (pELe.HasAttribute("DataType"))
            {
                strDataType = pELe.GetAttribute("DataType");
            }
            string strDIRType = "";
            if (pELe.HasAttribute("DIRType"))
            {
                strDIRType = pELe.GetAttribute("DIRType");
            }
            string strYear = "";
            if (pELe.HasAttribute("Year"))
            {
                strYear = pELe.GetAttribute("Year");
            }
            //deleted by chulili 20110921 ��һ��ר��ڵ�û��������
            //string strXZQ = "";
            //if (pELe.HasAttribute("XZQCode"))
            //{
            //    strXZQ = pELe.GetAttribute("XZQCode");
            //}
            //�����Ի����û��޸�
            FormAddFolder pFrm = new FormAddFolder(Plugin.ModuleCommon.TmpWorkSpace, Foldername, FolderScrip,strScale,strDataType,strYear,strDIRType,"�޸�ר��");
            if (pFrm.ShowDialog() != DialogResult.OK)
                return;
            pNode.Text = pFrm._Foldername;
            //�޸�����
            pELe.SetAttribute("NodeText", pFrm._Foldername);
            pELe.SetAttribute("Description", pFrm._FolderScrip);
            pELe.SetAttribute("DataScale", pFrm._Scale);
            pELe.SetAttribute("DataType", pFrm._DataType);
            pELe.SetAttribute("Year", pFrm._Year);
            pELe.SetAttribute("DIRType", pFrm._DIRType);
            //deleted by chulili 20110921 ��һ��ר��ڵ�û��������
            //pELe.SetAttribute("XZQCode", pFrm._XZQCode);
            XMLDoc.Save(_LayerXmlPath);
            //pNode.DataKey = pELe as object;
            ModuleMap.SetDataKey(pNode, pELe as XmlNode);
            SysCommon.ModSysSetting.IsLayerTreeChanged = true;

        }
        //�޸�ͼ��Ŀ¼���ڵ�
        private void ModifyRoot(DevComponents.AdvTree.Node pNode)
        {
            //������xml�ж�Ӧ�ڵ�
            XmlDocument XMLDoc = new XmlDocument();
            XMLDoc.Load(_LayerXmlPath);
            string strSearch = "//" + pNode.Tag + "[@NodeKey='" + pNode.Name + "']";
            XmlNode pxmlnode = XMLDoc.SelectSingleNode(strSearch);
            if (pxmlnode == null)
                return;
            XmlElement pELe = pxmlnode as XmlElement;
            //��ȡxml�нڵ�����
            string Foldername = pELe.GetAttribute("NodeText");
            string FolderScrip = pELe.GetAttribute("Description");
            //�����Ի����û��޸�
            FormAddDataSet pFrm = new FormAddDataSet(Foldername, FolderScrip, "�޸ĸ��ڵ�");
            if (pFrm.ShowDialog() != DialogResult.OK)
                return;
            pNode.Text = pFrm._DataSetname;
            //�޸�����
            pELe.SetAttribute("NodeText", pFrm._DataSetname);
            pELe.SetAttribute("Description", pFrm._DataSetScrip);
            XMLDoc.Save(_LayerXmlPath);
            SysCommon.ModSysSetting.IsLayerTreeChanged = true;
        }

        //ɾ���ڵ�
        private void Deletenode(DevComponents.AdvTree.Node pNode)
        {
            if (pNode == null)
            {
                return;
            }
            string strTag = pNode.Tag.ToString();
            string NodeName = strTag;
            //�ж��Ƿ������ݼ��ڵ�
            if (strTag.Contains("DataDIR"))
            {
                NodeName = "DataDIR";
            }
            //������xml�ж�Ӧ�ڵ�
            XmlDocument XMLDoc = new XmlDocument();
            XMLDoc.Load(_LayerXmlPath);
            string strSearch = "//" + NodeName + "[@NodeKey='" + pNode.Name + "']";
            XmlNode pxmlnode = XMLDoc.SelectSingleNode(strSearch);

            //�ֱ���xml�кͿؼ���ɾ���ڵ�
            //RemoveLayer(pNode);
            //added by chulili 20111011
            DevComponents.AdvTree.Node pParentNode = pNode.Parent;
            XmlNode pXmlParent = null;
            if (pxmlnode != null)
            {
                pXmlParent = pxmlnode.ParentNode;
            }

            //end added by chulili
            RemoveNodeFromMap(pNode );//changed by chulili ��Ӧ�ļ��м����ݼ��ڵ�,����ͼ�����ɾ��ͼ��
            pNode.Remove( );
            ModuleMap.SetDataKey(pParentNode, pXmlParent);

            //changed by chulili Ų��λ�� ��ʹpxmlnodeΪ�գ�ҲӦ����ɾ����ͼ�еĽڵ�
            if (pxmlnode == null)
            {
                SysCommon.ModSysSetting.IsLayerTreeChanged = true;
                return;
            }
            XmlNode pxmlParent = pxmlnode.ParentNode;
            if (pxmlParent != null)
            {
                pxmlParent.RemoveChild(pxmlnode);
            }
            //���汾��xml�ļ�
            XMLDoc.Save(_LayerXmlPath);
            SysCommon.ModSysSetting.IsLayerTreeChanged = true;
        }
        //�޸����ݼ�
        private void ModifyDataset(DevComponents.AdvTree.Node pNode)
        {
            //������xml�ж�Ӧ�ڵ�
            XmlDocument XMLDoc = new XmlDocument();
            XMLDoc.Load(_LayerXmlPath);
            string strTag = pNode.Tag.ToString();
            string NodeName = strTag;
            //��������ݼ��ڵ㣬����Ҫ�Խڵ�����������
            if (strTag.Contains("DataDIR"))
            {
                NodeName = "DataDIR";
            }
            string strSearch = "//" + NodeName + "[@NodeKey='" + pNode.Name + "']";
            XmlNode pxmlnode = XMLDoc.SelectSingleNode(strSearch);
            if (pxmlnode == null)
                return;
            XmlElement pELe = pxmlnode as XmlElement;
            string Datasetname = pELe.GetAttribute("NodeText");
            string DatasetScrip = pELe.GetAttribute("Description");
            //RemoveNodeFromMap(pNode);//added by chulili 20110630�ȴ���ͼ��ɾ�����ݼ��е�ͼ��
            //�����Ի����û��޸�

            FormAddDataSet pFrm = new FormAddDataSet(Datasetname, DatasetScrip,"�޸�ͼ����");
            DialogResult pResult = pFrm.ShowDialog();
            if (pResult != DialogResult.OK)

                return;
            RemoveNodeFromMap(pNode);
            pNode.Text = pFrm._DataSetname;
            pELe.SetAttribute("NodeText", pFrm._DataSetname);
            pELe.SetAttribute("Description", pFrm._DataSetScrip);
            //���汾��xml�ļ�
            XMLDoc.Save(_LayerXmlPath);
            //pNode.DataKey = pELe as object;
            ModuleMap.SetDataKey(pNode, pELe as XmlNode);
            SysCommon.ModSysSetting.IsLayerTreeChanged = true;
            if (pNode.Checked)
            {
                //m_pMapCtl.ActiveView.Deactivate();  //��ˢ��
                AddNodeToMap(pNode);
                //m_pMapCtl.ActiveView.Refresh();
                //m_pMapCtl.ActiveView.Activate(m_pMapCtl.hWnd);  //ˢ��
            }
        }
        //�޸�ͼ��
        private void ModifyLayer(DevComponents.AdvTree.Node pnode)
        {
            if (pnode == null) return;
            //����ͼ��ɾ��ԭ����ͼ����ͼ
            XmlNode pLayerNode = pnode.DataKey as XmlNode;
            if(pLayerNode==null) return;
            XmlElement pLayerEle=pLayerNode as XmlElement;
            string strDataType="";
            if(pLayerEle.HasAttribute("DataType"))
            {
                strDataType=pLayerEle.GetAttribute("DataType");
            }
            if (!strDataType.Contains("SERVICE"))
            {
                FormAddLayer pFrm = new FormAddLayer(null,_LayerXmlPath, m_pWks, pnode, false, this.ImageList,1);
                DialogResult pResult= pFrm.ShowDialog();

                //shduan 20110625 add�ж�

                if (pFrm.m_bIsModify && pResult==DialogResult.OK )
                {
                    RemoveLayer(pnode);
                    DevComponents.AdvTree.Node pNewNode = pFrm.CurNode;
                    if (pNewNode == null) return;
                    if (pNewNode.Checked)
                    {
                        //��ʾ�µ�ͼ�㵽��ͼ���
                        AddOrDelLyr(pNewNode, pNewNode.Checked);
                    }
                }
            }
            else
            {
                FormModifyServiceLayer pFrmModify = new FormModifyServiceLayer(_LayerXmlPath, m_pWks,pnode);
                DialogResult pRes = pFrmModify.ShowDialog();
                if (pRes == DialogResult.OK && pFrmModify._Changed ) 
                {
                    RemoveLayer(pnode);

                    if (pnode.Checked)
                    {
                        //��ʾ�µ�ͼ�㵽��ͼ���
                        AddOrDelLyr(pnode, pnode.Checked);
                    }
                }
            }
        }
        public void ZoomToLayer()
        {
            if (this.TreeDataLib.SelectedNode == null)
            {
                return;
            }
            //��ȡ��ѡͼ��ڵ�
            DevComponents.AdvTree.Node vNode = TreeDataLib.SelectedNode;
            //������ͼ��ڵ㣬������
            if (!vNode.Tag.ToString().Equals("Layer"))
                return;

            if (vNode.DataKey == null) return;
            try
            {
                //��ȡxml�ڵ�
                XmlNode layerNode = vNode.DataKey as XmlNode;
                string nodeKey = layerNode.Attributes["NodeKey"].Value;

                ILayer addLayer = null;
                //��ȡͼ��
                if (_DicAddLyrs.ContainsKey(nodeKey))
                    addLayer = _DicAddLyrs[nodeKey];    //���Ѽ��ص���ǰ��ͼ
                else if (_DicDelLyrs.ContainsKey(nodeKey))
                {
                    addLayer = _DicDelLyrs[nodeKey];    //���ӵ�ǰж��
                }
                else
                {   //��������ӵ�ͼ��
                    Exception errLayer = null;
                    addLayer = ModuleMap.AddLayerFromXml(ModuleMap._DicDataLibWks, layerNode, m_pWks, "", null, out errLayer);
                }
                if (addLayer == null) return;
                //���ŵ���ͼ��
                IActiveView pActiveView = m_pMapCtl.Map as IActiveView;
                pActiveView.Extent = addLayer.AreaOfInterest;
               
                //�������ŵ�ͼ�� ������������ xisheng 20111117********************************************
                //if (addLayer.MinimumScale > 0)
                  //  m_pMapCtl.Map.MapScale = addLayer.MinimumScale;//yjl20111031 add zoomtovisible 
                //�������ŵ�ͼ�� ������������ xisheng 20111117*****************************************end

                pActiveView.Refresh();
            }
            catch (Exception eError)
            {
                if (SysCommon.Log.Module.SysLog == null) SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(eError);
            }
        }
        //���ŵ�ͼ�� added by chulili 2011-06-17
        private void MenuItemZoomToLayer_Click(object sender, EventArgs e)
        {
            ZoomToLayer();
        }
        //shduan 20110625
        private void TreeDataLib_KeyPress(object sender, KeyPressEventArgs e)
        {
            //if(e.KeyChar  ==4)
            //DeleteTreeNode();
        }

        private void TreeDataLib_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {

                DeleteTreeNode();
            }
        }
        public void AutoMathLayerConfig()
        {
            DevComponents.AdvTree.Node pNode = TreeDataLib.SelectedNode;// layerTree.SelectedNode;
            if (pNode == null)
                return;
            //RemoveNodeFromMap(pNode);//�ȴ���ͼ��ɾ���ڵ�
            FormAutoMatch pFrm = new FormAutoMatch(this,_LayerXmlPath, m_pWks, pNode);
            DialogResult pResult= pFrm.ShowDialog();

            //XmlDocument pXmldoc = new XmlDocument();
            //pXmldoc.Load(_LayerXmlPath);
            ////���Ż��Զ�ƥ��
            //AutoMatchRender(pNode, pXmldoc);
            //pXmldoc.Save(_LayerXmlPath);
            //ModuleMap.IsLayerTreeChanged = true;
            //������ͼ����ӽڵ�
            if (pResult == DialogResult.OK)
            {
                if (pNode.Checked)
                {
                    //AddNodeToMap(pNode);
                    m_pMapCtl.ActiveView.Refresh();
                }
            }
            if (pResult == DialogResult.OK)
            {
                MessageBox.Show("�Զ�ƥ����ųɹ���", "ϵͳ��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        //�Զ�ƥ�����
        private void MenuItemSetRender_Click(object sender, EventArgs e)
        {
            AutoMathLayerConfig();
        }
        //�Ҽ��˵�����������Դ
        public void SetDbSource()
        {
            DevComponents.AdvTree.Node pNode = TreeDataLib.SelectedNode;// layerTree.SelectedNode;
            if (pNode == null)
                return;
            try
            {
                //RemoveNodeFromMap(pNode);//�ȴ���ͼ��ɾ���ڵ�
                FormSetDBsource pFrm = new FormSetDBsource(this,_LayerXmlPath, m_pWks, pNode);
                if (pFrm.ShowDialog() == DialogResult.OK)
                {
                    if (pNode.Checked)
                    {
                        //AddNodeToMap(pNode);
                        m_pMapCtl.ActiveView.Refresh();
                    }
                }
                pFrm = null;
            }
            catch
            { }
        }
        private void MenuItemSetDbsource_Click(object sender, EventArgs e)
        {
            SetDbSource();
        }
        //����ͼ����ӽڵ㣨��RemoveNodeFromMap������Ӧ��
        public void AddNodeToMap(DevComponents.AdvTree.Node pNode)
        {
            string strtag = pNode.Tag.ToString();
            if (strtag.Contains("DataDIR"))
            {
                strtag = "DataDIR";
            }
            switch (strtag)
            {
                case "Root":
                case "DIR":
                case "DataDIR":
                    //for (int i = 0; i < pNode.Nodes.Count; i++)
                    //{
                    //    DevComponents.AdvTree.Node tmpnode = pNode.Nodes[i];
                    //    AddNodeToMap(tmpnode);
                    //}
                    AddOrDelDataDir(pNode, pNode.CheckState);
                    break;
                case "Layer":
                    if (pNode.Checked)
                    {
                        AddOrDelLyr(pNode, pNode.Checked);
                    }

                    break;
            }
        }
        //����ͼ��ɾ���ڵ�
        public void RemoveNodeFromMap(DevComponents.AdvTree.Node pNode)
        {
            string strtag = pNode.Tag.ToString();
            if (strtag.Contains("DataDIR"))
            {
                strtag = "DataDIR";
            }
            switch (strtag)
            {
                case "Root":
                case "DIR":
                case "DataDIR":
                    for (int i = 0; i < pNode.Nodes.Count; i++)
                    {
                        DevComponents.AdvTree.Node tmpnode = pNode.Nodes[i];
                        RemoveNodeFromMap(tmpnode);
                        
                    }
                    RemoveDataDIRfromMap(pNode);
                    break;
                case "Layer":
                    RemoveLayer(pNode);
                    break;
            }
        }
        public void AddGroup()
        {
            DevComponents.AdvTree.Node pNode = TreeDataLib.SelectedNode;// layerTree.SelectedNode;
            if (pNode == null)
                return;
            //�Դ�������޸ģ��ϸ�������������ļ��еĽڵ�����
            if (!pNode.Tag.ToString().Equals("DIR") && !pNode.Tag.ToString().Contains("Root") && !pNode.Tag.ToString().Contains("DataDIR"))
                return;
            try
            {
                //�����Ի���
                FormAddDataSet pFrm = new FormAddDataSet();
                if (pFrm.ShowDialog() != DialogResult.OK)
                    return;
                //���ݴӶԻ��򷵻ص�����ֵ�����ýڵ�
                DevComponents.AdvTree.Node addNode = new DevComponents.AdvTree.Node();
                addNode.Text = pFrm._DataSetname;
                addNode.Tag = "DataDIR";
                addNode.Image = this.ImageList.Images["DataDIROpen"];
                addNode.CheckBoxVisible = true;
                addNode.CheckState = CheckState.Checked;
                string nodekey = Guid.NewGuid().ToString();
                addNode.Name = nodekey;
                pNode.Nodes.Add(addNode);
                //�����Ӧ��xml�ڵ�
                XmlDocument XMLDoc = new XmlDocument();
                XMLDoc.Load(_LayerXmlPath);
                //shduan 20110612 �޸�*******************************************************************************************
                //string strSearch = "//" + pNode.Tag + "[@NodeKey='" + pNode.Name + "']";
                //chulili��pNode�ļ����Ѿ��ϸ��޶�����˲����ж�pNode.Tag 
                string strSearch = "//" + pNode.Tag + "[@NodeKey='" + pNode.Name + "']";

                //****************************************************************************************************************
                XmlNode pxmlnode = XMLDoc.SelectSingleNode(strSearch);
                XmlElement childele = XMLDoc.CreateElement("DataDIR");

                childele.SetAttribute("NodeKey", nodekey);
                childele.SetAttribute("NodeText", pFrm._DataSetname);
                childele.SetAttribute("Description", pFrm._DataSetScrip);
                childele.SetAttribute("Enabled", "true");
                childele.SetAttribute("Expand", "100");
                pxmlnode.AppendChild(childele as XmlNode);
                XMLDoc.Save(_LayerXmlPath);
                //addNode.DataKey = childele as object ;
                ModuleMap.SetDataKey(addNode, childele as XmlNode);
                SysCommon.ModSysSetting.IsLayerTreeChanged = true;
            }
            catch
            { }
        }
        private void MenuItemAddGroup_Click(object sender, EventArgs e)
        {
            AddGroup();
        }
        //added by chulili 20110709 �ж��Ҽ��˵�����״̬
        //added by chulili 20110709 �ж��Ҽ��˵�����״̬
        private void TreeDataLib_NodeMouseDown(object sender, TreeNodeMouseEventArgs e)
        {
            System.Drawing.Point pPoint = new System.Drawing.Point(e.X, e.Y);

            if (e.Node == null)
            {
                return;
            }
            if (e.Button != MouseButtons.Right)
            {
                return;
            }
            string nodetag = e.Node.Tag.ToString();
            TreeDataLib.SelectedNode = e.Node;
            if (nodetag.Contains("DataDIR"))
            {
                nodetag = "DataDIR";
            }
            DevComponents.DotNetBar.ButtonItem item = null;
            if (!_isLayerConfig)//չʾϵͳ�Ҽ��˵�
            {

                if (_MapContextMenu != null) this.Controls.Add(_MapContextMenu);
                if (_LayerContextMenu != null) this.Controls.Add(_LayerContextMenu);
                switch (nodetag)
                {

                    case "Root":
                        //    if (_MapContextMenu != null)
                        //    {
                        //        item = _MapContextMenu.Items[0] as DevComponents.DotNetBar.ButtonItem;
                        //        if (item != null)
                        //        {
                        //            item.Popup(TreeDataLib.PointToScreen(pPoint));
                        //        }
                        //    }
                        break;
                    case "Layer":

                        //��ȡxml�ڵ�
                        if (e.Node.DataKey != null)
                        {
                            XmlNode layerNode = e.Node.DataKey as XmlNode;
                            string nodeKey = "";
                            if ((layerNode as XmlElement).HasAttribute("NodeKey"))
                            {
                                nodeKey = layerNode.Attributes["NodeKey"].Value;
                            }
                            ILayer addLayer = null;
                            //��ȡͼ��
                            Exception errLayer = null;
                            if (_DicAddLyrs.ContainsKey(nodeKey))
                                addLayer = _DicAddLyrs[nodeKey];    //���Ѽ��ص���ǰ��ͼ
                            else if (_DicDelLyrs.ContainsKey(nodeKey))
                            {
                                addLayer = _DicDelLyrs[nodeKey];    //���ӵ�ǰж��
                            }
                            else
                            {   //��������ӵ�ͼ��
                                addLayer = ModuleMap.AddLayerFromXml(SysCommon.ModuleMap._DicDataLibWks, layerNode, m_pWks, "", null, out errLayer);
                            }

                            if (_TocControl.Buddy is IPageLayoutControl2)
                            {
                                IPageLayoutControl2 pPageLayoutControl = _TocControl.Buddy as IPageLayoutControl2;
                                pPageLayoutControl.CustomProperty = addLayer;

                            }
                            else if (_TocControl.Buddy is IMapControl3)
                            {
                                IMapControl3 pMapcontrol = _TocControl.Buddy as IMapControl3;
                                pMapcontrol.CustomProperty = addLayer;
                            }
                        }

                        break;
                    case "OutLayer"://�ⲿ���ص�ͼ�� added by chulili 20110902
                        //��ȡxml�ڵ�
                        if (e.Node.DataKey != null)
                        {
                            ILayer pLayer = e.Node.DataKey as ILayer;
                            //if (_LayerContextMenu != null)
                            //{
                            //    item = _LayerContextMenu.Items[0] as DevComponents.DotNetBar.ButtonItem;
                            //    /*xisheng 20110909 �޸��Ҽ��˵�λ�� */
                            //    int y = TreeDataLib.PointToScreen(pPoint).Y;
                            //    int  x = TreeDataLib.PointToScreen(pPoint).X;
                            //    if ((y + 170) > SystemInformation.WorkingArea.Height)
                            //        y = SystemInformation.WorkingArea.Height - 170;
                            //    if (item != null)
                            //    {
                            //        //item.Popup(TreeDataLib.PointToScreen(pPoint));
                            //        item.Popup(x, y);
                            //    }
                            //}

                            if (_TocControl.Buddy is IPageLayoutControl2)
                            {
                                IPageLayoutControl2 pPageLayoutControl = _TocControl.Buddy as IPageLayoutControl2;
                                pPageLayoutControl.CustomProperty = pLayer;

                            }
                            else if (_TocControl.Buddy is IMapControl3)
                            {
                                IMapControl3 pMapcontrol = _TocControl.Buddy as IMapControl3;
                                pMapcontrol.CustomProperty = pLayer;
                            }
                        }
                        break;
                }
                if (_LayerContextMenu != null)
                {
                    item = _LayerContextMenu.Items[0] as DevComponents.DotNetBar.ButtonItem;
                    string strTagLower = nodetag.ToLower();
                    # region �˵�����֮ǰ���ж�����Ŀɼ�״̬����δ����������ļ��й�����������֪�Ӳ˵��������ǰ����д�� chulili 20111216
                    try//added by chulili 20111216 ���󱣻����û��޸��˲˵������ļ��󣬿����е������Ҳ���
                    {
                        if (strTagLower == "root")
                        {
                            item.SubItems["GeoSysUpdate.ControlsAddDataCommand"].Visible = true;    //�����ⲿ����
                            item.SubItems["GeoUtilities.ControlsSetCoordinateSys1"].Visible = true; //���ÿռ�ο�
                            item.SubItems["GeoSysUpdate.ControlsAddjustLayerOrder"].Visible = true; //����ͼ��˳��
                            item.SubItems["GeoSysUpdate.ControlsExpandAllNode"].Visible = true;     //չ���ڵ�
                            item.SubItems["GeoSysUpdate.ControlsCollapseAllNode"].Visible = true;   //�۵��ڵ�
                            item.SubItems["GeoUtilities.ControlsSetLimitScale1"].Visible = true;    //�������Ʊ�����
                            item.SubItems["GeoUtilities.ControlsZoomToLayer"].Visible = false;      //���ŵ�ͼ��
                            item.SubItems["GeoUtilities.ControlsZoomToVisibleScale1"].Visible = false;   //���ŵ��ɼ�������
                            item.SubItems["GeoSysUpdate.ControlsRemoveLayer"].Visible = false;       //�Ƴ�ͼ��
                            item.SubItems["GeoUtilities.ControlsSetLabel"].Visible = false;      //���ñ�ע
                            item.SubItems["GeoUtilities.ControlsDisplayLabel"].Visible = false;  //�Ƴ���ע
                            item.SubItems["GeoUserManager.CommandSymbol"].Visible = false;       //��������
                            item.SubItems["GeoUtilities.ControlsLayerAttribute"].Visible = false;    //�����Ա�
                            item.SubItems["GeoUtilities.ControlsLayerAtt"].Visible = false;      //ͼ������
                        }
                        else if (strTagLower.Contains("layer"))
                        {
                            item.SubItems["GeoSysUpdate.ControlsAddDataCommand"].Visible = false;    //�����ⲿ����
                            item.SubItems["GeoUtilities.ControlsSetCoordinateSys1"].Visible = false; //���ÿռ�ο�
                            item.SubItems["GeoSysUpdate.ControlsAddjustLayerOrder"].Visible = true; //����ͼ��˳��
                            item.SubItems["GeoSysUpdate.ControlsExpandAllNode"].Visible = false;     //չ���ڵ�
                            item.SubItems["GeoSysUpdate.ControlsCollapseAllNode"].Visible = false;   //�۵��ڵ�
                            item.SubItems["GeoUtilities.ControlsSetLimitScale1"].Visible = true;    //�������Ʊ�����
                            item.SubItems["GeoUtilities.ControlsZoomToLayer"].Visible = true;      //���ŵ�ͼ��
                            item.SubItems["GeoUtilities.ControlsZoomToVisibleScale1"].Visible = true;   //���ŵ��ɼ�������
                            item.SubItems["GeoSysUpdate.ControlsRemoveLayer"].Visible = true;       //�Ƴ�ͼ��
                            item.SubItems["GeoUtilities.ControlsSetLabel"].Visible = true;      //���ñ�ע
                            item.SubItems["GeoUtilities.ControlsDisplayLabel"].Visible = true;  //�Ƴ���ע
                            item.SubItems["GeoUserManager.CommandSymbol"].Visible = true;       //��������
                            item.SubItems["GeoUtilities.ControlsLayerAttribute"].Visible = true;    //�����Ա�
                            item.SubItems["GeoUtilities.ControlsLayerAtt"].Visible = true;      //ͼ������
                        }
                        else
                        {
                            item.SubItems["GeoSysUpdate.ControlsAddDataCommand"].Visible = false;    //�����ⲿ����
                            item.SubItems["GeoUtilities.ControlsSetCoordinateSys1"].Visible = false; //���ÿռ�ο�
                            item.SubItems["GeoSysUpdate.ControlsAddjustLayerOrder"].Visible = true; //����ͼ��˳��
                            item.SubItems["GeoSysUpdate.ControlsExpandAllNode"].Visible = true;     //չ���ڵ�
                            item.SubItems["GeoSysUpdate.ControlsCollapseAllNode"].Visible = true;   //�۵��ڵ�
                            item.SubItems["GeoUtilities.ControlsSetLimitScale1"].Visible = true;    //�������Ʊ�����
                            item.SubItems["GeoUtilities.ControlsZoomToLayer"].Visible = false;      //���ŵ�ͼ��
                            item.SubItems["GeoUtilities.ControlsZoomToVisibleScale1"].Visible = false;   //���ŵ��ɼ�������
                            item.SubItems["GeoSysUpdate.ControlsRemoveLayer"].Visible = false;       //�Ƴ�ͼ��
                            item.SubItems["GeoUtilities.ControlsSetLabel"].Visible = false;      //���ñ�ע
                            item.SubItems["GeoUtilities.ControlsDisplayLabel"].Visible = false;  //�Ƴ���ע
                            item.SubItems["GeoUserManager.CommandSymbol"].Visible = false;       //��������
                            item.SubItems["GeoUtilities.ControlsLayerAttribute"].Visible = false;    //�����Ա�
                            item.SubItems["GeoUtilities.ControlsLayerAtt"].Visible = false;      //ͼ������
                        }
                    }
                    catch
                    { }
                    # endregion
                    /*xisheng 20110909 �޸��Ҽ��˵�λ�� */
                    int y = TreeDataLib.PointToScreen(pPoint).Y, x = TreeDataLib.PointToScreen(pPoint).X;
                    if ((y + 170) > SystemInformation.WorkingArea.Height)
                        y = SystemInformation.WorkingArea.Height - 170;

                    if (item != null)
                    {
                        //item.Popup(TreeDataLib.PointToScreen(pPoint));
                        item.Popup(x, y);
                    }
                    //end
                }


            }


            else//������ϵͳ�Ҽ��˵�
            {

                if (_LayerTreeManageContextMenu != null)
                {
                    this.Controls.Add(_LayerTreeManageContextMenu);
                    item = _LayerTreeManageContextMenu.Items[0] as DevComponents.DotNetBar.ButtonItem;
                    if (item != null)
                    {
                        item.Popup(TreeDataLib.PointToScreen(pPoint));
                    }
                }
                if (e.Node.DataKey != null)
                {
                    XmlNode layerNode = e.Node.DataKey as XmlNode;
                    string nodeKey = "";
                    if ((layerNode as XmlElement).HasAttribute("NodeKey"))
                    {
                        nodeKey = layerNode.Attributes["NodeKey"].Value;
                    }
                    ILayer addLayer = null;
                    //��ȡͼ��
                    Exception errLayer = null;
                    if (_DicAddLyrs.ContainsKey(nodeKey))
                        addLayer = _DicAddLyrs[nodeKey];    //���Ѽ��ص���ǰ��ͼ
                    if (_TocControl != null)
                    {
                        if (_TocControl.Buddy is IPageLayoutControl2)
                        {
                            IPageLayoutControl2 pPageLayoutControl = _TocControl.Buddy as IPageLayoutControl2;
                            pPageLayoutControl.CustomProperty = addLayer;

                        }
                        else if (_TocControl.Buddy is IMapControl3)
                        {
                            IMapControl3 pMapcontrol = _TocControl.Buddy as IMapControl3;
                            pMapcontrol.CustomProperty = addLayer;
                        }
                    }
                }

                //this.contextMenuLayerTree.Visible = true;
                //switch (nodetag)
                //{
                //    case "Root"://���ڵ�
                //        //������ͨ�ò˵������м���Ľڵ㶼����
                //        //this.contextMenuLayerTree.Items["MenuItemSetDbsource"].Enabled = true;
                //        //this.contextMenuLayerTree.Items["MenuItemSetRender"].Enabled = true;

                //        this.contextMenuLayerTree.Items["MenuItemAddFolder"].Enabled = true;
                //        this.contextMenuLayerTree.Items["MenuItemAddGroup"].Enabled = true;
                //        this.contextMenuLayerTree.Items["MenuItemAddLayer"].Enabled = true;

                //        this.contextMenuLayerTree.Items["MenuItemAddDataset"].Enabled = true;

                //        //this.contextMenuLayerTree.Items["MenuItemModify"].Enabled = true;
                //        this.contextMenuLayerTree.Items["MenuDelete"].Enabled = false;

                //        this.contextMenuLayerTree.Items["MenuItemZoomToLayer"].Enabled = false;
                //        break;
                //    case "DIR"://�ļ��нڵ㣨ר��ڵ㣩
                //        //this.contextMenuLayerTree.Items["MenuItemSetDbsource"].Enabled = true;
                //        //this.contextMenuLayerTree.Items["MenuItemSetRender"].Enabled = true;

                //        this.contextMenuLayerTree.Items["MenuItemAddFolder"].Enabled = true;
                //        this.contextMenuLayerTree.Items["MenuItemAddGroup"].Enabled = true;
                //        this.contextMenuLayerTree.Items["MenuItemAddLayer"].Enabled = true;

                //        this.contextMenuLayerTree.Items["MenuItemAddDataset"].Enabled = true;

                //        //this.contextMenuLayerTree.Items["MenuItemModify"].Enabled = true;
                //        this.contextMenuLayerTree.Items["MenuDelete"].Enabled = true;

                //        this.contextMenuLayerTree.Items["MenuItemZoomToLayer"].Enabled = false;
                //        break;
                //    case "DataDIR"://���ݼ��ڵ㣨ͼ����ڵ㣩
                //        //this.contextMenuLayerTree.Items["MenuItemSetDbsource"].Enabled = true;
                //        //this.contextMenuLayerTree.Items["MenuItemSetRender"].Enabled = true;

                //        this.contextMenuLayerTree.Items["MenuItemAddFolder"].Enabled = false;
                //        this.contextMenuLayerTree.Items["MenuItemAddGroup"].Enabled = false;
                //        this.contextMenuLayerTree.Items["MenuItemAddLayer"].Enabled = true;

                //        this.contextMenuLayerTree.Items["MenuItemAddDataset"].Enabled = true;//changed by chulili 20110808�����ڷ���ڵ���������ݼ�

                //        //this.contextMenuLayerTree.Items["MenuItemModify"].Enabled = true;
                //        this.contextMenuLayerTree.Items["MenuDelete"].Enabled = true;

                //        this.contextMenuLayerTree.Items["MenuItemZoomToLayer"].Enabled = false;
                //        break;
                //    case "Layer"://ͼ��ڵ�
                //        //this.contextMenuLayerTree.Items["MenuItemSetDbsource"].Enabled = true ;
                //        //this.contextMenuLayerTree.Items["MenuItemSetRender"].Enabled = true;

                //        this.contextMenuLayerTree.Items["MenuItemAddFolder"].Enabled = false;
                //        this.contextMenuLayerTree.Items["MenuItemAddGroup"].Enabled = false;
                //        this.contextMenuLayerTree.Items["MenuItemAddLayer"].Enabled = false;

                //        this.contextMenuLayerTree.Items["MenuItemAddDataset"].Enabled = false;

                //        //this.contextMenuLayerTree.Items["MenuItemModify"].Enabled = true;
                //        this.contextMenuLayerTree.Items["MenuDelete"].Enabled = true;

                //        this.contextMenuLayerTree.Items["MenuItemZoomToLayer"].Enabled = true;
                //        break;
                //}

            }
        }
        //��������ͼ������  ���� ��ʾ �ɱ༭ �ɲ�ѯ  ��ѡ��  �Լ���� ��С������
        public void SetLayerAttributes()
        {
            DevComponents.AdvTree.Node pNode = TreeDataLib.SelectedNode;// layerTree.SelectedNode;
            if (pNode == null)
                return;
            try
            {
                //RemoveNodeFromMap(pNode);//�ȴ���ͼ��ɾ���ڵ�
                FormSetLayerAtt pFrm = new FormSetLayerAtt(this,_LayerXmlPath, pNode);
                DialogResult pResult= pFrm.ShowDialog();
                if (pResult == DialogResult.OK)
                {
                    if (pFrm.GetLoad())
                    {
                        SysCommon.CProgress vProgress = null;
                        vProgress = new SysCommon.CProgress("����'" + pNode.Text + "'");
                        //vProgress.EnableCancel = false;
                        vProgress.ShowDescription = false ;
                        vProgress.FakeProgress = true;
                        vProgress.TopMost = true;
                        vProgress.EnableCancel = true;
                        vProgress.EnableUserCancel(true);
                        vProgress.ShowProgress();
                        SetCheckState(pNode, CheckState.Checked, vProgress);
                        vProgress.Close();
                        vProgress = null;
                    }
                    else
                    {
                        SetCheckState(pNode, CheckState.Unchecked,null);
                    }
                }
                //if (pNode.Checked)
                //{
                //    AddNodeToMap(pNode);
                //    m_pMapCtl.ActiveView.Refresh();
                //}
                pFrm = null;
            }
            catch
            { }
        }
        private void SetCheckState(DevComponents.AdvTree.Node pNode,CheckState pCheckstate,SysCommon.CProgress vprog)
        {
            if (pNode == null)
                return;
            string strtag = pNode.Tag.ToString();
            if (strtag.Contains("DataDIR"))
            {
                strtag = "DataDIR";
            }
            switch (strtag)
            {
                case "Root":
                case"DIR":         
                    
                case "DataDIR":
                case "Layer":
                    pNode.SetChecked(pCheckstate,DevComponents.AdvTree.eTreeAction.Mouse);
                    break;
            }
            
        }
        public void RefreshOrderIDofLayer(string strNodeKey,string strXML)
        {
            ILayer pLayer = null;
            if (_DicAddLyrs != null)
            {
                if (_DicAddLyrs.Keys.Contains(strNodeKey))
                {
                    pLayer = _DicAddLyrs[strNodeKey];

                }
                else if (_DicDelLyrs != null)
                {
                    if (_DicDelLyrs.Keys.Contains(strNodeKey))
                    {
                        pLayer = _DicDelLyrs[strNodeKey];
                    }
                }
            }
            if (pLayer != null)
            {
                ILayerGeneralProperties pLayerGenPro = pLayer as ILayerGeneralProperties;
                //��ȡͼ�������
                pLayerGenPro.LayerDescription = strXML;
            }
        }
        public void CopyLayer()
        {
            _CopyLayerNode = null;
            if (this.TreeDataLib.SelectedNode == null)
            {
                return;
            }
            //��ȡ��ѡͼ��ڵ�
            DevComponents.AdvTree.Node vNode = TreeDataLib.SelectedNode;
            //������ͼ��ڵ㣬������
            if (!vNode.Tag.ToString().Equals("Layer"))
                return;

            if (vNode.DataKey == null) return;
            try
            {
                //��ȡxml�ڵ�
                XmlNode layerNode = vNode.DataKey as XmlNode;
                string nodeKey = layerNode.Attributes["NodeKey"].Value;

                ILayer addLayer = null;
                //��ȡͼ��
                if (_DicAddLyrs.ContainsKey(nodeKey))
                    addLayer = _DicAddLyrs[nodeKey];    //���Ѽ��ص���ǰ��ͼ
                else if (_DicDelLyrs.ContainsKey(nodeKey))
                {
                    addLayer = _DicDelLyrs[nodeKey];    //���ӵ�ǰж��
                }
                else
                {   //��������ӵ�ͼ��
                    Exception errLayer = null;
                    addLayer = ModuleMap.AddLayerFromXml(ModuleMap._DicDataLibWks, layerNode, m_pWks, "", null, out errLayer);
                }
                if (addLayer == null) return;

                _CopyLayerNode = vNode.Copy();
            }
            catch (Exception eError)
            {
                if (SysCommon.Log.Module.SysLog == null) SysCommon.Log.Module.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                SysCommon.Log.Module.SysLog.Write(eError);
            }
        }
        private void MenuItemPasteLayer_Click(object sender, EventArgs e)
        {
            PasteLayer();
        }
        public void PasteLayer()
        {
            if (_CopyLayerNode == null)
            {
                return;
            }
            if (this.TreeDataLib.SelectedNode == null)
            {
                return;
            }
            //��ȡ��ѡͼ��ڵ�
            bool bCheckState = _CopyLayerNode.Checked;

            _CopyLayerNode.CheckState = CheckState.Unchecked;
            DevComponents.AdvTree.Node vNode = TreeDataLib.SelectedNode;
            //������ͼ��ڵ㣬������
            if (!vNode.Tag.ToString().Equals("Layer"))
                return;
            if (vNode.Parent != null)
            {
                DevComponents.AdvTree.Node pParent = vNode.Parent;
                
                XmlDocument XMLDoc = new XmlDocument();
                XMLDoc.Load(_LayerXmlPath);
                //shduan 20110612 �޸�*******************************************************************************************
                //string strSearch = "//" + pNode.Tag + "[@NodeKey='" + pNode.Name + "']";
                string strSearch = "";
                if (vNode.Tag.ToString().Contains("DataDIR"))
                {
                    strSearch = "//DataDIR" + "[@NodeKey='" + vNode.Name + "']";
                }
                else
                {
                    strSearch = "//" + vNode.Tag + "[@NodeKey='" + vNode.Name + "']";
                }
                string strCopy = "";
                strCopy = "//" + _CopyLayerNode.Tag + "[@NodeKey='" + _CopyLayerNode.Name + "']";

                //****************************************************************************************************************
                XmlNode pxmlnode = XMLDoc.SelectSingleNode(strSearch);
                XmlNode pxmlnodeCopy = XMLDoc.SelectSingleNode(strCopy);
                if (pxmlnode != null)
                {
                    XmlNode pParentXmlnode = pxmlnode.ParentNode;
                    if (pParent != null && pParentXmlnode!=null)
                    {
                        
                        XmlNode pnewNode = pxmlnodeCopy.Clone();
                        XmlElement pNewEle=pnewNode as XmlElement;
                        string strNewNodeKey = System.Guid.NewGuid().ToString();
                        pNewEle.SetAttribute("NodeKey", strNewNodeKey);
                        pNewEle.SetAttribute("NodeText", _CopyLayerNode.Text+"����");
                        _CopyLayerNode.Text = _CopyLayerNode.Text + "����";
                        _CopyLayerNode.Name = strNewNodeKey;
                        pParentXmlnode.InsertBefore(pnewNode, pxmlnode);
                        _CopyLayerNode.DataKey = pnewNode as object;
                        pParent.Nodes.Insert(vNode.Index, _CopyLayerNode);

                        
                        XMLDoc.Save(_LayerXmlPath);
                        if (bCheckState)
                        {
                            _CopyLayerNode.SetChecked(true, eTreeAction.Mouse);
                        }
                        _CopyLayerNode = _CopyLayerNode.Copy();
                    }
                    
                }
                XMLDoc = null;
            }
        }
    }
}
