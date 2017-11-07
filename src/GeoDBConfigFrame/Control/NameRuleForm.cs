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
    public partial class NameRuleForm : DevComponents.DotNetBar.Office2007Form
    {
        public NameRuleForm()
        {
            InitializeComponent();
        }

        private string strfildname;//�õ��ֶ�����
        
        //ȡ��
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Hide();
            this.Dispose(true);
        }

        //����ֶ�
        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddField frm = new AddField();
            frm.flag = 1;
            frm.ShowDialog();
        }

        //�༭�ֶ�
        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count != 1)
            {
                MessageBox.Show("��ѡ��һ����б༭!", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            string mypath = dIndex.GetDbInfo();
            string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //������������
            GeoDataCenterDbFun db = new GeoDataCenterDbFun();
            AddField frm = new AddField();
            frm.flag = 2;
            frm.fleldname = listView.SelectedItems[0].Text.Trim();
            frm.ShowDialog();

        }

        //����
        private void btnUp_Click(object sender, EventArgs e)
        {

            if (listView.SelectedItems.Count != 1)
            {
                MessageBox.Show("��ѡ��һ������ƶ�!", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if(listView.SelectedItems[0].Index==0)
                return;
            listView.BeginUpdate();
            ListViewItem item=listView.SelectedItems[0];
            int index = listView.SelectedItems[0].Index;
            listView.Items.RemoveAt(index);
            listView.Items.Insert(index - 1, item);
            listView.EndUpdate();
            ChangeIndex();
        }

        //����
        private void btnDown_Click(object sender, EventArgs e)
        {

            if (listView.SelectedItems.Count != 1)
            {
                MessageBox.Show("��ѡ��һ������ƶ�!", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (listView.SelectedItems[0].Index ==listView.Items.Count-1)
                return;
            listView.BeginUpdate();
            ListViewItem item = listView.SelectedItems[0];
            int index = listView.SelectedItems[0].Index;
            listView.Items.RemoveAt(index);
            listView.Items.Insert(index +1, item);
            listView.EndUpdate();
            ChangeIndex();
        }

        //ɾ��
        private void btnDel_Click(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count != 1)
            {
                MessageBox.Show("��ѡ��һ�����ɾ��!", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            int index = listView.SelectedItems[0].Index;//ȡ��ѡ��������
            DialogResult result=MessageBox.Show("�Ƿ�ȷ��ɾ������?", "��ʾ",MessageBoxButtons.YesNo,MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
                string mypath = dIndex.GetDbInfo();
                string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //������������
                GeoDataCenterDbFun db = new GeoDataCenterDbFun();
                string name = listView.Items[index].Text;
                string strExp = "delete * from ͼ��������ʼ���� where �ֶ�����='" +name+"'";
                db.ExcuteSqlFromMdb(strCon, strExp);
                listView.Items.RemoveAt(index);
                MessageBox.Show("ɾ���ɹ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ChangeIndex()
        {
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            string mypath = dIndex.GetDbInfo();
            string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //������������
            GeoDataCenterDbFun db = new GeoDataCenterDbFun();
            string strExp = "";
            foreach(ListViewItem item in listView.Items)
            {
                int index = item.Index + 1;
                strExp = "update ͼ��������ʼ���� set ����='" +index+"' where �ֶ�����='" + item.Text + "'";
                db.ExcuteSqlFromMdb(strCon, strExp);
            }
        }
        private void NameRuleForm_Load(object sender, EventArgs e)
        {
            LoadListView();
        }
        private void LoadListView()
        {
           listView.Items.Clear();
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            string mypath = dIndex.GetDbInfo();
            string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //������������
            GeoDataCenterDbFun db = new GeoDataCenterDbFun();
          //  string strExp = "select * from ͼ��������ʼ���� order by ����";
            string strExp = "select �ֶ�����,����,�ֶ�����,�ֶγ���,ȱʡ,���� from ͼ��������ʼ���� order by ����";
            DataTable dt = db.GetDataTableFromMdb(strCon, strExp);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                listView.Items.Add(dt.Rows[i]["�ֶ�����"].ToString());
                listView.Items[i].SubItems.Add(dt.Rows[i]["����"].ToString());
                listView.Items[i].SubItems.Add(dt.Rows[i]["�ֶ�����"].ToString());
                listView.Items[i].SubItems.Add(dt.Rows[i]["�ֶγ���"].ToString());
                listView.Items[i].SubItems.Add(dt.Rows[i]["ȱʡ"].ToString());
            }

        }

        private void NameRuleForm_Activated(object sender, EventArgs e)
        {
            LoadListView();
        }

        private void listView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listView.SelectedItems.Count != 1)
            {
                MessageBox.Show("��ѡ��һ����б༭!", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            string mypath = dIndex.GetDbInfo();
            string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //������������
            GeoDataCenterDbFun db = new GeoDataCenterDbFun();
            AddField frm = new AddField();
            frm.flag = 2;
            frm.fleldname = listView.SelectedItems[0].Text.Trim();
            frm.ShowDialog();
        }

        //����������ʽ
        private void btnGet_Click(object sender, EventArgs e)
        {
            textBoxExp.Text = "";
            textBoxExample.Text = "";
            strfildname = "";
            string type;//�ֶ�����
           string Length;//�ֶγ���
            string strregex="";//���ʽ
            string strName = "";//�ֶ�����
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            string mypath = dIndex.GetDbInfo();
            string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //������������
            GeoDataCenterDbFun db = new GeoDataCenterDbFun();
            string strExp = "";
            foreach(ListViewItem item in listView.Items)
            {
                type=item.SubItems[2].Text;
                Length =item.SubItems[3].Text;
                strName = item.Text;
                switch (strName)
                {
                    case "ҵ��������":
                        strregex += "(^[A-Z]{"+Length+"}$)";
                        break;
                    case "���":
                        strregex += "(^20[0-9]{2}$)";
                        break;
                    case "ҵ��С�����":
                        strregex += "(^[0-9]{"+Length+"}$)";
                        break;
                    case "��������":
                        int ln=(Convert.ToInt32(Length))-1;
                        strregex += "(^[1-9][0-9]{"+ln+"}$)";
                        break;
                    case "������":
                        strregex += "(^[B-I]{"+Length+"}$)";
                        break;
                }
                strfildname += item.Text+"+";
                textBoxExample.Text += item.SubItems[4].Text;
                int index = item.Index + 1;
                strExp = "update ͼ��������ʼ���� set ����='" +  index+ "' where �ֶ�����='" + item.Text + "'";
                db.ExcuteSqlFromMdb(strCon,strExp);

            }
            textBoxExp.Text = strregex;
            strfildname = strfildname.Substring(0, strfildname.LastIndexOf("+"));
        }

        //�������ݿ�
        private void btnOk_Click(object sender, EventArgs e)
        {
            string Tambole="";
            strfildname = "";
            string type;//�ֶ�����
            string Length;//�ֶγ���
            string strregex = "";//���ʽ
            string strName = "";//�ֶ�����
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            string mypath = dIndex.GetDbInfo();
            string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //������������
            GeoDataCenterDbFun db = new GeoDataCenterDbFun();
            string strExp = "";
            foreach (ListViewItem item in listView.Items)
            {
                type = item.SubItems[2].Text;
                Length = item.SubItems[3].Text;
                strName = item.Text;
                switch (strName)
                {
                    case "ҵ��������":
                        strregex += "(^[A-Z]{" + Length + "}$)";
                        break;
                    case "���":
                        strregex += "(^20[0-9]{2}$)";
                        break;
                    case "ҵ��С�����":
                        strregex += "(^[0-9]{" + Length + "}$)";
                        break;
                    case "��������":
                        int ln = (Convert.ToInt32(Length)) - 1;
                        strregex += "(^[1-9][0-9]{" + ln + "}$)";
                        break;
                    case "������":
                        strregex += "(^[B-I]{" + Length + "}$)";
                        break;
                }
                strfildname += item.Text + "+";
                Tambole += item.SubItems[4].Text;
                int index = item.Index + 1;
                strExp = "update ͼ��������ʼ���� set ����='" + index + "' where �ֶ�����='" + item.Text + "'";
                db.ExcuteSqlFromMdb(strCon, strExp);

            }
            strfildname = strfildname.Substring(0, strfildname.LastIndexOf("+"));
                strExp = "delete * from  ͼ�����������";
                db.ExcuteSqlFromMdb(strCon, strExp);
                strExp = "insert into ͼ�����������(��������,ʾ��,�ֶ�����) values('" + strregex + "','" + Tambole + "','" + strfildname + "')";
                db.ExcuteSqlFromMdb(strCon, strExp);
                MessageBox.Show("���������Ѹ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
        }
    }
}