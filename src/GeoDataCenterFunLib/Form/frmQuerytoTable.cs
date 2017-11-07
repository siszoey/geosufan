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

namespace GeoDataCenterFunLib
{

    public partial class frmQuerytoTable: DevComponents.DotNetBar.Office2007Form
    {
        private DataTable m_dataSourceGrid;        //grid��datasource
        private IMapControlDefault m_pMapControl;                        //����ѯ�ĵ�ͼ����
        private enumQueryMode m_enumQueryMode;     //��ѯ��ʽ
        private bool _IsHightLight=false;
        private IFeatureCursor _FeaCursor = null;
        private int _FeaCnt = 0;
        private DataTable _DataTableAll = null;
        private DataTable _DataTablePart = null;
        private int _CurMaxID = 0;//��ǰҳ����ʾ�����id,��0��ʼ
        private int _PageID = 0;//��ǰҳ(��1��ʼ)
        private Dictionary<int, int> _DicFeature=new Dictionary<int,int> ();
        private string _FlashTag = "frmQueryToTable";
        private IFeatureClass _QueryFeatureClass = null;
        
        private int _OIDfieldIndex=0;

        private IQueryFilter _QueryFilterAll = null;
        private IQueryFilter _QueryFilterPart1 = null;
        private IQueryFilter _QueryFilterPart2 = null;

        private Dictionary<string, DataTable> _DicDatatable = null;
        private Dictionary<string, IFeatureClass> _DicFeatureClass = null;

        public enumQueryMode QueryMode
        {
            get
            {
                return m_enumQueryMode;
            }
        }
		public IMap m_Map { get; set; }// 20110802 xisheng 
        public IGeometry m_Geometry { get; set; }//20110802 xisheng 

