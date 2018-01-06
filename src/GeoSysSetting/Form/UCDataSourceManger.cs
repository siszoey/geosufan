using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using SysCommon.Gis;
namespace GeoSysSetting.SubControl
{
    public partial class UCDataSourceManger : UserControl
    {
        public IWorkspace m_TmpWorkSpace = null;
        //�Ҽ��˵�����
        
        //private String _layerTreePath = Application.StartupPath + "\\..\\res\\xml\\����ͼ����.xml";
        
        //changed by chulili 20110722����������Դ���� IAppPrivilegesRef->IAppFormRef
        public Plugin.Application.IAppFormRef m_AppFrmRef = null;
        private ILayer _SelSymLayer = null;

        private List<string> _DeleteNodeKeys = new List<string >();
        private string _Decimalstr = "";//added by chulili 20111008 ״̬����ǰ������С��λ����ʽ������
        public UCDataSourceManger(Plugin.Application.IAppFormRef hook)
        {
            InitializeComponent();
            //added by chulili 20111010 
            m_AppFrmRef = hook;
            Plugin.Application.IAppDBIntegraRef pDBIntegra = hook as Plugin.Application.IAppDBIntegraRef;
            Plugin.Application.IAppFormRef pAppFrm = hook as Plugin.Application.IAppFormRef;
            pAppFrm.LayerTree = this.layerTree as object;
            
            pDBIntegra.CurScaleVisible = true;
            pDBIntegra.MapControl = this.MapControlLayer.Object  as IMapControlDefault ;
            pDBIntegra.TOCControl = this.axTOCControl.Object as ITOCControlDefault ;
            //end added
            InitLayerTree();
            axTOCControl.SetBuddyControl(MapControlLayer.Object );
            
        }
        //��ʼ��ͼ���� added by chulili 20110601
        public void InitLayerTree()
        {
            this.MapControlLayer.Map.Name = "����ͼ��";
            //ZQ  20110827  add
            this.layerTree.isDragDrop = false;  //changed by chulili 20111107 ��������ק
            //end
            this.layerTree.isLayerConfig = true;
            this.layerTree.LayerVisible = false;
            this.layerTree.CurMap = this.MapControlLayer.Object  as ESRI.ArcGIS.Controls.IMapControl2 ;
            this.layerTree.CurWks = Plugin.ModuleCommon.TmpWorkSpace ;
            //string strpath = Application.StartupPath+"\\..\\res\\xml\\ͼ����.xml";
            if (GeoDBIntegration.ModuleData.v_AppDBIntegra.DicContextMenu.ContainsKey("ContextLayerTreeManage"))
            {
                this.layerTree.LayerTreeManageContextMenu = GeoDBIntegration.ModuleData.v_AppDBIntegra.DicContextMenu["ContextLayerTreeManage"];
            }
            SysCommon.ModSysSetting.CopyLayerTreeXmlFromDataBase(Plugin.ModuleCommon.TmpWorkSpace, ModPublicFun._layerTreePath);
            this.layerTree.TocControl = this.axTOCControl.Object as ITOCControlDefault ;
            this.layerTree.DicMenu = null;
            this.layerTree.LayerXmlPath = ModPublicFun._layerTreePath;
            this.layerTree.InitLayerDic();
            this.layerTree.LoadData();

            GetScaleDecimal();//added by chulili 20110816 ��ȡ���������ã�С��λ����
            
        }

        //added by chulili 20110722��Ӻ��������ж�״̬��ֱ��ˢ��
        public void RefreshLayerTreeEx()
        {
               //��ʼ��ͼ����ͼ
                SysCommon.CProgress vProgress = new SysCommon.CProgress("Ŀ¼����");
                vProgress.EnableCancel = false;
                vProgress.ShowDescription = true;
                vProgress.FakeProgress = true;
                vProgress.TopMost = true;
                vProgress.ShowProgress();
                vProgress.SetProgress("ˢ��ͼ���б�");
                SysCommon.ModSysSetting.CopyLayerTreeXmlFromDataBase(Plugin.ModuleCommon.TmpWorkSpace, ModPublicFun._layerTreePath);

                this.layerTree.LoadData();
                vProgress.Close();
                ModPublicFun._SaveLayerTree = true;
        }
        
        //����ͼ��Ŀ¼��Ŀ¼���ý���
        public void RefreshLayerTree()
        {
            if ( ModPublicFun._SaveLayerTree == false)
            {                //��ʼ��ͼ����ͼ
                SysCommon.CProgress vProgress = new SysCommon.CProgress("Ŀ¼����");
                vProgress.EnableCancel = false;
                vProgress.ShowDescription = true;
                vProgress.FakeProgress = true;
                vProgress.TopMost = true;
                vProgress.ShowProgress();
                vProgress.SetProgress("ˢ��ͼ���б�");
                SysCommon.ModSysSetting.CopyLayerTreeXmlFromDataBase (Plugin.ModuleCommon.TmpWorkSpace, ModPublicFun._layerTreePath);

                this.layerTree.LoadData();
                vProgress.Close();
                ModPublicFun._SaveLayerTree = true;
            }
        }
        //�޸�ͼ��Ŀɼ�״̬�������ͼ����˵����������Ϊ�޸ĺ��״̬
        public void ChangeLayerVisible(bool newvisible)
        {
            this.layerTree.LayerVisible = newvisible;
            GeoLayerTreeLib.LayerManager.ModuleMap.SetLayersVisibleAttri(MapControlLayer.Object as IMapControlDefault, newvisible);
            
            MapControlLayer.ActiveView.Refresh();

        }
        #region �Ҽ��˵�
        private void MapControlLayer_OnMouseDown(object sender, ESRI.ArcGIS.Controls.IMapControlEvents2_OnMouseDownEvent e)
        {
            if(e.button == 2)
            {
                System.Drawing.Point ClickPoint = MapControlLayer.PointToScreen(new System.Drawing.Point(e.x, e.y));
                axMapControlcontextMenu.Show(ClickPoint);
            }
        }

