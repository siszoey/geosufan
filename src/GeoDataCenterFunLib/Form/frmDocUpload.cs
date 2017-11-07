using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

//*********************************************************************************
//** �ļ�����frmDocUpload.cs
//** CopyRight (c) �人������Ϣ���̼������޹�˾�����������
//** �����ˣ�ϯʤ
//** ��  �ڣ�20011-03-15
//** �޸��ˣ�
//** ��  �ڣ�
//** ��  ����
//**
//** ��  ����1.0
//*********************************************************************************
namespace GeoDataCenterFunLib
{
    public partial class frmDocUpload : DevComponents.DotNetBar.Office2007Form
    {
        public frmDocUpload()
        {
            InitializeComponent();
        }
        int i = 0;
        string m_soucedir="";
        public static TreeNode Node;//���ݵ�Ԫ�����صĽڵ�
        
        private void frmDocUpload_Load(object sender, EventArgs e)
        {
             string strExp = "";
            comboBoxOpen.SelectedIndex = 0;
            //������������
            strExp = "select ����,ר������ from ��׼ר����Ϣ��";
            LoadData2(comboBoxType, strExp);
            //���ر�����
            strExp = "select ����,���� from �����ߴ����";
            LoadData2(comboBoxScale, strExp);
            //�������Ŀ¼
            strExp="select ����Ŀ¼�� from �ĵ�����Դ��Ϣ��";
            LoadData(comboBoxSource, strExp);
            //�������
            strExp = "select ��� from ���ݱ����";
            LoadData(comboBoxYear, strExp);
        }

        /// <summary>
        /// �����б����ȣ�����Դ
        /// </summary>
        /// <param name="cb">�б��</param>
        /// <param name="str">��Ҫִ�е�SQL���</param>
        private void LoadData(ComboBox cb, string str)
        {
            List<string> list = new List<string>();
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            string mypath = dIndex.GetDbInfo();
            string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //������������
            
            GeoDataCenterDbFun db = new GeoDataCenterDbFun();
            list = db.GetDataReaderFromMdb(strCon, str);
            for (int i = 0; i < list.Count; i++)
            {
                if(!cb.Items.Contains(list[i]))
                cb.Items.Add(list[i]);
            }
            if (cb.Items.Count != 0)
            {
                cb.SelectedIndex = 0;
            }
        }
        /// <summary>
        /// �����б��,������ǰ�������ں�
        /// </summary>
        /// <param name="cb">�б��</param>
        /// <param name="str">��Ҫִ�е�SQL���</param>
        private void LoadData2(ComboBox cb, string str)
        {
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            string mypath = dIndex.GetDbInfo();
            string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //������������
            GeoDataCenterDbFun db = new GeoDataCenterDbFun();
            DataTable dt = db.GetDataTableFromMdb(strCon, str);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                cb.Items.Add(dt.Rows[i][0] + "(" + dt.Rows[i][1] + ")");
            }
            if (cb.Items.Count != 0)
            {
                cb.SelectedIndex = 0;
            }
 
        }
        private void btnServer_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dg = new FolderBrowserDialog();
            if (dg.ShowDialog() == DialogResult.OK)
            {
                comboBoxSource.Text = dg.SelectedPath;
                m_soucedir = dg.SelectedPath;
            }
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            if (m_soucedir==comboBoxSource.Text&&!Directory.Exists(@comboBoxSource.Text))
            {
                MessageBox.Show("����Դ·��������!", "��ʾ",MessageBoxButtons.OK,MessageBoxIcon.Information);
                return;
            }

