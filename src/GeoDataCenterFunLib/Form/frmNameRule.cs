using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

//*********************************************************************************
//** �ļ�����frmDataUpload.cs
//** CopyRight (c) �人������Ϣ���̼������޹�˾�����������
//** �����ˣ�ϯʤ
//** ��  �ڣ�20011-03-25
//** �޸��ˣ�
//** ��  �ڣ�
//** ��  ����
//**
//** ��  ����1.0
//*********************************************************************************
namespace GeoDataCenterFunLib
{
    public partial class frmNameRule :DevComponents.DotNetBar.Office2007Form
    {

        private TextBox tb;
        public frmNameRule(TextBox textbox)
        {
            InitializeComponent();
            tb = textbox;
        }

        private string prename;//��ȡ������ǰ׺�ı�������

        string[] array = new string[6];

        private void frmNameRule_Load(object sender, EventArgs e)
        {
            prename=tb.Text;
            LoadCombox();
            if (prename.Length > 14)
            {
                //д���ݱ����
                AnalyseDataToArray(prename);
                ChangeComboxText();

            }
        }
        /// <summary>
        /// ����textbox�����б��text
        /// </summary>
        private void ChangeComboxText()
        {
            //����ҵ��������
            string strExp = "select ����,���� from ҵ��������� where ����='" + array[0] + "'";
            LoadComboxText(comboBoxBus, strExp);
            //������������
            strExp = "select ��������,�������� from ���ݵ�Ԫ�� where ��������='"+array[3]+"'";
            LoadComboxText(comboBoxArea, strExp);
            //����ҵ��С�����
            strExp = "select ����,ҵ��С����� from ҵ��С������ where ҵ��С�����='"+array[2]+"'and ҵ��������='"+array[0]+"'";
            LoadComboxText(comboBoxType, strExp);
            //���ر�����
            strExp = "select ����,���� from �����ߴ���� where ����='"+array[4]+"'";
            LoadComboxText(comboBoxScale, strExp);
             //�������
            comboBoxYear.Text = array[1];
        }
        /// <summary>
        ///  �����б��text
        /// </summary>
        /// <param name="cb">�б��</param>
        /// <param name="str">��Ҫִ�е�SQL���</param>
        private void LoadComboxText(ComboBox cb, string str)
        {
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            string mypath = dIndex.GetDbInfo();
            string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //������������
            GeoDataCenterDbFun db = new GeoDataCenterDbFun();
            DataTable dt = db.GetDataTableFromMdb(strCon, str);
            cb.Text=dt.Rows[0][0] + "(" + dt.Rows[0][1] + ")";
        }

        /// <summary>
        /// �����б��
        /// </summary>
        private void LoadCombox()
        {  
            //����ҵ��������
            string strExp = "select ����,���� from ҵ���������";
            LoadData2(comboBoxBus, strExp);
            //������������
            strExp = "select ��������,�������� from ���ݵ�Ԫ�� order by ��������";
            LoadData2(comboBoxArea, strExp);
            //����ҵ��С�����
            //comboBoxType.Items.Add
            //���ر�����
            strExp = "select ����,���� from �����ߴ����";
            LoadData2(comboBoxScale, strExp);
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
                if (!cb.Items.Contains(list[i]))
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
                MessageBox.Show("���������룩��ʽ����ȷ!", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return strname;
            }
        }


        /// <summary>
        /// �����ݷ������ַ�������
        /// </summary>
        /// <param name="filename">��������</param>
        public void AnalyseDataToArray(string filename)
        {
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            string mypath = dIndex.GetDbInfo();
            string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
            string strExp = "select �ֶ����� from ͼ�����������";
            GeoDataCenterDbFun db = new GeoDataCenterDbFun();
            string strname = db.GetInfoFromMdbByExp(strCon, strExp);
            string[] arrName = strname.Split('+');//�����ֶ�����
            for (int i = 0; i < arrName.Length; i++)
            {
                switch (arrName[i])
                {
                    case "ҵ��������":
                        array[0] = filename.Substring(0, 2);//ҵ��������
                        filename = filename.Remove(0, 2);
                        break;
                    case "���":
                        array[1] = filename.Substring(0, 4);//���
                        filename = filename.Remove(0, 4);
                        break;
                    case "ҵ��С�����":
                        array[2] = filename.Substring(0, 2);//ҵ��С�����
                        filename = filename.Remove(0, 2);
                        break;
                    case "��������":
                        array[3] = filename.Substring(0, 6);//��������
                        filename = filename.Remove(0, 6);
                        break;
                    case "������":
                        array[4] = filename.Substring(0, 1);//������
                        filename = filename.Remove(0, 1);
                        break;
                }
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            prename = "";//���ԭ������
            array[0] = GetCode(comboBoxBus.Text);//ҵ����������DZ
            array[1] = comboBoxYear.Text;//�����2009
            array[2] = GetCode(comboBoxType.Text);//ר�������01
            array[3] = GetCode(comboBoxArea.Text);//������420683
            array[4] = GetCode(comboBoxScale.Text);//��������G
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            string mypath = dIndex.GetDbInfo();
            string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //������������
            GeoDataCenterDbFun db = new GeoDataCenterDbFun();
            string strExp = "select �������� from ͼ�����������";
            string strRegex = db.GetInfoFromMdbByExp(strCon, strExp);//������ʽ
            strExp="select �ֶ����� from ͼ�����������";
            string strname=db.GetInfoFromMdbByExp(strCon,strExp);
            try
            {
                string[] arrRegex = strRegex.Split('(', ')');//����������ʽ
                string[] arrName = strname.Split('+');//�����ֶ�����

                Regex regex;

                for (int i = 0; i < arrName.Length; i++)
                {
                    regex = new Regex(arrRegex[2 * i + 1]);
                    switch (arrName[i])
                    {
                        case "ҵ��������":
                            if (!regex.IsMatch(array[0]))//ƥ��ҵ��������
                            {
                                MessageBox.Show("ҵ�������벻������������!", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return;
                            }
                            else prename += array[0];
                            break;
                        case "���":
                            if (!regex.IsMatch(array[1]))//ƥ�����
                            {
                                MessageBox.Show("��Ȳ�������������!", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return;
                            }
                            else prename += array[1];
                            break;
                        case "ҵ��С�����":
                            if (!regex.IsMatch(array[2]))//ƥ��ҵ��С�����
                            {
                                MessageBox.Show("ҵ��С����벻������������!", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return;
                            }
                            else prename += array[2];
                            break;
                        case "��������":
                            if (!regex.IsMatch(array[3]))//ƥ����������
                            {
                                MessageBox.Show("�������벻������������!", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return;
                            }
                            else prename += array[3];
                            break;
                        case "������":
                            if (!regex.IsMatch(array[4]))//ƥ�������
                            {
                                MessageBox.Show("�����߲�������������!", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return;
                            }
                            else prename += array[4];
                            break;
                    }

                }
            }
            catch
            {
                MessageBox.Show("û����������!", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            //����ǰ׺
            tb.Text = prename;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void comboBoxBus_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBoxType.Items.Clear();
           string  strExp = "select ����,ҵ��С����� from ҵ��С������ where ҵ��������='" + GetCode(comboBoxBus.Text) +"'";
            LoadData2(comboBoxType, strExp);
        }
    }
}