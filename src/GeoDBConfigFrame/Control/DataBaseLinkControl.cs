using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using GeoDataCenterFunLib;
using SysCommon;

namespace GeoDBConfigFrame
{
    public partial class DataBaseLinkControl  : DevComponents.DotNetBar.Office2007Form
    {
        private enumWSType m_wsType;            //���ݿ�����

        GetDataTreeInitIndex m_dIndex = new GetDataTreeInitIndex();
        public DataBaseLinkControl()
        {
            InitializeComponent();

            //��ʼ�����������ļ��м�¼����Ϣ
            string strDbType = m_dIndex.GetDbValue("dbType");
            string strServerPath = m_dIndex.GetDbValue("dbServerPath");
            txtServer.Text = strServerPath;
            cboDataType.Text = strDbType;

            if (strDbType.Equals("SDE"))
            {
                labelX5.Text = "��������";
                btnServer.Visible = false;
                btnServer.Enabled = false;
                txtService.Enabled = true;
                txtDataBase.Enabled = true;
                txtUser.Enabled = true;
                txtPassWord.Enabled = true;
                txtVersion.Enabled = true;

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

            InitializeComboBox();
        }

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
                labelX5.Text = "��������";
                btnServer.Visible = false;
                btnServer.Enabled = false;
                txtService.Enabled = true;
                txtDataBase.Enabled = true;
                txtUser.Enabled = true;
                txtPassWord.Enabled = true;
                txtVersion.Enabled = true;

                m_wsType = enumWSType.SDE;
            }
            else if (cboDataType.Text.Equals("PDB"))
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
            else if (cboDataType.Text.Equals("GDB"))
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
                    if(!FolderBrowser.SelectedPath.Contains(".gdb"))
                    {
                        MessageBox.Show("��ѡ��File Geodatabase���ݿ�");
                        return;
                    }
                    txtServer.Text = FolderBrowser.SelectedPath;
                    btnServer.Tooltip = FolderBrowser.SelectedPath;
                }
            }
        }

        //����
        private void buttonXTest_Click(object sender, EventArgs e)
        {

        }

        //ȷ�� 
        private void btnOK_Click(object sender, EventArgs e)
        {
            //��ʼ�����������ļ��м�¼����Ϣ
            string strServerPath = txtServer.Text;
            string strDbType = cboDataType.Text;

            if (cboDataType.Text.Equals("PDB") || cboDataType.Text.Equals("GDB"))
            {
                m_dIndex.SetDbValue("dbServerPath", strServerPath);
                m_dIndex.SetDbValue("dbType", strDbType);
            }
            else if (cboDataType.Text.Equals("SDE"))
            {
                string strService = this.txtService.Text.Trim();
                string strServerName = this.txtDataBase.Text.Trim();
                string strDbUser =this.txtUser.Text.Trim();
                string strDbPassword = this.txtPassWord.Text.Trim();
                string strVersion = this.txtVersion.Text.Trim();
                m_dIndex.SetDbValue("dbServerPath", strServerPath);
                m_dIndex.SetDbValue("dbType", strDbType);
                m_dIndex.SetDbValue("dbServerName", strServerName);
                m_dIndex.SetDbValue("dbUser", strDbUser);
                m_dIndex.SetDbValue("dbPassword", strDbPassword);
                m_dIndex.SetDbValue("dbService", strService);
                m_dIndex.SetDbValue("dbVersion", strVersion);
            }

            this.DialogResult = DialogResult.OK;
        }

        //ȡ��
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }


    }
}