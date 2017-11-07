using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;
using System.Xml;
using System.IO;

namespace GeoDataCenterFunLib
{

    public partial class frmQueryForest : DevComponents.DotNetBar.Office2007Form
    {
        
        private DataTable m_dataSourceGrid;        //grid��datasource

        private DataTable m_CategoryGrid;        //grid��datasource
        private IMapControlDefault m_pMapControl;                        //����ѯ�ĵ�ͼ����
        IList<string> listNodes = new List<string>();
        Dictionary<string, IList<string>> dicGroup = new Dictionary<string, IList<string>>();
        Dictionary<string, DataTable> dicDT = new Dictionary<string, DataTable>();
        IList<DataTable> ListDT = new List<DataTable>();
        public DataTable m_Tabel { get; set; }
        public DataRow m_Row { get; set; }
        public IFeature m_pFeature { get; set; }

        private IGeometry m_DrawGeometry = null;
        ///ZQ 201118 modify
        private  IActiveViewEvents_Event m_pActiveViewEvents;

		public IMap m_Map { get; set; }// 20110802 xisheng 
        public IGeometry m_Geometry { get; set; }//20110802 xisheng 
        IActiveView m_ActiveView = null;
        

        /// <summary>
        /// ��ʼ��
        /// </summary>
        /// <param name="pMapControl"></param>
        public frmQueryForest(IMapControlDefault pMapControl)
        {
            InitializeComponent();
            InitializeGrid();
            GetGroup();
            m_pMapControl = pMapControl;
            //added by chulili 20110731 ��ʼ���ֶ���Ӣ�Ķ����ֵ�
            if (SysCommon.ModField._DicFieldName.Keys.Count ==0)
            {
                SysCommon.ModField.InitNameDic(Plugin.ModuleCommon.TmpWorkSpace, SysCommon.ModField._DicFieldName, "���Զ��ձ�");
            }
            if (SysCommon.ModField._DicMatchFieldValue.Keys.Count == 0)
            {
                SysCommon.ModField.InitMatchFieldValueDic(Plugin.ModuleCommon.TmpWorkSpace);
            }
            if (SysCommon.ModField._ListHideFields == null)
            {
                SysCommon.ModField.GetHideFields();
            }
            //end added by chulili
        }

        private void AddColToGrid(DataGridView axiGrid, string HeaderText, string Key, bool Visible, int Width, string CellType)
        {
            DataGridViewColumn vColumn;
            switch (CellType)
            {
                case "TextBox":
                    vColumn = new DataGridViewTextBoxColumn();
                    break;
                case "Button":
                    vColumn = new DataGridViewButtonColumn();
                    break;
                case "CheckBox":
                    vColumn = new DataGridViewCheckBoxColumn();
                    break;
                case "ComboBox":
                    vColumn = new DataGridViewComboBoxColumn();
                    break;
                case "Image":
                    vColumn = new DataGridViewImageColumn();
                    break;
                case "Link":
                    vColumn = new DataGridViewLinkColumn();
                    break;
                default:
                    return;
            }

            vColumn.Visible = Visible;
            vColumn.Width = Width;
            vColumn.HeaderText = HeaderText;
            vColumn.Name = Key;
            vColumn.DataPropertyName = Key; //"C" + (axiGrid.Columns.Count + 1).ToString();
            axiGrid.Columns.Add(vColumn);
        }
        //�õ������������
        private bool GetGroup()
        {
            string m_XmlPath = Application.StartupPath + "\\SecondQueryConfig.xml";
            if (!System.IO.File.Exists(m_XmlPath)) 
            { 
                return false; 
            }
            XmlDocument pXmldoc = new XmlDocument();
            pXmldoc.Load(m_XmlPath);

            XmlNodeList xNodeList = pXmldoc.SelectNodes("TypeNameConfig/TypeName");

            if (xNodeList==null) 
            {
                return false; 
            }
            if (xNodeList.Count == 0)
            { 
                return false;
            }
            //advTree.Nodes.Clear();
            m_CategoryGrid.Clear();
            for (int i = 0; i < xNodeList.Count; i++)
            {
                string NodeName = xNodeList[i].Attributes["name"].Value.ToString();

                //DevComponents.AdvTree.Node featnode = new DevComponents.AdvTree.Node();
                //featnode.Text = NodeName;
                //advTree.Nodes.Add(featnode);
                m_CategoryGrid.Rows.Add(new object[] { NodeName });

                IList<string> listType = XNodeToList(xNodeList[i].ChildNodes);
                dicGroup.Add(NodeName, listType);
                listNodes.Add(NodeName);
            }
            return true;

        }

