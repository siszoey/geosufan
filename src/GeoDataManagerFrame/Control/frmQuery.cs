using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;

namespace GeoDataManagerFrame
{
    public partial class frmQuery : DevComponents.DotNetBar.Office2007Form
    {
        private IFeatureLayer m_pCurrentLayer; //��ǰͼ��
        public string m_sWhereClause
        {
            get;
            set;
        }

       
        DataTable m_dataTableField = new DataTable();

        public frmQuery(IFeatureLayer pFeatureLayer,string strWhereClause)
        {
            m_pCurrentLayer = pFeatureLayer;
            m_sWhereClause = strWhereClause;

            InitializeComponent();
            IntializeDataTable();
            GetLayerField();

            //if (this.ShowDialog() == DialogResult.OK)
            //{
            //    m_sWhereClause = richTextExpression.Text;
            //}
        }
        /// <summary>
        /// ��ʼ���ֶα�
        /// </summary>
        private void IntializeDataTable()
        {
            //�ֶα�������
            m_dataTableField.Columns.Add("Name", typeof(string));
            m_dataTableField.Columns.Add("Value", typeof(string));
            //ָ���ֶε�����Դ
            lstSyllable.DataSource = m_dataTableField;
            lstSyllable.DisplayMember = "Name";
            lstSyllable.ValueMember = "Value";
        }