        /// <summary>
        /// ��ʼ��������SQL��ѯ
        /// </summary>
        /// <param name="pMapControl"></param>
        public frmQuerytoTable(IMapControlDefault pMapControl)
        {
            InitializeComponent();
            labelItem.Visible = false;
            comboBoxFeatureLayer.Visible = false;

            //InitializeGrid();      //��ʼ��Grid�ı���  
            m_pMapControl = pMapControl;
            //added by chulili 20110731 ��ʼ���ֶ���Ӣ�Ķ����ֵ�
            if (SysCommon.ModField._DicFieldName.Keys.Count ==0)
            {
                SysCommon.ModField.InitNameDic(Plugin.ModuleCommon.TmpWorkSpace, SysCommon.ModField._DicFieldName,"���Զ��ձ�");
            }
            //changed by chulili 20110818 ͳһ��������ֵӳ��
            if (SysCommon.ModField._DicMatchFieldValue.Keys.Count == 0)
            {
                SysCommon.ModField.InitMatchFieldValueDic(Plugin.ModuleCommon.TmpWorkSpace);
            }
            //������Щ�ֶ�
            if (SysCommon.ModField._ListHideFields == null)
            {
                SysCommon.ModField.GetHideFields();
            }
            //if (ModQuery._DicGBname.Keys.Count == 0)
            //{
            //    ModQuery.InitNameDic(Plugin.ModuleCommon.TmpWorkSpace, ModQuery._DicGBname, "������ձ�");
            //}
            //if (ModQuery._DicClassname.Keys.Count == 0)
            //{
            //    ModQuery.InitNameDic(Plugin.ModuleCommon.TmpWorkSpace, ModQuery._DicClassname,"����������ձ�");
            //}
            //if (ModQuery._DicXZQname.Keys.Count == 0)
            //{
            //    ModQuery.InitNameDic(Plugin.ModuleCommon.TmpWorkSpace, ModQuery._DicXZQname, "�����������");
            //}
            //end added by chulili
        }
        /// <summary>
        /// �����ڰ�����������ཻ��ѯ
        /// </summary>
        /// <param name="pMapControl">����ѯ�ĵ�ͼ����</param>
        /// <param name="penumQueryMode">��ѯ��ʽ</param>
        public frmQuerytoTable(IMapControlDefault pMapControl, enumQueryMode penumQueryMode,bool isDiff)
        {
            InitializeComponent();
            labelItem.Visible = false;
            comboBoxFeatureLayer.Visible = false;

            //InitializeGrid();      //��ʼ��Grid�ı���  
            m_pMapControl = pMapControl;
            m_enumQueryMode = penumQueryMode;
            InitiaComboBox();
            switch(penumQueryMode)
            {
                case enumQueryMode.Top:

                    comboBoxFeatureLayer.SelectedIndex = 0;
                    break;
                case enumQueryMode.Visiable:
				case enumQueryMode.Layer:
                    comboBoxFeatureLayer.SelectedIndex = 1;
                    break;
                case enumQueryMode.Selectable:
                    comboBoxFeatureLayer.SelectedIndex = 2;
                    break;
                //case enumQueryMode.CurEdit://changed by chulili 20110707�ֲ����е�ǰ�༭ͼ��ѡ��
                //    comboBoxItem.SelectedIndex = 3;
                    //break;
                case enumQueryMode.All:
                    comboBoxFeatureLayer.SelectedIndex = 3;
                    break;
            }
            //added by chulili 20110731 ��ʼ���ֶ���Ӣ�Ķ����ֵ�
            if (SysCommon.ModField._DicFieldName.Keys.Count == 0)
            {
                SysCommon.ModField.InitNameDic(Plugin.ModuleCommon.TmpWorkSpace, SysCommon.ModField._DicFieldName,"���Զ��ձ�");
            }
            //changed by chulili 20110818 ͳһ��������ֵӳ��
            if (SysCommon.ModField._DicMatchFieldValue.Keys.Count == 0)
            {
                SysCommon.ModField.InitMatchFieldValueDic(Plugin.ModuleCommon.TmpWorkSpace);
            }
            //������Щ�ֶ�
            if (SysCommon.ModField._ListHideFields == null)
            {
                SysCommon.ModField.GetHideFields();
            }
            //if (ModQuery._DicGBname.Keys.Count == 0)
            //{
            //    ModQuery.InitNameDic(Plugin.ModuleCommon.TmpWorkSpace, ModQuery._DicGBname,"������ձ�");
            //}
            //if (ModQuery._DicClassname.Keys.Count == 0)
            //{
            //    ModQuery.InitNameDic(Plugin.ModuleCommon.TmpWorkSpace, ModQuery._DicClassname,"����������ձ�");
            //}
            //if (ModQuery._DicXZQname.Keys.Count == 0)
            //{
            //    ModQuery.InitNameDic(Plugin.ModuleCommon.TmpWorkSpace, ModQuery._DicXZQname, "�����������");
            //}
            //end added by chulili
        }
        /// <summary>
        /// ��ѯ��ͼ���Ƿ�ɲ�ѯ added by xisheng 20110802
        /// </summary>
        /// <param name="layer">ͼ��</param>
        /// <returns></returns>
        public bool GetIsQuery(IFeatureLayer layer)
        {
            ILayerGeneralProperties pLayerGenPro = layer as ILayerGeneralProperties;
            //��ȡ��ͼ���������Ϣ��ת��xml�ڵ�
            string strNodeXml = pLayerGenPro.LayerDescription;

            if (strNodeXml.Equals(""))
            {
                return true;
            }
            XmlDocument pXmldoc = new XmlDocument();
            pXmldoc.LoadXml(strNodeXml);
            //��ȡ�ڵ��NodeKey��Ϣ
            XmlNode pxmlnode = pXmldoc.SelectSingleNode("//AboutShow");
            if (pxmlnode == null)
            {
                pXmldoc = null;
                return true;
            }
            string strNodeKey = pxmlnode.Attributes["IsQuery"].Value.ToString();
            if (strNodeKey.Trim().ToUpper() == "FALSE")
            {
                pXmldoc = null;
                return false;
            }
            else
            {
                pXmldoc = null;
                return true;
            }

        }
        /// ������һ���ѯ,�����ѯ,ѡ���ѯ��һ���ѯ
        /// </summary>
        /// <param name="pMapControl">����ѯ�ĵ�ͼ����</param>
        /// <param name="penumQueryMode">��ѯ��ʽ</param>
        public frmQuerytoTable(IMapControlDefault pMapControl, enumQueryMode penumQueryMode)
        {
            InitializeComponent();
            labelItem.Visible = true;
            comboBoxFeatureLayer.Visible = true;

            //InitializeGrid();      //��ʼ��Grid�ı���  
            m_pMapControl = pMapControl;
            m_enumQueryMode = penumQueryMode;
            InitiaComboBox();
            switch (penumQueryMode)
            {
                case enumQueryMode.Top:

                    comboBoxFeatureLayer.SelectedIndex = 0;
                    break;
                case enumQueryMode.Visiable:
				case enumQueryMode.Layer:
                    comboBoxFeatureLayer.SelectedIndex = 1;
                    break;
                case enumQueryMode.Selectable:
                    comboBoxFeatureLayer.SelectedIndex = 2;
                    break;
                //case enumQueryMode.CurEdit://changed by chulili 20110707�ֲ����е�ǰ�༭ͼ��ѡ��
                //    comboBoxItem.SelectedIndex = 3;
                //    break;
                case enumQueryMode.All:
                    comboBoxFeatureLayer.SelectedIndex = 3;
                    break;
            }
            //added by chulili 20110731 ��ʼ���ֶ���Ӣ�Ķ����ֵ�
            if (SysCommon.ModField._DicFieldName.Keys.Count == 0)
            {
                SysCommon.ModField.InitNameDic(Plugin.ModuleCommon.TmpWorkSpace, SysCommon.ModField._DicFieldName,"���Զ��ձ�");
            }
            //changed by chulili 20110818 ͳһ��������ֵӳ��
            if (SysCommon.ModField._DicMatchFieldValue.Keys.Count == 0)
            {
                SysCommon.ModField.InitMatchFieldValueDic(Plugin.ModuleCommon.TmpWorkSpace);
            }
            //������Щ�ֶ�
            if (SysCommon.ModField._ListHideFields == null)
            {
                SysCommon.ModField.GetHideFields();
            }
            //if (ModQuery._DicGBname.Keys.Count == 0)
            //{
            //    ModQuery.InitNameDic(Plugin.ModuleCommon.TmpWorkSpace, ModQuery._DicGBname,"������ձ�");
            //}
            //if (ModQuery._DicClassname.Keys.Count == 0)
            //{
            //    ModQuery.InitNameDic(Plugin.ModuleCommon.TmpWorkSpace, ModQuery._DicClassname, "����������ձ�");
            //}
            //if (ModQuery._DicXZQname.Keys.Count == 0)
            //{
            //    ModQuery.InitNameDic(Plugin.ModuleCommon.TmpWorkSpace, ModQuery._DicXZQname, "�����������");
            //}
            //end added by chulili
        }
        /// <summary>
        /// ����ͼ�е�ͼ����ص��б��� added by xisheng 20110731
        /// </summary>
        private void InitiaComboBox()
        {
            for (int i = 0; i < m_pMapControl.Map.LayerCount; i++)
            {
                if (m_pMapControl.Map.get_Layer(i) is IGroupLayer)
                {
                    ICompositeLayer pLayer = m_pMapControl.Map.get_Layer(i) as ICompositeLayer;
                    for (int j = 0; j < pLayer.Count; j++)
                    {
                        ILayer pFLayer = pLayer.get_Layer(j) as ILayer;
                        if (pFLayer != null)
                        {
                            if (pFLayer is IFeatureLayer)
                            {
                                IFeatureLayer pFeatureLayer = pFLayer as IFeatureLayer;
								if (GetIsQuery(pFeatureLayer))//�ж��Ƿ�ɲ�ѯ 0802
                                {
                                    comboBoxFeatureLayer.Items.Add(pFeatureLayer.Name);
                                }
                            }
                        }
                    }
                }
                else
                {
                    ILayer pFLayer = m_pMapControl.Map.get_Layer(i) as ILayer;
                    if (pFLayer != null)
                    {
                        if (pFLayer is IFeatureLayer)
                        {
                            IFeatureLayer pFeatureLayer = pFLayer as IFeatureLayer;
                            if (GetIsQuery(pFeatureLayer))//�ж��Ƿ�ɲ�ѯ 0802
                            {
                                comboBoxFeatureLayer.Items.Add(pFeatureLayer.Name);
                            }
                        }
                    }
                }
            }
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

        
        /// <summary>��������Ӻ��� 20110802
        /// ������������������䴰�壬��������SQL��ѯ,ר�����ڵ�����ѯ,����,��·��ѯ
        /// </summary>
        /// <param name="pMap">����ѯ�ĵ�ͼ����</param>
        public void FillData(IFeatureClass pFeatureClass, IQueryFilter pQueryFilter,bool isHighLight)
        {
            if (pFeatureClass == null)
            {
                return;
            }
            _QueryFeatureClass = pFeatureClass;

            if (_FeaCursor != null)
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(_FeaCursor);
                _FeaCursor = null;
            }
            //if (_DicFeature == null)
            //{
            //    _DicFeature = new Dictionary<int, int>();
            //}
            labelItem.Visible = true;
            comboBoxFeatureLayer.Visible = true;
            if (!comboBoxFeatureLayer.Items.Contains(pFeatureClass.AliasName))
            {
                comboBoxFeatureLayer.Items.Add(pFeatureClass.AliasName);
            }
            for (int i = 0; i < comboBoxFeatureLayer.Items.Count; i++)
            {
                if (comboBoxFeatureLayer.Items[i].ToString() == pFeatureClass.AliasName)
                {
                    comboBoxFeatureLayer.SelectedIndex = i;
                    break;
                }
            }
            this.comboBoxFeatureLayer.Enabled = false;

            //m_dataSourceGrid.Clear();

            //��ȡ��ѯ��ͼ��

            progressBarItem.Visible = false;
            this.Refresh();
            bool bFirst = true;
            //added by chulili 20110801 ��Ӵ��󱣻�
            if (pFeatureClass == null)
                return;

            //ѭ��ȡ��ѡ��Feature��ID���õ�Feature
            //��ʼ����
            _DataTablePart = InitDataTable(pFeatureClass);
            _DataTableAll = InitDataTable(pFeatureClass);
            //�� �Ա㶯̬��ʾ
            this.gridRes.DataSource = _DataTablePart;

            //����������
            progressBarItem.Visible = true;
            this.Refresh();
            int SearchCnt = pFeatureClass.FeatureCount(pQueryFilter);
            progressBarItem.Maximum = SearchCnt + 100;
            progressBarItem.Minimum = 0;
            progressBarItem.Value = 0;

            ICursor pCursor1 = pFeatureClass.Search(pQueryFilter, false) as ICursor ;
            
            IRow pRow = pCursor1.NextRow();
            int indexFeature = 0;
            IFeature pFeature = null;
            while (pRow != null)
            {
                //������
                progressBarItem.Value = progressBarItem.Value + 1;

                _FeaCnt = _FeaCnt + 1;
                //_DicFeature.Add(indexFeature, pRow.OID);
                InsterFeaToTable(pRow, _DataTableAll);
                indexFeature = indexFeature + 1;
                pRow = pCursor1.NextRow();
            }
            int iIndex = 0;
            for (iIndex = 0; iIndex < 100; iIndex++)
            {
                //������
                progressBarItem.Value = progressBarItem.Value + 1;
                if (iIndex >= _DataTableAll.Rows.Count)
                {
                    break;
                }
                DataRow pDataRow = _DataTableAll.Rows[iIndex];
                DataRow pDataRow2 = _DataTablePart.NewRow();
                pDataRow2.ItemArray = pDataRow.ItemArray;
                _DataTablePart.Rows.Add(pDataRow2);
            }
            progressBarItem.Value = progressBarItem.Maximum;
            _CurMaxID = iIndex - 1;
            _PageID = 1;
            this.textBoxPage.ControlText = _PageID.ToString();
            //��һҳ״̬
            if (_FeaCnt > _CurMaxID + 1)
            {
                this.buttonNextPage.Enabled = true;
            }
            else
            {
                this.buttonNextPage.Enabled = false;
            }
            //��һҳ״̬
            if (_CurMaxID > 100)
            {
                this.buttonLastPage.Enabled = true;
            }
            else
            {
                this.buttonLastPage.Enabled = false;
            }

            labelItemMemo.Text = "���ҵ�" + _FeaCnt.ToString() + "��Ҫ��";
            progressBarItem.Value = 0;
            progressBarItem.Enabled = false;
            //���������ֶ�
            for (int i = 0; i < gridRes.Columns.Count; i++)
            {
                DataGridViewColumn pColumn = gridRes.Columns[i];
                pColumn.HeaderText = _DataTablePart.Columns[i].ColumnName;
                pColumn.Name = _DataTablePart.Columns[i].Caption;
                string strFieldname = pColumn.Name;
                if (SysCommon.ModField._ListHideFields.Contains(pColumn.Name))
                {
                    pColumn.Visible = false;
                }
            }
            this.Refresh();
            IGraphicsContainer psGra = m_pMapControl.Map as IGraphicsContainer;
            if (isHighLight)
            {
                //drawPolyLineElement(pListGeometry, psGra);
                (m_pMapControl.Map as IActiveView).PartialRefresh(esriViewDrawPhase.esriViewBackground, null, null);
            }
        }
        /// <summary>��������Ӻ��� 20110807
        /// ������featureclass
        /// ������������������䴰�壬��������SQL��ѯ,ר�����ڵ�����ѯ,����,��·��ѯ
        /// </summary>
        /// <param name="pMap">����ѯ�ĵ�ͼ����</param>
        public void FillData(List <string> ListLayername,List<IFeatureClass> ListFeatureClass, IQueryFilter pQueryFilter, IQueryFilter pQueryFilterOrder1, IQueryFilter pQueryFilterOrder2, bool isHighLight)
        {
            if (ListFeatureClass == null)
            {
                return;
            }
            if (ListFeatureClass.Count == 0)
                return;

            _QueryFeatureClass = ListFeatureClass[0];

            if (_FeaCursor != null)
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(_FeaCursor);
                _FeaCursor = null;
            }
            //if (_DicFeature == null)
            //{
            //    _DicFeature = new Dictionary<int, int>();
            //}
            labelItem.Visible = true;
            comboBoxFeatureLayer.Visible = true;

