using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

//*********************************************************************************
//** �ļ�����frmDocRedution.cs
//** CopyRight (c) �人������Ϣ���̼������޹�˾�����������
//** �����ˣ�ϯʤ
//** ��  �ڣ�20011-03-16
//** �޸��ˣ�
//** ��  �ڣ�
//** ��  ����
//**
//** ��  ����1.0
//*********************************************************************************
namespace GeoDataCenterFunLib
{
    public partial class frmDocRedution : DevComponents.DotNetBar.Office2007Form
    {
        public frmDocRedution()
        {
            InitializeComponent();
        }

        bool m_first;//�Ƿ��һ�μ����б��
        int[] m_state ={ 0, 0, 0, 0,0};//4��ѡ���б��ѡ��״̬
        public static TreeNode Node;//���ݵ�Ԫ�����صĽڵ�

        private void frmDocRedution_Load(object sender, EventArgs e)
        {
            Node = null;
            //��ʼ��������
            SysCommon.CProgress vProgress = new SysCommon.CProgress("���ڼ����ĵ�����");
            vProgress.EnableCancel = false;
            vProgress.ShowDescription = true;
            vProgress.FakeProgress = true;
            vProgress.TopMost = true;
            vProgress.ShowProgress();
            m_first = true;

            LoadGridView();
            //��������Ŀ¼
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            string mypath = dIndex.GetDbInfo();
            string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
            string strExp = "select ����Ŀ¼�� from �ĵ�����Դ��Ϣ��";
            GeoDataCenterDbFun db = new GeoDataCenterDbFun();
            List<string> list = db.GetDataReaderFromMdb(strCon, strExp);
            for (int i = 0; i < list.Count; i++)
            {
                if (!comboBoxCatalog.Items.Contains(list[i]))
                    comboBoxCatalog.Items.Add(list[i]);
            }
            if (comboBoxCatalog.Items.Count > 0)
                comboBoxCatalog.SelectedIndex = 0;
          
            vProgress.Close();
            this.Activate();
        }
        private void LoadGridView()
        {
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            string mypath = dIndex.GetDbInfo();
            string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
            string strExp = "select * from �ĵ������Ϣ��";
            GeoDataCenterDbFun db = new GeoDataCenterDbFun();
            DataTable dt = db.GetDataTableFromMdb(strCon, strExp);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                datagwSource.Rows.Add(new object[] { true, dt.Rows[i]["�ĵ�����"] });
            }
        }
        //���ѡ��״̬
        private void comboBoxYear_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (!m_first)
            {
                m_state[0] = comboBoxYear.SelectedIndex;
                ChangeGridView();
            }

        }
        //������ѡ��״̬
        private void comboBoxArea_TextChanged(object sender, EventArgs e)
        {
              ChangeGridView();
        }

        //������ѡ��״̬
        private void comboBoxScale_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_first)
            {
                m_state[2] = comboBoxScale.SelectedIndex;
                ChangeGridView();
            }
        }

        //ר������
        private void comboBoxSub_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (!m_first)
            {
                m_state[4] = comboBoxSub.SelectedIndex;
                ChangeGridView();
            }
        }

        //Ŀ¼ѡ��״̬
        private void comboBoxCatalog_SelectedIndexChanged(object sender, EventArgs e)
        {

                ChangeGridView();
        }

       
        //��һ�μ��ش���ʱ���������б��
        private void comboBoxYear_Click(object sender, EventArgs e)
        {
            if (m_first)
            {
                LoadCombox();
                m_first = false;
            }

        }

        //�����б��
        private void LoadCombox()
        {
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            string mypath = dIndex.GetDbInfo();
            string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
            string str_Exp = "select ��� from ���ݱ����";
            GeoDataCenterDbFun dbfun = new GeoDataCenterDbFun();
            List<string> list = dbfun.GetDataReaderFromMdb(strCon, str_Exp);
            comboBoxYear.Items.Add("�������");
            for (int i = 0; i < list.Count; i++)
            {
                if (!comboBoxYear.Items.Contains(list[i]))
                    comboBoxYear.Items.Add(list[i]);

            }
            if (comboBoxYear.Items.Count > 0)
                comboBoxYear.SelectedIndex = 0;
            //str_Exp = "select ��������,�������� from ���ݵ�Ԫ��";
            //DataTable dt = dbfun.GetDataTableFromMdb(strCon, str_Exp);
            //comboBoxArea.Items.Add("����������");
            //for (int i = 0; i < dt.Rows.Count; i++)
            //{
            //    comboBoxArea.Items.Add(dt.Rows[i]["��������"] + "(" + dt.Rows[i]["��������"] + ")");
            //}
            //if (comboBoxArea.Items.Count > 0)
            //    comboBoxArea.SelectedIndex = 0;
            str_Exp = "select ����,���� from �����ߴ����";
            DataTable dt = dbfun.GetDataTableFromMdb(strCon, str_Exp);
            comboBoxScale.Items.Add("���б�����");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                comboBoxScale.Items.Add(dt.Rows[i]["����"] + "(" + dt.Rows[i]["����"] + ")");
            }
            if (comboBoxScale.Items.Count > 0)
                comboBoxScale.SelectedIndex = 0;
            str_Exp = "select ����,ר������ from ��׼ר����Ϣ��";
            dt = dbfun.GetDataTableFromMdb(strCon, str_Exp);
            comboBoxSub.Items.Add("����ר������");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                comboBoxSub.Items.Add(dt.Rows[i]["����"] + "(" + dt.Rows[i]["ר������"] + ")");
            }
            if (comboBoxSub.Items.Count > 0)
                comboBoxSub.SelectedIndex = 0;

        }
        //ȫѡ��ť
        private void btnAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < datagwSource.Rows.Count; i++)
            {
                this.datagwSource.Rows[i].Cells[0].Value = true;
            }

        }
        //��ѡ��ť
        private void btnInverse_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < datagwSource.Rows.Count; i++)
            {
                if ((bool)datagwSource.Rows[i].Cells[0].EditedFormattedValue == false)
                {
                    this.datagwSource.Rows[i].Cells[0].Value = true;
                    //datagwSource.Rows[i].Selected = true;
                }
                else
                {
                    this.datagwSource.Rows[i].Cells[0].Value = false;
                    //datagwSource.Rows[i].Selected = false;
                }
            }
        }

        //�����ĵ�
        private void btn_Export_Click(object sender, EventArgs e)
        {
            bool flag = false;
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            string mypath = dIndex.GetDbInfo();
            string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
            GeoDataCenterDbFun db = new GeoDataCenterDbFun(); 
            string strExp="select ·�� from �ĵ�����Դ��Ϣ�� where ����Ŀ¼��='"+comboBoxCatalog.Text+"'";
            string path=db.GetInfoFromMdbByExp(strCon,strExp);
            foreach (DataGridViewRow row in datagwSource.Rows)
            {
                if ((bool)row.Cells[0].EditedFormattedValue == true)
                {
                    flag = true;
                }
            }
            if (!flag)
            {
                MessageBox.Show("û��ѡ����!", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            SysCommon.CProgress vProgress = new SysCommon.CProgress("��������ѡ���ĵ�");
            try
            {
                FolderBrowserDialog dlg =new FolderBrowserDialog();
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    //��ʼ��������
                   
                    vProgress.EnableCancel = false;
                    vProgress.ShowDescription = true;
                    vProgress.FakeProgress = true;
                    vProgress.TopMost = true;
                    vProgress.ShowProgress();

                    string cellvalue = "";
                    foreach (DataGridViewRow row in datagwSource.Rows)
                    {
                        if ((bool)row.Cells[0].EditedFormattedValue == true)
                        {
                            cellvalue = row.Cells[1].Value.ToString().Trim();
                            strExp = "select * from �ĵ������Ϣ�� where �ĵ�����='"+cellvalue+"' and �ĵ�����Ŀ¼='"+comboBoxCatalog.Text+"'";
                            DataTable dt= db.GetDataTableFromMdb(strCon, strExp);
                            string dir = path + "\\" + dt.Rows[0]["���"] + dt.Rows[0]["ר������"] + dt.Rows[0]["��������"] + "\\" + cellvalue + "." + dt.Rows[0]["�ĵ�����"];
                            string filepath=dlg.SelectedPath+ "\\" + cellvalue + "." + dt.Rows[0]["�ĵ�����"];
                            if (File.Exists(dir))
                            {
                                File.Copy(dir, filepath, true);
                            }
                        }
                    }
                    vProgress.Close();
                    MessageBox.Show("���سɹ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Activate();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                vProgress.Close();
                this.Activate();
            }
        }

        //ɾ���ĵ�
        private void btn_Del_Click(object sender, EventArgs e)
        {
            
            bool flag = false;
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            string mypath = dIndex.GetDbInfo();
            string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
            GeoDataCenterDbFun db = new GeoDataCenterDbFun(); 
            string strExp="select ·�� from �ĵ�����Դ��Ϣ�� where ����Ŀ¼��='"+comboBoxCatalog.Text+"'";
            string path=db.GetInfoFromMdbByExp(strCon,strExp);
            foreach (DataGridViewRow row in datagwSource.Rows)
            {
                if ((bool)row.Cells[0].EditedFormattedValue == true)
                {
                    flag = true;
                }
            }
            if (!flag)
            {
                MessageBox.Show("û��ѡ����!", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            try
            {
                string cellvalue = "";
                foreach (DataGridViewRow row in datagwSource.Rows)
                {
                    if ((bool)row.Cells[0].EditedFormattedValue == true)
                    {
                        cellvalue = row.Cells[1].Value.ToString().Trim();
                        strExp = "select * from �ĵ������Ϣ�� where �ĵ�����='" + cellvalue + "' and �ĵ�����Ŀ¼='" + comboBoxCatalog.Text + "'";
                        DataTable dt = db.GetDataTableFromMdb(strCon, strExp);
                        string dir = path + "\\" + dt.Rows[0]["���"] + dt.Rows[0]["ר������"] + dt.Rows[0]["��������"] + "\\" + cellvalue + "." + dt.Rows[0]["�ĵ�����"];
                        if (File.Exists(dir))
                        {
                            File.Delete(dir);
                            dir = dir.Substring(0,dir.LastIndexOf("\\"));
                            if (Directory.GetFiles(dir).Length == 0)//�����Ŀ¼��û�������ĵ�����ɾ����Ŀ¼
                                Directory.Delete(dir);
                            strExp = "delete * from �ĵ������Ϣ�� where �ĵ�����='" + cellvalue + "' and �ĵ�����Ŀ¼='" + comboBoxCatalog.Text + "'";
                            db.ExcuteSqlFromMdb(strCon, strExp);
                        }
                    }
                }
                datagwSource.Rows.Clear();
                LoadGridView();//���¼�������
                MessageBox.Show("ɾ���ɹ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //�б��״̬�ı�
        private void ChangeGridView()
        {
            datagwSource.Rows.Clear();
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            string mypath = dIndex.GetDbInfo();
            string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
            string str_Exp = "select �ĵ����� from �ĵ������Ϣ�� where �ĵ�����Ŀ¼='"+comboBoxCatalog.Text+"' ";
            GeoDataCenterDbFun db = new GeoDataCenterDbFun();
            if (Node != null && Convert.ToInt32(Node.Tag) != 0)//���ѡ��Ĳ�������������
            {
                switch (Convert.ToInt32(Node.Tag))
                {
                    case 1:
                        string code = GetCode(comboBoxArea.Text).Substring(0, 3);
                        str_Exp += "and �������� like '" + code + "___'";
                        break;
                    case 2:
                        code = GetCode(comboBoxArea.Text).Substring(0, 4);
                        str_Exp += "and �������� like '" +code + "__'";
                        break;
                    case 3:
                        str_Exp += "and ��������='" + GetCode(comboBoxArea.Text) + "'";
                        break;
                }
                
                
            }
            if (comboBoxScale.SelectedIndex > 0)//���ѡ��Ĳ������б�����
                str_Exp += "and ������ ='" + GetCode(comboBoxScale.Text) + "'";
            if (comboBoxSub.SelectedIndex > 0)//���ѡ��Ĳ�������ר������
                str_Exp += "and ר������='" + GetCode(comboBoxSub.Text) + "'";
            if (comboBoxYear.SelectedIndex > 0)//���ѡ��Ĳ����������
                str_Exp += "and ���='" + comboBoxYear.Text + "'";

            List<string> list = db.GetDataReaderFromMdb(strCon, str_Exp);
            for (int i = 0; i < list.Count; i++)
            {
                datagwSource.Rows.Add(new object[] { true, list[i] });
            }
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

        private void comboBoxArea_Click(object sender, EventArgs e)
        {
            GeoDataCenterDbFun dbfun = new GeoDataCenterDbFun();
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            string mypath = dIndex.GetDbInfo();
            string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
            frmDataUnitTree frm = new frmDataUnitTree();//��ʼ�����ݵ�Ԫ������
            frm.Location = new Point(this.Location.X + 45, this.Location.Y +140);
            frm.flag = 2;
            frm.ShowDialog();
            if (Node != null)//���ص�Node����NULL
            {
                if (Convert.ToInt32(Node.Tag) != 0)
                {

                    string strExp = "select �������� from ���ݵ�Ԫ�� where ��������='" + Node.Text + "'  and ���ݵ�Ԫ����='" + Node.Tag + "'";
                    string code = dbfun.GetInfoFromMdbByExp(strCon, strExp);
                    comboBoxArea.Text = Node.Text + "(" + code + ")";//Ϊ���ݵ�Ԫbox��ʾ����
                }
                else
                {
                    comboBoxArea.Text = Node.Text;//Ϊ���ݵ�Ԫbox��ʾ����
                }


            }
            
        }
    }
}