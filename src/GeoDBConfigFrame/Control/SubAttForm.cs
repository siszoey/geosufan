using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GeoDBConfigFrame
{
    public partial class SubAttForm : DevComponents.DotNetBar.Office2007Form
    {
        public string strSubCode;
        public string strSubName;
        public string strIndexFile;
        public string strMapSymIndexFile;
        public SubAttForm()
        {
            InitializeComponent();

        }

        public void SetFormTextBoxAtt()
        {
            textSubCode.Text = strSubCode;
            textSubName.Text = strSubName;
            if (strIndexFile.Trim() != "")
            {
                textIndexFile.Text = strIndexFile.Substring(0, strIndexFile.LastIndexOf('.'));
            }
            if (strMapSymIndexFile.Trim() != "")
            {
                textBoxMapIndexFile.Text = strMapSymIndexFile;
            }
        }

        public void GetFormTextBoxAtt()
        {
            strSubCode = textSubCode.Text.Trim();
            strSubName = textSubName.Text.Trim();
            strIndexFile = textIndexFile.Text.Trim() + ".xml";
            strMapSymIndexFile = textBoxMapIndexFile.Text.Trim();
        }
        private void btnOk_Click(object sender, EventArgs e)
        {
            //ר����� 
            if (textSubCode.Text.Trim().Equals(""))
            {
                MessageBox.Show("ר�����Ͳ���Ϊ��ֵ!","��ʾ");
                return;
            }
            //ר������ ����Ϊ��
            if (textSubName.Text.Trim().Equals(""))
            {
                MessageBox.Show("ר����������Ϊ��ֵ!", "��ʾ");
                return;
            }
            GetFormTextBoxAtt();
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

        //ר�����ͱ仯
        private void textSubCode_TextChanged(object sender, EventArgs e)
        {
           // textBoxMapIndexFile.Text = textSubCode.Text;//��ͼ�ļ�
            textIndexFile.Text = textSubCode.Text;//�ű��ļ�
        }

        //�򿪶Ի���ȡMXD���ļ�
        private void btnServer_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "��ͼ�ļ�(*.mxd)|*.mxd";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                textBoxMapIndexFile.Text = dlg.FileName;
            }
        }
    }
}