            if (_DicFeatureClass == null)
            {
                _DicFeatureClass = new Dictionary<string, IFeatureClass>();            
            }

            for (int i = 0; i < ListLayername.Count; i++)
            {
                comboBoxFeatureLayer.Items.Add(ListLayername[i]);
                if (!_DicFeatureClass.ContainsKey(ListLayername[i]))
                {
                    _DicFeatureClass.Add(ListLayername[i], ListFeatureClass[i]);
                }
            }
            string LayerName = ListLayername[0];
            for (int i = 0; i < comboBoxFeatureLayer.Items.Count; i++)
            {
                if (comboBoxFeatureLayer.Items[i].ToString() == ListLayername[0])
                {
                    comboBoxFeatureLayer.SelectedIndex = i;
                    break;
                }
            }
            this.comboBoxFeatureLayer.Enabled = true;

            //m_dataSourceGrid.Clear();

            //��ȡ��ѯ��ͼ��

            progressBarItem.Visible = false;
            this.Refresh();
            bool bFirst = true;
            //added by chulili 20110801 ��Ӵ��󱣻�
            if (_QueryFeatureClass  == null)
                return;
            _QueryFilterAll = pQueryFilter;
            _QueryFilterPart1 = pQueryFilterOrder1;
            _QueryFilterPart2 = pQueryFilterOrder2;
            //ѭ��ȡ��ѡ��Feature��ID���õ�Feature
            //��ʼ����
            _DataTablePart = InitDataTable(_QueryFeatureClass);
            _DataTableAll = InitDataTable(_QueryFeatureClass);
            if (_DicDatatable == null)
                _DicDatatable = new Dictionary<string, DataTable>();
            //�� �Ա㶯̬��ʾ
            this.gridRes.DataSource = _DataTablePart;

