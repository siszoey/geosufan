using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesGDB;
using System.IO;

namespace GeoDataCenterFunLib
{
    public partial class frmSelectDoc : DevComponents.DotNetBar.Office2007Form
    {
        public frmSelectDoc()
        {
            InitializeComponent();
        }
        public string value="";

        private void frmSelectDoc_Load(object sender, EventArgs e)
        {
            LoadComboBox();
             comboBoxSource.Text = value;
 
        }

        private void LoadComboBox()
        {
            GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
            string strExp = "select ����Ŀ¼�� from �ĵ�����Դ��Ϣ��";
            string mypath = dIndex.GetDbInfo();
            string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
            GeoDataCenterDbFun db = new GeoDataCenterDbFun();
            List<string> list = db.GetDataReaderFromMdb(strCon, strExp);
            for (int i = 0; i < list.Count; i++)
            {
                comboBoxSource.Items.Add(list[i]);//��������Դ�б��
            }
            if (list.Count > 0)
            {
                comboBoxSource.SelectedIndex = 0;//Ĭ��ѡ���һ��
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (comboBoxSource.Text.Trim() == ""&&comboBoxSource.Items.Count==0)
            {
                MessageBox.Show("Ŀ¼����Ϊ��", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            value = comboBoxSource.Text;
            this.DialogResult = DialogResult.OK;
            this.Hide();
            this.Dispose(true);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Hide();
            this.Dispose(true);
           
        }

        private void comboBoxSource_TextChanged(object sender, EventArgs e)
        {
            textBoxPath.Text = "";
            if (comboBoxSource.Text.Trim() != "")
            {
                GetDataTreeInitIndex dIndex = new GetDataTreeInitIndex();
                string strExp = "select ·�� from �ĵ�����Դ��Ϣ�� where ����Ŀ¼��='"+comboBoxSource.Text+"'";
                string mypath = dIndex.GetDbInfo();
                string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mypath + ";Mode=ReadWrite|Share Deny None;Persist Security Info=False";  //�����������ݿ��ַ���
                GeoDataCenterDbFun db = new GeoDataCenterDbFun();
                textBoxPath.Text=db.GetInfoFromMdbByExp(strCon, strExp);
            }
        }

    }
}