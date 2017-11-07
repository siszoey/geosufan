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

namespace GeoDBATool
{
    public partial class FrmSQLQuery : DevComponents.DotNetBar.Office2007Form
    {
        private IFeatureClass m_CurrentFeaCls;
        private string m_WhereClause = "";
        public string WhereClause
        {
            get
            {
                return m_WhereClause;
            }
            set
            {
                m_WhereClause = value;
            }
        }

        public FrmSQLQuery(IFeatureClass mFeaCls, List<IField> fieldList)
        {
            m_CurrentFeaCls = mFeaCls;
            if (m_CurrentFeaCls == null) return;
            InitializeComponent();

            IntialCom(fieldList);
        }

        /// <summary>
        /// ��ʼ���ֶ������б�
        /// </summary>
        /// <param name="fieldList"></param>
        private void IntialCom(List<IField> fieldList)
        {
            this.richTextExpression.Text = "";
            this.listViewField.Scrollable = true;
            this.listViewValue.Scrollable = true;


            //��ʼ���ֶ��б�
            ColumnHeader newColumnHeader1 = new ColumnHeader();
            newColumnHeader1.Width = listViewField.Width - 5;
            newColumnHeader1.Text = "�ֶ���";
            this.listViewField.Columns.Add(newColumnHeader1);

            //��ʼ��ֵ�б�
            ColumnHeader newColumnHeader2 = new ColumnHeader();
            newColumnHeader2.Width = listViewValue.Width - 5;
            newColumnHeader2.Text = "�ֶ�ֵ";
            this.listViewValue.Columns.Add(newColumnHeader2);

            //����б�
            listViewField.Items.Clear();
            listViewValue.Items.Clear();

            //��ʼ���ֶ��б�
            for (int i = 0; i < fieldList.Count; i++)
            {
                if (fieldList[i].Type == esriFieldType.esriFieldTypeBlob) continue;
                if (fieldList[i].Type == esriFieldType.esriFieldTypeGeometry) continue;
                if (fieldList[i].Type == esriFieldType.esriFieldTypeRaster) continue;
                if (fieldList[i].Type == esriFieldType.esriFieldTypeOID) continue;
                listViewField.Items.Add(fieldList[i].Name);
            }

        }

        #region �������ĵ���¼�
        private void btnBigger_Click(object sender, EventArgs e)
        {
            btnOperateClick(btnBigger);
        }

        private void btnSmaller_Click(object sender, EventArgs e)
        {
            btnOperateClick(btnSmaller);
        }

        private void btnEqual_Click(object sender, EventArgs e)
        {
            btnOperateClick(btnEqual);
        }

        private void btnBiggerEqual_Click(object sender, EventArgs e)
        {
            btnOperateClick(btnBiggerEqual);
        }

        private void btnSmallerEqual_Click(object sender, EventArgs e)
        {
            btnOperateClick(btnSmallerEqual);
        }

        private void btnNotEqual_Click(object sender, EventArgs e)
        {
            btnOperateClick(btnNotEqual);
        }

        private void btn1Ultra_Click(object sender, EventArgs e)
        {
            btnOperateClick(btn1Ultra);
        }

        private void btn2Ultra_Click(object sender, EventArgs e)
        {
            btnOperateClick(btn2Ultra);
        }

        private void btnPercent_Click(object sender, EventArgs e)
        {
            btnOperateClick(btnPercent);
        }

        private void btnIs_Click(object sender, EventArgs e)
        {
            btnOperateClick(btnIs);
        }

        private void btnOr_Click(object sender, EventArgs e)
        {
            btnOperateClick(btnOr);
        }

        private void btnNot_Click(object sender, EventArgs e)
        {
            btnOperateClick(btnNot);
        }

        private void btnLike_Click(object sender, EventArgs e)
        {
            btnOperateClick(btnLike);
        }

        private void btnAnd_Click(object sender, EventArgs e)
        {
            btnOperateClick(btnAnd);
        }