            ISelectionSet pSelectSet = _QueryFeatureClass.Select(pQueryFilter, esriSelectionType.esriSelectionTypeIDSet, esriSelectionOption.esriSelectionOptionNormal, (_QueryFeatureClass as IDataset).Workspace);
            if (pSelectSet == null)
            {
                labelItemMemo.Text = "���ҵ�0��Ҫ��";
                progressBarItem.Visible = false;
                this.Refresh();
                return;
            }
            //����������
            progressBarItem.Visible = true;
            this.Refresh();
            progressBarItem.Maximum = pSelectSet.Count + 100;
            progressBarItem.Minimum = 0;
            progressBarItem.Value = 0;

            ICursor pCursor1 = null, pCursor2 = null;
            pSelectSet.Search(pQueryFilterOrder1, false, out pCursor1);
            pSelectSet.Search(pQueryFilterOrder2, false, out pCursor2);

            IRow pRow = pCursor1.NextRow();
            int indexFeature = 0;
            IFeature pFeature = null;
            while (pRow != null)
            {
                //������
                progressBarItem.Value = progressBarItem.Value + 1;

                _FeaCnt = _FeaCnt + 1;
                //_DicFeature.Add(indexFeature, pRow.OID);
                InsterFeaToTable(pRow, _DataTableAll);
                indexFeature = indexFeature + 1;
                pRow = pCursor1.NextRow();
            }

            pRow = pCursor2.NextRow();
            while (pRow != null)
            {
                //������
                progressBarItem.Value = progressBarItem.Value + 1;

                _FeaCnt = _FeaCnt + 1;
                //_DicFeature.Add(indexFeature, pRow.OID);
                InsterFeaToTable(pRow, _DataTableAll);
                indexFeature = indexFeature + 1;
                pRow = pCursor2.NextRow();
            }
            int iIndex = 0;
            for (iIndex = 0; iIndex < 100; iIndex++)
            {
                //������
                progressBarItem.Value = progressBarItem.Value + 1;
                if (iIndex >= _DataTableAll.Rows.Count)
                {
                    break;
                }
                DataRow pDataRow = _DataTableAll.Rows[iIndex];
                DataRow pDataRow2 = _DataTablePart.NewRow();
                pDataRow2.ItemArray = pDataRow.ItemArray;
                _DataTablePart.Rows.Add(pDataRow2);
            }
            if (!_DicDatatable.ContainsKey(LayerName))
            {
                _DicDatatable.Add(LayerName, _DataTableAll);
            }
            progressBarItem.Value = progressBarItem.Maximum;
            _CurMaxID = iIndex - 1;
            _PageID = 1;
            this.textBoxPage.ControlText = _PageID.ToString();
            //��һҳ״̬
            if (_FeaCnt > _CurMaxID + 1)
            {
                this.buttonNextPage.Enabled = true;
            }
            else
            {
                this.buttonNextPage.Enabled = false;
            }
            //��һҳ״̬
            if (_CurMaxID > 100)
            {
                this.buttonLastPage.Enabled = true;
            }
            else
            {
                this.buttonLastPage.Enabled = false;
            }

            labelItemMemo.Text = "���ҵ�" + _FeaCnt.ToString() + "��Ҫ��";
            progressBarItem.Value = 0;
            progressBarItem.Enabled = false;
            //���������ֶ�
            for (int i = 0; i < gridRes.Columns.Count; i++)
            {
                DataGridViewColumn pColumn = gridRes.Columns[i];
                pColumn.HeaderText = _DataTablePart.Columns[i].ColumnName;
                pColumn.Name = _DataTablePart.Columns[i].Caption;
                string strFieldname = pColumn.Name;
                if (SysCommon.ModField._ListHideFields.Contains(pColumn.Name))
                {
                    pColumn.Visible = false;
                }
            }
            this.Refresh();
            IGraphicsContainer psGra = m_pMapControl.Map as IGraphicsContainer;
            if (isHighLight)
            {
                //drawPolyLineElement(pListGeometry, psGra);
                (m_pMapControl.Map as IActiveView).PartialRefresh(esriViewDrawPhase.esriViewBackground, null, null);
            }
        }
        /// <summary>��������Ӻ��� 20110802
        /// ������������������䴰�壬��������SQL��ѯ,ר�����ڵ�����ѯ,����,��·��ѯ
        /// </summary>
        /// <param name="pMap">����ѯ�ĵ�ͼ����</param>
        public void FillData(IFeatureClass pFeatureClass,string strLayerName, IQueryFilter pQueryFilter,IQueryFilter pQueryFilterOrder1,IQueryFilter pQueryFilterOrder2,bool isHighLight)
        {
            if (pFeatureClass == null)
            {
                return;
            }
            _QueryFeatureClass = pFeatureClass;

            if (_FeaCursor != null)
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(_FeaCursor);
                _FeaCursor = null;
            }
            //if (_DicFeature == null)
            //{
            //    _DicFeature = new Dictionary<int, int>();
            //}
            labelItem.Visible = true ;
            comboBoxFeatureLayer.Visible = true ;            
            if(!comboBoxFeatureLayer.Items.Contains(pFeatureClass.AliasName ))
            {
                comboBoxFeatureLayer.Items.Add(strLayerName);
            }
            for (int i = 0; i < comboBoxFeatureLayer.Items.Count; i++)
            {
                if (comboBoxFeatureLayer.Items[i].ToString() == strLayerName)
                {
                    comboBoxFeatureLayer.SelectedIndex = i;
                    break;
                }
            }
            this.comboBoxFeatureLayer.Enabled = false;

