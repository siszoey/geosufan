using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;
using SysCommon;
using System.Collections;
using ESRI.ArcGIS.Display;
using System.Xml;
using System.IO;

namespace GeoStatistics
{
    public partial class UcArea : UserControl
    {
        private string _LayerTreePath = System.Windows.Forms.Application.StartupPath + "\\..\\res\\xml\\��ѯͼ����_UcArea.xml"; //ͼ��Ŀ¼�ļ�·��
        private IWorkspace _WorkSpace = null;
        string m_strAreaName = "";
        private IActiveViewEvents_Event m_pActiveViewEvents;
        private bool _Writelog = true;  //added by chulili 2012-09-07 �Ƿ�д��־
        public bool WriteLog
        {
            get
            {
                return _Writelog;
            }
            set
            {
                _Writelog = value;
            }
        }
        public UcArea()
        {
            InitializeComponent();
            InitGridView(this.gridRes);
            m_lstFeaLyrs = new List<IFeatureLayer>();

            comboLayers.Text = "���ѡ�� ͳ��ͼ��";//�ı�ͼ��ѡ��ʽ ��ʼ��ͼ���б����� xisheng 20111119
            InitLayersTree();
        }
        //�ı�ͼ��ѡ��ʽ ����֮ǰ���� xisheng 20111119
        //����comboLayers��ʼ�� comboBoxExJsFld ����
        //20111102 wgf
        public void InitJsFld()
        {
            //if (m_pCurFeaCls == null)
            //    return;

            ////added by xisheng 20110827 ��ʼ���ֶ���Ӣ�Ķ����ֵ�
            //if (SysCommon.ModField._DicFieldName.Keys.Count == 0)
            //{
            //    SysCommon.ModField.InitNameDic(Plugin.ModuleCommon.TmpWorkSpace, SysCommon.ModField._DicFieldName, "���Զ��ձ�");

            //}
            ////changed by xisheng 20110827 ͳһ��������ֵӳ��
            //if (SysCommon.ModField._DicMatchFieldValue.Keys.Count == 0)
            //{
            //    SysCommon.ModField.InitMatchFieldValueDic(Plugin.ModuleCommon.TmpWorkSpace);
            //}

            ////��ȡ���Խṹ
            //this.comboBoxExJsFld.Items.Clear();
            //string sFieldName;
            //int iCount = m_pCurFeaCls.Fields.FieldCount;
            //for (int ii = 0; ii < iCount; ii++)
            //{
            //    if (m_pCurFeaCls.Fields.get_Field(ii).Type == esriFieldType.esriFieldTypeDouble)  //������
            //    {
            //        sFieldName = "";
            //        sFieldName = m_pCurFeaCls.Fields.get_Field(ii).Name;

            //        //����������ֶ�
            //        string strFieldChineseName = SysCommon.ModField.GetChineseNameOfField(sFieldName);

            //        comboBoxExJsFld.Items.Add(strFieldChineseName);
            //    }
            //}

        }

        /// <summary>
        /// ���ù�������Enable xisheng 20110804
        /// </summary>
        public bool CurSliderEnable
        {
            set
            {
                this.sliderBuffer.Enabled = value;
                this.dblBuffLen.Enabled = value;
            }
        }
        /// <summary>
        /// ��ǰ��Ҫ����
        /// </summary>
        private IFeatureClass m_pCurFeaCls = null;
        private ILayer  m_CurLayer = null;
        private List<IFeatureLayer> m_lstFeaLyrs;
        //
        private IMapControlDefault m_pMap;
        public IMapControlDefault CurMap
        {
            set { m_pMap = value; }
        }

        public bool boolok = false;
        private ESRI.ArcGIS.Geometry.IGeometry m_pGeometry;
        private ESRI.ArcGIS.Geometry.IGeometry m_pGeometry_start;
        public ESRI.ArcGIS.Geometry.IGeometry CurGeometry
        {
            set 
            {
                m_pGeometry = value;
                m_pGeometry_start = value;
            }
        }

        public DevComponents.DotNetBar.Controls.DataGridViewX CurGrid
        {
            get { return this.gridRes; }
        }

        public List<string> ListNum
        {
            get;
            set;
        }

        //�ı�ͼ��ѡ��ʽ ����֮ǰ���� xisheng 20111119
        public void InitLayers()
        {
        //    if (m_pMap == null) return;
        //    m_lstFeaLyrs.Clear();
        //    this.dblBuffLen.Text = "0";

        //    List<ILayer> lstLyrs = null;
        //    SysCommon.Gis.ModGisPub.GetLayersByMap(m_pMap.Map, ref lstLyrs);

        //    for (int i = 0; i < lstLyrs.Count; i++)
        //    {
        //        ILayer pLyr = lstLyrs[i];
        //        if (!(pLyr is IGeoFeatureLayer)) continue;

        //        IFeatureLayer pFeaLyr = pLyr as IFeatureLayer;
        //        if (pFeaLyr.FeatureClass == null) continue;
        //        IFeatureClass pFeaCls = pFeaLyr.FeatureClass;

        //        if (pFeaCls == null) continue;
        //        if (pFeaCls.FeatureType != esriFeatureType.esriFTSimple) continue;
        //        //if (pFeaCls.ShapeType != ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon) continue;
        //        m_lstFeaLyrs.Add(pFeaLyr);

        //        this.comboLayers.Items.Add(pLyr.Name);
        //    }

      
        //    //���¼��ŵ������� ��Ķ��ˢ��
        //    this.comboLayers.SelectedIndexChanged += new EventHandler(comboLayers_SelectedIndexChanged);

        //    if (this.comboLayers.Items.Count > 0)
        //    {
        //        this.comboLayers.SelectedIndex = 0;

        //        //wgf 20111102
        //        IFeatureLayer pFeaLyr = m_lstFeaLyrs[this.comboLayers.SelectedIndex];
        //        if (pFeaLyr != null)
        //        {
        //            if (pFeaLyr.FeatureClass != null)
        //            {
        //                m_pCurFeaCls = pFeaLyr.FeatureClass;
        //            }
                 
        //        }
        //        //end
        //    }

        //    InitJsFld();
        }