        private void btnOperateClick(DevComponents.DotNetBar.ButtonX button)
        {
            richTextExpression.Text += button.Text.Trim() + " ";
        }
        #endregion


        //�ֶ��б�˫���¼������ֶ����Ƽ��뵽richTextExpression��
        private void listViewField_MouseDoubleClick(object sender, MouseEventArgs e)
        {

            ListViewItem currentFieldItem = this.listViewField.GetItemAt(e.Location.X, e.Location.Y);
            if (this.listViewField.SelectedItems.Count > 1)
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ֻѡ��һ���ֶ�");
            }
            if (currentFieldItem == null) return;
            if (currentFieldItem.Selected == true)
            {
                string sValue = currentFieldItem.Text.Trim();
                this.richTextExpression.Text = this.richTextExpression.Text + sValue + " ";
            }

        }

        //ֵ�б�˫���¼������ֶε�ֵ���뵽richTextExpression��
        private void listViewValue_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewItem currentValueItem = this.listViewValue.GetItemAt(e.Location.X, e.Location.Y);
            //�����ǰͼ��Ϊ�ջ����ֶ�ֵΪ�գ��򷵻�
            if (currentValueItem == null) return;
            IDataset pDataSet = m_CurrentFeaCls as IDataset;
            IWorkspace pWorkSpace = pDataSet.Workspace;
            if (pWorkSpace == null) return;
            string sValue = currentValueItem.Text.Trim();

            string sFieldName = this.listViewField.SelectedItems[0].Text.Trim();
            if (sFieldName == "")
            {
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("��ʾ", "��ѡ���ֶ���");
            }

