using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using GeoDataCenterFunLib;

namespace GeoDBConfigFrame
{
    public partial class AddField :  DevComponents.DotNetBar.Office2007Form
    {
        public AddField()
        {
            InitializeComponent();
        }
        private int m_flag=1;//1��ʾ����ֶΣ�2��ʾ�༭�ֶ�
        private string m_fleldname;//��ȡ�ֶ�����
        private int id;
        public int flag
        {
            set { m_flag = value; }
        }
        public string fleldname
        {
            set { m_fleldname = value; }
        }

        private void AddField_Load(object sender, EventArgs e)
        {
            
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            string mypath = dIndex.GetDbInfo();
            string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //������������
            GeoDataCenterDbFun db = new GeoDataCenterDbFun();
            string strExp;
            if (m_flag == 1)//����ֶ�
                comboBoxType.SelectedIndex = 0;
            else//�༭�ֶ�
            {
                strExp = "select * from ͼ��������ʼ���� where �ֶ�����='" + m_fleldname + "'";
                DataTable dt = db.GetDataTableFromMdb(strCon, strExp);
                comBoxName.Text = m_fleldname;
                textBoxDescribe.Text = dt.Rows[0]["����"].ToString();
                comboBoxType.Text = dt.Rows[0]["�ֶ�����"].ToString();
                textBoxLength.Text = dt.Rows[0]["�ֶγ���"].ToString();
                checkBoxChange.Checked = (bool)(dt.Rows[0]["�ɱ�"]);
                textBoxDefault.Text = dt.Rows[0]["ȱʡ"].ToString();
                id = Convert.ToInt32(dt.Rows[0]["ID"]);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (comBoxName.Text == "")
            {
                MessageBox.Show("�ֶ����Ʋ���Ϊ��!", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (textBoxDefault.Text == "")
            {
                MessageBox.Show("ȱʡֵ����Ϊ��!", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            string mypath = dIndex.GetDbInfo();
            string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //������������
            GeoDataCenterDbFun db = new GeoDataCenterDbFun();
            string strExp;
            if (m_flag == 1)
            {
                strExp = "select max(����) from ͼ��������ʼ����";
                int index = Convert.ToInt32(db.GetInfoFromMdbByExp(strCon,strExp)) + 1;
                strExp = string.Format("insert into ͼ��������ʼ����(�ֶ�����,����,�ֶ�����,�ֶγ���,ȱʡ,�ɱ�,����) values('{0}','{1}','{2}','{3}','{4}',{5},'{6}')",
                    comBoxName.Text, textBoxDescribe.Text, comboBoxType.Text, textBoxLength.Text, textBoxDefault.Text, checkBoxChange.Checked,index.ToString());
                db.ExcuteSqlFromMdb(strCon, strExp);
                MessageBox.Show("����ֶγɹ�!", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                strExp = string.Format("update ͼ��������ʼ���� set �ֶ�����='{0}',����='{1}',�ֶ�����='{2}',�ֶγ���='{3}',ȱʡ='{4}',�ɱ�={5} where ID={6}",
                     comBoxName.Text, textBoxDescribe.Text, comboBoxType.Text, textBoxLength.Text, textBoxDefault.Text, checkBoxChange.Checked,id);
                db.ExcuteSqlFromMdb(strCon, strExp);
                MessageBox.Show("�༭�ֶγɹ�!", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }    
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void checkBoxChange_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxChange.Checked == true)
            {
                textBoxLength.Enabled = false;
                textBoxLength.Text = "";
            }
            else
                textBoxLength.Enabled = true;
        }

    }
}