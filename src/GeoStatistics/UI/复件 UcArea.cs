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

namespace GeoStatistics
{
    public partial class UcArea : UserControl
    {
        string m_strAreaName = "";

        public UcArea()
        {
            InitializeComponent();
            InitGridView(this.gridRes);
            m_lstFeaLyrs = new List<IFeatureLayer>();

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

        private List<IFeatureLayer> m_lstFeaLyrs;
        //
        private IMapControlDefault m_pMap;
        public IMapControlDefault CurMap
        {
            set { m_pMap = value; }
        }

        private ESRI.ArcGIS.Geometry.IGeometry m_pGeometry;
        public ESRI.ArcGIS.Geometry.IGeometry CurGeometry
        {
            set { m_pGeometry = value; }
        }

        public DevComponents.DotNetBar.Controls.DataGridViewX CurGrid
        {
            get { return this.gridRes; }
        }

        public void InitLayers()
        {
            if (m_pMap == null) return;
            m_lstFeaLyrs.Clear();
            this.dblBuffLen.Text = "0";

            List<ILayer> lstLyrs = null;
            SysCommon.Gis.ModGisPub.GetLayersByMap(m_pMap.Map, ref lstLyrs);

            for (int i = 0; i < lstLyrs.Count; i++)
            {
                ILayer pLyr = lstLyrs[i];
                if (!(pLyr is IGeoFeatureLayer)) continue;

                IFeatureLayer pFeaLyr = pLyr as IFeatureLayer;
                if (pFeaLyr.FeatureClass == null) continue;
                IFeatureClass pFeaCls = pFeaLyr.FeatureClass;

                if (pFeaCls == null) continue;
                if (pFeaCls.FeatureType != esriFeatureType.esriFTSimple) continue;
                //if (pFeaCls.ShapeType != ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon) continue;
                m_lstFeaLyrs.Add(pFeaLyr);

                this.comboLayers.Items.Add(pLyr.Name);
            }

            //���¼��ŵ������� ��Ķ��ˢ��
            this.comboLayers.SelectedIndexChanged += new EventHandler(comboLayers_SelectedIndexChanged);

            if (this.comboLayers.Items.Count > 0)
            {
                this.comboLayers.SelectedIndex = 0;
            }
        }

        void comboLayers_SelectedIndexChanged(object sender, EventArgs e)
        {
            //throw new Exception("The method or operation is not implemented.");
        }

        private void btnStatistics_Click(object sender, EventArgs e)
        {
            if (m_pCurFeaCls == null) return;

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
                DataTable dt = InitDataTalbe(pFeaCls);

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

                //��������ۺ�
                GetSum(dt, pFeaCls);

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

            DataTable dt = this.gridRes.DataSource as DataTable;

            for (int i = 0; i < this.gridRes.ColumnCount; i++)
            {
                this.comboType.Items.Add(this.gridRes.Columns[i].HeaderText);
            }
        }

        private void InitGridView(DevComponents.DotNetBar.Controls.DataGridViewX vGrid)
        {
            vGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            vGrid.ReadOnly = true;
            vGrid.AllowUserToAddRows = false;
            vGrid.AllowUserToDeleteRows = false;
        }

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

            //��Ӹ�����
            vRow["����"] = dt.Rows.Count.ToString();

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
            DataRow vRow = Dt.NewRow();

            for (int i = 0; i < Dt.Columns.Count; i++)
            {
                string strColumnName = Dt.Columns[i].ColumnName;
                int intIndex = pFea.Fields.FindField(strColumnName);
                if (intIndex < 0) continue;

                object obj = pFea.get_Value(intIndex);
                if (obj == null) continue;

                string strValue = obj.ToString();
                if (pFea.Fields.get_Field(intIndex).Type == esriFieldType.esriFieldTypeDouble)
                {
                    double dblTempa = 0;
                    double.TryParse(strValue, out dblTempa);
                    strValue = string.Format("{0:f2}", dblTempa);
                }

                vRow[strColumnName] = strValue;
            }
            
            //�������״Ҫ�� �����һ���ཻ���
            if (pFea.FeatureType == esriFeatureType.esriFTSimple && pFea.Shape!=null && pFea.Shape.GeometryType == esriGeometryType.esriGeometryPolygon && pGeo != null)
            {
                double dblIntersectArea=0;
                //���ཻ���
                if (!pFea.Shape.IsEmpty)
                {
                    ITopologicalOperator ptopo = pGeo as ITopologicalOperator;
                    IGeometry pIntersectGeometry = ptopo.Intersect(pFea.Shape, esriGeometryDimension.esriGeometry2Dimension);
                    if (pIntersectGeometry != null)
                    {
                        IArea pArea = pIntersectGeometry as IArea;
                        if (pArea != null) dblIntersectArea = pArea.Area;
                    }

                    vRow["�ཻ���"] = string.Format("{0:f2}", dblIntersectArea);
                }
            }

            Dt.Rows.Add(vRow);
        }

