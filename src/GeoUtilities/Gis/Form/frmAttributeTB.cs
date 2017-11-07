using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using DevComponents.DotNetBar;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Controls;
using SysCommon.Error;

namespace GeoUtilities
{
    public partial class frmAttributeTB : DevComponents.DotNetBar.Office2007Form
    {
        public frmAttributeTB()
        {
            InitializeComponent();
        }
        //��¼ҳ����
        private int m_iPageCount = 0;
        //��¼��ǰ��ҳ��
        private int m_iPage = 0;
        //��õ�ǰ��Table
        private DataTable m_SourceDataTable = null;
        private IMapControlDefault vMapControl = null;
        public IMapControlDefault MapControl
        {
            set { vMapControl = value; }
        }

        private IFeatureClass vFeatureClass = null;
        public IFeatureClass FeatureClass
        {
            set { vFeatureClass = value; }
        }

        private IFields vFields = null;
        public IFields Fields
        {
            set { vFields = value; }
        }
        
        //ȫѡ
        private void btnDisplayAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listViewFields.Items.Count; i++)
            {
                if (listViewFields.Items[i].Checked == false)
                {
                    listViewFields.Items[i].Checked = true;
                }
            }
        }
        //���ѡ��
        private void btnClearChecked_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listViewFields.Items.Count; i++)
            {
                if (listViewFields.Items[i].Checked == true )
                {
                    listViewFields.Items[i].Checked = false;
                }
            }
        }

        //ѡ���ֶ���ʾ,�����е���ʾ
        private void btnDisplaySelect_Click(object sender, EventArgs e)
        {
            try
            {
                dataGridViewX1.Visible = false;
                for (int k = 0; k < dataGridViewX1.Columns.Count; k++)
                {
                    this.dataGridViewX1.Columns[k].Visible = true;
                }
                ArrayList NonDisplayFields = new ArrayList();
                for (int i = 0; i < listViewFields.Items.Count; i++)
                {
                    try
                    {
                        if (listViewFields.Items[i].Checked == false)
                        {
                            NonDisplayFields.Add(listViewFields.Items[i].Text);
                        }
                    }
                    catch { }
                }
                this.dataGridViewX1.ReadOnly = true;
                if (NonDisplayFields.Count > 0)
                {
                    string FieldName = "";
                    for (int j = 0; j < NonDisplayFields.Count; j++)
                    {
                        try
                        {
                            FieldName = NonDisplayFields[j].ToString();
                            for (int n= 0; n < dataGridViewX1.Columns.Count; n++)
                            {
                                if (dataGridViewX1.Columns[n].HeaderText.ToString() == FieldName)
                                {
                                    this.dataGridViewX1.Columns[n].Visible = false;
                                    break;
                                }
                            }
                        }
                        catch { }
                    }
                }
                dataGridViewX1.Visible = true;
                this.dataGridViewX1.ReadOnly = true;
            }
            catch{}
        }
   

        //��˫��ĳ��ʱ��λ�����Ҫ��
        private void dataGridViewX1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow pDataGridViewRow = null;
            DataGridViewCell pDataGridViewCell = null;
            if (e.RowIndex == -1) return;
            pDataGridViewRow = dataGridViewX1.Rows[e.RowIndex];
            int IntObjectID = 0;
            string fileldName = vFeatureClass.OIDFieldName;
            pDataGridViewCell = pDataGridViewRow.Cells[fileldName];
            try
            {
                IntObjectID = Convert.ToInt32(pDataGridViewCell.Value);
            }
            catch
            { }

            FlashFeatrue(vFeatureClass, IntObjectID);
        }

        private void frmAttributeTB_Load(object sender, EventArgs e)
        {
            FillFields();
           // dataGridViewX1.Visible = false;
            TimerShow.Enabled = true;
            labelX1.Visible = false;
            
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void TimerShow_Tick(object sender, EventArgs e)
        {
            TimerShow.Enabled = false;
            btnDisplayAll.Enabled = false;
            btnDisplaySelect.Enabled = false;
            btnClearChecked.Enabled = false;

            //Ĭ���ǵ�ǰ��ͼ��Χ��
            //InitialControl(null);
            this.comboExtent.SelectedIndex = 1;

            btnDisplayAll.Enabled = true;
            btnDisplaySelect.Enabled = true;
            btnClearChecked.Enabled = true ;
        }

        #region   ��ʾ����   ����һ  �ٶȿ죬�����޷�ʵʱ��ʾ������
        /// <summary>
        /// DataTable��ҳ
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <param name="PageIndex">ҳ����,ע�⣺��1��ʼ</param>
        /// <param name="PageSize">ÿҳ��С</param>
        /// <returns>�ֺ�ҳ��DataTable����</returns>              
        public static DataTable GetPagedTable(DataTable dt, int PageIndex, int PageSize)
        {
            if (PageIndex == 0) { return dt; }
            DataTable newdt = dt.Copy();
            newdt.Clear();
            int rowbegin = (PageIndex - 1) * PageSize;
            int rowend = PageIndex * PageSize;

            if (rowbegin >= dt.Rows.Count)
            { return newdt; }

            if (rowend > dt.Rows.Count)
            { rowend = dt.Rows.Count; }
            for (int i = rowbegin; i <= rowend - 1; i++)
            {
                DataRow newdr = newdt.NewRow();
                DataRow dr = dt.Rows[i];
                foreach (DataColumn column in dt.Columns)
                {
                    newdr[column.ColumnName] = dr[column.ColumnName];
                }
                newdt.Rows.Add(newdr);
            }
            return newdt;
        }

        /// <summary>
        /// ���ط�ҳ��ҳ��
        /// </summary>
        /// <param name="count">������</param>
        /// <param name="pageye">ÿҳ��ʾ������</param>
        /// <returns>��� ��βΪ0���򷵻�1</returns>
        public static int PageCount(int count, int pageye)
        {
            int page = 0;
            int sesepage = pageye;
            if (count % sesepage == 0) { page = count / sesepage; }
            else { page = (count / sesepage) + 1; }
            if (page == 0) { page += 1; }
            return page;
        }
        private void SetBttEnable(int iPage,int iPageCount)
        {
            if(iPage==0){return;}
            txtPage.Text = m_iPage.ToString();
            if (iPage == 1)
            {
                bttFirst.Enabled = false;
                bttAfter.Enabled = false;
            }
            if (iPage == 1 && iPageCount > iPage)
            {
                bttNext.Enabled = true;
                bttLast.Enabled = true;
            }
            if (iPage > 1)
            {
                bttFirst.Enabled = true;
                bttAfter.Enabled = true;
                bttNext.Enabled = true;
                bttLast.Enabled = true;
            }
            if (iPage > 1 && iPage == iPageCount)
            {
                bttFirst.Enabled = true;
                bttAfter.Enabled = true;
                bttNext.Enabled = false;
                bttLast.Enabled = false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="DisplayAll">�ж��Ƿ���ȫ���ֶ���ʾ</param>
        /// <param name="NonDisplayFields">����ʾ���ֶε����ƽ��</param>
        private void InitialControl(IGeometry pGeometryFilter,string strType)
        {
            try
            {
                progressStep.Maximum = 100;
                progressStep.Minimum = 0;
                DataTable SourceDataTable = null;
                this.dataGridViewX1.DataSource = SourceDataTable;
                //SysCommon.CProgress vProgress = new SysCommon.CProgress();
                //vProgress.ShowDescription = true;
                //vProgress.ShowProgressNumber = true;
                //vProgress.TopMost = true;
                //vProgress.EnableCancel = true;
                //vProgress.EnableUserCancel(true);
                try
                {

                    progressStep.Value = 10;
                    //m_SourceDataTable = GetTable(vFields, vFeatureClass, pGeometryFilter, strType, vProgress);
                    m_SourceDataTable = SysCommon.Gis.ModGisPub.GetFeaturClassTable(vFeatureClass);
                    progressStep.Value = 40;
                    this.lblTips.Text = "���ƣ�" + m_SourceDataTable.Rows.Count + "��";
                    this.lblTips.Refresh();
                    if (m_SourceDataTable == null) { return; }
                    m_iPageCount = PageCount(m_SourceDataTable.Rows.Count, 1000);
                    m_iPage = 1;
                    SourceDataTable = GetPagedTable(m_SourceDataTable, m_iPage, 1000);
                    SetBttEnable(m_iPage, m_iPageCount);
                    labRecord.Text = SourceDataTable.Rows.Count.ToString();
                    progressStep.Value = 50;
                }
                catch (Exception ex)
                {
                    //vProgress.Close();
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", ex.Message);
                    progressStep.Value = 0;
                    return;
                }
                //vProgress.SetProgress("����װ������......");
                //vProgress.ProgresssValue = 0;
                //vProgress.MaxValue = 10;
                this.Cursor = Cursors.WaitCursor;//����괦�ڵȴ�״̬ xisheng 2011.07.11
                this.dataGridViewX1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
                this.dataGridViewX1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                this.dataGridViewX1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                progressStep.Value = 60;
                //vProgress.SetProgress(1);

                //if (!vProgress.UserAskCancel)
                //{
                this.dataGridViewX1.DataSource = SourceDataTable;
                progressStep.Value = 80;
                //}
                //vProgress.SetProgress(6);
                //if (!vProgress.UserAskCancel)
                //{
                //    //this.dataGridViewX1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                //}
                //vProgress.SetProgress(8);
                //�޸��б���Ϊ����
                ///zq 2011-1220 ��ʼ���ֵ��
                if (SysCommon.ModField._DicFieldName.Keys.Count ==0)
                {
                    SysCommon.ModField.InitNameDic(Plugin.ModuleCommon.TmpWorkSpace, SysCommon.ModField._DicFieldName,"���Զ��ձ�");
                }
                IField pField = null;
                for (int i = 0; i < vFields.FieldCount; i++)
                {
                    //if (vProgress.UserAskCancel)
                    //{
                    //    break;
                    //}
                    pField = vFields.get_Field(i);

                    try
                    {
                        ///��Ӣ�Ķ���
                        this.dataGridViewX1.Columns[pField.Name.ToString()].HeaderText = SysCommon.ModField.GetChineseNameOfField(pField.Name.ToString());
                    }
                    catch { }
                }
                progressStep.Value = 95;
                //vProgress.SetProgress(10);
                this.Cursor = Cursors.Default;//�����ָ�Ĭ�ϡ�xisheng 2011.07.11
                //vProgress.Close();
                //progressBarX1.Visible = false;
                this.dataGridViewX1.ReadOnly = true;
                progressStep.Value = 0;
            }
            catch { progressStep.Value = 0; }
        }
        //��ȡҪ�������ݱ�
        private DataTable GetTable(IFields pFields, IFeatureClass pFeatureClass, IGeometry pFilterGeometry, string strType, SysCommon.CProgress vProgress)
        {
            
            Application.DoEvents();
            vProgress.ShowProgress();
            vProgress.SetProgress("���ڲ�������......");

            vProgress.MaxValue = 10;
            vProgress.SetProgress(1);
            DataTable pDataTable = new DataTable();
            DataColumn pDataColumn = null;
            for (int i = 0; i < pFields.FieldCount; i++)
            {
                pDataColumn = new DataColumn(pFields.get_Field(i).Name);
                pDataTable.Columns.Add(pDataColumn);
                pDataColumn = null;
            }

            ISpatialFilter pSpatialFilter = null;
            if (pFilterGeometry != null)
            {
                pSpatialFilter = new SpatialFilterClass();
                pSpatialFilter.Geometry = pFilterGeometry;
                pSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
            }

            IFeatureCursor pFeatureCursor = null;
            IFeature pFeature = null;
            int FeatureCount = 0;
            ///ZQ 201118 modify
            if (strType != "ȫͼ��Χ")
            {
            FeatureCount = pFeatureClass.FeatureCount(pSpatialFilter);
            Application.DoEvents();
            }
            else
            {
                FeatureCount = SysCommon.Gis.ModGisPub.GetFeatureCount(pFeatureClass,null);
            }
           
            vProgress.SetProgress(7);
            //˽�к��� �����ֱ��д��tips����
            this.lblTips.Text = "���ƣ�" + FeatureCount + "��";
            this.lblTips.Refresh();
            //progressBarX1.Minimum = 0;
            //progressBarX1.Maximum = FeatureCount;
            //progressBarX1.Visible = true;
            int k = 0;
            if (strType != "ȫͼ��Χ")
            {
                pFeatureCursor = pFeatureClass.Search(pSpatialFilter, false);
            }
            else
            {
                pFeatureCursor = pFeatureClass.Search(null, false);
            }
            vProgress.SetProgress(10);
            vProgress.MaxValue = FeatureCount;
            vProgress.SetProgress("���ڼ�������......");
            pFeature = pFeatureCursor.NextFeature();
            DataRow pDataRow = null;
            int iProgress = 1;
            while (pFeature != null)
            {
                vProgress.SetProgress(iProgress);
                if (vProgress.UserAskCancel)
                {
                    break;
                }
                pDataRow = pDataTable.NewRow();
                for (int j = 0; j < pFields.FieldCount; j++)
                {
                    if (vProgress.UserAskCancel)
                    {
                        break;
                    }
                    if (pFields.FindField(pFeatureClass.ShapeFieldName) == j)
                    {
                        pDataRow[j] = pFeatureClass.ShapeType;
                    }
                    else
                    {
                        pDataRow[j] = pFeature.get_Value(j);
                    }
                }
                iProgress++;
                //k = k + 1;
                //progressBarX1.Value = k;
                ///////////Ч�ʣ�ֻ��ʾǰ1000�����Ժ�������ӷ�ҳ////
                //if (k > 1000)
                //{
                //   progressBarX1.Value = FeatureCount-1;
                //    break;
                //}
                ///////////**************************************////
                pDataTable.Rows.Add(pDataRow);
                pFeature = pFeatureCursor.NextFeature();
            }
            Application.DoEvents();
            System.Runtime.InteropServices.Marshal.ReleaseComObject(pFeatureCursor);
            return pDataTable;
        }
        #endregion

        #region  ��ʾ����    ������   ������ʾ�������������ٶȱȽ���
        private void vInitialControl()
        {
            this.dataGridViewX1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridViewX1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridViewX1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dataGridViewX1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            SetValue(vFields, vFeatureClass, this.dataGridViewX1);

            //�޸��б���Ϊ����
            IField pField = null;
            for (int i = 0; i < vFields.FieldCount; i++)
            {
                pField = vFields.get_Field(i);
                this.dataGridViewX1.Columns[i].HeaderText = pField.AliasName;
            }
            this.dataGridViewX1.ReadOnly = true;
        }
        private void SetValue(IFields pFields, IFeatureClass pFeatureClass, DevComponents.DotNetBar.Controls.DataGridViewX pDataGirdView)
        {
            DataGridViewTextBoxColumn pDataColumn = null;
            for (int i = 0; i < pFields.FieldCount; i++)
            {
                pDataColumn = new DataGridViewTextBoxColumn();
                pDataColumn.HeaderText = pFields.get_Field(i).AliasName;
                pDataColumn.Name = pFields.get_Field(i).Name;
                pDataGirdView.Columns.Add(pDataColumn);
                pDataColumn = null;
            }
            IFeatureCursor pFeatureCursor = null;
            IFeature pFeature = null;
            int FeatureCount = 0;
            FeatureCount = SysCommon.Gis.ModGisPub.GetFeatureCount(pFeatureClass,null);
            ///ZQ 2011 1128
            //progressBarX1.Minimum = 0;
            //progressBarX1.Maximum = FeatureCount;
            //progressBarX1.Visible = true;
            int k = 0;

            pFeatureCursor = pFeatureClass.Search(null, false);
            pFeature = pFeatureCursor.NextFeature();
            string StrFieldName = "";
            while (pFeature != null)
            {
                pDataGirdView.Rows.Add();
                for (int j = 0; j < pFields.FieldCount; j++)
                {
                    StrFieldName = pFields.get_Field(j).Name;
                    if (StrFieldName == pFeatureClass.ShapeFieldName)
                    {
                        pDataGirdView[StrFieldName, k].Value = pFeatureClass.ShapeType;
                    }
                    else
                    {
                        pDataGirdView[StrFieldName, k].Value = pFeature.get_Value(j);
                    }
                }
                //k = k + 1;
                //progressBarX1.Value = k;

                Application.DoEvents();
                pFeature = pFeatureCursor.NextFeature();
            }
        }
        #endregion

        #region   ��˸Ҫ��
        private void FlashFeatrue(IFeatureClass pFeatrueClass, int FeatureOID)
        {
            IFeature pFeature = null;
            IEnvelope pEnvelop = null;
            IActiveView pActiveView = null;
            pActiveView = vMapControl.ActiveView;
            IQueryFilter pQueryFilter = new QueryFilterClass();
            pQueryFilter.WhereClause =pFeatrueClass.OIDFieldName  + "=" + FeatureOID;

            IFeatureCursor pFeatrueCursor = null;
            pFeatrueCursor = pFeatrueClass.Search(pQueryFilter, false);
            pFeature = pFeatrueCursor.NextFeature();
            if (pFeature == null) return;
            if (pFeatrueClass.ShapeType == esriGeometryType.esriGeometryPoint)
            {
                ITopologicalOperator pTopo = null;
                pTopo = pFeature.ShapeCopy as ITopologicalOperator;
                IGeometry TempGeometry = null;
                TempGeometry = pTopo.Buffer(0.5);
                pEnvelop = TempGeometry.Envelope;
                pEnvelop.Expand(20, 20, true);
            }
            else
            {
                pEnvelop = pFeature.Extent;
                pEnvelop.Expand(1.5, 1.5, true);
            }
            
            Application.DoEvents();
            pActiveView.Extent = pEnvelop;
            Application.DoEvents();
            pActiveView.Refresh();
            Application.DoEvents();
            vMapControl.FlashShape(pFeature.ShapeCopy, 3, 200, null);

        }
        #endregion        
        
        #region   ����ֶ��б�
        private void FillFields()
        {
            IField pField = null;
            string FieldName = "";
            int j = 0;
            ///zq 2011-1220 ��ʼ���ֵ��
            if (SysCommon.ModField._DicFieldName.Keys.Count == 0)
            {
                SysCommon.ModField.InitNameDic(Plugin.ModuleCommon.TmpWorkSpace, SysCommon.ModField._DicFieldName, "���Զ��ձ�");
            }
            for (int i = 0; i < vFields.FieldCount; i++)
            {
                  pField = vFields.get_Field(i);
                  FieldName = pField.Name;
                  if (FieldName== "SHAPE" || FieldName== "Shape" || pField.Type == esriFieldType.esriFieldTypeBlob)
                  {
                      continue;
                  }
                      listViewFields.Items.Add(SysCommon.ModField.GetChineseNameOfField(FieldName));
                      listViewFields.Items[j].Checked = false;
                      j = j + 1;
            }
        }
        #endregion

        private void comboBoxEx1_SelectedIndexChanged(object sender, EventArgs e)
        {
            IGeometry pGeometryFilter = null;
            if (vMapControl == null) return;

            string strSel = this.comboExtent.SelectedItem.ToString();
            string strType = "";
            switch (strSel)
            {
                case "ȫͼ��Χ":
                    pGeometryFilter = null;
                    strType = "ȫͼ��Χ";
                    break;
                case "��ǰ��ͼ��Χ":
                    pGeometryFilter = vMapControl.ActiveView.Extent;
                    strType = "��ǰ��ͼ��Χ";
                    break;
                case "ѡ��Ҫ�ط�Χ":
                    pGeometryFilter = ConstructUnion(vMapControl.Map);
                    strType = "ѡ��Ҫ�ط�Χ";
                    if (pGeometryFilter == null)
                    {
                        MessageBox.Show("��ǰ��ͼ��ѡ��Ҫ�ء�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    break;
            }

            //���¶�ȡ����
            InitialControl(pGeometryFilter,strType);
        }

        /// <summary>
        /// ��õ�ǰ��ͼ�ϵ�ѡ��Ҫ��
        /// ZQ 2011 1203 ����Ҫ���Զ�ȥ�� ֻ�ϲ��ߺ���Ҫ��
        /// </summary>
        /// <param name="pMap"></param>
        /// <returns></returns>
        private IGeometry ConstructUnion(IMap pMap)
        {

            IGeometry pGeometry = null;
            try
            {
                IGeometryBag pGeometryBag = new GeometryBagClass();
                IGeometryCollection pGeometryCol = pGeometryBag as IGeometryCollection;
                object obj = System.Type.Missing;
                ISelection pSelection = pMap.FeatureSelection;
                IEnumFeature pEnumFeature = pSelection as IEnumFeature;
                IFeature pFeature = pEnumFeature.Next();
                while (pFeature != null)
                {
                    ///�ų���Ҫ��
                    if (pFeature.ShapeCopy.GeometryType != esriGeometryType.esriGeometryPoint && pFeature.ShapeCopy.GeometryType != esriGeometryType.esriGeometryMultipoint)
                    {
                        if (!pFeature.ShapeCopy.IsEmpty)
                        {
                            pGeometryCol.AddGeometry(pFeature.ShapeCopy, ref obj, ref obj);
                        }

                    }
                    pFeature = pEnumFeature.Next();
                }
                //����ϲ�
                ITopologicalOperator pTopo = new PolygonClass();
                pTopo.ConstructUnion(pGeometryCol as IEnumGeometry);
                pGeometry = pTopo as IGeometry;
                return pGeometry;
            }
            catch
            { return pGeometry= null; }
        }

        private void bttAfter_Click(object sender, EventArgs e)
        {
            m_iPage = m_iPage - 1;
            DataTable SourceDataTable = GetPagedTable(m_SourceDataTable, m_iPage, 1000);
            SetBttEnable(m_iPage, m_iPageCount);
            labRecord.Text = SourceDataTable.Rows.Count.ToString();
            this.dataGridViewX1.DataSource = SourceDataTable;
        }

        private void bttFirst_Click(object sender, EventArgs e)
        {
            m_iPage = 1;
            DataTable SourceDataTable = GetPagedTable(m_SourceDataTable, m_iPage, 1000);
            SetBttEnable(m_iPage, m_iPageCount);
            labRecord.Text = SourceDataTable.Rows.Count.ToString();
            this.dataGridViewX1.DataSource = SourceDataTable;
        }

        private void bttNext_Click(object sender, EventArgs e)
        {
            m_iPage = m_iPage + 1;
            DataTable SourceDataTable = GetPagedTable(m_SourceDataTable, m_iPage, 1000);
            SetBttEnable(m_iPage, m_iPageCount);
            labRecord.Text = SourceDataTable.Rows.Count.ToString();
            this.dataGridViewX1.DataSource = SourceDataTable;
        }

        private void bttLast_Click(object sender, EventArgs e)
        {
            m_iPage = m_iPageCount;
            DataTable SourceDataTable = GetPagedTable(m_SourceDataTable, m_iPage, 1000);
            SetBttEnable(m_iPage, m_iPageCount);
            labRecord.Text = SourceDataTable.Rows.Count.ToString();
            this.dataGridViewX1.DataSource = SourceDataTable;
        }
    }
}