        //�Ҽ�����Ŵ�
        private void MapZoomInMenuItem_Click(object sender, EventArgs e)
        {
            ESRI.ArcGIS.SystemUI.ICommand pCommand;
            pCommand = new ESRI.ArcGIS.Controls.ControlsMapZoomInToolClass();
            pCommand.OnCreate(MapControlLayer.Object);
            MapControlLayer.CurrentTool = pCommand as ESRI.ArcGIS.SystemUI.ITool;
        }

        //�Ҽ������С
        private void ZoomOutMenuItem_Click(object sender, EventArgs e)
        {
            ESRI.ArcGIS.SystemUI.ICommand pCommand;
            pCommand = new ESRI.ArcGIS.Controls.ControlsMapZoomOutToolClass();
            pCommand.OnCreate(MapControlLayer.Object);
            MapControlLayer.CurrentTool = pCommand as ESRI.ArcGIS.SystemUI.ITool;
        }

        //�Ҽ��������
        private void MapPanMenuItem_Click(object sender, EventArgs e)
        {

            ESRI.ArcGIS.SystemUI.ICommand pCommand;
            pCommand = new ESRI.ArcGIS.Controls.ControlsMapPanToolClass();
            pCommand.OnCreate(MapControlLayer.Object);
            MapControlLayer.CurrentTool = pCommand as ESRI.ArcGIS.SystemUI.ITool;
        }

        //�Ҽ����������С
        private void MapZoomOutFixedMenuItem_Click(object sender, EventArgs e)
        {
            ESRI.ArcGIS.SystemUI.ICommand pCommand;
            pCommand = new ESRI.ArcGIS.Controls.ControlsMapZoomOutFixedCommandClass();
            pCommand.OnCreate(MapControlLayer.Object);
            pCommand.OnClick();
        }

        //�Ҽ�������ķŴ�
        private void MapZoomInFixedMenuItem_Click(object sender, EventArgs e)
        {
            ESRI.ArcGIS.SystemUI.ICommand pCommand;
            pCommand = new ESRI.ArcGIS.Controls.ControlsMapZoomInFixedCommandClass();
            pCommand.OnCreate(MapControlLayer.Object);
            pCommand.OnClick();
        }

        //�Ҽ����ˢ��
        private void MapRefreshMenuItem_Click(object sender, EventArgs e)
        {
            ESRI.ArcGIS.SystemUI.ICommand pCommand;
            pCommand = new ESRI.ArcGIS.Controls.ControlsMapRefreshViewCommandClass();
            pCommand.OnCreate(MapControlLayer.Object);
            pCommand.OnClick();
        }

        //�Ҽ����ȫ��
        private void MapFullExtentMenuItem_Click(object sender, EventArgs e)
        {
            ESRI.ArcGIS.SystemUI.ICommand pCommand;
            pCommand = new ESRI.ArcGIS.Controls.ControlsMapFullExtentCommandClass();
            pCommand.OnCreate(MapControlLayer.Object);
            pCommand.OnClick();
        }

        //�Ҽ������
        private void BackCommandMenuItem_Click(object sender, EventArgs e)
        {
            ESRI.ArcGIS.SystemUI.ICommand pCommand;
            pCommand = new ESRI.ArcGIS.Controls.ControlsMapZoomToLastExtentBackCommandClass();
            pCommand.OnCreate(MapControlLayer.Object);
            pCommand.OnClick();
        }

        //�Ҽ����ǰ��
        private void ForwardCommandMenuItem_Click(object sender, EventArgs e)
        {
            ESRI.ArcGIS.SystemUI.ICommand pCommand;
            pCommand = new ESRI.ArcGIS.Controls.ControlsMapZoomToLastExtentForwardCommandClass();
            pCommand.OnCreate(MapControlLayer.Object);
            pCommand.OnClick();
        }

        #endregion