        private DataTable InitDataTalbe(IFeatureClass pFeaCls)
        {
            DataTable TempDt = new DataTable();

            for (int i = 0; i < pFeaCls.Fields.FieldCount; i++)
            {
                IField pField = pFeaCls.Fields.get_Field(i);
                if (pField.Type == esriFieldType.esriFieldTypeGeometry || pField.Type == esriFieldType.esriFieldTypeBlob) continue;

                //�����������ַ���������
                TempDt.Columns.Add(pField.Name,Type.GetType("System.String"));
            }

            TempDt.Columns.Add("����", Type.GetType("System.String"));

            //�������״Ҫ�� �����һ���ཻ���
            if (pFeaCls.FeatureType == esriFeatureType.esriFTSimple && pFeaCls.ShapeType == esriGeometryType.esriGeometryPolygon)
            {
                m_strAreaName = "�ཻ���";
                TempDt.Columns.Add(m_strAreaName, Type.GetType("System.String"));
            }

            return TempDt;
        }

        private void btnChart_Click(object sender, EventArgs e)
        {
            if (this.comboType.SelectedIndex < 0) return;
            string strStatictisValue = "";
            string strTitle = "";
            string strColumnTitle = "";

            try
            {
                if (m_strAreaName != "")
                {
                    strStatictisValue = m_strAreaName;
                    strTitle = "���ͳ��ͼ";
                    strColumnTitle = "���";
                }
                else
                {
                    strStatictisValue = "����";
                    strTitle = "����ͳ��ͼ";
                    strColumnTitle = "����";
                }

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
                for (int i = 0; i < dt.Rows.Count - 1; i++)
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


                frmStatisticsPic frm = new frmStatisticsPic();
                frm.ChartValue = values;
                frm.m_strChartTitle = strTitle;
                frm.m_strXtitle = this.comboType.Text;
                frm.m_strYtitle = strColumnTitle;
                frm.BindData();
                frm.ShowDialog();
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

                ModCommon.ExportDataGridview(this.gridRes,lstFields, "ռ�����"+DateTime.Now.ToString("yyyyMMdd"));
            }
            catch (Exception  ex)
            {

                MessageBox.Show("��ȷ���Ѿ���װMicrosoft Office Excel 2003" + Environment.NewLine + "������Ϣ��" + ex.Message, "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnGetSQL_Click(object sender, EventArgs e)
        {
            if (m_pCurFeaCls == null) return;

            SQLfrm vfrm = new SQLfrm(m_pCurFeaCls);
            vfrm.SQL = this.txtSQL.Text;
            if (vfrm.ShowDialog() != DialogResult.OK) return;

            this.txtSQL.Text = vfrm.SQL;
            vfrm = null;
        }

        private void comboLayers_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            m_pCurFeaCls = null;

            if (this.comboLayers.SelectedIndex < 0) return;
            IFeatureLayer pFeaLyr = m_lstFeaLyrs[this.comboLayers.SelectedIndex];
            if (pFeaLyr.FeatureClass == null) return;

            m_strAreaName = "";
            m_pCurFeaCls = pFeaLyr.FeatureClass;

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

        private IGeometry GetBufferGeometry(IGeometry pGeo, double dblBufLen)
        {
            if (pGeo == null) return null;

            ITopologicalOperator pTopo = pGeo as ITopologicalOperator;
            IGeometry pTempGeo=pTopo.Buffer(GetBuffSize());

            return pTempGeo;
        }

        private void sliderBuffer_ValueChanged(object sender, EventArgs e)
        {
            double dblSize = GetBuffSize();
            //���С������
            dblSize = dblSize - Math.Floor(dblSize);
            dblSize = dblSize + this.sliderBuffer.Value;

            this.dblBuffLen.Text = dblSize.ToString() ;
        }

        private IElement m_pElement = null;
        public IElement CurrentElement
        {
            get { return m_pElement; }
        }

        private double dblbufSize = 0;
        private void dblBuffLen_TextChanged(object sender, EventArgs e)
        {
    
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

        private void dblBuffLen_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //����ֻ��������
                string strTemp = "";
                strTemp = this.dblBuffLen.Text.Trim();
                if (strTemp == "")
                {
                    this.dblBuffLen.Text = "0";
                }
                else
                {
                    if (strTemp.Length != 1 && strTemp.Substring(strTemp.Length - 1) == ".") return;
                }


                double dblTemp = 0;
                if (!double.TryParse(this.dblBuffLen.Text, out dblTemp))
                {
                    this.dblBuffLen.Text = dblbufSize.ToString();
                    return;
                }
                dblbufSize = dblTemp;


                //����
                this.sliderBuffer.Value =Convert.ToInt32(dblBuffLen.Text);
                //��û��巶Χ
                m_pGeometry = GetBufferGeometry(m_pGeometry, GetBuffSize());

                //��element
                //��ɾ��
                if (m_pMap == null) return;
                IGraphicsContainer pMapGraphics = (IGraphicsContainer)m_pMap.Map;
                if (m_pElement != null) pMapGraphics.DeleteElement(m_pElement);

                m_pElement = SysCommon.Gis.ModGisPub.DoDrawGeometry(m_pMap, m_pGeometry, 128, 128, 128, false);
                m_pMap.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
            }
        }

    }
}
