//*********************************************************************************
//** �ļ�����TableForm.cs
//** CopyRight (c) 2000-2007 �人������Ϣ���̼������޹�˾���̲�
//** �����ˣ�chulili
//** ��  �ڣ�2011-02
//** �޸��ˣ�
//** ��  �ڣ�
//** ��  ������������޸ļ�¼
//**
//** ��  ����1.0
//*********************************************************************************
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;

using SysCommon.Gis;
using SysCommon.Error;
using SysCommon.Authorize;
using ESRI.ArcGIS.Geodatabase;
namespace GeoDBConfigFrame
{
    public partial class TableForm : DevComponents.DotNetBar.Office2007Form
    {

        private int m_FieldCount = 0;
        private string m_connstr;
        private string m_TableName;
        private IRow pRow=null;//�²�����row��
        
        DevComponents.DotNetBar.LabelX[] m_LabelX = new DevComponents.DotNetBar.LabelX[9];
        Label[] m_LabelY = new Label[9];
        DevComponents.DotNetBar.Controls.TextBoxX[] m_TextBoxX = new DevComponents.DotNetBar.Controls.TextBoxX[9];
        DataGridView m_gridView;

        string m_EditMode;
        //��ʼ����� ���������DataGridView�ؼ������ݿ����Ӵ�������
        public TableForm(DataGridView gridview,string connstr,string Tablename)
        {
            InitializeComponent();
            if (gridview != null)
                m_gridView = gridview;
            m_connstr = connstr;
            m_TableName = Tablename;
        }
        //��ʼ��label����  changed by xisheng ����Զ�����label�Ŀ�ȣ�Ȼ�����
        private void initLableButton(int labcount)
        {
            m_FieldCount = labcount;

            int Top = 17;
            for (int i = 0; i < labcount; i++)
            {
                m_LabelY[i] = new Label();
                m_LabelY[i].Top = Top;
                m_LabelY[i].Left = 23;
                m_LabelY[i].AutoSize = true;
                this.Controls.Add(m_LabelY[i]);
                Top += 30;
            }

            Top = 17;
            for (int i = 0; i < labcount; i++)
            {
                m_TextBoxX[i] = new DevComponents.DotNetBar.Controls.TextBoxX();
                m_TextBoxX[i].Top = Top;
                m_TextBoxX[i].Left = 75;
                m_TextBoxX[i].Width = 142;
                m_TextBoxX[i].Height = 20;
                this.Controls.Add(m_TextBoxX[i]);
                Top += 30;
            }
            this.buttonXOK.Top = m_LabelY[labcount - 1].Top + 40;
            this.buttonXCancel.Top = m_LabelY[labcount - 1].Top + 40;
            this.Height = this.buttonXOK.Top + 60;

        }
        //��ʼ���Ի���
        //����������༭ģʽ����Ӽ�¼���޸ļ�¼���֣�  �����������
        public void InitForm(string EditMode)
        {
            m_EditMode=EditMode;
            int i = 0;
            int maxwidth=0;
            initLableButton(m_gridView.ColumnCount);
            if (EditMode == "MODIFY")
                this.Text = "�޸ļ�¼";
            else
            {
                this.Text = "��Ӽ�¼";
                Add();
            }
            for (i = 0; i < m_gridView.ColumnCount ; i++)  //��1��ʼ
            {
                //��Ӽ�¼ֻ������ֶ����Ƴ�ʼ����ǩ����
                m_LabelY[i].Text = m_gridView.Columns[i].Name+":";
                if (m_LabelY[i].Width > maxwidth) maxwidth=m_LabelY[i].Width;
                if (m_gridView.Columns[i].Name.ToUpper() == "ID")
                    m_TextBoxX[i].Enabled = false;
                //�޸ļ�¼��Ҫ�����ֶ�ֵ��ʼ���༭������
                if (EditMode == "MODIFY")
                    m_TextBoxX[i].Text = m_gridView.SelectedRows[0].Cells[i].Value.ToString();
            }
            for (i = 0; i < m_gridView.ColumnCount; i++)
            {
                m_TextBoxX[i].Left = m_LabelY[i].Left + maxwidth;
            }
        }


        private bool Add_Ex()
        {
            //SysGisTable sysTable = new SysGisTable(ModData.gisDb);
            ////�ж��Ǹ��»������
            //try
            //{
            //    if (!sysTable.ExistData("user_info", "NAME='" + txtUser.Text.Trim().ToLower() + "'"))
            //    {
            //        if (!sysTable.NewRow("user_info", dicData, out exError))
            //        {
            //            ErrorHandle.ShowFrmErrorHandle("��ʾ", "���ʧ�ܣ�" + exError.Message);
            //            return;
            //        }
            //    } 
            //    this.DialogResult=DialogResult.OK;
            //}
            //catch (Exception ex)
            //{
            //    exError = ex;
            //    ModData.gisDb.EndTransaction(false, out exError);
            //    ErrorHandle.ShowFrmErrorHandle("��ʾ", exError.Message);
            //}
            return false;
        }