        #region �������˵�
        //����������
        private void buttonItemPan_Click(object sender, EventArgs e)
        {
            ESRI.ArcGIS.SystemUI.ICommand pCommand;
            pCommand = new ESRI.ArcGIS.Controls.ControlsMapPanToolClass();
            pCommand.OnCreate(MapControlLayer.Object);
            MapControlLayer.CurrentTool = pCommand as ESRI.ArcGIS.SystemUI.ITool;
        }
        //�������Ŵ�
        private void buttonItemZoomIn_Click(object sender, EventArgs e)
        {
            ESRI.ArcGIS.SystemUI.ICommand pCommand;
            pCommand = new ESRI.ArcGIS.Controls.ControlsMapZoomInToolClass();
            pCommand.OnCreate(MapControlLayer.Object);
            MapControlLayer.CurrentTool = pCommand as ESRI.ArcGIS.SystemUI.ITool;

        }
        //��������С
        private void buttonItemZoonOut_Click(object sender, EventArgs e)
        {
            ESRI.ArcGIS.SystemUI.ICommand pCommand;
            pCommand = new ESRI.ArcGIS.Controls.ControlsMapZoomOutToolClass();
            pCommand.OnCreate(MapControlLayer.Object);
            MapControlLayer.CurrentTool = pCommand as ESRI.ArcGIS.SystemUI.ITool;

        }
        //������ȫ��
        private void buttonItemFullExtent_Click(object sender, EventArgs e)
        {
            ESRI.ArcGIS.SystemUI.ICommand pCommand;
            pCommand = new ESRI.ArcGIS.Controls.ControlsMapFullExtentCommandClass();
            pCommand.OnCreate(MapControlLayer.Object);
            pCommand.OnClick();
        }
        //���������ķŴ�
        private void buttonItemZoonInfixed_Click(object sender, EventArgs e)
        {
            ESRI.ArcGIS.SystemUI.ICommand pCommand;
            pCommand = new ESRI.ArcGIS.Controls.ControlsMapZoomInFixedCommandClass();
            pCommand.OnCreate(MapControlLayer.Object);
            pCommand.OnClick();
        }
        //������������С
        private void buttonItemZoomOutfixed_Click(object sender, EventArgs e)
        {
            ESRI.ArcGIS.SystemUI.ICommand pCommand;
            pCommand = new ESRI.ArcGIS.Controls.ControlsMapZoomOutFixedCommandClass();
            pCommand.OnCreate(MapControlLayer.Object);
            pCommand.OnClick();
        }
        //������ˢ��
        private void buttonItemRefresh_Click(object sender, EventArgs e)
        {
            ESRI.ArcGIS.SystemUI.ICommand pCommand;
            pCommand = new ESRI.ArcGIS.Controls.ControlsMapRefreshViewCommandClass();
            pCommand.OnCreate(MapControlLayer.Object);
            pCommand.OnClick();
        }
        //��������
        private void buttonItemBack_Click(object sender, EventArgs e)
        {
            ESRI.ArcGIS.SystemUI.ICommand pCommand;
            pCommand = new ESRI.ArcGIS.Controls.ControlsMapZoomToLastExtentBackCommandClass();
            pCommand.OnCreate(MapControlLayer.Object);
            pCommand.OnClick();
        }
        //������ǰ��
        private void buttonItemForward_Click(object sender, EventArgs e)
        {
            ESRI.ArcGIS.SystemUI.ICommand pCommand;
            pCommand = new ESRI.ArcGIS.Controls.ControlsMapZoomToLastExtentForwardCommandClass();
            pCommand.OnCreate(MapControlLayer.Object);
            pCommand.OnClick();
        }
        #endregion

        private void UCDataSourceManger_VisibleChanged(object sender, EventArgs e)
        {
            //ModPublicFun.DealChangeSave(); //changed by chulili 20110722����������Դ���� IAppPrivilegesRef->IAppDBIntegraRef
            Plugin.Application.IAppDBIntegraRef pRef = m_AppFrmRef as Plugin.Application.IAppDBIntegraRef;
            if (pRef == null) return; //shduan 20110725

            if (this.Visible == true)
            {
                pRef.CurScaleVisible = true;
            }
            else
            {
                m_AppFrmRef.CoorXY = "";    //ȡ�Բ�ͬ�Ľӿ�
                pRef.CurScaleVisible = false;

            }
            //end changed by chulili
            //����û�δ����Ŀ¼�����ʼ��Ŀ¼�����б�(���˲����ƶ������Ŀ¼����˵�ʱ�ȽϺ���)
            //if (!issave)
            //{
            //    this.RefreshLayerTree();
            //}
        }
        private void GetScaleDecimal()
        {
            int intDecimal;
            Plugin.ModScale.GetScaleConfig(out intDecimal);
            _Decimalstr = "0.";
            for (int i = 0; i < intDecimal; i++)
            {
                _Decimalstr = _Decimalstr + "0";
            }
            if (_Decimalstr.EndsWith("."))
            {
                _Decimalstr = _Decimalstr.Substring(0, _Decimalstr.Length - 1);
            }
        }
        private void MapControlLayer_OnAfterDraw(object sender, IMapControlEvents2_OnAfterDrawEvent e)
        {
            if (_Decimalstr == "")
            {
                _Decimalstr = "0.00";
            }
            double CurScale = double.Parse((sender as AxMapControl).Map.MapScale.ToString(_Decimalstr));
            if (m_AppFrmRef != null)
            {
                m_AppFrmRef.CurScaleCmb.ControlText = CurScale.ToString();
                m_AppFrmRef.CurScaleCmb.Tooltip = CurScale.ToString();

            }
            this.layerTree.UpdateLayerNodeImage();
            //this.comboBoxCurScale.ControlText = (sender as AxMapControl).Map.MapScale.ToString().Trim();
            //this.comboBoxCurScale.Tooltip = MapControlLayer.Map.MapScale.ToString().Trim();

        }

        private void UCDataSourceManger_EnabledChanged(object sender, EventArgs e)
        {
            ModPublicFun.DealChangeSave();
            //changed by chulili 20110722����������Դ���� IAppPrivilegesRef->IAppDBIntegraRef
            Plugin.Application.IAppDBIntegraRef pRef = m_AppFrmRef as Plugin.Application.IAppDBIntegraRef;
            if (pRef == null) return; // shduan 20110725

            if (this.Enabled == true)
            {
                pRef.CurScaleVisible = true;    //״̬����ʾ��ǰ������
            }
            else
            {
                m_AppFrmRef.CoorXY = "";    //ȡ�Բ�ͬ�Ľӿ� ״̬������ʾX\Y����
                pRef.CurScaleVisible = false;//״̬������ʾ��ǰ������
            }
            //end changed by chulili
        }