            //m_dataSourceGrid.Clear();

            //��ȡ��ѯ��ͼ��

            progressBarItem.Visible = false;
            this.Refresh();
            bool bFirst = true;
            //added by chulili 20110801 ��Ӵ��󱣻�
            if (pFeatureClass == null)
                return;
            _QueryFilterAll = pQueryFilter;
            _QueryFilterPart1 = pQueryFilterOrder1;
            _QueryFilterPart2 = pQueryFilterOrder2;
            //ѭ��ȡ��ѡ��Feature��ID���õ�Feature
            //��ʼ����
            _DataTablePart = InitDataTable(pFeatureClass);
            _DataTableAll = InitDataTable(pFeatureClass); 
            //�� �Ա㶯̬��ʾ
            this.gridRes.DataSource = _DataTablePart;

            ISelectionSet pSelectSet = null;
            try
            {
                pSelectSet = pFeatureClass.Select(pQueryFilter, esriSelectionType.esriSelectionTypeIDSet, esriSelectionOption.esriSelectionOptionNormal, (pFeatureClass as IDataset).Workspace);

            }
            catch(Exception errtmp)
            {}
            if (pSelectSet == null)
            {
                labelItemMemo.Text = "���ҵ�0��Ҫ��";
                progressBarItem.Visible = false;
                this.Refresh();
                return;
            }
            //����������
            progressBarItem.Visible = true;
            this.Refresh();
            progressBarItem.Maximum = pSelectSet.Count+100;
            progressBarItem.Minimum = 0;
            progressBarItem.Value = 0;

            ICursor pCursor1 = null, pCursor2 = null;

                pSelectSet.Search(pQueryFilterOrder1, false, out pCursor1);
                pSelectSet.Search(pQueryFilterOrder2, false, out pCursor2);

            IRow pRow = pCursor1.NextRow();
            int indexFeature=0;
            IFeature pFeature = null;
            while (pRow != null)
            {
                //������
                progressBarItem.Value = progressBarItem.Value + 1;

                _FeaCnt = _FeaCnt + 1;
                //_DicFeature.Add(indexFeature, pRow.OID );
                InsterFeaToTable(pRow, _DataTableAll);
                indexFeature = indexFeature + 1;
                pRow = pCursor1.NextRow();
            }

            pRow = pCursor2.NextRow();
            while (pRow != null)
            {
                //������
                progressBarItem.Value = progressBarItem.Value + 1;

                _FeaCnt = _FeaCnt + 1;
                //_DicFeature.Add(indexFeature, pRow.OID );
                InsterFeaToTable(pRow, _DataTableAll);
                indexFeature = indexFeature + 1;
                pRow = pCursor2.NextRow();
            }
            int iIndex = 0;
            for (iIndex = 0; iIndex < 100; iIndex++)
            {
                //������
                progressBarItem.Value = progressBarItem.Value + 1;
                if (iIndex >= _DataTableAll.Rows.Count)
                {
                    break;
                }
                DataRow pDataRow = _DataTableAll.Rows[iIndex];
                DataRow pDataRow2 = _DataTablePart.NewRow();
                pDataRow2.ItemArray = pDataRow.ItemArray;
                _DataTablePart.Rows.Add(pDataRow2);
            }
            progressBarItem.Value = progressBarItem.Maximum;
            _CurMaxID = iIndex - 1;
            _PageID = 1;
            this.textBoxPage.ControlText = _PageID.ToString();
            //��һҳ״̬
            if (_FeaCnt > _CurMaxID + 1)
            {
                this.buttonNextPage.Enabled = true;
            }
            else
            {
                this.buttonNextPage.Enabled = false ;
            }
            //��һҳ״̬
            if (_CurMaxID> 100)
            {
                this.buttonLastPage.Enabled = true;
            }
            else
            {
                this.buttonLastPage.Enabled = false ;
            }

