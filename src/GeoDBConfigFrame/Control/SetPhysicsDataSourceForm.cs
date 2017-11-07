using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using GeoDataCenterFunLib;
using SysCommon;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.esriSystem;

//*********************************************************************************
//** �ļ�����SetPhysicsDataSourceForm.cs
//** CopyRight (c) �人������Ϣ���̼������޹�˾�����������
//** �����ˣ�ϯʤ
//** ��  �ڣ�20011-03-18
//** �޸��ˣ�
//** ��  �ڣ�
//** ��  ����
//**
//** ��  ����1.0
//*********************************************************************************
namespace GeoDBConfigFrame
{
    public partial class SetPhysicsDataSourceForm : DevComponents.DotNetBar.Office2007Form
    {
        private enumWSType m_wsType;            //���ݿ�����

        GetDataTreeInitIndex m_dIndex = new GetDataTreeInitIndex();
        public SetPhysicsDataSourceForm()
        {
            InitializeComponent();
            //��ʼ�����������ļ��м�¼����Ϣ
            string strDbType = m_dIndex.GetDbValue("dbType");
            //string strServerPath = m_dIndex.GetDbValue("dbServerPath");
            //txtServer.Text = strServerPath;
            cboDataType.Text = strDbType;

            if (strDbType.Equals("SDE"))
            {
                labelX5.Text = "��������";
                btnServer.Visible = false;
                btnServer.Enabled = false;
                //txtService.Enabled = true;
                txtDataBase.Enabled = true;
                txtUser.Enabled = true;
                txtPassWord.Enabled = true;
                //txtVersion.Enabled = true;

                m_wsType = enumWSType.SDE;

                txtDataBase.Text = m_dIndex.GetDbValue("dbServerName");
                txtUser.Text = m_dIndex.GetDbValue("dbUser");
                txtPassWord.Text = m_dIndex.GetDbValue("dbPassword");
                txtService.Text = m_dIndex.GetDbValue("dbService");
                txtVersion.Text = m_dIndex.GetDbValue("dbVersion");
            }
            else if (strDbType.Equals("PDB"))
            {
                labelX5.Text = "���ݿ⣺";
                btnServer.Visible = true;
                btnServer.Enabled = true;
                txtService.Enabled = false;
                txtUser.Enabled = false;
                txtDataBase.Enabled = false;
                txtPassWord.Enabled = false;
                txtVersion.Enabled = false;

                m_wsType = enumWSType.PDB;
            }
            else if (strDbType.Equals("GDB"))
            {
                labelX5.Text = "���ݿ⣺";
                btnServer.Visible = true;
                btnServer.Enabled = true;
                txtService.Enabled = false;
                txtUser.Enabled = false;
                txtDataBase.Enabled = false;
                txtPassWord.Enabled = false;
                txtVersion.Enabled = false;

                m_wsType = enumWSType.GDB;
            }
            
            InitializeComDSname();
            InitializeComboBox();
        }
        //��������Դ����
        private void InitializeComDSname()
        {
            comboBoxDsName.Items.Clear();
            string mypath = m_dIndex.GetDbInfo();
            string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
            string strExp = "select ����Դ���� from ��������Դ��";
            GeoDataCenterDbFun db=new GeoDataCenterDbFun();
            List<string> list = db.GetDataReaderFromMdb(strCon, strExp);
            for (int i = 0; i < list.Count; i++)
            {
                comboBoxDsName.Items.Add(list[i]);
            }
            if (comboBoxDsName.Items.Count > 0)
            {
                comboBoxDsName.SelectedIndex = 0;
            }

        }
        //��������Դ����
        private void InitializeComboBox()
        {
            cboDataType.Items.Add("SDE");
            cboDataType.Items.Add("PDB");
            cboDataType.Items.Add("GDB");
            if (cboDataType.Text.Equals("SDE"))
            {
                cboDataType.SelectedIndex = 0;
            }
            else if (cboDataType.Text.Equals("PDB"))
            {
                cboDataType.SelectedIndex = 1;
            }
            else if (cboDataType.Text.Equals("GDB"))
            {
                cboDataType.SelectedIndex = 2;
            }
            else
            {
                cboDataType.SelectedIndex = 0;
            }

        }
        //����Դ���ͷ����仯
        private void cboDataType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboDataType.Text.Equals("SDE"))
            {
                labelX5.Text = "����������:";
                btnServer.Visible = false;
                btnServer.Enabled = false;
                txtServer.Text = "";
                //txtService.Enabled = true;
                txtDataBase.Enabled = true;
                txtUser.Enabled = true;
                txtPassWord.Enabled = true;
                //txtVersion.Enabled = true;
                //if (comboBoxDsName.Items.Count > 0)
                //{
                //    comboBoxDsName.SelectedIndex = 0;
                //}
                m_wsType = enumWSType.SDE;
            }
            else if (cboDataType.Text.Equals("PDB"))
            {
                labelX5.Text = "���ݿ�:";
                btnServer.Visible = true;
                btnServer.Enabled = true;
                txtService.Enabled = false;
                txtUser.Enabled = false;
                txtDataBase.Enabled = false;
                txtPassWord.Enabled = false;
                txtVersion.Enabled = false;
                //if (comboBoxDsName.Items.Count > 0)
                //{
                //    comboBoxDsName.SelectedIndex = 0;
                //}
                m_wsType = enumWSType.PDB;
            }
            else if (cboDataType.Text.Equals("GDB"))
            {
                labelX5.Text = "���ݿ�:";
                btnServer.Visible = true;
                btnServer.Enabled = true;
                txtService.Enabled = false;
                txtUser.Enabled = false;
                txtDataBase.Enabled = false;
                txtPassWord.Enabled = false;
                txtVersion.Enabled = false;
                //if (comboBoxDsName.Items.Count > 0)
                //{
                //    comboBoxDsName.SelectedIndex = 0;
                //}
                m_wsType = enumWSType.GDB;
            }
        }

        //��mdb�ļ�
        private void btnServer_Click(object sender, EventArgs e)
        {
            if (cboDataType.Text.Equals("PDB"))
            {
                //���Ŀ�����ΪPDBʱ�����ť��Ч
                OpenFileDialog OpenFile = new OpenFileDialog();
                OpenFile.Title = "ѡ��Personal Geodatabase���ݿ�";
                OpenFile.Filter = "���ݿ�(*.mdb)|*.mdb";

                if (OpenFile.ShowDialog() == DialogResult.OK)
                {
                    txtServer.Text = OpenFile.FileName;
                    btnServer.Tooltip = OpenFile.FileName;
                }
            }
            else if (cboDataType.Text.Equals("GDB"))
            {
                FolderBrowserDialog FolderBrowser = new FolderBrowserDialog();
                if (FolderBrowser.ShowDialog() == DialogResult.OK)
                {
                    if (!FolderBrowser.SelectedPath.Contains(".gdb"))
                    {
                        MessageBox.Show("��ѡ��File Geodatabase���ݿ�");
                        return;
                    }
                    txtServer.Text = FolderBrowser.SelectedPath;
                    btnServer.Tooltip = FolderBrowser.SelectedPath;
                }
            }
        }

        private void comboBoxDsName_SelectedIndexChanged(object sender, EventArgs e)
        {
            GeoDataCenterDbFun db = new GeoDataCenterDbFun();
            if (comboBoxDsName.Text.Trim() == "")
            {
                
                cboDataType.Text = "";
                txtUser.Text = "";
                txtPassWord.Text = "";
                txtServer.Text =
                txtDataBase.Text = "";
                return;
            }
            else
            {
                string strExp = "select * from ��������Դ�� where ����Դ����='" + comboBoxDsName.Text + "'";
                string mypath = m_dIndex.GetDbInfo();
                string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
                DataTable dt = db.GetDataTableFromMdb(strCon, strExp);
                if (dt.Rows.Count == 0)
                    return;
                //cboDataType.Text = dt.Rows[0]["����Դ����"].ToString();
                txtUser.Text = dt.Rows[0]["�û�"].ToString();
                txtPassWord.Text = dt.Rows[0]["����"].ToString();
                cboDataType.SelectedItem = dt.Rows[0]["����Դ����"] ;
                if (dt.Rows[0]["����Դ����"].Equals("SDE"))
                {
                   //SDE
                    txtServer.Text = dt.Rows[0]["������"].ToString();
                    txtDataBase.Text = dt.Rows[0]["���ݿ�"].ToString();
                   
                }
                else
                {
                    txtServer.Text = dt.Rows[0]["���ݿ�"].ToString();
                    txtDataBase.Text = "";
                }
            }
        }

        //�½�����Դ
        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                GeoDataCenterDbFun db = new GeoDataCenterDbFun();
                if (comboBoxDsName.Text == "")
                {
                    MessageBox.Show("����Դ���Ʋ���Ϊ��!", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                string mypath = m_dIndex.GetDbInfo();
                string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
                string strExp = "select count(*) from ��������Դ�� where ����Դ����='" + comboBoxDsName.Text+ "'";
                int count=db.GetCountFromMdb(strCon, strExp);
                if (count > 0)
                {
                    MessageBox.Show("����Դ�����Ѵ���!", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (labelX5.Text == "���ݿ�:")
                {
                    strExp = string.Format("insert into ��������Դ��(����Դ����,������,���ݿ�,�û�,����,����Դ����) values('{0}','{1}','{2}','{3}','{4}','{5}')",
                    comboBoxDsName.Text, "", txtServer.Text, "", "", cboDataType.Text);
                }
                else
                {
                    strExp = string.Format("insert into ��������Դ��(����Դ����,������,���ݿ�,�û�,����,����Դ����) values('{0}','{1}','{2}','{3}','{4}','{5}')",
                        comboBoxDsName.Text, txtServer.Text, txtDataBase.Text, txtUser.Text, txtPassWord.Text, cboDataType.Text);
                }
                db.ExcuteSqlFromMdb(strCon, strExp);
                MessageBox.Show("�½��ɹ�!", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                InitializeComDSname();
            }
            catch
            {
                MessageBox.Show("�½�ʧ��!", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        //ɾ������Դ
        private void btnDel_Click(object sender, EventArgs e)
        {
            GeoDataCenterDbFun db = new GeoDataCenterDbFun();
            try
            {
                if (comboBoxDsName.Text == "")
                {
                    MessageBox.Show("����Դ���Ʋ���Ϊ��!", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                string mypath = m_dIndex.GetDbInfo();
                string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
               // string strExp = "select count(*) from ��������Դ�� where ����Դ����=��" + comboBoxDsName.Text + "�� ";
                //int i = db.GetCountFromMdb(strCon, strExp);
                //if (i == 0)
                //{

                //    MessageBox.Show("����Դ������!", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    return;
                //}
                string strExp = "delete from ��������Դ�� where  ����Դ����='" + comboBoxDsName.Text + "'";
                db.ExcuteSqlFromMdb(strCon, strExp);
                strExp = "delete from ���ݱ���� where ����Դ����='" + comboBoxDsName.Text + "'";//added by yjl remove noexist source data
                db.ExcuteSqlFromMdb(strCon, strExp);
                strExp = "delete from �߼�����Դ�� where  ����Դ����='" + comboBoxDsName.Text + "'";
                InitializeComDSname();
                MessageBox.Show("ɾ���ɹ�!", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message+",ɾ��ʧ��!", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        
        //����
        private void buttonXTest_Click(object sender, EventArgs e)
        {
            if (cboDataType.Text == "SDE")
            {
                try
                {//Workspace
                    IWorkspaceFactory pWorkspaceFactory;
                    pWorkspaceFactory = new SdeWorkspaceFactoryClass();

                    //PropertySet
                    IPropertySet pPropertySet;
                    pPropertySet = new PropertySetClass();
                    //pPropertySet.SetProperty("Service", comboBoxDsName.Text);
                    pPropertySet.SetProperty("Server", txtServer.Text);
                    pPropertySet.SetProperty("Database", txtDataBase.Text);
                    pPropertySet.SetProperty("Instance", "5151");//"port:" + txtService.Text
                    pPropertySet.SetProperty("user", txtUser.Text);
                    pPropertySet.SetProperty("password", txtPassWord.Text);
                    pPropertySet.SetProperty("version", "sde.DEFAULT");
                    IWorkspace pws = pWorkspaceFactory.Open(pPropertySet, 0);
                    MessageBox.Show("���ӳɹ�!", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch { MessageBox.Show("����ʧ��!", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information); }
            }
            if(cboDataType.Text == "PDB")
            {
                if(txtServer.Text=="")
                {
                    MessageBox.Show("��������Ϊ��!", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                  return;                
                }

                try
                {
                    IWorkspaceFactory pWorkspaceFactory;
                    pWorkspaceFactory = new AccessWorkspaceFactoryClass();
                   IWorkspace pws= pWorkspaceFactory.OpenFromFile(txtServer.Text, 0);
                   MessageBox.Show("���ӳɹ�!", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
               }
               catch { MessageBox.Show("����ʧ��!", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information); }
            }
            if (cboDataType.Text == "GDB")
            {
                if (txtServer.Text == "")
                {
                    MessageBox.Show("��������Ϊ��!", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                try
                {
                    IWorkspaceFactory pWorkspaceFactory;
                    pWorkspaceFactory = new FileGDBWorkspaceFactoryClass();
                    IWorkspace pws = pWorkspaceFactory.OpenFromFile(txtServer.Text, 0);
                    MessageBox.Show("���ӳɹ�!", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch { MessageBox.Show("����ʧ��!", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information); }
            }



        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

       
    }
}