        //�������ܣ������û���д�������Ӽ�¼
        //�����������  �����������
        private bool Addpre()
        {
            int i = 0;
            DataTable t = (DataTable)(m_gridView.DataSource);

            OleDbConnection mycon = new OleDbConnection(m_connstr);   //����OleDbConnection����ʵ�����������ݿ�
            string strExp = "";
            //�Ȳ�ѯ�����Ƿ����ͬ���ļ�¼
            strExp = "select * from " + m_TableName + " where";
            for (i = 0; i < m_gridView.ColumnCount; i++)
            {
                if (m_TextBoxX[i].Text.Equals(""))
                    strExp = strExp + " (" + m_LabelY[i].Text + "='' or " + m_LabelY[i].Text + " is null) and";
                else 
                    strExp = strExp + " " + m_LabelY[i].Text + "='" + m_TextBoxX[i].Text + "' and";
            }
            strExp = strExp.Substring(0, strExp.Length - 3);
            OleDbCommand aCommand = new OleDbCommand(strExp, mycon);
            mycon.Open();
            OleDbDataReader aReader = aCommand.ExecuteReader();
            //����ͬ���ļ�¼���˳�����
            if (aReader.Read())
            {
                aReader.Close();
                mycon.Close();
                return false;
            }
            aReader.Close();
            mycon.Close();

            DataRow tmprow;
            tmprow = t.NewRow();
            //����ӵ��¼�¼������
            for (i = 0; i < m_gridView.ColumnCount; i++)  
            {
                if (m_LabelY[i].Text.ToUpper().Equals("ID"))
                    continue;
                tmprow[m_LabelY[i].Text ] = m_TextBoxX[i].Text;
            }
            //��Ӽ�¼
            t.Rows.Add(tmprow);
            m_gridView.DataSource = t;
            return true;
        }
           //�������ܣ��޸ļ�¼
        //�����������  ���������������
        //�������ܣ������û���д�������Ӽ�¼
        //�����������  ���������bool
        private bool Add()
        {
            try
            {
                if (GeoDataCenterFunLib.LogTable.m_sysTable == null)
                    return false;
                Dictionary<string, object> pDic = new Dictionary<string, object>();
                Exception ex=null;
                ITable pTable = GeoDataCenterFunLib.LogTable.m_sysTable.OpenTable(m_TableName, out ex);
                if (pTable == null)
                    return false;
                pRow = pTable.CreateRow();
                for (int i = 0; i < m_gridView.ColumnCount; i++)  //��ʾ�²����ļ�¼ID���û�
                {
                    if (m_gridView.Columns[i].Name.ToUpper() == "ID")
                    {
                        m_TextBoxX[i].Text = pRow.OID.ToString();
                        ModDBOperate.oid = pRow.OID;
                        break;
                    }
                }
            }
            catch(Exception ex)
            {
                ErrorHandle.ShowFrmErrorHandle("��ʾ", "�޸ļ�¼ʧ�ܣ�" + ex.Message);
                return false;
            }
            return true;
        }
        //�������ܣ��޸ļ�¼for add
        //�����������  ���������������
        private bool ModifyAfterAdd()
        {
            if (GeoDataCenterFunLib.LogTable.m_sysTable == null)
                return false;
             Exception ex=null;
            ITable pTable = GeoDataCenterFunLib.LogTable.m_sysTable.OpenTable(m_TableName, out  ex);
            if (pTable == null)
                return false;
            string whereClause = pTable.OIDFieldName;
            Dictionary<string, object> pDic = new Dictionary<string, object>();
            for (int i = 0; i < m_gridView.ColumnCount; i++)  //��1��ʼ����һ����ID
            {
                if (m_gridView.Columns[i].Name.ToUpper() == "ID")
                {
                    whereClause += " = " + m_TextBoxX[i].Text;
                    continue;
                }
                //if (m_LabelX[i].Text.Contains(":"))
                //{
                //    m_LabelX[i].Text = m_LabelX[i].Text.Substring(0, m_LabelX[i].Text.LastIndexOf(":"));
                //}
                pDic.Add(m_gridView.Columns[i].Name, m_TextBoxX[i].Text);
            }
            Exception ex1 = null;
            try
            {
                return GeoDataCenterFunLib.LogTable.m_sysTable.UpdateRowByAliasName(m_TableName,whereClause,pDic, out ex1);//���¼�¼
            }
            catch (Exception err)
            {
                ErrorHandle.ShowFrmErrorHandle("��ʾ", "��Ӽ�¼ʧ�ܣ�" + err.Message);
                return false;
            }


        }
        //�������ܣ��޸ļ�¼
        //�����������  ���������������
        private bool Modify()
        {
            if (GeoDataCenterFunLib.LogTable.m_sysTable == null)
                return false;
            Exception err;
            ITable pTable = GeoDataCenterFunLib.LogTable.m_sysTable.OpenTable(m_TableName, out err);
            if (pTable == null)
                return false;
            string whereClause = pTable.OIDFieldName;
            Dictionary<string, object> pDic = new Dictionary<string, object>();
            for (int i = 0; i < m_gridView.ColumnCount; i++)  //��1��ʼ����һ����ID
            {
                if (m_gridView.Columns[i].Name.ToUpper() == "ID")
                {
                    whereClause += " = " + m_TextBoxX[i].Text;
                    continue;
                }
                pDic.Add(m_gridView.Columns[i].Name, m_TextBoxX[i].Text);
            }
            Exception ex = null;
            try
            {
                return GeoDataCenterFunLib.LogTable.m_sysTable.UpdateRowByAliasName(m_TableName, whereClause, pDic, out ex);
            }
            catch (Exception err1)
            {
                ErrorHandle.ShowFrmErrorHandle("��ʾ", "�޸ļ�¼ʧ�ܣ�" + err1.Message);
                return false;
            }


        }
        //�������ܣ��޸ļ�¼
        //�����������  ���������������
        private bool Modifypre()
        {
            
            //int i = 0;  writed by others commented by yjl begin 2011-6-11
            ////for (i = 0; i < m_gridView.ColumnCount ; i++)  //��1��ʼ����һ����ID
            ////{
            ////    m_gridView.SelectedRows[0].Cells[i].Value =m_TextBoxX[i].Text ;
            ////}
            ////OleDbConnection mycon = new OleDbConnection(m_connstr);   //����OleDbConnection����ʵ�����������ݿ�
            //string strExp = "";
            ////���ұ�����û�����޸ĺ���ͬ�ļ�¼
            //strExp = "select * from " + m_TableName +" where";
            //for (i = 0; i < m_gridView.ColumnCount; i++) 
            //{
            //    if (m_LabelX[i].Text.ToUpper().Equals("ID"))
            //        continue;
            //    if (m_TextBoxX[i].Text.Equals(""))
            //        strExp = strExp + " (" + m_LabelX[i].Text + "='' or " + m_LabelX[i].Text + " is null) and";
            //    else 
            //        strExp =strExp+ " " + m_LabelX[i].Text + "='" + m_TextBoxX[i].Text+ "' and";
            //}
            //strExp = strExp.Substring(0,strExp.Length - 3);
            //OleDbCommand aCommand = new OleDbCommand(strExp, mycon);     
            //mycon.Open();
            //OleDbDataReader aReader = aCommand.ExecuteReader();
            ////������д������޸ĺ���ͬ�ļ�¼�����˳�����
            //if(aReader.Read())
            //{
            //    aReader.Close ();
            //    mycon.Close();
            //    return false;
            //}
            //aReader.Close();
            ////�������sql��䣨����ҵ�����û��id�У�����ʹ��dataview�Դ��Ĺ��ܽ��и��£�
            //strExp="update " + m_TableName +" set ";
            //for (i = 0; i < m_gridView.ColumnCount; i++) 
            //{
            //    if (m_LabelX[i].Text.ToUpper().Equals("ID"))
            //        continue;
            //    strExp =strExp+ " " + m_LabelX[i].Text + "='" + m_TextBoxX[i].Text+ "',";
            //}
            //strExp = strExp.Substring(0,strExp.Length - 1);
            //strExp = strExp + " where ";
            //for (i = 0; i < m_gridView.ColumnCount; i++)
            //{
            //    if (m_gridView.Columns[i].Name.ToUpper().Equals("ID"))
            //        continue;
            //    if (m_gridView.SelectedRows[0].Cells[i].Value.ToString().Equals(""))
            //        strExp = strExp + " (" + m_gridView.Columns[i].Name + "='' or " + m_gridView.Columns[i].Name + " is null) and";
            //    else 
            //        strExp = strExp + " " + m_gridView.Columns[i].Name + "='" + m_gridView.SelectedRows[0].Cells[i].Value.ToString() + "' and";
            //}
            //strExp = strExp.Substring(0,strExp.Length - 3);
            ////ִ�и������
            //aCommand.CommandText=strExp;
            //aCommand.ExecuteNonQuery ();
            //mycon.Close();  
            //writed by others commented by yjl end 
            return true;
        }
        //����ȷ����ťʱ���ݱ༭ģʽѡ��ִ�еĹ��ܺ���
        private void buttonXOK_Click(object sender, EventArgs e)
        {
            if (m_EditMode == "ADD")
                if (ModifyAfterAdd())
                {
                    MessageBox.Show("��ӳɹ���", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                    return;
            else
                if (Modify())
                    MessageBox.Show("�޸ĳɹ���", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    return;
            this.DialogResult = DialogResult.OK;
            
        }
        //����ȡ����ťʱ�����Ӧ
        private void buttonXCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;

        }
    }
}