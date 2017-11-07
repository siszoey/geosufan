using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace GeoUtilities
{
    public partial class FrmSQLQuery : DevComponents.DotNetBar.Office2007Form
    {
        private IMapControlDefault m_MapControlDefault;
        private IMap m_pMap;
        private IFeatureLayer m_pCurrentLayer;
        private bool m_Show;
        public SysCommon.BottomQueryBar _QueryBar
        {
            get;
            set;
        }

        public FrmSQLQuery(IMapControlDefault pMapControl,bool bShow)
        {
            m_MapControlDefault = pMapControl;
            m_pMap = pMapControl.Map;
            m_Show = bShow;
            InitializeComponent();
            cmbselmode.Enabled = !m_Show;
        }
        private void btnOperateClick(DevComponents.DotNetBar.ButtonX button)
        {
            richTextExpression.Text += button.Text.Trim()+" ";
        }
        #region �������ĵ���¼�
        private void btnBigger_Click(object sender, EventArgs e)
        {
             btnOperateClick(btnBigger);
        }

        private void btnSmaller_Click(object sender, EventArgs e)
        {
            btnOperateClick(btnSmaller );
        }

        private void btnEqual_Click(object sender, EventArgs e)
        {
            btnOperateClick(btnEqual );
        }

        private void btnBiggerEqual_Click(object sender, EventArgs e)
        {
            btnOperateClick(btnBiggerEqual );
        }

        private void btnSmallerEqual_Click(object sender, EventArgs e)
        {
            btnOperateClick(btnSmallerEqual );
        }

        private void btnNotEqual_Click(object sender, EventArgs e)
        {
            btnOperateClick(btnNotEqual );
        }

        private void btn1Ultra_Click(object sender, EventArgs e)
        {
            btnOperateClick(btn1Ultra );
        }

        private void btn2Ultra_Click(object sender, EventArgs e)
        {
            btnOperateClick(btn2Ultra );
        }

        private void btnPercent_Click(object sender, EventArgs e)
        {
            btnOperateClick(btnPercent );
        }

        private void btnIs_Click(object sender, EventArgs e)
        {
            btnOperateClick(btnIs );
        }

        private void btnOr_Click(object sender, EventArgs e)
        {
            btnOperateClick(btnOr );
        }

        private void btnNot_Click(object sender, EventArgs e)
        {
            btnOperateClick(btnNot );
        }

        private void btnLike_Click(object sender, EventArgs e)
        {
            btnOperateClick(btnLike );
        }

        private void btnAnd_Click(object sender, EventArgs e)
        {
            btnOperateClick(btnAnd );
        }
        #endregion

        private void FrmSQLQuery_Load(object sender, EventArgs e)
        {
            this.richTextExpression.Text = "";
            //ͼ��ѡ�����ݼ���
            IFeatureLayer pFeaTureLayer;
            for (int iIndex = 0; iIndex < m_pMap.LayerCount; iIndex++)
            {
                ILayer pLayer = m_pMap.get_Layer(iIndex);
                if (pLayer is IGroupLayer)
                {
                    if (pLayer.Name == "ʾ��ͼ") continue;
                    ICompositeLayer pComLayer = pLayer as ICompositeLayer;
                    for(int i=0;i<pComLayer.Count;i++)
                    {
                        ILayer mLayer =pComLayer.get_Layer(i);
                        if (mLayer is IFeatureLayer)
                        {
                            pFeaTureLayer = mLayer as IFeatureLayer;
                            this.cmblayersel.Items.Add(pFeaTureLayer.Name);//
                        }
                    }
                }
                else if (pLayer is IFeatureLayer)
                {
                    pFeaTureLayer = m_pMap.get_Layer(iIndex) as IFeatureLayer;
                    this.cmblayersel.Items.Add(pFeaTureLayer.Name);//
                }
            }
            if (this.cmblayersel.Items.Count > 0)
            {
                this.cmblayersel.SelectedIndex = 0;
            }
            this.listViewField.Scrollable = true;
            this.listViewValue.Scrollable = true;

            //ѡ��ģʽ���ݼ���
            object[] objArray = new object[4];
            objArray[0] = "����һ���µ�ѡ����";
            objArray[1] = "��ӵ���ǰѡ����";
            objArray[2] = "�ӵ�ǰѡ�������Ƴ�";
            objArray[3] = "�ӵ�ǰѡ������ѡ��";

            this.cmbselmode.Items.AddRange(objArray);//
            this.cmbselmode.SelectedIndex = 0;

            
            //��ʼ���ֶ��б�
            ColumnHeader newColumnHeader1 = new ColumnHeader();
            newColumnHeader1.Width = listViewField.Width - 5;
            newColumnHeader1.Text = "�ֶ���";
            //newColumnHeader1.AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
            this.listViewField.Columns.Add(newColumnHeader1);

            //��ʼ��ֵ�б�
            ColumnHeader newColumnHeader2 = new ColumnHeader();
            newColumnHeader2.Width = listViewValue.Width - 5;
            newColumnHeader2.Text = "�ֶ�ֵ";
            //newColumnHeader2.AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
            this.listViewValue.Columns.Add(newColumnHeader2);

            //��Χ
            this.comboExtent.SelectedIndex = 0;
        }

        private void cmblayersel_SelectedIndexChanged(object sender, EventArgs e)
        {
            int Index = this.cmblayersel.SelectedIndex;                        //��ȡѡ���������

            for (int i = 0; i < m_pMap.LayerCount; i++)
            {
                ILayer pLayer = m_pMap.get_Layer(i);
                if (pLayer is IGroupLayer)
                {
                    if (pLayer.Name == "ʾ��ͼ") continue;
                    ICompositeLayer pComLayer = pLayer as ICompositeLayer;
                    for (int j = 0; j < pComLayer.Count; j++)
                    {
                        ILayer mLayer = pComLayer.get_Layer(j);
                        if (mLayer is IFeatureLayer&&mLayer.Name==this.cmblayersel.Items[Index].ToString())
                        {
                            m_pCurrentLayer = mLayer as IFeatureLayer;
                        }
                    }
                }
                else if (pLayer is IFeatureLayer && pLayer.Name == this.cmblayersel.Items[Index].ToString())
                {
                    m_pCurrentLayer = pLayer as IFeatureLayer; 
                }
            }
                //ע���ڳ�ʼ��ʱ����������������һ��

            this.listViewField.Items.Clear();                             //��Ϊ�ֶ�ֵ����ͼ����ı䣬���
            this.listViewValue.Items.Clear();

            if (m_pCurrentLayer == null)                                     //������ѡ��ͼ�� ����ʾ
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("ϵͳ��ʾ","��ѡͼ����ڴ���");
                return;
            }

            IFeatureClass pFeatureClass = m_pCurrentLayer.FeatureClass; //��ȡҪ����ļ���

            //ѭ���õ�ÿһ���ֶβ������ֶ����ͽ��ж�Ӧ�Ĳ���
            //�˴��Ժ�Ҫ�������Ӹ����жϺͶ�Ӧ����
            for (int iIndex = 0; iIndex < pFeatureClass.Fields.FieldCount; iIndex++)
            {
                IField pField = pFeatureClass.Fields.get_Field(iIndex);
                switch (pField.Type)
                {
                    case esriFieldType.esriFieldTypeSmallInteger:
                    case esriFieldType.esriFieldTypeInteger:
                    case esriFieldType.esriFieldTypeSingle:
                    case esriFieldType.esriFieldTypeDouble:
                    case esriFieldType.esriFieldTypeString:
                    case esriFieldType.esriFieldTypeDate:
                    case esriFieldType.esriFieldTypeOID:
                        ListViewItem newItem = new ListViewItem(new string[] { pField.Name });
                        this.listViewField.Items.Add(newItem);
                        break;
                    default:
                        break;
                }
            }
           
            
        }
        //�ֶ��б�˫���¼������ֶ����Ƽ��뵽richTextExpression��
        private void listViewField_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            
           ListViewItem currentFieldItem= this.listViewField.GetItemAt(e.Location.X, e.Location.Y);
             if (this.listViewField.SelectedItems.Count > 1)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ֻѡ��һ���ֶ�");
            }
            if (currentFieldItem == null) return;
            if (currentFieldItem.Selected == true)
            {
                string sValue = currentFieldItem.Text.Trim();
                this.richTextExpression.Text = this.richTextExpression.Text + sValue +" " ;
            }
           
        }

        //ֵ�б�˫���¼������ֶε�ֵ���뵽richTextExpression��
        private void listViewValue_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewItem currentValueItem = this.listViewValue.GetItemAt(e.Location.X, e.Location.Y);
            //�����ǰͼ��Ϊ�ջ����ֶ�ֵΪ�գ��򷵻�
            if (m_pCurrentLayer == null || currentValueItem == null) return;
            IFeatureClass pFeatureClass = m_pCurrentLayer.FeatureClass;
            IDataset pDataSet = pFeatureClass as IDataset;
            IWorkspace pWorkSpace = pDataSet.Workspace;
            if (pWorkSpace == null) return;
            string sValue = currentValueItem.Text.Trim();

            string sFieldName = this.listViewField.SelectedItems[0].Text.Trim();
            if (sFieldName == "")
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ѡ���ֶ���");
            }

            int iFieldIndex = m_pCurrentLayer.FeatureClass.Fields.FindField(sFieldName);
            IField pField = m_pCurrentLayer.FeatureClass.Fields.get_Field(iFieldIndex);

            if (pField.VarType > 1 && pField.VarType < 6)
            {
                this.richTextExpression.Text = this.richTextExpression.Text + sValue + " ";
            }
            else if (pField.VarType == 7)
            {//date //"2009-5-18 0:00:00"
                int lastIndex = sValue.IndexOf(" ");//�ո������ֵ
                if (pWorkSpace.Type == esriWorkspaceType.esriFileSystemWorkspace)
                {//�ļ���
                    sValue = sValue.Substring(0, lastIndex);
                    this.richTextExpression.Text = this.richTextExpression.Text + "date '" + sValue + "'";
                }
                else if (pWorkSpace.Type == esriWorkspaceType.esriLocalDatabaseWorkspace)
                { //GDB,PDB 
                    if (pWorkSpace.WorkspaceFactory.GetClassID().Value.ToString() == "{71FE75F0-EA0C-4406-873E-B7D53748AE7E}")
                    {//GDB
                        sValue = sValue.Substring(0, lastIndex);
                        this.richTextExpression.Text = this.richTextExpression.Text +"date '" + sValue + " 00:00:00'";
                    }
                    else
                    {//PDB
                        string interV = sValue.Substring(5,lastIndex-5);
                        string firstV = sValue.Substring(0, 4);
                        sValue = interV + "-" + firstV;
                        this.richTextExpression.Text = this.richTextExpression.Text +"#"+ sValue + " 00:00:00#";
                    }
                }
                else if (pWorkSpace.Type == esriWorkspaceType.esriRemoteDatabaseWorkspace)
                {//SDE
                    sValue = sValue.Substring(0, lastIndex);
                    this.richTextExpression.Text = this.richTextExpression.Text + "TO_DATE('" + sValue + " 00:00:00','YYYY-MM-DD HH24:MI:SS')";
                }

            }
            else if (pField.VarType == 8)
            {//string
                this.richTextExpression.Text = this.richTextExpression.Text + "'" + sValue + "'";
            }
        }

        //ͼ��ѡ�� ���ܱ༭
        private void cmblayersel_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }
        //ѡ��ʽ ���ܱ༭
        private void cmbselmode_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        //��ʾ���ܵ�ֵ
        private void btnDisplayValue_Click(object sender, EventArgs e)
        {
            //�����ڵ�ǰ�� ���� ������ ѡ���ֶ�ʱ����
            if (m_pCurrentLayer == null || this.listViewField.SelectedItems.Count  == 0) return;

            string sFieldName = this.listViewField.SelectedItems[0].Text.Trim();        //��ȡѡ������ַ���
            //string sFieldName = listBox_Field.SelectedValue.ToString();         //��ȡѡ������ַ���

            IFeatureClass pFeatureClass = m_pCurrentLayer.FeatureClass;         //�õ�Ҫ�ؼ���
            if (pFeatureClass == null) return;

            try
            {
                //ע����ȷʹ��FeatureLayer��FeatureClass
                //IFeatureCursor pFeatureCursor = pFeatureClass.Search(null, false);  //��������ѯ�õ�ȫ����Feature
                IFeatureCursor pFeatureCursor = m_pCurrentLayer.Search(null, false);

                List<string> listValue = new List<string>();                        //����ΨһҪ�� ֵ�ļ��� 
                IFeature pFeature = pFeatureCursor.NextFeature();                   //ȡ��Ҫ��ʵ��

                this.listViewValue.Items.Clear();

                while (pFeature != null)
                {
                    int iFieldIndex = pFeature.Fields.FindField(sFieldName);            //�õ��ֶ�ֵ������
                    object objValue = pFeature.get_Value(iFieldIndex);

                    if (objValue != null)
                    {
                        string sValue = objValue.ToString();
                        if (!listValue.Contains(sValue))
                        {
                            if (listValue.Count > 200)                           //�Ƿ񳬹���������¼
                            {
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("ϵͳ��ʾ","����ֵ�ļ�¼�Ѿ�����200���������ټ�����ʾ!");
                                break;
                            }

                            listValue.Add(sValue);
                            ListViewItem newItem = new ListViewItem(new string[] { sValue });
                            this.listViewValue.Items.Add(newItem);
                        }
                    }
                    pFeature = pFeatureCursor.NextFeature();
                }

                Marshal.ReleaseComObject(pFeatureCursor);
                listViewValue.Sort();
                this.listViewValue.Update();                                         //����
            }
            catch (Exception ex)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("ϵͳ��ʾ","��ȡ�ֶ�ֵ�������󣬴���ԭ��Ϊ" + ex.Message);
            }
        }

        //������ʽ
        private void btnClearExpression_Click(object sender, EventArgs e)
        {
            this.richTextExpression.ResetText();
        }
        //��֤���ʽ
        private void btnValidateExpression_Click(object sender, EventArgs e)
        {
            if (this.richTextExpression.Text.Trim() == "")
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("ϵͳ��ʾ","���ʽΪ�գ���������ʽ��");
                return;
            }
            string whereClause = this.richTextExpression.Text.Trim();

            CheckExpression(whereClause, true);
        }

        //���ؽ����
        private void btnLoadResults_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlgOpenFile = new OpenFileDialog();

            dlgOpenFile.FilterIndex = 2;
            dlgOpenFile.Title = "��ѡ����Ҫ���صĽ����";
            dlgOpenFile.Filter = "All Files (*.*)|*.*|Text Files(*.exp)|*.exp";

            if (dlgOpenFile.ShowDialog() == DialogResult.OK)
            {
                richTextExpression.LoadFile(dlgOpenFile.FileName);
            }
        }
        //��������
        private void btnSaveResults_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlgSaveFile = new SaveFileDialog();

            dlgSaveFile.FilterIndex = 2;
            dlgSaveFile.Title = "��ָ������������·��";
            dlgSaveFile.Filter = "All Files (*.*)|*.*|Text Files(*.exp)|*.exp";

            if (dlgSaveFile.ShowDialog() == DialogResult.OK)
            {
                richTextExpression.SaveFile(dlgSaveFile.FileName);
            }
        }
        //ʾ��
        private void btnSample_Click(object sender, EventArgs e)
        {
            //��յ�ǰ�ı��ʽ��
            this.richTextExpression.ResetText();

            if (m_pCurrentLayer == null) return;

            //�ֶο�����ֶ���
            if (this.listViewField.Items.Count > 1)
            {
                //��ȡ��ǰ��IFeatureClass,Ȼ��ȡ��Feature��ָ��
                IFeatureClass pFeatureClass = m_pCurrentLayer.FeatureClass;
                //ע����ȷʹ��FeatureLayer��FeatureClass
                //IFeatureCursor pFeatureCursor = pFeatureClass.Search(null, false);
                IFeatureCursor pFeatureCursor = m_pCurrentLayer.Search(null, false);

                //����ȡ��ÿһ��feature
                IFeature pFeature = pFeatureCursor.NextFeature();
                if(pFeature != null)
                {
                    string sValue;
                    string sFieldName = this.listViewField.Items[0].Text;
                    int iIndex = pFeatureClass.Fields.FindField(sFieldName);

                    IField pField = pFeatureClass.Fields.get_Field(iIndex);

                    //�������ͺ�bolb���͵�ֵ ��Ϊ"shape"
                    if (pField.Type == esriFieldType.esriFieldTypeGeometry || pField.Type == esriFieldType.esriFieldTypeBlob)
                    {
                        sValue = "shape";
                    }
                    else
                    {
                        //һֱѭ����ȡ����ȷ��ֵ
                        sValue = pFeature.get_Value(iIndex).ToString();
                        this.richTextExpression.Text = sFieldName + " = " + "'" + sValue + "'";

                        if (CheckExpression(this.richTextExpression.Text.Trim(), false) == true) return;

                    }
                }

                Marshal.ReleaseComObject(pFeatureCursor);
            }
        }
        /// У����ʽ�Ƿ�Ϸ�
        private bool CheckExpression(string sExpression, bool bShow)
        {
            if (m_pCurrentLayer == null) return false;

            //��ȡ��ǰͼ��� featureclass
            IFeatureClass pFeatCls = m_pCurrentLayer.FeatureClass;
            //�����ѯ������
            IQueryFilter pFilter = null;
            if (m_pGeometryFilter == null)
            {
                pFilter = new QueryFilterClass();
            }
            else
            {
                pFilter = new SpatialFilterClass();
                ISpatialFilter pSpatialFilter = pFilter as ISpatialFilter;
                pSpatialFilter.Geometry = m_pGeometryFilter;
                pSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
            }

            try
            {
                //��ֵ��������
                pFilter.WhereClause = sExpression;
                //�õ���ѯ���
                int intCount = pFeatCls.FeatureCount(pFilter);
                if (intCount>0)
                {
                    if (bShow == true)
                    {
                        SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("ϵͳ��ʾ", "���ʽ��ȷ!");
                    }
                    return true;
                }
                else
                {
                    if (bShow == true)
                    {
                        SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("ϵͳ��ʾ", "���ʽ��ȷ������������Ҫ��,������ʽ");
                    }
                    return false;
                }
            }
            catch
            {
                if (bShow == true)
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("ϵͳ��ʾ","���ʽ��д����,������ʽ");
                }
                return false;
            }
        }
       //�˳���
        private void FrmSQLQuery_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Dispose(true);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            //û�е�ǰͼ��ֱ���˳�
            if (m_pCurrentLayer == null) return;

            string whereClause = this.richTextExpression.Text.Trim();
            if (CheckExpression(whereClause, false) == false) return;

            //��ȡ��ǰͼ��� featureclass
            IFeatureClass pFeatClass = m_pCurrentLayer.FeatureClass;

            //�����ѯ������
            IQueryFilter pQueryFilter = null;
            if (m_pGeometryFilter == null)
            {
                pQueryFilter = new QueryFilterClass();
            }
            else
            {
                pQueryFilter = new SpatialFilterClass();
                ISpatialFilter pSpatialFilter = pQueryFilter as ISpatialFilter;
                pSpatialFilter.Geometry = m_pGeometryFilter;
                pSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
            }
            

            //��ֵ��������
            pQueryFilter.WhereClause = whereClause;

            //��ֵ��ѯ��ʽ,�ɲ�ѯ��ʽ��combo���
            esriSelectionResultEnum pSelectionResult;
            switch (this.cmbselmode.SelectedItem.ToString())
            {
                case ("����һ���µ�ѡ����"):
                    pSelectionResult = esriSelectionResultEnum.esriSelectionResultNew;
                    break;
                case ("��ӵ���ǰѡ����"):
                    pSelectionResult = esriSelectionResultEnum.esriSelectionResultAdd;
                    break;
                case ("�ӵ�ǰѡ�������Ƴ�"):
                    pSelectionResult = esriSelectionResultEnum.esriSelectionResultSubtract;
                    break;
                case ("�ӵ�ǰѡ������ѡ��"):
                    pSelectionResult = esriSelectionResultEnum.esriSelectionResultAnd;
                    break;
                default:
                    return;
            }

            if (m_Show)
            {
                //���в�ѯ�����������ʾ����
               // frmQuery frm = new frmQuery(m_MapControlDefault);
                //frm.FillData(m_pCurrentLayer, pQueryFilter, pSelectionResult);
                //frm.TopMost = true;//��ֹ��ѯ�����С��     ����   20110709
                //frm.Show();
                _QueryBar.m_pMapControl = m_MapControlDefault;
                _QueryBar.EmergeQueryData(m_MapControlDefault.Map, m_pCurrentLayer, pQueryFilter, pSelectionResult);
                try
                {
                    DevComponents.DotNetBar.Bar pBar = _QueryBar.Parent.Parent as DevComponents.DotNetBar.Bar;
                    if (pBar != null)
                    {
                        pBar.AutoHide = false;
                        //pBar.SelectedDockTab = 1;
                        int tmpindex = pBar.Items.IndexOf("dockItemDataCheck");
                        pBar.SelectedDockTab = tmpindex;
                    }
                }
                catch
                { }
            }
            else
            {
                IFeatureSelection pFeatureSelection = m_pCurrentLayer as IFeatureSelection;
                pFeatureSelection.SelectFeatures(pQueryFilter, pSelectionResult, false);
                m_MapControlDefault.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, null, m_MapControlDefault.ActiveView.FullExtent);
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        /// <summary>
        /// �ռ��ϵ ��Χ����
        /// </summary>
        private IGeometry m_pGeometryFilter = null;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboExtent_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_pMap == null) return;

            switch (this.comboExtent.SelectedItem.ToString())
            {
                case "ȫͼ��Χ":
                    m_pGeometryFilter = null;
                    break;
                case "��ǰ��ͼ��Χ":
                    IActiveView pAv=m_pMap as IActiveView;
                    m_pGeometryFilter = pAv.Extent;
                    break;
                case "ѡ��Ҫ�ط�Χ":
                    m_pGeometryFilter = ConstructUnion(m_pMap);
                    if (m_pGeometryFilter == null)
                    {
                        MessageBox.Show("��ǰ��ͼ��ѡ��Ҫ�ء�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    break;
            }
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

    }
}