        //��ȡ���������
        private IList<string> XNodeToList(XmlNodeList xNodeList)
        {
            IList<string> newList = new List<string>();
            for (int i = 0; i < xNodeList.Count; i++)
            {
                string Value = xNodeList[i].Attributes["name"].Value.ToString();
                newList.Add(Value);
            }
            return newList;
        }
        /// <summary>
        /// ���ݷ�Χ����������䴰��
        /// </summary>
        /// <param name="pMap">����ѯ�ĵ�ͼ����</param>
        /// <param name="pGeometry">��ѯ��Χ</param>
        public void FillData(IMap pMap, IGeometry pGeometry, esriSpatialRelEnum pesriSpatialRelEnum)
        {
            string strNodeKey = SysCommon.ModSysSetting.GetLinBanLayerNodeKey(Plugin.ModuleCommon.TmpWorkSpace);
            ILayer pLayer = SysCommon.ModuleMap.GetLayerByNodeKey(null, pMap, strNodeKey, null, true);
            IFeatureLayer pFeatureLayer = pLayer as IFeatureLayer;
            if (pFeatureLayer != null)
            {
                ISpatialFilter pSpatialFilter = new SpatialFilterClass();
                pSpatialFilter.Geometry = pGeometry;
                pSpatialFilter.SpatialRel = pesriSpatialRelEnum;
                IFeatureCursor pFeatureCursor = pFeatureLayer.Search(pSpatialFilter, false);
                Application.DoEvents();

                IFeature pFeat = pFeatureCursor.NextFeature();
                if (pFeat != null)
                {
                    m_pFeature = pFeat;
                }
                System.Runtime.InteropServices.Marshal.ReleaseComObject(pFeatureCursor);
                pFeatureCursor = null;

                string strName = "";
                try
                {
                    strName = dataGridCategory.SelectedCells[0].Value.ToString();
                }
                catch
                { }
                InitValueGrid(strName);
            }
        }
        private void InitValueGrid(string strCategoryName)
        {
            if (strCategoryName != "")
            {
                IList<string> listType = dicGroup[strCategoryName];
                IFeature pfeature = m_pFeature;
                if (pfeature != null)
                {
                    m_dataSourceGrid.Clear();

                    for (int i = 0; i < pfeature.Fields.FieldCount; i++)
                    {
                        string FieldName = m_pFeature.Fields.get_Field(i).Name;
                        string values = m_pFeature.get_Value(i).ToString();
                        if (pfeature.Fields.get_Field(i).Type != esriFieldType.esriFieldTypeGeometry)
                        {

                            if (listType.Contains(FieldName))
                            {
                                m_dataSourceGrid.Rows.Add(new object[] { SysCommon.ModField.GetChineseNameOfField(FieldName), SysCommon.ModXZQ.GetChineseName (Plugin .ModuleCommon .TmpWorkSpace ,FieldName, values) });
                            }
                            else if (strCategoryName == "������Ϣ")
                            {
                                m_dataSourceGrid.Rows.Add(new object[] { SysCommon.ModField.GetChineseNameOfField(FieldName), SysCommon.ModXZQ.GetChineseName(Plugin.ModuleCommon.TmpWorkSpace, FieldName, values) });
                            }
                        }

                    }
                }
            }
        }
        /// <summary>
        /// ����RGB����  
        /// </summary>
        /// <param name="r">Red</param>
        /// <param name="g">Green</param>
        /// <param name="b">Blue</param>
        /// <returns></returns>
        public static IRgbColor getRGB(int r, int g, int b)
        {
            IRgbColor pRgbColor = new RgbColorClass();
            pRgbColor.Red = r;
            pRgbColor.Green = g;
            pRgbColor.Blue = b;
            return pRgbColor;
        }
        private void drawgeometryXOR(IGeometry pGeometry)
        {
            if (pGeometry == null)//�������رջ���ȡ�� �Ͳ����� xisheng 2011.06.28
            {
                return;
            }
            IScreenDisplay pScreenDisplay = m_pMapControl.ActiveView.ScreenDisplay;
            ISymbol pSymbol = null;
            //��ɫ����
            IRgbColor pRGBColor = new RgbColorClass();
            pRGBColor.UseWindowsDithering = false;
            pRGBColor = getRGB(255, 0, 0);
            pRGBColor.Transparency = 255;
            try
            {
                switch (pGeometry.GeometryType.ToString())
                {
                    case "esriGeometryPoint"://��Ҫ��
                        ISimpleMarkerSymbol pMarkerSymbol = new SimpleMarkerSymbolClass();
                        pMarkerSymbol.Size = 7.0;
                        pMarkerSymbol.Style = esriSimpleMarkerStyle.esriSMSCircle;
                        pMarkerSymbol.Color = pRGBColor;
                        pSymbol = (ISymbol)pMarkerSymbol;
                        pSymbol.ROP2 = esriRasterOpCode.esriROPCopyPen;
                        break;
                    case "esriGeometryPolyline"://��Ҫ��
                        ISimpleLineSymbol pPolyLineSymbol = new SimpleLineSymbolClass();
                        pPolyLineSymbol.Color = pRGBColor;
                        pPolyLineSymbol.Width = 2.5;
                        pPolyLineSymbol.Style = esriSimpleLineStyle.esriSLSSolid;
                        pSymbol = (ISymbol)pPolyLineSymbol;
                        ///ZQ  20111117 modify
                        pSymbol.ROP2 = esriRasterOpCode.esriROPCopyPen;
                        break;
                    case "esriGeometryPolygon"://��Ҫ��
                        ISimpleFillSymbol pFillSymbol = new SimpleFillSymbolClass();
                        ISimpleLineSymbol pLineSymbol = new SimpleLineSymbolClass();

                        pSymbol = (ISymbol)pFillSymbol;
                        pSymbol.ROP2 = esriRasterOpCode.esriROPCopyPen;
                        /// end
                        pLineSymbol.Color = pRGBColor;
                        pLineSymbol.Width = 1.5;
                        pLineSymbol.Style = esriSimpleLineStyle.esriSLSSolid;
                        pFillSymbol.Outline = pLineSymbol;

                        pFillSymbol.Color = pRGBColor;
                        pFillSymbol.Style = esriSimpleFillStyle.esriSFSDiagonalCross;
                        break;
                }


                pScreenDisplay.StartDrawing(pScreenDisplay.hDC, (System.Int16)ESRI.ArcGIS.Display.esriScreenCache.esriNoScreenCache);  //esriScreenCache.esriNoScreenCache -1
                pScreenDisplay.SetSymbol(pSymbol);
                switch (pGeometry.GeometryType.ToString())
                {
                    case "esriGeometryPoint"://��Ҫ��
                        pScreenDisplay.DrawPoint(pGeometry);
                        break;
                    case "esriGeometryPolyline"://��Ҫ��
                        pScreenDisplay.DrawPolyline(pGeometry);
                        break;
                    case "esriGeometryPolygon"://��Ҫ��
                        pScreenDisplay.DrawPolygon(pGeometry);
                        break;
                }
                pScreenDisplay.FinishDrawing();

            }
            catch
            { }
            finally
            {
                pSymbol = null;
                pRGBColor = null;
            }
        }
        ///ZQ 201118 modify
        //����ȥʱ����
        internal void AfterDraw(IDisplay Display, esriViewDrawPhase phase)
        {
            if (this.IsDisposed == true) return;
            if (phase == esriViewDrawPhase.esriViewForeground) drawgeometryXOR(m_DrawGeometry);
        }
        private void frmQuery_Load(object sender, EventArgs e)
        {
            m_ActiveView = m_pMapControl.Map as IActiveView;
            m_pActiveViewEvents = m_ActiveView as IActiveViewEvents_Event;
            SysCommon.ScreenDraw.list.Add(AfterDraw);
        }