        private void UCDataSourceManger_Load(object sender, EventArgs e)
        {
            this.splitContainer1.SplitterDistance = this.splitContainer1.Width / 5;
        }

        private void axTOCControl_OnMouseDown(object sender, ITOCControlEvents_OnMouseDownEvent e)
        {
            //if (e.button == 1)
            //{
            //    esriTOCControlItem pItem = new esriTOCControlItem();
            //    IBasicMap pMap = new MapClass();
            //    ILayer pLayer = new FeatureLayerClass();
            //    object other = new object();
            //    object index = new object();
            //    ILegendGroup pLegendGroup;
            //    _SelSymLayer = null;

            //    axTOCControl.HitTest(e.x, e.y, ref pItem, ref pMap, ref pLayer, ref other, ref index);
            //    if (pLayer == null)
            //        return;

            //    if (pItem == esriTOCControlItem.esriTOCControlItemLayer)
            //    {
            //        if (pLayer.GetType() is IAnnotationSublayer) return;
            //        else
            //        {
            //            _SelSymLayer = pLayer;
            //        }
            //    }
            //}
            IBasicMap pMap = null;
            ILayer pLayer = null;
            System.Object other = null;
            System.Object LayerIndex = null;
            System.Drawing.Point pPoint = new System.Drawing.Point(e.x, e.y);

            esriTOCControlItem TOCItem = esriTOCControlItem.esriTOCControlItemNone;
            ITOCControl2 tocControl = (ITOCControl2)axTOCControl.Object;

            tocControl.HitTest(e.x, e.y, ref TOCItem, ref pMap, ref pLayer, ref other, ref LayerIndex);
            if (e.button == 2 && GeoDBIntegration.ModuleData.v_AppDBIntegra.DicContextMenu != null)
            {
                DevComponents.DotNetBar.ButtonItem item = null;
                DevComponents.DotNetBar.ContextMenuBar menuBar = GeoDBIntegration.ModuleData.v_AppDBIntegra.DicContextMenu["TOCContextMenu2"];
                DevComponents.DotNetBar.ContextMenuBar menuBarLayer = GeoDBIntegration.ModuleData.v_AppDBIntegra.DicContextMenu["TOCLayerContextMenu2"];
                this.Controls.Add(menuBar);
                this.Controls.Add(menuBarLayer);
                switch (TOCItem)
                {
                    case esriTOCControlItem.esriTOCControlItemMap:
                        if (GeoDBIntegration.ModuleData.v_AppDBIntegra.DicContextMenu.ContainsKey("TOCContextMenu2"))
                        {
                            if (GeoDBIntegration.ModuleData.v_AppDBIntegra.DicContextMenu["TOCContextMenu2"] != null)
                            {
                                if (GeoDBIntegration.ModuleData.v_AppDBIntegra.DicContextMenu["TOCContextMenu2"].Items.Count > 0)
                                {
                                    item = GeoDBIntegration.ModuleData.v_AppDBIntegra.DicContextMenu["TOCContextMenu2"].Items[0] as DevComponents.DotNetBar.ButtonItem;
                                    if (item != null)
                                    {
                                        item.Popup(axTOCControl.PointToScreen(pPoint));
                                    }
                                }
                            }
                        }
                        break;
                    case esriTOCControlItem.esriTOCControlItemLayer:

                        if (!(pLayer is IGroupLayer || pLayer is IFeatureLayer || pLayer is IDataLayer)) return;
                        if (pLayer is IFeatureLayer || pLayer is IDataLayer)
                        {
                            IFeatureLayer pFeatureLayer = pLayer as IFeatureLayer;
                            if (GeoDBIntegration.ModuleData.v_AppDBIntegra.DicContextMenu.ContainsKey("TOCLayerContextMenu2"))
                            {
                                if (GeoDBIntegration.ModuleData.v_AppDBIntegra.DicContextMenu["TOCLayerContextMenu2"] != null)
                                {
                                    if (GeoDBIntegration.ModuleData.v_AppDBIntegra.DicContextMenu["TOCLayerContextMenu2"].Items.Count > 0)
                                    {
                                        item = GeoDBIntegration.ModuleData.v_AppDBIntegra.DicContextMenu["TOCLayerContextMenu2"].Items[0] as DevComponents.DotNetBar.ButtonItem;
                                        if (item != null)
                                        {
                                            item.Popup(axTOCControl.PointToScreen(pPoint));
                                        }
                                    }
                                }
                            }
                            if (axTOCControl.Buddy is IPageLayoutControl2)
                            {
                                IPageLayoutControl2 pPageLayoutControl = axTOCControl.Buddy as IPageLayoutControl2;
                                pPageLayoutControl.CustomProperty = pLayer;

                            }
                            else if (axTOCControl.Buddy is IMapControl3)
                            {
                                IMapControl3 pMapcontrol = axTOCControl.Buddy as IMapControl3;
                                pMapcontrol.CustomProperty = pLayer;
                            }

                        }
                        break;

                }
            }

        }