        void comboLayers_SelectedIndexChanged(object sender, EventArgs e)
        {
            //throw new Exception("The method or operation is not implemented.");
        }

        private void btnStatistics_Click(object sender, EventArgs e)
        {
            if (m_pCurFeaCls == null) 
                return;

            this.Cursor = Cursors.WaitCursor;
            try
            {
                //�ռ����
                IQueryFilter pQueryFilter = null;
                if (m_pGeometry != null)
                {
                    ISpatialFilter pSpatialFilter = new SpatialFilterClass();
                    pSpatialFilter.Geometry = m_pGeometry;
                    pSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                    pQueryFilter = pSpatialFilter as ISpatialFilter;
                }
                else
                {
                    pQueryFilter = new QueryFilterClass();
                }
                //��������
                pQueryFilter.WhereClause = this.txtSQL.Text;

                IFeatureClass pFeaCls = m_pCurFeaCls;

                IRelQueryTable pRelQueryTable = pFeaCls as IRelQueryTable;
                if (pRelQueryTable != null)
                {
                    IRelationshipClass pRelShip = pRelQueryTable.RelationshipClass;
                    pFeaCls = pRelShip.OriginClass as IFeatureClass;
                }

                //��ʼ����
                DataTable dt = null;

                dt=InitDataTalbe(pFeaCls,m_CurLayer);


                //�� �Ա㶯̬��ʾ
                this.gridRes.DataSource = dt;

                IFeatureCursor pFeaCursor = pFeaCls.Search(pQueryFilter, false);
                IFeature pFea = pFeaCursor.NextFeature();

                while (pFea != null)
                {

                    InsterFeaToTable(pFea, dt, m_pGeometry);
                    pFea = pFeaCursor.NextFeature();
                }

                System.Runtime.InteropServices.Marshal.ReleaseComObject(pFeaCursor);
                pFeaCursor = null;

                //wgf 20111102 ���ѡ���˼����ֶ� �����͸��ݱ������㡰ͳ�������
                if (this.comboBoxExJsFld.SelectedIndex >0)
                {
                    ModefyFldArea(dt);
                }
                //end 

                if (m_strAreaName != "")
                {
                    //��������ۺ�
                    GetSum(dt, pFeaCls);
                }
                //added by chulili 20110901���������ֶ�
                for (int i = 0; i < gridRes.Columns.Count; i++)
                {
                    DataGridViewColumn pColumn = gridRes.Columns[i];
                    //pColumn.HeaderText = dt.Columns[i].ColumnName;                    
                    //pColumn.Name = dt.Columns[i].Caption; //changed by chulili 20111011ɾ������Ժ� �ֶο�����ʾ������
                    string strColumnName = dt.Columns[i].Caption;
                    
                    if (SysCommon.ModField._ListHideFields != null)
                    {
                        if (SysCommon.ModField._ListHideFields.Contains(strColumnName))

                        {
                            pColumn.Visible = false;
                        }
                    }
                }
                if (this.WriteLog)
                {
                    Plugin.LogTable.Writelog("Ҫ��ͳ��,ͳ��ͼ��Ϊ" + comboLayers.Text + "");//xisheng ��־��¼ 0928;
                }
                //end added by chulili 
                //���α��
                EditGrid(pFeaCls);

                //��ʼͳ��ͼ��combobox
                InitChartWhere();
            }
            catch (Exception ex)
            {
                
            }

            this.Cursor = Cursors.Default ;

        }

        private void EditGrid(IFeatureClass pFeaCls)
        {
            for(int i=0;i<this.gridRes.Columns.Count;i++)
            {
                string strName=this.gridRes.Columns[i].Name;
                if(pFeaCls.Fields.FindField(strName)>-1)
                {
                    IField pField=pFeaCls.Fields.get_Field(pFeaCls.Fields.FindField(strName));
                    if (pField.AliasName != "") this.gridRes.Columns[i].HeaderText = pField.AliasName;
                }
            }
        }


        private void InitChartWhere()
        {
            this.comboType.Items.Clear();
            this.comboTypeY.Items.Clear();

            DataTable dt = this.gridRes.DataSource as DataTable;

            for (int i = 0; i < this.gridRes.ColumnCount; i++)
            {
                this.comboType.Items.Add(this.gridRes.Columns[i].HeaderText);
                if (ListNum.Contains(gridRes.Columns[i].HeaderText.Trim()))
                {
                    comboTypeY.Items.Add(gridRes.Columns[i].HeaderText.Trim());//������ֵ�͵�ͳ��
                }
            }
            if (comboType.Items.Count > 0)
            {//added by xisheng 20110819
                this.comboType.SelectedIndex = 0;
            }
            if (comboTypeY.Items.Count > 0)
            {//added by xisheng 20110819
                if (m_strAreaName != "" && ListNum.Contains(m_strAreaName))
                    this.comboTypeY.SelectedItem = m_strAreaName;
                else
                    this.comboTypeY.SelectedIndex = 0;
            }
            
          
        }

        private void InitGridView(DevComponents.DotNetBar.Controls.DataGridViewX vGrid)
        {
            vGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            vGrid.ReadOnly = true;
            vGrid.AllowUserToAddRows = false;
            vGrid.AllowUserToDeleteRows = false;
        }

