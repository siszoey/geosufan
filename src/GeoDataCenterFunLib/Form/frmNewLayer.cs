using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GeoDataCenterFunLib
{
    public partial class frmNewLayer : DevComponents.DotNetBar.Office2007Form
    {
        public frmNewLayer(List<string> list,ListView listview)
        {
            InitializeComponent();
            m_list = list;
            m_listview = listview;
        }

        private List<string> m_list;
        private ListView m_listview;
        private void comboBoxBig_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBoxBig.Items.Count>0)
            ChangeComboxSub();
        }
        private void ChangeComboxSub()
        {
            comboBoxSub.Items.Clear();
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            string strExp = "";
            string mypath = dIndex.GetDbInfo();
            string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
            GeoDataCenterDbFun db = new GeoDataCenterDbFun();
            string strall = comboBoxBig.Text;
            string[] BigClass = strall.Split('(', ')');
            strExp = "select ����,ҵ��С����� from ҵ��С������ where ҵ��������='" + BigClass[1] + "'";
            DataTable dt = db.GetDataTableFromMdb(strCon, strExp);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                comboBoxSub.Items.Add(dt.Rows[i]["����"] + "(" + dt.Rows[i]["ҵ��С�����"] + ")");
            }

            if (comboBoxSub.Items.Count > 0)
                comboBoxSub.SelectedIndex = 0;

        }

        private void LoadComBig()
        {
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            string strExp = "";
            string mypath = dIndex.GetDbInfo();
            string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
            GeoDataCenterDbFun db = new GeoDataCenterDbFun();
            strExp = "select ����,���� from ҵ���������";
            DataTable dt = db.GetDataTableFromMdb(strCon, strExp);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                comboBoxBig.Items.Add(dt.Rows[i]["����"] + "(" + dt.Rows[i]["����"] + ")");
            }
            if (comboBoxBig.Items.Count > 0)
                comboBoxBig.SelectedIndex = 0;
        }

        private void frmNewLayer_Load(object sender, EventArgs e)
        {
            LoadComBig();
            for (int ii = 0; ii < m_list.Count; ii++)
            {
                comboBoxCode.Items.Add(m_list[ii]);
            }
            if (comboBoxCode.Items.Count > 0)
                comboBoxCode.SelectedIndex = 0;
        }

        //ѡ�����
        private void comboBoxCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            //GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            //string strExp = "";
            //string mypath = dIndex.GetDbInfo();
            //string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
            //GeoDataCenterDbFun db = new GeoDataCenterDbFun();
            //strExp="select * from ��׼ͼ����Ϣ�� where ����=''"
           
        }

        //
        private void btnApply_Click(object sender, EventArgs e)
        {
            frmDataUpload frm = new frmDataUpload();
            if (textBoxDescri.Text.Trim() == "")
            {
                MessageBox.Show("ͼ����������Ϊ��", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            string strExp = "";
            string mypath = dIndex.GetDbInfo();
            string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
            GeoDataCenterDbFun db = new GeoDataCenterDbFun();
            strExp = string.Format("insert into ��׼ͼ����Ϣ��(����,����,ҵ��������,ҵ��С�����) values('{0}','{1}','{2}','{3}')",
                comboBoxCode.Text, textBoxDescri.Text, GetCode(comboBoxBig.Text),GetCode(comboBoxSub.Text));
            db.ExcuteSqlFromMdb(strCon, strExp);
            comboBoxCode.Items.Remove(comboBoxCode.Text);
            if (comboBoxCode.Items.Count == 0)
            {
                comboBoxCode.Text = "";
                btnApply.Enabled = false;
            }
            else
            {
                comboBoxCode.SelectedIndex = 0;
            }
            textBoxDescri.Text = "";

            foreach (ListViewItem item in m_listview.Items)
            {
                string strName = item.SubItems[1].Text;
                if (strName.Length > 15)
                {
                    string strforward = strName.Substring(0, 15);
                    strName = strName.Substring(15);
                    item.SubItems[1].Text = frm.GetForwadName(strforward, strName);

                }
                string strdescri = GetDescrib(strName);
                if (strdescri.Trim() != "")
                {
                    item.SubItems[2].Text = strdescri;
                }
                else
                {
                    item.SubItems[2].Text = "��Ҫ����";
                }
                
            }
            m_listview.Refresh();
            
        }
        private string GetDescrib(string str)
        {
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            string mypath = dIndex.GetDbInfo();
            string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
            string strExp = "select ���� from ��׼ͼ����Ϣ�� where ����='" + str + "'";
            GeoDataCenterDbFun db = new GeoDataCenterDbFun();
            string strreturn = db.GetInfoFromMdbByExp(strCon, strExp);
            return strreturn;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (comboBoxCode.Text.Trim() != "")
            {
            if (textBoxDescri.Text.Trim() == "")
            {
                MessageBox.Show("ͼ����������Ϊ��", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
           
                GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
                string strExp = "";
                string mypath = dIndex.GetDbInfo();
                string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
                GeoDataCenterDbFun db = new GeoDataCenterDbFun();
                strExp = string.Format("insert into ��׼ͼ����Ϣ��(����,����,ҵ��������,ҵ��С�����) values('{0}','{1}','{2}','{3}')",
                    comboBoxCode.Text, textBoxDescri.Text, GetCode(comboBoxBig.Text), GetCode(comboBoxSub.Text));
                db.ExcuteSqlFromMdb(strCon, strExp);
            }

            this.DialogResult = DialogResult.OK;
            this.Hide();
            this.Dispose(true);
        }

        private void btnCancle_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Hide();
            this.Dispose(true);
        }
        /// <summary>
        /// ͨ������(����)�����ô���
        /// </summary>
        /// <param name="strname">����(����)</param>
        private string GetCode(string strname)
        {
            try
            {
                string[] arr = strname.Split('(', ')');
                return arr[1];
            }
            catch
            {
                return strname;
            }
        }

        private void comboBoxCode_TextChanged(object sender, EventArgs e)
        {
            if (comboBoxCode.Text.Trim() == "")
                btnApply.Enabled = false;
            else
                btnApply.Enabled = true;
        }
    }
}