        private void GetLayerField()
        {
            m_dataTableField.Rows.Clear();
            if (m_pCurrentLayer == null)
            {
                MessageBox.Show("ѡ��ͼ�����", "ϵͳ��ʾ");
                return;
            }
            IFeatureClass pFeatureClass = m_pCurrentLayer.FeatureClass;
            for (int i = 0; i < pFeatureClass.Fields.FieldCount; i++)
            {
                IField pField = pFeatureClass.Fields.get_Field(i);
                object[] values = new object[2];
                values[0] = m_pCurrentLayer.FeatureClass.Fields.get_Field(i).Name + "��" + SysCommon.ModField.GetChineseNameOfField(m_pCurrentLayer.FeatureClass.Fields.get_Field(i).AliasName) + "��";
                values[1] = pField.Name;
                switch (pField.Type)
                {
                    case esriFieldType.esriFieldTypeSmallInteger:
                    case esriFieldType.esriFieldTypeInteger:
                    case esriFieldType.esriFieldTypeSingle:
                    case esriFieldType.esriFieldTypeDouble:
                    case esriFieldType.esriFieldTypeString:
                    case esriFieldType.esriFieldTypeOID:
                    case esriFieldType.esriFieldTypeDate:
                        m_dataTableField.Rows.Add(values);
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// �������������
        /// </summary>
        /// <param name="button"></param>
        private void btnClick(ButtonX button)
        {
            richTextExpression.Text = richTextExpression.Text + button.Text+" ";//xisheng 20110804
        }
        /// <summary>
        /// ����ѯ����Ƿ����
        /// </summary>
        /// <param name="whereClause"></param>
        /// <param name="bShow"></param>
        /// <returns></returns>
        private bool CheckExpression(string whereClause, bool bShow)
        {
            if (m_pCurrentLayer == null) return false;

            IFeatureClass pFeatureClass = m_pCurrentLayer.FeatureClass;
            IQueryFilter pQueryFilter = new QueryFilterClass();
            try
            {
                //if (whereClause == "" || whereClause == null) return false;
                pQueryFilter.WhereClause = whereClause;
                IFeatureCursor pFeatureCursor = pFeatureClass.Search(pQueryFilter, false);
                IFeature pFeature = pFeatureCursor.NextFeature();
                if (pFeature != null)
                {
                    if (bShow == true)
                    {
                        MessageBox.Show("���ʽ��ȷ��", "ϵͳ��ʾ");
                    }
                    else if (bShow == false)
                    {
                        return true;
                    }
                    return true;
                }
                else
                {
                    if (bShow == true)
                    {
                        MessageBox.Show("�˱��ʽ��������Ҫ��,������ʽ��", "ϵͳ��ʾ");
                    }
                    return false;
                }
            }
            catch
            {
                if (bShow == true)
                {
                    MessageBox.Show("�˱��ʽ��������Ҫ��,������ʽ��", "ϵͳ��ʾ");
                }
                return false;
            }
        }

        #region ����������¼�
        private void btnBigger_Click(object sender, EventArgs e)
        {
            btnClick(btnBigger);
        }

        private void btnSmaller_Click(object sender, EventArgs e)
        {
            btnClick(btnSmaller);
        }

        private void btnEqual_Click(object sender, EventArgs e)
        {
            btnClick(btnEqual);
        }

        private void btnW_Click(object sender, EventArgs e)
        {
            btnClick(btnW);
        }

        private void btnBiggerEqual_Click(object sender, EventArgs e)
        {
            btnClick(btnBiggerEqual);
        }

        private void btnSmallerEqual_Click(object sender, EventArgs e)
        {
            btnClick(btnSmallerEqual);
        }

        private void btnNotEqual_Click(object sender, EventArgs e)
        {
            btnClick(btnNotEqual);
        }

        private void btnX_Click(object sender, EventArgs e)
        {
            btnClick(btnX);
        }

        private void btnLike_Click(object sender, EventArgs e)
        {
            btnClick(btnLike);
        }

        private void btnAnd_Click(object sender, EventArgs e)
        {
            btnClick(btnAnd);
        }

        private void btnOr_Click(object sender, EventArgs e)
        {
            btnClick(btnOr);
        }

        private void btnParentheses_Click(object sender, EventArgs e)
        {
            btnClick(btnParentheses);
        }

        private void btnIs_Click(object sender, EventArgs e)
        {
            btnClick(btnIs);
        }

        private void btnNot_Click(object sender, EventArgs e)
        {
            btnClick(btnNot);
        }
        #endregion

        private void frmQuery_Load(object sender, EventArgs e)
        {
            //���ʽ��գ���ذ�ť������
            richTextExpression.Text = m_sWhereClause;
            if (richTextExpression.Text == "")
            {
                btnClear.Enabled = false;
                btnSave.Enabled = false;
                btnVerify.Enabled = false;
            }
            //for (int j = 0; j < m_pCurrentLayer.FeatureClass.Fields.FieldCount; j++)
            //{
            //    //ZQ 2011 modify  ������Ķ���
            //    lstSyllable.Items.Add(m_pCurrentLayer.FeatureClass.Fields.get_Field(j).Name + "��" + SysCommon.ModField.GetChineseNameOfField(m_pCurrentLayer.FeatureClass.Fields.get_Field(j).AliasName) + "��");
            //}
        }

        private void richTextExpression_TextChanged(object sender, EventArgs e)
        {
            if (richTextExpression.Text == "")
            {
                btnClear.Enabled = false;
                btnVerify.Enabled = false;
                btnSave.Enabled = false;
            }
            else if (richTextExpression.Text != "" && richTextExpression.Text.Trim() == "")
            {
                btnClear.Enabled = true;
                btnSave.Enabled = false;
                btnVerify.Enabled = false;
            }
            else
            {
                btnClear.Enabled = true;
                btnSave.Enabled = true;
                btnVerify.Enabled = true;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Dispose(true);
        }

        /// <summary>
        /// ˫���ֶΣ���������ʽ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lstSyllable_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int iIndex = lstSyllable.IndexFromPoint(e.Location);
            if (lstSyllable.GetSelected(iIndex) == true)
            {
                string sValue = lstSyllable.SelectedValue.ToString();
                richTextExpression.Text = richTextExpression.Text + sValue + " ";
            }
        }
        /// <summary>
        /// ˫��ֵ����������ʽ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lstValue_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int iIndex = lstValue.IndexFromPoint(e.Location);
            if (m_pCurrentLayer == null || iIndex == -1) return;
            string sValue = lstValue.SelectedValue.ToString(); //�޸� ����ȡlstValueѡ�е�ֵ     ����   20110706
            string sFieldName = lstSyllable.SelectedValue.ToString();

            int iFieldIndex = m_pCurrentLayer.FeatureClass.Fields.FindField(sFieldName);
            IField pField = m_pCurrentLayer.FeatureClass.Fields.get_Field(iFieldIndex);
            if (pField.VarType > 1 && pField.VarType < 6)
            {
                richTextExpression.Text = richTextExpression.Text + sValue + " ";
            }
            else
            {
                richTextExpression.Text = richTextExpression.Text + "'" + sValue + "'";
            }
        }

        private void btnUniqueValues_Click(object sender, EventArgs e)
        {
            ////�����ڵ�ǰ���Լ�������ѡ����ֶ�ʱ������
            //if (m_pCurrentLayer == null || lstSyllable.SelectedItem == null) return;
            ////���Table
            //m_dataTableValue.Rows.Clear();
            ////��ȡ��ǰ��FeatureClass
            //IFeatureClass pFeatureClass = m_pCurrentLayer.FeatureClass;
            ////��FeatureClass�˻�ȡ����·��
            //string dataPath = pFeatureClass.FeatureDataset.Workspace.PathName;

            ////��ѯ���ֶ���
            //string sFieldName = lstSyllable.SelectedValue.ToString();
            //string tableName = m_pCurrentLayer.Name; //��ѯ�ı���
            //string queryStr = "select distinct " + sFieldName + " from " + tableName; //��ѯ���
            //string connStr = @"provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + dataPath;
            //OleDbConnection Connection;
            //Connection = new OleDbConnection(connStr);

            //OleDbDataAdapter dataAdapter = new OleDbDataAdapter(queryStr, Connection);
            //Connection.Open();
            //dataAdapter.Fill(m_dataTableValue); //��Table��ֵ

            //m_dataTableValue.Select(string.Empty, sFieldName);

            //lstValue.DisplayMember = sFieldName;
            //lstValue.ValueMember = sFieldName;
            //lstValue.DataSource = m_dataTableValue;
            DataTable m_dataTableValue = new DataTable();
            //�ֶα�������
            m_dataTableValue.Columns.Add("Name", typeof(string));
            m_dataTableValue.Columns.Add("Value", typeof(string));
            //ָ���ֶε�����Դ

            lstValue.DisplayMember = "Name";
            lstValue.ValueMember = "Value";
            //�����ڵ�ǰ�� ���� ������ ѡ���ֶ�ʱ����
            if (m_pCurrentLayer == null || this.lstSyllable.SelectedItems.Count == 0) return;

            string sFieldName = this.lstSyllable.SelectedValue.ToString();        //��ȡѡ������ַ���

            IFeatureClass pFeatureClass = m_pCurrentLayer.FeatureClass;         //�õ�Ҫ�ؼ���
            if (pFeatureClass == null) return;
            //this.lstValue.Items.Clear();
            try
            {
                IQueryFilter pQueryFilter = new QueryFilterClass();
                pQueryFilter.WhereClause = "";
                IFeatureCursor pCursor = pFeatureClass.Search(pQueryFilter, false);
                System.Collections.IEnumerator enumerator;
                IDataStatistics DS = new DataStatisticsClass();
                DS.Field = sFieldName;//����Ψһֵ�ֶ�
                DS.Cursor = pCursor as ICursor;//������Դ
                enumerator = DS.UniqueValues;//�õ�Ψһֵ
                enumerator.Reset();//����ָ���һ��ֵ
                while (enumerator.MoveNext())//����Ψһֵ
                {
                    object[] values = new object[2];
                     string strTemp=enumerator.Current.ToString();
                    values[0] = strTemp + "��" + SysCommon.ModField .GetDomainValueOfFieldValue (pFeatureClass,sFieldName,strTemp)+ "��";
                    values[1] = strTemp;
                    m_dataTableValue.Rows.Add(values);
                }

                System.Runtime.InteropServices.Marshal.ReleaseComObject(pCursor);
                pCursor = null;
                lstValue.DataSource = m_dataTableValue;
                //lstValue.Sorted=true;
                lstValue.Update();
            }
            catch (Exception ex)
            {
                MessageBox.Show("��ȡ�ֶ�ֵ�������󣬴���ԭ��Ϊ" + ex.Message, "ϵͳ��ʾ");
            }

        }

        #region ���ʽ����
        /// <summary>
        /// ���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClear_Click(object sender, EventArgs e)
        {
            richTextExpression.ResetText();
        }
        /// <summary>
        /// ��֤
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnVerify_Click(object sender, EventArgs e)
        {
            string whereClause = richTextExpression.Text.Trim();
            CheckExpression(whereClause, true);
        }
        /// <summary>
        /// ����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlgOpenFile = new OpenFileDialog();
            dlgOpenFile.Title = "��";
            dlgOpenFile.Filter = "All Files|*.*|Expressions|*.exp";
            dlgOpenFile.FilterIndex = 2;
            if (dlgOpenFile.ShowDialog() == DialogResult.OK)
            {
                richTextExpression.LoadFile(dlgOpenFile.FileName);
            }

        }
        /// <summary>
        /// ����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlgSaveFile = new SaveFileDialog();
            dlgSaveFile.Title = "���Ϊ";
            dlgSaveFile.Filter = "All Files|*.*|Expressions|*.exp";
            dlgSaveFile.FilterIndex = 2;
            if (dlgSaveFile.ShowDialog() == DialogResult.OK)
            {
                richTextExpression.SaveFile(dlgSaveFile.FileName);
            }
        }
        /// <summary>
        /// ����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExample_Click(object sender, EventArgs e)
        {

        }

        #endregion

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!CheckExpression(richTextExpression.Text.Trim(), false))
            {
                MessageBox.Show("���ʽ������������ԣ�","��ʾ");
                return;
            } 
            m_sWhereClause = richTextExpression.Text.Trim();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}