            OpenFileDialog OpenFile = new OpenFileDialog();
            OpenFile.Filter = "Word�ĵ�|*.doc|PDF�ĵ�|*.pdf";
            OpenFile.Multiselect = true;
            if (OpenFile.ShowDialog() == DialogResult.OK)
            {

                foreach (string file in OpenFile.FileNames)
                {
                    for (int j = 0; j < i; j++)
                    {
                        string strExist = listViewDoc.Items[j].Text.Trim();
                       
                        if (strExist.CompareTo(file) == 0)
                        {
                            MessageBox.Show("�ļ��Ѵ������б���", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.Cursor = Cursors.Default;
                            return;
                        }

                    }
                    listViewDoc.Items.Add(file);
                    listViewDoc.Items[i].SubItems.Add("�ȴ����");
                    listViewDoc.Items[i].Checked = true;
                    i++;
                }
            }

        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listViewDoc.Items)
            {
                if (item.Checked)
                {
                    listViewDoc.Items.Remove(item);
                    i--;
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            listViewDoc.Items.Clear();
            i = 0;
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
        
        private void btnOK_Click(object sender, EventArgs e)
        {
            string physicdir="";
            string strExp;
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            string mypath = dIndex.GetDbInfo();
            string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //������������
            GeoDataCenterDbFun db = new GeoDataCenterDbFun();
            if (comboBoxScale.Text == "" || comboBoxArea.Text == "" || comboBoxYear.Text == "" || comboBoxType.Text == "")
            {
                MessageBox.Show("��ѡ���Ϊ��!", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string strscale = GetCode(comboBoxScale.Text); //��ñ����ߴ���
            string strType = GetCode(comboBoxType.Text);//���ר������
            string strArea = GetCode(comboBoxArea.Text);//����������
            string strAreaname = comboBoxArea.Text.Substring(0, comboBoxArea.Text.LastIndexOf('('));//�����������

            //��� ר�� ���������� Ҫ���չ����滻
            string dir = comboBoxYear.Text +strType +strArea;
            if (m_soucedir != comboBoxSource.Text)
            {
                strExp = "select ·�� from �ĵ�����Դ��Ϣ�� where ����Ŀ¼��='" + comboBoxSource.Text + "'";
                physicdir = db.GetInfoFromMdbByExp(strCon, strExp) + @"\" + dir;//·��
            }
            else
            {
                physicdir = comboBoxSource.Text + @"\" + dir;//·��
            }
            if (!Directory.Exists(physicdir))
            {
                Directory.CreateDirectory(physicdir);
            }
            foreach (ListViewItem item in listViewDoc.Items)
            {
                try
                {
                    //string[] str = item.Text.Split('.');
                    string strfile = item.Text.Substring(item.Text.LastIndexOf("\\") + 1);
                    string[] strBuffer = strfile.Split('.');
                    string strFileName = strBuffer[0].ToString();
                    string strFileType = strBuffer[1].ToString();

                    if (item.Checked && item.SubItems[1].Text == "�ȴ����")
                    {
                        item.SubItems[1].Text = "�������";
                        listViewDoc.Refresh();
                        if (File.Exists(item.Text))
                        {
                            File.Copy(item.Text, physicdir + "\\" + strfile, true);
                        }
                        strExp = string.Format("insert into �ĵ������Ϣ�� (��������,��������,���,������,ר������,�ĵ�����,�ĵ�����,�ĵ�����Ŀ¼,����) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}')",
                            strArea, strAreaname, comboBoxYear.Text, strscale, strType, strFileName, strFileType, comboBoxSource.Text, comboBoxOpen.SelectedIndex.ToString());
                        db.ExcuteSqlFromMdb(strCon, strExp);
                        item.SubItems[1].Text = "������";
                    }
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    item.SubItems[1].Text = "���ʧ��";
                }

            }
            MessageBox.Show("���������", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void comboBoxArea_Click(object sender, EventArgs e)
        {
            
            GeoDataCenterDbFun dbfun = new GeoDataCenterDbFun();
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            string mypath = dIndex.GetDbInfo();
            string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //������������

            frmDataUnitTree frm = new frmDataUnitTree();//��ʼ�����ݵ�Ԫ������
            frm.Location = new Point(this.Location.X + 58, this.Location.Y +160);
            frm.flag = 3;
            frm.ShowDialog();
            if (Node != null)//���ص�Node����NULL
            {
                if (Convert.ToInt32(Node.Tag) != 0)
                {
                    string strExp = "select �������� from ���ݵ�Ԫ�� where ��������='" + Node.Text + "'  and ���ݵ�Ԫ����='" + Node.Tag + "'";
                    string code = dbfun.GetInfoFromMdbByExp(strCon, strExp);
                    comboBoxArea.Text = Node.Text + "(" + code + ")";//Ϊ���ݵ�Ԫbox��ʾ����
                }
            }
        }

        //ȫѡ��ť
        private void btnAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listViewDoc.Items.Count; i++)
            {
                listViewDoc.Items[i].Checked = true;
            }

        }
        //��ѡ��ť
        private void btnInverse_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listViewDoc.Items.Count; i++)
            {
                if (listViewDoc.Items[i].Checked == false)
                {
                    listViewDoc.Items[i].Checked = true;
                    //datagwSource.Rows[i].Selected = true;
                }
                else
                {
                    listViewDoc.Items[i].Checked = false;
                    //datagwSource.Rows[i].Selected = false;
                }
            }
        }

        private void listViewDoc_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
       
    }
}