        //��ѯ����ر�
        private void frmQuery_FormClosed(object sender, FormClosedEventArgs e)
        {

            m_DrawGeometry = null;
            if (m_ActiveView != null)
            {
                m_ActiveView.PartialRefresh(esriViewDrawPhase.esriViewForeground, null, null); //changed by xisheng 20110803
            }
            m_dataSourceGrid.Clear();
            m_dataSourceGrid = null;
            this.Dispose(true);
            SysCommon.ScreenDraw.list.Remove(AfterDraw);
            this.Dispose(true);
            
        }
        //��ʼ��grid ��ʾ������

        private void InitializeGrid()
        {
            dataGridViewX.Columns.Clear();
            dataGridViewX.ReadOnly = true;
            dataGridViewX.AutoGenerateColumns = false;
            m_dataSourceGrid = new DataTable();
            m_dataSourceGrid.Columns.Add("Name", typeof(System.String));
            m_dataSourceGrid.Columns["Name"].ReadOnly = true;
            m_dataSourceGrid.Columns.Add("Value", typeof(System.String));
            m_dataSourceGrid.Columns["Value"].ReadOnly = true;
            AddColToGrid(dataGridViewX, "�ֶ�����", "Name", true, 132, "TextBox");
            AddColToGrid(dataGridViewX, "�ֶ�ֵ", "Value", true, 132, "TextBox");
            dataGridViewX.Columns[dataGridViewX.ColumnCount - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewX.DataSource = m_dataSourceGrid;

            dataGridCategory.Columns.Clear();
            dataGridCategory.ReadOnly = true;
            dataGridCategory.AutoGenerateColumns = false;
            m_CategoryGrid = new DataTable();
            m_CategoryGrid.Columns.Add("Name1", typeof(System.String));
            m_CategoryGrid.Columns["Name1"].ReadOnly = true;
            AddColToGrid(dataGridCategory, "��������", "Name1", true, 132, "TextBox");
            dataGridCategory.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridCategory.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            
            dataGridCategory.DataSource = m_CategoryGrid;

        }
        //private void advTree_NodeClick(object sender, DevComponents.AdvTree.TreeNodeMouseEventArgs e)
        //{
        //    if (advTree.SelectedNode == null)
        //    {
        //        return;
        //    }
        //    string strName = advTree.SelectedNode.Text;
        //    IList<string> listType = dicGroup[strName];
        //    IFeature pfeature = m_pFeature;
        //    if (pfeature != null)
        //    {
        //        m_dataSourceGrid.Clear();

        //        for (int i = 0; i < pfeature.Fields.FieldCount; i++)
        //        {
        //            string FieldName = m_pFeature.Fields.get_Field(i).Name;
        //            string values = m_pFeature.get_Value(i).ToString();
        //            if (pfeature.Fields.get_Field(i).Type != esriFieldType.esriFieldTypeGeometry)
        //            {

        //                if (listType.Contains(FieldName))
        //                {
        //                    m_dataSourceGrid.Rows.Add(new object[] { SysCommon.ModField.GetChineseNameOfField(FieldName), SysCommon.ModField.GetChineseOfFieldValue(FieldName, values) });
        //                }
        //                else if (strName == "������Ϣ")
        //                {
        //                    m_dataSourceGrid.Rows.Add(new object[] { SysCommon.ModField.GetChineseNameOfField(FieldName), SysCommon.ModField.GetChineseOfFieldValue(FieldName, values) });
        //                }
        //            }

        //        }
        //    }

        //}
        
        private void dataGridCategory_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            string strName = "";
            try
            {
                strName=dataGridCategory.SelectedCells[0].Value.ToString();
            }
            catch
            { }
            InitValueGrid(strName);

            
        }    

    }
}