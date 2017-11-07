using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using GeoDataCenterFunLib;

//*********************************************************************************
//** �ļ�����frmDataUpload.cs
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

    public partial class SetLogicDataSourceForm : DevComponents.DotNetBar.Office2007Form
    {
        public SetLogicDataSourceForm()
        {
            InitializeComponent();
        }
        GetDataTreeInitIndex m_dIndex = new GetDataTreeInitIndex();
        private void SetLogicDataSourceForm_Load(object sender, EventArgs e)
        {
            string mypath = m_dIndex.GetDbInfo();
            string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
            string strExp = "select ����Դ���� from ��������Դ��";
            GeoDataCenterDbFun db = new GeoDataCenterDbFun();
            List<string> list = db.GetDataReaderFromMdb(strCon, strExp);
            for (int i = 0; i < list.Count; i++)
            {
                comboBoxDsName.Items.Add(list[i]);
            }
            if (comboBoxDsName.Items.Count > 0)
            {
                comboBoxDsName.SelectedIndex = 0;
            }
            
            strExp = "select �������� from ���ݵ�Ԫ��";
            list = new List<string>();
            list = db.GetDataReaderFromMdb(strCon, strExp);
            for (int i = 0; i < list.Count; i++)
            {
                comboBoxAreaCode.Items.Add(list[i]);
            }
            if (comboBoxAreaCode.Items.Count > 0)
            {
                comboBoxAreaCode.SelectedIndex = 0;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void comboBoxAreaCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxAreaCode.Text != "")
            {
                string mypath = m_dIndex.GetDbInfo();
                string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
                string strExp = "select �������� from ���ݵ�Ԫ�� where ��������='"+comboBoxAreaCode.Text+"'";
                 GeoDataCenterDbFun db = new GeoDataCenterDbFun();
                 txtAreaName.Text = db.GetInfoFromMdbByExp(strCon, strExp);
            }
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            if (comboBoxDsName.Text == "")
            {
                MessageBox.Show("����ԴΪ�գ��޷�ɾ��", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string mypath = m_dIndex.GetDbInfo();
            string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
            string strExp = "delete * from �߼�����Դ�� where ��������='"+comboBoxAreaCode.Text+"' and ����Դ����='"+comboBoxDsName.Text+"'";
            GeoDataCenterDbFun db = new GeoDataCenterDbFun();
            db.ExcuteSqlFromMdb(strCon, strExp);
            MessageBox.Show("ɾ���ɹ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnNew_Click(object sender, EventArgs e)
        {

            if (comboBoxDsName.Text == "")
            {
                MessageBox.Show("����������������Դ", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string mypath = m_dIndex.GetDbInfo();
            string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
            string strExp = "select count(*) from �߼�����Դ�� where ��������='" + comboBoxAreaCode.Text + "' and ����Դ����='" + comboBoxDsName.Text + "'";
            GeoDataCenterDbFun db = new GeoDataCenterDbFun();
            int i=db.GetCountFromMdb(strCon, strExp);
            if (i != 0)
            {
                MessageBox.Show("����Դ������", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            strExp = "insert into �߼�����Դ��(��������,����Դ����) values('" + comboBoxAreaCode.Text + "','" + comboBoxDsName.Text + "')";
            db.ExcuteSqlFromMdb(strCon,strExp);
            MessageBox.Show("�½��ɹ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}