        //wgf 20111102
        private void ModefyFldArea(DataTable dt)
        {
            if (dt == null)
                return;
            string strGeoName = "";
            string strJsFld = "";
            strJsFld = this.comboBoxExJsFld.SelectedItem.ToString();
            strGeoName = m_strAreaName;
            if (dt.Columns.Contains(strGeoName))
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string strAllFld="";
                    string strArea = dt.Rows[i][strGeoName].ToString();
                    string strJsArea = dt.Rows[i][strJsFld].ToString();
                    if(dt.Columns.Contains("ͳ�Ƴ���"))
                    {
                        strAllFld = "SHAPE_Length";
                    }
                    else
                    {
                        strAllFld = "SHAPE_Area";
                    }
                    if(dt.Columns.Contains(strAllFld))
                    {
                        string strAreaValue = "";
                        string strAllArea = dt.Rows[i][strAllFld].ToString();
                        double dblSubArea = 0;
                        double dJsArea = 0;
                        double dAllArea = 0;
                        double dResultArea = 0;
                        double.TryParse(strArea, out dblSubArea);
                        double.TryParse(strJsArea, out dJsArea);
                        double.TryParse(strAllArea, out dAllArea);
                        if (dblSubArea > 0 && dJsArea > 0 && dAllArea> 0)
                        {
                            dResultArea = dJsArea * (dblSubArea/dAllArea);
                            strAreaValue = string.Format("{0:f2}", dResultArea);
                            dt.Rows[i].BeginEdit();
                            dt.Rows[i][strGeoName]= strAreaValue;
                            dt.Rows[i].EndEdit();
                        }
                    }
                   
                }

     
            }

        }
        //end

        private void GetSum(DataTable dt, IFeatureClass pFeaCls)
        {
            string strGeoName = "";

            strGeoName = m_strAreaName;
            double dblArea = 0;

            //���һ��
            DataRow vRow = dt.NewRow();

            if (dt.Columns.Contains(strGeoName))
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string strArea = dt.Rows[i][strGeoName].ToString();

                    double dblSubArea = 0;
                    if (double.TryParse(strArea, out dblSubArea))
                    {
                        dblArea = dblArea + dblSubArea;
                    }
                }

                vRow[strGeoName] = dblArea.ToString();
            }

            ////��Ӹ�����
            //vRow["����"] = dt.Rows.Count.ToString();

            //��Ӹ����� �ڵ�һ��
            if (dt.Columns.Count > 1)
            {
                vRow[0] = "�ܼ�";
            }
            dt.Rows.Add(vRow);
        }

        //����Ҫ�����ݵ�����ȥ
        private void InsterFeaToTable(IFeature pFea, DataTable Dt,IGeometry pGeo)
        {
            if (Dt == null) return;
            ListNum = new List<string>();
            DataRow vRow = Dt.NewRow();
            Exception ex=null ;
                for (int i = 0; i < Dt.Columns.Count; i++)
                {

                    string strColumnName = Dt.Columns[i].ColumnName;//�����ֶ���
                    string strCaption = Dt.Columns[i].Caption;//Ӣ���ֶ���
                    int intIndex = pFea.Fields.FindField(strCaption);
                    if (intIndex < 0) continue;

                    object obj = pFea.get_Value(intIndex);
                    if (obj == null) continue;

                    string strValue = obj.ToString();
                    if (pFea.Fields.get_Field(intIndex).Type == esriFieldType.esriFieldTypeDouble)
                    {
                        double dblTempa = 0;
                        bool result = double.TryParse(strValue, out dblTempa);

                        if (result)
                        {
                            if (!ListNum.Contains(strColumnName))
                                ListNum.Add(strColumnName);//�����ֵ�͵��� 20110820 xisheng
                        }
                        strValue = string.Format("{0:f2}", dblTempa);
                    }
                    if (SysCommon.ModField._DicMatchFieldValue.Keys.Contains(strCaption))
                    {
                        strValue = SysCommon.ModField.GetChineseOfFieldValue(strCaption, strValue);

                    }

                    vRow[strColumnName] = strValue;
                }

                try
                {
                    //�������״����ӳ���
                    if (pFea.FeatureType == esriFeatureType.esriFTSimple && pFea.Shape != null && pFea.Shape.GeometryType == esriGeometryType.esriGeometryPolyline && pGeo != null)
                    {
                        double dblLength = 0;
                        //�󳤶�
                        if (!pFea.Shape.IsEmpty)
                        {
                            IPolyline pPolygon = pFea.Shape as IPolyline;
                            if (pPolygon != null) dblLength = pPolygon.Length;

                            vRow["ͳ�Ƴ���"] = string.Format("{0:f2}", dblLength);
                            ListNum.Insert(0, "ͳ�Ƴ���");
                        }
                    }
                }
                catch (Exception e1)
                { }
                try
                {
                    //�������״Ҫ�� �����һ�����
                    if (pFea.FeatureType == esriFeatureType.esriFTSimple && pFea.Shape != null && pFea.Shape.GeometryType == esriGeometryType.esriGeometryPolygon && pGeo != null)
                    {
                        double dblIntersectArea = 0;
                        //�����
                        if (!pFea.Shape.IsEmpty)
                        {
                            ITopologicalOperator ptopo = pGeo as ITopologicalOperator;
                            IGeometry pIntersectGeometry = ptopo.Intersect(pFea.Shape, esriGeometryDimension.esriGeometry2Dimension);
                            if (pIntersectGeometry != null)
                            {
                                IArea pArea = pIntersectGeometry as IArea;
                                if (pArea != null) dblIntersectArea = pArea.Area;
                            }

                            vRow["ͳ�����"] = string.Format("{0:f2}", dblIntersectArea);
                            ListNum.Insert(0, "ͳ�����");
                        }
                    }
                }
                catch (Exception e2)
                { }

                Dt.Rows.Add(vRow);
            

        }

        private DataTable InitDataTalbe(IFeatureClass pFeaCls,ILayer pLayer)
        {
            DataTable TempDt = new DataTable();
            //added by xisheng 20110827 ��ʼ���ֶ���Ӣ�Ķ����ֵ�
            if (SysCommon.ModField._DicFieldName.Keys.Count == 0)

            {
                SysCommon.ModField.InitNameDic(Plugin.ModuleCommon.TmpWorkSpace, SysCommon.ModField._DicFieldName, "���Զ��ձ�");

            }
            //changed by xisheng 20110827 ͳһ��������ֵӳ��
            if (SysCommon.ModField._DicMatchFieldValue.Keys.Count == 0)

            {
                SysCommon.ModField.InitMatchFieldValueDic(Plugin.ModuleCommon.TmpWorkSpace);
            }
            //������Щ�ֶ�
            if (SysCommon.ModField._ListHideFields == null)
            {
                SysCommon.ModField.GetHideFields();
            }
            ILayerFields pLayerFields = pLayer as ILayerFields;
            for (int i = 0; i < pFeaCls.Fields.FieldCount; i++)
            {
                IField pField = pFeaCls.Fields.get_Field(i);
                if (pField.Type == esriFieldType.esriFieldTypeGeometry || pField.Type == esriFieldType.esriFieldTypeBlob) continue;
                if (pField.Type != esriFieldType.esriFieldTypeOID)
                {
                    if (pLayerFields != null)
                    {
                        IFieldInfo pFieldInfo = pLayerFields.get_FieldInfo (i);
                        //pLayerFields .FieldInfo[i]
                        if (pFieldInfo.Visible == false)
                        {
                            continue;
                        }
                    }
                }
                //�����������ַ���������

                //���ò�ѯ����Ӣ�Ķ����ֵ�����ʼ����ṹ xisheng 20110827
                string strFieldName = pField.Name;
                string strFieldChineseName = SysCommon.ModField.GetChineseNameOfField(strFieldName);
                DataColumn pColumn = new DataColumn();
                pColumn.ColumnName = strFieldChineseName;
                pColumn.Caption = strFieldName;
                pColumn.DataType = Type.GetType("System.String");
                TempDt.Columns.Add(pColumn);
            }

            //�������״Ҫ�� �����һ������
            if (pFeaCls.FeatureType == esriFeatureType.esriFTSimple && pFeaCls.ShapeType == esriGeometryType.esriGeometryPolyline)
            {
                m_strAreaName = "ͳ�Ƴ���";
                TempDt.Columns.Add(m_strAreaName, Type.GetType("System.String"));
            }

            //�������״Ҫ�� �����һ�����
            if (pFeaCls.FeatureType == esriFeatureType.esriFTSimple && pFeaCls.ShapeType == esriGeometryType.esriGeometryPolygon)
            {
                m_strAreaName = "ͳ�����";
                TempDt.Columns.Add(m_strAreaName, Type.GetType("System.String"));
            }
            if (pFeaCls.FeatureType == esriFeatureType.esriFTSimple && pFeaCls.ShapeType == esriGeometryType.esriGeometryPoint)
            {
                m_strAreaName = "";
                //TempDt.Columns.Add(m_strAreaName, Type.GetType("System.String"));
            }

            return TempDt;
        }

        private void btnChart_Click(object sender, EventArgs e)
        {
            if (this.comboType.SelectedIndex < 0 || this.comboTypeY.SelectedIndex < 0)
            {
                MessageBox.Show("����ָ�������ͳ�Ƶ��ֶ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string strStatictisValue = "";
            string strTitle = "";
            string strColumnTitle = "";
            ArrayList XLable = new ArrayList();
            ArrayList ColorGuider = new ArrayList();
            ArrayList[] CharData = { new ArrayList() };

            try
            {
                strStatictisValue = comboTypeY.SelectedItem.ToString();
                strTitle = strStatictisValue + "ͳ��ͼ";
                strColumnTitle = strStatictisValue;


                string strWhere = this.comboType.Text;
                for (int i = 0; i < this.gridRes.Columns.Count; i++)
                {
                    string strTemp = this.gridRes.Columns[i].HeaderText;
                    if (strTemp == strWhere)
                    {
                        strWhere = this.gridRes.Columns[i].DataPropertyName;
                        break;
                    }
                }

                //
                SortedDictionary<string, double> values = new SortedDictionary<string, double>();
                string strValue = strStatictisValue;
                DataTable dt = this.gridRes.DataSource as DataTable;
                int count = 0;
                if (dt.Columns.Contains("ͳ�Ƴ���") || dt.Columns.Contains("ͳ�����"))//������ߺ�����ȡ���һ��
                {
                    count= dt.Rows.Count - 1;
                }
                else//����ǵ㣬û���������һ����ȡ���һ��
                {
                   count=  dt.Rows.Count;
                }
                for (int i = 0; i < count; i++)
                {
                    string strKey = "";
                    object obj = dt.Rows[i][strWhere];
                    if (obj == null)
                    {
                        strKey = "����";
                    }
                    else
                    {
                        strKey = obj.ToString();
                    }
                    if (strKey == "") strKey = "����";

                    //ֵ
                    string strTempValue = "";
                    object objvalue = dt.Rows[i][strValue];
                    if (objvalue == null)
                    {
                        strTempValue = "0";
                    }
                    else
                    {
                        strTempValue = objvalue.ToString();
                    }


                    double dblSubArea = 0;
                    if (!double.TryParse(strTempValue, out dblSubArea)) dblSubArea = 0;
                   
                    if (values.ContainsKey(strKey))
                    {
                        double dblArea = 0;
                        values.TryGetValue(strKey, out dblArea);
                        dblArea = dblArea + dblSubArea;
                        values.Remove(strKey);
                        values.Add(strKey, dblArea);
                    }
                    else
                    {
                        values.Add(strKey, dblSubArea);
                    }
                   
                }
                ////��ȡͳ������ ZQ  20111014 add 
                foreach (string Keys in values.Keys)
                {
                    XLable.Add(Keys);
                    CharData[0].Add(values[Keys]);
                }
                if (this.WriteLog)
                {
                    Plugin.LogTable.Writelog("��ͳ��ͼ,�����ֶ�Ϊ" + this.comboType.Text + ",ͳ�Ƶ���" + strColumnTitle);
                }
                ////����ͳ��ͼ��ʾ�Ŀؼ�    ZQ 20111014  Modify
                //frmStatisticsCharlte pfrm = new frmStatisticsCharlte();
                //pfrm.strTitle = strTitle;
                //pfrm.strXLabels = this.comboType.Text;
                //pfrm.strYLabels = strColumnTitle;
                //pfrm.ArrXLable = XLable;
                //pfrm.ArrCharData = CharData;
                //pfrm.ArrColorGuider = ColorGuider;
                //pfrm.ShowDialog();
                //frmStatisticsPic frm = new frmStatisticsPic();
                //frm.ChartValue = values;
                //frm.m_strChartTitle = strTitle;
                //frm.m_strXtitle = this.comboType.Text;
                //frm.m_strYtitle = strColumnTitle;
                //frm.BindData();
                //frm.ShowDialog();
            }
            catch (Exception ex)
            {

            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {

            try
            {
                List<string> lstFields = new List<string>();
                List<string> lstNames = new List<string>();
                for (int i = 0; i < gridRes.ColumnCount; i++)
                {
                    lstFields.Add(gridRes.Columns[i].HeaderText);
                    lstNames.Add(gridRes.Columns[i].Name);
                }

                frmFields frm = new frmFields();
                frm.lstSourceFields = lstFields;
                frm.lstSourceNames = lstNames;
                frm.ShowDialog();
                if (frm.DialogResult != DialogResult.OK) return;

                lstFields = frm.lstTagFields;
                if (this.WriteLog)
                {
                    Plugin.LogTable.Writelog("����ͳ�ƽ����");
                }
                ModCommon.ExportDataGridview(this.gridRes,lstFields, "ռ�����"+DateTime.Now.ToString("yyyyMMdd"));
            }
            catch (Exception  ex)
            {
                 //ZQ  20110827  modify
                MessageBox.Show("�밲װMicrosoft Office ��� ", "��ʾ��", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //end
            }
        }

        private void btnGetSQL_Click(object sender, EventArgs e)
        {
            if (m_pCurFeaCls == null) return;

            SQLfrm vfrm = new SQLfrm(m_pCurFeaCls);
            /// ZQ  20111116 add
            vfrm.pGeometry = m_pGeometry;
            vfrm.SQL = this.txtSQL.Text;
            if (vfrm.ShowDialog() != DialogResult.OK) return;
            
            this.txtSQL.Text = vfrm.SQL;
            vfrm = null;
        }
        //�ı�ͼ��ѡ��ʽ ����֮ǰ���� xisheng 20111119
        private void comboLayers_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            //m_pCurFeaCls = null;

            //if (this.comboLayers.SelectedIndex < 0)
            //    return;
            //IFeatureLayer pFeaLyr = m_lstFeaLyrs[this.comboLayers.SelectedIndex];
            //if (pFeaLyr.FeatureClass == null)
            //    return;

            //m_strAreaName = "";
            //m_pCurFeaCls = pFeaLyr.FeatureClass;
            //InitJsFld(); //����������

        }

        /// <summary>
        /// ��õ�ǰ��ͼ�ϵ�ѡ��Ҫ��
        /// </summary>
        /// <param name="pMap"></param>
        /// <returns></returns>
        private IGeometry ConstructUnion(IMap pMap)
        {
            ISelection pSelection = pMap.FeatureSelection;

            //
            IEnumFeature pEnumFeature = pSelection as IEnumFeature;
            IFeature pFeature = pEnumFeature.Next();

            //���ǵ��е� �� ������ ����ֻѡ��һ��Ҫ��
            if (pFeature == null) return null;

            IGeometry pGeometry = pFeature.ShapeCopy;
            ITopologicalOperator2 pTopo2 = pGeometry as ITopologicalOperator2;
            pTopo2.IsKnownSimple_2 = false;
            pTopo2.Simplify();
            return pTopo2 as IGeometry;
        }

        private IGeometry GetBufferGeometry(IGeometry pGeo, double dBufferSize)
        {
            if (pGeo == null) return null;

            //UnitConverter punitConverter = new UnitConverterClass();
            //if (m_pMap.MapUnits == esriUnits.esriDecimalDegrees)
            //{
            //    dBufferSize = punitConverter.ConvertUnits(dBufferSize, esriUnits.esriMeters, esriUnits.esriDecimalDegrees);
            //}//ת������Ǿ�γ�ȵĵ�ͼ xisheng 20110731
            //==���ֱ�Ӳ�������ԭpGeometry�ᱻ�ı�
            //���п�¡����ȡtopoʵ��
            IClone pClone = (IClone)pGeo;
            ITopologicalOperator pTopo;
            if (pGeo.GeometryType != esriGeometryType.esriGeometryBag)
            {
                pTopo = pClone.Clone() as ITopologicalOperator;

                //topo�ǿ�����л��壬��ȡ������ m_pBufferGeometry
                if (pTopo != null) pGeo = pTopo.Buffer(dBufferSize);
            }
            else
            {
                IGeometryCollection pGeometryBag = (IGeometryCollection)pClone.Clone();
                pTopo = (ITopologicalOperator)pGeometryBag.get_Geometry(0);
                IGeometry pUnionGeom = pTopo.Buffer(dBufferSize);
                for (int i = 1; i < pGeometryBag.GeometryCount; i++)
                {
                    pTopo = (ITopologicalOperator)pGeometryBag.get_Geometry(i);
                    IGeometry pTempGeom = pTopo.Buffer(dBufferSize);
                    pTopo = (ITopologicalOperator)pUnionGeom;
                    pUnionGeom = pTopo.Union(pTempGeom);
                }
                pGeo = pUnionGeom;
            }
            // m_pBufferGeometryΪ�գ�ֱ�ӷ���
            if (pGeo == null) return null;

            //�� m_pBufferGeometry��topo���м��ٻ��
            pTopo = pGeo as ITopologicalOperator;
            if (pTopo != null) pTopo.Simplify();
            return pGeo;
        }

        private void sliderBuffer_ValueChanged(object sender, EventArgs e)
        {
            //double dblSize = GetBuffSize();
            ////���С������
            //dblSize = dblSize - Math.Floor(dblSize);
            //dblSize = dblSize + this.sliderBuffer.Value;
            if (ChangeText)
            {
                this.dblBuffLen.Text = sliderBuffer.Value.ToString();

            }
            else
            {
                ChangeText = true;
            }
        }


        private double dblbufSize = 0;
        private bool ChangeText = true;
        private string  strlastValue = "";
        private int iValueLast = 0;
        private void dblBuffLen_TextChanged(object sender, EventArgs e)
        {
            ChangeText = false;
            try
            {
                
                int iValue = Convert.ToInt32(dblBuffLen.Text) > 1000 ? 1000 : Convert.ToInt32(dblBuffLen.Text);
                iValueLast = iValue;//��¼�ϴ�  20110802 xisheng

                if (iValue < 0)
                {
                    dblBuffLen.Text = Convert.ToString(1);
                    iValue = 1;
                }
                sliderBuffer.Value = iValue;
               // DrawGeometry();
               IGeometry pGeometry = GetBufferGeometry(m_pGeometry_start, GetBuffSize());
               m_pMap.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewForeground, null, null);
                drawgeometryXOR(pGeometry);
          
            }
            catch
            {
                if(dblBuffLen.Text.Trim()=="")
                    strlastValue="0";
               // (ee.Message,"��ʾ");
                dblBuffLen.Text = Convert.ToString(iValueLast);//20110802 xisheng
            }
           
        }

        private double GetBuffSize()
        {
            double dBufferSize = 0;

            //double.TryParse(this.dblBuffLen.Text, out dblSize);
            dBufferSize = Convert.ToDouble(dblBuffLen.Text);/*/ 10*/ ; //20110802 xisheng
            dBufferSize = dBufferSize < 1 ? 1 : dBufferSize;//���û���ֵ�������ó�0 xisheng 20110722
            if (dBufferSize == 0.0) dBufferSize = 0.001;
            //ת������Ǿ�γ�ȵĵ�ͼ xisheng 20110731
            UnitConverter punitConverter = new UnitConverterClass();
            if (m_pMap.MapUnits == esriUnits.esriDecimalDegrees)
            {
                dBufferSize = punitConverter.ConvertUnits(dBufferSize, esriUnits.esriMeters, esriUnits.esriDecimalDegrees);
            }//ת������Ǿ�γ�ȵĵ�ͼ xisheng 20110731
            return dBufferSize;
        }

        //public void DrawGeometry()
        //{
        //    //����ֻ��������
        //    string strTemp = "";
        //    strTemp = this.dblBuffLen.Text.Trim();
        //    if (strTemp == "")
        //    {
        //        this.dblBuffLen.Text = "0";
        //    }
        //    else
        //    {
        //        if (strTemp.Length != 1 && strTemp.Substring(strTemp.Length - 1) == ".") return;
        //    }


        //    double dblTemp = 0;
        //    if (!double.TryParse(this.dblBuffLen.Text, out dblTemp))
        //    {
        //        this.dblBuffLen.Text = dblbufSize.ToString();
        //        return;
        //    }
        //    dblbufSize = dblTemp;


        //    //����
        //    //this.sliderBuffer.Value = Convert.ToInt32(dblBuffLen.Text);
        //    //��û��巶Χ
        //    m_pGeometry = GetBufferGeometry(m_pGeometry_start, GetBuffSize());

        //    //��element
        //    //��ɾ��
        //    if (m_pMap == null) return;
        //    IGraphicsContainer pMapGraphics = (IGraphicsContainer)m_pMap.Map;
        //    if (m_pElement != null)
        //    {
        //        pMapGraphics.DeleteElement(m_pElement);
        //        //m_pMap.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewBackground, null, null);
        //        m_pElement = null;
        //    }
        //    m_pElement = SysCommon.Gis.ModGisPub.DoDrawGeometry(m_pMap, m_pGeometry, 255, 170, 0, true);
        //    m_pMap.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, m_pElement, null);
        //    //pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        //}

      

        /// <summary>
        /// ����pGeometry��ͼ��
        /// </summary>
        
        private void drawgeometryXOR(IGeometry pPolygon)
        {
            if (this.boolok)//�������رջ���ȡ�� �Ͳ����� xisheng 2011.06.28
            {
                return;
            }

            //����
            //this.sliderBuffer.Value = Convert.ToInt32(dblBuffLen.Text);
            //��û��巶Χ
            
            IScreenDisplay pScreenDisplay = m_pMap.ActiveView.ScreenDisplay;
            ISimpleFillSymbol pFillSymbol = new SimpleFillSymbolClass();
            ISimpleLineSymbol pLineSymbol = new SimpleLineSymbolClass();

            try
            {
                //��ɫ����
                IRgbColor pRGBColor = new RgbColorClass();
                pRGBColor.UseWindowsDithering = false;
                ISymbol pSymbol = (ISymbol)pFillSymbol;
                pSymbol.ROP2 = esriRasterOpCode.esriROPNotXOrPen;

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
                    m_pGeometry = pPolygon;
                }
                //�����ѻ����Ķ����
                else
                {
                    if (m_pGeometry != null)
                    {
                        pScreenDisplay.DrawPolygon(m_pGeometry);
                    }
                }
                
                pScreenDisplay.FinishDrawing();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("���ƻ��巶Χ����:" + ex.Message, "��ʾ");
                pFillSymbol = null;
            }
        }
        //����ȥʱ����
        internal void UcAreaAfterDraw(IDisplay Display, esriViewDrawPhase phase)
        //private void m_pActiveViewEvents_AfterDraw(IDisplay Display, esriViewDrawPhase phase)
        {
            if (this.boolok == true)
            {
                m_pGeometry = null;
                m_pGeometry_start = null;
                return;
            }
            if (phase == esriViewDrawPhase.esriViewForeground) drawgeometryXOR(null);
        }

        private void UcArea_Load(object sender, EventArgs e)
        {
            if (m_pMap != null)
            {
                ///ZQ  2011 1129 modify 
                SysCommon.ScreenDraw.list.Add(UcAreaAfterDraw);
                //m_pActiveViewEvents = m_pMap.ActiveView as IActiveViewEvents_Event;
                //try
                //{

                //    m_pActiveViewEvents.AfterDraw += new IActiveViewEvents_AfterDrawEventHandler(m_pActiveViewEvents_AfterDraw);

                //}
                //catch
                //{
                //}
            }

        }

        //������������ѡ��ͼ��
        private void comboLayers_Click(object sender, EventArgs e)
        {
            this.advTreeLayers.Width = this.comboLayers.Width;
            this.advTreeLayers.Visible = true;
            this.advTreeLayers.Focus();
            //m_pCurFeaCls = null;
            //SysCommon.SelectLayerByTree frm = new SysCommon.SelectLayerByTree(Plugin.ModuleCommon.TmpWorkSpace,Plugin.ModuleCommon.ListUserdataPriID );
            //frm._LayerTreePath = System.Windows.Forms.Application.StartupPath + "\\..\\res\\xml\\ͳ��ͼ����.xml";
            //if (frm.ShowDialog() == DialogResult.OK)
            //{
            //    if (frm.m_NodeKey.Trim() != "")
            //    {
            //        //added by chulili 2012-11-13 ���һ��m_CurLayer�������洢��ǰͳ�Ƶ�ͼ��
            //        XmlDocument pXmldoc = new XmlDocument();
            //        pXmldoc.Load(frm._LayerTreePath);
            //        m_CurLayer = SysCommon.ModuleMap.GetLayerByNodeKey(Plugin.ModuleCommon.TmpWorkSpace, m_pMap.Map, frm.m_NodeKey, pXmldoc);
            //        pXmldoc = null;
            //        if (m_CurLayer != null)
            //        {
            //            try
            //            {
            //                m_pCurFeaCls = (m_CurLayer as IFeatureLayer).FeatureClass;
            //            }
            //            catch
            //            { }
            //        }
            //        //added by chulili 2012-11-13 end
            //        if (m_pCurFeaCls == null)
            //        {
            //            m_pCurFeaCls = SysCommon.ModSysSetting.GetFeatureClassByNodeKey(Plugin.ModuleCommon.TmpWorkSpace, frm._LayerTreePath, frm.m_NodeKey);
            //        }
            //    }

            //    if (m_pCurFeaCls != null)
            //        comboLayers.Text = frm.m_NodeText;
            //}
            //System.IO.File.Delete(frm._LayerTreePath);
            //frm = null;
            

        }

        private void advTreeLayers_NodeClick(object sender, DevComponents.AdvTree.TreeNodeMouseEventArgs e)
        {
            DealSelectNodeEX();
        }
        private void DealSelectNodeEX()
        {
            string LayerID = "";
            if (this.advTreeLayers.SelectedNode == null)
                return;
            if (advTreeLayers.SelectedNode.Tag.ToString() != "Layer")//����Ҷ�ӽڵ� ����
            {
                return;
            }

            LayerID = GetNodeKey(advTreeLayers.SelectedNode);
            if (string.IsNullOrEmpty(LayerID))
                return;
            this.advTreeLayers.Visible = false;
            XmlDocument pXmldoc = new XmlDocument();
            pXmldoc.Load(_LayerTreePath);
            m_CurLayer = SysCommon.ModuleMap.GetLayerByNodeKey(Plugin.ModuleCommon.TmpWorkSpace, m_pMap.Map, LayerID, pXmldoc);
            pXmldoc = null;
            if (m_CurLayer != null)
            {
                try
                {
                    m_pCurFeaCls = (m_CurLayer as IFeatureLayer).FeatureClass;
                }
                catch
                { }
            }
            //added by chulili 2012-11-13 end
            if (m_pCurFeaCls == null)
            {
                m_pCurFeaCls = SysCommon.ModSysSetting.GetFeatureClassByNodeKey(Plugin.ModuleCommon.TmpWorkSpace, _LayerTreePath , LayerID);
            }

            if (m_pCurFeaCls != null)
            {
                comboLayers.Text = advTreeLayers.SelectedNode.Text;
            }
        }
        //ͨ��NODE �õ�NODYKEY
        private string GetNodeKey(DevComponents.AdvTree.Node Node)
        {
            // labelErr.Text = "";
            XmlNode xmlnode = (XmlNode)Node.DataKey;
            XmlElement xmlelement = xmlnode as XmlElement;
            string strDataType = "";
            if (xmlelement.HasAttribute("DataType"))
            {
                strDataType = xmlnode.Attributes["DataType"].Value;
            }
            if (strDataType == "RD" || strDataType == "RC")//��Ӱ������ ����
            {
                // labelErr.Text = "��ѡ��ʸ�����ݽ��в���!";
                return "";
            }
            if (xmlelement.HasAttribute("IsQuery"))
            {
                if (xmlelement["IsQuery"].Value == "False")
                {
                    // labelErr.Text = "��ͼ�㲻�ɲ�ѯ!";
                    return "";
                }
            }
            if (xmlelement.HasAttribute("NodeKey"))
            {
                return xmlelement.GetAttribute("NodeKey");

            }
            return "";

        }
        public void InitLayersTree()
        {
            ModSysSetting.CopyLayerTreeXmlFromDataBase(Plugin.ModuleCommon.TmpWorkSpace, _LayerTreePath);
            if (File.Exists(_LayerTreePath))
            {

                XmlDocument LayerTreeXmldoc = new XmlDocument();

                LayerTreeXmldoc.Load(_LayerTreePath);
                advTreeLayers.Nodes.Clear();

                //��ȡXml�ĸ��ڵ㲢��Ϊ���ڵ�ӵ�UltraTree��
                XmlNode xmlnodeRoot = LayerTreeXmldoc.DocumentElement;
                XmlElement xmlelementRoot = xmlnodeRoot as XmlElement;

                xmlelementRoot.SetAttribute("NodeKey", "Root");
                string sNodeText = xmlelementRoot.GetAttribute("NodeText");

                //�������趨���ĸ��ڵ�
                DevComponents.AdvTree.Node treenodeRoot = new DevComponents.AdvTree.Node();
                treenodeRoot.Name = "Root";
                treenodeRoot.Text = sNodeText;

                treenodeRoot.Tag = "Root";
                treenodeRoot.DataKey = xmlelementRoot;
                treenodeRoot.Expanded = true;
                this.advTreeLayers.Nodes.Add(treenodeRoot);

                treenodeRoot.Image = this.ImageList.Images["Root"];
                InitLayerTreeByXmlNode(treenodeRoot, xmlnodeRoot);
                LayerTreeXmldoc = null;
            }
        }
        //���������ļ���ʾͼ����
        private void InitLayerTreeByXmlNode(DevComponents.AdvTree.Node treenode, XmlNode xmlnode)
        {

            for (int iChildIndex = 0; iChildIndex < xmlnode.ChildNodes.Count; iChildIndex++)
            {
                XmlElement xmlElementChild = xmlnode.ChildNodes[iChildIndex] as XmlElement;
                if (xmlElementChild == null)
                {
                    continue;
                }
                else if (xmlElementChild.Name == "ConfigInfo")
                {
                    continue;
                }
                //��Xml�ӽڵ��"NodeKey"��"NodeText"�������������ӽڵ�
                string sNodeKey = xmlElementChild.GetAttribute("NodeKey");
                if (Plugin.ModuleCommon.ListUserdataPriID != null)
                {
                    if (!Plugin.ModuleCommon.ListUserdataPriID.Contains(sNodeKey))
                    {
                        continue;
                    }
                }
                string sNodeText = xmlElementChild.GetAttribute("NodeText");

                DevComponents.AdvTree.Node treenodeChild = new DevComponents.AdvTree.Node();
                treenodeChild.Name = sNodeKey;
                treenodeChild.Text = sNodeText;

                treenodeChild.DataKey = xmlElementChild;
                treenodeChild.Tag = xmlElementChild.Name;


                treenode.Nodes.Add(treenodeChild);

                //�ݹ�
                if (xmlElementChild.Name != "Layer")
                {
                    InitLayerTreeByXmlNode(treenodeChild, xmlElementChild as XmlNode);
                }

                InitializeNodeImage(treenodeChild);
            }

        }
        /// <summary>
        /// ͨ������ڵ��tag��ѡ���Ӧ��ͼ��        
        /// </summary>
        /// <param name="treenode"></param>
        private void InitializeNodeImage(DevComponents.AdvTree.Node treenode)
        {
            switch (treenode.Tag.ToString())
            {
                case "Root":
                    treenode.Image = this.ImageList.Images["Root"];
                    treenode.CheckBoxVisible = false;
                    break;
                case "SDE":
                    treenode.Image = this.ImageList.Images["SDE"];
                    break;
                case "PDB":
                    treenode.Image = this.ImageList.Images["PDB"];
                    break;
                case "FD":
                    treenode.Image = this.ImageList.Images["FD"];
                    break;
                case "FC":
                    treenode.Image = this.ImageList.Images["FC"];
                    break;
                case "TA":
                    treenode.Image = this.ImageList.Images["TA"];
                    break;
                case "DIR":
                    treenode.Image = this.ImageList.Images["DIR"];
                    //treenode.CheckBoxVisible = false;
                    break;
                case "DataDIR":
                    treenode.Image = this.ImageList.Images["DataDIRHalfOpen"];
                    break;
                case "DataDIR&AllOpened":
                    treenode.Image = this.ImageList.Images["DataDIROpen"];
                    break;
                case "DataDIR&Closed":
                    treenode.Image = this.ImageList.Images["DataDIRClosed"];
                    break;
                case "DataDIR&HalfOpened":
                    treenode.Image = this.ImageList.Images["DataDIRHalfOpen"];
                    break;
                case "Layer":
                    XmlNode xmlnodeChild = (XmlNode)treenode.DataKey;
                    if (xmlnodeChild != null && xmlnodeChild.Attributes["FeatureType"] != null)
                    {
                        string strFeatureType = xmlnodeChild.Attributes["FeatureType"].Value;

                        switch (strFeatureType)
                        {
                            case "esriGeometryPoint":
                                treenode.Image = this.ImageList.Images["_point"];
                                break;
                            case "esriGeometryPolyline":
                                treenode.Image = this.ImageList.Images["_line"];
                                break;
                            case "esriGeometryPolygon":
                                treenode.Image = this.ImageList.Images["_polygon"];
                                break;
                            case "esriFTAnnotation":
                                treenode.Image = this.ImageList.Images["_annotation"];
                                break;
                            case "esriFTDimension":
                                treenode.Image = this.ImageList.Images["_Dimension"];
                                break;
                            case "esriGeometryMultiPatch":
                                treenode.Image = this.ImageList.Images["_MultiPatch"];
                                break;
                            default:
                                treenode.Image = this.ImageList.Images["Layer"];
                                break;
                        }
                    }
                    else
                    {
                        treenode.Image = this.ImageList.Images["Layer"];
                    }
                    break;
                case "RC":
                    treenode.Image = this.ImageList.Images["RC"];
                    break;
                case "RD":
                    treenode.Image = this.ImageList.Images["RD"];
                    break;
                case "SubType":
                    treenode.Image = this.ImageList.Images["SubType"];
                    break;
                default:
                    break;
            }//end switch
        }

        private void advTreeLayers_Leave(object sender, EventArgs e)
        {
            HideLayerTree();
        }

        private void groupPanel1_Click(object sender, EventArgs e)
        {
            HideLayerTree();
        }

        private void labelX1_Click(object sender, EventArgs e)
        {
            HideLayerTree();
        }

        private void labelX3_Click(object sender, EventArgs e)
        {
            HideLayerTree();
        }

        private void labelX4_Click(object sender, EventArgs e)
        {
            HideLayerTree();
        }
        public void HideLayerTree()
        {
            if (this.advTreeLayers.Visible)
            {
                this.advTreeLayers.Visible = false;
            }
        }
        private void UcArea_Click(object sender, EventArgs e)
        {
            HideLayerTree();
        }        
    }
}