            int iFieldIndex = m_CurrentFeaCls.Fields.FindField(sFieldName);
            IField pField = m_CurrentFeaCls.Fields.get_Field(iFieldIndex);

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
                        this.richTextExpression.Text = this.richTextExpression.Text + "date '" + sValue + " 00:00:00'";
                    }
                    else
                    {//PDB
                        string interV = sValue.Substring(5, lastIndex - 5);
                        string firstV = sValue.Substring(0, 4);
                        sValue = interV + "-" + firstV;
                        this.richTextExpression.Text = this.richTextExpression.Text + "#" + sValue + " 00:00:00#";
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

        //��ʾ���ܵ�ֵ
        private void btnDisplayValue_Click(object sender, EventArgs e)
        {
            //�����ڵ�ǰ�� ���� ������ ѡ���ֶ�ʱ����
            if (this.listViewField.SelectedItems.Count == 0) return;

            string sFieldName = this.listViewField.SelectedItems[0].Text.Trim();        //��ȡѡ������ַ���

            try
            {
                //ע����ȷʹ��FeatureLayer��FeatureClass
                IFeatureCursor pFeatureCursor = m_CurrentFeaCls.Search(null, false);

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
                                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("ϵͳ��ʾ", "����ֵ�ļ�¼�Ѿ�����200���������ټ�����ʾ!");
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
                //*******************************************************************
                //guozheng added
                if (ModData.SysLog != null)
                {
                    ModData.SysLog.Write(ex, null, DateTime.Now);
                }
                else
                {
                    ModData.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                    ModData.SysLog.Write(ex, null, DateTime.Now);
                }
                //********************************************************************
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("ϵͳ��ʾ", "��ȡ�ֶ�ֵ�������󣬴���ԭ��Ϊ" + ex.Message);
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
                SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("ϵͳ��ʾ", "���ʽΪ�գ���������ʽ��");
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


            //�ֶο�����ֶ���
            if (this.listViewField.Items.Count > 1)
            {
                //��ȡ��ǰ��IFeatureClass,Ȼ��ȡ��Feature��ָ��

                //ע����ȷʹ��FeatureLayer��FeatureClass
                IFeatureCursor pFeatureCursor = m_CurrentFeaCls.Search(null, false);

                //����ȡ��ÿһ��feature
                IFeature pFeature = pFeatureCursor.NextFeature();
                if (pFeature != null)
                {
                    string sValue;
                    string sFieldName = this.listViewField.Items[0].Text;
                    int iIndex = m_CurrentFeaCls.Fields.FindField(sFieldName);

                    IField pField = m_CurrentFeaCls.Fields.get_Field(iIndex);

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

            //�����ѯ������
            IQueryFilter pFilter = new QueryFilterClass();

            try
            {
                //��ֵ��������
                pFilter.WhereClause = sExpression;
                //�õ���ѯ���
                //ע����ȷʹ��FeatureLayer��FeatureClass
                //IFeatureCursor pFeatCursor = m_pCurrentLayer.Search(pFilter, false);
                IFeatureCursor pFeatCursor = m_CurrentFeaCls.Search(pFilter, false);
                //ȡ�õ�һ��feature
                IFeature pFeat = pFeatCursor.NextFeature();
                Marshal.ReleaseComObject(pFeatCursor);
                if (pFeat != null)
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
                        SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("ϵͳ��ʾ", "�˱��ʽ��������Ҫ��,������ʽ");
                    }
                    return false;
                }
            }
            catch (Exception e)
            {
                //*******************************************************************
                //guozheng added
                if (ModData.SysLog != null)
                {
                    ModData.SysLog.Write(e, null, DateTime.Now);
                }
                else
                {
                    ModData.SysLog = new SysCommon.Log.clsWriteSystemFunctionLog();
                    ModData.SysLog.Write(e, null, DateTime.Now);
                }
                //********************************************************************
                if (bShow == true)
                {
                    SysCommon.Error.ErrorHandle.ShowFrmErrorHandle("ϵͳ��ʾ", "�˱��ʽ��������Ҫ��,������ʽ");
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
            string whereClause = this.richTextExpression.Text.Trim();
            if (CheckExpression(whereClause, false) == false) return;

            m_WhereClause = whereClause;
            //��ȡ��ǰͼ��� featureclass
            //IFeatureClass pFeatClass = m_pCurrentLayer.FeatureClass;

            ////�����ѯ������
            //IQueryFilter pQueryFilter = new QueryFilterClass();
            ////��ֵ��������
            //pQueryFilter.WhereClause = whereClause;

            ////��ֵ��ѯ��ʽ,�ɲ�ѯ��ʽ��combo���
            //esriSelectionResultEnum pSelectionResult;
            //switch (this.cmbselmode.SelectedItem.ToString())
            //{
            //    case ("����һ���µ�ѡ����"):
            //        pSelectionResult = esriSelectionResultEnum.esriSelectionResultNew;
            //        break;
            //    case ("��ӵ���ǰѡ����"):
            //        pSelectionResult = esriSelectionResultEnum.esriSelectionResultAdd;
            //        break;
            //    case ("�ӵ�ǰѡ�������Ƴ�"):
            //        pSelectionResult = esriSelectionResultEnum.esriSelectionResultSubtract;
            //        break;
            //    case ("�ӵ�ǰѡ������ѡ��"):
            //        pSelectionResult = esriSelectionResultEnum.esriSelectionResultAnd;
            //        break;
            //    default:
            //        return;
            //}

            //if (m_Show)
            //{
            //    //���в�ѯ�����������ʾ����
            //    GeoUtilities.frmQuery frm = new GeoUtilities.frmQuery(m_MapControlDefault);
            //    frm.FillData(m_pCurrentLayer, pQueryFilter, pSelectionResult);
            //    frm.Show();
            //}
            //else
            //{
            //    IFeatureSelection pFeatureSelection = m_pCurrentLayer as IFeatureSelection;
            //    pFeatureSelection.SelectFeatures(pQueryFilter, pSelectionResult, false);
            //    m_MapControlDefault.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, null, m_MapControlDefault.ActiveView.FullExtent);
            //}

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

    }
}