        private void axTOCControl_OnMouseMove(object sender, ITOCControlEvents_OnMouseMoveEvent e)
        {
            //IBasicMap pMap = new MapClass();
            //ILayer pLayer = new FeatureLayerClass();
            //object pOther = new object();
            //esriTOCControlItem pItem = new esriTOCControlItem();
            //object pIndex = new object();   
            ////ʵ�ֵ���ͼ��˳����
            //if(e.button == 1)
            //{
            //    axTOCControl.HitTest(e.x, e.y, ref pItem, ref pMap, ref pLayer, ref pOther, ref pIndex);    
            //}
            //if(pItem != esriTOCControlItem.esriTOCControlItemNone)
            //{
            //    Icon icon = new Icon( "icon.ico");
            //    axTOCControl.MouseIcon = icon;
            //    axTOCControl.MousePointer = esriControlsMousePointer.esriPointerCustom;
            //}

        }

        private void axTOCControl_OnMouseUp(object sender, ITOCControlEvents_OnMouseUpEvent e)
        {
            //esriTOCControlItem pItem = new esriTOCControlItem();
            //IBasicMap pMap = new MapClass(); 
            //ILayer pLayer = new FeatureLayerClass(); 
            //object pOther = new object();
            //object pIndex = new object();
            //int i,j;
            //bool bUpdataToc;
            //axTOCControl.MousePointer = esriControlsMousePointer.esriPointerArrow;
            ////ʵ�ֵ���ͼ��˳����
            //if(e.button == 1)
            //{
            //    axTOCControl.HitTest(e.x, e.y, ref pItem, ref pMap, ref pLayer, ref pOther, ref pIndex);
            //}

            //if (pItem == esriTOCControlItem.esriTOCControlItemLayer || pItem == esriTOCControlItem.esriTOCControlItemLegendClass)
            //{
            //    if (pLayer == null || _SelSymLayer == null || _SelSymLayer == pLayer)
            //        return;
            //    if (e.button == 1)
            //    {
            //        int Toindex = -1;
            //        for (i = 0; i < pMap.LayerCount; i++)
            //        {
            //            ILayer pLayTmp;
            //            pLayTmp = pMap.get_Layer(i);
            //            //�õ������ǰ������ֵ
            //            if (pLayer == pLayTmp)
            //            {
            //                Toindex = i;
            //                break ;
            //            }
            //        }
            //        if (Toindex > 0)
            //        {
            //            ((IMap)pMap).MoveLayer(_SelSymLayer, i);
            //            (MapControlLayer.Map as IActiveView).Refresh();
            //        }
            //    }
            //}
            
        }


        #region Ŀ¼�����Ҽ��˵�
        public string  GetTagOfSeletedNode()
        {
            return this.layerTree.GetTagOfSeletedNode();
        }
        //��������Դ
        public void SetDbSource()
        {
            this.layerTree.SetDbSource();
        }
        //�޸Ľڵ�
        public void ModifyNode()
        {
            this.layerTree.ModifyNode();
        }
        //ɾ���ڵ�
        public void DeleteNode()
        {
            this.layerTree.DeleteTreeNode();
            this.layerTree.GetDeleteNodeKeys(ref _DeleteNodeKeys);
        }
        public void SaveDataRight()
        {
            SysGisTable pTable = new SysGisTable(m_TmpWorkSpace);
            Exception err = null;
            for (int i = 0; i < _DeleteNodeKeys.Count; i++)
            {
                string strNodeKey = _DeleteNodeKeys[i];
                pTable.DeleteRows("role_Data", "DATAPRI_ID='" + strNodeKey + "'", out err);
            }
            pTable = null;
                
        }
        //�Զ�ƥ��ͼ������
        public void AutoMathLayerConfig()
        {
            this.layerTree.AutoMathLayerConfig();
        }
        //���ͼ��
        public void AddLayer()
        {
            this.layerTree.AddLayer();
        }
        public void AddWMSserviceLayer()
        {
            this.layerTree.AddWMSserviceLayer();
        }
        //���ͼ����
        public void AddGroup()
        {
            this.layerTree.AddGroup();
        }
        //���ר��
        public void AddFolder()
        {
            this.layerTree.AddFolder();
        }
        //������ݼ�
        public void AddDataSet()
        {
            this.layerTree.AddDataSet();
        }
        //���ŵ�ͼ��
        public void ZoomToLayer()
        {
            this.layerTree.ZoomToLayer();
        }
        //������������
        public void SetLayerAttributes()
        {
            this.layerTree.SetLayerAttributes();
        }
        //�Ƿ����ճ��ͼ��
        public bool CanPasteLayer()
        {
            return this.layerTree.CanPasteLayer();
        }
        //����ͼ��
        public void CopyLayer()
        {
            this.layerTree.CopyLayer();
        }
        //ճ��ͼ��
        public void PasteLayer()
        {
            this.layerTree.PasteLayer();
        }
        #endregion

        private void buttonItemMapScan_Click(object sender, EventArgs e)
        {
            if (buttonItemMapScan.Checked == false)
            {
                buttonItemMapScan.Checked = true;
                ChangeLayerVisible(true);
                //Plugin.LogTable.Writelog("ͼ�����");//xisheng 2011.07.09 ������־   deleted by chulili 2012-09-07 ɽ��ֻ��¼��Ҫ��������־
            }
            else
            {
                buttonItemMapScan.Checked = false;
                ChangeLayerVisible(false);
            }
        }
        //����Ŀ¼֮ǰ������ÿ��ͼ���OrderID
        //���map�к�һ��ͼ���˳���С��ǰһ��ͼ�㣬���һ��˳����޸�Ϊǰһ��˳���+0.01��Ϊ�����ظ���
        //DealLayerOrderID������SetOrderIDofAllLayer�����Ⱥ�ִ�У����ʹ�ã���ɱ���Ŀ¼֮ǰ��ͼ���˳��Ž��д���

        //DealLayerOrderID������ɶ�map�е�ǰ���ص�ͼ�����˳��ŵ��������ã�������ͼ���������˳��
        //SetOrderIDofAllLayer������ɶ�xml�е�ͼ��ڵ��˳��Ž������¸�ֵ����С���󶼸�������ֵ
        //RefreshOrderIDofLayer����ˢ��map��ͼ���˳���
        public void DealLayerOrderID()
        {
            IMap pMap=this.MapControlLayer.Map;
            if (pMap.LayerCount == 0) return;
            //�ȴ����һ��ͼ��
            ILayer pLayer = pMap.get_Layer(0);
            string strOrderid = GetOrderIDofLayer(pLayer);
            int intOrderid = -1;
            if (!strOrderid.Equals(""))
            {
                intOrderid = int.Parse(strOrderid);
            }
            if (intOrderid < 0)
            {
                SetOrderIDofLayer(pLayer,0.01);//��һ�������û����Ч��˳��ţ���ֵ0.01
            }
            for (int i = 0; i < pMap.LayerCount - 1; i++)
            {
                ILayer pTmplayer = pMap.get_Layer(i);
                if (pTmplayer is IGroupLayer)
                {
                    DealLayerOrderIDofGroupLayer(pTmplayer as IGroupLayer);//�����ͼ�������ͣ����ȶ�ͼ�����ڲ����д��� 
                }
            }
            for (int i = 0; i < pMap.LayerCount-1; i++)
            {
                ILayer pLayeri = pMap.get_Layer(i);
                ILayer pLayerj = pMap.get_Layer(i + 1);
                string strOrderID_i = GetOrderIDofLayer(pLayeri );
                string strOrderID_j = GetOrderIDofLayer(pLayerj );
                double dOrderID_i = -1;
                double dOrderID_j = -1;
                if (!strOrderID_i.Equals(""))
                    dOrderID_i = double.Parse(strOrderID_i);
                if (!strOrderID_j.Equals(""))
                    dOrderID_j = double.Parse(strOrderID_j );
                //�����һ��ͼ��˳���С��ǰһ��ͼ�㣬���������ú�һ��˳���
                if (dOrderID_j < dOrderID_i)
                {
                    dOrderID_j = dOrderID_i + 0.01;//+0.01��Ϊ�˷�ֹ˳����ظ���ԭ��˳��Ŷ�������
                    SetOrderIDofLayer(pLayerj,dOrderID_j );
                }

            }
        }
        //����ͼ�����ڲ�ͼ����˳��
        private void SetOrderIDofLayerInGroup(XmlDocument pXmldoc, XmlNode pGroupNode)
        {
            if (pGroupNode == null) return;
            if (pXmldoc == null) return;
            XmlNodeList pXmlnodelist = null;
            pXmlnodelist = pGroupNode.SelectNodes("//Layer");

            SetOrderIDofList(pXmlnodelist, "Layer", pXmldoc);
        }
        //�����õĺ���
        //�Լ����е�ͼ�����˳��ŵ������ͳһ��ֵ�����Ⱥ�˳������ֵ
        private void SetOrderIDofList(XmlNodeList pXmlnodelist, string strLayerType, XmlDocument pXmldoc)
        {
            if (pXmlnodelist == null) return;
            if (pXmldoc == null) return;
            if (pXmlnodelist.Count == 0) return;
            IDictionary<double, string> pDicofLayer = new Dictionary<double, string>();
            List<double> ListOrderID = new List<double>();
            for (int i = 0; i < pXmlnodelist.Count; i++)
            {
                XmlNode pNode = pXmlnodelist[i];
                XmlElement pEle = pNode as XmlElement;
                //��ȡͼ��Ψһ��ʶ
                string strNodeKey = "";
                if (pEle.HasAttribute("NodeKey"))
                {
                    strNodeKey = pEle.GetAttribute("NodeKey");
                }
                //��ȡͼ��˳���
                string strOrderID = "";
                if (pEle.HasAttribute("OrderID"))
                {
                    strOrderID = pEle.GetAttribute("OrderID");
                }
                //��ͼ��Ψһ��ʶ����˳��Ŵ����ֵ�
                if (!strOrderID.Equals(""))
                {
                    double dOrderID = double.Parse(strOrderID);
                    while (pDicofLayer.Keys.Contains(dOrderID))
                    {
                        dOrderID = dOrderID + 0.0001;
                    }
                    pDicofLayer.Add(dOrderID, strNodeKey);
                    ListOrderID.Add(dOrderID);
                    if (strLayerType.ToUpper().Equals("GROUPLAYER"))//�����ͼ���飬����ڲ�ͼ���ٴ�ִ������˳��ź���
                    {
                        SetOrderIDofLayerInGroup(pXmldoc, pNode);
                    }
                }
            }
            int intSetCurOrderID = 1;
            //��ͼ���˳��Ž�������
            for (int i = 0; i < ListOrderID.Count - 1; i++)
            {
                double dCurOrderID = ListOrderID[i];
                int ItemOfMin = i;
                for (int j = i + 1; j < ListOrderID.Count; j++)
                {
                    if (ListOrderID[j] < dCurOrderID)
                    {
                        dCurOrderID = ListOrderID[j];
                        ItemOfMin = j;
                    }
                }
                double tmpOrderID = ListOrderID[i];
                ListOrderID[i] = dCurOrderID;
                ListOrderID[ItemOfMin] = tmpOrderID;
            }
            //����˳�����С����ȡ��ͼ�㣬��ͼ�����¸���˳��ţ�������˳��ţ�
            for (int i = 0; i < ListOrderID.Count; i++)
            {
                double dTmpOrderID = ListOrderID[i];
                string strNodeKey = pDicofLayer[dTmpOrderID];

                SetOrderIDofLayer(strNodeKey, intSetCurOrderID, strLayerType);

                intSetCurOrderID = intSetCurOrderID + 1;
            }
        }
        //Ϊxml������ͼ��ڵ�����OrderID��������Ѿ���OrderID�Ľڵ�
        public void SetOrderIDofAllLayer(string strLayerType)
        {
            XmlDocument pXmldoc = new XmlDocument();
            pXmldoc.Load(ModPublicFun._layerTreePath);
            
            //��ȡ����Layer�ڵ�
            XmlNodeList pXmlnodelist = null;
            if (strLayerType.ToUpper().Equals("LAYER"))
            {
                pXmlnodelist = pXmldoc.SelectNodes("//Layer");
            }
            else
            {
                pXmlnodelist = pXmldoc.SelectNodes("//DataDIR");
            }
            SetOrderIDofList(pXmlnodelist, strLayerType, pXmldoc);

        }
        //ˢ�µ������ͼ��˳��ţ�֧��ͼ���������Լ�ͼ������
        public void RefreshOrderIDofLayer(ILayer pLayer,XmlDocument pXmldoc)
        {
            if (pLayer == null) return;
            ILayerGeneralProperties pLayerGenPro = pLayer as ILayerGeneralProperties;
            //��ȡͼ�������
            string strNodeXml = pLayerGenPro.LayerDescription;
            if (strNodeXml == "")
            {
                return;
            }
            XmlDocument pTmpXmlDoc = new XmlDocument();
            pTmpXmlDoc.LoadXml(strNodeXml);
            //����xml�ڵ㣬����NodeKey�ڽڵ����ѯ
            XmlNode pNode = pTmpXmlDoc.ChildNodes[0];
            string strNodeKey = pNode.Attributes["NodeKey"].Value.ToString();
            string strSearch = "";
            if (pLayer is IGroupLayer)
            {
                strSearch = "//DataDIR [@NodeKey='" + strNodeKey + "']";
            }
            else
            {
                strSearch = "//Layer [@NodeKey='" + strNodeKey + "']";
            }
            pTmpXmlDoc = null;
            XmlNode pNewLayerNode = pXmldoc.SelectSingleNode(strSearch);
            if (pNewLayerNode != null)
            {
                pLayerGenPro.LayerDescription = pNewLayerNode.OuterXml;
                this.layerTree.RefreshOrderIDofLayer(strNodeKey, pNewLayerNode.OuterXml);
            }
            
        }
        //ˢ������map��ͼ���˳��ţ�ˢ�³���xml��ͼ���˳���һ��
        public void RefreshOrderIDofAllLayer()
        {
            XmlDocument pNewXmldoc = new XmlDocument();
            pNewXmldoc.Load(ModPublicFun._layerTreePath);
            for (int i = 0; i < MapControlLayer.Map.LayerCount; i++)
            {
                ILayer pLayer = MapControlLayer.Map.get_Layer(i);
                //ˢ�µ�ǰͼ���˳���
                RefreshOrderIDofLayer(pLayer, pNewXmldoc);
                //���ͼ���������⴦��
                if (pLayer is IGroupLayer)
                {
                    ICompositeLayer pComLayer = pLayer as ICompositeLayer;
                    if (pComLayer != null)
                    {
                        for (int j = 0; j < pComLayer.Count; j++)
                        {
                            ILayer pTmpLayer = pComLayer.get_Layer(j);
                            RefreshOrderIDofLayer(pTmpLayer, pNewXmldoc);
                        }
                    }
                }
            }
        }
        //����ͼ�����ڲ�ͼ����˳���
        //���map�к�һ��ͼ���˳���С��ǰһ��ͼ�㣬���һ��˳����޸�Ϊǰһ��˳���+0.01��Ϊ�����ظ���
        private void DealLayerOrderIDofGroupLayer(IGroupLayer pGroupLayer)
        {
            if (pGroupLayer == null)
                return;
            ICompositeLayer pComLayer = pGroupLayer as ICompositeLayer;
            if (pComLayer != null)
            {
                if (pComLayer.Count == 0) return;
                //�ȴ����һ��ͼ��
                ILayer pLayer = pComLayer.get_Layer(0);
                string strOrderid = GetOrderIDofLayer(pLayer);
                double dOrderid = -1;
                if (!strOrderid.Equals(""))
                {
                    dOrderid = double.Parse(strOrderid);
                }
                if (dOrderid < 0)
                {
                    SetOrderIDofLayer(pLayer, 0.01);
                }
                for (int i = 0; i < pComLayer.Count - 1; i++)
                {
                    ILayer pLayeri = pComLayer.get_Layer(i);
                    ILayer pLayerj = pComLayer.get_Layer(i + 1);
                    string strOrderID_i = GetOrderIDofLayer(pLayeri);
                    string strOrderID_j = GetOrderIDofLayer(pLayerj);
                    double dOrderID_i = -1;
                    double dOrderID_j = -1;
                    if (!strOrderID_i.Equals(""))
                        dOrderID_i = double.Parse(strOrderID_i);
                    if (!strOrderID_j.Equals(""))
                        dOrderID_j = double.Parse(strOrderID_j);
                    //�����һ��ͼ��˳���С��ǰһ�������������ú�һ��ͼ��˳���
                    if (dOrderID_j < dOrderID_i)
                    {
                        dOrderID_j = dOrderID_i + 0.01;//+0.01��Ϊ�˷�ֹ˳����ظ���ԭ��˳��Ŷ�������
                        SetOrderIDofLayer(pLayerj, dOrderID_j);
                    }

                }
            }
        }
        //Ϊͼ�㸳˳��ţ��˴���˫������
        private void SetOrderIDofLayer(ILayer pLayer,double dOrderID)
        {
            try
            {
                ILayerGeneralProperties pLayerGenPro = pLayer as ILayerGeneralProperties;
                //��ȡͼ�������
                string strNodeXml = pLayerGenPro.LayerDescription;
                XmlDocument pXmlDoc = new XmlDocument();
                pXmlDoc.LoadXml(strNodeXml);
                //����xml�ڵ㣬����NodeKey�ڽڵ����ѯ
                XmlNode pNode = pXmlDoc.ChildNodes[0];
                string strNodeKey = pNode.Attributes["NodeKey"].Value.ToString();
                pXmlDoc = null;
                //Ϊͼ������˳��ţ�˫������
                XmlDocument pnewXmldoc = new XmlDocument();
                pnewXmldoc.Load(ModPublicFun._layerTreePath);
                string strSearch = "";
                if (pLayer is IGroupLayer)
                {   //����ͼ���飬��ȡ���ݼ��ڵ�
                    strSearch = "//DataDIR [@NodeKey='" + strNodeKey + "']";
                }
                else
                {   //���ǵ�ͼ�㣬��ȡͼ��ڵ�
                    strSearch = "//Layer [@NodeKey='" + strNodeKey + "']";
                }
                XmlNode pLayerNode = pnewXmldoc.SelectSingleNode(strSearch);
                XmlElement pLayerEle = pLayerNode as XmlElement;
                if (pLayerEle!= null)
                {
                    pLayerEle.SetAttribute("OrderID",dOrderID.ToString());
                }
                pLayerGenPro.LayerDescription = pLayerNode.OuterXml;//��һ�����Ҫ�����޸ĵ�˳��ű����ȥ
                pnewXmldoc.Save(ModPublicFun._layerTreePath);
            }
            catch
            {
            }
        }
        //Ϊͼ�㸳˳��ţ��˴������ͣ����˳��������ձ��浽xml�е�˳���
        public void SetOrderIDofLayer(string strNodeKey, int intOrderID,string strLayerType)
        {
            //Ϊͼ������˳��ţ�˫������
            XmlDocument pnewXmldoc = new XmlDocument();
            pnewXmldoc.Load(ModPublicFun._layerTreePath);
            string strSearch = "";
            //����NodeKey��Ӧ��ͼ��ڵ�
            strSearch = "";
            if (strLayerType.ToUpper().Equals("LAYER"))
            {
                strSearch = "//Layer [@NodeKey='" + strNodeKey + "']";
            }
            else
            {
                strSearch = "//DataDIR [@NodeKey='" + strNodeKey + "']";
            }

            XmlNode pLayerNode = pnewXmldoc.SelectSingleNode(strSearch);
            XmlElement pLayerEle = pLayerNode as XmlElement;
            if (pLayerEle != null)
            {   //Ϊͼ�㸳˳���
                pLayerEle.SetAttribute("OrderID", intOrderID.ToString());
            }
            pnewXmldoc.Save(ModPublicFun._layerTreePath);
        }
        //added by chulili 20110731 ��ȡͼ��˳��ţ������ַ��ͣ�Ϊ��ͬʱ��Ӧ���κ�˫������
        private string GetOrderIDofLayer(ILayer pLayer)
        {
            try
            {
                ILayerGeneralProperties pLayerGenPro = pLayer as ILayerGeneralProperties;
                //��ȡͼ�������
                string strNodeXml = pLayerGenPro.LayerDescription;
                XmlDocument pXmlDoc = new XmlDocument();
                pXmlDoc.LoadXml(strNodeXml);
                //����xml�ڵ㣬����NodeKey�ڽڵ����ѯ
                XmlNode pNode = pXmlDoc.ChildNodes[0];
                string strOrder = "";
                try
                {
                    strOrder = pNode.Attributes["OrderID"].Value.ToString();
                }
                catch
                { }
                return strOrder;
            }
            catch
            {
            }
            return "";

        }

        private void MapControlLayer_OnMouseMove(object sender, IMapControlEvents2_OnMouseMoveEvent e)
        {
            
            if (m_AppFrmRef != null)
            {
                double TempX = double.Parse(e.mapX.ToString("0.00"));
                double TempY = double.Parse(e.mapY.ToString("0.00"));
                m_AppFrmRef.CoorXY = "X:" + TempX + "   Y:" + TempY;
                
            }
        }
    }
}