            labelItemMemo.Text = "���ҵ�" + _FeaCnt.ToString() + "��Ҫ��";
            progressBarItem.Value = 0;
            progressBarItem.Enabled  = false;
            //���������ֶ�
            for (int i = 0; i < gridRes.Columns.Count; i++)
            {
                DataGridViewColumn pColumn = gridRes.Columns[i];
                pColumn.HeaderText = _DataTablePart.Columns[i].ColumnName;
                pColumn.Name = _DataTablePart.Columns[i].Caption;
                if (SysCommon.ModField._ListHideFields != null)
                {
                    if (SysCommon.ModField._ListHideFields.Contains(pColumn.Name))
                    {
                        pColumn.Visible = false;
                    }
                }
            }
            this.Refresh();
            IGraphicsContainer psGra = m_pMapControl.Map as IGraphicsContainer;
            if (isHighLight)
            {
                //drawPolyLineElement(pListGeometry, psGra);
                (m_pMapControl.Map as IActiveView).PartialRefresh(esriViewDrawPhase.esriViewBackground, null, null);
            }
        }
        ////����Ҫ�����ݵ�����ȥ
        //private void InsterFeaToTable(IFeature pFea, DataTable Dt)
        //{
        //    if (Dt == null) return;
        //    DataRow vRow = Dt.NewRow();

        //    for (int i = 0; i < Dt.Columns.Count; i++)
        //    {
        //        string strColumnName = Dt.Columns[i].ColumnName;
        //        int intIndex = pFea.Fields.FindField(strColumnName);
        //        if (intIndex < 0) continue;

        //        object obj = pFea.get_Value(intIndex);
        //        if (obj == null) continue;

        //        string strValue = obj.ToString();
        //        if (pFea.Fields.get_Field(intIndex).Type == esriFieldType.esriFieldTypeDouble)
        //        {
        //            double dblTempa = 0;
        //            double.TryParse(strValue, out dblTempa);
        //            strValue = string.Format("{0:f2}", dblTempa);
        //        }

        //        vRow[strColumnName] = strValue;
        //    }

        //    Dt.Rows.Add(vRow);
        //}
        //����Ҫ�����ݵ�����ȥ
        private void InsterFeaToTable(IRow pRow, DataTable Dtall)
        {
            if (Dtall == null) return;
            DataRow vRow = Dtall.NewRow();
            
            for (int i = 0; i < Dtall.Columns.Count; i++)
            {
                string strColumnName = Dtall.Columns[i].ColumnName;//�����ֶ���
                string strCaption = Dtall.Columns[i].Caption;//Ӣ���ֶ���
                int intIndex = pRow.Fields.FindField(strCaption);
                
                if (intIndex < 0) continue;

                object obj = pRow.get_Value(intIndex);
                if (obj == null) continue;

                string strValue = obj.ToString();
                if (pRow.Fields.get_Field(intIndex).Type == esriFieldType.esriFieldTypeDouble)
                {
                    double dblTempa = 0;
                    double.TryParse(strValue, out dblTempa);
                    strValue = string.Format("{0:f2}", dblTempa);
                }
                //added by chulili 20110818 ͳһ���ֶ�ֵ������Ӣ��ӳ��
                if (SysCommon.ModField._DicMatchFieldValue.Keys.Contains(strCaption))
                {
                    strValue = SysCommon.ModField.GetChineseOfFieldValue(strCaption, strValue);
                }
                //if (strColumnName.Equals("���������"))
                //{
                //    strValue = SysCommon.ModField.GetChineseNameOfGB(strValue);
                //}
                vRow[strColumnName] = strValue;
            }

            Dtall.Rows.Add(vRow);
        }
        private DataTable InitDataTable(IFeatureClass pFeaCls)
        {
            DataTable TempDt = new DataTable();

            for (int i = 0; i < pFeaCls.Fields.FieldCount; i++)
            {
                try
                {
                    IField pField = pFeaCls.Fields.get_Field(i);
                    if (pField.Type == esriFieldType.esriFieldTypeGeometry || pField.Type == esriFieldType.esriFieldTypeBlob) continue;
                    string strFieldName = pField.Name;
                    string strFieldChineseName = SysCommon.ModField.GetChineseNameOfField(strFieldName);
                    DataColumn pColumn = new DataColumn();
                    pColumn.ColumnName = strFieldChineseName;
                    pColumn.Caption = strFieldName;
                    pColumn.DataType = Type.GetType("System.String");
                    //�����������ַ���������
                    //TempDt.Columns.Add(strFieldChineseName, Type.GetType("System.String"));
                    TempDt.Columns.Add(pColumn);
                    if (pFeaCls.OIDFieldName == strFieldName)
                    {
                        _OIDfieldIndex = TempDt.Columns.IndexOf(strFieldChineseName);
                    }
                }
                catch(Exception err)
                {
                    continue;
                }
            }            

            //TempDt.Columns.Add("����", Type.GetType("System.String"));

            return TempDt;
        }
      

  

        //��ѯ����ر�
        private void frmQuery_FormClosed(object sender, FormClosedEventArgs e)
        {
            //m_pMapControl.CurrentTool = null;
            //yjladded Ϊ�˸�����ʾ��ѯ��Ҫ��
            IGraphicsContainer pGra = m_pMapControl.Map as IGraphicsContainer;
            pGra.Reset();
            IElement pEle = pGra.Next();
            
            while (pEle != null)
            {
                if ((pEle as IElementProperties).Name == "frmQueryToTable" || (pEle as IElementProperties).Name == "RoadQuery")
                {
                    pGra.DeleteElement(pEle);
                    pGra.Reset();
                }

                pEle = pGra.Next();
            }
            (m_pMapControl.Map as IActiveView).PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null); //changed by xisheng 20110803
            _DataTablePart.Clear();
            _DataTableAll.Clear();
            //_DicFeature.Clear();
            if (_DicDatatable != null)
            {
                _DicDatatable.Clear();
            }
            if (_DicFeatureClass != null)
            {
                _DicFeatureClass.Clear();
            }

            _DataTablePart = null;
            _DataTableAll = null;
            //_DicFeature = null;
            _DicDatatable = null;
            _DicFeatureClass = null;

            //20110731 xisheng
            //(pGra as IActiveView).Refresh();
            //yjladded Ϊ�˸�����ʾ��ѯ��Ҫ��
            
        }

        //added by chulili 20110802 �ӱ𴦿�������
        //��mapcontrol�ϻ������
        private void drawPolyLineElement(List<IGeometry> ListGeometry, IGraphicsContainer pGra)
        {
            if (ListGeometry == null)
                return;
            pGra.DeleteAllElements();
            //ISimpleFillSymbol pFillSymbol = new SimpleFillSymbolClass();
            ISimpleLineSymbol pLineSymbol = new SimpleLineSymbolClass();           
            
            try
            {
                IRgbColor pRGBColor = new RgbColorClass();
                pRGBColor.UseWindowsDithering = false;
                pRGBColor.Red = 0;
                pRGBColor.Green = 0;
                pRGBColor.Blue = 255;
                pLineSymbol.Color = pRGBColor;
                pLineSymbol.Width = 2;

                pRGBColor.Transparency = 0;
                //pFillSymbol.Style = esriSimpleFillStyle.esriSFSDiagonalCross;
                for (int i = 0; i < ListGeometry.Count; i++)
                {
                    IPolyline pLine = ListGeometry[i] as IPolyline;
                    if (pLine != null)
                    {
                        
                        ILineElement pPolylineElement = new LineElementClass();
                        (pPolylineElement as IElement).Geometry = pLine as IGeometry;
                        pPolylineElement.Symbol = pLineSymbol;
                        //(pPolylineElement as IElementProperties).Name == "frmQueryToTable";
                        IElementProperties pProperty = pPolylineElement as IElementProperties;
                        pProperty.Name = "RoadQuery";
                        pGra.AddElement(pPolylineElement as IElement, 0);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("���Ʒ�Χ����:" + ex.Message, "ϵͳ��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                pLineSymbol = null;
            }
        }

        private void buttonNextPage_Click(object sender, EventArgs e)
        {           
            _DataTablePart.Clear();
            if (_CurMaxID + 1 >= _FeaCnt)
            {
                return;
            }
            int i = 0;
            for (i = _CurMaxID + 1; i <= _CurMaxID+100; i++)
            {
                if (i >= _FeaCnt)
                {
                    break;
                }
                DataRow pDataRow = _DataTableAll.Rows[i];
                DataRow pDataRow2 = _DataTablePart.NewRow();
                pDataRow2.ItemArray = pDataRow.ItemArray;
                _DataTablePart.Rows.Add(pDataRow2);
            }
            _CurMaxID = i-1;
            _PageID = _PageID + 1;
            this.textBoxPage.ControlText = _PageID.ToString();
            //��һҳ״̬
            if (_FeaCnt > _CurMaxID + 1)
            {
                this.buttonNextPage.Enabled = true;
            }
            else
            {
                this.buttonNextPage.Enabled = false;
            }
            //��һҳ״̬
            if (_CurMaxID > 100)
            {
                this.buttonLastPage.Enabled = true;
            }
            else
            {
                this.buttonLastPage.Enabled = false;
            }
        }

        private void buttonLastPage_Click(object sender, EventArgs e)
        {
            _DataTablePart.Clear();
            int i = 0;
            if (_PageID <= 1)
            {
                return;
            }
            for (i = (_PageID - 2) * 100; i < (_PageID - 2) * 100 + 100; i++)
            {
                if (i >= _FeaCnt)
                {
                    break;
                }
                DataRow pDataRow = _DataTableAll.Rows[i];
                DataRow pDataRow2 = _DataTablePart.NewRow();
                pDataRow2.ItemArray = pDataRow.ItemArray;
                _DataTablePart.Rows.Add(pDataRow2);
            }
            _CurMaxID = i - 1;
            _PageID = _PageID - 1;
            this.textBoxPage.ControlText = _PageID.ToString();
            //��һҳ״̬
            if (_FeaCnt > _CurMaxID + 1)
            {
                this.buttonNextPage.Enabled = true;
            }
            else
            {
                this.buttonNextPage.Enabled = false;
            }
            //��һҳ״̬
            if (_CurMaxID > 100)
            {
                this.buttonLastPage.Enabled = true;
            }
            else
            {
                this.buttonLastPage.Enabled = false;
            }
        }

        private void buttonAll_Click(object sender, EventArgs e)
        {
            _DataTablePart.Clear();
            int i = 0;
            progressBarItem.Enabled = true;
            this.Refresh();
            progressBarItem.Maximum = _FeaCnt;
            progressBarItem.Value = 0;
            for (i = 0; i < _FeaCnt; i++)
            {
                progressBarItem.Value = progressBarItem.Value + 1;
                DataRow pDataRow = _DataTableAll.Rows[i];
                DataRow pDataRow2 = _DataTablePart.NewRow();
                pDataRow2.ItemArray = pDataRow.ItemArray;
                _DataTablePart.Rows.Add(pDataRow2);
            }
            _CurMaxID = i - 1;
            _PageID =  1;
            this.textBoxPage.ControlText = _PageID.ToString();
            //��һҳ״̬ 
            this.buttonNextPage.Enabled = false;

            //��һҳ״̬
            this.buttonLastPage.Enabled = false;
            progressBarItem.Value = 0;
        }

        private void gridRes_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

            string strFeatureOID = gridRes.SelectedRows[0].Cells[_QueryFeatureClass.OIDFieldName].Value.ToString();
            if (strFeatureOID.Equals(""))
                return;
            
            
            try
            {
                int iFeatureOID = Convert.ToInt32(strFeatureOID);
                IFeature pFeature = _QueryFeatureClass.GetFeature(iFeatureOID);
                
                if (pFeature == null)
                {
                    return;
                }
                //yjladded Ϊ�˸�����ʾ��ѯ��Ҫ��
                IGraphicsContainer pGra = m_pMapControl.Map as IGraphicsContainer;
                pGra.Reset();
                IElement pEle = pGra.Next();
                //yjl0729,add,�ֲ�ˢ��δ������
                (pGra as IActiveView).PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);//����ʱ��2��
                while (pEle != null)
                {
                    if ((pEle as IElementProperties).Name == _FlashTag)
                    {
                        pGra.DeleteElement(pEle);
                        pGra.Reset();
                    }

                    pEle = pGra.Next();
                }
                (pGra as IActiveView).PartialRefresh(esriViewDrawPhase.esriViewBackground, null, null);
                //yjladded Ϊ�˸�����ʾ��ѯ��Ҫ��
                if (pFeature != null)
                {
                    SysCommon.Gis.ModGisPub.ZoomToFeature(m_pMapControl, pFeature);
                    Application.DoEvents();
                    ModDBOperator._FlashTagName = _FlashTag;
                    ModDBOperator.FlashFeature(pFeature, m_pMapControl);
                    //m_pMapControl.FlashShape(pfeature.Shape, 2, 500, null);
                }
            }
            catch
            {
            }
        }

        private void textBoxPage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    int iPage = Convert.ToInt32(textBoxPage.ControlText.Trim());
                    GotoPage(iPage );
                }
                catch
                { }
            }
        }
        private void GotoPage(int pageID)
        {
            if ((pageID - 1) * 100 < _FeaCnt)
            {
                _DataTablePart.Clear();
                int i = 0;
                for (i = (pageID - 1) * 100; i < (pageID - 1) * 100+100; i++)
                {
                    if (i >= _FeaCnt)
                    {
                        break;
                    }
                    DataRow pDataRow = _DataTableAll.Rows[i];
                    DataRow pDataRow2 = _DataTablePart.NewRow();
                    pDataRow2.ItemArray = pDataRow.ItemArray;
                    _DataTablePart.Rows.Add(pDataRow2);
                }
                _CurMaxID = i - 1;
                _PageID = pageID;

                //��һҳ״̬
                if (_FeaCnt > _CurMaxID + 1)
                {
                    this.buttonNextPage.Enabled = true;
                }
                else
                {
                    this.buttonNextPage.Enabled = false;
                }
                //��һҳ״̬
                if (_CurMaxID > 100)
                {
                    this.buttonLastPage.Enabled = true;
                }
                else
                {
                    this.buttonLastPage.Enabled = false;
                }
            }
        }

        private void textBoxPage_KeyPress(object sender, KeyPressEventArgs e)
        {
            string strnum = "0123456789";
            if (!char.IsControl(e.KeyChar ) && (!strnum.Contains(e.KeyChar.ToString())))
            {
                e.Handled = true;
            }
        }

        private void comboBoxFeatureLayer_SelectedIndexChanged(object sender, EventArgs e)
        {
            string strLayerName = comboBoxFeatureLayer.SelectedItem.ToString()  ;
            if (_QueryFilterAll == null)
            {
                return;
            }
            if (_DicFeatureClass.ContainsKey(strLayerName))
            {
                FillData(strLayerName);
            }

        }
        private void FillData(string strLayerName)
        {
            if (_DicDatatable == null)
            {
                _DicDatatable = new Dictionary<string, DataTable>();
            }
            if (_DicFeatureClass.ContainsKey(strLayerName))
            {
                _QueryFeatureClass = _DicFeatureClass[strLayerName];
            }
            if (_DicDatatable.ContainsKey(strLayerName))
            {
                _DataTableAll = _DicDatatable[strLayerName];
                _FeaCnt = _DataTableAll.Rows.Count;
                //����������
                progressBarItem.Visible = true;
                this.Refresh();
                progressBarItem.Maximum = 100;
                progressBarItem.Minimum = 0;
                progressBarItem.Value = 0;
            }
            else
            {
                //_DataTableAll = null;
                _DataTableAll = InitDataTable(_QueryFeatureClass);
                //�� �Ա㶯̬��ʾ
                ISelectionSet pSelectSet = _QueryFeatureClass.Select(_QueryFilterAll, esriSelectionType.esriSelectionTypeIDSet, esriSelectionOption.esriSelectionOptionNormal, (_QueryFeatureClass as IDataset).Workspace);
                if (pSelectSet == null)
                {
                    labelItemMemo.Text = "���ҵ�0��Ҫ��";
                    progressBarItem.Visible = false;
                    this.Refresh();
                    return;
                }
                //����������
                progressBarItem.Visible = true;
                this.Refresh();
                progressBarItem.Maximum = pSelectSet.Count + 100;
                progressBarItem.Minimum = 0;
                progressBarItem.Value = 0;

                ICursor pCursor1 = null, pCursor2 = null;
                pSelectSet.Search(_QueryFilterPart1 , false, out pCursor1);
                pSelectSet.Search(_QueryFilterPart2 , false, out pCursor2);

                IRow pRow = pCursor1.NextRow();
                int indexFeature = 0;
                _FeaCnt = 0;
                IFeature pFeature = null;
                while (pRow != null)
                {
                    //������
                    progressBarItem.Value = progressBarItem.Value + 1;

                    _FeaCnt = _FeaCnt + 1;
                    //_DicFeature.Add(indexFeature, pRow.OID);
                    InsterFeaToTable(pRow, _DataTableAll);
                    indexFeature = indexFeature + 1;
                    pRow = pCursor1.NextRow();
                }

                pRow = pCursor2.NextRow();
                while (pRow != null)
                {
                    //������
                    progressBarItem.Value = progressBarItem.Value + 1;

                    _FeaCnt = _FeaCnt + 1;
                    //_DicFeature.Add(indexFeature, pRow.OID);
                    InsterFeaToTable(pRow, _DataTableAll);
                    indexFeature = indexFeature + 1;
                    pRow = pCursor2.NextRow();
                }
            }
            int iIndex = 0;
            _DataTablePart = null;
            _DataTablePart = InitDataTable(_QueryFeatureClass);

            _DataTablePart.Clear();
            for (iIndex = 0; iIndex < 100; iIndex++)
            {
                //������
                progressBarItem.Value = progressBarItem.Value + 1;
                if (iIndex >= _FeaCnt)
                {
                    break;
                }
                DataRow pDataRow = _DataTableAll.Rows[iIndex];
                DataRow pDataRow2 = _DataTablePart.NewRow();
                pDataRow2.ItemArray = pDataRow.ItemArray;
                _DataTablePart.Rows.Add(pDataRow2);
            }
            gridRes.DataSource = _DataTablePart;
            //���������ֶ�
            for (int i = 0; i < gridRes.Columns.Count; i++)
            {
                DataGridViewColumn pColumn = gridRes.Columns[i];
                pColumn.HeaderText = _DataTablePart.Columns[i].ColumnName;
                pColumn.Name = _DataTablePart.Columns[i].Caption ;
                string strTmpName=pColumn.Name;
                if (SysCommon.ModField._ListHideFields.Contains(pColumn.Name))
                {
                    pColumn.Visible = false;
                }
            }
            progressBarItem.Value = progressBarItem.Maximum;
            _CurMaxID = iIndex - 1;
            _PageID = 1;
            this.textBoxPage.ControlText = _PageID.ToString();
            //��һҳ״̬
            if (_FeaCnt > _CurMaxID + 1)
            {
                this.buttonNextPage.Enabled = true;
            }
            else
            {
                this.buttonNextPage.Enabled = false;
            }
            //��һҳ״̬
            if (_CurMaxID > 100)
            {
                this.buttonLastPage.Enabled = true;
            }
            else
            {
                this.buttonLastPage.Enabled = false;
            }

            labelItemMemo.Text = "���ҵ�" + _FeaCnt.ToString() + "��Ҫ��";
            progressBarItem.Value = 0;
            progressBarItem.Enabled = false;
            this.Refresh();
        }
    }
} 